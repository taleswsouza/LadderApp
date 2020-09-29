namespace LadderApp
{
    partial class VisualInstructionUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // VisualInstructionUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "VisualInstructionUserControl";
            this.Size = new System.Drawing.Size(98, 91);
            this.Load += new System.EventHandler(this.ControleLivre_Load);
            this.SizeChanged += new System.EventHandler(this.ControleLivre_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ControleLivre_Paint);
            this.Enter += new System.EventHandler(this.ControleLivre_Enter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ControleLivre_KeyDown);
            this.Leave += new System.EventHandler(this.ControleLivre_Leave);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ControleLivre_MouseDoubleClick);
            this.Resize += new System.EventHandler(this.ControleLivre_Resize);
            this.ResumeLayout(false);

        }

        #endregion






    }
}
