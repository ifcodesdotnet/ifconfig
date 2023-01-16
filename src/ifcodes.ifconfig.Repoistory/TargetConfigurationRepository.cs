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
                throw new JsonReaderException("targets.json contains invalid json.", ex); 
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException("targets.json not found in specified path.", path ,ex);
            }
            catch (Exception ex)
            {
                throw new Exception("unrecoverable error occurred when reading targets.json.", ex);
            }
        }

        public string[] GetSourcePathsFromConfigurationRepository(string path)
        {
            try
            {
                return _fileSystem.Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new DirectoryNotFoundException("unable to find configuration repository directory.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("unrecoverable error occurred while attempting to get source paths from configuration repository.", ex);
            }
        }
    }
}