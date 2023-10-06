namespace InterViewScheduler
{
    partial class Form2
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            dropDownItem1 = new ColorPicker();
            dropDownItem2 = new ColorPicker();
            colorPicker1 = new ColorPicker();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            button1 = new Button();
            dataGridView1 = new DataGridView();
            colorPicker2 = new ColorPicker();
            RecordId = new Label();
            button3 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dropDownItem1
            // 
            dropDownItem1.Location = new Point(0, 0);
            dropDownItem1.Name = "dropDownItem1";
            dropDownItem1.Size = new Size(121, 28);
            dropDownItem1.TabIndex = 0;
            // 
            // dropDownItem2
            // 
            dropDownItem2.Location = new Point(0, 0);
            dropDownItem2.Name = "dropDownItem2";
            dropDownItem2.Size = new Size(121, 28);
            dropDownItem2.TabIndex = 0;
            // 
            // colorPicker1
            // 
            colorPicker1.Location = new Point(0, 0);
            colorPicker1.Name = "colorPicker1";
            colorPicker1.Size = new Size(121, 28);
            colorPicker1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            label1.Location = new Point(5, 7);
            label1.Name = "label1";
            label1.Size = new Size(142, 23);
            label1.TabIndex = 0;
            label1.Text = "Recruter Details:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 43);
            label2.Name = "label2";
            label2.Size = new Size(58, 20);
            label2.TabIndex = 1;
            label2.Text = "Name:-";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(17, 91);
            label3.Name = "label3";
            label3.Size = new Size(55, 20);
            label3.TabIndex = 2;
            label3.Text = "Email:-";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 136);
            label4.Name = "label4";
            label4.Size = new Size(89, 20);
            label4.TabIndex = 3;
            label4.Text = "ColorCode:-";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(113, 43);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(390, 27);
            textBox1.TabIndex = 4;
            textBox1.KeyPress += textBox1_KeyPress;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(113, 91);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(390, 27);
            textBox2.TabIndex = 5;
            textBox2.KeyPress += textBox2_KeyPress;
            // 
            // button1
            // 
            button1.Location = new Point(251, 178);
            button1.Name = "button1";
            button1.Size = new Size(123, 35);
            button1.TabIndex = 7;
            button1.Text = "Save";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(0, 236);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 29;
            dataGridView1.Size = new Size(555, 213);
            dataGridView1.TabIndex = 8;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // colorPicker2
            // 
            colorPicker2.DrawMode = DrawMode.OwnerDrawFixed;
            colorPicker2.DropDownStyle = ComboBoxStyle.DropDownList;
            colorPicker2.FormattingEnabled = true;
            colorPicker2.Location = new Point(113, 133);
            colorPicker2.Name = "colorPicker2";
            colorPicker2.Size = new Size(390, 28);
            colorPicker2.TabIndex = 9;
            colorPicker2.SelectedIndexChanged += colorPicker2_SelectedIndexChanged;
            // 
            // RecordId
            // 
            RecordId.AutoSize = true;
            RecordId.Location = new Point(54, 27);
            RecordId.Name = "RecordId";
            RecordId.Size = new Size(0, 20);
            RecordId.TabIndex = 11;
            RecordId.Visible = false;
            // 
            // button3
            // 
            button3.Location = new Point(380, 178);
            button3.Name = "button3";
            button3.Size = new Size(123, 35);
            button3.TabIndex = 12;
            button3.Text = "Delete";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(559, 451);
            Controls.Add(button3);
            Controls.Add(RecordId);
            Controls.Add(colorPicker2);
            Controls.Add(dataGridView1);
            Controls.Add(button1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form2";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Interview Scheduler - Recruter Details";
            Load += Form2_Load_1;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button button1;
        private DataGridView dgvCandList;
        private DataGridView dataGridView1;
        private ColorPicker colorPicker1;
        private ColorPicker dropDownItem1;
        private ColorPicker dropDownItem2;
        private ColorPicker colorPicker2;
        private Button button2;
        private Label RecordId;
        private Button button3;
    }
}