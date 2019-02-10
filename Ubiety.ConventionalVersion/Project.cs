using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Ubiety.ConventionalVersion
{
    public class Project
    {
        private Project(string file, Version version)
        {
            File = file;
            Version = version;
        }

        public string File { get; }

        public Version Version { get; }

        public static IEnumerable<Project> DiscoverProjects(string directory)
        {
            return Directory
                .GetFiles(directory, "*.csproj", SearchOption.AllDirectories)
                .Where(IsVersionable)
                .Select(Create)
                .ToList();
        }

        public static bool IsVersionable(string projectFile)
        {
            if (GetVersion(projectFile) == null)
            {
                return false;
            }

            return true;
        }

        public static Version GetVersion(string projectFile)
        {
            XDocument document = XDocument.Load(projectFile);
            var versionString = document.Element("Version").Value;

            return new Version(versionString);
        }

        public void SetVersion(Version nextVersion)
        {
            XDocument document = XDocument.Load(File);
            document.Element("Version").Value = nextVersion.ToString();
            document.Save(File);
        }

        public static Project Create(string projectFile)
        {
            return new Project(projectFile, GetVersion(projectFile));
        }
    }
}
