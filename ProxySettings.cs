using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DeskTime
{
    public class ProxySettings : Form
    {
        public Form f1;

        private IContainer components;

        private Button button2;

        private Button button1;

        private Label label4;

        public TextBox proxy_pass;

        private Label label3;

        public TextBox proxy_user;

        private Label label2;

        public TextBox proxy_port;

        private Label label1;

        public TextBox proxy_address;

        public RadioButton proxy_yes;

        public RadioButton proxy_ie;

        public RadioButton proxy_no;

        public ProxySettings()
        {
            InitializeComponent();
        }

        private void ProxySettings_Load(object sender, EventArgs e)
        {
            switch (MainWin.ProxyStyle)
            {
                case 0:
                    proxy_no.Checked = true;
                    break;
                case 1:
                    proxy_ie.Checked = true;
                    break;
                case 2:
                    proxy_yes.Checked = true;
                    break;
            }
            proxy_address.Text = MainWin.ProxyAddress;
            int proxyPort = MainWin.ProxyPort;
            proxy_port.Text = proxyPort.ToString();
            proxy_user.Text = MainWin.ProxyUser;
            proxy_pass.Text = MainWin.ProxyPassword;
            proxy_address.ReadOnly = !proxy_yes.Checked;
            proxy_port.ReadOnly = !proxy_yes.Checked;
            proxy_user.ReadOnly = !proxy_yes.Checked;
            proxy_pass.ReadOnly = !proxy_yes.Checked;
        }

        private void proxy_no_CheckedChanged(object sender, EventArgs e)
        {
            proxy_address.ReadOnly = !proxy_yes.Checked;
            proxy_port.ReadOnly = !proxy_yes.Checked;
            proxy_user.ReadOnly = !proxy_yes.Checked;
            proxy_pass.ReadOnly = !proxy_yes.Checked;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeskTime.ProxySettings));
            button2 = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            proxy_pass = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            proxy_user = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            proxy_port = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            proxy_address = new System.Windows.Forms.TextBox();
            proxy_yes = new System.Windows.Forms.RadioButton();
            proxy_ie = new System.Windows.Forms.RadioButton();
            proxy_no = new System.Windows.Forms.RadioButton();
            SuspendLayout();
            button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            button2.Location = new System.Drawing.Point(233, 235);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(63, 21);
            button2.TabIndex = 25;
            button2.Text = "OK";
            button2.UseVisualStyleBackColor = true;
            button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            button1.Location = new System.Drawing.Point(153, 235);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(70, 21);
            button1.TabIndex = 24;
            button1.Text = "Cancel";
            button1.UseVisualStyleBackColor = true;
            label4.AutoSize = true;
            label4.BackColor = System.Drawing.Color.Transparent;
            label4.Location = new System.Drawing.Point(52, 205);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(53, 13);
            label4.TabIndex = 23;
            label4.Text = "Password";
            proxy_pass.Location = new System.Drawing.Point(123, 202);
            proxy_pass.Name = "proxy_pass";
            proxy_pass.PasswordChar = '*';
            proxy_pass.Size = new System.Drawing.Size(173, 20);
            proxy_pass.TabIndex = 22;
            label3.AutoSize = true;
            label3.BackColor = System.Drawing.Color.Transparent;
            label3.Location = new System.Drawing.Point(52, 172);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(55, 13);
            label3.TabIndex = 21;
            label3.Text = "Username";
            proxy_user.Location = new System.Drawing.Point(123, 169);
            proxy_user.Name = "proxy_user";
            proxy_user.Size = new System.Drawing.Size(173, 20);
            proxy_user.TabIndex = 20;
            label2.AutoSize = true;
            label2.BackColor = System.Drawing.Color.Transparent;
            label2.Location = new System.Drawing.Point(52, 139);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(26, 13);
            label2.TabIndex = 19;
            label2.Text = "Port";
            proxy_port.Location = new System.Drawing.Point(123, 136);
            proxy_port.Name = "proxy_port";
            proxy_port.Size = new System.Drawing.Size(173, 20);
            proxy_port.TabIndex = 18;
            label1.AutoSize = true;
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.Location = new System.Drawing.Point(52, 106);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(45, 13);
            label1.TabIndex = 17;
            label1.Text = "Address";
            proxy_address.Location = new System.Drawing.Point(123, 103);
            proxy_address.Name = "proxy_address";
            proxy_address.Size = new System.Drawing.Size(173, 20);
            proxy_address.TabIndex = 16;
            proxy_yes.AutoSize = true;
            proxy_yes.BackColor = System.Drawing.Color.Transparent;
            proxy_yes.Location = new System.Drawing.Point(32, 74);
            proxy_yes.Name = "proxy_yes";
            proxy_yes.Size = new System.Drawing.Size(117, 17);
            proxy_yes.TabIndex = 15;
            proxy_yes.TabStop = true;
            proxy_yes.Text = "Enter proxy settings";
            proxy_yes.UseVisualStyleBackColor = false;
            proxy_yes.CheckedChanged += new System.EventHandler(proxy_no_CheckedChanged);
            proxy_ie.AutoSize = true;
            proxy_ie.BackColor = System.Drawing.Color.Transparent;
            proxy_ie.Location = new System.Drawing.Point(32, 49);
            proxy_ie.Name = "proxy_ie";
            proxy_ie.Size = new System.Drawing.Size(191, 17);
            proxy_ie.TabIndex = 14;
            proxy_ie.TabStop = true;
            proxy_ie.Text = "Use Internet Explorer proxy settings";
            proxy_ie.UseVisualStyleBackColor = false;
            proxy_ie.CheckedChanged += new System.EventHandler(proxy_no_CheckedChanged);
            proxy_no.AutoSize = true;
            proxy_no.BackColor = System.Drawing.Color.Transparent;
            proxy_no.Location = new System.Drawing.Point(32, 23);
            proxy_no.Name = "proxy_no";
            proxy_no.Size = new System.Drawing.Size(98, 17);
            proxy_no.TabIndex = 13;
            proxy_no.TabStop = true;
            proxy_no.Text = "Don't use proxy";
            proxy_no.UseVisualStyleBackColor = false;
            proxy_no.CheckedChanged += new System.EventHandler(proxy_no_CheckedChanged);
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(337, 280);
            base.Controls.Add(button2);
            base.Controls.Add(button1);
            base.Controls.Add(label4);
            base.Controls.Add(proxy_pass);
            base.Controls.Add(label3);
            base.Controls.Add(proxy_user);
            base.Controls.Add(label2);
            base.Controls.Add(proxy_port);
            base.Controls.Add(label1);
            base.Controls.Add(proxy_address);
            base.Controls.Add(proxy_yes);
            base.Controls.Add(proxy_ie);
            base.Controls.Add(proxy_no);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.Name = "ProxySettings";
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "ProxySettings";
            base.Load += new System.EventHandler(ProxySettings_Load);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
