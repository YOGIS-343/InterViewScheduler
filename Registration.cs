using CalendarQuickstart;
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
    public partial class Registration : Form
    {
        public Registration()
        {
            InitializeComponent();
        }
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = RegUsernameText.Text;
            string password = RegPassText.Text;


            byte[] encData_byte = new byte[password.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
            string encodedData = Convert.ToBase64String(encData_byte);


            List<User> users = JsonHelper.DeserializeFromFile<User>("Data\\users.json");

            var found = users.FirstOrDefault(c => c.Username == username);
            if (found != null)
            {
                MessageBox.Show("User Already Exists!");
            }
            else
            {
                users.Add(new User { Username = username, Password = encodedData });
                JsonHelper.SerializeToFile(users, "Data\\users.json");
                MessageBox.Show("Registration successful!");

                Close();
            }
        }
        private void RegPassText_KeyUp(object sender, KeyEventArgs e)
        {

        }
        private void ShowcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowcheckBox.Checked)
            {
                RegCnfrmPassText.UseSystemPasswordChar = false;
                RegPassText.UseSystemPasswordChar = false;
            }
            else
            {
                RegCnfrmPassText.UseSystemPasswordChar = true;
                RegPassText.UseSystemPasswordChar = true;
            }
        }
        private void RegCnfrmPassText_TextChanged(object sender, EventArgs e)
        {
            if (RegCnfrmPassText.Text != "" && RegPassText.Text == RegCnfrmPassText.Text)
            {
                errorProvider1.SetError(RegCnfrmPassText, "");
            }
            else
            {
                errorProvider1.SetError(RegCnfrmPassText, "Password does not match");
            }
        }
        private void RegUsernameText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).SelectionStart == 0)
                e.Handled = (e.KeyChar == (char)Keys.Space);
            else
                e.Handled = false;
        }
    }
}
