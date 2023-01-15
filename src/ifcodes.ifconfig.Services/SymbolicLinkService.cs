#region Imports 
using ifcodes.ifconfig.Types;
using Microsoft.Extensions.Logging;
using System;
using ifcodes.ifconfig.Repoistory.Abstractions;
using ifcodes.ifconfig.Services.Abstractions;
using System.IO.Abstractions;
#endregion

namespace ifcodes.ifconfig.Services
{
    public class SymbolicLinkService : ISymbolicLinkService
    {
        #region Dependency Injection
        private readonly ILogger<SymbolicLinkService> _logger;
        private readonly IEnvironmentVariableRepistory _environmentVariableRepistory;
        private readonly ITargetConfigurationRepository _targetConfigurationRepository;
        private readonly IFileSystem _fileSystem;

        public SymbolicLinkService(
            ILogger<SymbolicLinkService> logger
            , IEnvironmentVariableRepistory environmentVariableRepistory
            , ITargetConfigurationRepository targetConfigurationRepository
            , IFileSystem fileSystem
            )
        {
            _logger = logger;
            _environmentVariableRepistory = environmentVariableRepistory;
            _targetConfigurationRepository = targetConfigurationRepository;
            _fileSystem = fileSystem;
        }
        #endregion

        public void ApplyConfiguration(string targets)
        {
            _logger.Log(LogLevel.Trace, "attempting to apply configurations for all targets in " + targets + "... ");

            try
            {
                TargetConfiguration targetConfiguration = _targetConfigurationRepository.GetConfigurationFromFile(targets);

                _logger.Log(LogLevel.Trace, "successfully retrieved configurations for all targets from file " + targets + "... ");

                string homeDirectoryPath = _environmentVariableRepistory.GetHomeDirectory();

                foreach (Application configurableApplication in targetConfiguration.Applications)
                {
                    string configurationDirectory = _fileSystem.Path.Combine(new[] { targetConfiguration.RepositoryPath, configurableApplication.Name });

                    //absolute paths to configuration files stored in .dotfiles directories
                    string[] sourceFilePaths = _targetConfigurationRepository.GetSourcePathsFromConfigurationRepository(configurationDirectory);

                    foreach (string sourceAbsoluteFilePath in sourceFilePaths)
                    {
                        string absoluteTargetDirectory = _fileSystem.Path.Combine(new[] { homeDirectoryPath, configurableApplication.TargetDirectory });

                        _logger.Log(LogLevel.Trace, "checking if target directory " + absoluteTargetDirectory + " exists ...");

                        if (!_fileSystem.Directory.Exists(absoluteTargetDirectory))
                        {
                            _logger.Log(LogLevel.Trace, "target directory does not exist at " + absoluteTargetDirectory + " ...");

                            _logger.Log(LogLevel.Trace, "attempting to create target directory " + absoluteTargetDirectory + "... ");

                            IDirectory directory = (IDirectory)_fileSystem.Directory.CreateDirectory(absoluteTargetDirectory);

                            if (directory.Exists(absoluteTargetDirectory))
                            {
                                _logger.Log(LogLevel.Trace, "successfully created target directory at " + absoluteTargetDirectory + " ...");
                            }
                        }
                        else
                        {
                            _logger.Log(LogLevel.Trace, "target directory exists ... continuing application configuration process ...");
                        }

                        string sourceFileName = _fileSystem.Path.GetFileName(sourceAbsoluteFilePath);

                        string targetAbsoluteFilePath = _fileSystem.Path.Combine(new[] { absoluteTargetDirectory, sourceFileName });

                        _logger.Log(LogLevel.Trace, "attempting to create symbolic link from " + sourceAbsoluteFilePath + " to " + targetAbsoluteFilePath + " ...");

                        if (!_fileSystem.File.Exists(targetAbsoluteFilePath))
                        {
                            IFileInfo file = (IFileInfo)_fileSystem.File.CreateSymbolicLink(targetAbsoluteFilePath, sourceAbsoluteFilePath);

                            if (file.Exists)
                            {
                                _logger.Log(LogLevel.Information, "successfully applied " + file.Name + " ...");
                            }
                        }
                        else
                        {
                            _logger.Log(LogLevel.Information, "configuration already exists at " + targetAbsoluteFilePath + " ...");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("unrecoverable error occurred while attempting to apply configuration ...", ex);
            }
        }

        public void ApplyConfiguration(string targets, string application)
        {
            _logger.Log(LogLevel.Trace, "attempting to apply configuration(s) for " + application + " ...");

            try
            {
                TargetConfiguration targetConfiguration = _targetConfigurationRepository.GetConfigurationFromFile(targets);

                _logger.Log(LogLevel.Trace, "successfully retrieved target configuration from file " + targets + " ...");

                string homeDirectoryPath = _environmentVariableRepistory.GetHomeDirectory();

                foreach (Application configurableApplication in targetConfiguration.Applications)
                {
                    if (configurableApplication.IsSameNameAs(application))
                    {
                        _logger.Log(LogLevel.Trace, "configurations available for " + application + " ... begin configuration application process ...");

                        string configurationDirectory = _fileSystem.Path.Combine(new[] { targetConfiguration.RepositoryPath, configurableApplication.Name });

                        //absolute paths to configuration files stored in .dotfiles directories
                        string[] sourceFilePaths = _targetConfigurationRepository.GetSourcePathsFromConfigurationRepository(configurationDirectory);

                        foreach (string sourceAbsoluteFilePath in sourceFilePaths)
                        {
                            string absoluteTargetDirectory = _fileSystem.Path.Combine(new[] { homeDirectoryPath, configurableApplication.TargetDirectory });

                            _logger.Log(LogLevel.Trace, "checking if target directory " + absoluteTargetDirectory + " exists ...");

                            if (!_fileSystem.Directory.Exists(absoluteTargetDirectory))
                            {
                                _logger.Log(LogLevel.Trace, "target directory does not exist at " + absoluteTargetDirectory + " ...");

                                _logger.Log(LogLevel.Trace, "attempting to create target directory " + absoluteTargetDirectory + " ...");

                                IDirectory directory = (IDirectory)_fileSystem.Directory.CreateDirectory(absoluteTargetDirectory);

                                if (directory.Exists(absoluteTargetDirectory))
                                {
                                    _logger.Log(LogLevel.Trace, "successfully created target directory at " + absoluteTargetDirectory + " ...");
                                }
                            }
                            else
                            {
                                _logger.Log(LogLevel.Trace, "target directory exists ... continuing application configuration process ...");
                            }

                            string sourceFileName = _fileSystem.Path.GetFileName(sourceAbsoluteFilePath);

                            string targetAbsoluteFilePath = _fileSystem.Path.Combine(new[] { absoluteTargetDirectory, sourceFileName });

                            _logger.Log(LogLevel.Trace, "attempting to create symbolic link from " + sourceAbsoluteFilePath + " to " + targetAbsoluteFilePath + " ...");

                            if (!_fileSystem.File.Exists(targetAbsoluteFilePath))
                            {
                                IFileInfo file = (IFileInfo)_fileSystem.File.CreateSymbolicLink(targetAbsoluteFilePath, sourceAbsoluteFilePath);

                                if (file.Exists)
                                {
                                    _logger.Log(LogLevel.Information, "successfully applied " + file.Name + " for " + application + " ...");
                                }
                            }
                            else
                            {
                                _logger.Log(LogLevel.Information, "configuration already exists for " + application + " at " + targetAbsoluteFilePath + " ...");
                            }
                        }
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, "no configurations available for " + application + " ...");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("unrecoverable error occurred while attempting to apply configuration for " + application + " ...", ex);
            }
        }

        public void RemoveConfiguration(string targets)
        {
            _logger.Log(LogLevel.Trace, "attempting to remove configurations for all targets in " + targets + "... ");

            try
            {
                TargetConfiguration targetConfiguration = _targetConfigurationRepository.GetConfigurationFromFile(targets);

                _logger.Log(LogLevel.Trace, "successfully retrieved configurations for all targets from file " + targets + "... ");

                string homeDirectoryPath = _environmentVariableRepistory.GetHomeDirectory();

                foreach (Application configurableApplication in targetConfiguration.Applications)
                {
                    string configurationDirectory = _fileSystem.Path.Combine(new[] { targetConfiguration.RepositoryPath, configurableApplication.Name });

                    //absolute paths to configuration files stored in .dotfiles directories
                    string[] sourceFilePaths = _targetConfigurationRepository.GetSourcePathsFromConfigurationRepository(configurationDirectory);

                    foreach (string sourceAbsoluteFilePath in sourceFilePaths)
                    {
                        string absoluteTargetDirectory = _fileSystem.Path.Combine(new[] { homeDirectoryPath, configurableApplication.TargetDirectory });

                        string sourceFileName = _fileSystem.Path.GetFileName(sourceAbsoluteFilePath);

                        string targetAbsoluteFilePath = _fileSystem.Path.Combine(new[] { absoluteTargetDirectory, sourceFileName });

                        _fileSystem.File.Delete(targetAbsoluteFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("unrecoverable error occurred while attempting to remove all configurations.", ex);
            }
        }

        public void RemoveConfiguration(string targets, string application)
        {
            _logger.Log(LogLevel.Trace, "attempting to remove configuration(s) for " + application + " ...");

            try
            {
                TargetConfiguration targetConfiguration = _targetConfigurationRepository.GetConfigurationFromFile(targets);

                _logger.Log(LogLevel.Trace, "successfully retrieved configurations for all targets from file " + targets + "... ");

                string homeDirectoryPath = _environmentVariableRepistory.GetHomeDirectory();

                foreach (Application configurableApplication in targetConfiguration.Applications)
                {
                    if (configurableApplication.IsSameNameAs(application))
                    {
                        string configurationDirectory = _fileSystem.Path.Combine(new[] { targetConfiguration.RepositoryPath, configurableApplication.Name });

                        //absolute paths to configuration files stored in .dotfiles directories
                        string[] sourceFilePaths = _targetConfigurationRepository.GetSourcePathsFromConfigurationRepository(configurationDirectory);

                        foreach (string sourceAbsoluteFilePath in sourceFilePaths)
                        {
                            string absoluteTargetDirectory = _fileSystem.Path.Combine(new[] { homeDirectoryPath, configurableApplication.TargetDirectory });

                            string sourceFileName = _fileSystem.Path.GetFileName(sourceAbsoluteFilePath);

                            string targetAbsoluteFilePath = _fileSystem.Path.Combine(new[] { absoluteTargetDirectory, sourceFileName });

                            _fileSystem.File.Delete(targetAbsoluteFilePath);
                        }
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, "no configurations available to remove for " + application + " ...");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("unrecoverable error occurred while attempting to remove configuration for " + application + ".", ex);
            }
        }
    }
}