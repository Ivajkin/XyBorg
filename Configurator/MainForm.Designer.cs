namespace Configurator
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ConfigHeader = new System.Windows.Forms.PictureBox();
            this.SaveNExitBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.screenSizeChLBx = new System.Windows.Forms.ComboBox();
            this.hdrEnabledChckBx = new System.Windows.Forms.CheckBox();
            this.fullscreenChckBx = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.fullscreenLbl = new System.Windows.Forms.Label();
            this.SaveNRunBtn = new System.Windows.Forms.Button();
            this.ExitBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigHeader)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConfigHeader
            // 
            this.ConfigHeader.Image = global::Configurator.Resources.ConfigHeader;
            this.ConfigHeader.Location = new System.Drawing.Point(5, 5);
            this.ConfigHeader.Margin = new System.Windows.Forms.Padding(5);
            this.ConfigHeader.Name = "ConfigHeader";
            this.ConfigHeader.Size = new System.Drawing.Size(492, 230);
            this.ConfigHeader.TabIndex = 0;
            this.ConfigHeader.TabStop = false;
            // 
            // SaveNExitBtn
            // 
            this.SaveNExitBtn.BackColor = System.Drawing.Color.Gray;
            this.SaveNExitBtn.ForeColor = System.Drawing.Color.White;
            this.SaveNExitBtn.Location = new System.Drawing.Point(157, 490);
            this.SaveNExitBtn.Name = "SaveNExitBtn";
            this.SaveNExitBtn.Size = new System.Drawing.Size(112, 32);
            this.SaveNExitBtn.TabIndex = 2;
            this.SaveNExitBtn.Text = "Сохранить и выйти";
            this.SaveNExitBtn.UseVisualStyleBackColor = false;
            this.SaveNExitBtn.Click += new System.EventHandler(this.SaveNExitBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.screenSizeChLBx);
            this.groupBox1.Controls.Add(this.hdrEnabledChckBx);
            this.groupBox1.Controls.Add(this.fullscreenChckBx);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.fullscreenLbl);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(5, 243);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(492, 241);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки";
            // 
            // screenSizeChLBx
            // 
            this.screenSizeChLBx.FormattingEnabled = true;
            this.screenSizeChLBx.Items.AddRange(new object[] {
            "640x360",
            "640x480",
            "1280x720(*)",
            "1280x1024",
            "1920x1080"});
            this.screenSizeChLBx.Location = new System.Drawing.Point(152, 108);
            this.screenSizeChLBx.Name = "screenSizeChLBx";
            this.screenSizeChLBx.Size = new System.Drawing.Size(121, 21);
            this.screenSizeChLBx.TabIndex = 2;
            this.screenSizeChLBx.SelectedIndexChanged += new System.EventHandler(this.screenSizeChLBx_SelectedIndexChanged);
            // 
            // hdrEnabledChckBx
            // 
            this.hdrEnabledChckBx.AutoSize = true;
            this.hdrEnabledChckBx.Location = new System.Drawing.Point(248, 191);
            this.hdrEnabledChckBx.Name = "hdrEnabledChckBx";
            this.hdrEnabledChckBx.Size = new System.Drawing.Size(15, 14);
            this.hdrEnabledChckBx.TabIndex = 1;
            this.hdrEnabledChckBx.UseVisualStyleBackColor = true;
            this.hdrEnabledChckBx.CheckedChanged += new System.EventHandler(this.hdrEnabledChckBx_CheckedChanged);
            // 
            // fullscreenChckBx
            // 
            this.fullscreenChckBx.AutoSize = true;
            this.fullscreenChckBx.Location = new System.Drawing.Point(251, 33);
            this.fullscreenChckBx.Name = "fullscreenChckBx";
            this.fullscreenChckBx.Size = new System.Drawing.Size(15, 14);
            this.fullscreenChckBx.TabIndex = 1;
            this.fullscreenChckBx.UseVisualStyleBackColor = true;
            this.fullscreenChckBx.CheckedChanged += new System.EventHandler(this.fullscreenChckBx_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Размеры экрана";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 192);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Использовать эффект HDR";
            // 
            // fullscreenLbl
            // 
            this.fullscreenLbl.AutoSize = true;
            this.fullscreenLbl.Location = new System.Drawing.Point(51, 34);
            this.fullscreenLbl.Name = "fullscreenLbl";
            this.fullscreenLbl.Size = new System.Drawing.Size(194, 13);
            this.fullscreenLbl.TabIndex = 0;
            this.fullscreenLbl.Text = "Запустить в полноэкранном режиме";
            // 
            // SaveNRunBtn
            // 
            this.SaveNRunBtn.BackColor = System.Drawing.Color.Gray;
            this.SaveNRunBtn.ForeColor = System.Drawing.Color.White;
            this.SaveNRunBtn.Location = new System.Drawing.Point(12, 490);
            this.SaveNRunBtn.Name = "SaveNRunBtn";
            this.SaveNRunBtn.Size = new System.Drawing.Size(139, 32);
            this.SaveNRunBtn.TabIndex = 2;
            this.SaveNRunBtn.Text = "Сохранить и запустить";
            this.SaveNRunBtn.UseVisualStyleBackColor = false;
            this.SaveNRunBtn.Click += new System.EventHandler(this.SaveNRunBtn_Click);
            // 
            // ExitBtn
            // 
            this.ExitBtn.BackColor = System.Drawing.Color.Gray;
            this.ExitBtn.ForeColor = System.Drawing.Color.White;
            this.ExitBtn.Location = new System.Drawing.Point(275, 490);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(131, 32);
            this.ExitBtn.TabIndex = 2;
            this.ExitBtn.Text = "Выйти без сохранения";
            this.ExitBtn.UseVisualStyleBackColor = false;
            this.ExitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(501, 534);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SaveNRunBtn);
            this.Controls.Add(this.ExitBtn);
            this.Controls.Add(this.SaveNExitBtn);
            this.Controls.Add(this.ConfigHeader);
            this.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(517, 572);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(517, 572);
            this.Name = "MainForm";
            this.Text = "Configurator";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ConfigHeader)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox ConfigHeader;
        private System.Windows.Forms.Button SaveNExitBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button SaveNRunBtn;
        private System.Windows.Forms.Button ExitBtn;
        private System.Windows.Forms.Label fullscreenLbl;
        private System.Windows.Forms.CheckBox fullscreenChckBx;
        private System.Windows.Forms.CheckBox hdrEnabledChckBx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox screenSizeChLBx;
        private System.Windows.Forms.Label label2;
    }
}

