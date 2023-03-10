#region Imports
using ifcodes.ifconfig.Repoistory;
using ifcodes.ifconfig.Repoistory.Abstractions;
using ifcodes.ifconfig.Services;
using ifcodes.ifconfig.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace ifcodes.ifconfig.Tests
{
    [TestFixture]

    internal class SymbolicLinkServiceTests
    {
        [Test]
        public void Successfully_Apply_Configuration()
        {
            //Arrange 
            Mock<ILogger<SymbolicLinkService>> mockLogger = new Mock<ILogger<SymbolicLinkService>>();
            Mock<IEnvironmentVariableRepistory> mockEnvironmentVariableRepository = new Mock<IEnvironmentVariableRepistory>();
            Mock<ITargetConfigurationRepository> mockTargetRepository = new Mock<ITargetConfigurationRepository>();

            MockFileSystem mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(@"C:\\Users\\test\\.dotfiles\\targets.json", new MockFileData("{ \"ConfigurationRepository\":\"C:\\\\Users\\\\test\\\\.dotfiles\",\"Applications\":[{ \"Name\":\"autohotkey\",\"TargetDirectory\":\"\\\\AppData\\\\Roaming\\\\Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\"},{ \"Name\":\"test\",\"TargetDirectory\":\"\"},{ \"Name\":\"git\",\"TargetDirectory\":\"\"}]}"));
            mockFileSystem.AddDirectory("C:\\Users\\test\\");
            mockFileSystem.AddDirectory("C:\\Users\\test\\.dotfiles\\test");
            mockFileSystem.AddFile("C:\\Users\\test\\.dotfiles\\test\\notes.txt", new MockFileData("Ismael was here hehe"));

            mockEnvironmentVariableRepository
                .Setup(x => x.GetHomeDirectory())
                .Returns("C:\\Users\\test");



            mockTargetRepository
                .Setup(x => x.GetConfigurationFromFile("C:\\Users\\test\\.dotfiles\\targets.json"))
                .Returns(new TargetConfiguration() {
                    RepositoryPath = "C:\\Users\\test\\.dotfiles",
                    Applications = new List<Application>()
                    {
                        new Application()
                        {
                            Name = "autohotkey",
                            TargetDirectory = "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup"
                        },
                        new Application()
                        {
                            Name = "test",
                            TargetDirectory = ""
                        },
                        new Application()
                        {
                            Name = "git",
                            TargetDirectory = ""
                        }
                    }
                });

            mockTargetRepository
                .Setup(x => x.GetSourcePathsFromConfigurationRepository("C:\\Users\\test\\.dotfiles\\test"))
                .Returns(new string[] {
                    "C:\\Users\\test\\.dotfiles\\test\\notes.txt"
                }); 
                


            //act
            ILogger<SymbolicLinkService> logger = mockLogger.Object;
            IEnvironmentVariableRepistory environmentVariableRepistory = mockEnvironmentVariableRepository.Object;
            ITargetConfigurationRepository targetRepository = mockTargetRepository.Object;

            SymbolicLinkService service = new SymbolicLinkService(
                logger,
                environmentVariableRepistory,
                 targetRepository,
                mockFileSystem);



            //assert
            service.ApplyConfiguration("C:\\Users\\test\\.dotfiles\\targets.json", "test");




            Assert.True(mockFileSystem.FileExists("C:\\Users\\test\\notes.txt"));

        }

        [Test]
        public void Successfully_Remove_Configuration()
        {

        }

        [Test]
        public void Successfully_Apply_All_Configurations()
        {

        }

        [Test]
        public void Successfully_Remove_All_Configurations()
        {

        }

    }
}