namespace LadderApp
{
    partial class LadderForm
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
            this.components = new System.ComponentModel.Container();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.lblstatusMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.mnuContextAtInstruction = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAddressing = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMemory = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTimer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCounter = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuInput = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuClearAddress = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuInsertLadderLine = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuInsertLadderLineAbove = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuInsertLadderLineBelow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExtendParallelBranchAbove = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExtendParallelBranchBelow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToggleBit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToggleBitPulse = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTipOutputBox = new System.Windows.Forms.ToolTip(this.components);
            this.statusBar.SuspendLayout();
            this.mnuContextAtInstruction.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblstatusMessage});
            this.statusBar.Location = new System.Drawing.Point(0, 172);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusBar.Size = new System.Drawing.Size(594, 22);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "Pronto";
            // 
            // lblstatusMensagem
            // 
            this.lblstatusMessage.Name = "lblstatusMensagem";
            this.lblstatusMessage.Size = new System.Drawing.Size(39, 17);
            this.lblstatusMessage.Text = "Ready";
            // 
            // mnuContextAtInstruction
            // 
            this.mnuContextAtInstruction.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddressing,
            this.mnuInsertLadderLine,
            this.mnuExtendParallelBranchAbove,
            this.mnuExtendParallelBranchBelow,
            this.mnuToggleBit,
            this.mnuToggleBitPulse});
            this.mnuContextAtInstruction.Name = "mnuContextAtInstruction";
            this.mnuContextAtInstruction.Size = new System.Drawing.Size(205, 158);
            this.mnuContextAtInstruction.Text = "Menu";
            // 
            // mnuAddressing
            // 
            this.mnuAddressing.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMemory,
            this.mnuTimer,
            this.mnuCounter,
            this.mnuInput,
            this.mnuOutput,
            this.toolStripSeparator1,
            this.mnuClearAddress});
            this.mnuAddressing.Name = "mnuAddressing";
            this.mnuAddressing.Size = new System.Drawing.Size(204, 22);
            this.mnuAddressing.Text = "Addressing";
            // 
            // mnuMemory
            // 
            this.mnuMemory.Name = "mnuMemory";
            this.mnuMemory.Size = new System.Drawing.Size(180, 22);
            this.mnuMemory.Text = "Memory";
            // 
            // mnuTimer
            // 
            this.mnuTimer.Name = "mnuTimer";
            this.mnuTimer.Size = new System.Drawing.Size(180, 22);
            this.mnuTimer.Text = "Timer";
            // 
            // mnuCounter
            // 
            this.mnuCounter.Name = "mnuCounter";
            this.mnuCounter.Size = new System.Drawing.Size(180, 22);
            this.mnuCounter.Text = "Counter";
            // 
            // mnuInput
            // 
            this.mnuInput.Name = "mnuInput";
            this.mnuInput.Size = new System.Drawing.Size(180, 22);
            this.mnuInput.Text = "Input";
            // 
            // mnuOutput
            // 
            this.mnuOutput.Name = "mnuOutput";
            this.mnuOutput.Size = new System.Drawing.Size(180, 22);
            this.mnuOutput.Text = "Output";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // menuLimparEndereco
            // 
            this.mnuClearAddress.Name = "mnuClearAddress";
            this.mnuClearAddress.Size = new System.Drawing.Size(180, 22);
            this.mnuClearAddress.Text = "Clean";
            this.mnuClearAddress.ToolTipText = "Clear address of selected instruction.";
            this.mnuClearAddress.Click += new System.EventHandler(this.mnuClearAddress_Click);
            // 
            // menuInsereLinha
            // 
            this.mnuInsertLadderLine.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuInsertLadderLineAbove,
            this.mnuInsertLadderLineBelow});
            this.mnuInsertLadderLine.Name = "mnuInsertLadderLine";
            this.mnuInsertLadderLine.Size = new System.Drawing.Size(204, 22);
            this.mnuInsertLadderLine.Text = "Insert Line";
            // 
            // mnuInsertLadderLineAbove
            // 
            this.mnuInsertLadderLineAbove.Name = "mnuInsertLadderLineAbove";
            this.mnuInsertLadderLineAbove.Size = new System.Drawing.Size(180, 22);
            this.mnuInsertLadderLineAbove.Text = "Above";
            this.mnuInsertLadderLineAbove.Click += new System.EventHandler(this.mnuInsertLadderLineAbove_Click);
            // 
            // mnuInsertLadderLineBelow
            // 
            this.mnuInsertLadderLineBelow.Name = "mnuInsertLadderLineBelow";
            this.mnuInsertLadderLineBelow.Size = new System.Drawing.Size(180, 22);
            this.mnuInsertLadderLineBelow.Text = "Below";
            this.mnuInsertLadderLineBelow.Click += new System.EventHandler(this.mnuInsertLadderLineBelow_Click);
            // 
            // mnuExtendParallelBranchAbove
            // 
            this.mnuExtendParallelBranchAbove.Enabled = false;
            this.mnuExtendParallelBranchAbove.Name = "mnuExtendParallelBranchAbove";
            this.mnuExtendParallelBranchAbove.Size = new System.Drawing.Size(204, 22);
            this.mnuExtendParallelBranchAbove.Text = "Extend Parallel Branch Above";
            this.mnuExtendParallelBranchAbove.Visible = true;
            // 
            // mnuExtendParallelBranchBelow
            // 
            this.mnuExtendParallelBranchBelow.Enabled = false;
            this.mnuExtendParallelBranchBelow.Name = "mnuExtendParallelBranchBelow";
            this.mnuExtendParallelBranchBelow.Size = new System.Drawing.Size(204, 22);
            this.mnuExtendParallelBranchBelow.Text = "Extend Parallel Branch Below";
            this.mnuExtendParallelBranchBelow.Visible = true;
            // 
            // menuToggleBit
            // 
            this.mnuToggleBit.Name = "mnuToggleBit";
            this.mnuToggleBit.Size = new System.Drawing.Size(204, 22);
            this.mnuToggleBit.Text = "Toggle Bit";
            this.mnuToggleBit.Click += new System.EventHandler(this.mnuToggleBit_Click);
            // 
            // menuToggleBitPulse
            // 
            this.mnuToggleBitPulse.Name = "menuToggleBitPulse";
            this.mnuToggleBitPulse.Size = new System.Drawing.Size(204, 22);
            this.mnuToggleBitPulse.Text = "Toggle Bit Pulse (1 scan)";
            this.mnuToggleBitPulse.Click += new System.EventHandler(this.mnuToggleBitPulse_Click);
            // 
            // toolTipQuadrosSaida
            // 
            this.toolTipOutputBox.AutoPopDelay = 2000;
            this.toolTipOutputBox.InitialDelay = 500;
            this.toolTipOutputBox.ReshowDelay = 100;
            this.toolTipOutputBox.ShowAlways = true;
            // 
            // DiagramaLadder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(594, 194);
            this.Controls.Add(this.statusBar);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(190, 210);
            this.Name = "DiagramaLadder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ladder";
            //this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.LadderForm_Scroll);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LadderForm_MouseClick);
            this.Resize += new System.EventHandler(this.DiagramaLadder_Resize);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.mnuContextAtInstruction.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel lblstatusMessage;
        public System.Windows.Forms.ContextMenuStrip mnuContextAtInstruction;
        public System.Windows.Forms.ToolStripMenuItem mnuAddressing;
        public System.Windows.Forms.ToolStripMenuItem mnuMemory;
        public System.Windows.Forms.ToolStripMenuItem mnuTimer;
        public System.Windows.Forms.ToolStripMenuItem mnuCounter;
        public System.Windows.Forms.ToolStripMenuItem mnuInput;
        public System.Windows.Forms.ToolStripMenuItem mnuOutput;
        public System.Windows.Forms.ToolStripMenuItem mnuInsertLadderLine;
        public System.Windows.Forms.ToolStripMenuItem mnuInsertLadderLineAbove;
        public System.Windows.Forms.ToolStripMenuItem mnuInsertLadderLineBelow;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuClearAddress;
        public System.Windows.Forms.ToolStripMenuItem mnuExtendParallelBranchAbove;
        public System.Windows.Forms.ToolStripMenuItem mnuExtendParallelBranchBelow;
        public System.Windows.Forms.ToolStripMenuItem mnuToggleBit;
        private System.Windows.Forms.ToolStripMenuItem mnuToggleBitPulse;
        private System.Windows.Forms.ToolTip toolTipOutputBox;
    }
}