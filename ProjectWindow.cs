using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DeskTime.Properties;
using Timer = System.Windows.Forms.Timer;

namespace DeskTime
{
    public class ProjectWindow : Form
    {
        public Form parentMainForm;

        public const int WM_NCLBUTTONDOWN = 161;

        public const int HT_CAPTION = 2;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        private static readonly IntPtr HWND_TOP = new IntPtr(0);

        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private const uint SWP_NOSIZE = 1u;

        private const uint SWP_NOMOVE = 2u;

        private const uint TOPMOST_FLAGS = 3u;

        private static uint WS_POPUP = 2147483648u;

        private static uint WS_EX_TOPMOST = 8u;

        private static uint WS_EX_TOOLWINDOW = 128u;

        private static string projectComboBoxPlaceholder = "Project Name";

        private static string projectTaskComboBoxPlaceholder = "Task Name";

        private bool started;

        private static readonly object UpdateProjectsListLock = new object();

        private int timer_track_led_image = 1;

        private IContainer components;

        private Timer update_time;

        private Button close;

        private Label name;

        private PictureBox drop;

        private Label state;

        private Button minimize;

        private Label top;

        private Label bgstuff;

        private ComboBox selectProjectComboBox;

        private ComboBox selectProjectTaskComboBox;

        private Label bg1;

        private PictureBox timer_track_led;

        private Timer projectLedIndicator;

        private Label selectProjectLabel;

        private Label bg2;

