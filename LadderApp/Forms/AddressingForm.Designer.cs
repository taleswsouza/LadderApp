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
            this.lblMemoria = new System.Windows.Forms.Label();
            this.txtMemoria = new System.Windows.Forms.NumericUpDown();
            this.txtEntrada = new System.Windows.Forms.NumericUpDown();
            this.lblEntrada = new System.Windows.Forms.Label();
            this.txtSaida = new System.Windows.Forms.NumericUpDown();
            this.lblSaida = new System.Windows.Forms.Label();
            this.txtTemporizador = new System.Windows.Forms.NumericUpDown();
            this.lblTemporizador = new System.Windows.Forms.Label();
            this.txtContador = new System.Windows.Forms.NumericUpDown();
            this.lblContador = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.txtMemoria)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEntrada)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSaida)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTemporizador)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContador)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMemoria
            // 
            this.lblMemoria.AutoSize = true;
            this.lblMemoria.Location = new System.Drawing.Point(55, 24);
            this.lblMemoria.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMemoria.Name = "lblMemoria";
            this.lblMemoria.Size = new System.Drawing.Size(47, 13);
            this.lblMemoria.TabIndex = 0;
            this.lblMemoria.Text = "Memory:";
            // 
            // txtMemoria
            // 
            this.txtMemoria.Location = new System.Drawing.Point(106, 22);
            this.txtMemoria.Margin = new System.Windows.Forms.Padding(2);
            this.txtMemoria.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtMemoria.Name = "txtMemoria";
            this.txtMemoria.Size = new System.Drawing.Size(50, 20);
            this.txtMemoria.TabIndex = 1;
            // 
            // txtEntrada
            // 
            this.txtEntrada.Location = new System.Drawing.Point(106, 45);
            this.txtEntrada.Margin = new System.Windows.Forms.Padding(2);
            this.txtEntrada.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtEntrada.Name = "txtEntrada";
            this.txtEntrada.Size = new System.Drawing.Size(50, 20);
            this.txtEntrada.TabIndex = 3;
            // 
            // lblEntrada
            // 
            this.lblEntrada.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEntrada.AutoSize = true;
            this.lblEntrada.Location = new System.Drawing.Point(68, 45);
            this.lblEntrada.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEntrada.Name = "lblEntrada";
            this.lblEntrada.Size = new System.Drawing.Size(34, 13);
            this.lblEntrada.TabIndex = 2;
            this.lblEntrada.Text = "Input:";
            this.lblEntrada.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtSaida
            // 
            this.txtSaida.Location = new System.Drawing.Point(106, 67);
            this.txtSaida.Margin = new System.Windows.Forms.Padding(2);
            this.txtSaida.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtSaida.Name = "txtSaida";
            this.txtSaida.Size = new System.Drawing.Size(50, 20);
            this.txtSaida.TabIndex = 5;
            // 
            // lblSaida
            // 
            this.lblSaida.AutoSize = true;
            this.lblSaida.Location = new System.Drawing.Point(60, 69);
            this.lblSaida.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSaida.Name = "lblSaida";
            this.lblSaida.Size = new System.Drawing.Size(42, 13);
            this.lblSaida.TabIndex = 4;
            this.lblSaida.Text = "Output:";
            // 
            // txtTemporizador
            // 
            this.txtTemporizador.Location = new System.Drawing.Point(106, 90);
            this.txtTemporizador.Margin = new System.Windows.Forms.Padding(2);
            this.txtTemporizador.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtTemporizador.Name = "txtTemporizador";
            this.txtTemporizador.Size = new System.Drawing.Size(50, 20);
            this.txtTemporizador.TabIndex = 7;
            // 
            // lblTemporizador
            // 
            this.lblTemporizador.AutoSize = true;
            this.lblTemporizador.Location = new System.Drawing.Point(66, 92);
            this.lblTemporizador.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTemporizador.Name = "lblTemporizador";
            this.lblTemporizador.Size = new System.Drawing.Size(36, 13);
            this.lblTemporizador.TabIndex = 6;
            this.lblTemporizador.Text = "Timer:";
            // 
            // txtContador
            // 
            this.txtContador.Location = new System.Drawing.Point(106, 113);
            this.txtContador.Margin = new System.Windows.Forms.Padding(2);
            this.txtContador.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.txtContador.Name = "txtContador";
            this.txtContador.Size = new System.Drawing.Size(50, 20);
            this.txtContador.TabIndex = 9;
            // 
            // lblContador
            // 
            this.lblContador.AutoSize = true;
            this.lblContador.Location = new System.Drawing.Point(55, 113);
            this.lblContador.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContador.Name = "lblContador";
            this.lblContador.Size = new System.Drawing.Size(47, 13);
            this.lblContador.TabIndex = 8;
            this.lblContador.Text = "Counter:";
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
            this.btnOk.Click += new System.EventHandler(this.FechaJanela);
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(117, 166);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(77, 33);
            this.btnCancelar.TabIndex = 10;
            this.btnCancelar.Text = "Cancel";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.FechaJanela);
            // 
            // Memoria
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(219, 211);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtContador);
            this.Controls.Add(this.lblContador);
            this.Controls.Add(this.txtTemporizador);
            this.Controls.Add(this.lblTemporizador);
            this.Controls.Add(this.txtSaida);
            this.Controls.Add(this.lblSaida);
            this.Controls.Add(this.txtEntrada);
            this.Controls.Add(this.lblEntrada);
            this.Controls.Add(this.txtMemoria);
            this.Controls.Add(this.lblMemoria);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Memoria";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Addressing";
            ((System.ComponentModel.ISupportInitialize)(this.txtMemoria)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEntrada)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSaida)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTemporizador)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContador)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMemoria;
        private System.Windows.Forms.NumericUpDown txtMemoria;
        private System.Windows.Forms.NumericUpDown txtEntrada;
        private System.Windows.Forms.Label lblEntrada;
        private System.Windows.Forms.NumericUpDown txtSaida;
        private System.Windows.Forms.Label lblSaida;
        private System.Windows.Forms.NumericUpDown txtTemporizador;
        private System.Windows.Forms.Label lblTemporizador;
        private System.Windows.Forms.NumericUpDown txtContador;
        private System.Windows.Forms.Label lblContador;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancelar;
    }
}