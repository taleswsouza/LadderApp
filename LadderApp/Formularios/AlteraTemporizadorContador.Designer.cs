namespace LadderApp.Formularios
{
    partial class AlteraTemporizadorContador
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
            this.txtTitulo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAcumulado = new System.Windows.Forms.NumericUpDown();
            this.txtPreset = new System.Windows.Forms.NumericUpDown();
            this.lblBaseTempo = new System.Windows.Forms.Label();
            this.cmbBaseTempo = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbTipo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtAcumulado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPreset)).BeginInit();
            this.SuspendLayout();
            // 
            // txtTitulo
            // 
            this.txtTitulo.AutoSize = true;
            this.txtTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTitulo.Location = new System.Drawing.Point(92, 5);
            this.txtTitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.txtTitulo.Name = "txtTitulo";
            this.txtTitulo.Size = new System.Drawing.Size(71, 20);
            this.txtTitulo.TabIndex = 0;
            this.txtTitulo.Text = "lblTitulo";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 75);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Preset (PR):";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 105);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Accumulated (AC):";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAcumulado
            // 
            this.txtAcumulado.Location = new System.Drawing.Point(121, 102);
            this.txtAcumulado.Margin = new System.Windows.Forms.Padding(2);
            this.txtAcumulado.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtAcumulado.Name = "txtAcumulado";
            this.txtAcumulado.Size = new System.Drawing.Size(77, 20);
            this.txtAcumulado.TabIndex = 4;
            // 
            // txtPreset
            // 
            this.txtPreset.Location = new System.Drawing.Point(120, 72);
            this.txtPreset.Margin = new System.Windows.Forms.Padding(2);
            this.txtPreset.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtPreset.Name = "txtPreset";
            this.txtPreset.Size = new System.Drawing.Size(78, 20);
            this.txtPreset.TabIndex = 5;
            // 
            // lblBaseTempo
            // 
            this.lblBaseTempo.AutoSize = true;
            this.lblBaseTempo.Location = new System.Drawing.Point(34, 40);
            this.lblBaseTempo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBaseTempo.Name = "lblBaseTempo";
            this.lblBaseTempo.Size = new System.Drawing.Size(82, 13);
            this.lblBaseTempo.TabIndex = 6;
            this.lblBaseTempo.Text = "Time base (BT):";
            this.lblBaseTempo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbBaseTempo
            // 
            this.cmbBaseTempo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBaseTempo.FormattingEnabled = true;
            this.cmbBaseTempo.Items.AddRange(new object[] {
            "10 ms",
            "100 ms",
            "1 s",
            "1 m"});
            this.cmbBaseTempo.Location = new System.Drawing.Point(119, 37);
            this.cmbBaseTempo.Margin = new System.Windows.Forms.Padding(2);
            this.cmbBaseTempo.Name = "cmbBaseTempo";
            this.cmbBaseTempo.Size = new System.Drawing.Size(79, 21);
            this.cmbBaseTempo.TabIndex = 7;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(34, 134);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(143, 134);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbTipo
            // 
            this.cmbTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipo.FormattingEnabled = true;
            this.cmbTipo.Items.AddRange(new object[] {
            "ON",
            "OF"});
            this.cmbTipo.Location = new System.Drawing.Point(80, 5);
            this.cmbTipo.Margin = new System.Windows.Forms.Padding(2);
            this.cmbTipo.Name = "cmbTipo";
            this.cmbTipo.Size = new System.Drawing.Size(92, 21);
            this.cmbTipo.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(198, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "(0...255)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(198, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "(0...255)";
            // 
            // AlteraTemporizadorContador
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(254, 211);
            this.Controls.Add(this.cmbTipo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cmbBaseTempo);
            this.Controls.Add(this.lblBaseTempo);
            this.Controls.Add(this.txtPreset);
            this.Controls.Add(this.txtAcumulado);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTitulo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "AlteraTemporizadorContador";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change <%>";
            this.Load += new System.EventHandler(this.AlteraTemporizadorContador_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtAcumulado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPreset)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label txtTitulo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txtAcumulado;
        private System.Windows.Forms.NumericUpDown txtPreset;
        private System.Windows.Forms.Label lblBaseTempo;
        private System.Windows.Forms.ComboBox cmbBaseTempo;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbTipo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}