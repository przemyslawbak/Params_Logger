using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Params_Logger
{
    public interface IParamsLogger : IAsyncLoggerInit
    {
        void Called(params object[] values);
        void Ended(params object[] values);
        void Info(string value);
        void Error(string value);
        void Prop(object value, [CallerMemberName] string propertyName = null);
        Task GetLogger { get; set; }
    }
}