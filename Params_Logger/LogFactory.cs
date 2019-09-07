using Params_Logger.Models;
using Params_Logger.Services;

namespace Params_Logger
{
    public class LogFactory : ILogFactory
    {
        private bool _created;

        public ILogger GetLogger()
        {
            ConfigModel config = GetLoggerConfig();

            ILogger newLogger = CreateNewLogger(config);

            _created = true;

            return newLogger;
        }

        private ILogger CreateNewLogger(ConfigModel config)
        {
            Logger newLogger = new Logger(config);

            if (config.DeleteLogs && !_created)
                config.FileService.DeleteFile(config.LogFile);

            return newLogger;
        }

        private ConfigModel GetLoggerConfig()
        {
            IConfigService configService = new ConfigService();
            IFileService fileService = new FileService();
            IStringService stringService = new StringService();
            IProcessingPlant processingPlant = new ProcessingPlant();

            return configService.GetConfig(stringService, fileService, processingPlant);
        }

    }
}
