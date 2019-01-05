using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Player
{
    public partial class CustomMessageBox : Form
    {
        public CustomMessageBox()
        {
            InitializeComponent();
        }
        static CustomMessageBox MsgBox;
        static DialogResult result = DialogResult.OK;

        public static DialogResult Show(string Error, string Text)
        {
            MsgBox = new CustomMessageBox();
            MsgBox.msbContent.Text = Error;
            MsgBox.msbContent.Text += Text;
            MsgBox.ShowDialog();
            return result;
        }

        private void msbButton_Click(object sender, EventArgs e)
        {
            result = DialogResult.OK;
            MsgBox.Close();
        }
    }
}
