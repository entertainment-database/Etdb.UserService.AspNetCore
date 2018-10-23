﻿using System;
using System.IO;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Etdb.UserService.Bootstrap
{
    public class Program
    {
        private static readonly string LogPath = Path.Combine(AppContext.BaseDirectory, "Logs", $"{Assembly.GetEntryAssembly().GetName().Name}.log");

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(Program.LogPath)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", 
                    theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services.AddAutofac())
                .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, true.ToString())
                .UseContentRoot(AppContext.BaseDirectory)
                .UseStartup<Startup>()
                .UseSerilog()
                .UseKestrel();
    }
}