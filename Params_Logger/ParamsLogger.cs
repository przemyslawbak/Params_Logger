using Autofac;
using Params_Logger.Models;
using Params_Logger.Services;
using Params_Logger.Startup;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

namespace Params_Logger
{
    public class ParamsLogger : IParamsLogger, IAsyncInitialization
    {
        private readonly IFileService _fileService;
        private readonly IStringService _stringService;
        private readonly IConfigService _configService;
        private Timer _savingTimer;
        private AppDomain _currentDomain;

        //config fields
        private string _logFile;
        private bool _debugOnly;
        private bool _executeOnDebugSettings;
        private bool _deleteLogs;

        public ParamsLogger(IFileService fileService, IStringService stringService, IConfigService configService)
        {
            _fileService = fileService;
            _stringService = stringService;

            LogList = new List<LogModel>();

            Initialization = RunConfigAsync();
        }

        public List<LogModel> LogList { get; set; } //list of added logs
        public Task Initialization { get; set; } //async init Task

        private ConfigModel _config;
        public ConfigModel Config
        {
            get => _config;
            set
            {
                _config = value;
                _logFile = _config.LogFile;
                _debugOnly = _config.DebugOnly;
                _executeOnDebugSettings = _config.ExecuteOnDebugSettings;
                _deleteLogs = _config.DeleteLogs;
            }
        }

        /// <summary>
        /// configuration method, loading variables from log.config
        /// if file not found, setting up defaults
        /// </summary>
        private async Task RunConfigAsync()
        {
            IContainer container = BootStrapper.BootStrap();
            _currentDomain = AppDomain.CurrentDomain;

            ConfigModel config = _configService.GetConfig();

            if (_deleteLogs)
            {
                _fileService.DeleteFile(_logFile);
            }

            if (_executeOnDebugSettings) // if on DEBUG
            {
                await RunTimerAsync();

                UnhandledExceptionsHandler();
            }
        }

        /// <summary>
        /// handling unhandled fatal exceptions
        /// </summary>
        private void UnhandledExceptionsHandler()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledExceptionAsync);
        }

        /// <summary>
        /// unhandled exception event
        /// </summary>
        async void OnUnhandledExceptionAsync(object sender, UnhandledExceptionEventArgs args)
        {
            Error(string.Format("Runtime terminating: {0}", args.IsTerminating));

            await ProcessLogList();
        }

        /// <summary>
        /// starts timer when saving logs will be temporary suspended
        /// when timer is stopped, ProcessLogList is called for processing and saving logs
        /// after this timer is restarted
        /// </summary>
        private async Task RunTimerAsync()
        {
            _savingTimer = new Timer();

            if ((!_savingTimer.Enabled || _savingTimer == null) && LogList.Count > 0 && !string.IsNullOrEmpty(_logFile))
            {
                await ProcessLogList();
            }
            _savingTimer = new Timer();
            _savingTimer.Elapsed += new ElapsedEventHandler(ResetTimerAsync);
            _savingTimer.Interval = 10;
            _savingTimer.Start();
        }

        /// <summary>
        /// loops back to timer start, disables _savingTimer.Enabled property
        /// </summary>
        private async void ResetTimerAsync(object sender, EventArgs e)
        {
            _savingTimer.Enabled = false;

            await RunTimerAsync();
        }

        /// <summary>
        /// created new log list for iteration and clears previous collection
        /// iterates new collection to process items
        /// </summary>
        /// <returns></returns>
        private async Task ProcessLogList()
        {
            List<LogModel> iterateMe = LogList.ToList();
            LogList.Clear();

            foreach (LogModel item in iterateMe)
            {
                string line = _stringService.GetStringAttributes(item);

                await _fileService.SaveLogAsync(line, _logFile);
            }
        }

        /// <summary>
        /// interface implementation
        /// can be called from property setter
        /// </summary>
        /// <param name="value">property value</param>
        /// <param name="propertyName">property name</param>
        public void Prop(object value, [CallerMemberName] string propertyName = null)
        {
            if (_executeOnDebugSettings) // if on DEBUG
            {
                DateTime date = DateTime.Now;

                object[] arguments = { propertyName, value };

                MethodBase callingMethod = GetMethod(new StackTrace().GetFrame(1));

                LogList.Add(new LogModel { MethodName = nameof(Prop), Arguments = arguments, Date = date, Method = callingMethod });
            }
        }

        /// <summary>
        /// interface implementation
        /// can be called from methods
        /// </summary>
        /// <param name="arguments">array of passed arguments</param>
        public void Called(params object[] arguments)
        {
            if (_executeOnDebugSettings) // if on DEBUG
            {
                DateTime date = DateTime.Now;

                MethodBase callingMethod = GetMethod(new StackTrace().GetFrame(1));

                LogList.Add(new LogModel { MethodName = nameof(Called), Arguments = arguments, Date = date, Method = callingMethod });
            }
        }

        /// <summary>
        /// interface implementation
        /// can be called from methods
        /// </summary>
        /// <param name="arguments">array of passed arguments</param>
        public void Ended(params object[] arguments)
        {
            if (_executeOnDebugSettings) // if on DEBUG
            {
                DateTime date = DateTime.Now;

                MethodBase callingMethod = GetMethod(new StackTrace().GetFrame(1));

                LogList.Add(new LogModel { MethodName = nameof(Ended), Arguments = arguments, Date = date, Method = callingMethod });
            }
        }

        /// <summary>
        /// interface implementation
        /// can be called to pass information about some app event
        /// </summary>
        /// <param name="arguments">string info</param>
        public void Info(string value)
        {
            if (_executeOnDebugSettings) // if on DEBUG
            {
                DateTime date = DateTime.Now;

                object[] arguments = { value };

                MethodBase callingMethod = GetMethod(new StackTrace().GetFrame(1));

                LogList.Add(new LogModel { MethodName = nameof(Info), Arguments = arguments, Date = date, Method = callingMethod });
            }
        }

        /// <summary>
        /// interface implementation
        /// can be called from catch to pass information about exception
        /// </summary>
        /// <param name="arguments">string info</param>
        public void Error(string value)
        {
            if (_executeOnDebugSettings) // if on DEBUG
            {
                DateTime date = DateTime.Now;

                object[] arguments = { value };

                MethodBase callingMethod = GetMethod(new StackTrace().GetFrame(1));

                LogList.Add(new LogModel { MethodName = nameof(Error), Arguments = arguments, Date = date, Method = callingMethod });
            }
        }

        /// <summary>
        /// gets MethodBase for passed stack frame
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private MethodBase GetMethod(StackFrame frame)
        {
            MethodBase callingMethod = frame.GetMethod();

            if (callingMethod != null && (callingMethod.Name == "MoveNext" || callingMethod.Name == "Run"))
            {
                callingMethod = GetRealMethodFromAsyncMethod(callingMethod);
            }

            return callingMethod;
        }

        /// <summary>
        /// in case of async method retrieved from the stack, method is searching for real name of the method
        /// </summary>
        /// <returns>founded method, or null in case if not found</returns>
        private static MethodBase GetRealMethodFromAsyncMethod(MethodBase asyncMethod)
        {
            try
            {
                var generatedType = asyncMethod.DeclaringType;
                var originalType = generatedType.DeclaringType;
                var matchingMethods =
                    from methodInfo in originalType.GetMethods()
                    let attr = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>()
                    where attr != null && attr.StateMachineType == generatedType
                    select methodInfo;

                // If this throws, the async method scanning failed.
                MethodInfo foundMethod = matchingMethods.Single();
                return foundMethod;
            }
            catch
            {
                return null;
            }
        }
    }
}
