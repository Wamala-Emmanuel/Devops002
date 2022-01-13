using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.Services;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace GatewayService.Tests.Services
{
    public class DirectoryServiceTests
    {
        private readonly DirectoryService _directoryService;
        private readonly ILogger<DirectoryService> _logger;
        private readonly MockFileSystem _mockFileSystem;
        private readonly string _folderPath = "./wwwroot/exports";
        private readonly string _newPath = "./wwwroot/exports/foo.csv";
        private readonly int _buffer = 1024;
        private readonly Guid _id;

        public DirectoryServiceTests(ITestOutputHelper testOutputHelper)
        {
            _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\wwwroot\exports\hello.csv", new MockFileData("Testing is meh.") },
            });

            _logger = testOutputHelper.BuildLoggerFor<DirectoryService>();

            _id = Guid.NewGuid();

            _mockFileSystem.AddFile($"./wwwroot/exports/{_id}.csv", new MockFileData("Testing,is,nin."));

            _directoryService = new DirectoryService(_logger, _mockFileSystem);
        }

        [Fact]
        public void Service_ShouldNotThrowException()
        {
            _directoryService.CreateTempFile(_folderPath, $"{_id}.csv", $"{_id}.csv", _buffer);
        }
      
        [Fact]
        public void Service_ShouldCreateExportFile()
        {
            _directoryService.CreateTempFile(_folderPath, $"{_id}.csv", $"{_id}.csv", _buffer);

            var fileName = $"{_folderPath}/{_id}.csv";

            Assert.True(_mockFileSystem.FileExists(fileName));
        }


        [Fact]
        public void FileExists_ShouldNotThrowException()
        {
            _directoryService.FileExists($"{_folderPath}/{_id}.csv");
        }

        [Fact]
        public void FileExists_ShouldReturnTrueWhenFileExists()
        {
            var result = _directoryService.FileExists($"{_folderPath}/{_id}.csv");

            Assert.True(result);
        }

        [Fact]
        public void FileExists_ShouldReturnFalseWhenFileExists()
        {
            var result = _directoryService.FileExists($"{_id}.csv");

            Assert.False(result);
        }

        [Fact]
        public void DeleteDirectory_ShouldNotThrowException()
        {
            _directoryService.DeleteDirectory($"{_folderPath}");
        }

        [Fact]
        public void DeleteFile_ShouldNotThrowException()
        {
            _directoryService.DeleteFile($"{ _id}.csv");
        }
        
        [Fact]
        public void RenameFile_ShouldNotThrowException()
        {
            _directoryService.RenameFile($"{_folderPath}\\hello.csv", _newPath);
        }
        
        [Fact]
        public void RenameFile_ShouldRenameFile()
        {
            _directoryService.RenameFile($"{_folderPath}\\hello.csv", _newPath);

            var result = _directoryService.FileExists(_newPath);

            Assert.True(result);
        }
    }
}
