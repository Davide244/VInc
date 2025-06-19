using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace VInc
{
    public class LoggingSystem
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Success,
            Warning,
            Error,
            Fatal
        }

        public struct LogEntry
        {
            public LogLevel Level;
            public string Message;
            public DateTime Timestamp;
        }

        //public SynchronizedCollection<LogEntry> LogEntries = new SynchronizedCollection<LogEntry>();

        public LoggingSystem()
        {
            PrintLogo();

            //LogEntries = new SynchronizedCollection<LogEntry>();
            //LogEntries.Add(new LogEntry { Level = LogLevel.Info, Message = "Logging system initialized", Timestamp = DateTime.Now });

            PrintLog(LogLevel.Info, "Logging system initialized");

            // Print all log levels:
            //PrintLog(LogLevel.Debug, "Debug message");
            //PrintLog(LogLevel.Info, "Info message");
            //PrintLog(LogLevel.Success, "Success message");
            //PrintLog(LogLevel.Warning, "Warning message");
            //PrintLog(LogLevel.Error, "Error message");
            //PrintLog(LogLevel.Fatal, "Fatal message");
        }

        public void PrintLogo()
        {
            Console.WriteLine(
                @"
 /$$    /$$ /$$$$$$                    
| $$   | $$|_  $$_/                    
| $$   | $$  | $$   /$$$$$$$   /$$$$$$$
|  $$ / $$/  | $$  | $$__  $$ /$$_____/
 \  $$ $$/   | $$  | $$  \ $$| $$      
  \  $$$/    | $$  | $$  | $$| $$      
   \  $/    /$$$$$$| $$  | $$|  $$$$$$$
    \_/    |______/|__/  |__/ \_______/
");

            Console.WriteLine($"VInc | {Program.versionText} | MIT\n");
        }

        public void PrintLog(LogLevel level, string message)
        {
            // LOG MESSAGE STRUCTURE:
            // [LOGLEVEL / HH:MM:SS]: MESSAGE
            // COLOR OF [LOGLEVEL / HH:MM:SS]: IS "S"
            // COLOR OF MESSAGE: IS "E"

            // COLOR FORMATS:
            // Debug: S = Gray, E = White
            // Info: S = Cyan, E = White
            // Success: S = lGreen, E = White
            // Warning: S = Yellow, E = White
            // Error: S = Red, E = White
            // Fatal: S = White, E = White ---- THIS ONE HAS A RED BACKGROUND AND WHITE TEXT

            string logLevelString = level switch
            {
                LogLevel.Debug => "DEBUG",
                LogLevel.Info => "INFO ",
                LogLevel.Success => "GOOD ",
                LogLevel.Warning => "WARN ",
                LogLevel.Error => "ERROR",
                LogLevel.Fatal => "FATAL",
                _ => "Unknown"
            };

            ConsoleColor sColor = level switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info => ConsoleColor.Cyan,
                LogLevel.Success => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Fatal => ConsoleColor.White,
                _ => ConsoleColor.White
            };

            ConsoleColor eColor = ConsoleColor.White;

            if (level == LogLevel.Fatal)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ForegroundColor = sColor;
            Console.Write($"[{logLevelString} | {DateTime.Now:HH:mm:ss}]: ");
            Console.ForegroundColor = eColor;
            Console.WriteLine(message);

            Console.ResetColor();
        }
    }
}
