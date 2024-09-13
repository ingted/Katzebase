namespace NTDLS.WinFormsHelpers
{
    /// <summary>
    /// Various WinForms ListView extensions for invoking common tasks to prevent cross-thread-operations.
    /// </summary>
    public static class ListViewInvokeExtensions
    {
        /// <summary>
        /// Invokes the ListView to delete an item.
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="item"></param>
        public static void InvokeDeleteItem(this ListView listView, ListViewItem item)
        {
            if (listView.InvokeRequired)
            {
                listView.Invoke(new Action<ListViewItem>(listView.InvokeDeleteItem), item);
            }
            else
            {
                listView.Items.Remove(item);
            }
        }

        /// <summary>
        /// Invokes the ListView to delete all selected items.
        /// </summary>
        /// <param name="listView"></param>
        public static void InvokeDeleteSelectedItems(this ListView listView)
        {
            if (listView.InvokeRequired)
            {
                listView.Invoke(new Action(listView.InvokeDeleteSelectedItems));
            }
            else
            {
                var items = new List<ListViewItem>();

                foreach (ListViewItem item in listView.SelectedItems)
                {
                    items.Add(item);
                }

                foreach (var item in items)
                {
                    listView.Items.Remove(item);
                }
            }
        }

        /// <summary>
        /// Invokes the ListView to clear all selections.
        /// </summary>
        /// <param name="listView"></param>
        public static void InvokeClearSelection(this ListView listView)
        {
            if (listView.InvokeRequired)
            {
                listView.Invoke(new Action(listView.InvokeClearSelection));
            }
            else
            {
                listView.SelectedItems.Clear();
            }
        }

        /// <summary>
        /// Invokes the ListView to clear all items.
        /// </summary>
        /// <param name="listView"></param>
        public static void InvokeClearRows(this ListView listView)
        {
            if (listView.InvokeRequired)
            {
                listView.Invoke(new Action(listView.InvokeClearRows));
            }
            else
            {
                listView.Items.Clear();
            }
        }

    }
}
