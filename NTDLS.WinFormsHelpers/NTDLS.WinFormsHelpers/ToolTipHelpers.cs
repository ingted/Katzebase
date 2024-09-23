using NTDLS.Helpers;

namespace NTDLS.WinFormsHelpers
{
    /// <summary>
    /// Various methods for handling tooltips.
    /// </summary>
    public static class ToolTipHelpers
    {
        /// <summary>
        /// Adds a tooltip to a given set of WinForms controls.
        /// </summary>
        /// <param name="tooltip">The tooltip control.</param>
        /// <param name="controls">The controls to add the tooltip to.</param>
        /// <param name="text">The text to be displayed</param>
        /// <param name="wrapLength">Optional number of characters to wrap the tooltip text to.</param>
        public static void AddControls(this ToolTip tooltip, Control[] controls, string text, int wrapLength = 50)
        {
            foreach (var control in controls)
            {
                tooltip.SetToolTip(control, Text.InsertLineBreaks(text, wrapLength));
            }
        }

        /// <summary>
        /// Adds a tooltip to a given WinForms control.
        /// </summary>
        /// <param name="tooltip">The tooltip control.</param>
        /// <param name="control">The control to add the tooltip to.</param>
        /// <param name="text">The text to be displayed</param>
        /// <param name="wrapLength">Optional number of characters to wrap the tooltip text to.</param>
        public static void AddControls(this ToolTip tooltip, Control control, string text, int wrapLength = 50)
        {
            tooltip.SetToolTip(control, Text.InsertLineBreaks(text, wrapLength));
        }

        /// <summary>
        /// Creates a tooltip control and hooks the given form for automatic disposal.
        /// </summary>
        /// <param name="form">The form that will own the tooltip control.</param>
        public static ToolTip CreateToolTipControl(Form form)
        {
            var tooltip = new ToolTip
            {
                InitialDelay = 100,
                ReshowDelay = 100,
                AutoPopDelay = 5000,
                ShowAlways = true
            };

            form.Disposed += (object? sender, EventArgs e) =>
            {
                tooltip.Dispose();
            };

            return tooltip;
        }


        /// <summary>
        /// Creates a tooltip control with custom settings and hooks the given form for automatic disposal.
        /// </summary>
        /// <param name="form">The form that will own the tooltip control.</param>
        /// <param name="initialDelay"></param>
        /// <param name="reshowDelay"></param>
        /// <param name="autoPopDelay"></param>
        /// <param name="showAlways"></param>
        public static ToolTip CreateToolTipControl(Form form, int initialDelay, int reshowDelay, int autoPopDelay, bool showAlways)
        {
            var tooltip = new ToolTip
            {
                InitialDelay = initialDelay,
                ReshowDelay = reshowDelay,
                AutoPopDelay = autoPopDelay,
                ShowAlways = showAlways
            };

            form.Disposed += (object? sender, EventArgs e) =>
            {
                tooltip.Dispose();
            };

            return tooltip;
        }
    }
}
