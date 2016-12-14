using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using MySoft.Data;
using MySoft.Data.MsAccess;
using MySoft.Data.Oracle;
using MySoft.Data.SqlServer;

namespace MySoft.Tools.EntityDesign
{
    public partial class EntityDesign : Form
    {
        private Dictionary<string, string> dictFiles = new Dictionary<string, string>();
        private Dictionary<string, string> dictTypeFiles = new Dictionary<string, string>();
        public EntityDesign()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (txtConnStr.Text.Trim().Length == 0)
            {
                MessageBox.Show("连接字符串不能为空!");
                return;
            }

            if (btnConnect.Text == "断开服务器连接")
            {
                EnableGenEntity(false);
                return;
            }

            RefreshConnectionStringAutoComplete();

            DataSet dsTables = null;
            DataSet dsViews = null;

            try
            {
                if (radioSql.Checked || radioSql2005.Checked)
                {
                    DbProvider dbProvider = new SqlServerProvider(txtConnStr.Text);
                    DbSession.SetDefault(dbProvider);

                    if (radioSql2005.Checked)
                    {
                        dsTables = DbSession.Default.FromSql("select [name] from sysobjects where xtype = 'U' and [name] <> 'sysdiagrams' order by [name]").ToDataSet();
                    }
                    else
                    {
                        dsTables = DbSession.Default.FromSql("select [name] from sysobjects where xtype = 'U' and status > 0 order by [name]").ToDataSet();
                    }
                    foreach (DataRow row in dsTables.Tables[0].Rows)
                    {
                        tables.Items.Add(row["Name"].ToString());
                    }

                    if (radioSql2005.Checked)
                    {
                        dsViews = DbSession.Default.FromSql("select [name] from sysobjects where xtype = 'V' order by [name]").ToDataSet();
                    }
                    else
                    {
                        dsViews = DbSession.Default.FromSql("select [name] from sysobjects where xtype = 'V' and status > 0 order by [name]").ToDataSet();
                    }
                    foreach (DataRow row in dsViews.Tables[0].Rows)
                    {
                        views.Items.Add(row["Name"].ToString());
                    }
                }
                else if (radioOracle.Checked)
                {
                    DbProvider dbProvider = new OracleProvider(txtConnStr.Text);
                    DbSession.SetDefault(dbProvider);

                    dsTables = DbSession.Default.FromSql("select table_name Name from user_tables").ToDataSet();
                    foreach (DataRow row in dsTables.Tables[0].Rows)
                    {
                        tables.Items.Add(row["Name"].ToString());
                    }

                    dsViews = DbSession.Default.FromSql("select view_name Name from user_views").ToDataSet();
                    foreach (DataRow row in dsViews.Tables[0].Rows)
                    {
                        views.Items.Add(row["Name"].ToString());
                    }
                }
                else if (radioAccess.Checked)
                {
                    DbProvider dbProvider = new MsAccessProvider(txtConnStr.Text);
                    DbSession.SetDefault(dbProvider);

                    string connStr = txtConnStr.Text;
                    ADODB.ConnectionClass conn = new ADODB.ConnectionClass();
                    conn.Open(connStr);

                    ADODB.Recordset rsTables = conn.GetType().InvokeMember("OpenSchema", BindingFlags.InvokeMethod, null, conn, new object[] { ADODB.SchemaEnum.adSchemaTables }) as ADODB.Recordset;
                    ADODB.Recordset rsViews = conn.GetType().InvokeMember("OpenSchema", BindingFlags.InvokeMethod, null, conn, new object[] { ADODB.SchemaEnum.adSchemaViews }) as ADODB.Recordset;

                    while (!rsViews.EOF)
                    {
                        if (rsTables.Fields["TABLE_TYPE"].Value.ToString() == "VIEW")
                        {
                            views.Items.Add(rsViews.Fields["TABLE_NAME"].Value.ToString());
                        }
                        rsViews.MoveNext();
                    }

                    while (!rsTables.EOF)
                    {
                        if (rsTables.Fields["TABLE_TYPE"].Value.ToString() == "TABLE")
                        {
                            tables.Items.Add(rsTables.Fields["TABLE_NAME"].Value.ToString());
                        }
                        rsTables.MoveNext();
                    }

                    rsTables.Close();
                    rsViews.Close();

                    conn.Close();
                }
                else if (radioSQLite.Checked)
                {
                    DbProvider dbProvider = DbProviderFactory.CreateDbProvider(DbProviderType.SQLite, txtConnStr.Text);
                    DbSession.SetDefault(dbProvider);

                    System.Data.Common.DbConnection conn = DbSession.Default.CreateConnection();
                    conn.Open();
                    DataTable table1 = conn.GetSchema("TABLES");
                    DataTable table2 = conn.GetSchema("VIEWS");
                    conn.Close();
                    foreach (DataRow row in table1.Rows)
                    {
                        if (row["TABLE_TYPE"].ToString().ToUpper() == "TABLE")
                        {
                            tables.Items.Add(row["TABLE_NAME"].ToString());
                        }
                    }
                    foreach (DataRow row in table2.Rows)
                    {
                        views.Items.Add(row["TABLE_NAME"].ToString());
                    }
                }
                else if (radioMySQL.Checked)
                {
                    DbProvider dbProvider = DbProviderFactory.CreateDbProvider(DbProviderType.MySql, txtConnStr.Text);
                    DbSession.SetDefault(dbProvider);

                    System.Data.Common.DbConnection conn = DbSession.Default.CreateConnection();
                    conn.Open();
                    DataTable table1 = conn.GetSchema("TABLES");
                    DataTable table2 = conn.GetSchema("VIEWS");
                    conn.Close();
                    foreach (DataRow row in table1.Rows)
                    {
                        if (row["TABLE_TYPE"].ToString() == "BASE TABLE")
                        {
                            tables.Items.Add(row["TABLE_NAME"].ToString());
                        }
                    }
                    foreach (DataRow row in table2.Rows)
                    {
                        views.Items.Add(row["TABLE_NAME"].ToString());
                    }
                }


                EnableGenEntity(true);
            }
            catch (Exception ex)
            {
                EnableGenEntity(false);
                MessageBox.Show(ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private AutoCompleteStringCollection connStrs = new AutoCompleteStringCollection();
        private const string CONN_STR_HIS_File = "ConnectionStringsHistory.txt";

        public void RefreshConnectionStringAutoComplete()
        {
            if (!string.IsNullOrEmpty(txtConnStr.Text))
            {
                if (!connStrs.Contains(txtConnStr.Text))
                {
                    connStrs.Add(txtConnStr.Text);
                }
            }
        }

        private void SaveConnectionStringAutoComplete()
        {
            StreamWriter sw = new StreamWriter(CONN_STR_HIS_File);

            foreach (string line in connStrs)
            {
                sw.WriteLine(line);
            }

            sw.Close();
        }

        private void LoadConnectionStringAutoComplete()
        {
            if (File.Exists(CONN_STR_HIS_File))
            {
                connStrs.Clear();

                StreamReader sr = new StreamReader(CONN_STR_HIS_File);
                while (!sr.EndOfStream)
                {
                    connStrs.Add(sr.ReadLine().Trim());
                }
                sr.Close();
            }

            txtConnStr.AutoCompleteCustomSource = connStrs;
        }

        private void EnableGenEntity(bool enabled)
        {
            if (enabled)
            {
                btnGen.Enabled = true;
                chkCreateAssembly.Enabled = true;
                txtConnStr.Enabled = false;
                btnConnect.Text = "断开服务器连接";

                radioAccess.Enabled = false;
                radioSql.Enabled = false;
                radioSql2005.Enabled = false;
                radioSQLite.Enabled = false;
                radioOracle.Enabled = false;
                radioMySQL.Enabled = false;
            }
            else
            {
                btnGen.Enabled = false;
                chkCreateAssembly.Enabled = false;
                txtConnStr.Enabled = true;
                btnConnect.Text = "连接服务器";
                selectAll.Checked = false;
                tables.Items.Clear();
                views.Items.Clear();
                output.Text = "";

                radioAccess.Enabled = true;
                radioSql.Enabled = true;
                radioSql2005.Enabled = true;
                radioSQLite.Enabled = true;
                radioOracle.Enabled = true;
                radioMySQL.Enabled = true;
            }
        }

        private void selectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (selectAll.Checked)
            {
                for (int i = 0; i < tables.Items.Count; i++)
                {
                    tables.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < tables.Items.Count; i++)
                {
                    tables.SetItemChecked(i, false);
                }
            }
        }

        private void selectAllView_CheckedChanged(object sender, EventArgs e)
        {
            if (selectAllView.Checked)
            {
                for (int i = 0; i < views.Items.Count; i++)
                {
                    views.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < views.Items.Count; i++)
                {
                    views.SetItemChecked(i, false);
                }
            }
        }

        private string GenType(string typeStr)
        {
            if (typeStr == typeof(string).ToString())
            {
                return "string";
            }
            else if (typeStr == typeof(int).ToString())
            {
                return "int";
            }
            else if (typeStr == typeof(long).ToString())
            {
                return "long";
            }
            else if (typeStr == typeof(short).ToString())
            {
                return "short";
            }
            else if (typeStr == typeof(byte).ToString())
            {
                return "byte";
            }
            else if (typeStr == typeof(byte[]).ToString())
            {
                return "byte[]";
            }
            else if (typeStr == typeof(bool).ToString())
            {
                return "bool";
            }
            else if (typeStr == typeof(decimal).ToString())
            {
                return "decimal";
            }
            else if (typeStr == typeof(char).ToString())
            {
                return "char";
            }
            else if (typeStr == typeof(sbyte).ToString())
            {
                return "sbyte";
            }
            else if (typeStr == typeof(float).ToString())
            {
                return "float";
            }
            else if (typeStr == typeof(double).ToString())
            {
                return "double";
            }
            else if (typeStr == typeof(object).ToString())
            {
                return "object";
            }
            else if (typeStr == typeof(Guid).ToString())
            {
                return "Guid";
            }
            else if (typeStr == typeof(DateTime).ToString())
            {
                return "DateTime";
            }
            else
            {
                return typeStr;
            }
        }

        private string GenTypeVB(string typeStr)
        {
            if (typeStr == typeof(string).ToString())
            {
                return "String";
            }
            else if (typeStr == typeof(int).ToString())
            {
                return "Integer";
            }
            else if (typeStr == typeof(uint).ToString())
            {
                return "UInteger";
            }
            else if (typeStr == typeof(long).ToString())
            {
                return "Long";
            }
            else if (typeStr == typeof(ulong).ToString())
            {
                return "ULong";
            }
            else if (typeStr == typeof(short).ToString())
            {
                return "Short";
            }
            else if (typeStr == typeof(ushort).ToString())
            {
                return "UShort";
            }
            else if (typeStr == typeof(byte).ToString())
            {
                return "Byte";
            }
            else if (typeStr == typeof(byte[]).ToString())
            {
                return "Byte()";
            }
            else if (typeStr == typeof(bool).ToString())
            {
                return "Boolean";
            }
            else if (typeStr == typeof(decimal).ToString())
            {
                return "Decimal";
            }
            else if (typeStr == typeof(char).ToString())
            {
                return "Char";
            }
            else if (typeStr == typeof(sbyte).ToString())
            {
                return "SByte";
            }
            else if (typeStr == typeof(Single).ToString())
            {
                return "Single";
            }
            else if (typeStr == typeof(double).ToString())
            {
                return "Double";
            }
            else if (typeStr == typeof(object).ToString())
            {
                return "Object";
            }
            else if (typeStr == typeof(Guid).ToString())
            {
                return "Guid";
            }
            else if (typeStr == typeof(DateTime).ToString())
            {
                return "Date";
            }
            else
            {
                return typeStr.Replace("[", "(").Replace("]", ")");
            }
        }

        /// <summary>
        /// 获取描述信息
        /// </summary>
        /// <returns></returns>
        private string GetDescription(string tableName, string columnName)
        {
            string sql = string.Empty;
            if (radioSql.Checked)
            {
                // " where   d.name='要查询的表'   --如果只查询指定表,加上此条件"
                sql = "select description=g.value from syscolumns a inner join sysobjects d "
                     + " on a.id=d.id and d.xtype='u' and d.name<>'dtproperties'"
                     + " left join sysproperties g on a.id=g.id and a.colid=g.smallid"
                     + " where d.name = '" + tableName + "' and a.name = '" + columnName + "'";
            }
            else if (radioSql2005.Checked)
            {
                sql = "select description=value from sys.tables left join sys.syscolumns "
                    + " on object_id = id left join sys.extended_properties "
                    + " on id = major_id and colid = minor_id "
                    + " where sys.tables.name = '" + tableName + "' and sys.syscolumns.name = '" + columnName + "'";
            }
            else if (radioSQLite.Checked)
            {
                System.Data.Common.DbConnection conn = DbSession.Default.CreateConnection();
                conn.Open();
                DataTable table = conn.GetSchema("COLUMNS");
                conn.Close();
                foreach (DataRow row in table.Rows)
                {
                    if (row["TABLE_NAME"].ToString() == tableName && row["COLUMN_NAME"].ToString() == columnName)
                    {
                        return row["DESCRIPTION"].ToString();
                    }
                }
            }
            else
            {
                return null;
            }

            try
            {
                object retVal = DbSession.Default.FromSql(sql).ToScalar();
                if (retVal == DBNull.Value || retVal == null)
                    return null;
                return retVal.ToString();
            }
            catch
            {
                return null;
            }
        }

        private bool IsColumnPrimaryKey(string name, string column)
        {
            if (radioSql.Checked || radioSql2005.Checked)
            {
                int tableid = Convert.ToInt32(DbSession.Default.FromSql("select id from sysobjects where [name] = '" + name + "'").ToScalar());
                DataSet ds = DbSession.Default.FromSql("select a.name FROM syscolumns a inner join sysobjects d on a.id=d.id and d.xtype='U' "
                + " and d.name<>'dtproperties' where (SELECT count(*) FROM sysobjects WHERE (name in (SELECT name FROM sysindexes WHERE (id = a.id) AND "
                + " (indid in (SELECT indid FROM sysindexkeys WHERE (id = a.id) AND (colid in (SELECT colid FROM syscolumns WHERE (id = a.id) "
                + " AND (name = a.name))))))) AND (xtype = 'PK'))>0 and d.id = " + tableid).ToDataSet();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == column)
                    {
                        return true;
                    }
                }
            }
            else if (radioOracle.Checked)
            {
                DataSet ds = DbSession.Default.FromSql("select col.column_name from user_constraints con,user_cons_columns col where con.constraint_name = "
                + " col.constraint_name and con.constraint_type='P' and upper(col.table_name) = '" + name.ToUpper() + "'").ToDataSet();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == column)
                    {
                        return true;
                    }
                }
            }
            else if (radioAccess.Checked)
            {
                string connStr = txtConnStr.Text;
                ADODB.ConnectionClass conn = new ADODB.ConnectionClass();
                conn.Open(connStr);

                ADODB.Recordset rs = conn.GetType().InvokeMember("OpenSchema", BindingFlags.InvokeMethod, null, conn, new object[] { ADODB.SchemaEnum.adSchemaPrimaryKeys }) as ADODB.Recordset;
                rs.Filter = "TABLE_NAME='" + name + "'";

                while (!rs.EOF)
                {
                    if (rs.Fields["COLUMN_NAME"].Value.ToString() == column)
                    {
                        rs.Close();
                        conn.Close();
                        return true;
                    }

                    rs.MoveNext();
                }
            }
            else if (radioSQLite.Checked)
            {
                System.Data.Common.DbConnection conn = DbSession.Default.CreateConnection();
                conn.Open();
                DataTable table = conn.GetSchema("COLUMNS");
                conn.Close();
                foreach (DataRow row in table.Rows)
                {
                    if (row["TABLE_NAME"].ToString() == name && row["COLUMN_NAME"].ToString() == column)
                    {
                        return Convert.ToBoolean(row["PRIMARY_KEY"].ToString());
                    }
                }
            }
            else if (radioMySQL.Checked)
            {
                System.Data.Common.DbConnection conn = DbSession.Default.CreateConnection();
                conn.Open();
                DataTable table = conn.GetSchema("COLUMNS");
                conn.Close();
                foreach (DataRow row in table.Rows)
                {
                    if (row["TABLE_NAME"].ToString() == name && row["COLUMN_NAME"].ToString() == column)
                    {
                        return row["COLUMN_KEY"].ToString().ToUpper() == "PRI";
                    }
                }
            }

            return false;
        }

        private bool IsColumnReadOnly(string name, string column)
        {
            if (radioSql.Checked || radioSql2005.Checked)
            {
                int tableid = Convert.ToInt32(DbSession.Default.FromSql("select id from sysobjects where [name] = '" + name + "'").ToScalar());
                byte status = Convert.ToByte(DbSession.Default.FromSql("select status from syscolumns where [name] = '" + column + "' and id =" + tableid).ToScalar());
                return status == 128;
            }
            else if (radioAccess.Checked)
            {
                string connStr = txtConnStr.Text;
                ADODB.ConnectionClass conn = new ADODB.ConnectionClass();
                conn.Open(connStr);

                ADODB.Recordset rs = conn.GetType().InvokeMember("OpenSchema", BindingFlags.InvokeMethod, null, conn, new object[] { ADODB.SchemaEnum.adSchemaColumns }) as ADODB.Recordset;
                rs.Filter = "TABLE_NAME='" + name + "'";

                while (!rs.EOF)
                {
                    if (rs.Fields["COLUMN_NAME"].Value.ToString() == column && ((int)rs.Fields["DATA_TYPE"].Value) == 3 && Convert.ToByte(rs.Fields["COLUMN_FLAGS"].Value) == 90)
                    {
                        rs.Close();
                        conn.Close();
                        return true;
                    }

                    rs.MoveNext();
                }
            }
            else if (radioSQLite.Checked)
            {
                System.Data.Common.DbConnection conn = DbSession.Default.CreateConnection();
                conn.Open();
                DataTable table = conn.GetSchema("COLUMNS");
                conn.Close();
                foreach (DataRow row in table.Rows)
                {
                    if (row["TABLE_NAME"].ToString() == name && row["COLUMN_NAME"].ToString() == column && row["DATA_TYPE"].ToString() == "INTEGER")
                    {
                        return true;
                    }
                }
            }
            else if (radioMySQL.Checked)
            {
                System.Data.Common.DbConnection conn = DbSession.Default.CreateConnection();
                conn.Open();
                DataTable table = conn.GetSchema("COLUMNS");
                conn.Close();
                foreach (DataRow row in table.Rows)
                {
                    if (row["TABLE_NAME"].ToString() == name && row["COLUMN_NAME"].ToString() == column && row["EXTRA"].ToString().ToUpper() == "AUTO_INCREMENT")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsColumnNullable(string name, string column)
        {
            if (radioSql.Checked || radioSql2005.Checked)
            {
                int tableid = Convert.ToInt32(DbSession.Default.FromSql("select id from sysobjects where [name] = '" + name + "'").ToScalar());
                int isnullable = Convert.ToInt32(DbSession.Default.FromSql("select isnullable from syscolumns where [name] = '" + column + "' and id = " + tableid).ToScalar());
                return isnullable == 1;
            }
            else if (radioOracle.Checked)
            {
                string isnullable = DbSession.Default.FromSql("select NULLABLE,COLUMN_NAME,DATA_TYPE,DATA_PRECISION,DATA_SCALE from user_tab_columns "
                + " where upper(table_name) ='" + name.ToUpper() + "' and upper(column_name) = '" + column.ToUpper() + "'").ToScalar<string>();
                return isnullable.ToUpper() == "Y";
            }
            else if (radioSQLite.Checked)
            {
                System.Data.Common.DbConnection conn = DbSession.Default.CreateConnection();
                conn.Open();
                DataTable table = conn.GetSchema("COLUMNS");
                conn.Close();
                foreach (DataRow row in table.Rows)
                {
                    if (row["TABLE_NAME"].ToString() == name && row["COLUMN_NAME"].ToString() == column)
                    {
                        return Convert.ToBoolean(row["IS_NULLABLE"].ToString());
                    }
                }
            }
            else if (radioMySQL.Checked)
            {
                System.Data.Common.DbConnection conn = DbSession.Default.CreateConnection();
                conn.Open();
                DataTable table = conn.GetSchema("COLUMNS");
                conn.Close();
                foreach (DataRow row in table.Rows)
                {
                    if (row["TABLE_NAME"].ToString() == name && row["COLUMN_NAME"].ToString() == column)
                    {
                        return row["IS_NULLABLE"].ToString().ToUpper() == "YES";
                    }
                }
            }
            else if (radioAccess.Checked)
            {
                string connStr = txtConnStr.Text;
                ADODB.ConnectionClass conn = new ADODB.ConnectionClass();
                conn.Open(connStr);

                ADODB.Recordset rs = conn.GetType().InvokeMember("OpenSchema", BindingFlags.InvokeMethod, null, conn, new object[] { ADODB.SchemaEnum.adSchemaColumns }) as ADODB.Recordset;
                rs.Filter = "TABLE_NAME='" + name + "'";

                while (!rs.EOF)
                {
                    if (rs.Fields["COLUMN_NAME"].Value.ToString() == column)
                    {
                        bool result = Convert.ToBoolean(rs.Fields["IS_NULLABLE"].Value);
                        rs.Close();
                        conn.Close();
                        return result;
                    }

                    rs.MoveNext();
                }
            }

            return false;
        }

        private static string ParseMappingName(string name)
        {
            return name.Trim().Replace(" ", "_").Replace(".", "__");
        }

        private void btnGen_Click(object sender, EventArgs e)
        {
            string 服务类代码 = string.Empty;
            string 服务类命名空间代码 = string.Empty;

            int selectCount = tables.CheckedItems.Count + views.CheckedItems.Count;
            if (selectCount == 0)
            {
                MessageBox.Show("请选择要生成的表或视图！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnGen.Enabled = false;
            chkCreateAssembly.Enabled = false;

            string fileText = string.Empty;
            output.Text = "";
            richTextBox1.Text = "";
            dictFiles.Clear();

            foreach (string table in tables.CheckedItems)
            {
                output.SelectionColor = Color.Black;
                output.SelectionFont = new Font(new FontFamily("宋体"), 12, FontStyle.Regular, GraphicsUnit.Pixel);
                output.Select(output.TextLength, 0);
                fileText = GenEntity(table, false, chkCreateEntity.Checked);

                string tableName = ConvertCharToUpperOrLower(ParseMappingName(table), checkUpperTableChar.Checked);
                dictFiles.Add(tableName, fileText);

                if (!chkCreateAssembly.Checked || chkCreateEntity.Checked)
                {
                    output.AppendText(fileText + "\r\n");
                    output.Select(output.TextLength, 0);
                    output.ScrollToCaret();
                }

                服务类代码 += CreateServiceClassCode(table);
            }

            foreach (string view in views.CheckedItems)
            {
                output.SelectionColor = Color.Black;
                output.SelectionFont = new Font(new FontFamily("宋体"), 12, FontStyle.Regular, GraphicsUnit.Pixel);
                output.Select(output.TextLength, 0);
                fileText = GenEntity(view, true, chkCreateEntity.Checked);

                string tableName = ConvertCharToUpperOrLower(ParseMappingName(view), checkUpperTableChar.Checked);
                dictFiles.Add(tableName, fileText);

                if (!chkCreateAssembly.Checked || chkCreateEntity.Checked)
                {
                    output.AppendText(fileText + "\r\n");
                    output.Select(output.TextLength, 0);
                    output.ScrollToCaret();
                }

                服务类代码 += CreateServiceClassCode(view);
            }

            output.Text = output.Text.TrimEnd('\r', '\n');
            richTextBox1.Text = richTextBox1.Text.TrimEnd('\r', '\n');

            if (chkCreateAssembly.Checked && !chkCreateEntity.Checked)
            {
                CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();

                //设置编译参数。   
                CompilerParameters paras = new CompilerParameters();
                paras.GenerateExecutable = false;
                paras.GenerateInMemory = true;

                //添加引用   
                paras.ReferencedAssemblies.Add("System.dll");
                paras.ReferencedAssemblies.Add("MySoft.Data.dll");

                StringBuilder txtBody = new StringBuilder();
                txtBody.AppendLine("using System;");
                txtBody.AppendLine("using System.Collections.Generic;");
                txtBody.AppendLine("using System.Text;");
                txtBody.AppendLine("using MySoft.Data.Design;");
                txtBody.AppendLine("");
                txtBody.AppendLine(string.Format("namespace {0}", txtOutputNamespace.Text));
                txtBody.AppendLine("{");

                foreach (string value in dictFiles.Values)
                {
                    txtBody.Append(value);
                    txtBody.Append(Environment.NewLine);
                }

                txtBody.AppendLine("}");

                服务类命名空间代码 = CreateServiceCode(textBox1.Text, 服务类代码);
                richTextBox1.Text = 服务类命名空间代码;

                //编译代码。   
                CompilerResults result = provider.CompileAssemblyFromSource(paras, txtBody.ToString());

                if (result.Errors.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int index = 0; index < result.Errors.Count; index++)
                    {
                        var err = result.Errors[index];
                        sb.AppendFormat("{0}、({1})-->", ++index, err.ErrorNumber);
                        sb.AppendLine(err.ErrorText);
                    }

                    MessageBox.Show("生成实体时出现了" + result.Errors.Count + "个错误！\r\n" + sb.ToString(), "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Assembly ass = result.CompiledAssembly;
                output.Text = string.Empty;
                Application.DoEvents();

                output.Text = new CodeGenHelper(txtOutputNamespace.Text, advForm).GenEntitiesEx(ass, outputLanguage.SelectedIndex);
                dictFiles.Clear();

                Type[] types = ass.GetTypes();
                if (types != null && types.Length > 0)
                {
                    List<Type> typelist = new List<Type>(types);
                    typelist.RemoveAll(type =>
                    {
                        return !type.IsInterface;
                    });

                    string ns = typelist[0].Namespace;
                    string[] files = output.Text.Split(new string[] { "namespace " + ns, "Namespace " + ns }, StringSplitOptions.RemoveEmptyEntries);
                    IList<string> filelist = new List<string>(files);
                    filelist.RemoveAt(0);

                    dictTypeFiles = new Dictionary<string, string>();
                    for (int index = 0; index < typelist.Count; index++)
                    {
                        if (outputLanguage.SelectedIndex == 0)
                            dictTypeFiles[typelist[index].Name] = "namespace " + ns + filelist[index];
                        else
                            dictTypeFiles[typelist[index].Name] = "Namespace " + ns + filelist[index];
                    }
                }

                btnGen.Enabled = true;
                chkCreateAssembly.Enabled = true;

                MessageBox.Show("生成实体成功！", "生成实体", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                btnGen.Enabled = true;
                chkCreateAssembly.Enabled = true;

                MessageBox.Show("生成实体接口成功！", "生成实体接口", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string FirstCharToUpper(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else if (str.Length > 1)
            {
                return str.Substring(0, 1).ToUpper() + str.Substring(1);
            }
            else
            {
                return str.ToUpper();
            }
        }

        private string ConvertCharToUpperOrLower(string str, bool ischecked)
        {

            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            if (txtPrefix.Text.Trim().Length == 0)
            {
                if (ischecked)
                {
                    if (radioAll.Checked)
                    {
                        return str.ToUpper();
                    }
                    else
                    {
                        return FirstCharToUpper(str);
                    }
                }
                return str;
            }
            else
            {
                string convertStr = "";
                string prefix = txtPrefix.Text;
                int index = str.IndexOf(prefix);
                while (index >= 0)
                {
                    if (convertStr == "")
                    {
                        convertStr = str.Substring(0, index + prefix.Length).ToLower();
                    }
                    else
                    {
                        if (ischecked)
                        {
                            convertStr += FirstCharToUpper(str.Substring(0, index + prefix.Length));
                        }
                        else
                        {
                            convertStr += str.Substring(0, index + prefix.Length);
                        }
                    }
                    str = str.Substring(index + prefix.Length);
                    index = str.IndexOf(prefix);
                }

                if (ischecked)
                {
                    if (radioAll.Checked)
                    {
                        return convertStr + str.ToUpper();
                    }
                    else
                    {
                        return convertStr + FirstCharToUpper(str);
                    }
                }
                return convertStr + str;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            outputLanguage.SelectedIndex = 0;

            LoadConnectionStringAutoComplete();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveConnectionStringAutoComplete();
        }

        private string GenEntity(string name, bool isView, bool isEntity)
        {
            DataSet ds = DbSession.Default.FromSql("select * from __[" + name + "]__ where 1 = 2").ToDataSet();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int index = 0;
            int columns = ds.Tables[0].Columns.Count;

            string tableName = ConvertCharToUpperOrLower(ParseMappingName(name), checkUpperTableChar.Checked);

            if (outputLanguage.SelectedIndex == 0 || (chkCreateAssembly.Checked && !chkCreateEntity.Checked))
            {
                if (isEntity)
                {
                    sb.Append("\t[Serializable]\r\n");
                    sb.Append(string.Format("\tpublic class {0}\r\n", tableName));
                    sb.Append("\t{\r\n");
                }
                else
                {
                    if (isView)
                    {
                        sb.Append(string.Format("\t[ReadOnly]\r\n"));
                    }

                    if (txtExceptPrefix.Text.Trim() != string.Empty && tableName.ToLower().StartsWith(txtExceptPrefix.Text.ToLower().Trim()))
                    {
                        if (!chkNoMapping.Checked)
                            sb.Append(string.Format("\t[Mapping(\"" + name + "\")]\r\n"));
                        sb.Append(string.Format("\tpublic interface {0} : IEntity\r\n", tableName.Substring(tableName.ToLower().IndexOf(txtExceptPrefix.Text.ToLower().Trim()) + txtExceptPrefix.Text.ToLower().Trim().Length)));
                    }
                    else
                    {
                        if (name.Contains(" ") || name.Contains("."))
                            sb.Append(string.Format("\t[Mapping(\"" + name + "\")]\r\n"));
                        sb.Append(string.Format("\tpublic interface {0} : IEntity\r\n", tableName));
                    }
                    sb.Append("\t{\r\n");
                }

                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    index++;
                    string description = GetDescription(name, column.ColumnName);
                    if (description != null)
                        description = description.Replace("\"", "\\\"");

                    if (isEntity)
                    {
                        if (!string.IsNullOrEmpty(description))
                        {
                            sb.Append("\t\t/// <summary>\r\n");
                            sb.Append("\t\t/// " + description + "\r\n");
                            sb.Append("\t\t/// </summary>\r\n");
                        }

                        string fieldName = ConvertCharToUpperOrLower(ParseMappingName(column.ColumnName), checkUpperFieldChar.Checked);
                        sb.Append(string.Format("\t\tpublic {0}{2} {1} ", GenType(column.DataType.ToString()), fieldName, (column.DataType.IsValueType && IsColumnNullable(name, column.ColumnName) ? "?" : "")));
                        sb.Append("{ get; set; }\r\n");
                    }
                    else
                    {
                        if (IsColumnPrimaryKey(name, column.ColumnName))
                        {
                            sb.Append(string.Format("\t\t[PrimaryKey]\r\n"));
                        }

                        if (!string.IsNullOrEmpty(description))
                        {
                            sb.Append(string.Format("\t\t[Description(\"" + description + "\")]\r\n"));
                        }

                        string fieldName = ConvertCharToUpperOrLower(ParseMappingName(column.ColumnName), checkUpperFieldChar.Checked);
                        if (string.Compare(GetTableNameRemovePrefix(tableName), fieldName, true) == 0)
                        {
                            sb.Append(string.Format("\t\t[Mapping(\"" + column.ColumnName + "\")]\r\n"));

                            if (string.Compare(GetTableNameRemovePrefix(tableName), fieldName.ToUpper()) == 0)
                            {
                                fieldName += "_New";
                            }
                            else
                            {
                                fieldName = fieldName.ToUpper();
                            }
                        }
                        else if (column.ColumnName.Contains(" ") || column.ColumnName.Contains("."))
                        {
                            sb.Append(string.Format("\t\t[Mapping(\"" + column.ColumnName + "\")]\r\n"));
                        }

                        sb.Append(string.Format("\t\t{0}{2} {1} ", GenType(column.DataType.ToString()), fieldName, (column.DataType.IsValueType && IsColumnNullable(name, column.ColumnName) ? "?" : "")));

                        if (isView || IsColumnReadOnly(name, column.ColumnName))
                        {
                            sb.Append("{ get; }\r\n");
                        }
                        else
                        {
                            sb.Append("{ get; set; }\r\n");
                        }
                    }
                    Application.DoEvents();
                }

                sb.Append("\t}\r\n");
            }
            else if (outputLanguage.SelectedIndex == 1)
            {
                if (isEntity)
                {
                    sb.Append("\t<[Serializable]()> _\r\n");
                    sb.Append(string.Format("\tPublic Class {0}\r\n", tableName));
                }
                else
                {
                    if (isView)
                    {
                        sb.Append(string.Format("\t<[ReadOnly]()> _\r\n"));
                    }

                    if (CheckTableName(tableName))
                    {
                        if (!chkNoMapping.Checked)
                            sb.Append(string.Format("\t<[Mapping](\"" + name + "\")> _\r\n"));
                        sb.Append(string.Format("\tPublic Interface {0}\r\n\t\tInherits IEntity\r\n", GetTableNameRemovePrefix(tableName)));
                    }
                    else
                    {
                        if (name.Contains(" ") || name.Contains("."))
                            sb.Append(string.Format("\t<[Mapping](\"" + name + "\")> _\r\n"));
                        sb.Append(string.Format("\tPublic Interface {0}\r\n\t\tInherits IEntity\r\n", tableName));
                    }
                }

                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    index++;

                    string description = GetDescription(name, column.ColumnName);
                    if (isEntity)
                    {
                        if (!string.IsNullOrEmpty(description))
                        {
                            sb.Append("\t\t''' <summary>\r\n");
                            sb.Append("\t\t''' " + description + "\r\n");
                            sb.Append("\t\t''' </summary>\r\n");
                        }

                        string fieldName = ConvertCharToUpperOrLower(ParseMappingName(column.ColumnName), checkUpperFieldChar.Checked);
                        if (column.DataType.IsValueType && IsColumnNullable(name, column.ColumnName))
                        {
                            sb.Append(string.Format("\t\tPublic Property {1}() As Nullable(Of {0})\r\n", GenTypeVB(column.DataType.ToString()), fieldName));
                        }
                        else
                        {
                            sb.Append(string.Format("\t\tPublic Property {1}() As {0}\r\n", GenTypeVB(column.DataType.ToString()), fieldName));
                        }
                    }
                    else
                    {
                        if (IsColumnPrimaryKey(name, column.ColumnName))
                        {
                            sb.Append(string.Format("\t\t<[PrimaryKey]()> _\r\n"));
                        }

                        if (!string.IsNullOrEmpty(description))
                        {
                            sb.Append(string.Format("\t\t<[Description](\"" + description + "\")> _\r\n"));
                        }

                        sb.Append("\t\t");

                        if (isView || IsColumnReadOnly(name, column.ColumnName))
                        {
                            sb.Append("ReadOnly ");
                        }

                        string fieldName = ConvertCharToUpperOrLower(ParseMappingName(column.ColumnName), checkUpperFieldChar.Checked);
                        if (string.Compare(GetTableNameRemovePrefix(tableName), fieldName, true) == 0)
                        {
                            sb.Append(string.Format("<[Mapping](\"" + column.ColumnName + "\")> _\r\n"));
                            sb.Append("\t\t");

                            if (string.Compare(GetTableNameRemovePrefix(tableName), fieldName.ToUpper()) == 0)
                            {
                                fieldName += "_New";
                            }
                            else
                            {
                                fieldName = fieldName.ToUpper();
                            }
                        }
                        else if (column.ColumnName.Contains(" ") || column.ColumnName.Contains("."))
                        {
                            sb.Append(string.Format("<[Mapping](\"" + column.ColumnName + "\")> _\r\n"));
                            sb.Append("\t\t");
                        }

                        if (column.DataType.IsValueType && IsColumnNullable(name, column.ColumnName))
                        {
                            sb.Append(string.Format("Property {1}() As Nullable(Of {0})\r\n", GenTypeVB(column.DataType.ToString()), fieldName));
                        }
                        else
                        {
                            sb.Append(string.Format("Property {1}() As {0}\r\n", GenTypeVB(column.DataType.ToString()), fieldName));
                        }
                    }
                }

                if (isEntity)
                    sb.Append("\tEnd Class\r\n");
                else
                    sb.Append("\tEnd Interface\r\n");
            }

            return sb.ToString();
        }

        private string GetTableNameRemovePrefix(string tableName)
        {
            if (CheckTableName(tableName))
                return tableName.Substring(tableName.ToLower().IndexOf(txtExceptPrefix.Text.ToLower().Trim()) + txtExceptPrefix.Text.ToLower().Trim().Length);
            else
                return tableName;
        }

        private bool CheckTableName(string tableName)
        {
            return txtExceptPrefix.Text.Trim() != string.Empty && tableName.ToLower().StartsWith(txtExceptPrefix.Text.ToLower().Trim());
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = openFileDialog1.FileName;
                btnGenEntity.Enabled = true;
                btnAdvOpt.Enabled = true;
                advForm.RefreshEntities(Assembly.LoadFrom(txtFileName.Text).GetTypes());
                txtOutputNamespace.Text = string.Concat(Assembly.LoadFrom(txtFileName.Text).FullName.Split(',')[0], ".Model");
                AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = System.IO.Path.GetDirectoryName(txtFileName.Text);
            }
        }

        private void btnGenEntity_Click(object sender, EventArgs e)
        {
            btnGenEntity.Enabled = false;

            Assembly ass = Assembly.LoadFrom(txtFileName.Text);
            output.Text = string.Empty;
            Application.DoEvents();
            output.Text = new CodeGenHelper(txtOutputNamespace.Text, advForm).GenEntitiesEx(ass, outputLanguage.SelectedIndex);

            dictFiles.Clear();

            Type[] types = ass.GetTypes();
            if (types != null)
            {
                List<Type> typelist = new List<Type>(types);
                typelist.RemoveAll(type =>
                {
                    return !type.IsInterface;
                });

                string[] files = output.Text.Split(new string[] { "namespace", "Namespace" }, StringSplitOptions.RemoveEmptyEntries);
                IList<string> filelist = new List<string>(files);
                filelist.RemoveAt(0);

                dictTypeFiles = new Dictionary<string, string>();
                int rowIndex = 0;
                for (int index = 0; index < typelist.Count; index++)
                {
                    if (advForm.IsEntityEnabled(typelist[index].Name))
                    {
                        if (outputLanguage.SelectedIndex == 0)
                            dictTypeFiles[typelist[index].Name] = "namespace" + filelist[rowIndex];
                        else
                            dictTypeFiles[typelist[index].Name] = "Namespace" + filelist[rowIndex];

                        rowIndex++;
                    }
                }
            }

            btnGenEntity.Enabled = true;
        }

        private AdvOptForm advForm = new AdvOptForm();

        private void btnAdvOpt_Click(object sender, EventArgs e)
        {
            advForm.ShowDialog();
        }

        private void tsmiSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                output.SelectAll();
            }
            catch { }
        }

        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetData(DataFormats.Text, output.SelectedText);
            }
            catch { }
        }

        private void tsmiClear_Click(object sender, EventArgs e)
        {
            output.Clear();
        }

        private void tsmiPaste_Click(object sender, EventArgs e)
        {
            try
            {
                string value = Clipboard.GetData(DataFormats.Text).ToString();
                output.AppendText(value);
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtFolder.Text))
            {
                MessageBox.Show("选择的路径不存在！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dictTypeFiles.Count == 0 && dictFiles.Count == 0)
            {
                MessageBox.Show("请先点击生成实体代码！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (outputLanguage.SelectedIndex == 0)
            {
                if (dictFiles.Count != 0)
                {
                    foreach (string txtKey in dictFiles.Keys)
                    {
                        string fileName = txtKey;
                        if (txtExceptPrefix.Text.Trim() != string.Empty && fileName.ToLower().StartsWith(txtExceptPrefix.Text.ToLower().Trim()))
                        {
                            fileName = fileName.Substring(fileName.ToLower().IndexOf(txtExceptPrefix.Text.ToLower().Trim()) + txtExceptPrefix.Text.ToLower().Trim().Length);
                        }
                        string filePath = string.Empty;
                        if (!chkCreateEntity.Checked)
                            filePath = string.Format("{0}\\I{1}.cs", txtFolder.Text, fileName);
                        else
                            filePath = string.Format("{0}\\{1}.cs", txtFolder.Text, fileName);

                        //如果文件存在则先删除
                        if (File.Exists(filePath))
                            File.Delete(filePath);

                        StringBuilder txtBody = new StringBuilder();
                        txtBody.AppendLine("using System;");
                        txtBody.AppendLine("using System.Collections.Generic;");
                        txtBody.AppendLine("using System.Text;");

                        if (!chkCreateEntity.Checked)
                        {
                            txtBody.AppendLine("using MySoft.Data.Design;");
                        }
                        txtBody.AppendLine("");
                        txtBody.AppendLine(string.Format("namespace {0}", txtOutputNamespace.Text));
                        txtBody.AppendLine("{");
                        txtBody.Append(dictFiles[txtKey]);
                        txtBody.AppendLine("}");

                        File.AppendAllText(filePath, txtBody.ToString());
                    }

                    if (chkCreateEntity.Checked)
                        MessageBox.Show("实体文件生成成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("实体接口文件生成成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    foreach (string txtKey in dictTypeFiles.Keys)
                    {
                        string filePath = string.Format("{0}\\{1}.cs", txtFolder.Text, txtKey);

                        //如果文件存在则先删除
                        if (File.Exists(filePath))
                            File.Delete(filePath);

                        File.AppendAllText(filePath, dictTypeFiles[txtKey]);

                        if (this.checkBox1.Checked)
                        {
                            string filePath1 = string.Format("{0}\\{1}Service.cs", txtFolder.Text, txtKey);

                            //如果文件存在则先删除
                            if (File.Exists(filePath1))
                                File.Delete(filePath1);

                            File.AppendAllText(filePath1, CreateServiceCode(this.textBox1.Text, CreateServiceClassCode(txtKey)));
                        }
                    }

                    MessageBox.Show("实体文件生成成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {
                if (dictFiles.Count != 0)
                {
                    foreach (string txtKey in dictFiles.Keys)
                    {
                        string fileName = txtKey;
                        if (txtExceptPrefix.Text.Trim() != string.Empty && fileName.ToLower().StartsWith(txtExceptPrefix.Text.ToLower().Trim()))
                        {
                            fileName = fileName.Substring(fileName.ToLower().IndexOf(txtExceptPrefix.Text.ToLower().Trim()) + txtExceptPrefix.Text.ToLower().Trim().Length);
                        }
                        string filePath = string.Format("{0}\\I{1}.vb", txtFolder.Text, fileName);

                        //如果文件存在则先删除
                        if (File.Exists(filePath))
                            File.Delete(filePath);

                        StringBuilder txtBody = new StringBuilder();
                        txtBody.AppendLine("Imports System");
                        txtBody.AppendLine("Imports System.Collections.Generic");
                        txtBody.AppendLine("Imports System.Text");
                        txtBody.AppendLine("Imports MySoft.Data.Design");
                        txtBody.AppendLine("");
                        txtBody.AppendLine(string.Format("Namespace {0}", txtOutputNamespace.Text));
                        txtBody.Append(dictFiles[txtKey]);
                        txtBody.AppendLine("End Namespace");

                        File.AppendAllText(filePath, txtBody.ToString());
                    }

                    MessageBox.Show("实体接口文件生成成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    foreach (string txtKey in dictTypeFiles.Keys)
                    {
                        string filePath = string.Format("{0}\\{1}.vb", txtFolder.Text, txtKey);

                        //如果文件存在则先删除
                        if (File.Exists(filePath))
                            File.Delete(filePath);

                        File.AppendAllText(filePath, dictTypeFiles[txtKey]);
                    }

                    MessageBox.Show("实体文件生成成功！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
        }

        private void chkCreateAssembly_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCreateAssembly.Checked)
            {
                btnGen.Text = "生成实体";
                chkCreateEntity.Visible = true;
                chkCreateEntity.Checked = false;
                this.checkBox1.Visible = true;
                this.checkBox1.Checked = true;
            }
            else
            {
                btnGen.Text = "生成实体接口";
                chkCreateEntity.Visible = false;
                chkCreateEntity.Checked = false;
                this.checkBox1.Visible = false;
                this.checkBox1.Checked = false;
            }
        }
        
        private string CreateServiceCode(string namespacename,string classcode)
        {
            StringBuilder CodeString = new StringBuilder();

            CodeString.AppendLine("using System;");
            CodeString.AppendLine("using System.Collections.Generic;");
            CodeString.AppendLine("using System.Linq;");
            CodeString.AppendLine("using System.Text;");
            CodeString.AppendLine("using MySoft.Data;");
            CodeString.AppendLine("using System.Data;");
            CodeString.AppendLine("using "+this.txtOutputNamespace.Text.Trim()+";");
            CodeString.AppendLine("");
            CodeString.AppendLine("namespace " + namespacename);
            CodeString.AppendLine("{");
            CodeString.AppendLine(classcode);
            CodeString.AppendLine("}");
            CodeString.AppendLine("");

            return CodeString.ToString();
        }

        private string CreateServiceClassCode(string classname)
        {
            StringBuilder CodeString = new StringBuilder();

            CodeString.AppendLine("    public class " + classname + "Service : BaseDao<" + classname + ">");
            CodeString.AppendLine("    {");
            CodeString.AppendLine("        #region \"单例\"");
            CodeString.AppendLine("        private static " + classname + "Service service;");
            CodeString.AppendLine("        public static " + classname + "Service Instance");
            CodeString.AppendLine("        {");
            CodeString.AppendLine("            get");
            CodeString.AppendLine("            {");
            CodeString.AppendLine("                if (service == null)");
            CodeString.AppendLine("                {");
            CodeString.AppendLine("                    service = new " + classname + "Service();");
            CodeString.AppendLine("                }");
            CodeString.AppendLine("                return service;");
            CodeString.AppendLine("            }");
            CodeString.AppendLine("        }");
            CodeString.AppendLine("        #endregion");
            CodeString.AppendLine("");
            CodeString.AppendLine("    }");

            return CodeString.ToString();
        }
    }

    public interface IAdvOpt
    {
        bool EnableAdvOpt { get; }
        bool IsEntityEnabled(string name);
        void RefreshEntities(Type[] types);
    }
}