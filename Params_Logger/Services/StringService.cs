using System;
using System.Reflection;
using System.Text;
using Params_Logger.Models;

namespace Params_Logger.Services
{
    //NOTE: not possible to get argument variables with reflection, best way is to use 'nameof': https://stackoverflow.com/a/2566177/11972985
    //NOTE: not possible to get parameter values with reflection: https://stackoverflow.com/a/1867496/11972985

    public class StringService : IStringService
    {
        /// <summary>
        /// interface inmplementation
        /// processing log object to get string data
        /// </summary>
        /// <param name="log">log object</param>
        public string GetStringAttributes(LogModel log)
        {
            string methodName = string.Empty;
            string className = string.Empty;
            ParameterInfo[] parameters = { };

            if (log.Method != null && log.Method.ReflectedType != null)
            {
                methodName = log.Method.Name;
                className = log.Method.ReflectedType.Name;
                parameters = log.Method.GetParameters();
            }

            log.MethodName = log.MethodName.ToUpper();

            return BuildLine(log.Date, log.MethodName, className, methodName, parameters, log.Arguments);
        }

        /// <summary>
        /// based on passed data builds string log data to be saved in file
        /// </summary>
        /// <param name="date">datetime of event</param>
        /// <param name="type">type of event</param>
        /// <param name="className">member name</param>
        /// <param name="methodName">method</param>
        /// <param name="parameters">passed parameters</param>
        /// <param name="arguments">passed arguments</param>
        /// <returns></returns>
        private string BuildLine(DateTime date, string type, string className, string methodName, ParameterInfo[] parameters, object[] arguments)
        {
            bool areParams = parameters.Length > 0;
            bool combineParamsArgs = parameters.Length == arguments.Length;
            bool areArgs = arguments.Length > 0;
            bool noArgs = arguments.Length == 0;
            bool typeProps = type == "PROP";
            bool typeCalled = type == "CALLED";
            bool typeInfo = type == "INFO";
            bool typeError = type == "ERROR";
            bool typeOther = !combineParamsArgs && !typeProps && !typeCalled && !typeInfo;

            string begin = "";
            string param = "";
            string separator = "";
            string argum = "";

            begin = BuildBegin(date, type, className, methodName);

            if (typeCalled && areParams && combineParamsArgs) //CALLED
            {
                param = BuildCalled(parameters, arguments);
            }
            else if (typeCalled && !areParams) //CALLED
            {
                param = "()";
            }
            else if (typeProps) //PROP
            {
                param = BuildProp(arguments);
            }
            else if (typeInfo || typeError) //INFO & ERROR
            {
                param = BuildInfoError(arguments);
            }
            else if (areParams && !combineParamsArgs && !typeCalled) //all OTHER
            {
                param = BuildOther(arguments, parameters);
            }

            if (!typeCalled && !typeProps && !typeInfo && !typeError)
            {
                separator = "|";
            }

            if (areArgs && typeOther) //all OTHER
            {
                argum = BuildArguments(arguments);
            }
            else if (noArgs && typeOther) //none
            {
                argum = "none";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(begin);
            sb.Append(param);
            sb.Append(separator);
            sb.Append(argum);

            return sb.ToString();
        }

        /// <summary>
        /// building arguments of the string
        /// for other types of event
        /// </summary>
        /// <returns>string data</returns>
        private string BuildArguments(object[] arguments)
        {
            StringBuilder sb = new StringBuilder();

            for (var i = 0; i < arguments.Length; i++)
            {
                sb.Append("(");
                sb.Append(arguments[i].GetType().Name);
                sb.Append(")");
                sb.Append("=");
                sb.Append(arguments[i].ToString());
                if (i < arguments.Length - 1)
                    sb.Append("; ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// building parameters of the string
        /// for other types of event
        /// </summary>
        /// <returns>string data</returns>
        private string BuildOther(object[] arguments, ParameterInfo[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(");
            for (var i = 0; i < parameters.Length; i++)
            {
                sb.Append(parameters[i].ParameterType.Name);
                sb.Append(" ");
                sb.Append(parameters[i].Name);
                if (i < parameters.Length - 1)
                    sb.Append(", ");
            }
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// building parameters of the string
        /// for info types of event
        /// </summary>
        /// <returns>string data</returns>
        private string BuildInfoError(object[] arguments)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append("(");
            sb.Append(arguments[0]);
            sb.Append(")");
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// building parameter of the string
        /// for prop types of event
        /// </summary>
        /// <returns>string data</returns>
        private string BuildProp(object[] arguments)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append(arguments[1]);
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// building parameters of the string
        /// for call types of event
        /// </summary>
        /// <returns>string data</returns>
        private string BuildCalled(ParameterInfo[] parameters, object[] arguments)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            for (var i = 0; i < parameters.Length; i++)
            {
                sb.Append("(");
                sb.Append(parameters[i].ParameterType.Name);
                sb.Append(")");
                sb.Append(parameters[i].Name);
                if (!string.IsNullOrEmpty(arguments[i].ToString()))
                    sb.Append("=");
                sb.Append(arguments[i].ToString());
                if (i < parameters.Length - 1)
                    sb.Append(", ");
            }
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// building first part of the string containig time, class name, method name
        /// for other types of event
        /// </summary>
        /// <returns>string data</returns>
        private string BuildBegin(DateTime date, string type, string className, string methodName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(date.ToLongTimeString());
            sb.Append(".");
            sb.Append(date.Millisecond.ToString());
            sb.Append("|");
            sb.Append(type);
            sb.Append("|");
            sb.Append(className);
            sb.Append("|");
            sb.Append(methodName);

            return sb.ToString();
        }
    }
}
