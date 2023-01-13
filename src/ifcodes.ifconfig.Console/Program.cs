#region Imports
using CommandLine;
using ifcodes.ifconfig.Console.Verbs;
using System;
using Serilog;
using ifcodes.ifconfig.Types;
#endregion

namespace ifcodes.ifconfig.Console
{
    internal class Program
    {
        #region Initialize Static Logger 
        static Program()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(
                 outputTemplate: "{Level} {Message:l}{NewLine} {Exception}"
                )
                .MinimumLevel.Information()
                .CreateBootstrapLogger();
        } 
        #endregion

        static int Main(string[] args)
        {
            try
            {
                Parser parser = new Parser(configuration => {
                    configuration.HelpWriter = null;
                    configuration.CaseSensitive = false;
                    configuration.IgnoreUnknownArguments = false;
                });

                ParserResult<object> result = parser.ParseArguments<ApplyOptions, RemoveOptions>(args);

                return result.MapResult(
                    (ApplyOptions options) => ExecutionContext.ExecuteApply(options),
                    (RemoveOptions options) => ExecutionContext.ExecuteRemove(options),
                    errors => ExecutionContext.HandleErrors(result, errors));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, ex.Message);

                return Convert.ToInt32(ExitCode.Failure);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}