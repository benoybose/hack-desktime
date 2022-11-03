using System;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Windows.Forms;

namespace DeskTime
{
    public class ForgotForm : Form
    {
        public string email;

        public int status;

        private IContainer components;

        private Button button1;

        private TextBox textBox1;

        private Label label1;

        public ForgotForm()
        {
            InitializeComponent();
            email = "";
            status = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            email = textBox1.Text;
            status = 0;
            try
            {
                Uri uri = new Uri(string.Format(MainWin.hosts.ActiveApiEndPoint() + "forgotpassword?email=" + HttpUtility.UrlEncode(email)));
                Cursor = Cursors.WaitCursor;
                RESTService.GetResponse(uri, delegate (Response Response)
                {
                    if (Response != null)
                    {
                        status = Response.Status;
                        Invoke((MethodInvoker)delegate
                        {
                            Cursor = Cursors.Default;
                            if (status == 2)
                            {
                                Close();
                            }
                        });
                        switch (status)
                        {
                            case 0:
                                MessageBox.Show("Invalid email address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                break;
                            case 1:
                                MessageBox.Show("No such email address was found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                break;
                            case 2:
                                MessageBox.Show("Check your inbox for email from DeskTime.", "Password recovery", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                break;
                        }
                        return;
                    }
                    throw new InvalidOperationException("Something went wrong: unknown error");
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Cursor = Cursors.Default;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                button1_Click(null, null);
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeskTime.ForgotForm));
            button1 = new System.Windows.Forms.Button();
            textBox1 = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            button1.Location = new System.Drawing.Point(181, 51);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(130, 23);
            button1.TabIndex = 2;
            button1.Text = "Send a new password";
            button1.UseVisualStyleBackColor = true;
            button1.Click += new System.EventHandler(button1_Click);
            textBox1.Location = new System.Drawing.Point(12, 25);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(299, 20);
            textBox1.TabIndex = 1;
            textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(textBox1_KeyDown);
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(193, 13);
            label1.TabIndex = 0;
            label1.Text = "Enter email address you signed up with:";
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            base.ClientSize = new System.Drawing.Size(323, 89);
            base.Controls.Add(button1);
            base.Controls.Add(textBox1);
            base.Controls.Add(label1);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ForgotForm";
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Forgot password?";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
