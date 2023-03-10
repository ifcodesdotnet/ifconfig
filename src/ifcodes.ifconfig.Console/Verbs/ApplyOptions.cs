#region Imports
using CommandLine;
#endregion

namespace ifcodes.ifconfig.Console.Verbs
{
    //[Verb("apply", HelpText = "Apply configuration to target(s).")]
    public class ApplyOptions
    {
        //[Option('t', "targets", Required = true, HelpText = "path to targets file.")]
        public string Targets { get; set; }

        //[Option('a', "app", Required = true, HelpText = "app to configure, setting this to all will apply configuration to all applications in target configuration.")]
        public string Application { get; set; }

        //todo: I can create a base class for all verbs or atleast for the apply and remove verbs 
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