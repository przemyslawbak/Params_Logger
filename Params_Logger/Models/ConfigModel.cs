using System;
using System.Collections.Generic;
using System.Text;

namespace Params_Logger.Models
{
    public class ConfigModel
    {
        public string LogFile { get; set; }
        public bool DebugOnly { get; set; }
        public bool DeleteLogs { get; set; }
        public bool ExecuteOnDebugSettings { get; set; }
        public bool FileLog { get; set; }
        public bool ConsoleLog { get; set; }
        public bool InfoOnly { get; set; }
    }
}
