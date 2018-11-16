using AutoMapper;
using Etdb.UserService.AutoMapper.Resolver;
using Etdb.UserService.AutoMapper.TypeConverters;
using Etdb.UserService.Cqrs.Abstractions.Commands;
using Etdb.UserService.Domain.Entities;
using Etdb.UserService.Domain.Enums;
using Etdb.UserService.Presentation;

namespace Etdb.UserService.AutoMapper.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            this.CreateMap<User, UserDto>()
                .ForMember(destination => destination.IsExternalUserLogin,
                    options => options.MapFrom(src => src.SignInProvider != SignInProvider.UsernamePassword))
                .ForMember(destination => destination.ProfileImageUrl,
                    options => options.MapFrom<UserProfileImageUrlResolver>());

            this.CreateMap<UserRegisterDto, UserRegisterCommand>()
                .ConvertUsing<UserRegisterCommandTypeConverter>();

            this.CreateMap<Email, EmailDto>();
        }
    }
}