using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Params_Logger
{
    public interface IParamsLogger
    {
        Logger GetLogger();
    }
}