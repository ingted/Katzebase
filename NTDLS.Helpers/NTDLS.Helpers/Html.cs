using System.Text.RegularExpressions;

namespace NTDLS.Helpers
{
    /// <summary>
    /// Helper functions for dealing with text (but specifically HTML).
    /// </summary>
    public static class Html
    {
        /// <summary>
        /// Removes all traces of HTML tags from a given string.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string StripHtml(string html)
        {
            html = html.Replace("\'", ""); //Compress "don't" -> "dont"
            html = html.Replace("`", ""); //Compress "don't" -> "dont"
            html = html.Replace("’", ""); //Compress "don't" -> "dont"
            html = (new Regex("<(.|\n)+?>")).Replace(html, " "); //Remove all text between < and >
            html = (new Regex("\\[\\[(.|\n)+?\\]\\]")).Replace(html, " "); //Remove all text between [[ and ]]
            html = (new Regex("\\&(.|\n)+?\\;")).Replace(html, " "); //Remove all text between & and ;
            html = (new Regex("[^A-Za-z]")).Replace(html, " "); //Remove all non-alpha-numeric
            html = (new Regex(@"\s+")).Replace(html, " "); // compress all whitespace to one space.

            return html.Trim();
        }
    }
}
