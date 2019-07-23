namespace LadderApp
{
    partial class ProjetoLadder
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Dispositivo");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Memoria");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Configuracoes", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Programa Ladder");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Entradas");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Saidas");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Memoria");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Temporizadores");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Contadores");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Enderecamento", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Projeto", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4,
            treeNode10});
            this.ArvoreProjeto = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // ArvoreProjeto
            // 
            this.ArvoreProjeto.Location = new System.Drawing.Point(9, 10);
            this.ArvoreProjeto.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ArvoreProjeto.Name = "ArvoreProjeto";
            treeNode1.Name = "NoDispositivo";
            treeNode1.Text = "Dispositivo";
            treeNode2.Name = "NoConfMemoria";
            treeNode2.Text = "Memoria";
            treeNode3.Name = "NoConfiguracoes";
            treeNode3.Text = "Configuracoes";
            treeNode4.Name = "NoProgramaLadder";
            treeNode4.Text = "Programa Ladder";
            treeNode5.Name = "NoEntradas";
            treeNode5.Text = "Entradas";
            treeNode6.Name = "NoSaidas";
            treeNode6.Text = "Saidas";
            treeNode7.Name = "NoMemoria";
            treeNode7.Text = "Memoria";
            treeNode8.Name = "NoTemporizadores";
            treeNode8.Text = "Temporizadores";
            treeNode9.Name = "NoContadores";
            treeNode9.Text = "Contadores";
            treeNode10.Name = "NoEnderecamento";
            treeNode10.Text = "Enderecamento";
            treeNode11.Name = "NoProjeto";
            treeNode11.Text = "Projeto";
            this.ArvoreProjeto.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode11});
            this.ArvoreProjeto.Size = new System.Drawing.Size(202, 192);
            this.ArvoreProjeto.TabIndex = 0;
            this.ArvoreProjeto.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ArvoreProjeto_NodeMouseDoubleClick);
            // 
            // ProjetoLadder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 211);
            this.Controls.Add(this.ArvoreProjeto);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ProjetoLadder";
            this.Text = " ProjetoLadder";
            this.SizeChanged += new System.EventHandler(this.ProjetoLadder_Resize);
            this.Resize += new System.EventHandler(this.ProjetoLadder_Resize);
            this.Shown += new System.EventHandler(this.ProjetoLadder_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProjetoLadder_FormClosing);
            this.Load += new System.EventHandler(this.ProjetoLadder_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView ArvoreProjeto;

    }
}