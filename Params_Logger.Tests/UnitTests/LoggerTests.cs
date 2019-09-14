using Moq;
using Params_Logger.Models;
using Params_Logger.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Params_Logger.Tests.UnitTests
{
    public class LoggerTests
    {
        private readonly ConfigModel _config;
        private readonly Mock<IProcessingPlant> _processingPlantMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IStringService> _stringServiceMock;
        private readonly Logger _logger;

        public LoggerTests()
        {
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

            _logger = new Logger(_config);
        }

        [Fact]
        private void Prop_Called_SetsNewLogProperty()
        {
            _logger.ClockNow = new DateTime(2000, 01, 1, 1, 1, 1);

            _logger.Prop(1);

            Assert.NotNull(_logger.NewLog);
            Assert.Equal("Prop", _logger.NewLog.MethodName);
            Assert.Equal(new object[] { nameof(Prop_Called_SetsNewLogProperty), 1 }, _logger.NewLog.Arguments);
            Assert.Equal("Prop_Called_SetsNewLogProperty", _logger.NewLog.Method.Name);
            Assert.Equal(new DateTime(2000, 01, 1, 1, 1, 1), _logger.NewLog.Date);
        }

        [Fact]
        private void Called_Called_SetsNewLogProperty()
        {
            _logger.ClockNow = new DateTime(2000, 01, 1, 1, 1, 1);

            _logger.Called(1, "someString", true);

            Assert.NotNull(_logger.NewLog);
            Assert.Equal("Called", _logger.NewLog.MethodName);
            Assert.Equal(new object[] { 1, "someString", true }, _logger.NewLog.Arguments);
            Assert.Equal("Called_Called_SetsNewLogProperty", _logger.NewLog.Method.Name);
            Assert.Equal(new DateTime(2000, 01, 1, 1, 1, 1), _logger.NewLog.Date);
        }

        [Fact]
        private void Called_Info_SetsNewLogProperty()
        {
            _logger.ClockNow = new DateTime(2000, 01, 1, 1, 1, 1);

            _logger.Info("someInfo");

            Assert.NotNull(_logger.NewLog);
            Assert.Equal("Info", _logger.NewLog.MethodName);
            Assert.Equal(new object[] { "someInfo" }, _logger.NewLog.Arguments);
            Assert.Equal("Called_Info_SetsNewLogProperty", _logger.NewLog.Method.Name);
            Assert.Equal(new DateTime(2000, 01, 1, 1, 1, 1), _logger.NewLog.Date);
        }

        [Fact]
        private void Called_Error_SetsNewLogProperty()
        {
            _logger.ClockNow = new DateTime(2000, 01, 1, 1, 1, 1);

            _logger.Error("someError");

            Assert.NotNull(_logger.NewLog);
            Assert.Equal("Error", _logger.NewLog.MethodName);
            Assert.Equal(new object[] { "someError" }, _logger.NewLog.Arguments);
            Assert.Equal("Called_Error_SetsNewLogProperty", _logger.NewLog.Method.Name);
            Assert.Equal(new DateTime(2000, 01, 1, 1, 1, 1), _logger.NewLog.Date);
        }
    }
}
