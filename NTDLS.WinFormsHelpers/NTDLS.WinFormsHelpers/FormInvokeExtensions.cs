namespace NTDLS.WinFormsHelpers
{
    /// <summary>
    /// Various WinForms Form extensions for invoking common tasks to prevent cross-thread-operations.
    /// </summary>
    public static class FormInvokeExtensions
    {
        /// <summary>
        /// Closes a form with the given DialogResult.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="result"></param>
        public static void InvokeClose(this Form form, DialogResult result)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(() => form.InvokeClose(result));
            }
            else
            {
                form.DialogResult = result;
                form.Close();
            }
        }

        /// <summary>
        /// Invokes the form to show a message box.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static DialogResult InvokeMessageBox(this Form form, string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (form.InvokeRequired)
            {
                return form.Invoke(new Func<DialogResult>(() => form.InvokeMessageBox(message, title, buttons, icon)));
            }
            else
            {
                return MessageBox.Show(form, message, title, buttons, icon);
            }
        }

        /// <summary>
        /// Invokes the form to show a message box.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static DialogResult InvokeMessageBox(this Form form, string message, string title, MessageBoxButtons buttons)
        {
            if (form.InvokeRequired)
            {
                return form.Invoke(new Func<DialogResult>(() => form.InvokeMessageBox(message, title, buttons)));
            }
            else
            {
                return MessageBox.Show(form, message, title, buttons);
            }
        }

        /// <summary>
        /// Invokes the form to show a message box.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static DialogResult InvokeMessageBox(this Form form, string message, string title)
        {
            if (form.InvokeRequired)
            {
                return form.Invoke(new Func<DialogResult>(() => form.InvokeMessageBox(message, title)));
            }
            else
            {
                return MessageBox.Show(form, message, title);
            }
        }
    }
}
