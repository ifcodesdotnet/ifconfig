#region Imports
using System.Collections.Generic;
#endregion

namespace ifcodes.ifconfig.Types
{
    public class TargetConfiguration
    {
        public string RepositoryPath { get; set; }

        public List<Application> Applications { get; set; }
    }
}