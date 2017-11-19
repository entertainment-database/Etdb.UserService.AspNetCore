﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ETDB.API.ServiceBase.Common.Factory;
using ETDB.API.UserService.Data;
using ETDB.API.UserService.Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETDB.API.UserService.Scaffold
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables();

            var configurationRoot = builder.Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(configurationRoot)
                .AddScoped<UserServiceContext>()
                .BuildServiceProvider();

            var databaseContext = serviceProvider
                .GetService<UserServiceContext>();

            if (databaseContext.Database.GetPendingMigrations().Any())
            {
                Console.WriteLine("Migration(s) not applied, apply now?");
                if (!Console.ReadKey().KeyChar.ToString().Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                databaseContext.Database.Migrate();
            }

            var hasher = new HasherFactory()
                .CreateHasher(KeyDerivationPrf.HMACSHA1);

            var salt = hasher
                .GenerateSalt();

            var admin = new User
            {
                UserName = "admin",
                Salt = salt,
                Password = hasher.CreateSaltedHash("admin", salt),
                Email = "admin@etdb.io",
                Name = "Admin",
                LastName = "Admin"
            };

            databaseContext.Set<User>().Add(admin);

            databaseContext.SaveChanges();

            var securityRoles = new List<Securityrole>
            {
                new Securityrole
                {
                    Designation = "Administrator"
                },
                new Securityrole
                {
                    Designation = "User"
                }
            };

            foreach (var role in securityRoles)
            {
                databaseContext.Set<Securityrole>().Add(role);
                databaseContext.SaveChanges();
                databaseContext.Set<UserSecurityrole>().Add(new UserSecurityrole
                {
                    SecurityroleId = role.Id,
                    UserId = admin.Id,
                });
                databaseContext.SaveChanges();
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
        }
    }
}