using Params_Logger.Models;

namespace Params_Logger.Services
{
    public interface IConfigService
    {
        ConfigModel GetConfig(IStringService stringService, IFileService fileService, IProcessingPlant processingPlant);
    }
}