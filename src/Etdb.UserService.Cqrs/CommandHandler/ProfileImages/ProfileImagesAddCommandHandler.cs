using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Etdb.ServiceBase.Exceptions;
using Etdb.UserService.Cqrs.Abstractions.Commands.ProfileImages;
using Etdb.UserService.Cqrs.Misc;
using Etdb.UserService.Domain.ValueObjects;
using Etdb.UserService.Presentation.Users;
using Etdb.UserService.Services.Abstractions;
using Etdb.UserService.Services.Abstractions.Models;
using MediatR;

namespace Etdb.UserService.Cqrs.CommandHandler.ProfileImages
{
    public class
        ProfileImagesAddCommandHandler : IRequestHandler<ProfileImagesAddCommand,
            IEnumerable<ProfileImageMetaInfoDto>>
    {
        private readonly IUsersService usersService;
        private readonly IResourceLockingAdapter resourceLockingAdapter;
        private readonly IProfileImageUrlFactory profileImageUrlFactory;

        public ProfileImagesAddCommandHandler(IUsersService usersService,
            IResourceLockingAdapter resourceLockingAdapter, IProfileImageUrlFactory profileImageUrlFactory)
        {
            this.usersService = usersService;
            this.resourceLockingAdapter = resourceLockingAdapter;
            this.profileImageUrlFactory = profileImageUrlFactory;
        }

        public async Task<IEnumerable<ProfileImageMetaInfoDto>> Handle(ProfileImagesAddCommand command,
            CancellationToken cancellationToken)
        {
            var user = await this.usersService.FindByIdAsync(command.UserId);

            if (user == null) throw new ResourceNotFoundException("The requested user could not be found");

            if (!await this.resourceLockingAdapter.LockAsync(user.Id, TimeSpan.FromSeconds(30)))
                throw WellknownExceptions.UserResourceLockException(user.Id);

            var profileImagePairs = command.UploadImageMetaInfos.Select((imageMetaInfo, index) =>
                {
                    var profileImage = ProfileImage.Create(Guid.NewGuid(),
                        user.Id,
                        imageMetaInfo.Name,
                        imageMetaInfo.ContentType.MediaType,
                        !user.ProfileImages.Any() && index == 0);

                    var storeImageMetaInfo = new StorableImage(user.Id, profileImage, imageMetaInfo.Image);

                    return (profileImage, storeImageMetaInfo);
                })
                .ToArray();


            await this.usersService.EditAsync(user,
                profileImagePairs.Select(pair => pair.storeImageMetaInfo).ToArray());

            await this.resourceLockingAdapter.UnlockAsync(user.Id);

            return profileImagePairs
                .Select(pair => pair.profileImage)
                .Select(source => new ProfileImageMetaInfoDto(source.Id,
                    this.profileImageUrlFactory.GenerateUrl(source, user.Id),
                    this.profileImageUrlFactory.GetResizeUrl(source, user.Id),
                    this.profileImageUrlFactory.GetDeleteUrl(source, user.Id),
                    source.IsPrimary, source.CreatedAt));
        }
    }
}