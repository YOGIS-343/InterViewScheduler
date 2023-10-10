namespace InterViewScheduler
{
    partial class Login
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
            label1 = new Label();
            BtnLogin = new Button();
            label2 = new Label();
            label3 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            BtnRegister = new Button();
            ShowcheckBox = new CheckBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(220, 31);
            label1.Name = "label1";
            label1.Size = new Size(90, 38);
            label1.TabIndex = 0;
            label1.Text = "Login";
            // 
            // BtnLogin
            // 
            BtnLogin.Location = new Point(360, 211);
            BtnLogin.Name = "BtnLogin";
            BtnLogin.Size = new Size(94, 37);
            BtnLogin.TabIndex = 1;
            BtnLogin.Text = "Login";
            BtnLogin.UseVisualStyleBackColor = true;
            BtnLogin.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(12, 105);
            label2.Name = "label2";
            label2.Size = new Size(105, 25);
            label2.TabIndex = 2;
            label2.Text = "Username  :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(12, 166);
            label3.Name = "label3";
            label3.Size = new Size(101, 25);
            label3.TabIndex = 3;
            label3.Text = "Password  :";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(123, 106);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(331, 27);
            textBox1.TabIndex = 4;
            textBox1.KeyPress += textBox1_KeyPress;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(123, 167);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(331, 27);
            textBox2.TabIndex = 5;
            textBox2.UseSystemPasswordChar = true;
            // 
            // BtnRegister
            // 
            BtnRegister.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnRegister.Location = new Point(255, 211);
            BtnRegister.Name = "BtnRegister";
            BtnRegister.Size = new Size(90, 37);
            BtnRegister.TabIndex = 6;
            BtnRegister.Text = "Register";
            BtnRegister.UseVisualStyleBackColor = true;
            BtnRegister.Click += BtnRegister_Click;
            // 
            // ShowcheckBox
            // 
            ShowcheckBox.AutoSize = true;
            ShowcheckBox.Location = new Point(123, 200);
            ShowcheckBox.Name = "ShowcheckBox";
            ShowcheckBox.Size = new Size(67, 24);
            ShowcheckBox.TabIndex = 17;
            ShowcheckBox.Text = "Show";
            ShowcheckBox.UseVisualStyleBackColor = true;
            ShowcheckBox.CheckedChanged += ShowcheckBox_CheckedChanged;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(486, 314);
            Controls.Add(ShowcheckBox);
            Controls.Add(BtnRegister);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(BtnLogin);
            Controls.Add(label1);
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button BtnLogin;
        private Label label2;
        private Label label3;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button BtnRegister;
        private CheckBox ShowcheckBox;
    }
}