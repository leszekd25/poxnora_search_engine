using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine
{
    public delegate void LogCallback(Log.LogOption o, Log.LogSource s, string d);
    static public class Log
    {
        // types of messages
        [Flags]
        public enum LogOption { NONE = 0, ERROR = 1, WARNING = 2, INFO = 4, ALL = 7 }

        // sources of messages
        public enum LogSource { PoxDB, Net, UI, Utility, Main, _UNK }

        // a single log entry type
        struct LogData
        {
            public DateTime time;
            public LogOption option;
            public LogSource source;
            public string data;

            public LogData(DateTime t, LogOption o, LogSource s, string d) { time = t;  option = o; source = s; data = d; }

            public override string ToString()
            {
                return time.ToString()+": [" + option.ToString() + "] " + source.ToString() + ": " + data;
            }
        }

        static public LogCallback OnLog;
        static LogOption option = LogOption.ALL;                 // message types which will be written to a file
        static List<LogData> log_list = new List<LogData>();      // messages stored here

        // these three are utility functions

        // logs message of type INFO
        static public void Info(LogSource s, string d)
        {
            log_list.Add(new LogData(DateTime.Now, LogOption.INFO, s, d));
            OnLog?.Invoke(LogOption.INFO, s, d);
        }

        // logs message of type WARNING
        static public void Warning(LogSource s, string d)
        {
            log_list.Add(new LogData(DateTime.Now, LogOption.WARNING, s, d));
            OnLog?.Invoke(LogOption.WARNING, s, d);
        }

        // logs message of type ERROR
        static public void Error(LogSource s, string d)
        {
            log_list.Add(new LogData(DateTime.Now, LogOption.ERROR, s, d));
            OnLog?.Invoke(LogOption.ERROR, s, d);
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
                        sw.WriteLine(ld.ToString());
                }
            }
            fs.Close();
        }
    }
}
