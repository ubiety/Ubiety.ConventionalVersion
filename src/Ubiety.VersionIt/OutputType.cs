namespace Ubiety.VersionIt
{
    /// <summary>
    ///     Version output type.
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        ///     Standard console output (version and changelog).
        /// </summary>
        Standard,

        /// <summary>
        ///     Build server environment version variable output (No changelog).
        /// </summary>
        BuildServer,

        /// <summary>
        ///     Just version the project.
        /// </summary>
        Version,

        /// <summary>
        ///     Just generate the changelog.
        /// </summary>
        Changelog,
    }
}
