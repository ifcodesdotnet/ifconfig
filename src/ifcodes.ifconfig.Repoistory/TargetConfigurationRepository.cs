#region Imports
using ifcodes.ifconfig.Types;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Abstractions;
using ifcodes.ifconfig.Repoistory.Abstractions;
#endregion

namespace ifcodes.ifconfig.Repoistory
{
    public class TargetConfigurationRepository : ITargetConfigurationRepository
    {
        #region Dependency Injection
        private readonly IFileSystem _fileSystem;

        public TargetConfigurationRepository(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        #endregion

        public TargetConfiguration GetConfigurationFromFile(string path)
        {
            try
            {
                string json = _fileSystem.File.ReadAllText(path);

                return JsonConvert.DeserializeObject<TargetConfiguration>(json);
            }
            catch (JsonReaderException ex)
            {
                throw new JsonReaderException(Constants.Messaging.TARGETS_INVALID_JSON, ex); 
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException(Constants.Messaging.TARGETS_NOT_FOUND, path ,ex);
            }
            catch (Exception ex)
            {
                throw new Exception(Constants.Messaging.UNRECOVERABLE_ERROR_READING_TARGETS, ex);
            }
        }

        public string[] GetSourcePathsFromConfigurationRepository(string path)
        {
            try
            {
                return _fileSystem.Directory.GetFiles(path, Constants.Characters.ASTERISK, SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new DirectoryNotFoundException(Constants.Messaging.CONFIGURATION_REPOSITORY_DIRECTORY_NOT_FOUND, ex);
            }
            catch (Exception ex)
            {
                throw new Exception(Constants.Messaging.UNRECOVERABLE_ERROR_GETTING_SOURCE_PATHS, ex);
            }
        }
    }
}