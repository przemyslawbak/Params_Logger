using Params_Logger.Models;
using Params_Logger.Services;

namespace Params_Logger
{
    public class LogFactory
    {
        private Facade _facade;
        private bool _created;

        public LogFactory()
        {
            _facade = new Facade(new FileService(), new ConfigService(), new StringService(), new ProcessingPlant());
        }

        public Logger GetLogger()
        {
            ConfigModel config = GetLoggerConfig();

            Logger newLogger = CreateNewLogger(config);

            _created = true;

            return newLogger;
        }

        private Logger CreateNewLogger(ConfigModel config)
        {
            Logger newLogger = new Logger(config);

            if (config.DeleteLogs && !_created)
                config.FileService.DeleteFile(config.LogFile);

            return newLogger;
        }

        private ConfigModel GetLoggerConfig()
        {
            IConfigService configService = _facade.ConfigService;
            IFileService fileService = _facade.FileService;
            IStringService stringService = _facade.StringService;
            IProcessingPlant processingPlant = _facade.ProcessingPlant;

            return configService.GetConfig(stringService, fileService, processingPlant);
        }

    }
}
