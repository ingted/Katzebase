namespace TestHarness
{
    internal class Program
    {
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using var form = new FormMain();
            form.ShowDialog();
        }
    }
}
