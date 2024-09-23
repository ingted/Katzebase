namespace NTDLS.Helpers
{
    /// <summary>
    /// Functions for formatting various data types.
    /// </summary>
    public class Formatters
    {
        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(decimal size)
            => FileSize((long)size);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(int size)
            => FileSize((double)size);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(float size)
            => FileSize((double)size);

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(long size)
        {
            decimal dSize = size;
            var format = new string[] { "{0} B", "{0} KB", "{0} MB", "{0} GB", "{0} TB", "{0} PB", "{0} EB" };

            int i = 0;
            while (i < format.Length && dSize >= 1024)
            {
                dSize /= 1024;
                i++;
            }

            return string.Format(format[i], dSize.ToString("N2"));
        }

        /// <summary>
        /// Formats a file size to B, KB, MB, etc.
        /// </summary>
        public static string FileSize(double size)
        {
            var format = new string[] { "{0} B", "{0} KB", "{0} MB", "{0} GB", "{0} TB", "{0} PB", "{0} EB" };

            int i = 0;
            while (i < format.Length && size >= 1024)
            {
                size = (100 * size / 1024.0) / 100.0;
                i++;
            }

            return string.Format(format[i], size.ToString("N2"));
        }
    }
}
