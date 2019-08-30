using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Params_Logger.Services
{
    public class FileService : IFileService
    {
        /// <summary>
        /// deletes file on aplication start, if deleteLogs = true in log.config
        /// </summary>
        /// <returns>string data</returns>
        public void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// file reader
        /// </summary>
        /// <param name="file">file path</param>
        /// <returns>file lines collection</returns>
        public List<string> ReadFileLines(string file)
        {
            using (var reader = File.OpenText(file))
            {
                var fileText = reader.ReadToEnd();

                var array = fileText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                return new List<string>(array);
            }
        }

        /// <summary>
        /// saves async log event line
        /// </summary>
        public async Task SaveLogAsync(string line, string logFile) //DO NOT LOG -> makes endless loop!
        {
            try
            {
                using (TextWriter LineBuilder = new StreamWriter(logFile, true))
                {
                    await LineBuilder.WriteLineAsync(line);
                }
            }
            catch
            {
                await Task.Delay(5);
                await SaveLogAsync(line + " <--DELAYED", logFile);
            }
        }
    }
}
