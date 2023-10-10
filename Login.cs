using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterViewScheduler
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            byte[] encData_byte = new byte[password.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
            string encodedData = Convert.ToBase64String(encData_byte);

            List<User> users = JsonHelper.DeserializeFromFile<User>("users.json");
            User user = users.Find(u => u.Username == username && u.Password == encodedData);

            if (user != null)
            {
                Form1 frm = new Form1();
                frm.ShowDialog();
                Close();
            }
            else
            {
                MessageBox.Show("Login failed. Invalid credentials.");
            }
        }



        private void BtnRegister_Click(object sender, EventArgs e)
        {
            Registration frm = new Registration();
            frm.ShowDialog();
        }

        private void ShowcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowcheckBox.Checked)
            {
                textBox2.UseSystemPasswordChar = false;

            }
            else
            {
                textBox2.UseSystemPasswordChar = true;


            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).SelectionStart == 0)
                e.Handled = (e.KeyChar == (char)Keys.Space);
            else
                e.Handled = false;
        }
    }
}