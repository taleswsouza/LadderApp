using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LadderApp.Formularios
{
    public partial class frmSenha : Form
    {
        public frmSenha()
        {
            InitializeComponent();
        }

        private void frmSenha_Load(object sender, EventArgs e)
        {
            txtSenha.Focus();
        }
    }
}