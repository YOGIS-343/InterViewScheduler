using CalendarQuickstart;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace InterViewScheduler
{
    public partial class InterviewerAdd : Form
    {
        private string filePath;
        public List<Interviewers> loopslist;
        private Regex expr;
        int indexRow;
        private List<Interviewers> interviewers;
        private List<Combobox_Item> dropDownItems = new List<Combobox_Item>();
        public InterviewerAdd()
        {
            InitializeComponent();
            filePath = "Data\\Interviewer.json";
            loopslist = new List<Interviewers>();
            colorPicker1.DrawItem += (sender, e) => OnDrawItem(sender, e);
            colorPicker1.Items.AddRange(CreateColorCodeList().ToArray());

            interviewers = ReadJsonFile();
        }
        private List<Combobox_Item> CreateColorCodeList()
        {
            dropDownItems.Add(new Combobox_Item("Red", new Bitmap("Colorcode\\Red.jpg"), "1"));
            dropDownItems.Add(new Combobox_Item("Black", new Bitmap("Colorcode\\Black.jpg"), "2"));
            dropDownItems.Add(new Combobox_Item("Yellow", new Bitmap("Colorcode\\Yellow.jpg"), "3"));
            dropDownItems.Add(new Combobox_Item("Green", new Bitmap("Colorcode\\green.png"), "4"));
            dropDownItems.Add(new Combobox_Item("Brown", new Bitmap("Colorcode\\Brown.jpg"), "5"));
            dropDownItems.Add(new Combobox_Item("Purple", new Bitmap("Colorcode\\Purple.jpg"), "6"));
            return dropDownItems;
        }

        protected void OnDrawItem(Object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {

                e.DrawBackground();
                e.DrawFocusRectangle();
                Combobox_Item item = (Combobox_Item)colorPicker1.Items[e.Index];

                e.Graphics.DrawImage(item.Image, e.Bounds.Left, e.Bounds.Top);

                e.Graphics.DrawString(item.Text, e.Font, new
                        SolidBrush(e.ForeColor), e.Bounds.Left + item.Image.Width, e.Bounds.Top + 2);
            }


        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedItem = (Combobox_Item)colorPicker1.SelectedItem;

                if (txtName.Text == "" || txtInterviewerEmail.Text == "" || txtZoomURL.Text == "" || txtMeetingId.Text == "" || txtPasscode.Text == "" || colorPicker1.SelectedItem == null)
                {
                    MessageBox.Show("Please Ensure You Fill all Details");

                }
                else if (!Isvalidemail(txtInterviewerEmail.Text))
                {
                    MessageBox.Show("Invalid  email address");
                }
                else
                {
                    int Id = Convert.ToInt32(RecordId.Text == "" ? interviewers.Count() + 1 : RecordId.Text);
                    var found = interviewers.FirstOrDefault(c => c.Id == Id);
                    if (found != null)
                    {
                        found.Id = Id;
                        found.Name = txtName.Text;
                        found.interviewerEmail = txtInterviewerEmail.Text;
                        found.ZoomUrl = txtZoomURL.Text;
                        found.MeettingId = txtMeetingId.Text;
                        found.PassCode = txtPasscode.Text;
                        found.InterViewerColorCode = selectedItem.ColorRGB;
                        WriteJsonFile(interviewers);
                        MessageBox.Show("Data Updated successfully.");

                    }
                    else
                    {
                        Interviewers interviewers1 = new Interviewers
                        {
                            Id = Id,
                            Name = txtName.Text,
                            interviewerEmail = txtInterviewerEmail.Text,
                            ZoomUrl = txtZoomURL.Text,
                            MeettingId = txtMeetingId.Text,
                            PassCode = txtPasscode.Text,
                            InterViewerColorCode = selectedItem.ColorRGB,

                        };
                        interviewers.Add(interviewers1);

                        WriteJsonFile(interviewers);

                        MessageBox.Show("Data added successfully.");
                    }

                    RecordId.Text = "";
                    txtName.Text = "";
                    txtInterviewerEmail.Text = "";
                    txtZoomURL.Text = "";
                    txtMeetingId.Text = "";
                    txtPasscode.Text = "";
                    colorPicker1.SelectedItem = null;
                }
                InterviewerAdd_Load(sender, e);
            }
            catch
            {
                MessageBox.Show("Please Enter Details");
            }
        }

        public List<Interviewers> ReadJsonFile()
        {

            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, List<Interviewers>>>(jsonData);
                if (jsonObject.ContainsKey("Interviewers"))
                {
                    interviewers = jsonObject["Interviewers"];
                }
                else
                {
                    interviewers = new List<Interviewers>();
                }

            }
            else
            {
                interviewers = new List<Interviewers>();
            }

            return interviewers;
        }

        public void WriteJsonFile(List<Interviewers> interviewers)
        {
            var jsonObject = new Dictionary<string, List<Interviewers>>
        {
            { "Interviewers", interviewers }
        };
            string jsonData = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }

        private void InterviewerAdd_Load(object sender, EventArgs e)
        {
            string Rjson = File.ReadAllText(filePath);

            var dataSet = JsonConvert.DeserializeObject<DataSet>(Rjson);

            var table = dataSet.Tables[0];
            dataGridView1.DataSource = table;
            dataGridView1.Columns[0].Visible = false;

        }

        public bool Isvalidemail(string email)
        {
            expr = new Regex(@"\b[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}\b", RegexOptions.IgnoreCase);
            if (expr.IsMatch(email))
            {
                return true;
            }
            else return false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                indexRow = e.RowIndex;
                DataGridViewRow row = dataGridView1.Rows[indexRow];
                RecordId.Text = row.Cells[0].Value.ToString();
                txtName.Text = row.Cells[1].Value.ToString();
                txtInterviewerEmail.Text = row.Cells[2].Value.ToString();
                txtZoomURL.Text = row.Cells[3].Value.ToString();
                txtMeetingId.Text = row.Cells[4].Value.ToString();
                txtPasscode.Text = row.Cells[5].Value.ToString();
                colorPicker1.SelectedItem = dropDownItems.FirstOrDefault(x => x.ColorRGB == row.Cells[6].Value.ToString());

            }
            catch
            {
                MessageBox.Show("No Record Found");

            }


        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as System.Windows.Forms.TextBox).SelectionStart == 0)
                e.Handled = (e.KeyChar == (char)Keys.Space);
            else
                e.Handled = false;
        }

        private void txtInterviewerEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as System.Windows.Forms.TextBox).SelectionStart == 0)
                e.Handled = (e.KeyChar == (char)Keys.Space);
            else
                e.Handled = false;
        }

        private void txtZoomURL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as System.Windows.Forms.TextBox).SelectionStart == 0)
                e.Handled = (e.KeyChar == (char)Keys.Space);
            else
                e.Handled = false;
        }

        private void txtMeetingId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as System.Windows.Forms.TextBox).SelectionStart == 0)
                e.Handled = (e.KeyChar == (char)Keys.Space);
            else
                e.Handled = false;
        }

        private void Delete_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    int Id = Convert.ToInt32(RecordId.Text == "" ? interviewers.Count() + 1 : RecordId.Text);
                    var found = interviewers.FirstOrDefault(c => c.Id == Id);
                    if (found != null)
                    {

                        interviewers.Remove(found);

                        WriteJsonFile(interviewers);

                        string Rjson = File.ReadAllText(filePath);


                        var table = JsonConvert.DeserializeObject<DataSet>(Rjson).Tables[0];
                        dataGridView1.DataSource = table;


                    }

                }

            }
            catch (Exception)
            {
                throw new ArgumentException("User has not deleted");
            }
        }
    }







}

