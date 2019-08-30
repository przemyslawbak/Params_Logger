using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Params_Logger.Model
{
    public class LogModel
    {
        public string MethodName { get; set; }
        public object[] Arguments { get; set; }
        public DateTime Date { get; set; }
        public MethodBase Method { get; set; }
    }
}
