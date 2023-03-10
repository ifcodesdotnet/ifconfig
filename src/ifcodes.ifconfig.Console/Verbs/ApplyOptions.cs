#region Imports
using CommandLine;
#endregion

namespace ifcodes.ifconfig.Console.Verbs
{
    [Verb("apply", HelpText = "Apply configuration to target(s).")]
    public class ApplyOptions
    {
        [Option('t', "targets", Required = true, HelpText = "path to targets file.")]
        public string Targets { get; set; }


        [Option('a', "app", Required = true, HelpText = "app to configure, setting this to all will apply configuration to all applications in target configuration.")]
        public string Application { get; set; }
    }
}