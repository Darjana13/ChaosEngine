using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChaosEngine
{
    static class Logger
    {
        private static string logPath;
        public static void Initialize()
        {
            Initialize("Logs\\" + DateTime.Now.Date.ToString() + " - " + DateTime.Now.TimeOfDay.ToString() + ".txt");
        }
        public static void Initialize(string path)
        {
            string dirPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            File.WriteAllText(path, "");
            logPath = path;
        }
        public static void Log(string text)
        {
            LogRaw('[' + DateTime.Now.TimeOfDay.ToString() + "]: " + text);
        }
        public static void LogRaw(string text)
        {
            File.AppendAllText(logPath, text);
        }
    }
}
