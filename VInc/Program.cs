namespace VInc
{
    internal class Program
    {
        public static readonly string versionText = "1.0.0a";
        public static LoggingSystem loggingSystem = new LoggingSystem();

        static void Main(string[] args)
        {
            loggingSystem.PrintLog(LoggingSystem.LogLevel.Info, $"VInc version {versionText} started.");

            // Check if second argument is provided (Format string)
            if (args.Length > 1)
            {
                string formatString = args[1];
                loggingSystem.PrintLog(LoggingSystem.LogLevel.Info, $"Using format string: {formatString}");
                // Here you would add the logic to use the format string.
                // For now, we just log that we are using it.
            }
            else
            {
                loggingSystem.PrintLog(LoggingSystem.LogLevel.Warning, "No format string provided. Defaulting to standard format.");
            }

            // Read first argument. It is the relative path to the .rc file(s) seperated by a comma.
            if (args.Length > 0)
            {
                string[] rcFiles = args[0].Split(',');
                foreach (string rcFile in rcFiles)
                {
                    loggingSystem.PrintLog(LoggingSystem.LogLevel.Info, $"Processing .rc file: {rcFile.Trim()}");
                    // Here you would add the logic to process the .rc file.
                    // For now, we just log that we are processing it.
                }
            }
            else
            {
                loggingSystem.PrintLog(LoggingSystem.LogLevel.Fatal, "No .rc files provided. Please provide a comma-separated list of .rc files.");
                Environment.Exit(1);
            }

            loggingSystem.PrintLog(LoggingSystem.LogLevel.Info, "VInc processing completed. Thank you for using VInc.");
        }
    }
}
