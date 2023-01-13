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
                throw new Exception(Constants.Messaging.UNRECOVERABLE_ERROR_GETTING_USER_HOME_DIRECTORY, ex); 
            }
        }
    }
}