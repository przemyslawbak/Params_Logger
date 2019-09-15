using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Params_Logger.Models;

namespace Params_Logger.Services
{
    public class ConfigService : IConfigService
    {
        private IFileService _fileService;

        //config defaults
        private readonly string _logFileDefaults = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "log.txt");
        private readonly bool _debugOnlyDefaults = true;
        private readonly bool _deleteLogsDefaults = true;
        private readonly bool _fileLogDefaults = true;
        private readonly bool _consoleLogDefaults = true;
        private readonly bool _infoOnlygDefaults = true;

        //config variables
        private string _logFile;
        private bool _debugOnly;
        private bool _executeOnDebugSettings;
        private bool _deleteLogs;
        private bool _fileLog;
        private bool _consoleLog;
        private bool _infoOnly;

        public ConfigService()
        {
        }

        public ConfigModel GetConfig(IStringService stringService, IFileService fileService, IProcessingPlant processingPlant)
        {
            _fileService = fileService;

            string mainDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string path = GetLogConfigPath();

            if (string.IsNullOrEmpty(path))
            {
                _logFile = _logFileDefaults;
                _debugOnly = _debugOnlyDefaults;
                _deleteLogs = _deleteLogsDefaults;
                _fileLog = _fileLogDefaults;
                _consoleLog = _consoleLogDefaults;
                _infoOnly = _infoOnlygDefaults;
            }
            else
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = path;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

                _logFile = Path.Combine(mainDirectory, GetLogPath(config));
                _debugOnly = GetDebugOnlyPath(config);
                _deleteLogs = GetDeleteLogs(config);
                _fileLog = GetFileLog(config);
                _consoleLog = GetConsoleLog(config);
                _infoOnly = GetInfoOnly(config);
            }

            _executeOnDebugSettings = GetOnDebugSettings();

            return new ConfigModel()
            {
                DebugOnly = _debugOnly,
                DeleteLogs = _deleteLogs,
                ExecuteOnDebugSettings = _executeOnDebugSettings,
                LogFile = _logFile,
                ConsoleLog = _consoleLog,
                FileLog = _fileLog,
                InfoOnly = _infoOnly,
                StringService = stringService,
                FileService = fileService,
                ProcessingPlant = processingPlant
            };
        }

        /// <summary>
        /// gets setting for infoOnly from log.config or returns default
        /// </summary>
        /// <param name="config">app config</param>
        /// <returns>bool value</returns>
        private bool GetInfoOnly(Configuration config)
        {
            return GetBoolConfig("infoOnlyConsole", config);
        }

        /// <summary>
        /// gets setting for fileLog from log.config or returns default
        /// </summary>
        /// <param name="config">app config</param>
        /// <returns>bool value</returns>
        private bool GetConsoleLog(Configuration config)
        {
            return GetBoolConfig("consoleLog", config);
        }

        /// <summary>
        /// gets setting for consoleLog from log.config or returns default
        /// </summary>
        /// <param name="config">app config</param>
        /// <returns>bool value</returns>
        private bool GetFileLog(Configuration config)
        {
            return GetBoolConfig("fileLog", config);
        }

        /// <summary>
        /// setup debug settings
        /// </summary>
        /// <returns>bool _executeOnDebugSettings</returns>
        private bool GetOnDebugSettings()
        {
            return (!_debugOnly || (_debugOnly && Debugger.IsAttached)) ? true : false;
        }

        /// <summary>
        /// gets setting for deleteLogs from log.config or returns default
        /// </summary>
        /// <param name="config">app config</param>
        /// <returns>bool value</returns>
        private bool GetDeleteLogs(Configuration config)
        {
            return GetBoolConfig("deleteLogs", config);
        }

        /// <summary>
        /// gets setting for debugOnly from log.config or returns default
        /// </summary>
        /// <param name="config">app config</param>
        /// <returns>bool value</returns>
        private bool GetDebugOnlyPath(Configuration config)
        {
            return GetBoolConfig("debugOnly", config);
        }

        /// <summary>
        /// gets setting for logFile from log.config or returns default
        /// </summary>
        /// <param name="config">app config</param>
        /// <returns>string path value</returns>
        private string GetLogPath(Configuration config)
        {
            string logFile = config.AppSettings.Settings["logFile"].Value;

            return (!string.IsNullOrEmpty(logFile)) ? logFile : _logFileDefaults;
        }

        /// <summary>
        /// looking for log.config files, in the project folders
        /// </summary>
        /// <returns>file path</returns>
        private string GetLogConfigPath()
        {
            string newDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string[] files = Directory.GetFiles(newDirectory, "log.config", SearchOption.AllDirectories);

            if (files.Length > 0)
                return IterateAndGetPath(files);
            else
                return string.Empty;
        }

        /// <summary>
        /// generic DRY method for bool props
        /// </summary>
        /// <param name="propName">prop name</param>
        /// <param name="config">config object</param>
        /// <returns></returns>
        private bool GetBoolConfig(string propName, Configuration config)
        {
            string value = config.AppSettings.Settings[propName].Value;
            bool isParsed = bool.TryParse(value, out bool result);

            return isParsed ? result : _deleteLogsDefaults;
        }

        /// <summary>
        /// iterating file array, if any contain phrases: logFile, debugOnly, deleteLogs (all mandatory)
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private string IterateAndGetPath(string[] files)
        {
            bool ok = false;

            foreach (var file in files)
            {
                List<string> lines = _fileService.ReadFileLines(file);

                if (lines.Any(l => l.Contains("logFile")) && lines.Any(l => l.Contains("debugOnly")) && lines.Any(l => l.Contains("deleteLogs")))
                    ok = true;

                if (ok)
                {
                    return file;
                }
            }

            return string.Empty;
        }
    }
}
