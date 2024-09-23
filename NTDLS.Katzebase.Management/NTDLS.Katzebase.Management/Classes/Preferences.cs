﻿using Newtonsoft.Json;
using NTDLS.Helpers;

namespace NTDLS.Katzebase.Management.Classes
{
    internal class Preferences
    {
        static private Preferences? _instance = null;
        static public Preferences Instance
        {
            get
            {
                EnsureInstanceIsCreated();
                return _instance.EnsureNotNull();
            }
        }

        public static void EnsureInstanceIsCreated()
        {
            if (_instance == null)
            {
                if (Directory.Exists(DirectoryPath) == false)
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                if (File.Exists(FilePath) == false)
                {
                    var dummy = new Preferences();
                    var dummyJson = JsonConvert.SerializeObject(dummy);
                    File.WriteAllText(FilePath, dummyJson);
                }

                _instance = JsonConvert.DeserializeObject<Preferences>(File.ReadAllText(FilePath))
                    ?? new Preferences();
            }
        }

        public List<string> RecentProjects { get; set; } = new();

        public void AddRecentProject(string projectFile)
        {
            RecentProjects.RemoveAll(o => o.Equals(projectFile, StringComparison.InvariantCultureIgnoreCase));
            RecentProjects.Insert(0, projectFile);
        }

        public void RemoveRecentProject(string projectFile)
        {
            RecentProjects.RemoveAll(o => o.Equals(projectFile, StringComparison.InvariantCultureIgnoreCase));
        }

        static public void Save()
        {
            EnsureInstanceIsCreated();

            File.WriteAllText(FilePath, JsonConvert.SerializeObject(_instance));
        }

        static public string DirectoryPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Client.KbConstants.FriendlyName);
            }
        }

        static public string FilePath
        {
            get
            {
                return Path.Combine(DirectoryPath, "UI.json");
            }
        }

        public int FormStudioWidth { get; set; } = 1200;
        public int FormStudioHeight { get; set; } = 800;
        public int ResultsSplitterDistance { get; set; } = 400;
        public int ObjectExplorerSplitterDistance { get; set; } = 350;
    }
}