        private Label selectProjectTaskLabel;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams obj = base.CreateParams;
                obj.Style = (int)WS_POPUP;
                obj.ExStyle = (int)(WS_EX_TOPMOST + WS_EX_TOOLWINDOW);
                obj.X = 100;
                obj.Y = 100;
                return obj;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        [CLSCompliant(false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public ProjectWindow()
        {
            InitializeComponent();
            Rectangle workingArea = Screen.FromControl(this).WorkingArea;
            base.Left = Convert.ToInt32(RegistryService.GetValue("pwx", (workingArea.Width - base.Width - 20).ToString()) as string);
            base.Top = Convert.ToInt32(RegistryService.GetValue("pwy", (workingArea.Height - base.Height - 20).ToString()) as string);
            if (base.Left < 0)
            {
                base.Left = 0;
            }
            if (base.Left + base.Width > workingArea.Width)
            {
                base.Left = workingArea.Width - base.Width;
            }
            if (base.Top < 0)
            {
                base.Top = 0;
            }
            if (base.Top + base.Height > workingArea.Height)
            {
                base.Top = workingArea.Height - base.Height;
            }
            started = true;
        }

        private void close_Click(object sender, EventArgs e)
        {
            (parentMainForm as MainWin).projWinShown = true;
            Hide();
        }

        private void state_MouseClick(object sender, MouseEventArgs e)
        {
            state_MouseClick(sender, e, forceLabels: false);
        }

        private void state_MouseClick(object sender, MouseEventArgs e, bool forceLabels = false)
        {
            if (forceLabels)
            {
                selectProjectComboBox.Text = projectComboBoxPlaceholder;
                selectProjectTaskComboBox.Text = projectTaskComboBoxPlaceholder;
            }
            if ((parentMainForm as MainWin).currentProjectId != 0)
            {
                (parentMainForm as MainWin).stopProjectMenuItem_Click(null, null);
                selectProjectComboBox.Enabled = true;
                selectProjectTaskComboBox.Enabled = true;
                UpdateProjectsList();
                selectProjectComboBox.Visible = true;
                selectProjectTaskComboBox.Visible = true;
                selectProjectLabel.Visible = false;
                selectProjectTaskLabel.Visible = false;
            }
            else
            {
                string text = selectProjectComboBox.Text.Trim();
                string text2 = selectProjectTaskComboBox.Text.Trim();
                if (text == projectComboBoxPlaceholder || text == "")
                {
                    return;
                }
                if (text2 == projectTaskComboBoxPlaceholder)
                {
                    text2 = "";
                }
                selectProjectLabel.Text = "Project: " + text;
                selectProjectLabel.Visible = true;
                selectProjectTaskLabel.Text = "Task: " + text2;
                selectProjectTaskLabel.Visible = true;
                selectProjectComboBox.Enabled = true;
                selectProjectTaskComboBox.Enabled = true;
                (parentMainForm as MainWin).StartProject(text, text2);
                selectProjectComboBox.Enabled = false;
                selectProjectComboBox.Visible = false;
                selectProjectTaskComboBox.Enabled = false;
                selectProjectTaskComboBox.Visible = false;
            }
            state_MouseEnter(null, null);
        }

        private void Name_Click(object sender, EventArgs e)
        {
        }

        private void Drop_Click(object sender, EventArgs e)
        {
            ShowProjectsMenu();
        }

        private void ShowProjectsMenu()
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Create a project / Search for a project", null, create_project_clicked);
            toolStripMenuItem.AutoSize = true;
            toolStripMenuItem.Width = bgstuff.Width;
            toolStripMenuItem.Image = null;
            contextMenuStrip.Items.Add(toolStripMenuItem);
            _ = toolStripMenuItem.Height;
            for (int num = (parentMainForm as MainWin).recentProjects.Count - 1; num >= 0; num--)
            {
                ProjectItem recentProject = (parentMainForm as MainWin).recentProjects[num];
                List<ProjectItem> allProjects = (parentMainForm as MainWin).allProjects;
                toolStripMenuItem = new ToolStripMenuItem(recentProject.name, null, recentProjectItemClicked);
                toolStripMenuItem.AutoSize = true;
                toolStripMenuItem.Width = bgstuff.Width;
                toolStripMenuItem.Image = null;
                if (recentProject.tasks.Count > 0)
                {
                    for (int num2 = recentProject.tasks.Count - 1; num2 >= 0; num2--)
                    {
                        ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
                        toolStripMenuItem2.Tag = recentProject;
                        toolStripMenuItem2.Text = recentProject.tasks[num2].name;
                        toolStripMenuItem2.Click += recentTaskItemClicked;
                        toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
                    }
                }
                else
                {
                    ToolStripMenuItem value = new ToolStripMenuItem
                    {
                        Text = "No recent tasks",
                        Enabled = false
                    };
                    toolStripMenuItem.DropDownItems.Add(value);
                }
                int num3 = allProjects.IndexOf(allProjects.Where((ProjectItem p) => p.id == recentProject.id).FirstOrDefault());
                if (num3 > -1 && allProjects[num3].tasks.Count > 0)
                {
                    toolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
                    ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem
                    {
                        Text = "Show all"
                    };
                    for (int num4 = allProjects[num3].tasks.Count - 1; num4 >= 0; num4--)
                    {
                        ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem
                        {
                            Tag = allProjects[num3],
                            Text = allProjects[num3].tasks[num4].name
                        };
                        toolStripMenuItem4.Click += recentTaskItemClicked;
                        toolStripMenuItem3.DropDownItems.Add(toolStripMenuItem4);
                    }
                    toolStripMenuItem.DropDownItems.Add(toolStripMenuItem3);
                }
                contextMenuStrip.Items.Add(toolStripMenuItem);
            }
            Point p2 = new Point(0, 0);
            p2 = state.PointToScreen(p2);
            p2.X += bgstuff.Width - contextMenuStrip.Width;
            contextMenuStrip.Show(p2);
        }

        public void create_project_clicked(object sender, EventArgs e)
        {
            (parentMainForm as MainWin).createNewProjectMenuItem_Click(sender, e);
        }

        public void recentProjectItemClicked(object sender, EventArgs e)
        {
            (parentMainForm as MainWin).recentProjectItemClicked(sender, e);
        }

        public void recentTaskItemClicked(object sender, EventArgs e)
        {
            (parentMainForm as MainWin).TaskItemClicked(sender, e);
        }

        private void state_MouseEnter(object sender, EventArgs e)
        {
            state.ForeColor = Color.FromArgb(255, 255, 255);
            if ((parentMainForm as MainWin).currentProjectId != 0)
            {
                stateTitle("Stop timer");
                state.BackColor = Color.FromArgb(255, 64, 0);
                timer_track_led.BackColor = Color.FromArgb(255, 64, 0);
                timer_track_led.Visible = true;
            }
            else
            {
                stateTitle("Start timer");
                state.BackColor = Color.FromArgb(95, 185, 42);
                timer_track_led.BackColor = Color.FromArgb(95, 185, 42);
                timer_track_led.Visible = false;
            }
        }

        private void state_MouseLeave(object sender, EventArgs e)
        {
            state.BackColor = Color.FromArgb(255, 255, 255);
            state.ForeColor = Color.FromArgb(37, 45, 51);
            timer_track_led.BackColor = Color.FromArgb(255, 255, 255);
            if ((parentMainForm as MainWin).currentProjectId != 0)
            {
                _ = (parentMainForm as MainWin).currentProjectTime / 3600;
                _ = (parentMainForm as MainWin).currentProjectTime % 3600 / 60;
                stateTitle(Tools.GetPrettyTime((parentMainForm as MainWin).currentProjectTime));
                selectProjectComboBox.Enabled = false;
                selectProjectComboBox.Visible = false;
                selectProjectTaskComboBox.Enabled = false;
                selectProjectTaskComboBox.Visible = false;
                timer_track_led.Visible = true;
                string currentProjectName = (parentMainForm as MainWin).currentProjectName;
                string text = (parentMainForm as MainWin).currentTaskName;
                if (text == projectTaskComboBoxPlaceholder)
                {
                    text = "";
                }
                selectProjectLabel.Text = "Project: " + currentProjectName;
                selectProjectLabel.Visible = true;
                selectProjectTaskLabel.Text = "Task: " + text;
                selectProjectTaskLabel.Visible = true;
            }
            else
            {
                selectProjectComboBox.Enabled = true;
                selectProjectComboBox.Visible = true;
                selectProjectTaskComboBox.Enabled = true;
                selectProjectTaskComboBox.Visible = true;
                timer_track_led.Visible = false;
                stateTitle("Start timer");
                selectProjectLabel.Visible = false;
                selectProjectTaskLabel.Visible = false;
            }
        }

        private void stateTitle(string text)
        {
            state.Text = text;
            if (base.Visible && !state.Visible)
            {
                top.Text = text;
            }
        }

        private string lastProjectTitle()
        {
            string text = "";
            if ((parentMainForm as MainWin).lastProjectName != null)
            {
                text += (parentMainForm as MainWin).lastProjectName;
            }
            if ((parentMainForm as MainWin).lastTaskName != null && (parentMainForm as MainWin).lastTaskName != "")
            {
                text = text + " - " + (parentMainForm as MainWin).lastTaskName;
            }
            return text;
        }

        private void update_tick()
        {
            if (base.Visible)
            {
                if (name.Text != lastProjectTitle())
                {
                    UpdateProjectsList();
                    name.Text = lastProjectTitle();
                }
                Point position = Cursor.Position;
                position = state.PointToClient(position);
                if (position.X < 0 || position.Y < 0 || position.X > state.Width || position.Y > state.Height)
                {
                    state_MouseLeave(null, null);
                }
                Application.DoEvents();
            }
        }

        public void ShowProjectWin(bool center = false, bool stop = false)
        {
            if (state.InvokeRequired)
            {
                state.Invoke((MethodInvoker)delegate
                {
                    ShowProjectWin(center, stop);
                });
                return;
            }
            if (center && !base.Visible)
            {
                CenterToScreen();
            }
            if (!base.Visible)
            {
                base.Visible = true;
            }
            else
            {
                UpdateProjectsList();
            }
            if (stop)
            {
                state_MouseClick(null, null, forceLabels: true);
                selectProjectComboBox.Focus();
            }
        }

        public void UpdateProjectsList()
        {
            lock (UpdateProjectsListLock)
            {
                selectProjectComboBox.Items.Clear();
                selectProjectTaskComboBox.Items.Clear();
                List<ProjectItem> list = new List<ProjectItem>((parentMainForm as MainWin).recentProjects);
                List<ProjectItem> list2 = new List<ProjectItem>((parentMainForm as MainWin).allProjects);
                string currentProjectName = (parentMainForm as MainWin).currentProjectName;
                string currentTaskName = (parentMainForm as MainWin).currentTaskName;
                int count = list.Count;
                for (int num = count - 1; num >= 0; num--)
                {
                    ProjectItem projectItem = list[num];
                    selectProjectComboBox.Items.Add(projectItem.name);
                    if (currentProjectName == projectItem.name)
                    {
                        selectProjectComboBox.SelectedIndex = count - 1 - num;
                    }
                }
                for (int j = 0; j < list2.Count; j++)
                {
                    ProjectItem Item = list2[j];
                    if (list.IndexOf(list.Where((ProjectItem p) => p.name == Item.name).FirstOrDefault()) == -1)
                    {
                        selectProjectComboBox.Items.Add(Item.name);
                    }
                }
                int num2 = list.IndexOf(list.Where((ProjectItem p) => p.name == selectProjectComboBox.Text).FirstOrDefault());
                int num3 = list2.IndexOf(list2.Where((ProjectItem p) => p.name == selectProjectComboBox.Text).FirstOrDefault());
                List<TaskItem> list3 = null;
                if (num2 > -1)
                {
                    list3 = list[num2].tasks ?? null;
                    if (list3 != null)
                    {
                        for (int num4 = list3.Count - 1; num4 >= 0; num4--)
                        {
                            selectProjectTaskComboBox.Items.Add(list3[num4].name);
                            if (currentTaskName == list3[num4].name)
                            {
                                selectProjectTaskComboBox.SelectedIndex = list3.Count - 1 - num4;
                            }
                        }
                    }
                }
                if (num3 <= -1)
                {
                    return;
                }
                ProjectItem selectedAllProject = list2[num3];
                if (selectedAllProject.tasks.Count <= 0)
                {
                    return;
                }
                int num5 = -1;
                int i;
                for (i = 0; i < selectedAllProject.tasks.Count; i++)
                {
                    if (list3 != null)
                    {
                        num5 = list3.IndexOf(list3.Where((TaskItem p) => p.name == selectedAllProject.tasks[i].name).FirstOrDefault());
                    }
                    if (num5 == -1)
                    {
                        selectProjectTaskComboBox.Items.Add(selectedAllProject.tasks[i].name);
                    }
                }
            }
        }

        private void selectProjectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                selectProjectTaskComboBox.Items.Clear();
                selectProjectTaskComboBox.Text = projectTaskComboBoxPlaceholder;
                List<ProjectItem> recentProjects = (parentMainForm as MainWin).recentProjects;
                List<ProjectItem> allProjects = (parentMainForm as MainWin).allProjects;
                int num = recentProjects.IndexOf(recentProjects.Where((ProjectItem p) => p.name == selectProjectComboBox.SelectedItem.ToString()).FirstOrDefault());
                int num2 = allProjects.IndexOf(allProjects.Where((ProjectItem p) => p.name == selectProjectComboBox.SelectedItem.ToString()).FirstOrDefault());
                List<TaskItem> list = null;
                if (num > -1)
                {
                    list = recentProjects[num].tasks ?? null;
                    if (list != null)
                    {
                        for (int num3 = list.Count - 1; num3 >= 0; num3--)
                        {
                            selectProjectTaskComboBox.Items.Add(list[num3].name);
                            if ((parentMainForm as MainWin).currentTaskName == list[num3].name)
                            {
                                selectProjectTaskComboBox.SelectedIndex = list.Count - 1 - num3;
                            }
                        }
                    }
                }
                if (num2 <= -1)
                {
                    return;
                }
                ProjectItem selectedAllProject = allProjects[num2];
                if (selectedAllProject.tasks.Count <= 0)
                {
                    return;
                }
                int num4 = -1;
                int i;
                for (i = 0; i < selectedAllProject.tasks.Count; i++)
                {
                    if (list != null)
                    {
                        num4 = list.IndexOf(list.Where((TaskItem p) => p.name == selectedAllProject.tasks[i].name).FirstOrDefault());
                    }
                    if (num4 == -1)
                    {
                        selectProjectTaskComboBox.Items.Add(selectedAllProject.tasks[i].name);
                    }
                }
            }
            catch (Exception)
            {
                selectProjectTaskComboBox.Items.Clear();
            }
        }

