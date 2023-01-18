#region Imports
using CommandLine;
using ifcodes.ifconfig.Console.Verbs;
using System;
using ifcodes.ifconfig.Types;
#endregion

namespace ifcodes.ifconfig.Console
{
    internal class Program
    {
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
                System.Console.WriteLine("failure in main");

                System.Console.ReadKey(true);

                return Convert.ToInt32(ExitCode.Failure);
            }
        }
    }
}