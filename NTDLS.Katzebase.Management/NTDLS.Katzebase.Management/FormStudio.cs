using NTDLS.Helpers;
using NTDLS.Katzebase.Client;
using NTDLS.Katzebase.Management.Classes;
using NTDLS.Katzebase.Management.Controls;
using NTDLS.Katzebase.Management.Properties;
using System.Data;
using System.Diagnostics;
using System.Text;
using static NTDLS.Katzebase.Management.Controls.TabFilePage;

namespace NTDLS.Katzebase.Management
{
    public partial class FormStudio : Form
    {
        private bool _timerTicking = false;
        private bool _firstShown = true;
        private readonly EditorFactory? _editorFactory = null;
        private readonly ImageList _treeImages = new();
        private readonly System.Windows.Forms.Timer _toolbarSyncTimer = new();

        public string _lastAddress = string.Empty;
        public int _lastPort;
        public string _lastUsername = string.Empty;
        public string _lastPasswordHash = string.Empty;

        private readonly string _firstLoadFilename = string.Empty;

        public FormStudio()
        {
            InitializeComponent();
            _editorFactory = new EditorFactory(this, tabControlBody);
            Text = KbConstants.FriendlyName;
        }

        public FormStudio(string firstLoadFilename)
        {
            InitializeComponent();
            _editorFactory = new EditorFactory(this, tabControlBody);
            _firstLoadFilename = firstLoadFilename;
            Text = KbConstants.FriendlyName;
        }

        private void FormStudio_Load(object sender, EventArgs e)
        {
            treeViewProject.Dock = DockStyle.Fill;
            splitContainerObjectExplorer.Dock = DockStyle.Fill;
            splitContainerMacros.Dock = DockStyle.Fill;
            tabControlBody.Dock = DockStyle.Fill;
            treeViewMacros.Dock = DockStyle.Fill;

            _treeImages.ColorDepth = ColorDepth.Depth32Bit;
            _treeImages.Images.Add("Folder", Resources.TreeFolder);
            _treeImages.Images.Add("Server", Resources.TreeServer);
            _treeImages.Images.Add("Schema", Resources.TreeSchema);
            _treeImages.Images.Add("Index", Resources.TreeIndex);
            _treeImages.Images.Add("FieldFolder", Resources.TreeDocument);
            _treeImages.Images.Add("Field", Resources.TreeField);
            _treeImages.Images.Add("IndexFolder", Resources.TreeIndexFolder);
            _treeImages.Images.Add("TreeNotLoaded", Resources.TreeNotLoaded);
            treeViewProject.ImageList = _treeImages;

            treeViewProject.BeforeExpand += TreeViewProject_BeforeExpand;
            treeViewProject.NodeMouseClick += TreeViewProject_NodeMouseClick;

            treeViewMacros.ShowNodeToolTips = true;
            //treeViewMacros.Nodes.AddRange(...);
            treeViewMacros.ItemDrag += TreeViewMacros_ItemDrag;

            Shown += FormStudio_Shown;
            FormClosing += FormStudio_FormClosing;

            tabControlBody.MouseUp += TabControlBody_MouseUp;

            splitContainerMacros.Panel2Collapsed = true;

            splitContainerObjectExplorer.SplitterDistance = Preferences.Instance.ObjectExplorerSplitterDistance;
            Width = Preferences.Instance.FormStudioWidth;
            Height = Preferences.Instance.FormStudioHeight;

            _toolbarSyncTimer.Tick += ToolbarSyncTimer_Tick;
            _toolbarSyncTimer.Interval = 250;
            _toolbarSyncTimer.Start();

            // Get the screen where the mouse cursor is located
            var screen = Screen.FromPoint(Cursor.Position);

            // Center the form on the screen
            StartPosition = FormStartPosition.Manual;

            Location = new Point(
                screen.Bounds.X + (screen.Bounds.Width - this.Width) / 2,
                screen.Bounds.Y + (screen.Bounds.Height - this.Height) / 2
            );
        }

        private void ToolbarSyncTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                lock (this)
                {
                    if (_timerTicking)
                    {
                        return;
                    }
                    _timerTicking = true;
                }

                SyncToolbarAndMenuStates();

