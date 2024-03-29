﻿using Params_Logger.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Params_Logger
{
    public class Logger : ILogger
    {
        private readonly object lockObject = new object();
        private readonly ConfigModel _config;

        public Logger(ConfigModel config)
        {
            _config = config;
        }

        public LogModel NewLog { get; set; }

        /// <summary>
        /// if _clock not assigned in unit test, gets DateTime.Now and resets property
        /// </summary>
        private DateTime _clock;
        public DateTime ClockNow
        {
            get
            {
                if (_clock == new DateTime())
                {
                    _clock = DateTime.Now;
                }

                DateTime toReturn = _clock;

                ClockNow = new DateTime();

                return toReturn;
            }
            set { _clock = value; }
        }

        /// <summary>
        /// interface implementation
        /// can be called from property setter
        /// </summary>
        /// <param name="value">property value</param>
        /// <param name="propertyName">property name</param>
        public void Prop(object value, [CallerMemberName] string propertyName = null)
        {
            if (_config.ExecuteOnDebugSettings) // if on DEBUG
            {
                DateTime date = ClockNow;

                object[] arguments = { propertyName, value };

                MethodBase callingMethod = GetMethod(new StackTrace().GetFrame(1));

                ProcessNewLogAsync(new LogModel { MethodName = nameof(Prop), Arguments = arguments, Date = date, Method = callingMethod }).Wait();
            }
        }

        /// <summary>
        /// interface implementation
        /// can be called from methods
        /// </summary>
        /// <param name="arguments">array of passed arguments</param>
        public void Called(params object[] arguments)
        {
            if (_config.ExecuteOnDebugSettings) // if on DEBUG
            {
                DateTime date = ClockNow;

                MethodBase callingMethod = GetMethod(new StackTrace().GetFrame(1));

                ProcessNewLogAsync(new LogModel { MethodName = nameof(Called), Arguments = arguments, Date = date, Method = callingMethod }).Wait();
            }
        }

        /// <summary>
        /// interface implementation
        /// can be called to pass information about some app event
        /// </summary>
        /// <param name="arguments">string info</param>
        public void Info(string value)
        {
            if (_config.ExecuteOnDebugSettings) // if on DEBUG
            {
                DateTime date = ClockNow;

                object[] arguments = { value };

                MethodBase callingMethod = GetMethod(new StackTrace().GetFrame(1));

                ProcessNewLogAsync(new LogModel { MethodName = nameof(Info), Arguments = arguments, Date = date, Method = callingMethod }).Wait();
            }
        }

        /// <summary>
        /// interface implementation
        /// can be called from catch to pass information about exception
        /// </summary>
        /// <param name="arguments">string info</param>
        public void Error(string value)
        {
            if (_config.ExecuteOnDebugSettings) // if on DEBUG
            {
                DateTime date = ClockNow;

                object[] arguments = { value };

                MethodBase callingMethod = GetMethod(new StackTrace().GetFrame(1));

                ProcessNewLogAsync(new LogModel { MethodName = nameof(Error), Arguments = arguments, Date = date, Method = callingMethod }).Wait();
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

        /// <summary>
        /// called in NewLog property setter
        /// </summary>
        /// <param name="newLog">LogModel to be processed</param>
        private async Task ProcessNewLogAsync(LogModel newLog)
        {
            NewLog = newLog;

            string line = _config.StringService.GetStringAttributes(newLog);

            await _config.ProcessingPlant.SaveAndDisplay(line, _config);
        }
    }
}
