using System.Collections;

namespace NetTunnel.UI.Helpers
{
    /// <summary>
    /// Very basic ListViewItemComparer to allow sorting of rows.
    /// </summary>
    public class ListViewItemComparer : IComparer
    {
        private int column;
        private SortOrder sortOrder;

        /// <summary>
        /// Creates a new instance of the ListViewItemComparer.
        /// </summary>
        public ListViewItemComparer()
        {
            column = 0;
            sortOrder = SortOrder.Ascending;
        }

        /// <summary>
        /// Performs a comparison of two ListView rows.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object? x, object? y)
        {
            var item1 = (ListViewItem?)x;
            var item2 = (ListViewItem?)y;

            // Use the appropriate column's text for comparison
            string text1 = item1?.SubItems[column]?.Text ?? string.Empty;
            string text2 = item2?.SubItems[column]?.Text ?? string.Empty;

            // Compare the two items based on the text and sorting order
            int result = String.Compare(text1, text2, StringComparison.OrdinalIgnoreCase);

            if (sortOrder == SortOrder.Descending)
                result = -result;

            return result;
        }

        /// <summary>
        /// Gets/sets the sort column ordinal.
        /// </summary>
        public int SortColumn
        {
            get { return column; }
            set { column = value; }
        }

        /// <summary>
        /// Gets/sets the sort order of the SortColumn.
        /// </summary>
        public SortOrder SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }
    }
}
