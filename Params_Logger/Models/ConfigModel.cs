using Params_Logger.Services;

namespace Params_Logger.Models
{
    public class ConfigModel
    {
        public IStringService StringService { get; set; }
        public IFileService FileService { get; set; }
        public IProcessingPlant ProcessingPlant { get; set; }
        public string LogFile { get; set; }
        public bool DebugOnly { get; set; }
        public bool DeleteLogs { get; set; }
        public bool ExecuteOnDebugSettings { get; set; }
        public bool FileLog { get; set; }
        public bool ConsoleLog { get; set; }
        public bool InfoOnly { get; set; }
    }
}
