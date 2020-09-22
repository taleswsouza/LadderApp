namespace LadderApp
{
    partial class DiagramaLadder
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
            this.lblstatusMensagem = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuControle = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuEnderecamento = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMemoria = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTemporizador = new System.Windows.Forms.ToolStripMenuItem();
            this.menuContador = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEntrada = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaida = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuLimparEndereco = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInsereLinha = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInsereLinhaAcima = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInsereLinhaAbaixo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEstenderParaleloAcima = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEstenderParaleloAbaixo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToggleBit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToggleBitPulse = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTipQuadrosSaida = new System.Windows.Forms.ToolTip(this.components);
            this.statusBar.SuspendLayout();
            this.menuControle.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblstatusMensagem});
            this.statusBar.Location = new System.Drawing.Point(0, 172);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusBar.Size = new System.Drawing.Size(594, 22);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "Pronto";
            // 
            // lblstatusMensagem
            // 
            this.lblstatusMensagem.Name = "lblstatusMensagem";
            this.lblstatusMensagem.Size = new System.Drawing.Size(39, 17);
            this.lblstatusMensagem.Text = "Ready";
            // 
            // menuControle
            // 
            this.menuControle.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEnderecamento,
            this.menuInsereLinha,
            this.menuEstenderParaleloAcima,
            this.menuEstenderParaleloAbaixo,
            this.menuToggleBit,
            this.menuToggleBitPulse});
            this.menuControle.Name = "menuControle";
            this.menuControle.Size = new System.Drawing.Size(205, 158);
            this.menuControle.Text = "Menu";
            // 
            // menuEnderecamento
            // 
            this.menuEnderecamento.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMemoria,
            this.menuTemporizador,
            this.menuContador,
            this.menuEntrada,
            this.menuSaida,
            this.toolStripSeparator1,
            this.menuLimparEndereco});
            this.menuEnderecamento.Name = "menuEnderecamento";
            this.menuEnderecamento.Size = new System.Drawing.Size(204, 22);
            this.menuEnderecamento.Text = "Addressing";
            // 
            // menuMemoria
            // 
            this.menuMemoria.Name = "menuMemoria";
            this.menuMemoria.Size = new System.Drawing.Size(180, 22);
            this.menuMemoria.Text = "Memory";
            // 
            // menuTemporizador
            // 
            this.menuTemporizador.Name = "menuTemporizador";
            this.menuTemporizador.Size = new System.Drawing.Size(180, 22);
            this.menuTemporizador.Text = "Timer";
            // 
            // menuContador
            // 
            this.menuContador.Name = "menuContador";
            this.menuContador.Size = new System.Drawing.Size(180, 22);
            this.menuContador.Text = "Counter";
            // 
            // menuEntrada
            // 
            this.menuEntrada.Name = "menuEntrada";
            this.menuEntrada.Size = new System.Drawing.Size(180, 22);
            this.menuEntrada.Text = "Input";
            // 
            // menuSaida
            // 
            this.menuSaida.Name = "menuSaida";
            this.menuSaida.Size = new System.Drawing.Size(180, 22);
            this.menuSaida.Text = "Output";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // menuLimparEndereco
            // 
            this.menuLimparEndereco.Name = "menuLimparEndereco";
            this.menuLimparEndereco.Size = new System.Drawing.Size(180, 22);
            this.menuLimparEndereco.Text = "Clean";
            this.menuLimparEndereco.ToolTipText = "Limpa o endereco do simbolo selecionado.";
            this.menuLimparEndereco.Click += new System.EventHandler(this.menuLimparEndereco_Click);
            // 
            // menuInsereLinha
            // 
            this.menuInsereLinha.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuInsereLinhaAcima,
            this.menuInsereLinhaAbaixo});
            this.menuInsereLinha.Name = "menuInsereLinha";
            this.menuInsereLinha.Size = new System.Drawing.Size(204, 22);
            this.menuInsereLinha.Text = "Insert Line";
            // 
            // menuInsereLinhaAcima
            // 
            this.menuInsereLinhaAcima.Name = "menuInsereLinhaAcima";
            this.menuInsereLinhaAcima.Size = new System.Drawing.Size(180, 22);
            this.menuInsereLinhaAcima.Text = "Above";
            this.menuInsereLinhaAcima.Click += new System.EventHandler(this.menuInsereLinhaAcima_Click);
            // 
            // menuInsereLinhaAbaixo
            // 
            this.menuInsereLinhaAbaixo.Name = "menuInsereLinhaAbaixo";
            this.menuInsereLinhaAbaixo.Size = new System.Drawing.Size(180, 22);
            this.menuInsereLinhaAbaixo.Text = "Below";
            this.menuInsereLinhaAbaixo.Click += new System.EventHandler(this.menuInsereLinhaAbaixo_Click);
            // 
            // menuEstenderParaleloAcima
            // 
            this.menuEstenderParaleloAcima.Enabled = false;
            this.menuEstenderParaleloAcima.Name = "menuEstenderParaleloAcima";
            this.menuEstenderParaleloAcima.Size = new System.Drawing.Size(204, 22);
            this.menuEstenderParaleloAcima.Text = "Estender Paralelo Acima";
            this.menuEstenderParaleloAcima.Visible = false;
            this.menuEstenderParaleloAcima.Click += new System.EventHandler(this.menuEstenderParaleloAcima_Click);
            // 
            // menuEstenderParaleloAbaixo
            // 
            this.menuEstenderParaleloAbaixo.Enabled = false;
            this.menuEstenderParaleloAbaixo.Name = "menuEstenderParaleloAbaixo";
            this.menuEstenderParaleloAbaixo.Size = new System.Drawing.Size(204, 22);
            this.menuEstenderParaleloAbaixo.Text = "Estender Paralelo Abaixo";
            this.menuEstenderParaleloAbaixo.Visible = false;
            // 
            // menuToggleBit
            // 
            this.menuToggleBit.Name = "menuToggleBit";
            this.menuToggleBit.Size = new System.Drawing.Size(204, 22);
            this.menuToggleBit.Text = "Toggle Bit";
            this.menuToggleBit.Click += new System.EventHandler(this.menuToggleBit_Click);
            // 
            // menuToggleBitPulse
            // 
            this.menuToggleBitPulse.Name = "menuToggleBitPulse";
            this.menuToggleBitPulse.Size = new System.Drawing.Size(204, 22);
            this.menuToggleBitPulse.Text = "Toggle Bit Pulse (1 scan)";
            this.menuToggleBitPulse.Click += new System.EventHandler(this.menuToggleBitPulse_Click);
            // 
            // toolTipQuadrosSaida
            // 
            this.toolTipQuadrosSaida.AutoPopDelay = 2000;
            this.toolTipQuadrosSaida.InitialDelay = 500;
            this.toolTipQuadrosSaida.ReshowDelay = 100;
            this.toolTipQuadrosSaida.ShowAlways = true;
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
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.DiagramaLadder_Scroll);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DiagramaLadder_MouseClick);
            this.Resize += new System.EventHandler(this.DiagramaLadder_Resize);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.menuControle.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel lblstatusMensagem;
        public System.Windows.Forms.ContextMenuStrip menuControle;
        public System.Windows.Forms.ToolStripMenuItem menuEnderecamento;
        public System.Windows.Forms.ToolStripMenuItem menuMemoria;
        public System.Windows.Forms.ToolStripMenuItem menuTemporizador;
        public System.Windows.Forms.ToolStripMenuItem menuContador;
        public System.Windows.Forms.ToolStripMenuItem menuEntrada;
        public System.Windows.Forms.ToolStripMenuItem menuSaida;
        public System.Windows.Forms.ToolStripMenuItem menuInsereLinha;
        public System.Windows.Forms.ToolStripMenuItem menuInsereLinhaAcima;
        public System.Windows.Forms.ToolStripMenuItem menuInsereLinhaAbaixo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuLimparEndereco;
        public System.Windows.Forms.ToolStripMenuItem menuEstenderParaleloAcima;
        public System.Windows.Forms.ToolStripMenuItem menuEstenderParaleloAbaixo;
        public System.Windows.Forms.ToolStripMenuItem menuToggleBit;
        private System.Windows.Forms.ToolStripMenuItem menuToggleBitPulse;
        private System.Windows.Forms.ToolTip toolTipQuadrosSaida;
    }
}