                _timerTicking = false;
            }
            catch { }
        }

        private void SyncToolbarAndMenuStates()
        {
            var tabFilePage = CurrentTabFilePage();

            toolStripStatusLabelServerName.Text = $"Server: {tabFilePage?.Client?.Host}:{tabFilePage?.Client?.Port}";
            toolStripStatusLabelProcessId.Text = "PID: " + tabFilePage?.Client?.ProcessId.ToString("N0") ?? string.Empty;

            bool isTabOpen = (tabFilePage != null);
            bool isTextSelected = (tabFilePage != null) && (tabFilePage?.Editor?.SelectionLength > 0);

            toolStripButtonCloseCurrentTab.Enabled = isTabOpen;
            toolStripButtonCopy.Enabled = isTextSelected;
            toolStripButtonCut.Enabled = isTextSelected;
            toolStripButtonFind.Enabled = isTabOpen;
            toolStripButtonPaste.Enabled = isTabOpen;
            toolStripButtonRedo.Enabled = isTabOpen;
            toolStripButtonReplace.Enabled = isTabOpen;
            toolStripButtonExecuteScript.Enabled = isTabOpen && (tabFilePage?.IsScriptExecuting == false);
            toolStripButtonExplainPlan.Enabled = isTabOpen && (tabFilePage?.IsScriptExecuting == false);
            toolStripButtonStop.Enabled = isTabOpen && (tabFilePage?.IsScriptExecuting == true);

            toolStripButtonUndo.Enabled = isTabOpen;

            toolStripButtonDecreaseIndent.Enabled = isTextSelected;
            toolStripButtonIncreaseIndent.Enabled = isTextSelected;

            toolStripButtonSave.Enabled = isTabOpen;
            toolStripButtonSaveAll.Enabled = isTabOpen;
            toolStripButtonSnippets.Enabled = isTabOpen;
        }

        #region Form events.

        private void FormStudio_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (CloseAllTabs() == false)
            {
                e.Cancel = true;
            }
        }

        private void FormStudio_Shown(object? sender, EventArgs e)
        {
            if (_firstShown == false)
            {
                return;
            }

            if (_firstShown)
            {
                try
                {
                    _firstShown = false;

                    Connect();

                    if (string.IsNullOrEmpty(_firstLoadFilename))
                    {
                        var tabFilePage = CreateNewTab(_lastAddress, _lastPort, _lastUsername, _lastPasswordHash, FormUtility.GetNextNewFileName());
                        tabFilePage.Editor.Text = "set TraceWaitTimes false\r\nGO\r\n";
                        tabFilePage.Editor.SelectionStart = tabFilePage.Editor.Text.Length;
                        tabFilePage.IsSaved = true;
                    }
                    else
                    {
                        CreateNewTab(_lastAddress, _lastPort, _lastUsername, _lastPasswordHash, Path.GetFileName(_firstLoadFilename)).OpenFile(_firstLoadFilename);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", KbConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            SyncToolbarAndMenuStates();
        }

        #endregion

        #region Project Treeview Shenanigans.

        private void FlattenedTreeViewNodes(ref List<ServerTreeNode> flatList, ServerTreeNode parent)
        {
            foreach (var node in parent.Nodes.Cast<ServerTreeNode>())
            {
                flatList.Add(node);
                FlattenedTreeViewNodes(ref flatList, node);
            }
        }

        private ServerTreeNode? GetProjectAssetsNode(ServerTreeNode startNode)
        {
            foreach (var node in startNode.Nodes.Cast<ServerTreeNode>())
            {
                if (node.Text == "Assets")
                {
                    return node;
                }
                var result = GetProjectAssetsNode(node);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }


        private void TreeViewProject_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            var popupMenu = new ContextMenuStrip();
            treeViewProject.SelectedNode = e.Node;

            popupMenu.ItemClicked += PopupMenu_ItemClicked;

            var node = e.Node as ServerTreeNode;
            if (node == null)
            {
                throw new Exception("Invalid node type.");
            }

            popupMenu.Tag = e.Node as ServerTreeNode;

            if (node.NodeType == Constants.ServerNodeType.Server)
            {
                popupMenu.Items.Add("Refresh", FormUtility.TransparentImage(Resources.ToolFind));
            }
            else if (node.NodeType == Constants.ServerNodeType.Schema)
            {
                popupMenu.Items.Add("Create Index", FormUtility.TransparentImage(Resources.Asset));
                popupMenu.Items.Add("Select top n...", FormUtility.TransparentImage(Resources.Workload));
                popupMenu.Items.Add("Sample Schema", FormUtility.TransparentImage(Resources.Workload));
                popupMenu.Items.Add("Analyze Schema", FormUtility.TransparentImage(Resources.Workload));
                popupMenu.Items.Add("-");
                popupMenu.Items.Add("Drop Schema", FormUtility.TransparentImage(Resources.Asset));
                popupMenu.Items.Add("-");
                popupMenu.Items.Add("Refresh", FormUtility.TransparentImage(Resources.ToolFind));
            }
            else if (node.NodeType == Constants.ServerNodeType.IndexFolder)
            {
                popupMenu.Items.Add("Create Index", FormUtility.TransparentImage(Resources.Asset));
                popupMenu.Items.Add("-");
                popupMenu.Items.Add("Refresh", FormUtility.TransparentImage(Resources.ToolFind));
            }
            else if (node.NodeType == Constants.ServerNodeType.Index)
            {
                popupMenu.Items.Add("Analyze Index", FormUtility.TransparentImage(Resources.Asset));
                popupMenu.Items.Add("Script Index", FormUtility.TransparentImage(Resources.Asset));
                popupMenu.Items.Add("-");
                popupMenu.Items.Add("Rebuild Index", FormUtility.TransparentImage(Resources.Asset));
                popupMenu.Items.Add("Drop Index", FormUtility.TransparentImage(Resources.Asset));
                popupMenu.Items.Add("-");
                popupMenu.Items.Add("Refresh", FormUtility.TransparentImage(Resources.ToolFind));
            }

            popupMenu.Show(treeViewProject, e.Location);
        }

        private void PopupMenu_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                var menuStrip = (sender as ContextMenuStrip).EnsureNotNull();
                var node = (menuStrip.Tag as ServerTreeNode).EnsureNotNull();

                if (e.ClickedItem?.Text == "Refresh")
                {
                    if (node.NodeType == Constants.ServerNodeType.Server)
                    {
                        node.Nodes.Clear();
                        TreeManagement.PopulateServer(treeViewProject, node.ServerAddress, node.ServerPort, node.Username, node.PasswordHash);
                        foreach (TreeNode expandNode in treeViewProject.Nodes)
                        {
                            expandNode.Expand();
                        }
                    }
                    else if (node.NodeType == Constants.ServerNodeType.Schema)
                    {
                        node.Nodes.Clear();
                        TreeManagement.PopulateSchemaNodeOnExpand(treeViewProject, node);
                    }
                }
                else if (e.ClickedItem?.Text == "Delete")
                {
                    /*
                    var messageBoxResult = MessageBox.Show($"Delete {node.Text}?", $"Delete {node.NodeType}?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (messageBoxResult == DialogResult.Yes)
                    {
                        node.Remove();
                    }
                    */
                }
                else if (e.ClickedItem?.Text == "Select top n...")
                {
                    var rootNode = TreeManagement.GetRootNode(node);
                    var tabFilePage = CreateNewTab(rootNode.ServerAddress, rootNode.ServerPort, rootNode.Username, rootNode.PasswordHash, FormUtility.GetNextNewFileName());
                    tabFilePage.Editor.Text = $"SELECT TOP 100\r\n\t*\r\nFROM\r\n\t{TreeManagement.FullSchemaPath(node)}\r\n";
                    tabFilePage.Editor.SelectionStart = tabFilePage.Editor.Text.Length;
                    tabFilePage.ExecuteCurrentScriptAsync(ExecuteType.Execute);
                }
                else if (e.ClickedItem?.Text == "Analyze Schema")
                {
                    var rootNode = TreeManagement.GetRootNode(node);
                    var tabFilePage = CreateNewTab(rootNode.ServerAddress, rootNode.ServerPort, rootNode.Username, rootNode.PasswordHash, FormUtility.GetNextNewFileName());
                    tabFilePage.Editor.Text = $"ANALYZE SCHEMA {TreeManagement.FullSchemaPath(node)} --WITH (IncludePhysicalPages = true)\r\n";
                    tabFilePage.Editor.SelectionStart = tabFilePage.Editor.Text.Length;
                    tabFilePage.ExecuteCurrentScriptAsync(ExecuteType.Execute);
                }
                else if (e.ClickedItem?.Text == "Sample Schema")
                {
                    var rootNode = TreeManagement.GetRootNode(node);
                    var tabFilePage = CreateNewTab(rootNode.ServerAddress, rootNode.ServerPort, rootNode.Username, rootNode.PasswordHash, FormUtility.GetNextNewFileName());
                    tabFilePage.Editor.Text = $"SAMPLE {TreeManagement.FullSchemaPath(node)} 100\r\n";
                    tabFilePage.Editor.SelectionStart = tabFilePage.Editor.Text.Length;
                    tabFilePage.TabSplitContainer.SplitterDistance = 60;
                    tabFilePage.ExecuteCurrentScriptAsync(ExecuteType.Execute);
                }
                else if (e.ClickedItem?.Text == "Drop Index")
                {
                    var rootNode = TreeManagement.GetRootNode(node);
                    var tabFilePage = CreateNewTab(rootNode.ServerAddress, rootNode.ServerPort, rootNode.Username, rootNode.PasswordHash, FormUtility.GetNextNewFileName());
                    tabFilePage.Editor.Text = $"DROP INDEX {node.Text} ON {TreeManagement.FullSchemaPath(node)}\r\n";
                    tabFilePage.Editor.SelectionStart = tabFilePage.Editor.Text.Length;
                    tabFilePage.TabSplitContainer.SplitterDistance = 60;
                }
                else if (e.ClickedItem?.Text == "Analyze Index")
                {
                    var rootNode = TreeManagement.GetRootNode(node);
                    var tabFilePage = CreateNewTab(rootNode.ServerAddress, rootNode.ServerPort, rootNode.Username, rootNode.PasswordHash, FormUtility.GetNextNewFileName());
                    tabFilePage.Editor.Text = $"ANALYZE INDEX {node.Text} ON {TreeManagement.FullSchemaPath(node)}\r\n";
                    tabFilePage.Editor.SelectionStart = tabFilePage.Editor.Text.Length;
                    tabFilePage.TabSplitContainer.SplitterDistance = 60;
                    tabFilePage.ExecuteCurrentScriptAsync(ExecuteType.Execute);
                }
                else if (e.ClickedItem?.Text == "Rebuild Index")
                {
                    var rootNode = TreeManagement.GetRootNode(node);
                    using (var client = new KbClient(rootNode.ServerAddress, rootNode.ServerPort, rootNode.Username, rootNode.PasswordHash, $"{KbConstants.FriendlyName}.UI.Query"))
                    {
                        client.QueryTimeout = TimeSpan.FromDays(10); //TODO: Make this configurable.

                        var result = client.Schema.Indexes.Get(TreeManagement.FullSchemaPath(node), node.Text);
                        if (result != null && result.Index != null)
                        {
                            var text = new StringBuilder("REBUILD ");
                            text.Append(result.Index.IsUnique ? "UNIQUEKEY" : "INDEX");
                            text.Append($" {result.Index.Name} ON {TreeManagement.FullSchemaPath(node)}");
                            text.AppendLine($" WITH (PARTITIONS={result.Index.Partitions})");

                            var tabFilePage = CreateNewTab(rootNode.ServerAddress, rootNode.ServerPort, rootNode.Username, rootNode.PasswordHash, FormUtility.GetNextNewFileName());
                            tabFilePage.Editor.Text = text.ToString();
                            tabFilePage.Editor.SelectionStart = tabFilePage.Editor.Text.Length;
                            tabFilePage.TabSplitContainer.SplitterDistance = 60;
                        }
                    }
                }
                else if (e.ClickedItem?.Text == "Script Index")
                {
                    var rootNode = TreeManagement.GetRootNode(node);
                    using (var client = new KbClient(rootNode.ServerAddress, rootNode.ServerPort, "admin", "password", $"{KbConstants.FriendlyName}.UI.Query"))
                    {
                        client.QueryTimeout = TimeSpan.FromDays(10); //TODO: Make this configurable.

                        var result = client.Schema.Indexes.Get(TreeManagement.FullSchemaPath(node), node.Text);
                        if (result != null && result.Index != null)
                        {
                            var text = new StringBuilder("CREATE ");
                            text.Append(result.Index.IsUnique ? "UNIQUEKEY" : "INDEX");
                            text.Append($" {result.Index.Name}");
                            text.AppendLine("(");
                            foreach (var attribute in result.Index.Attributes)
                            {
                                text.AppendLine($"    {attribute.Field},");
                            }
                            text.Length -= 3;//Remove trialing ",\r\n"
                            text.Append($"\r\n) ON {TreeManagement.FullSchemaPath(node)}");
                            text.AppendLine($" WITH (PARTITIONS={result.Index.Partitions})");

                            var tabFilePage = CreateNewTab(rootNode.ServerAddress, rootNode.ServerPort, rootNode.Username, rootNode.PasswordHash, FormUtility.GetNextNewFileName());
                            tabFilePage.Editor.Text = text.ToString();
                            tabFilePage.Editor.SelectionStart = tabFilePage.Editor.Text.Length;
                            tabFilePage.TabSplitContainer.SplitterDistance = 60;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", KbConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Project Treeview Events.

        private void TreeViewProject_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            try
            {
                if (e.Node != null && (e.Node as ServerTreeNode)?.NodeType == Classes.Constants.ServerNodeType.Schema)
                {
                    TreeManagement.PopulateSchemaNodeOnExpand(treeViewProject, (ServerTreeNode)e.Node);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", KbConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Macros Treeview Bullshit.

        private void TreeViewMacros_ItemDrag(object? sender, ItemDragEventArgs e)
        {
            if (e.Item != null)
            {
                DoDragDrop(e.Item, DragDropEffects.All);
            }
        }

        #endregion

        #region Body Tab Magic.

        private void TabControlBody_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var clickedTab = GetClickedTab(e.Location);
                if (clickedTab == null)
                {
                    return;
                }

                var popupMenu = new ContextMenuStrip();
                popupMenu.ItemClicked += PopupMenu_tabControlScripts_MouseUp_ItemClicked;

                popupMenu.Tag = clickedTab;

                popupMenu.Items.Add("Close", FormUtility.TransparentImage(Properties.Resources.ToolCloseFile));
                popupMenu.Items.Add("-");
                popupMenu.Items.Add("Close all but this", FormUtility.TransparentImage(Properties.Resources.ToolCloseFile));
                popupMenu.Items.Add("Close all", FormUtility.TransparentImage(Properties.Resources.ToolCloseFile));
                popupMenu.Items.Add("-");
                popupMenu.Items.Add("Find in project", FormUtility.TransparentImage(Properties.Resources.ToolFind));
                popupMenu.Items.Add("Open containing folder", FormUtility.TransparentImage(Properties.Resources.ToolOpenFile));
                popupMenu.Show(tabControlBody, e.Location);
            }
        }

        private void PopupMenu_tabControlScripts_MouseUp_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            var contextMenu = sender as ContextMenuStrip;
            if (contextMenu == null)
            {
                return;
            }

            contextMenu.Hide();

            ToolStripItem? clickedItem = e?.ClickedItem;
            if (clickedItem == null)
            {
                return;
            }

            TabFilePage? clickedTab = contextMenu.Tag as TabFilePage;
            if (clickedTab == null)
            {
                return;
            }

            if (clickedItem.Text == "Close")
            {
                CloseTab(clickedTab);
            }
            else if (clickedItem.Text == "Open containing folder")
            {
                if (clickedTab != null)
                {
                    var directory = clickedTab.FilePath;

                    if (Directory.Exists(clickedTab.FilePath) == false)
                    {
                        directory = Path.GetDirectoryName(directory);
                    }

                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = directory,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
            }
            else if (clickedItem.Text == "Close all but this")
            {
                var tabsToClose = new List<TabFilePage>();

                //Minimize the number of "SelectedIndexChanged" events that get fired.
                //We get a big ol' thread exception when we don't do  Looks like an internal control exception.
                tabControlBody.SelectedTab = clickedTab;
                System.Windows.Forms.Application.DoEvents(); //Make sure the message pump can actually select the tab before we start closing.

                foreach (var tabFilePage in tabControlBody.TabPages.Cast<TabFilePage>())
                {
                    if (tabFilePage != clickedTab)
                    {
                        tabsToClose.Add(tabFilePage);
                    }
                }

                foreach (var tabFilePage in tabsToClose)
                {
                    if (CloseTab(tabFilePage) == false)
                    {
                        break;
                    }
                }
            }
            else if (clickedItem.Text == "Close all")
            {
                CloseAllTabs();
            }

            //UpdateToolbarButtonStates();
        }

        private TabFilePage? GetClickedTab(Point mouseLocation)
        {
            for (int i = 0; i < tabControlBody.TabCount; i++)
            {
                Rectangle r = tabControlBody.GetTabRect(i);
                if (r.Contains(mouseLocation))
                {
                    return (TabFilePage)tabControlBody.TabPages[i];
                }
            }
            return null;
        }

        private TabFilePage? FindTabByFileName(string filePath)
        {
            foreach (var tab in tabControlBody.TabPages.Cast<TabFilePage>())
            {
                if (tab.FilePath.ToLower() == filePath.ToLower())
                {
                    return tab;
                }
            }
            return null;
        }

        private TabFilePage CreateNewTab(string serverAddress, int serverPort, string username, string passwordHash, string tabText = "")
        {
            return TabFilePage.Create(_editorFactory.EnsureNotNull(), serverAddress, serverPort, username, passwordHash, tabText);
        }


        /// <summary>
        /// Removes a tab, saved or not - no prompting.
        /// </summary>
        /// <param name="tab"></param>
        private void RemoveTab(TabFilePage? tab)
        {
            if (tab != null)
            {
                tabControlBody.TabPages.Remove(tab);
                tab.Dispose();
            }
            SyncToolbarAndMenuStates();
        }

        bool Connect()
        {
            try
            {
                using var form = new FormConnect();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _lastAddress = form.ServerHost;
                    _lastPort = form.ServerPort;
                    _lastUsername = form.Username;
                    _lastPasswordHash = form.PasswordHash;

                    TreeManagement.PopulateServer(treeViewProject, _lastAddress, _lastPort, _lastUsername, _lastPasswordHash);

                    foreach (TreeNode node in treeViewProject.Nodes)
                    {
                        node.Expand();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, KbConstants.FriendlyName);
            }

            return false;
        }

        bool Disconnect()
        {
            if (CloseAllTabs() == false)
            {
                return false;
            }

            treeViewProject.Nodes.Clear();

            return true;
        }

        bool CloseAllTabs()
        {
            //Minimize the number of "SelectedIndexChanged" events that get fired.
            //We get a big ol' thread exception when we don't do  Looks like an internal control exception.
            tabControlBody.SelectedIndex = 0;
            Application.DoEvents(); //Make sure the message pump can actually select the tab before we start closing.

            tabControlBody.SuspendLayout();

            bool result = true;
            while (tabControlBody.TabPages.Count != 0)
            {
                if (!CloseTab(tabControlBody.TabPages[tabControlBody.TabPages.Count - 1] as TabFilePage))
                {
                    result = false;
                    break;
                }
            }

            SyncToolbarAndMenuStates();

            tabControlBody.ResumeLayout();

            return result;
        }

        /// <summary>
        /// User friendly tab close.
        /// </summary>
        /// <param name="tab"></param>
        private bool CloseTab(TabFilePage? tab)
        {
            if (tab != null)
            {
                if (tab.IsSaved == false)
                {
                    var messageBoxResult = MessageBox.Show(
                        "Save \"" + tab.Text.Trim(['*']) + "\" before closing?", "Save File?",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (messageBoxResult == DialogResult.Yes)
                    {
                        if (SaveTab(tab) == false)
                        {
                            return false;
                        }
                    }
                    else if (messageBoxResult == DialogResult.No)
                    {
                    }
                    else //Cancel and otherwise.
                    {
                        SyncToolbarAndMenuStates();
                        return false;
                    }
                }

                RemoveTab(tab);
            }

            SyncToolbarAndMenuStates();
            return true;
        }

        private TabFilePage? CurrentTabFilePage()
        {
            var currentTab = tabControlBody.SelectedTab as TabFilePage;
            if (currentTab?.Editor != null)
            {
                return (TabFilePage)currentTab.Editor.Tag;
            }
            return null;
        }

        #endregion

        #region Toolbar Clicks.

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var form = new FormSettings();
            form.ShowDialog();
        }

        private void ExecuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTabFilePage()?.ExecuteCurrentScriptAsync(ExecuteType.Execute);
        }

        private void ExplainPlanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTabFilePage()?.ExecuteCurrentScriptAsync(ExecuteType.ExplainPlan);
        }

        private void ExplainOperationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTabFilePage()?.ExecuteCurrentScriptAsync(ExecuteType.ExplainOperations);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenTab();
        }

        private void SaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var selection = CurrentTabFilePage();
            if (selection != null)
            {
                SaveTab(selection);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selection = CurrentTabFilePage();
            if (selection != null)
            {
                SaveTabAs(selection);
            }
        }

        private void ToolStripButtonOpen_Click(object sender, EventArgs e)
        {
            OpenTab();
        }

        private void ToolStripButtonStop_Click(object sender, EventArgs e)
        {
            CurrentTabFilePage()?.ExecuteStopCommand();
        }

        private void ToolStripButtonExecuteScript_Click(object sender, EventArgs e)
        {
            CurrentTabFilePage()?.ExecuteCurrentScriptAsync(ExecuteType.Execute);
        }

        private void ToolStripButtonExplainPlan_Click(object sender, EventArgs e)
        {
            CurrentTabFilePage()?.ExecuteCurrentScriptAsync(ExecuteType.ExplainPlan);
        }

        private void ToolStripButtonCloseCurrentTab_Click(object sender, EventArgs e)
        {
            var selection = CurrentTabFilePage();
            CloseTab(selection);
        }

        private void ToolStripButtonSave_Click(object sender, EventArgs e)
        {
            var selection = CurrentTabFilePage();
            if (selection != null)
            {
                SaveTab(selection);
            }
        }

        bool SaveTab(TabFilePage tab)
        {
            if (tab.IsFileOpen == false)
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = $"{Client.KbConstants.FriendlyName} Script (*.kbs)|*.kbs|All files (*.*)|*.*";
                    sfd.FileName = tab.FilePath;
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        return tab.Save(sfd.FileName);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return tab.Save();
        }

        bool SaveTabAs(TabFilePage tab)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = $"{KbConstants.FriendlyName} Script (*.kbs)|*.kbs|All files (*.*)|*.*";
                sfd.FileName = tab.FilePath;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    return tab.Save(sfd.FileName);
                }
                else
                {
                    return false;
                }
            }
        }

        private bool OpenTab()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = $"{KbConstants.FriendlyName} Script (*.kbs)|*.kbs|All files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var alreadyOpenTab = FindTabByFileName(ofd.FileName);
                    if (alreadyOpenTab == null)
                    {
                        CreateNewTab(_lastAddress, _lastPort, _lastUsername, _lastPasswordHash, Path.GetFileName(ofd.FileName)).OpenFile(ofd.FileName);
                    }
                    else
                    {
                        tabControlBody.SelectedTab = alreadyOpenTab;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void ToolStripButtonSaveAll_Click(object sender, EventArgs e)
        {
            foreach (var tabFilePage in tabControlBody.TabPages.Cast<TabFilePage>())
            {
                if (SaveTab(tabFilePage) == false)
                {
                    return;
                }
            }
        }

        private void ToolStripButtonFind_Click(object sender, EventArgs e)
        {
            ShowFind();
        }

        private void ToolStripButtonReplace_Click(object sender, EventArgs e)
        {
            ShowReplace();
        }

        private void ToolStripButtonRedo_Click(object sender, EventArgs e)
        {
            var tabFilePage = CurrentTabFilePage();
            tabFilePage?.Editor.Redo();
        }

        private void ToolStripButtonUndo_Click(object sender, EventArgs e)
        {
            var tabFilePage = CurrentTabFilePage();
            tabFilePage?.Editor.Undo();
        }

        private void ToolStripButtonCut_Click(object sender, EventArgs e)
        {
            var tabFilePage = CurrentTabFilePage();
            tabFilePage?.Editor.Cut();
        }

        private void ToolStripButtonCopy_Click(object sender, EventArgs e)
        {
            var tabFilePage = CurrentTabFilePage();
            tabFilePage?.Editor.Copy();
        }

        private void ToolStripButtonPaste_Click(object sender, EventArgs e)
        {
            var tabFilePage = CurrentTabFilePage();
            tabFilePage?.Editor.Paste();
        }

        private void ToolStripButtonIncreaseIndent_Click(object sender, EventArgs e)
        {
            IncreaseCurrentTabIndent();
        }

        public void IncreaseCurrentTabIndent()
        {
            var tabFilePage = CurrentTabFilePage();
            if (tabFilePage != null)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void ToolStripButtonDecreaseIndent_Click(object sender, EventArgs e)
        {
            DecreaseCurrentTabIndent();
        }

        public void DecreaseCurrentTabIndent()
        {
            SendKeys.Send("+({TAB})");
        }

        private void toolStripButtonMacros_Click(object sender, EventArgs e)
        {
            splitContainerMacros.Panel2Collapsed = !splitContainerMacros.Panel2Collapsed;
        }

        private void ToolStripButtonProject_Click(object sender, EventArgs e)
        {
            splitContainerObjectExplorer.Panel1Collapsed = !splitContainerObjectExplorer.Panel1Collapsed;
        }

        public void ShowReplace()
        {
            var info = CurrentTabFilePage();
            if (info != null)
            {
                info.ReplaceTextForm.ShowDialog();
            }
        }

        public void ShowFind()
        {
            var info = CurrentTabFilePage();
            if (info != null)
            {
                info.FindTextForm.ShowDialog();
            }
        }

        public void FindNext()
        {
            var info = CurrentTabFilePage();
            if (info != null)
            {
                if (string.IsNullOrEmpty(info.FindTextForm.SearchText))
                {
                    info.FindTextForm.ShowDialog();
                }
                else
                {
                    info.FindTextForm.FindNext();
                }
            }
        }

        private void ToolStripButtonOutput_Click(object sender, EventArgs e)
        {
            var tab = CurrentTabFilePage();
            if (tab != null)
            {
                tab.CollapseSplitter = !tab.CollapseSplitter;
            }
        }

        private void ToolStripButtonSnippets_Click(object sender, EventArgs e)
        {
            var tabFilePage = CurrentTabFilePage();
            if (tabFilePage != null)
            {

                using (var form = new FormSnippets())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        tabFilePage.Editor.Document.Insert(tabFilePage.Editor.CaretOffset, form.SelectedSnippetText);
                    }
                }
            }
        }

        #endregion

        #region Form Menu.

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selection = CurrentTabFilePage();
            if (selection == null)
            {
                return;
            }
            SaveTab(selection);
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selection = CurrentTabFilePage();
            CloseTab(selection);
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var form = new FormAbout();
            form.ShowDialog();
        }

        private void SaveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var tabFilePage in tabControlBody.TabPages.Cast<TabFilePage>())
            {
                if (SaveTab(tabFilePage) == false)
                {
                    break;
                }
            }
        }

        private void ToolStripButtonNewFile_Click(object sender, EventArgs e)
        {
            try
            {
                var tabFilePage = CreateNewTab(_lastAddress, _lastPort, _lastUsername, _lastPasswordHash);
                tabFilePage.Editor.Text = "set TraceWaitTimes false\r\nGO\r\n";
                tabFilePage.Editor.SelectionStart = tabFilePage.Editor.Text.Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", KbConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void DisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllTabs();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        private void FormStudio_DragDrop(object sender, DragEventArgs e)
        {
            var files = e.Data?.GetData(DataFormats.FileDrop, false) as string[];
            if (files != null)
            {
                foreach (var file in files)
                {
                    var alreadyOpenTab = FindTabByFileName(file);
                    if (alreadyOpenTab == null)
                    {
                        CreateNewTab(_lastAddress, _lastPort, _lastUsername, _lastPasswordHash, Path.GetFileName(file)).OpenFile(file);
                    }
                    else
                    {
                        tabControlBody.SelectedTab = alreadyOpenTab;
                    }
                }
            }
        }

        private void FormStudio_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private string GetDraggedItemPath(ServerTreeNode node)
        {
            if (node.NodeType == Constants.ServerNodeType.Schema)
            {
                string path = string.Empty;
                while (node != null && node.NodeType != Constants.ServerNodeType.Server)
                {
                    path = $"{node.Text}:{path}";

                    node = (ServerTreeNode)node.Parent;
                }
                return path.Trim(':');
            }
            else if (node.NodeType == Constants.ServerNodeType.Field
                || node.NodeType == Constants.ServerNodeType.Index)
            {
                return node.Text;
            }

            return string.Empty;
        }

        private void TreeViewProject_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Item is TreeNode node)
            {
                string text = GetDraggedItemPath((ServerTreeNode)node);

                if (string.IsNullOrEmpty(text) == false)
                {
                    treeViewProject.DoDragDrop(text, DragDropEffects.Copy);
                }
            }
        }

        private void FormStudio_ResizeEnd(object sender, EventArgs e)
        {
            Preferences.Instance.FormStudioWidth = Width;
            Preferences.Instance.FormStudioHeight = Height;
        }

        private void SplitContainerProject_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Preferences.Instance.ObjectExplorerSplitterDistance = splitContainerObjectExplorer.SplitterDistance;
        }
    }
}
