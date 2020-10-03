namespace LadderApp
{
    partial class ProjectForm
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Device");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Addressing");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Configuration", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Ladder Program");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Input");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Output");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Memories");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Timer");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Counter");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Addressing", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Project", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4,
            treeNode10});
            this.tvnProjectTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // tvnProjectTree
            // 
            this.tvnProjectTree.Location = new System.Drawing.Point(9, 10);
            this.tvnProjectTree.Margin = new System.Windows.Forms.Padding(2);
            this.tvnProjectTree.Name = "tvnProjectTree";
            treeNode1.Name = "tvnDeviceNode";
            treeNode1.Text = "Device";
            treeNode2.Name = "tvnAddressingConfigurationNode";
            treeNode2.Text = "Addressing";
            treeNode3.Name = "tvnConfigurationNode";
            treeNode3.Text = "Configuration";
            treeNode4.Name = "tvnLadderProgramNode";
            treeNode4.Text = "Ladder Program";
            treeNode5.Name = "tvnInputsNode";
            treeNode5.Text = "Input";
            treeNode6.Name = "tvnOutputsNode";
            treeNode6.Text = "Output";
            treeNode7.Name = "tvnMemoriesNode";
            treeNode7.Text = "Memories";
            treeNode8.Name = "tvnTimersNode";
            treeNode8.Text = "Timer";
            treeNode9.Name = "tvnCountersNode";
            treeNode9.Text = "Counter";
            treeNode10.Name = "tvnAddressingNode";
            treeNode10.Text = "Addressing";
            treeNode11.Name = "tvnProjectNode";
            treeNode11.Text = "Project";
            this.tvnProjectTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode11});
            this.tvnProjectTree.Size = new System.Drawing.Size(202, 192);
            this.tvnProjectTree.TabIndex = 0;
            this.tvnProjectTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvnProjectTree_NodeMouseDoubleClick);
            // 
            // ProjetoLadder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 211);
            this.Controls.Add(this.tvnProjectTree);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ProjetoLadder";
            this.Text = " Project";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProjetoLadder_FormClosing);
            this.Load += new System.EventHandler(this.ProjetoLadder_Load);
            this.Shown += new System.EventHandler(this.ProjetoLadder_Shown);
            this.SizeChanged += new System.EventHandler(this.ProjetoLadder_Resize);
            this.Resize += new System.EventHandler(this.ProjetoLadder_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView tvnProjectTree;

    }
}