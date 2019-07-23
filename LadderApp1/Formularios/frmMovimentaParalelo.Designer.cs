namespace LadderApp1.Formularios
{
    partial class frmMovimentaParalelo
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
            this.btnEsquerda = new System.Windows.Forms.Button();
            this.lblPosicao = new System.Windows.Forms.Label();
            this.btnDireita = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnEsquerda
            // 
            this.btnEsquerda.Location = new System.Drawing.Point(55, 38);
            this.btnEsquerda.Name = "btnEsquerda";
            this.btnEsquerda.Size = new System.Drawing.Size(68, 28);
            this.btnEsquerda.TabIndex = 0;
            this.btnEsquerda.Text = "<---";
            this.btnEsquerda.UseVisualStyleBackColor = true;
            // 
            // lblPosicao
            // 
            this.lblPosicao.Location = new System.Drawing.Point(140, 39);
            this.lblPosicao.Name = "lblPosicao";
            this.lblPosicao.Size = new System.Drawing.Size(45, 27);
            this.lblPosicao.TabIndex = 1;
            this.lblPosicao.Text = "PPP";
            this.lblPosicao.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDireita
            // 
            this.btnDireita.Location = new System.Drawing.Point(202, 38);
            this.btnDireita.Name = "btnDireita";
            this.btnDireita.Size = new System.Drawing.Size(68, 28);
            this.btnDireita.TabIndex = 2;
            this.btnDireita.Text = "--->";
            this.btnDireita.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(55, 100);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(67, 29);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(203, 100);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(67, 29);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // frmMovimentaParalelo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 150);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnDireita);
            this.Controls.Add(this.lblPosicao);
            this.Controls.Add(this.btnEsquerda);
            this.Name = "frmMovimentaParalelo";
            this.Text = "Movimenta Paralelo";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEsquerda;
        private System.Windows.Forms.Label lblPosicao;
        private System.Windows.Forms.Button btnDireita;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancelar;
    }
}