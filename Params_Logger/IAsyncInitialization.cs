using System.Threading.Tasks;

namespace Params_Logger
{
    public interface IAsyncLoggerInit
    {
        Task GetLogger { get; }
    }
}
