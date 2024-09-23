namespace NTDLS.WinFormsHelpers
{
    /// <summary>
    /// Various WinForms Control extensions for invoking common tasks to prevent cross-thread-operations.
    /// </summary>
    public static class ControlInvokeExtensions
    {
        /// <summary>
        /// Invokes the control to set its text.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        public static void InvokeSetText(this Control control, string text)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => control.Text = text));
            }
            else
            {
                control.Text = text;
            }
        }

        /// <summary>
        /// Invokes the form to enable or disable a control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="enabled"></param>
        public static void InvokeEnableControl(this Control control, bool enabled)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => control.Enabled = enabled));
            }
            else
            {
                control.Enabled = enabled;
            }
        }

        /// <summary>
        /// Invokes the form to enable a control.
        /// </summary>
        /// <param name="control"></param>
        public static void InvokeEnable(this Control control)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => control.Enabled = true));
            }
            else
            {
                control.Enabled = true;
            }
        }

        /// <summary>
        /// Invokes the form to disable a control.
        /// </summary>
        /// <param name="control"></param>
        public static void InvokeDisable(this Control control)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => control.Enabled = false));
            }
            else
            {
                control.Enabled = false;
            }
        }
    }
}
