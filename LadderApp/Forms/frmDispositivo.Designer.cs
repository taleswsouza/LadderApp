namespace LadderApp
{
    partial class frmDispositivo
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
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Bits per port");
            this.lblFabricante = new System.Windows.Forms.Label();
            this.lblSerie = new System.Windows.Forms.Label();
            this.lblModelo = new System.Windows.Forms.Label();
            this.lblQtdPortas = new System.Windows.Forms.Label();
            this.lblBits = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.ArvorePinos = new System.Windows.Forms.TreeView();
            this.grpConfiguraPino = new System.Windows.Forms.GroupBox();
            this.rbOutro = new System.Windows.Forms.RadioButton();
            this.rbSaida = new System.Windows.Forms.RadioButton();
            this.rbEntrada = new System.Windows.Forms.RadioButton();
            this.rbEntradaOuSaida = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnAplicar = new System.Windows.Forms.Button();
            this.lblQtdBitsPorta = new System.Windows.Forms.Label();
            this.grpConfiguraPino.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFabricante
            // 
            this.lblFabricante.AutoSize = true;
            this.lblFabricante.Location = new System.Drawing.Point(11, 13);
            this.lblFabricante.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFabricante.Name = "lblFabricante";
            this.lblFabricante.Size = new System.Drawing.Size(73, 13);
            this.lblFabricante.TabIndex = 1;
            this.lblFabricante.Text = "Manufacturer:";
            // 
            // lblSerie
            // 
            this.lblSerie.AutoSize = true;
            this.lblSerie.Location = new System.Drawing.Point(11, 39);
            this.lblSerie.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSerie.Name = "lblSerie";
            this.lblSerie.Size = new System.Drawing.Size(39, 13);
            this.lblSerie.TabIndex = 3;
            this.lblSerie.Text = "Series:";
            // 
            // lblModelo
            // 
            this.lblModelo.AutoSize = true;
            this.lblModelo.ForeColor = System.Drawing.Color.Red;
            this.lblModelo.Location = new System.Drawing.Point(11, 65);
            this.lblModelo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblModelo.Name = "lblModelo";
            this.lblModelo.Size = new System.Drawing.Size(39, 13);
            this.lblModelo.TabIndex = 5;
            this.lblModelo.Text = "Model:";
            // 
            // lblQtdPortas
            // 
            this.lblQtdPortas.AutoSize = true;
            this.lblQtdPortas.Location = new System.Drawing.Point(11, 91);
            this.lblQtdPortas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblQtdPortas.Name = "lblQtdPortas";
            this.lblQtdPortas.Size = new System.Drawing.Size(85, 13);
            this.lblQtdPortas.TabIndex = 7;
            this.lblQtdPortas.Text = "Number of ports:";
            // 
            // lblBits
            // 
            this.lblBits.AutoSize = true;
            this.lblBits.Location = new System.Drawing.Point(194, 8);
            this.lblBits.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBits.Name = "lblBits";
            this.lblBits.Size = new System.Drawing.Size(94, 13);
            this.lblBits.TabIndex = 8;
            this.lblBits.Text = "Pins configuration:";
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(277, 288);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(69, 32);
            this.btnCancelar.TabIndex = 11;
            this.btnCancelar.Text = "Cancel";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // ArvorePinos
            // 
            this.ArvorePinos.Location = new System.Drawing.Point(194, 27);
            this.ArvorePinos.Margin = new System.Windows.Forms.Padding(2);
            this.ArvorePinos.Name = "ArvorePinos";
            treeNode7.Name = "NoPinos";
            treeNode7.Text = "Bits per port";
            this.ArvorePinos.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode7});
            this.ArvorePinos.Size = new System.Drawing.Size(221, 243);
            this.ArvorePinos.TabIndex = 12;
            this.ArvorePinos.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ArvorePortas_AfterSelect);
            this.ArvorePinos.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ArvorePortas_NodeMouseClick);
            // 
            // grpConfiguraPino
            // 
            this.grpConfiguraPino.Controls.Add(this.rbOutro);
            this.grpConfiguraPino.Controls.Add(this.rbSaida);
            this.grpConfiguraPino.Controls.Add(this.rbEntrada);
            this.grpConfiguraPino.Controls.Add(this.rbEntradaOuSaida);
            this.grpConfiguraPino.Location = new System.Drawing.Point(11, 186);
            this.grpConfiguraPino.Margin = new System.Windows.Forms.Padding(2);
            this.grpConfiguraPino.Name = "grpConfiguraPino";
            this.grpConfiguraPino.Padding = new System.Windows.Forms.Padding(2);
            this.grpConfiguraPino.Size = new System.Drawing.Size(178, 84);
            this.grpConfiguraPino.TabIndex = 13;
            this.grpConfiguraPino.TabStop = false;
            this.grpConfiguraPino.Text = "Bit Configuration x:";
            this.grpConfiguraPino.Visible = false;
            // 
            // rbOutro
            // 
            this.rbOutro.AutoSize = true;
            this.rbOutro.Location = new System.Drawing.Point(89, 18);
            this.rbOutro.Margin = new System.Windows.Forms.Padding(2);
            this.rbOutro.Name = "rbOutro";
            this.rbOutro.Size = new System.Drawing.Size(81, 17);
            this.rbOutro.TabIndex = 3;
            this.rbOutro.TabStop = true;
            this.rbOutro.Text = "Unavailable";
            this.rbOutro.UseVisualStyleBackColor = true;
            // 
            // rbSaida
            // 
            this.rbSaida.AutoSize = true;
            this.rbSaida.Location = new System.Drawing.Point(5, 61);
            this.rbSaida.Margin = new System.Windows.Forms.Padding(2);
            this.rbSaida.Name = "rbSaida";
            this.rbSaida.Size = new System.Drawing.Size(57, 17);
            this.rbSaida.TabIndex = 2;
            this.rbSaida.TabStop = true;
            this.rbSaida.Text = "Output";
            this.rbSaida.UseVisualStyleBackColor = true;
            // 
            // rbEntrada
            // 
            this.rbEntrada.AutoSize = true;
            this.rbEntrada.Location = new System.Drawing.Point(5, 39);
            this.rbEntrada.Margin = new System.Windows.Forms.Padding(2);
            this.rbEntrada.Name = "rbEntrada";
            this.rbEntrada.Size = new System.Drawing.Size(49, 17);
            this.rbEntrada.TabIndex = 1;
            this.rbEntrada.TabStop = true;
            this.rbEntrada.Text = "Input";
            this.rbEntrada.UseVisualStyleBackColor = true;
            // 
            // rbEntradaOuSaida
            // 
            this.rbEntradaOuSaida.AutoSize = true;
            this.rbEntradaOuSaida.Location = new System.Drawing.Point(5, 17);
            this.rbEntradaOuSaida.Margin = new System.Windows.Forms.Padding(2);
            this.rbEntradaOuSaida.Name = "rbEntradaOuSaida";
            this.rbEntradaOuSaida.Size = new System.Drawing.Size(70, 17);
            this.rbEntradaOuSaida.TabIndex = 0;
            this.rbEntradaOuSaida.TabStop = true;
            this.rbEntradaOuSaida.Text = "Not Used";
            this.rbEntradaOuSaida.UseVisualStyleBackColor = true;
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
            // btnAplicar
            // 
            this.btnAplicar.Location = new System.Drawing.Point(82, 288);
            this.btnAplicar.Margin = new System.Windows.Forms.Padding(2);
            this.btnAplicar.Name = "btnAplicar";
            this.btnAplicar.Size = new System.Drawing.Size(69, 32);
            this.btnAplicar.TabIndex = 15;
            this.btnAplicar.Text = "Apply";
            this.btnAplicar.UseVisualStyleBackColor = true;
            this.btnAplicar.Click += new System.EventHandler(this.btnAplicar_Click);
            // 
            // lblQtdBitsPorta
            // 
            this.lblQtdBitsPorta.AutoSize = true;
            this.lblQtdBitsPorta.Location = new System.Drawing.Point(12, 117);
            this.lblQtdBitsPorta.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblQtdBitsPorta.Name = "lblQtdBitsPorta";
            this.lblQtdBitsPorta.Size = new System.Drawing.Size(117, 13);
            this.lblQtdBitsPorta.TabIndex = 17;
            this.lblQtdBitsPorta.Text = "Number of bits per port:";
            // 
            // frmDispositivo
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(435, 331);
            this.Controls.Add(this.lblQtdBitsPorta);
            this.Controls.Add(this.btnAplicar);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpConfiguraPino);
            this.Controls.Add(this.ArvorePinos);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.lblBits);
            this.Controls.Add(this.lblQtdPortas);
            this.Controls.Add(this.lblModelo);
            this.Controls.Add(this.lblSerie);
            this.Controls.Add(this.lblFabricante);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmDispositivo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Device";
            this.grpConfiguraPino.ResumeLayout(false);
            this.grpConfiguraPino.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFabricante;
        private System.Windows.Forms.Label lblSerie;
        private System.Windows.Forms.Label lblModelo;
        private System.Windows.Forms.Label lblQtdPortas;
        private System.Windows.Forms.Label lblBits;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.TreeView ArvorePinos;
        private System.Windows.Forms.GroupBox grpConfiguraPino;
        private System.Windows.Forms.RadioButton rbOutro;
        private System.Windows.Forms.RadioButton rbSaida;
        private System.Windows.Forms.RadioButton rbEntrada;
        private System.Windows.Forms.RadioButton rbEntradaOuSaida;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnAplicar;
        private System.Windows.Forms.Label lblQtdBitsPorta;
    }
}