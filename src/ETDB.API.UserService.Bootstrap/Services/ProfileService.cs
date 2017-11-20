﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ETDB.API.ServiceBase.Abstractions.Repositories;
using ETDB.API.UserService.Domain.Entities;
using ETDB.API.UserService.Repositories.Base;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace ETDB.API.UserService.Bootstrap.Services
{
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.IssuedClaims = context.Subject.Claims.ToList();

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(context.IsActive);
        }
    }
}
