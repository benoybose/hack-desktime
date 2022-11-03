using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DeskTime.Properties;

namespace DeskTime
{
    public class Onboarding : Form
    {
        public ContextMenuStrip notifyMenu;

        public int tourSteps;

        private IContainer components;

        private Label tourTitle;

        private RichTextBox tourText;

        private Label tourSkip;

        private Label tourNext;

        public Onboarding()
        {
            InitializeComponent();
            base.FormBorderStyle = FormBorderStyle.None;
            base.TransparencyKey = Color.Black;
            BackColor = Color.Black;
        }

        private void SkipTour_Click(object sender, EventArgs e)
        {
            SkipTourClicked();
        }

        public void SkipTourClicked()
        {
            Close();
            notifyMenu.AutoClose = true;
            notifyMenu.Close();
            RegistryService.SetValue("skipIntro", "1");
        }

        private void Next_Click(object sender, EventArgs e)
        {
            NextClicked();
        }

        public void NextClicked()
        {
            tourSteps++;
            switch (tourSteps)
            {
                case 1:
                    {
                        Console.WriteLine("Case 1");
                        tourTitle.Text = "Projects";
                        tourText.Text = "Here you can quickly start working on your recent Projects or create a new one.";
                        int index = notifyMenu.Items.IndexOfKey("createNewProjectMenuItem");
                        base.Location = new Point(notifyMenu.Bounds.Location.X - base.Width, notifyMenu.Bounds.Location.Y - base.Height + (notifyMenu.Items[index].Bounds.Location.Y + 7));
                        Activate();
                        base.Visible = true;
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("Case 2");
                        tourTitle.Text = "Private time";
                        tourText.Text = "When taking a break from assignments, use Private time and it will not be tracked.";
                        int index = notifyMenu.Items.IndexOfKey("pauseToolStripMenuItem");
                        base.Location = new Point(notifyMenu.Bounds.Location.X - base.Width, notifyMenu.Bounds.Location.Y - base.Height + (notifyMenu.Items[index].Bounds.Location.Y + 7));
                        break;
                    }
                case 3:
                    {
                        Console.WriteLine("Case 3");
                        tourTitle.Text = "Show Desktime";
                        tourText.Text = "Now click “Show Desktime” to check out all the other features DeskTime has to offer!";
                        tourSkip.Text = "CLOSE";
                        tourSkip.Visible = false;
                        tourNext.Visible = false;
                        int index = notifyMenu.Items.IndexOfKey("viewStatisticsToolStripMenuItem");
                        base.Location = new Point(notifyMenu.Bounds.Location.X - base.Width, notifyMenu.Bounds.Location.Y - base.Height + (notifyMenu.Items[index].Bounds.Location.Y + 7));
                        notifyMenu.AutoClose = true;
                        RegistryService.SetValue("skipIntro", "1");
                        break;
                    }
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
            tourTitle = new System.Windows.Forms.Label();
            tourText = new System.Windows.Forms.RichTextBox();
            tourSkip = new System.Windows.Forms.Label();
            tourNext = new System.Windows.Forms.Label();
            SuspendLayout();
            tourTitle.AutoSize = true;
            tourTitle.BackColor = System.Drawing.Color.Transparent;
            tourTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            tourTitle.ForeColor = System.Drawing.Color.FromArgb(64, 71, 81);
            tourTitle.Location = new System.Drawing.Point(10, 17);
            tourTitle.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            tourTitle.Name = "tourTitle";
            tourTitle.Size = new System.Drawing.Size(98, 25);
            tourTitle.TabIndex = 0;
            tourTitle.Text = "Projects";
            tourText.BackColor = System.Drawing.Color.White;
            tourText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            tourText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 186);
            tourText.ForeColor = System.Drawing.Color.FromArgb(64, 71, 81);
            tourText.Location = new System.Drawing.Point(15, 50);
            tourText.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            tourText.Name = "tourText";
            tourText.ReadOnly = true;
            tourText.Size = new System.Drawing.Size(325, 49);
            tourText.TabIndex = 0;
            tourText.TabStop = false;
            tourText.Text = "Here you can quickly start working on your recent Projects or create a new one.";
            tourSkip.AutoSize = true;
            tourSkip.BackColor = System.Drawing.Color.Transparent;
            tourSkip.Cursor = System.Windows.Forms.Cursors.Hand;
            tourSkip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 186);
            tourSkip.ForeColor = System.Drawing.Color.FromArgb(95, 185, 42);
            tourSkip.Location = new System.Drawing.Point(12, 102);
            tourSkip.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            tourSkip.Name = "tourSkip";
            tourSkip.Size = new System.Drawing.Size(79, 16);
            tourSkip.TabIndex = 3;
            tourSkip.Text = "SKIP TOUR";
            tourSkip.Click += new System.EventHandler(SkipTour_Click);
            tourNext.AutoSize = true;
            tourNext.BackColor = System.Drawing.Color.Transparent;
            tourNext.Cursor = System.Windows.Forms.Cursors.Hand;
            tourNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 186);
            tourNext.ForeColor = System.Drawing.Color.FromArgb(95, 185, 42);
            tourNext.Location = new System.Drawing.Point(317, 102);
            tourNext.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            tourNext.Name = "tourNext";
            tourNext.Size = new System.Drawing.Size(44, 16);
            tourNext.TabIndex = 4;
            tourNext.Text = "NEXT";
            tourNext.Click += new System.EventHandler(Next_Click);
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Gray;
            BackgroundImage = DeskTime.Properties.Resources.IntroPopup;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            base.ClientSize = new System.Drawing.Size(395, 136);
            base.ControlBox = false;
            base.Controls.Add(tourNext);
            base.Controls.Add(tourSkip);
            base.Controls.Add(tourText);
            base.Controls.Add(tourTitle);
            DoubleBuffered = true;
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            base.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "Onboarding";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            base.TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
