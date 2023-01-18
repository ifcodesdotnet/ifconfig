#region Imports
using CommandLine;
using CommandLine.Text;
using ifcodes.ifconfig.Console.Verbs;
using ifcodes.ifconfig.Repoistory;
using ifcodes.ifconfig.Repoistory.Abstractions;
using ifcodes.ifconfig.Services;
using ifcodes.ifconfig.Services.Abstractions;
using ifcodes.ifconfig.Types;
using ifcodes.ifutilities.ifhosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.File.Header;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
#endregion

namespace ifcodes.ifconfig.Console
{
    internal class ExecutionContext
    {
        #region Configure Application Hosting
        private static readonly IHost _host;

        static ExecutionContext()
        {
            try
            {
                string environment = HostingEnvironment.GetAssemblyBuildConfigurationFromType(typeof(ExecutionContext));

                _host = Host.CreateDefaultBuilder()
                    .ConfigureServices((hostingContext, services) =>
                    {
                        services.AddScoped<ISymbolicLinkService, SymbolicLinkService>();
                        services.AddScoped<IFileSystem, FileSystem>();
                        services.AddScoped<IEnvironmentVariableRepistory, EnvironmentVariableRepistory>();
                        services.AddScoped<ITargetConfigurationRepository, TargetConfigurationRepository>();
                    })
                    .UseSerilog((context, services, configuration) =>
                    {
                        if (environment.Contains("debug"))
                        {
                            configuration.WriteTo.File(new CSVFormatter(), ".\\Logs\\log.csv"
                               , hooks: new HeaderWriter("Timestamp,Level,Message,Exception")
                               )
                           .MinimumLevel.Verbose()
                           .MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
                        }
                        else
                        {
                            configuration.WriteTo.File(new CSVFormatter(), ".\\Logs\\log.csv"
                                , hooks: new HeaderWriter("Timestamp,Level,Message,Exception")
                                )
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
                        }
                    })
                    .Start();
            }
            catch (Exception ex)
            {
                throw new Exception("An unrecoverable error occurred during application host configuration.", ex);
            }
        }
        #endregion

        public static int ExecuteApply(ApplyOptions options)
        {
            ILogger<ExecutionContext> _logger = _host.Services.GetService<ILogger<ExecutionContext>>();

            try
            {
                ISymbolicLinkService _symLinkservice = _host.Services.GetService<ISymbolicLinkService>();

                if (!string.IsNullOrEmpty(options.Application))
                {
                    if (options.IsApplicationAll())
                    {
                        _symLinkservice.ApplyConfiguration(options.Targets);
                    }
                    else
                    {
                        _symLinkservice.ApplyConfiguration(options.Targets, options.Application);
                    }

                    return Convert.ToInt32(ExitCode.Success);
                }

                return Convert.ToInt32(ExitCode.Failure);
            }
            catch (Exception ex)
            {
                System.Console.Write("fatal: " + ex.Message);

                _logger.Log(LogLevel.Critical, ex, ex.Message);  

                return Convert.ToInt32(ExitCode.Failure);
            }
        }

        public static int ExecuteRemove(RemoveOptions options)
        {
            ILogger<ExecutionContext> _logger = _host.Services.GetService<ILogger<ExecutionContext>>();

            try
            {
                ISymbolicLinkService _symLinkservice = _host.Services.GetService<ISymbolicLinkService>();

                _symLinkservice.RemoveConfiguration(options.Targets, options.Application);

                return Convert.ToInt32(ExitCode.Success);
            }
            catch (Exception ex)
            {
                System.Console.Write("fatal: " + ex.Message);

                _logger.Log(LogLevel.Critical, ex.Message);

                return Convert.ToInt32(ExitCode.Failure);
            }
        }

        public static int HandleErrors<T>(ParserResult<T> result, IEnumerable<Error> errors)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;

                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);

            System.Console.Write(helpText);

            return Convert.ToInt32(ExitCode.Failure);
        }
    }
}