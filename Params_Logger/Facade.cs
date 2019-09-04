using Params_Logger.Services;

namespace Params_Logger
{
    public class Facade
    {
        public Facade(IFileService fileServ, IConfigService configServ, IStringService stringServ)
        {
            FileService = fileServ;
            ConfigService = configServ;
            StringService = stringServ;
        }

        public IFileService FileService { get; set; }
        public IConfigService ConfigService { get; set; }
        public IStringService StringService { get; set; }
    }
}
