using NTDLS.Persistence;

namespace TestHarness
{
    internal class Program
    {
        class UIPreferences
        {
            public string Username { get; set; } = string.Empty;
            public bool RememberPassword { get; set; }
        }

        static void Main(string[] args)
        {

            //Example of saving preferences:
            var preferences = new UIPreferences()
            {
                Username = "JPatterson",
                RememberPassword = false
            };

            //The file will be given the name of the class type and the given program 
            //  name and will be stored in the common programs data directory.
            CommonApplicationData.SaveToDisk("MyProgramName", preferences);

            //Exmaple loading preferences:
            var loadedPreferences = CommonApplicationData.LoadFromDisk<UIPreferences>("MyProgramName");
        }
    }
}
