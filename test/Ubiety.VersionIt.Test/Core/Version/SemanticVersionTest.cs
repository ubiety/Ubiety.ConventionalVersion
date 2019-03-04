using Shouldly;
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
            SemanticVersion.TryParse(version, tagPrefix, out var semanticVersion).ShouldBeTrue();
            major.ShouldBe(semanticVersion.Major);
            minor.ShouldBe(semanticVersion.Minor);
            patch.ShouldBe(semanticVersion.Patch);
            tag.ShouldBe(semanticVersion.PreRelease);
        }

        [Theory]
        [InlineData("testing")]
        [InlineData("test.123.4")]
        public void InvalidParsing(string version)
        {
            SemanticVersion.TryParse(version, null, out _).ShouldBeFalse();
        }

        [Fact]
        public void VersionSorting()
        {
            SemanticVersion.Parse("1.0.0", null).ShouldBeGreaterThan(SemanticVersion.Parse("1.0.0-beta", null));
            SemanticVersion.Parse("1.2.0", null).ShouldBeGreaterThan(SemanticVersion.Parse("1.1.0", null));
            SemanticVersion.Parse("1.0.0-beta.1", null).ShouldBeLessThan(SemanticVersion.Parse("1.0.0-beta.2", null));
        }
    }
}
