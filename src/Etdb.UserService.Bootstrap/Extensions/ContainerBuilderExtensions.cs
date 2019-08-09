﻿using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.FluentBuilder;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using Etdb.ServiceBase.Cqrs.Abstractions.Bus;
using Etdb.ServiceBase.Cqrs.Abstractions.Validation;
using Etdb.ServiceBase.Cqrs.Bus;
using Etdb.ServiceBase.Cryptography.Abstractions.Hashing;
using Etdb.ServiceBase.Cryptography.Hashing;
using Etdb.ServiceBase.DocumentRepository.Abstractions;
using Etdb.ServiceBase.Services;
using Etdb.ServiceBase.Services.Abstractions;
using Etdb.UserService.Authentication.Abstractions.Strategies;
using Etdb.UserService.Authentication.Strategies;
using Etdb.UserService.AutoMapper.Profiles;
using Etdb.UserService.Bootstrap.AutofacModules;
using Etdb.UserService.Cqrs.CommandHandler.Users;
using Etdb.UserService.Domain.Enums;
using Etdb.UserService.Repositories;
using Etdb.UserService.Services;
using Etdb.UserService.Services.Abstractions;
using Etdb.UserService.Worker;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace Etdb.UserService.Bootstrap.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static void SetupDependencies(this ContainerBuilder containerBuilder,
            IWebHostEnvironment hostingEnvironment)
        {
            var builder = new AutofacFluentBuilder(containerBuilder
                    .AddMediatR(typeof(UserRegisterCommandHandler).Assembly)
                    .AddAutoMapper(typeof(UsersProfile).Assembly))
                .ApplyModule(new DocumentDbContextModule(hostingEnvironment))
                .ApplyModule(new ResourceCachingModule(hostingEnvironment))
                .RegisterResolver(ExternalAuthenticationStrategyResolver)
                .RegisterTypeAsSingleton<ActionContextAccessor, IActionContextAccessor>()
                .RegisterTypeAsSingleton<Hasher, IHasher>()
                .RegisterTypeAsSingleton<FileService, IFileService>()
                .RegisterTypeAsSingleton<ImageCompressionService, IImageCompressionService>()
                .RegisterTypeAsScoped<Bus, IBus>()
                .RegisterTypeAsScoped<HttpContextAccessor, IHttpContextAccessor>()
                .RegisterTypeAsScoped<GoogleAuthenticationStrategy, IGoogleAuthenticationStrategy>()
                .RegisterTypeAsScoped<FacebookAuthenticationStrategy, IFacebookAuthenticationStrategy>()
                .RegisterTypeAsScoped<UsersService, IUsersService>()
                .RegisterTypeAsScoped<ApplicationUser, IApplicationUser>()
                .RegisterTypeAsScoped<UserUrlFactory, IUserUrlFactory>()
                .AddClosedTypeAsScoped(typeof(ICommandValidation<>),
                    new[] {typeof(UserRegisterCommandHandler).Assembly})
                .AddClosedTypeAsScoped(typeof(IDocumentRepository<,>), new[] {typeof(UserServiceDbContext).Assembly});

            if (!hostingEnvironment.IsAnyLocalDevelopment()) return;

            builder.RegisterTypeAsTransient<AuthenticationLogCleanupHostedService, IHostedService>();
        }


        private static IExternalAuthenticationStrategy ExternalAuthenticationStrategyResolver(
            IComponentContext componentContext,
            IEnumerable<Parameter> @params)
        {
            var provider = @params.TypedAs<AuthenticationProvider>();

            switch (provider)
            {
                case AuthenticationProvider.Google:
                {
                    return componentContext.Resolve<IGoogleAuthenticationStrategy>();
                }

                case AuthenticationProvider.Facebook:
                {
                    return componentContext.Resolve<IFacebookAuthenticationStrategy>();
                }

                case AuthenticationProvider.Twitter:
                {
                    throw new NotImplementedException();
                }

                case AuthenticationProvider.UsernamePassword:
                    throw new ArgumentOutOfRangeException(nameof(provider));
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider));
            }
        }
    }
}