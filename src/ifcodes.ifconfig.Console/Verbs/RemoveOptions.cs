#region Imports
using CommandLine;
#endregion

namespace ifcodes.ifconfig.Console.Verbs
{
    [Verb("remove", HelpText = "Remove configuration from target(s).")]
    public class RemoveOptions
    {
        [Option('t', "targets", Required = true, HelpText = "path to targets file.")]
        public string Targets { get; set; }


        [Option('a', "app", Required = true, HelpText = "app configuration to remove.")]
        public string Application { get; set; }
    }
}