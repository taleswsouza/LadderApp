namespace LadderApp
{
    partial class AddressingForm
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
            this.lblMemory = new System.Windows.Forms.Label();
            this.txtMemory = new System.Windows.Forms.NumericUpDown();
            this.txtInput = new System.Windows.Forms.NumericUpDown();
            this.lblInput = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.NumericUpDown();
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtTimer = new System.Windows.Forms.NumericUpDown();
            this.lblTimer = new System.Windows.Forms.Label();
            this.txtCounter = new System.Windows.Forms.NumericUpDown();
            this.lblCounter = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.txtMemory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCounter)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMemory
            // 
            this.lblMemory.AutoSize = true;
            this.lblMemory.Location = new System.Drawing.Point(55, 24);
            this.lblMemory.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(47, 13);
            this.lblMemory.TabIndex = 0;
            this.lblMemory.Text = "Memory:";
            // 
            // txtMemory
            // 
            this.txtMemory.Location = new System.Drawing.Point(106, 22);
            this.txtMemory.Margin = new System.Windows.Forms.Padding(2);
            this.txtMemory.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtMemory.Name = "txtMemory";
            this.txtMemory.Size = new System.Drawing.Size(50, 20);
            this.txtMemory.TabIndex = 1;
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(106, 45);
            this.txtInput.Margin = new System.Windows.Forms.Padding(2);
            this.txtInput.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(50, 20);
            this.txtInput.TabIndex = 3;
            // 
            // lblInput
            // 
            this.lblInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(68, 45);
            this.lblInput.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(34, 13);
            this.lblInput.TabIndex = 2;
            this.lblInput.Text = "Input:";
            this.lblInput.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(106, 67);
            this.txtOutput.Margin = new System.Windows.Forms.Padding(2);
            this.txtOutput.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(50, 20);
            this.txtOutput.TabIndex = 5;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(60, 69);
            this.lblOutput.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(42, 13);
            this.lblOutput.TabIndex = 4;
            this.lblOutput.Text = "Output:";
            // 
            // txtTimer
            // 
            this.txtTimer.Location = new System.Drawing.Point(106, 90);
            this.txtTimer.Margin = new System.Windows.Forms.Padding(2);
            this.txtTimer.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtTimer.Name = "txtTimer";
            this.txtTimer.Size = new System.Drawing.Size(50, 20);
            this.txtTimer.TabIndex = 7;
            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.Location = new System.Drawing.Point(66, 92);
            this.lblTimer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(36, 13);
            this.lblTimer.TabIndex = 6;
            this.lblTimer.Text = "Timer:";
            // 
            // txtCounter
            // 
            this.txtCounter.Location = new System.Drawing.Point(106, 113);
            this.txtCounter.Margin = new System.Windows.Forms.Padding(2);
            this.txtCounter.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtCounter.Name = "txtCounter";
            this.txtCounter.Size = new System.Drawing.Size(50, 20);
            this.txtCounter.TabIndex = 9;
            // 
            // lblCounter
            // 
            this.lblCounter.AutoSize = true;
            this.lblCounter.Location = new System.Drawing.Point(55, 113);
            this.lblCounter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCounter.Name = "lblCounter";
            this.lblCounter.Size = new System.Drawing.Size(47, 13);
            this.lblCounter.TabIndex = 8;
            this.lblCounter.Text = "Counter:";
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(24, 166);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(77, 33);
            this.btnOk.TabIndex = 10;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.CloseWindow);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(117, 166);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 33);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.Cancel);
            // 
            // AddressingForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(219, 211);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtCounter);
            this.Controls.Add(this.lblCounter);
            this.Controls.Add(this.txtTimer);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.txtMemory);
            this.Controls.Add(this.lblMemory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "AddressingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Addressing";
            ((System.ComponentModel.ISupportInitialize)(this.txtMemory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCounter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.NumericUpDown txtMemory;
        private System.Windows.Forms.NumericUpDown txtInput;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.NumericUpDown txtOutput;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.NumericUpDown txtTimer;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.NumericUpDown txtCounter;
        private System.Windows.Forms.Label lblCounter;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}