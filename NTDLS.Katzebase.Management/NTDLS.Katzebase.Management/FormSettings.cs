using NTDLS.Persistence;
using NTDLS.WinFormsHelpers;

namespace NTDLS.Katzebase.Management
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();

            AcceptButton = buttonSave;
            CancelButton = buttonCancel;

            textBoxUIQueryTimeOut.Text = $"{Program.Settings.UIQueryTimeOut:n0}";
            textBoxQueryTimeOut.Text = $"{Program.Settings.QueryTimeOut:n0}";
            textBoxMaximumRows.Text = $"{Program.Settings.MaximumRows:n0}";
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var settings = new ManagementSettings
                {
                    UIQueryTimeOut = textBoxUIQueryTimeOut.GetAndValidateNumeric(1, 600, "UI query time-out must be between [min] and [max]."),
                    QueryTimeOut = textBoxQueryTimeOut.GetAndValidateNumeric(-1, 86400, "Query time-out must be between [min] (infinite) and [max]."),
                    MaximumRows = textBoxMaximumRows.GetAndValidateNumeric(-1, int.MaxValue, "Maximum rows must be between [min] (no maximum) and [max].")
                };

                LocalUserApplicationData.SaveToDisk($"{Client.KbConstants.FriendlyName}\\Management", settings);
                Program.Settings = settings;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Client.KbConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
