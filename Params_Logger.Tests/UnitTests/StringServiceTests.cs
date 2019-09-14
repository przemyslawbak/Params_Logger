using Params_Logger.Models;
using Params_Logger.Services;
using System;
using System.Diagnostics;
using Xunit;

namespace Params_Logger.Tests.UnitTests
{
    public class StringServiceTests
    {
        private StringService _service;

        public StringServiceTests()
        {
            _service = new StringService();
        }

        [Fact]
        public void GetStringAttributes_Called_ReturnsStringLogLine()
        {
            LogModel model = new LogModel() { Arguments = new object[] { "someString" }, Date = new DateTime(2000, 1, 2, 3, 4, 5, 678), Method = new StackTrace().GetFrame(0).GetMethod(), MethodName = "CALLED" };

            string result = _service.GetStringAttributes(model);

            Assert.Equal("03:04:05.678|CALLED|StringServiceTests|GetStringAttributes_Called_ReturnsStringLogLine()", result);
        }
    }
}
