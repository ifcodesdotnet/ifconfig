#region Imports
using CommandLine;
#endregion

namespace ifcodes.ifconfig.Console.Verbs
{
    [Verb("remove", HelpText = "Remove configuration from target(s).")]
    public class RemoveOptions
    {
        [Option('t', "targets", Required = true)]
        public string Targets { get; set; }

        [Option('a', "app", Required = true)]
        public string Application { get; set; }

        public bool IsApplicationAll()
        {
            if (!string.IsNullOrEmpty(this.Application))
            {
                if (this.Application.ToLower() == "all")
                {
                    return true;
                }
            }

            return false;
        }
    }
}