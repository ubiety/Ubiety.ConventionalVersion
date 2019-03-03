using Ubiety.VersionIt.Core.Version;
using Xunit;

namespace Ubiety.VersionIt.Test.Core.Version
{
    public class SemanticVersionTest
    {
        [Theory]
        [InlineData("1.2.3", 1, 2, 3, null, null)]
        [InlineData("1.2.3-beta", 1, 2, 3, "beta", null)]
        [InlineData("v1.2.3", 1, 2, 3, null, "[vV]")]
        public void ValidateParsing(string version, int major, int minor, int patch, string tag, string tagPrefix)
        {
            Assert.True(SemanticVersion.TryParse(version, tagPrefix, out var semanticVersion));
            Assert.Equal(major, semanticVersion.Major);
            Assert.Equal(minor, semanticVersion.Minor);
            Assert.Equal(patch, semanticVersion.Patch);
            Assert.Equal(tag, semanticVersion.PreRelease);
        }
    }
}
