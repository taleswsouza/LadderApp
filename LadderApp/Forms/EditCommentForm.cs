using LadderApp.Model;
using System;
using System.Windows.Forms;

namespace LadderApp.Formularios
{
    public partial class EditCommentForm : Form
    {
        public EditCommentForm(Address address) : this()
        {
            txtComment.Text = address.Comment.Trim();
            this.Text = $"Edit Comment {address.GetName()}";
        }

        public EditCommentForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}