#region Imports
using ifcodes.ifconfig.Types;
#endregion 

namespace ifcodes.ifconfig.Repoistory.Abstractions
{
    public interface ITargetConfigurationRepository
    {
        TargetConfiguration GetConfigurationFromFile(string path);

        string[] GetSourcePathsFromConfigurationRepository(string path); 
    }
}