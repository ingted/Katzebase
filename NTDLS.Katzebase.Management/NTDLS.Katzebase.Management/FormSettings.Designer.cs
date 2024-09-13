using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NTDLS.Katzebase.Management
{
    public partial class FormSettings : Form
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

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FormSettings));
            labelUIQueryTimeOut = new Label();
            textBoxUIQueryTimeOut = new TextBox();
            textBoxQueryTimeOut = new TextBox();
            labelQueryTimeOut = new Label();
            textBoxMaximumRows = new TextBox();
            labelMaximumRows = new Label();
            buttonSave = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // labelUIQueryTimeOut
            // 
            labelUIQueryTimeOut.AutoSize = true;
            labelUIQueryTimeOut.Location = new Point(12, 30);
            labelUIQueryTimeOut.Name = "labelUIQueryTimeOut";
            labelUIQueryTimeOut.Size = new Size(101, 15);
            labelUIQueryTimeOut.TabIndex = 0;
            labelUIQueryTimeOut.Text = "UI query time-out";
            // 
            // textBoxUIQueryTimeOut
            // 
            textBoxUIQueryTimeOut.Location = new Point(119, 27);
            textBoxUIQueryTimeOut.Name = "textBoxUIQueryTimeOut";
            textBoxUIQueryTimeOut.Size = new Size(100, 23);
            textBoxUIQueryTimeOut.TabIndex = 0;
            // 
            // textBoxQueryTimeOut
            // 
            textBoxQueryTimeOut.Location = new Point(119, 56);
            textBoxQueryTimeOut.Name = "textBoxQueryTimeOut";
            textBoxQueryTimeOut.Size = new Size(100, 23);
            textBoxQueryTimeOut.TabIndex = 1;
            // 
            // labelQueryTimeOut
            // 
            labelQueryTimeOut.AutoSize = true;
            labelQueryTimeOut.Location = new Point(24, 59);
            labelQueryTimeOut.Name = "labelQueryTimeOut";
            labelQueryTimeOut.Size = new Size(89, 15);
            labelQueryTimeOut.TabIndex = 2;
            labelQueryTimeOut.Text = "Query time-out";
            // 
            // textBoxMaximumRows
            // 
            textBoxMaximumRows.Location = new Point(119, 85);
            textBoxMaximumRows.Name = "textBoxMaximumRows";
            textBoxMaximumRows.Size = new Size(100, 23);
            textBoxMaximumRows.TabIndex = 2;
            // 
            // labelMaximumRows
            // 
            labelMaximumRows.AutoSize = true;
            labelMaximumRows.Location = new Point(24, 88);
            labelMaximumRows.Name = "labelMaximumRows";
            labelMaximumRows.Size = new Size(90, 15);
            labelMaximumRows.TabIndex = 4;
            labelMaximumRows.Text = "Maximum rows";
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(90, 138);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(75, 23);
            buttonSave.TabIndex = 3;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += ButtonSave_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(171, 138);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // FormSettings
            // 
            ClientSize = new Size(258, 175);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(textBoxMaximumRows);
            Controls.Add(labelMaximumRows);
            Controls.Add(textBoxQueryTimeOut);
            Controls.Add(labelQueryTimeOut);
            Controls.Add(textBoxUIQueryTimeOut);
            Controls.Add(labelUIQueryTimeOut);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormSettings";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        private Label labelUIQueryTimeOut;
        private TextBox textBoxQueryTimeOut;
        private Label labelQueryTimeOut;
        private TextBox textBoxMaximumRows;
        private Label labelMaximumRows;
        private Button buttonSave;
        private Button buttonCancel;
        private TextBox textBoxUIQueryTimeOut;
    }
}
