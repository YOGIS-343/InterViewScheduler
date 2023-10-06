﻿// [START calendar_quickstart]
using CalendarQuickstart;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace InterViewScheduler
{
    public partial class Form2 : Form
    {
        int indexRow;
        private Regex expr;
        public string Filepath;
        public string jsonData;
        private List<Schedulers> schedulers;
        List<Combobox_Item> combobox_Items = new List<Combobox_Item>();

        public Form2()
        {
            InitializeComponent();
            //Filepath = "C:\\Users\\ShubhamY\\InterViewScheduler-Source2.020230928101432\\InterViewScheduler-Source2.0\\InterViewScheduler\\bin\\Debug\\net6.0-windows\\Data\\ScheduleBy.json";
            // Filepath = "C:\\Users\\ShubhamY\\InterViewScheduler-Source2.020230928101432\\InterViewScheduler-Source2.0\\InterViewScheduler\\bin\\Debug\\net6.0-windows\\Data\\schedule.json";
            Filepath = "C:\\Users\\ShubhamY\\Source\\GitRepos\\InterViewScheduler\\Data\\ScheduleBy.json";
            colorPicker2.DrawItem += (sender, e) => OnDrawItem(sender, e);
            colorPicker2.Items.AddRange(CreateColorCodeList().ToArray());
            schedulers = ReadJsonFile();
        }

        private List<Combobox_Item> CreateColorCodeList()
        {
            combobox_Items.Add(new Combobox_Item("Red", new Bitmap("Colorcode\\Red.jpg"), "1"));
            combobox_Items.Add(new Combobox_Item("Black", new Bitmap("Colorcode\\Black.jpg"), "2"));
            combobox_Items.Add(new Combobox_Item("Yellow", new Bitmap("Colorcode\\Yellow.jpg"), "3"));
            combobox_Items.Add(new Combobox_Item("Green", new Bitmap("Colorcode\\green.png"), "4"));
            combobox_Items.Add(new Combobox_Item("Brown", new Bitmap("Colorcode\\Brown.jpg"), "5"));
            combobox_Items.Add(new Combobox_Item("Purple", new Bitmap("Colorcode\\Purple.jpg"), "6"));

            return combobox_Items;
        }

        protected void OnDrawItem(Object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {

                e.DrawBackground();
                e.DrawFocusRectangle();
                Combobox_Item item = (Combobox_Item)colorPicker2.Items[e.Index];

                e.Graphics.DrawImage(item.Image, e.Bounds.Left, e.Bounds.Top);
                // Draw the value (in this case, the color name)
                e.Graphics.DrawString(item.Text, e.Font, new
                        SolidBrush(e.ForeColor), e.Bounds.Left + item.Image.Width, e.Bounds.Top + 2);
            }
        }
        public void button1_Click(object sender, EventArgs e)
        {

            try
            {
                var selectedItem = (Combobox_Item)colorPicker2.SelectedItem;


                if (textBox1.Text == "" || textBox2.Text == "" || colorPicker2.SelectedItem == null)
                {
                    MessageBox.Show("please fill all the details!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                else if (!Isvalidemail(textBox2.Text))
                {
                    MessageBox.Show("error,Enter valid Email");

                }
                else
                {

                    int Id = Convert.ToInt32(RecordId.Text == "" ? schedulers.Count() + 1 : RecordId.Text);
                    var found = schedulers.FirstOrDefault(c => c.Id == Id);
                    if (found != null)
                    {
                        found.Id = Id;
                        found.Name = textBox1.Text;
                        found.Email = textBox2.Text;
                        found.ColorCode = selectedItem.ColorRGB;


                        WriteJsonFile(schedulers);
                        MessageBox.Show("Data Updated successfully.");


                    }
                    else
                    {
                        Schedulers newScheduler = new Schedulers
                        {
                            Id = Id,
                            Name = textBox1.Text,
                            Email = textBox2.Text,
                            ColorCode = selectedItem.ColorRGB,
                            // ColorCode = textBox3.Text
                        };
                        schedulers.Add(newScheduler);



                        WriteJsonFile(schedulers);

                        MessageBox.Show("Data added successfully.");

                    }


                    RecordId.Text = "";
                    textBox1.Text = "";
                    textBox2.Text = "";
                    colorPicker2.SelectedItem = null;

                }
                string Rjson = File.ReadAllText(Filepath);


                var table = JsonConvert.DeserializeObject<DataSet>(Rjson).Tables[0];
                dataGridView1.DataSource = table;

            }
            catch
            {
                MessageBox.Show("please fill all the details!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void Form2_Load_1(object sender, EventArgs e)
        {
            string Rjson = File.ReadAllText(Filepath);

            var dataSet = JsonConvert.DeserializeObject<DataSet>(Rjson);

            var table = dataSet.Tables[0];
            dataGridView1.DataSource = table;

        }

        public List<Schedulers> ReadJsonFile()
        {


            if (File.Exists(Filepath))
            {
                string jsonData = File.ReadAllText(Filepath);
                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, List<Schedulers>>>(jsonData);


                if (jsonObject.ContainsKey("Schedulers"))
                {
                    schedulers = jsonObject["Schedulers"];
                }
                else
                {
                    schedulers = new List<Schedulers>();
                }

            }
            else
            {
                schedulers = new List<Schedulers>();
            }

            return schedulers;
        }
        public void WriteJsonFile(List<Schedulers> schedulers)
        {
            var jsonObject = new Dictionary<string, List<Schedulers>>
            {
                { "Schedulers", schedulers }
            };
            string jsonData = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            File.WriteAllText(Filepath, jsonData);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                indexRow = e.RowIndex;
                DataGridViewRow row = dataGridView1.Rows[indexRow];

                RecordId.Text = row.Cells[0].Value.ToString();
                textBox1.Text = row.Cells[1].Value.ToString();
                textBox2.Text = row.Cells[2].Value.ToString();
                // colorPicker2. = row.Cells[3].Value.ToString();
                colorPicker2.SelectedItem = combobox_Items.FirstOrDefault(x => x.ColorRGB == row.Cells[3].Value.ToString());
                //textBox3.Text = row.Cells[2].Value.ToString();
            }
            catch
            {
                MessageBox.Show("Select proper cell from gridview");
            }
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

        private void colorPicker2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).SelectionStart == 0)
                e.Handled = (e.KeyChar == (char)Keys.Space);
            else
                e.Handled = false;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).SelectionStart == 0)
                e.Handled = (e.KeyChar == (char)Keys.Space);
            else
                e.Handled = false;
        }
    }
}

