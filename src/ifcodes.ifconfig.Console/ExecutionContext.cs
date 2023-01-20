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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.File.Header;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                            configuration
                            .WriteTo.File(new CSVFormatter(), ".\\Logs\\log.csv", hooks: new HeaderWriter("Timestamp,Level,Message,Exception"))
                            .MinimumLevel.Verbose();
                        }
                        else
                        {
                            configuration.WriteTo.Console(outputTemplate: "{Message:l}{NewLine}").MinimumLevel.Verbose(); 
                        }

                        configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
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
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex, ex.Message);

                System.Console.ReadKey(true);

                return Convert.ToInt32(ExitCode.Failure);
            }
        }

        public static int ExecuteRemove(RemoveOptions options)
        {
            try
            {
                ISymbolicLinkService _symLinkservice = _host.Services.GetService<ISymbolicLinkService>();

                if (options.IsApplicationAll())
                {
                    _symLinkservice.RemoveConfiguration(options.Targets);
                }
                else
                {
                    _symLinkservice.RemoveConfiguration(options.Targets, options.Application);
                }

                return Convert.ToInt32(ExitCode.Success);
            }
            catch (Exception ex)
            {
                System.Console.ReadKey(true);

                return Convert.ToInt32(ExitCode.Failure);
            }
        }

        public static int HandleErrors<T>(ParserResult<T> result, IEnumerable<Error> errors)
        {
            foreach (Error error in errors)
            {
                switch (error.Tag)
                {
                    case ErrorType.NoVerbSelectedError:
                        {
                            System.Console.WriteLine("usage: config [--version] [--help] <command> [<args>] \n");

                            System.Console.WriteLine("These are the possible config commands:");

                            System.Console.WriteLine("   apply              Create an empty Git repository or reinitialize an existing one");
                            System.Console.WriteLine("   remove             Create an empty Git repository or reinitialize an existing one");

                            return Convert.ToInt32(ExitCode.Success);
                        }
                    case ErrorType.HelpVerbRequestedError:
                        {
                            //--help
                            HelpVerbRequestedError err = (HelpVerbRequestedError)error;

                            System.Console.WriteLine("usage: config [--version] [--help] <command> [<args>] \n");

                            System.Console.WriteLine("These are the possible config commands:");

                            System.Console.WriteLine("   apply              Create an empty Git repository or reinitialize an existing one");
                            System.Console.WriteLine("   remove             Create an empty Git repository or reinitialize an existing one");

                            return Convert.ToInt32(ExitCode.Success);
                        }
                    case ErrorType.VersionRequestedError:
                        {
                            //--version 
                            System.Console.WriteLine("ifconfig version 0.1.0-beta");

                            return Convert.ToInt32(ExitCode.Success);
                        }
                    case ErrorType.BadVerbSelectedError:
                        {
                            //blah --app autohotkey --targets --blah C:\\Users\\if\\.iffiles\\targets.json

                            BadVerbSelectedError err = (BadVerbSelectedError)error;

                            System.Console.WriteLine("error in verb");

                            return Convert.ToInt32(ExitCode.Failure);
                        }
                    case ErrorType.MissingRequiredOptionError:
                        {
                            //apply --app autohotkey 
                            //or 
                            //apply --targets C:\\Users\\if\\.iffiles\\targets.json

                            MissingRequiredOptionError err = (MissingRequiredOptionError)error;

                            if (err.NameInfo.LongName == "targets")
                            {
                                System.Console.WriteLine("fatal: <targets | t> argument must be passed \n");
                            }
                            else if (err.NameInfo.LongName == "app")
                            {
                                System.Console.WriteLine("fatal: <app | a> argument must be passed \n");
                            }

                            return Convert.ToInt32(ExitCode.Failure);
                        }

                }
            }

            return Convert.ToInt32(ExitCode.Failure);
        }
    }
}