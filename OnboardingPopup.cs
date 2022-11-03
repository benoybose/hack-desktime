using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DeskTime.Properties;

namespace DeskTime
{
    public class OnboardingPopup : Form
    {
        private IContainer components;

        private PictureBox CloseStartTour;

        public Label tourTitle;

        private PictureBox pictureBox1;

        public OnboardingPopup()
        {
            InitializeComponent();
            base.FormBorderStyle = FormBorderStyle.None;
            base.TransparencyKey = Color.Black;
            BackColor = Color.Black;
        }

        private void CloseStartTour_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OnboardingPopup_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tourTitle_Click(object sender, EventArgs e)
        {
            Close();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeskTime.OnboardingPopup));
            tourTitle = new System.Windows.Forms.Label();
            CloseStartTour = new System.Windows.Forms.PictureBox();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)CloseStartTour).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            tourTitle.AutoSize = true;
            tourTitle.BackColor = System.Drawing.Color.Transparent;
            tourTitle.Cursor = System.Windows.Forms.Cursors.Hand;
            tourTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14f);
            tourTitle.ForeColor = System.Drawing.Color.FromArgb(64, 71, 81);
            tourTitle.Location = new System.Drawing.Point(36, 21);
            tourTitle.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            tourTitle.Name = "tourTitle";
            tourTitle.Size = new System.Drawing.Size(241, 24);
            tourTitle.TabIndex = 1;
            tourTitle.Text = "This is Your DeskTime app.";
            tourTitle.Click += new System.EventHandler(tourTitle_Click);
            CloseStartTour.BackColor = System.Drawing.Color.Transparent;
            CloseStartTour.BackgroundImage = (System.Drawing.Image)resources.GetObject("CloseStartTour.BackgroundImage");
            CloseStartTour.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            CloseStartTour.Cursor = System.Windows.Forms.Cursors.Hand;
            CloseStartTour.Location = new System.Drawing.Point(273, 9);
            CloseStartTour.Margin = new System.Windows.Forms.Padding(0);
            CloseStartTour.Name = "CloseStartTour";
            CloseStartTour.Size = new System.Drawing.Size(8, 8);
            CloseStartTour.TabIndex = 5;
            CloseStartTour.TabStop = false;
            CloseStartTour.Click += new System.EventHandler(CloseStartTour_Click);
            pictureBox1.BackColor = System.Drawing.Color.Transparent;
            pictureBox1.BackgroundImage = DeskTime.Properties.Resources.desktime;
            pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            pictureBox1.Location = new System.Drawing.Point(10, 21);
            pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(24, 24);
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Gray;
            BackgroundImage = DeskTime.Properties.Resources.IntroStartPopup;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            base.ClientSize = new System.Drawing.Size(290, 64);
            base.ControlBox = false;
            base.Controls.Add(pictureBox1);
            base.Controls.Add(CloseStartTour);
            base.Controls.Add(tourTitle);
            DoubleBuffered = true;
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            base.Margin = new System.Windows.Forms.Padding(1);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "OnboardingPopup";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            base.TopMost = true;
            base.Click += new System.EventHandler(OnboardingPopup_Click);
            ((System.ComponentModel.ISupportInitialize)CloseStartTour).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
