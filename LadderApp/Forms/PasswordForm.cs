using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LadderApp.Formularios
{
    public partial class PasswordForm : Form
    {
        public PasswordForm()
        {
            InitializeComponent();
        }

        private void frmSenha_Load(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }
    }
}