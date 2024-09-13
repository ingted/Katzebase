using NTDLS.WinFormsHelpers;

namespace TestHarness
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void buttonProgress_Click(object sender, EventArgs e)
        {
            var progressForm = new ProgressForm();

            progressForm.Execute(() =>
            {
                progressForm.SetProgressMaximum(30);

                for (int i = 0; i < 30; i++)
                {
                    Thread.Sleep(100);

                    if (i == 15)
                    {
                        progressForm.MessageBox("Half way there!", "Caption");

                    }

                    progressForm.SetProgressValue(i);
                }
            });
        }
    }
}
