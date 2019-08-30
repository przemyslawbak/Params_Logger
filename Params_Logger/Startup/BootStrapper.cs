using Autofac;
using Params_Logger.Services;

namespace Params_Logger.Startup
{
    public class BootStrapper
    {
        public static IContainer BootStrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConfigService>()
              .As<IConfigService>().SingleInstance();

            builder.RegisterType<FileService>()
              .As<IFileService>().SingleInstance();

            builder.RegisterType<StringService>()
              .As<IStringService>().SingleInstance();

            return builder.Build();
        }
    }
}
