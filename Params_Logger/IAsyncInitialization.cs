using System.Threading.Tasks;

namespace Params_Logger
{
    public interface IAsyncInitialization
    {
        Task TimerInitialization { get; }
    }
}
