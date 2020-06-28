using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine
{
    public delegate void LogCallback(string s);
    static public class Log
    {
        // types of messages
        [Flags]
        public enum LogOption { NONE = 0, ERROR = 1, WARNING = 2, INFO = 4, ALL = 7 }

        // a single log entry type
        struct LogData
        {
            public LogOption option;
            public string data;

            public LogData(LogOption o, string d) { option = o; data = d; }
        }

        static public LogCallback OnLog;
        static LogOption option = LogOption.NONE;                 // message types which will be written to a file
        static List<LogData> log_list = new List<LogData>();      // messages stored here

        // these three are utility functions

        // logs message of type INFO
        static public void Info(string d)
        {
            log_list.Add(new LogData(LogOption.INFO, d));
            System.Diagnostics.Debug.WriteLine("[INFO] " + d);
            OnLog?.Invoke(d);
        }

        // logs message of type WARNING
        static public void Warning(string d)
        {
            log_list.Add(new LogData(LogOption.WARNING, d));
            System.Diagnostics.Debug.WriteLine("[WARNING] " + d);
            OnLog?.Invoke(d);
        }

        // logs message of type ERROR
        static public void Error(string d)
        {
            log_list.Add(new LogData(LogOption.ERROR, d));
            System.Diagnostics.Debug.WriteLine("[ERROR] " + d);
            OnLog?.Invoke(d);
        }

        // allows messages of selected type to show in log file
        static public void SetOption(LogOption o)
        {
            option |= o;
        }

        // saves all messages to the file
        static public void SaveLog(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                foreach (LogData ld in log_list)
                {
                    if ((ld.option & option) != 0)
                        sw.WriteLine("[" + ld.option.ToString() + "] " + ld.data);
                }
            }
            fs.Close();
        }
    }
}
