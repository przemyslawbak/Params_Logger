using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Params_Logger.Models;

namespace Params_Logger.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IFileService _fileService;

        //config defaults
        private readonly string _logFileDefaults = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "log.txt");
        private readonly bool _debugOnlyDefaults = true;
        private readonly bool _deleteLogsDefaults = true;

        //config variables
        private string _logFile;
        private bool _debugOnly;
        private bool _executeOnDebugSettings;
        private bool _deleteLogs;

        public ConfigService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public ConfigModel GetConfig()
        {
            string path = GetLogConfigPath();

            if (string.IsNullOrEmpty(path))
            {
                _logFile = _logFileDefaults;
                _debugOnly = _debugOnlyDefaults;
                _deleteLogs = _deleteLogsDefaults;
            }
            else
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = path;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

                _logFile = GetLogPath(config);
                _debugOnly = GetDebugOnlyPath(config);
                _deleteLogs = GetDeleteLogs(config);
            }

            _executeOnDebugSettings = GetOnDebugSettings();

            return new ConfigModel()
            {
                DebugOnly = _debugOnly,
                DeleteLogs = _deleteLogs,
                ExecuteOnDebugSettings = _executeOnDebugSettings,
                LogFile = _logFile
            };
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

            return isParsed ? Convert.ToBoolean(value) : _deleteLogsDefaults;
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
