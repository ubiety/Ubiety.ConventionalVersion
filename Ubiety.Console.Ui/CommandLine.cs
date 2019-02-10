using System.Drawing;
using Colorful;

namespace Ubiety.Console.Ui
{
    public static class CommandLine
    {
        public static IPlatform Platform { get; } = new Platform { Verbosity = VerbosityLevel.All };

        public static void Exit(string message, int exitCode)
        {
            Platform.WriteLine(message, Color.Red);
            Platform.Exit(exitCode);
        }

        public static void Information(string message)
        {
            Platform.WriteLine(message, Color.LightGray);
        }

        public static void Step(string message)
        {
            string stepMessage = "{0} {1}";
            var stepFormatters = new Formatter[]
            {
                new Formatter("√", Color.Green),
                new Formatter(message, Color.LightGray)
            };

            Platform.WriteLine(stepMessage, Color.White, stepFormatters);
        }
    }
}
