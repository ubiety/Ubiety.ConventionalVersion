using System.Drawing;
using Colorful;

namespace Ubiety.Console.Ui
{
    public enum VerbosityLevel
    {
        All,
        Silent
    }

    public interface IPlatform
    {
        VerbosityLevel Verbosity { get; set; }
        void Exit(int exitCode);
        void WriteLine(string message, Color color, Formatter[] formatters = null);
    }
}