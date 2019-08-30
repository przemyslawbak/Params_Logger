using System.Collections.Generic;
using System.Threading.Tasks;

namespace Params_Logger.Services
{
    public interface IFileService
    {
        List<string> ReadFileLines(string file);
        Task SaveLogAsync(string line, string _logFile);
        void DeleteFile(string logFile);
    }
}