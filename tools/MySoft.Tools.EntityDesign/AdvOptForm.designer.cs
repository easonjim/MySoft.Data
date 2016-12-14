namespace MySoft.Tools.EntityDesign
{
    partial class AdvOptForm
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
            this.checkEnableAdvOpt = new System.Windows.Forms.CheckBox();
            this.listEntities = new System.Windows.Forms.CheckedListBox();
            this.checkSelectAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkEnableAdvOpt
            // 
            this.checkEnableAdvOpt.AutoSize = true;
            this.checkEnableAdvOpt.Location = new System.Drawing.Point(13, 12);
            this.checkEnableAdvOpt.Name = "checkEnableAdvOpt";
            this.checkEnableAdvOpt.Size = new System.Drawing.Size(240, 16);
            this.checkEnableAdvOpt.TabIndex = 0;
            this.checkEnableAdvOpt.Text = "创建以下选中的实体接口到对应的实体类";
            this.checkEnableAdvOpt.UseVisualStyleBackColor = true;
            this.checkEnableAdvOpt.CheckedChanged += new System.EventHandler(this.checkEnableAdvOpt_CheckedChanged);
            // 
            // listEntities
            // 
            this.listEntities.CheckOnClick = true;
            this.listEntities.Enabled = false;
            this.listEntities.FormattingEnabled = true;
            this.listEntities.Location = new System.Drawing.Point(12, 65);
            this.listEntities.Name = "listEntities";
            this.listEntities.Size = new System.Drawing.Size(258, 260);
            this.listEntities.Sorted = true;
            this.listEntities.TabIndex = 1;
            // 
            // checkSelectAll
            // 
            this.checkSelectAll.AutoSize = true;
            this.checkSelectAll.Enabled = false;
            this.checkSelectAll.Location = new System.Drawing.Point(13, 43);
            this.checkSelectAll.Name = "checkSelectAll";
            this.checkSelectAll.Size = new System.Drawing.Size(114, 16);
            this.checkSelectAll.TabIndex = 2;
            this.checkSelectAll.Text = "全部选中/不选中";
            this.checkSelectAll.UseVisualStyleBackColor = true;
            this.checkSelectAll.CheckedChanged += new System.EventHandler(this.checkSelectAll_CheckedChanged);
            // 
            // AdvOptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 341);
            this.Controls.Add(this.checkSelectAll);
            this.Controls.Add(this.listEntities);
            this.Controls.Add(this.checkEnableAdvOpt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvOptForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkEnableAdvOpt;
        private System.Windows.Forms.CheckedListBox listEntities;
        private System.Windows.Forms.CheckBox checkSelectAll;
    }
}