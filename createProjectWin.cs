using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DeskTime
{
    public class createProjectWin : Form
    {
        public Form parentMainForm;

        private IContainer components;

        public ComboBox taskName;

        private Label label2;

        public ComboBox projectName;

        private Button cancelButton;

        private Button createButton;

        private Label label1;

        public createProjectWin()
        {
            InitializeComponent();
        }

        private void createProjectWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            (parentMainForm as MainWin).cf = null;
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            if (projectName.Text.Length > 0)
            {
                base.Visible = false;
                (parentMainForm as MainWin).StartProject(projectName.Text, taskName.Text);
                Close();
            }
        }

        private void projectName_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = projectName.SelectedIndex;
            try
            {
                ProjectItem projectItem = (parentMainForm as MainWin).allProjects[selectedIndex];
                List<TaskItem> list = ((projectItem.tasks != null) ? projectItem.tasks : null);
                taskName.Items.Clear();
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        taskName.Items.Add(list[i].name);
                    }
                }
            }
            catch (Exception)
            {
                taskName.Items.Clear();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void projectName_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void taskName_KeyDown(object sender, KeyEventArgs e)
        {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeskTime.createProjectWin));
            taskName = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            projectName = new System.Windows.Forms.ComboBox();
            cancelButton = new System.Windows.Forms.Button();
            createButton = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            taskName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            taskName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            taskName.FormattingEnabled = true;
            taskName.Location = new System.Drawing.Point(86, 36);
            taskName.Name = "taskName";
            taskName.Size = new System.Drawing.Size(176, 21);
            taskName.TabIndex = 1;
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(19, 37);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(63, 13);
            label2.TabIndex = 11;
            label2.Text = "Task name:";
            projectName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            projectName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            projectName.FormattingEnabled = true;
            projectName.Location = new System.Drawing.Point(86, 9);
            projectName.Name = "projectName";
            projectName.Size = new System.Drawing.Size(176, 21);
            projectName.TabIndex = 0;
            projectName.SelectedIndexChanged += new System.EventHandler(projectName_SelectedIndexChanged);
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(104, 70);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += new System.EventHandler(cancelButton_Click);
            createButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            createButton.Location = new System.Drawing.Point(184, 70);
            createButton.Name = "createButton";
            createButton.Size = new System.Drawing.Size(75, 23);
            createButton.TabIndex = 2;
            createButton.Text = "&Start";
            createButton.UseVisualStyleBackColor = true;
            createButton.Click += new System.EventHandler(createButton_Click);
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 10);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(72, 13);
            label1.TabIndex = 7;
            label1.Text = "Project name:";
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(271, 100);
            base.Controls.Add(taskName);
            base.Controls.Add(label2);
            base.Controls.Add(projectName);
            base.Controls.Add(cancelButton);
            base.Controls.Add(createButton);
            base.Controls.Add(label1);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "createProjectWin";
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Create a project / Search for a project";
            base.TopMost = true;
            base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(createProjectWin_FormClosed);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
