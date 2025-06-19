namespace VInc
{
    internal class Program
    {
        public static readonly string versionText = "1.0.0a";
        public static LoggingSystem loggingSystem = new LoggingSystem();

        public static string formatString = "x.x.x.+";

        static void Main(string[] args)
        {       
            loggingSystem.PrintLog(LoggingSystem.LogLevel.Info, $"VInc version {versionText} started.");

            // Check if second argument is provided (Format string)
            if (args.Length > 1)
            {
                string formatStringIn = args[1];
                // Check format string with regex
                if (System.Text.RegularExpressions.Regex.IsMatch(formatStringIn, @"^(?:x|\+|YYYY|MM|DD|hh|mm)(?:[.,](?:x|\+|YYYY|MM|DD|hh|mm)){3}$"))
                {
                    formatString = formatStringIn;
                }
                else
                {
                    loggingSystem.PrintLog(LoggingSystem.LogLevel.Fatal, "Invalid format string provided.");
                    Environment.Exit(1);
                }
                loggingSystem.PrintLog(LoggingSystem.LogLevel.Info, $"Using format string: {formatString}");
            }
            else
            {
                loggingSystem.PrintLog(LoggingSystem.LogLevel.Warning, $"No format string provided. Defaulting to standard format. ({formatString})");
            }

            // Read first argument. It is the relative path to the .rc file(s) separated by a comma.
            if (args.Length > 0)
            {
                string[] rcFiles = args[0].Split(',');
                foreach (string rcFile in rcFiles)
                {
                    string trimmedFile = rcFile.Trim();
                    if (!string.IsNullOrEmpty(trimmedFile))
                    {
                        loggingSystem.PrintLog(LoggingSystem.LogLevel.Info, $"Processing .rc file: {trimmedFile}");
                        // Check if file exists
                        if (System.IO.File.Exists(trimmedFile))
                        {
                            // VS_VERSION_INFO from .rc file
                            try
                            {
                                // Read the file content
                                string fileContent = System.IO.File.ReadAllText(trimmedFile);

                                // Read the VS_VERSION_INFO from the file
                                if (fileContent.Contains("VS_VERSION_INFO"))
                                {
                                    // Extract the VS_VERSION_INFO section
                                    int startIndex = fileContent.IndexOf("VS_VERSION_INFO");
                                    int endIndex = fileContent.IndexOf("END", startIndex);
                                    string versionInfo = fileContent.Substring(startIndex, endIndex - startIndex);
                                    // Process the version info
                                    string updatedVersionInfo = ProcessVersionInfo(versionInfo);

                                    // Write the updated version info back to the file
                                    fileContent = fileContent.Replace(versionInfo, updatedVersionInfo);
                                    System.IO.File.WriteAllText(trimmedFile, fileContent);
                                    loggingSystem.PrintLog(LoggingSystem.LogLevel.Success, $"Successfully updated version info in {trimmedFile}");
                                }
                                else
                                {
                                    loggingSystem.PrintLog(LoggingSystem.LogLevel.Warning, $"No VS_VERSION_INFO found in {trimmedFile}");
                                }
                            }
                            catch (Exception ex)
                            {
                                loggingSystem.PrintLog(LoggingSystem.LogLevel.Error, $"Error processing {trimmedFile}: {ex.Message}");
                            }
                        }
                        else
                        {
                            loggingSystem.PrintLog(LoggingSystem.LogLevel.Error, $"File not found: {trimmedFile}");
                        }
                    }
                }
            }
            else
            {
                loggingSystem.PrintLog(LoggingSystem.LogLevel.Fatal, "No .rc files provided. Please provide a comma-separated list of .rc files.");
                Environment.Exit(1);
            }

            loggingSystem.PrintLog(LoggingSystem.LogLevel.Info, "VInc processing completed. Thank you for using VInc.");
        }

        public static string ProcessVersionInfo(string versionInfo)
        {
            // Read FILEVERSION and PRODUCTVERSION
            var fileVersionMatch = System.Text.RegularExpressions.Regex.Match(versionInfo, @"FILEVERSION\s+(\d+),\s*(\d+),\s*(\d+),\s*(\d+)");
            var productVersionMatch = System.Text.RegularExpressions.Regex.Match(versionInfo, @"PRODUCTVERSION\s+(\d+),\s*(\d+),\s*(\d+),\s*(\d+)");

            // Parse version numbers
            int[] versionInts = new int[4];
            if (fileVersionMatch.Success)
            {
                for (int i = 1; i <= 4; i++)
                    int.TryParse(fileVersionMatch.Groups[i].Value, out versionInts[i - 1]);
            }

            // Apply the version format
            int[] formattedVersion = ApplyVersionFormat(versionInts);

            // Print the formatted version
            loggingSystem.PrintLog(LoggingSystem.LogLevel.Info, $"Update version from {versionInts[0]}.{versionInts[1]}.{versionInts[2]}.{versionInts[3]} to {formattedVersion[0]}.{formattedVersion[1]}.{formattedVersion[2]}.{formattedVersion[3]}");

            // Prepare new version strings
            string updatedFileVersion = $"FILEVERSION {formattedVersion[0]},{formattedVersion[1]},{formattedVersion[2]},{formattedVersion[3]}";
            string updatedProductVersion = $"PRODUCTVERSION {formattedVersion[0]},{formattedVersion[1]},{formattedVersion[2]},{formattedVersion[3]}";
            string updatedFileVersionStr = $"VALUE \"FileVersion\", \"{formattedVersion[0]},{formattedVersion[1]},{formattedVersion[2]},{formattedVersion[3]}\"";
            string updatedProductVersionStr = $"VALUE \"ProductVersion\", \"{formattedVersion[0]},{formattedVersion[1]},{formattedVersion[2]},{formattedVersion[3]}\"";

            // Replace all occurrences in the versionInfo block
            string updatedVersionInfo = versionInfo;
            // Replace FILEVERSION and PRODUCTVERSION lines
            updatedVersionInfo = System.Text.RegularExpressions.Regex.Replace(updatedVersionInfo, @"FILEVERSION\s+\d+,\s*\d+,\s*\d+,\s*\d+", updatedFileVersion);
            updatedVersionInfo = System.Text.RegularExpressions.Regex.Replace(updatedVersionInfo, @"PRODUCTVERSION\s+\d+,\s*\d+,\s*\d+,\s*\d+", updatedProductVersion);
            // Replace VALUE "FileVersion" and VALUE "ProductVersion" lines
            updatedVersionInfo = System.Text.RegularExpressions.Regex.Replace(updatedVersionInfo, @"VALUE\s+""FileVersion"",\s*""[^""]*""", updatedFileVersionStr);
            updatedVersionInfo = System.Text.RegularExpressions.Regex.Replace(updatedVersionInfo, @"VALUE\s+""ProductVersion"",\s*""[^""]*""", updatedProductVersionStr);
            return updatedVersionInfo;
        }
            
        public static int[] ApplyVersionFormat(int[] versionInts) 
        {
            int[] newInts = new int[4];

            // Replace all , with . in formatString and then split by .
            string[] formatParts = formatString.Replace(',', '.').Split('.');

            for (int i = 0; i < 4; i++) 
            {
                switch (formatParts[i])
                {
                    case "x":
                        newInts[i] = versionInts[i];
                        break;
                    case "+":
                        newInts[i] = versionInts[i] + 1;
                        break;
                    case "YYYY":
                        newInts[i] = DateTime.Now.Year;
                        break;
                    case "MM":
                        newInts[i] = DateTime.Now.Month;
                        break;
                    case "DD":
                        newInts[i] = DateTime.Now.Day;
                        break;
                    case "hh":
                        newInts[i] = DateTime.Now.Hour;
                        break;
                    case "mm":
                        newInts[i] = DateTime.Now.Minute;
                        break;
                    default:
                        throw new ArgumentException($"Invalid format part: {formatParts[i]}");
                }
            }

            return newInts;
        }
    }
}
