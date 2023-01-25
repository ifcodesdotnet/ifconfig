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
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
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
                            .WriteTo.Console(outputTemplate: "{Message:l}{NewLine}{Exception}")
                            .MinimumLevel.Verbose();
                        }
                        else
                        {
                            configuration
                            .WriteTo.Console(outputTemplate: "{Message:l}{NewLine}")
                            .MinimumLevel.Information(); 
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
                _logger.Log(LogLevel.Critical, ex.Message);

                System.Console.ReadKey(true);

                return Convert.ToInt32(ExitCode.Failure);
            }
        }

        public static int ExecuteRemove(RemoveOptions options)
        {
            ILogger<ExecutionContext> _logger = _host.Services.GetService<ILogger<ExecutionContext>>();

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
                _logger.Log(LogLevel.Critical, ex.Message);

                System.Console.ReadKey(true);

                return Convert.ToInt32(ExitCode.Failure);
            }
        }

        public static int HandleErrors<T>(ParserResult<T> result, IEnumerable<Error> errors)
        {
            //http://courses.cms.caltech.edu/cs11/material/general/usage.html

            foreach (Error error in errors)
            {
                switch (error.Tag)
                {
                    case ErrorType.NoVerbSelectedError:
                        {
                            //no commands passed
                            System.Console.WriteLine("usage: (if)config [--version] [--help] <command> [<args>] \n");

                            System.Console.WriteLine("possible config commands:");

                            System.Console.WriteLine("   apply              Apply configurations from .dotfiles directory based on app and targets");
                            System.Console.WriteLine("   remove             Remove configurations from .dotfiles directory based on app and targets");

                            System.Console.ReadKey(true);

                            return Convert.ToInt32(ExitCode.Success);
                        }
                    case ErrorType.HelpVerbRequestedError:
                        {
                            //--help
                            System.Console.WriteLine("usage: (if)config [--version] [--help] <command> [<args>] \n");

                            System.Console.WriteLine("possible config commands:");

                            System.Console.WriteLine("   apply              Apply configurations from .dotfiles directory based on app and targets");
                            System.Console.WriteLine("   remove             Remove configurations from .dotfiles directory based on app and targets");

                            System.Console.ReadKey(true);

                            return Convert.ToInt32(ExitCode.Success);
                        }
                    case ErrorType.VersionRequestedError:
                        {
                            //--version 
                            System.Console.WriteLine("(if)config version 0.1.0-beta");

                            System.Console.ReadKey(true);

                            return Convert.ToInt32(ExitCode.Success);
                        }
                    case ErrorType.BadVerbSelectedError:
                        {
                            //blah --app autohotkey --targets --blah C:\\Users\\if\\.iffiles\\targets.json

                            BadVerbSelectedError err = (BadVerbSelectedError)error;

                            //got this wording / format from doing git test
                            System.Console.WriteLine("(if)config: " + err.Token + " is not a (if)config command. See '(if)config --help'.");

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
                    case ErrorType.HelpRequestedError:
                        {
                            string verb = result.TypeInfo.Current.Name;

                            verb = verb.ToLower(); 

                            if (verb.Contains("apply"))
                            {
                                verb = "apply";

                                System.Console.WriteLine("usage: config " + verb + " <targets|t> <app|a> \n");
                                System.Console.WriteLine("required command args:");
                                System.Console.WriteLine("    targets           Absolute path to targets file");
                                System.Console.WriteLine("    app               Application to configure from targets file entry");
                            }

                            if (verb.Contains("remove"))
                            {
                                verb = "remove";

                                System.Console.WriteLine("usage: config " + verb + " <targets|t> <app|a> \n");
                                System.Console.WriteLine("required command args:");
                                System.Console.WriteLine("    targets           Absolute path to targets file");
                                System.Console.WriteLine("    app               Application to configure from targets file entry");
                            }

                            System.Console.ReadKey(true);

                            return Convert.ToInt32(ExitCode.Success);
                        }
                }
            }
            return Convert.ToInt32(ExitCode.Failure);
        }
    }
}