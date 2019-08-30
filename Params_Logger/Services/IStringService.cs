using System.Threading.Tasks;
using Params_Logger.Models;

namespace Params_Logger.Services
{
    public interface IStringService
    {
        string GetStringAttributes(LogModel item);
    }
}