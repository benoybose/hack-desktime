using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DeskTime
{
    public class UpdateDlg : Form
    {
        private IContainer components;

        public Label lbInfo;

        public Button btOk;

        public Button btNo;

        public Button btYes;

        public UpdateDlg()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeskTime.UpdateDlg));
            lbInfo = new System.Windows.Forms.Label();
            btOk = new System.Windows.Forms.Button();
            btNo = new System.Windows.Forms.Button();
            btYes = new System.Windows.Forms.Button();
            SuspendLayout();
            lbInfo.AutoSize = true;
            lbInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            lbInfo.Location = new System.Drawing.Point(9, 11);
            lbInfo.Name = "lbInfo";
            lbInfo.Size = new System.Drawing.Size(282, 13);
            lbInfo.TabIndex = 11;
            lbInfo.Text = "New DeskTime version available. Do you want to update?";
            btOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            btOk.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            btOk.Location = new System.Drawing.Point(211, 37);
            btOk.Name = "btOk";
            btOk.Size = new System.Drawing.Size(75, 23);
            btOk.TabIndex = 10;
            btOk.Text = "Ok";
            btOk.UseVisualStyleBackColor = true;
            btOk.Visible = false;
            btNo.DialogResult = System.Windows.Forms.DialogResult.No;
            btNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            btNo.Location = new System.Drawing.Point(211, 37);
            btNo.Name = "btNo";
            btNo.Size = new System.Drawing.Size(75, 23);
            btNo.TabIndex = 9;
            btNo.Text = "No";
            btNo.UseVisualStyleBackColor = true;
            btYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            btYes.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            btYes.Location = new System.Drawing.Point(130, 37);
            btYes.Name = "btYes";
            btYes.Size = new System.Drawing.Size(75, 23);
            btYes.TabIndex = 8;
            btYes.Text = "Yes";
            btYes.UseVisualStyleBackColor = true;
            base.AcceptButton = btNo;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = btOk;
            base.ClientSize = new System.Drawing.Size(301, 70);
            base.Controls.Add(lbInfo);
            base.Controls.Add(btOk);
            base.Controls.Add(btNo);
            base.Controls.Add(btYes);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "UpdateDlg";
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "DeskTime Update";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
