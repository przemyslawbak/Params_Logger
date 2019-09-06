using Params_Logger.Models;
using System;
using System.Threading.Tasks;

namespace Params_Logger.Services
{
    public class ProcessingPlant : IProcessingPlant
    {
        /// <summary>
        /// displaying and saving log depending on loaded properties
        /// </summary>
        /// <param name="line">string log line</param>
        /// <param name="config">configuration object</param>
        public async Task SaveAndDisplay(string line, ConfigModel config)
        {
            if (config.FileLog)
                await config.FileService.SaveLogAsync(line, config.LogFile);
            if (config.ConsoleLog && !config.InfoOnly)
                Console.WriteLine(line);
            else if (config.ConsoleLog && config.InfoOnly && line.Contains("|INFO|"))
                Console.WriteLine(line);
        }
    }
}
