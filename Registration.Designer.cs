namespace InterViewScheduler
{
    partial class Registration
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
            components = new System.ComponentModel.Container();
            label1 = new Label();
            BtnRegister = new Button();
            RegPassText = new TextBox();
            RegUsernameText = new TextBox();
            label3 = new Label();
            label2 = new Label();
            CnfrmPassBtn = new Label();
            RegCnfrmPassText = new TextBox();
            ShowcheckBox = new CheckBox();
            errorProvider1 = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(154, 9);
            label1.Name = "label1";
            label1.Size = new Size(176, 38);
            label1.TabIndex = 1;
            label1.Text = "Registration";
            // 
            // BtnRegister
            // 
            BtnRegister.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnRegister.Location = new Point(359, 224);
            BtnRegister.Name = "BtnRegister";
            BtnRegister.Size = new Size(90, 37);
            BtnRegister.TabIndex = 12;
            BtnRegister.Text = "Register";
            BtnRegister.UseVisualStyleBackColor = true;
            BtnRegister.Click += BtnRegister_Click;
            // 
            // RegPassText
            // 
            RegPassText.Location = new Point(172, 118);
            RegPassText.Name = "RegPassText";
            RegPassText.Size = new Size(277, 27);
            RegPassText.TabIndex = 11;
            RegPassText.UseSystemPasswordChar = true;
            RegPassText.KeyUp += RegPassText_KeyUp;
            // 
            // RegUsernameText
            // 
            RegUsernameText.Location = new Point(172, 68);
            RegUsernameText.Name = "RegUsernameText";
            RegUsernameText.Size = new Size(277, 27);
            RegUsernameText.TabIndex = 10;
            RegUsernameText.KeyPress += RegUsernameText_KeyPress;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(65, 117);
            label3.Name = "label3";
            label3.Size = new Size(101, 25);
            label3.TabIndex = 9;
            label3.Text = "Password  :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(61, 67);
            label2.Name = "label2";
            label2.Size = new Size(105, 25);
            label2.TabIndex = 8;
            label2.Text = "Username  :";
            // 
            // CnfrmPassBtn
            // 
            CnfrmPassBtn.AutoSize = true;
            CnfrmPassBtn.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            CnfrmPassBtn.Location = new Point(-1, 167);
            CnfrmPassBtn.Name = "CnfrmPassBtn";
            CnfrmPassBtn.Size = new Size(167, 25);
            CnfrmPassBtn.TabIndex = 13;
            CnfrmPassBtn.Text = "confirm Password  :";
            // 
            // RegCnfrmPassText
            // 
            RegCnfrmPassText.Location = new Point(172, 168);
            RegCnfrmPassText.Name = "RegCnfrmPassText";
            RegCnfrmPassText.Size = new Size(277, 27);
            RegCnfrmPassText.TabIndex = 14;
            RegCnfrmPassText.UseSystemPasswordChar = true;
            RegCnfrmPassText.TextChanged += RegCnfrmPassText_TextChanged;
            // 
            // ShowcheckBox
            // 
            ShowcheckBox.AutoSize = true;
            ShowcheckBox.Location = new Point(172, 201);
            ShowcheckBox.Name = "ShowcheckBox";
            ShowcheckBox.Size = new Size(67, 24);
            ShowcheckBox.TabIndex = 16;
            ShowcheckBox.Text = "Show";
            ShowcheckBox.UseVisualStyleBackColor = true;
            ShowcheckBox.CheckedChanged += ShowcheckBox_CheckedChanged;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // Registration
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(498, 293);
            Controls.Add(ShowcheckBox);
            Controls.Add(RegCnfrmPassText);
            Controls.Add(CnfrmPassBtn);
            Controls.Add(BtnRegister);
            Controls.Add(RegPassText);
            Controls.Add(RegUsernameText);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Registration";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Registration";
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button BtnRegister;
        private TextBox RegPassText;
        private TextBox RegUsernameText;
        private Label label3;
        private Label label2;
        private Label CnfrmPassBtn;
        private TextBox RegCnfrmPassText;
        private CheckBox ShowcheckBox;
        private ErrorProvider errorProvider1;
    }
}