        public void update_time_Tick(object sender, EventArgs e)
        {
            update_tick();
        }

        private void ProjectWindow_LocationChanged(object sender, EventArgs e)
        {
        }

        private void ProjectWindow_Move(object sender, EventArgs e)
        {
            if (started)
            {
                Application.UserAppDataRegistry.SetValue("pwx", base.Left.ToString());
                Application.UserAppDataRegistry.SetValue("pwy", base.Top.ToString());
            }
        }

        private void ProjectWindow_Load(object sender, EventArgs e)
        {
            SetWindowPos(base.Handle, HWND_TOPMOST, 0, 0, 0, 0, 3u);
        }

        private void minimize_Click(object sender, EventArgs e)
        {
            if (state.Visible)
            {
                base.Height -= state.Height + bg1.Height + bg2.Height + 3;
                state.Visible = false;
                name.Visible = false;
                top.Text = state.Text;
            }
            else
            {
                state.Visible = true;
                name.Visible = true;
                top.Text = "DeskTime";
                base.Height += state.Height + bg1.Height + bg2.Height + 3;
            }
        }

        private void top_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(base.Handle, 161, 2, 0);
            }
        }

        private void ProjectLedIndicator_Tick(object sender, EventArgs e)
        {
            if (timer_track_led_image == 1)
            {
                timer_track_led.Image = Resources.oval_2;
                timer_track_led_image = 2;
            }
            else
            {
                timer_track_led_image = 1;
                timer_track_led.Image = Resources.oval_1;
            }
        }

        private void selectProjectComboBox_DropDown(object sender, EventArgs e)
        {
            selectProjectComboBox.AutoCompleteMode = AutoCompleteMode.None;
        }

        private void selectProjectComboBox_DropDownClosed(object sender, EventArgs e)
        {
            selectProjectComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeskTime.ProjectWindow));
            update_time = new System.Windows.Forms.Timer(components);
            close = new System.Windows.Forms.Button();
            state = new System.Windows.Forms.Label();
            drop = new System.Windows.Forms.PictureBox();
            name = new System.Windows.Forms.Label();
            minimize = new System.Windows.Forms.Button();
            top = new System.Windows.Forms.Label();
            bgstuff = new System.Windows.Forms.Label();
            selectProjectComboBox = new System.Windows.Forms.ComboBox();
            selectProjectTaskComboBox = new System.Windows.Forms.ComboBox();
            bg1 = new System.Windows.Forms.Label();
            timer_track_led = new System.Windows.Forms.PictureBox();
            projectLedIndicator = new System.Windows.Forms.Timer(components);
            selectProjectLabel = new System.Windows.Forms.Label();
            bg2 = new System.Windows.Forms.Label();
            selectProjectTaskLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)drop).BeginInit();
            ((System.ComponentModel.ISupportInitialize)timer_track_led).BeginInit();
            SuspendLayout();
            update_time.Enabled = true;
            update_time.Interval = 1000;
            update_time.Tick += new System.EventHandler(update_time_Tick);
            close.BackColor = System.Drawing.Color.White;
            close.BackgroundImage = (System.Drawing.Image)resources.GetObject("close.BackgroundImage");
            close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            close.FlatAppearance.BorderSize = 0;
            close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            close.ForeColor = System.Drawing.Color.White;
            close.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            close.Location = new System.Drawing.Point(171, 1);
            close.Margin = new System.Windows.Forms.Padding(0);
            close.Name = "close";
            close.Size = new System.Drawing.Size(25, 23);
            close.TabIndex = 0;
            close.TabStop = false;
            close.UseVisualStyleBackColor = false;
            close.Click += new System.EventHandler(close_Click);
            state.BackColor = System.Drawing.Color.White;
            state.Cursor = System.Windows.Forms.Cursors.Hand;
            state.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 186);
            state.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            state.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            state.Location = new System.Drawing.Point(1, 81);
            state.Name = "state";
            state.Size = new System.Drawing.Size(195, 37);
            state.TabIndex = 103;
            state.Text = "23 hours 59 minutes";
            state.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            state.MouseClick += new System.Windows.Forms.MouseEventHandler(state_MouseClick);
            state.MouseEnter += new System.EventHandler(state_MouseEnter);
            state.MouseLeave += new System.EventHandler(state_MouseLeave);
            drop.BackColor = System.Drawing.Color.White;
            drop.BackgroundImage = (System.Drawing.Image)resources.GetObject("drop.BackgroundImage");
            drop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            drop.Location = new System.Drawing.Point(178, 263);
            drop.Name = "drop";
            drop.Size = new System.Drawing.Size(14, 15);
            drop.TabIndex = 18;
            drop.TabStop = false;
            drop.Click += new System.EventHandler(Drop_Click);
            name.AutoEllipsis = true;
            name.BackColor = System.Drawing.Color.White;
            name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 186);
            name.ForeColor = System.Drawing.Color.FromArgb(110, 116, 122);
            name.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            name.Location = new System.Drawing.Point(4, 261);
            name.Name = "name";
            name.Size = new System.Drawing.Size(164, 18);
            name.TabIndex = 100;
            name.Text = "Project name";
            name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            name.UseMnemonic = false;
            name.Click += new System.EventHandler(Name_Click);
            minimize.BackColor = System.Drawing.Color.White;
            minimize.BackgroundImage = (System.Drawing.Image)resources.GetObject("minimize.BackgroundImage");
            minimize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            minimize.FlatAppearance.BorderSize = 0;
            minimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            minimize.ForeColor = System.Drawing.Color.White;
            minimize.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            minimize.Location = new System.Drawing.Point(145, 1);
            minimize.Margin = new System.Windows.Forms.Padding(0);
            minimize.Name = "minimize";
            minimize.Size = new System.Drawing.Size(26, 23);
            minimize.TabIndex = 0;
            minimize.TabStop = false;
            minimize.UseVisualStyleBackColor = false;
            minimize.Click += new System.EventHandler(minimize_Click);
            top.AutoEllipsis = true;
            top.BackColor = System.Drawing.Color.White;
            top.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 186);
            top.ForeColor = System.Drawing.Color.FromArgb(53, 62, 68);
            top.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            top.Location = new System.Drawing.Point(1, 1);
            top.Margin = new System.Windows.Forms.Padding(0);
            top.Name = "top";
            top.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            top.Size = new System.Drawing.Size(144, 23);
            top.TabIndex = 21;
            top.Text = "DeskTime";
            top.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            top.MouseDown += new System.Windows.Forms.MouseEventHandler(top_MouseDown);
            bgstuff.BackColor = System.Drawing.Color.White;
            bgstuff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 186);
            bgstuff.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            bgstuff.Location = new System.Drawing.Point(1, 260);
            bgstuff.Name = "bgstuff";
            bgstuff.Size = new System.Drawing.Size(195, 23);
            bgstuff.TabIndex = 17;
            bgstuff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            selectProjectComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            selectProjectComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            selectProjectComboBox.BackColor = System.Drawing.Color.White;
            selectProjectComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            selectProjectComboBox.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            selectProjectComboBox.FormattingEnabled = true;
            selectProjectComboBox.Location = new System.Drawing.Point(5, 28);
            selectProjectComboBox.Margin = new System.Windows.Forms.Padding(3, 3, 3, 30);
            selectProjectComboBox.Name = "selectProjectComboBox";
            selectProjectComboBox.Size = new System.Drawing.Size(187, 21);
            selectProjectComboBox.TabIndex = 101;
            selectProjectComboBox.Text = "Project Name";
            selectProjectComboBox.DropDown += new System.EventHandler(selectProjectComboBox_DropDown);
            selectProjectComboBox.SelectedIndexChanged += new System.EventHandler(selectProjectComboBox_SelectedIndexChanged);
            selectProjectComboBox.SelectionChangeCommitted += new System.EventHandler(selectProjectComboBox_SelectedIndexChanged);
            selectProjectComboBox.DropDownClosed += new System.EventHandler(selectProjectComboBox_DropDownClosed);
            selectProjectTaskComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            selectProjectTaskComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            selectProjectTaskComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            selectProjectTaskComboBox.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            selectProjectTaskComboBox.FormattingEnabled = true;
            selectProjectTaskComboBox.Location = new System.Drawing.Point(5, 56);
            selectProjectTaskComboBox.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            selectProjectTaskComboBox.Name = "selectProjectTaskComboBox";
            selectProjectTaskComboBox.Size = new System.Drawing.Size(187, 21);
            selectProjectTaskComboBox.TabIndex = 102;
            selectProjectTaskComboBox.Text = "Task Name";
            bg1.BackColor = System.Drawing.Color.White;
            bg1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            bg1.Location = new System.Drawing.Point(1, 25);
            bg1.Name = "bg1";
            bg1.Size = new System.Drawing.Size(195, 27);
            bg1.TabIndex = 103;
            timer_track_led.BackColor = System.Drawing.Color.White;
            timer_track_led.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            timer_track_led.Image = DeskTime.Properties.Resources.oval_1;
            timer_track_led.Location = new System.Drawing.Point(8, 91);
            timer_track_led.Name = "timer_track_led";
            timer_track_led.Size = new System.Drawing.Size(18, 18);
            timer_track_led.TabIndex = 104;
            timer_track_led.TabStop = false;
            projectLedIndicator.Enabled = true;
            projectLedIndicator.Interval = 1500;
            projectLedIndicator.Tick += new System.EventHandler(ProjectLedIndicator_Tick);
            selectProjectLabel.AutoEllipsis = true;
            selectProjectLabel.BackColor = System.Drawing.Color.White;
            selectProjectLabel.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            selectProjectLabel.Location = new System.Drawing.Point(1, 29);
            selectProjectLabel.Name = "selectProjectLabel";
            selectProjectLabel.Padding = new System.Windows.Forms.Padding(4, 0, 0, 4);
            selectProjectLabel.Size = new System.Drawing.Size(195, 26);
            selectProjectLabel.TabIndex = 105;
            selectProjectLabel.Text = "Project Name";
            selectProjectLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            selectProjectLabel.Visible = false;
            bg2.BackColor = System.Drawing.Color.White;
            bg2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            bg2.Location = new System.Drawing.Point(1, 53);
            bg2.Name = "bg2";
            bg2.Size = new System.Drawing.Size(195, 27);
            bg2.TabIndex = 106;
            selectProjectTaskLabel.AutoEllipsis = true;
            selectProjectTaskLabel.BackColor = System.Drawing.Color.White;
            selectProjectTaskLabel.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            selectProjectTaskLabel.Location = new System.Drawing.Point(1, 52);
            selectProjectTaskLabel.Name = "selectProjectTaskLabel";
            selectProjectTaskLabel.Padding = new System.Windows.Forms.Padding(4, 1, 0, 4);
            selectProjectTaskLabel.Size = new System.Drawing.Size(195, 27);
            selectProjectTaskLabel.TabIndex = 107;
            selectProjectTaskLabel.Text = "Task Name";
            selectProjectTaskLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            selectProjectTaskLabel.Visible = false;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(218, 218, 218);
            base.ClientSize = new System.Drawing.Size(197, 119);
            base.Controls.Add(selectProjectTaskLabel);
            base.Controls.Add(selectProjectLabel);
            base.Controls.Add(timer_track_led);
            base.Controls.Add(selectProjectComboBox);
            base.Controls.Add(selectProjectTaskComboBox);
            base.Controls.Add(bg1);
            base.Controls.Add(top);
            base.Controls.Add(minimize);
            base.Controls.Add(drop);
            base.Controls.Add(name);
            base.Controls.Add(bgstuff);
            base.Controls.Add(close);
            base.Controls.Add(state);
            base.Controls.Add(bg2);
            DoubleBuffered = true;
            ForeColor = System.Drawing.SystemColors.ControlText;
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            base.MinimizeBox = false;
            base.Name = "ProjectWindow";
            base.ShowInTaskbar = false;
            Text = "ProjectWindow";
            base.TopMost = true;
            base.Load += new System.EventHandler(ProjectWindow_Load);
            base.LocationChanged += new System.EventHandler(ProjectWindow_LocationChanged);
            base.Move += new System.EventHandler(ProjectWindow_Move);
            ((System.ComponentModel.ISupportInitialize)drop).EndInit();
            ((System.ComponentModel.ISupportInitialize)timer_track_led).EndInit();
            ResumeLayout(false);
        }
    }
}
