#region Imports
using System;
using ifcodes.ifconfig.Repoistory.Abstractions;
#endregion

namespace ifcodes.ifconfig.Repoistory
{
    public class EnvironmentVariableRepistory : IEnvironmentVariableRepistory
    {
        public string GetHomeDirectory()
        {
            try
            { 
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            catch (Exception ex)
            {
                throw new Exception("unrecoverable error occurred while getting user home directory.", ex); 
            }
        }
    }
}