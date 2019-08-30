using System;
using System.Reflection;

namespace Params_Logger.Models
{
    public class LogModel
    {
        public string MethodName { get; set; }
        public object[] Arguments { get; set; }
        public DateTime Date { get; set; }
        public MethodBase Method { get; set; }
    }
}
