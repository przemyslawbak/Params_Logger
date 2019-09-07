using System;

namespace Params_Logger
{
    public class ParamsLogger : IParamsLogger
    {
        public static ParamsLogger LogInstance
        {
            get { return _lazyInstance.Value; }
        }

        private ILogFactory _factory;
        public ILogFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new LogFactory();
                }

                return _factory;
            }
            set { _factory = value; }
        }

        private static readonly Lazy<ParamsLogger> _lazyInstance = new Lazy<ParamsLogger>(() => new ParamsLogger());

        public ILogger GetLogger()
        {
            return Factory.GetLogger();
        }
    }
}
