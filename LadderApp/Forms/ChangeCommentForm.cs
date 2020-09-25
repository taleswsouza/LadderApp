using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LadderApp.Formularios
{
    public partial class ChangeCommentForm : Form
    {
        public ChangeCommentForm(Address address) : this()
        {
            txtComment.Text = address.Comment.Trim();
            this.Text = $"Comment {address.Name}";
        }

        public ChangeCommentForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}