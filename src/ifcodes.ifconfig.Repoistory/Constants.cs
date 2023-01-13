using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ifcodes.ifconfig.Repoistory
{
    public static class Constants
    {
        public static class Messaging
        {
            public const string TARGETS_NOT_FOUND = "targets.json not found in specified path.";
            public const string TARGETS_INVALID_JSON = "targets.json contains invalid json.";
            public const string UNRECOVERABLE_ERROR_READING_TARGETS = "unrecoverable error occurred when reading targets.json.";



            public const string CONFIGURATION_REPOSITORY_DIRECTORY_NOT_FOUND = "unable to find configuration repository directory.";
            public const string UNRECOVERABLE_ERROR_GETTING_SOURCE_PATHS = "unrecoverable error occurred while attempting to get source paths from configuration repository.";




            public const string UNRECOVERABLE_ERROR_DELETEING_FILE = "unrecoverable error occurred while attempting delete file.";

            


            public const string UNRECOVERABLE_ERROR_GETTING_USER_HOME_DIRECTORY = "unrecoverable error occurred while getting user home directory.";

            public const string INVALID_PATH_WHEN_ATTEMPTING_TO_DELETE_FILE = "invalid path passed when attempting to delete file.";



            public const string UNRECOVERABLE_ERROR_APPLYING_CONFIGURATION = "unrecoverable error occurred while attempting to apply configuration for ";


        }

        public class Characters
        {
            public const string ESCAPED_BACKSLASH = "\\";
            public const string ASTERISK = "*";
        }
    }
   
}
