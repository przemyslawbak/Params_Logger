using Moq;
using Params_Logger.Models;
using Params_Logger.Services;
using System;
using Xunit;

namespace Params_Logger.Tests.UnitTests
{
    public class ParamsLoggerTests
    {
        private readonly Mock<ILogFactory> _factoryMock;
        private readonly Mock<IProcessingPlant> _processingPlantMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IStringService> _stringServiceMock;
        private readonly ParamsLogger _logger;

        private readonly ConfigModel _config;

        public ParamsLoggerTests()
        {
            _factoryMock = new Mock<ILogFactory>();
            _fileServiceMock = new Mock<IFileService>();
            _stringServiceMock = new Mock<IStringService>();
            _processingPlantMock = new Mock<IProcessingPlant>();
            _config = new ConfigModel()
            {
                ConsoleLog = true,
                DebugOnly = true,
                InfoOnly = true,
                FileLog = true,
                ExecuteOnDebugSettings = true,
                DeleteLogs = true,
                LogFile = "",
                ProcessingPlant = _processingPlantMock.Object,
                FileService = _fileServiceMock.Object,
                StringService = _stringServiceMock.Object
            };

            _factoryMock.Setup(f => f.GetLogger())
                .Returns(new Logger(_config)); ;

            _logger = new ParamsLogger();
        }

        [Fact]
        private void GetLogger_Called_RrturnsLogger()
        {
            _logger.Factory = _factoryMock.Object;

            ILogger _log = _logger.GetLogger();
            Type type = _log.GetType();

            Assert.NotNull(_log);
            Assert.IsAssignableFrom<ILogger>(_log);
            Assert.True(type.GetMethod("Prop") != null);
            Assert.True(type.GetMethod("Called") != null);
            Assert.True(type.GetMethod("Info") != null);
            Assert.True(type.GetMethod("Error") != null);
        }
    }
}
