using JoySlots_WPF.Services.Interfaces;

namespace JoySlots_WPF.Services
{
    public class Logger : ILogger
    {
        public bool IsDebugActive { get; private set; }
        public bool ShowTimestamps { get; set; }
        private enum LogMessageTypes
        {
            Classic = 0,
            Info,
            Warning,
            Error,
            Fatal
        }

        public Logger(bool IsDebuggingActive)
        {
            IsDebugActive = IsDebuggingActive;
        }

        private void PrintMessageLog(string WhoSends, string message, LogMessageTypes messageType)
        {
            if (!IsDebugActive) return;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[");
            switch (messageType)
            {
                case LogMessageTypes.Classic:
                    Console.ResetColor();
                    Console.Write("Log");
                    break;
                case LogMessageTypes.Info:
                    Console.ForegroundColor= ConsoleColor.Green;
                    Console.Write("Info");
                    break;
                case LogMessageTypes.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Warning");
                    break;
                case LogMessageTypes.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Error");
                    break;
                case LogMessageTypes.Fatal:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("FATAL");
                    break;
                default:
                    Console.ResetColor();
                    Console.Write("Log");
                    break;
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(" # ");
            Console.ResetColor();
            Console.Write($"<{WhoSends}>");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(" - ");
            Console.ResetColor();
            Console.Write($"({DateTime.Now:HH:mm:ss:fff})");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("]\n\t");
            Console.ResetColor();
            Console.WriteLine($"{message}\n");
        }

        public void Log(string WhoSends, string Message, params object[] args)
        {
            string format = Message;
            if (args.Length > 0)
                format = string.Format(Message, args);
            PrintMessageLog(WhoSends, format, LogMessageTypes.Classic);
        }

        public void LogInfo(string WhoSends, string Message, params object[] args)
        {
            string format = Message;
            if (args.Length > 0)
                format = string.Format(Message, args);
            PrintMessageLog(WhoSends, format, LogMessageTypes.Info);
        }

        public void LogError(string WhoSends, string Message, params object[] args)
        {
            string format = Message;
            if (args.Length > 0)
                format = string.Format(Message, args);
            PrintMessageLog(WhoSends, format, LogMessageTypes.Error);
        }

        public void LogFatal(string WhoSends, string Message, params object[] args)
        {
            string format = Message;
            if (args.Length > 0)
                format = string.Format(Message, args);
            PrintMessageLog(WhoSends, format, LogMessageTypes.Fatal);
        }

        public void LogWarning(string WhoSends, string Message, params object[] args)
        {
            string format = Message;
            if (args.Length > 0)
                format = string.Format(Message, args);
            PrintMessageLog(WhoSends, format, LogMessageTypes.Warning);
        }
    }
}
