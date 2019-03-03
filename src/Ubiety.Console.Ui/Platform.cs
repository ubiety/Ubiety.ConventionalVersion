using System;
using System.Drawing;
using Colorful;

namespace Ubiety.Console
{
    /// <summary>
    ///     Platform implementation.
    /// </summary>
    public class Platform : IPlatform
    {
        /// <inheritdoc />
        public VerbosityLevel Verbosity { get; set; }

        /// <inheritdoc />
        public void Exit(int exitCode)
        {
            Environment.Exit(exitCode);
        }

        /// <inheritdoc />
        public void WriteLine(string message, Color color, Formatter[] formatters = null)
        {
            if (Verbosity == VerbosityLevel.Silent)
            {
                return;
            }

            if (formatters is null)
            {
                Colorful.Console.WriteLine(message, color);
            }
            else
            {
                Colorful.Console.WriteLineFormatted(message, color, formatters);
            }
        }

        /// <summary>
        ///     Write a line to the console.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <param name="formatters">Formatters to use on the message.</param>
        public void WriteLine(string message, Formatter[] formatters = null)
        {
            WriteLine(message, Color.Empty, formatters);
        }
    }
}
