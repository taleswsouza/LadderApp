namespace LadderApp
{
    partial class DeviceForm
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Pin/bit per port");
            this.lblManufacturer = new System.Windows.Forms.Label();
            this.lblSeries = new System.Windows.Forms.Label();
            this.lblModel = new System.Windows.Forms.Label();
            this.lblNumberOfPorts = new System.Windows.Forms.Label();
            this.lblPinsConfiguration = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tvnPinsTree = new System.Windows.Forms.TreeView();
            this.grpPinConfiguration = new System.Windows.Forms.GroupBox();
            this.rbUnavailable = new System.Windows.Forms.RadioButton();
            this.rbOutput = new System.Windows.Forms.RadioButton();
            this.rbInput = new System.Windows.Forms.RadioButton();
            this.rbNotUsed = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblNumberOfBitsPerPort = new System.Windows.Forms.Label();
            this.grpPinConfiguration.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblManufacturer
            // 
            this.lblManufacturer.AutoSize = true;
            this.lblManufacturer.Location = new System.Drawing.Point(11, 13);
            this.lblManufacturer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblManufacturer.Name = "lblManufacturer";
            this.lblManufacturer.Size = new System.Drawing.Size(73, 13);
            this.lblManufacturer.TabIndex = 1;
            this.lblManufacturer.Text = "Manufacturer:";
            // 
            // lblSeries
            // 
            this.lblSeries.AutoSize = true;
            this.lblSeries.Location = new System.Drawing.Point(11, 39);
            this.lblSeries.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSeries.Name = "lblSeries";
            this.lblSeries.Size = new System.Drawing.Size(39, 13);
            this.lblSeries.TabIndex = 3;
            this.lblSeries.Text = "Series:";
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.ForeColor = System.Drawing.Color.Red;
            this.lblModel.Location = new System.Drawing.Point(11, 65);
            this.lblModel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(39, 13);
            this.lblModel.TabIndex = 5;
            this.lblModel.Text = "Model:";
            // 
            // lblNumberOfPorts
            // 
            this.lblNumberOfPorts.AutoSize = true;
            this.lblNumberOfPorts.Location = new System.Drawing.Point(11, 91);
            this.lblNumberOfPorts.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNumberOfPorts.Name = "lblNumberOfPorts";
            this.lblNumberOfPorts.Size = new System.Drawing.Size(85, 13);
            this.lblNumberOfPorts.TabIndex = 7;
            this.lblNumberOfPorts.Text = "Number of ports:";
            // 
            // lblPinsConfiguration
            // 
            this.lblPinsConfiguration.AutoSize = true;
            this.lblPinsConfiguration.Location = new System.Drawing.Point(194, 8);
            this.lblPinsConfiguration.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPinsConfiguration.Name = "lblPinsConfiguration";
            this.lblPinsConfiguration.Size = new System.Drawing.Size(94, 13);
            this.lblPinsConfiguration.TabIndex = 8;
            this.lblPinsConfiguration.Text = "Pins configuration:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(277, 288);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(69, 32);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tvnPinsTree
            // 
            this.tvnPinsTree.Location = new System.Drawing.Point(194, 27);
            this.tvnPinsTree.Margin = new System.Windows.Forms.Padding(2);
            this.tvnPinsTree.Name = "tvnPinsTree";
            treeNode1.Name = "tvnPinsNode";
            treeNode1.Text = "Pin/bit per port";
            this.tvnPinsTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvnPinsTree.Size = new System.Drawing.Size(221, 243);
            this.tvnPinsTree.TabIndex = 12;
            this.tvnPinsTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvnPinsTree_AfterSelect);
            this.tvnPinsTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvnPinsTree_NodeMouseClick);
            // 
            // grpPinConfiguration
            // 
            this.grpPinConfiguration.Controls.Add(this.rbUnavailable);
            this.grpPinConfiguration.Controls.Add(this.rbOutput);
            this.grpPinConfiguration.Controls.Add(this.rbInput);
            this.grpPinConfiguration.Controls.Add(this.rbNotUsed);
            this.grpPinConfiguration.Location = new System.Drawing.Point(11, 186);
            this.grpPinConfiguration.Margin = new System.Windows.Forms.Padding(2);
            this.grpPinConfiguration.Name = "grpPinConfiguration";
            this.grpPinConfiguration.Padding = new System.Windows.Forms.Padding(2);
            this.grpPinConfiguration.Size = new System.Drawing.Size(178, 84);
            this.grpPinConfiguration.TabIndex = 13;
            this.grpPinConfiguration.TabStop = false;
            this.grpPinConfiguration.Text = "Bit Configuration x:";
            this.grpPinConfiguration.Visible = false;
            // 
            // rbUnavailable
            // 
            this.rbUnavailable.AutoSize = true;
            this.rbUnavailable.Location = new System.Drawing.Point(89, 18);
            this.rbUnavailable.Margin = new System.Windows.Forms.Padding(2);
            this.rbUnavailable.Name = "rbUnavailable";
            this.rbUnavailable.Size = new System.Drawing.Size(81, 17);
            this.rbUnavailable.TabIndex = 3;
            this.rbUnavailable.TabStop = true;
            this.rbUnavailable.Text = "Unavailable";
            this.rbUnavailable.UseVisualStyleBackColor = true;
            // 
            // rbOutput
            // 
            this.rbOutput.AutoSize = true;
            this.rbOutput.Location = new System.Drawing.Point(5, 61);
            this.rbOutput.Margin = new System.Windows.Forms.Padding(2);
            this.rbOutput.Name = "rbOutput";
            this.rbOutput.Size = new System.Drawing.Size(57, 17);
            this.rbOutput.TabIndex = 2;
            this.rbOutput.TabStop = true;
            this.rbOutput.Text = "Output";
            this.rbOutput.UseVisualStyleBackColor = true;
            // 
            // rbInput
            // 
            this.rbInput.AutoSize = true;
            this.rbInput.Location = new System.Drawing.Point(5, 39);
            this.rbInput.Margin = new System.Windows.Forms.Padding(2);
            this.rbInput.Name = "rbInput";
            this.rbInput.Size = new System.Drawing.Size(49, 17);
            this.rbInput.TabIndex = 1;
            this.rbInput.TabStop = true;
            this.rbInput.Text = "Input";
            this.rbInput.UseVisualStyleBackColor = true;
            // 
            // rbNotUsed
            // 
            this.rbNotUsed.AutoSize = true;
            this.rbNotUsed.Location = new System.Drawing.Point(5, 17);
            this.rbNotUsed.Margin = new System.Windows.Forms.Padding(2);
            this.rbNotUsed.Name = "rbNotUsed";
            this.rbNotUsed.Size = new System.Drawing.Size(70, 17);
            this.rbNotUsed.TabIndex = 0;
            this.rbNotUsed.TabStop = true;
            this.rbNotUsed.Text = "Not Used";
            this.rbNotUsed.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(182, 288);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(69, 32);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(82, 288);
            this.btnApply.Margin = new System.Windows.Forms.Padding(2);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(69, 32);
            this.btnApply.TabIndex = 15;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lblNumberOfBitsPerPort
            // 
            this.lblNumberOfBitsPerPort.AutoSize = true;
            this.lblNumberOfBitsPerPort.Location = new System.Drawing.Point(12, 117);
            this.lblNumberOfBitsPerPort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNumberOfBitsPerPort.Name = "lblNumberOfBitsPerPort";
            this.lblNumberOfBitsPerPort.Size = new System.Drawing.Size(117, 13);
            this.lblNumberOfBitsPerPort.TabIndex = 17;
            this.lblNumberOfBitsPerPort.Text = "Number of bits per port:";
            // 
            // DeviceForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(435, 331);
            this.Controls.Add(this.lblNumberOfBitsPerPort);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpPinConfiguration);
            this.Controls.Add(this.tvnPinsTree);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblPinsConfiguration);
            this.Controls.Add(this.lblNumberOfPorts);
            this.Controls.Add(this.lblModel);
            this.Controls.Add(this.lblSeries);
            this.Controls.Add(this.lblManufacturer);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DeviceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Device";
            this.grpPinConfiguration.ResumeLayout(false);
            this.grpPinConfiguration.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblManufacturer;
        private System.Windows.Forms.Label lblSeries;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.Label lblNumberOfPorts;
        private System.Windows.Forms.Label lblPinsConfiguration;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TreeView tvnPinsTree;
        private System.Windows.Forms.GroupBox grpPinConfiguration;
        private System.Windows.Forms.RadioButton rbUnavailable;
        private System.Windows.Forms.RadioButton rbOutput;
        private System.Windows.Forms.RadioButton rbInput;
        private System.Windows.Forms.RadioButton rbNotUsed;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblNumberOfBitsPerPort;
    }
}