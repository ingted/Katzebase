using NTDLS.WinFormsHelpers.Controls;

namespace NTDLS.WinFormsHelpers
{
    /// <summary>
    /// Progress form used for multi-threaded progress reporting.
    /// </summary>
    partial class FormProgress
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProgress));
            buttonCancel = new Button();
            pbProgress = new ProgressBar();
            labelHeader = new Label();
            labelBody = new Label();
            spinningActivity = new ActivityIndicator();
            SuspendLayout();
            // 
            // buttonCancel
            // 
            buttonCancel.Enabled = false;
            buttonCancel.Location = new Point(286, 120);
            buttonCancel.Margin = new Padding(4, 3, 4, 3);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(88, 27);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // pbProgress
            // 
            pbProgress.Location = new Point(59, 87);
            pbProgress.Margin = new Padding(4, 3, 4, 3);
            pbProgress.Name = "pbProgress";
            pbProgress.Size = new Size(314, 27);
            pbProgress.Style = ProgressBarStyle.Marquee;
            pbProgress.TabIndex = 2;
            // 
            // lblHeader
            // 
            labelHeader.AutoEllipsis = true;
            labelHeader.Location = new Point(56, 14);
            labelHeader.Margin = new Padding(4, 0, 4, 0);
            labelHeader.Name = "lblHeader";
            labelHeader.Size = new Size(317, 38);
            labelHeader.TabIndex = 3;
            labelHeader.Text = "Please wait...";
            // 
            // lblBody
            // 
            labelBody.AutoEllipsis = true;
            labelBody.Location = new Point(56, 52);
            labelBody.Margin = new Padding(4, 0, 4, 0);
            labelBody.Name = "lblBody";
            labelBody.Size = new Size(317, 31);
            labelBody.TabIndex = 4;
            labelBody.Text = "Please wait...";
            // 
            // spinningActivity
            // 
            spinningActivity.ActiveSegmentColor = Color.FromArgb(35, 146, 33);
            spinningActivity.AutoIncrement = true;
            spinningActivity.AutoIncrementFrequency = 100D;
            spinningActivity.BehindTransitionSegmentIsActive = false;
            spinningActivity.InactiveSegmentColor = Color.FromArgb(218, 218, 218);
            spinningActivity.Location = new Point(13, 20);
            spinningActivity.Margin = new Padding(4, 0, 4, 0);
            spinningActivity.Name = "spinningActivity";
            spinningActivity.Size = new Size(35, 32);
            spinningActivity.TabIndex = 4;
            spinningActivity.TransitionSegment = 9;
            spinningActivity.TransitionSegmentColor = Color.FromArgb(129, 242, 121);
            // 
            // FormProgress
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(387, 160);
            ControlBox = false;
            Controls.Add(labelBody);
            Controls.Add(spinningActivity);
            Controls.Add(labelHeader);
            Controls.Add(pbProgress);
            Controls.Add(buttonCancel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormProgress";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Please wait...";
            Shown += FormProgress_Shown;
            ResumeLayout(false);
        }

        #endregion

        private Button buttonCancel;
        private ProgressBar pbProgress;
        private Label labelHeader;
        private Label labelBody;
        private ActivityIndicator spinningActivity;
    }
}