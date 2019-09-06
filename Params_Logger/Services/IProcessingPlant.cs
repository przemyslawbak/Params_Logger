using Params_Logger.Models;
using System.Threading.Tasks;

namespace Params_Logger.Services
{
    public interface IProcessingPlant
    {
        Task SaveAndDisplay(string line, ConfigModel config);
    }
}