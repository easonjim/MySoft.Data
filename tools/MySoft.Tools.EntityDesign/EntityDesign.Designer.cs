namespace MySoft.Tools.EntityDesign
{
    partial class EntityDesign
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
            this.components = new System.ComponentModel.Container();
            this.tables = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.selectAll = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.views = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.output = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiClear = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.txtConnStr = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.radioAccess = new System.Windows.Forms.RadioButton();
            this.outputLanguage = new System.Windows.Forms.ComboBox();
            this.selectAllView = new System.Windows.Forms.CheckBox();
            this.btnGen = new System.Windows.Forms.Button();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnGenEntity = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtOutputNamespace = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnAdvOpt = new System.Windows.Forms.Button();
            this.checkUpperFieldChar = new System.Windows.Forms.CheckBox();
            this.radioFirst = new System.Windows.Forms.RadioButton();
            this.radioAll = new System.Windows.Forms.RadioButton();
            this.radioSql2005 = new System.Windows.Forms.RadioButton();
            this.radioSql = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioOracle = new System.Windows.Forms.RadioButton();
            this.radioMySQL = new System.Windows.Forms.RadioButton();
            this.radioSQLite = new System.Windows.Forms.RadioButton();
            this.checkUpperTableChar = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtExceptPrefix = new System.Windows.Forms.TextBox();
            this.chkNoMapping = new System.Windows.Forms.CheckBox();
            this.chkCreateAssembly = new System.Windows.Forms.CheckBox();
            this.chkCreateEntity = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tables
            // 
            this.tables.CheckOnClick = true;
            this.tables.FormattingEnabled = true;
            this.tables.Location = new System.Drawing.Point(12, 55);
            this.tables.Name = "tables";
            this.tables.Size = new System.Drawing.Size(220, 260);
            this.tables.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "所有表";
            // 
            // selectAll
            // 
            this.selectAll.AutoSize = true;
            this.selectAll.Location = new System.Drawing.Point(12, 11);
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(114, 16);
            this.selectAll.TabIndex = 2;
            this.selectAll.Text = "全部选中/不选中";
            this.selectAll.UseVisualStyleBackColor = true;
            this.selectAll.Click += new System.EventHandler(this.selectAll_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 345);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "所有视图";
            // 
            // views
            // 
            this.views.CheckOnClick = true;
            this.views.FormattingEnabled = true;
            this.views.Location = new System.Drawing.Point(12, 360);
            this.views.Name = "views";
            this.views.Size = new System.Drawing.Size(220, 244);
            this.views.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(250, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "输出类型";
            // 
            // output
            // 
            this.output.ContextMenuStrip = this.contextMenuStrip1;
            this.output.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.output.Location = new System.Drawing.Point(-1, 0);
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(620, 344);
            this.output.TabIndex = 7;
            this.output.Text = "";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSelectAll,
            this.toolStripSeparator1,
            this.tsmiCopy,
            this.tsmiPaste,
            this.toolStripSeparator2,
            this.tsmiClear});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(118, 104);
            this.contextMenuStrip1.Text = "全选(&A)";
            // 
            // tsmiSelectAll
            // 
            this.tsmiSelectAll.Name = "tsmiSelectAll";
            this.tsmiSelectAll.Size = new System.Drawing.Size(117, 22);
            this.tsmiSelectAll.Text = "全选(&A)";
            this.tsmiSelectAll.Click += new System.EventHandler(this.tsmiSelectAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(114, 6);
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.Size = new System.Drawing.Size(117, 22);
            this.tsmiCopy.Text = "复制(&C)";
            this.tsmiCopy.Click += new System.EventHandler(this.tsmiCopy_Click);
            // 
            // tsmiPaste
            // 
            this.tsmiPaste.Name = "tsmiPaste";
            this.tsmiPaste.Size = new System.Drawing.Size(117, 22);
            this.tsmiPaste.Text = "粘贴(&V)";
            this.tsmiPaste.Click += new System.EventHandler(this.tsmiPaste_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(114, 6);
            // 
            // tsmiClear
            // 
            this.tsmiClear.Name = "tsmiClear";
            this.tsmiClear.Size = new System.Drawing.Size(117, 22);
            this.tsmiClear.Text = "清除(&D)";
            this.tsmiClear.Click += new System.EventHandler(this.tsmiClear_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(251, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "连接服务器及数据库的字符串";
            // 
            // txtConnStr
            // 
            this.txtConnStr.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtConnStr.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtConnStr.Location = new System.Drawing.Point(253, 26);
            this.txtConnStr.Name = "txtConnStr";
            this.txtConnStr.Size = new System.Drawing.Size(619, 21);
            this.txtConnStr.TabIndex = 9;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(253, 99);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(302, 21);
            this.btnConnect.TabIndex = 10;
            this.btnConnect.Text = "连接服务器";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // radioAccess
            // 
            this.radioAccess.AutoSize = true;
            this.radioAccess.Location = new System.Drawing.Point(0, 24);
            this.radioAccess.Name = "radioAccess";
            this.radioAccess.Size = new System.Drawing.Size(77, 16);
            this.radioAccess.TabIndex = 12;
            this.radioAccess.Text = "MS Access";
            this.radioAccess.UseVisualStyleBackColor = true;
            // 
            // outputLanguage
            // 
            this.outputLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outputLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputLanguage.FormattingEnabled = true;
            this.outputLanguage.Items.AddRange(new object[] {
            "C#",
            "VB.NET"});
            this.outputLanguage.Location = new System.Drawing.Point(309, 148);
            this.outputLanguage.Name = "outputLanguage";
            this.outputLanguage.Size = new System.Drawing.Size(100, 21);
            this.outputLanguage.TabIndex = 18;
            // 
            // selectAllView
            // 
            this.selectAllView.AutoSize = true;
            this.selectAllView.Location = new System.Drawing.Point(12, 321);
            this.selectAllView.Name = "selectAllView";
            this.selectAllView.Size = new System.Drawing.Size(114, 16);
            this.selectAllView.TabIndex = 19;
            this.selectAllView.Text = "全部选中/不选中";
            this.selectAllView.UseVisualStyleBackColor = true;
            this.selectAllView.Click += new System.EventHandler(this.selectAllView_CheckedChanged);
            // 
            // btnGen
            // 
            this.btnGen.Enabled = false;
            this.btnGen.Location = new System.Drawing.Point(570, 99);
            this.btnGen.Name = "btnGen";
            this.btnGen.Size = new System.Drawing.Size(302, 21);
            this.btnGen.TabIndex = 20;
            this.btnGen.Text = "生成实体接口";
            this.btnGen.UseVisualStyleBackColor = true;
            this.btnGen.Click += new System.EventHandler(this.btnGen_Click);
            // 
            // txtPrefix
            // 
            this.txtPrefix.Location = new System.Drawing.Point(805, 73);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(66, 21);
            this.txtPrefix.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(674, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 12);
            this.label5.TabIndex = 22;
            this.label5.Text = "不参与转换字符串前缀";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(538, 176);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(67, 21);
            this.btnBrowse.TabIndex = 25;
            this.btnBrowse.Text = "浏览";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(253, 177);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = true;
            this.txtFileName.Size = new System.Drawing.Size(279, 21);
            this.txtFileName.TabIndex = 24;
            // 
            // btnGenEntity
            // 
            this.btnGenEntity.Enabled = false;
            this.btnGenEntity.Location = new System.Drawing.Point(611, 177);
            this.btnGenEntity.Name = "btnGenEntity";
            this.btnGenEntity.Size = new System.Drawing.Size(122, 21);
            this.btnGenEntity.TabIndex = 26;
            this.btnGenEntity.Text = "生成实体类";
            this.btnGenEntity.UseVisualStyleBackColor = true;
            this.btnGenEntity.Click += new System.EventHandler(this.btnGenEntity_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "dll";
            this.openFileDialog1.Filter = "Entity Design Assembly|*.dll";
            // 
            // txtOutputNamespace
            // 
            this.txtOutputNamespace.Location = new System.Drawing.Point(516, 148);
            this.txtOutputNamespace.Name = "txtOutputNamespace";
            this.txtOutputNamespace.Size = new System.Drawing.Size(118, 21);
            this.txtOutputNamespace.TabIndex = 28;
            this.txtOutputNamespace.Text = "Example.Model";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(417, 152);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 12);
            this.label6.TabIndex = 27;
            this.label6.Text = "输出实体命名空间";
            // 
            // btnAdvOpt
            // 
            this.btnAdvOpt.Enabled = false;
            this.btnAdvOpt.Location = new System.Drawing.Point(739, 177);
            this.btnAdvOpt.Name = "btnAdvOpt";
            this.btnAdvOpt.Size = new System.Drawing.Size(134, 21);
            this.btnAdvOpt.TabIndex = 29;
            this.btnAdvOpt.Text = "高级选项";
            this.btnAdvOpt.UseVisualStyleBackColor = true;
            this.btnAdvOpt.Click += new System.EventHandler(this.btnAdvOpt_Click);
            // 
            // checkUpperFieldChar
            // 
            this.checkUpperFieldChar.AutoSize = true;
            this.checkUpperFieldChar.Location = new System.Drawing.Point(495, 76);
            this.checkUpperFieldChar.Name = "checkUpperFieldChar";
            this.checkUpperFieldChar.Size = new System.Drawing.Size(120, 16);
            this.checkUpperFieldChar.TabIndex = 31;
            this.checkUpperFieldChar.Text = "转换字段名为大写";
            this.checkUpperFieldChar.UseVisualStyleBackColor = true;
            // 
            // radioFirst
            // 
            this.radioFirst.AutoSize = true;
            this.radioFirst.Checked = true;
            this.radioFirst.Location = new System.Drawing.Point(705, 52);
            this.radioFirst.Name = "radioFirst";
            this.radioFirst.Size = new System.Drawing.Size(59, 16);
            this.radioFirst.TabIndex = 32;
            this.radioFirst.TabStop = true;
            this.radioFirst.Text = "首字符";
            this.radioFirst.UseVisualStyleBackColor = true;
            // 
            // radioAll
            // 
            this.radioAll.AutoSize = true;
            this.radioAll.Location = new System.Drawing.Point(762, 52);
            this.radioAll.Name = "radioAll";
            this.radioAll.Size = new System.Drawing.Size(47, 16);
            this.radioAll.TabIndex = 33;
            this.radioAll.Text = "全部";
            this.radioAll.UseVisualStyleBackColor = true;
            // 
            // radioSql2005
            // 
            this.radioSql2005.AutoSize = true;
            this.radioSql2005.Location = new System.Drawing.Point(85, 3);
            this.radioSql2005.Name = "radioSql2005";
            this.radioSql2005.Size = new System.Drawing.Size(71, 16);
            this.radioSql2005.TabIndex = 34;
            this.radioSql2005.Text = "SQL 2005";
            this.radioSql2005.UseVisualStyleBackColor = true;
            // 
            // radioSql
            // 
            this.radioSql.AutoSize = true;
            this.radioSql.Checked = true;
            this.radioSql.Location = new System.Drawing.Point(0, 3);
            this.radioSql.Name = "radioSql";
            this.radioSql.Size = new System.Drawing.Size(71, 16);
            this.radioSql.TabIndex = 35;
            this.radioSql.TabStop = true;
            this.radioSql.Text = "SQL 2000";
            this.radioSql.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioOracle);
            this.panel1.Controls.Add(this.radioMySQL);
            this.panel1.Controls.Add(this.radioSQLite);
            this.panel1.Controls.Add(this.radioSql2005);
            this.panel1.Controls.Add(this.radioSql);
            this.panel1.Controls.Add(this.radioAccess);
            this.panel1.Location = new System.Drawing.Point(253, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(226, 44);
            this.panel1.TabIndex = 36;
            // 
            // radioOracle
            // 
            this.radioOracle.AutoSize = true;
            this.radioOracle.Location = new System.Drawing.Point(162, 3);
            this.radioOracle.Name = "radioOracle";
            this.radioOracle.Size = new System.Drawing.Size(59, 16);
            this.radioOracle.TabIndex = 36;
            this.radioOracle.Text = "Oracle";
            this.radioOracle.UseVisualStyleBackColor = true;
            // 
            // radioMySQL
            // 
            this.radioMySQL.AutoSize = true;
            this.radioMySQL.Location = new System.Drawing.Point(162, 24);
            this.radioMySQL.Name = "radioMySQL";
            this.radioMySQL.Size = new System.Drawing.Size(53, 16);
            this.radioMySQL.TabIndex = 36;
            this.radioMySQL.Text = "MySQL";
            this.radioMySQL.UseVisualStyleBackColor = true;
            // 
            // radioSQLite
            // 
            this.radioSQLite.AutoSize = true;
            this.radioSQLite.Location = new System.Drawing.Point(85, 24);
            this.radioSQLite.Name = "radioSQLite";
            this.radioSQLite.Size = new System.Drawing.Size(59, 16);
            this.radioSQLite.TabIndex = 36;
            this.radioSQLite.Text = "SQLite";
            this.radioSQLite.UseVisualStyleBackColor = true;
            // 
            // checkUpperTableChar
            // 
            this.checkUpperTableChar.AutoSize = true;
            this.checkUpperTableChar.Location = new System.Drawing.Point(495, 54);
            this.checkUpperTableChar.Name = "checkUpperTableChar";
            this.checkUpperTableChar.Size = new System.Drawing.Size(108, 16);
            this.checkUpperTableChar.TabIndex = 36;
            this.checkUpperTableChar.Text = "转换表名为大写";
            this.checkUpperTableChar.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(617, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(257, 12);
            this.label7.TabIndex = 37;
            this.label7.Text = "将表名或字段名                  转换为大写";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(692, 583);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 42;
            this.button1.Text = "选择文件夹";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(253, 583);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(433, 21);
            this.txtFolder.TabIndex = 43;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(775, 583);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(98, 23);
            this.button2.TabIndex = 44;
            this.button2.Text = "生成多个文件";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(252, 127);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 46;
            this.label8.Text = "去除表前缀";
            // 
            // txtExceptPrefix
            // 
            this.txtExceptPrefix.Location = new System.Drawing.Point(319, 123);
            this.txtExceptPrefix.Name = "txtExceptPrefix";
            this.txtExceptPrefix.Size = new System.Drawing.Size(61, 21);
            this.txtExceptPrefix.TabIndex = 45;
            // 
            // chkNoMapping
            // 
            this.chkNoMapping.AutoSize = true;
            this.chkNoMapping.Location = new System.Drawing.Point(391, 126);
            this.chkNoMapping.Name = "chkNoMapping";
            this.chkNoMapping.Size = new System.Drawing.Size(168, 16);
            this.chkNoMapping.TabIndex = 47;
            this.chkNoMapping.Text = "通过配置文件配置映射关系";
            this.chkNoMapping.UseVisualStyleBackColor = true;
            // 
            // chkCreateAssembly
            // 
            this.chkCreateAssembly.AutoSize = true;
            this.chkCreateAssembly.Enabled = false;
            this.chkCreateAssembly.Location = new System.Drawing.Point(560, 126);
            this.chkCreateAssembly.Name = "chkCreateAssembly";
            this.chkCreateAssembly.Size = new System.Drawing.Size(144, 16);
            this.chkCreateAssembly.TabIndex = 48;
            this.chkCreateAssembly.Text = "从数据库直接生成实体";
            this.chkCreateAssembly.UseVisualStyleBackColor = true;
            this.chkCreateAssembly.CheckedChanged += new System.EventHandler(this.chkCreateAssembly_CheckedChanged);
            // 
            // chkCreateEntity
            // 
            this.chkCreateEntity.AutoSize = true;
            this.chkCreateEntity.Location = new System.Drawing.Point(705, 126);
            this.chkCreateEntity.Name = "chkCreateEntity";
            this.chkCreateEntity.Size = new System.Drawing.Size(96, 16);
            this.chkCreateEntity.TabIndex = 51;
            this.chkCreateEntity.Text = "生成简单实体";
            this.chkCreateEntity.UseVisualStyleBackColor = true;
            this.chkCreateEntity.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(252, 204);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(627, 373);
            this.tabControl1.TabIndex = 52;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.output);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(619, 347);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "实体类";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richTextBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(619, 347);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "服务类";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.richTextBox1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox1.Location = new System.Drawing.Point(-1, 1);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(620, 344);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(640, 152);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(101, 12);
            this.label9.TabIndex = 27;
            this.label9.Text = "输出服务命名空间";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(739, 148);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(148, 21);
            this.textBox1.TabIndex = 28;
            this.textBox1.Text = "Example.Service";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(803, 125);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(84, 16);
            this.checkBox1.TabIndex = 53;
            this.checkBox1.Text = "生成服务类";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            // 
            // EntityDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 623);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chkCreateEntity);
            this.Controls.Add(this.chkCreateAssembly);
            this.Controls.Add(this.txtExceptPrefix);
            this.Controls.Add(this.chkNoMapping);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkUpperTableChar);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.radioAll);
            this.Controls.Add(this.radioFirst);
            this.Controls.Add(this.checkUpperFieldChar);
            this.Controls.Add(this.btnAdvOpt);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtOutputNamespace);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnGenEntity);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.txtPrefix);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.selectAllView);
            this.Controls.Add(this.outputLanguage);
            this.Controls.Add(this.btnGen);
            this.Controls.Add(this.txtConnStr);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.views);
            this.Controls.Add(this.selectAll);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tables);
            this.Controls.Add(this.label7);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "EntityDesign";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据库实体生成工具 v1.1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox tables;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox selectAll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox views;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox output;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtConnStr;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.RadioButton radioAccess;
        private System.Windows.Forms.ComboBox outputLanguage;
        private System.Windows.Forms.CheckBox selectAllView;
        private System.Windows.Forms.Button btnGen;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnGenEntity;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtOutputNamespace;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnAdvOpt;
        private System.Windows.Forms.CheckBox checkUpperFieldChar;
        private System.Windows.Forms.RadioButton radioFirst;
        private System.Windows.Forms.RadioButton radioAll;
        private System.Windows.Forms.RadioButton radioSql2005;
        private System.Windows.Forms.RadioButton radioSql;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkUpperTableChar;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton radioSQLite;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiClear;
        private System.Windows.Forms.ToolStripMenuItem tsmiPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtExceptPrefix;
        private System.Windows.Forms.CheckBox chkNoMapping;
        private System.Windows.Forms.CheckBox chkCreateAssembly;
        private System.Windows.Forms.RadioButton radioOracle;
        private System.Windows.Forms.RadioButton radioMySQL;
        private System.Windows.Forms.CheckBox chkCreateEntity;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

