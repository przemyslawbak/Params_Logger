using System;

namespace Params_Logger
{
    public sealed class ParamsLogger : IParamsLogger
    {
        static readonly LogFactory factory = new LogFactory();
        public static ParamsLogger LogInstance
        {
            get { return _lazyInstance.Value; }
        }

        private static readonly Lazy<ParamsLogger> _lazyInstance = new Lazy<ParamsLogger>(() => new ParamsLogger());

        public Logger GetLogger()
        {
            return factory.GetLogger();
        }
    }
}
