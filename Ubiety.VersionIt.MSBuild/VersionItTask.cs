using Microsoft.Build.Utilities;

namespace Ubiety.VersionIt.MSBuild
{
    public class VersionItTask : ToolTask
    {
        protected override string ToolName => "versionit";

        protected override string GenerateFullPathToTool()
        {
            return $"dotnet ${ToolName}";
        }
    }
}
