using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace DropQuery
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string base_query = @"SELECT * FROM (SELECT m.name as 怪物名称, m.mobid as 怪物ID, i.name as 物品名称, i.itemid as 物品ID, CAST(chance AS FLOAT)/10000 as 爆率 FROM drop_data as d INNER JOIN mob_name as m INNER JOIN item_name as i WHERE d.dropperid==m.mobid and d.itemid==i.itemid) as v";
        private string connectionString;
        private SQLiteConnection dbConnection;
        string dataPath;

        private void Form1_Load(object sender, EventArgs e)
        {
            dataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\drop.db";
            if (!File.Exists(dataPath))
            {
                byte[] data = DropQuery.Properties.Resources.drop;
                System.IO.FileStream fileStream = new System.IO.FileStream(dataPath, System.IO.FileMode.Create);
                fileStream.Write(data, 0, (int)(data.Length));
                fileStream.Close();
            }
            connectionString = "data source = " + dataPath;
            dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();
            SQLiteDataAdapter da = new SQLiteDataAdapter(base_query, dbConnection);
            DataSet ds = new DataSet();
            da.Fill(ds, "query");
            this.dataGridView1.DataSource = ds.Tables[0];
            setColumn_width();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query;
            if (this.textBox1.Text != "")
            {
                query = @" WHERE 怪物名称 like '%{key}%' or 物品名称 like '%{key}%' or 物品ID like '%{key}%' or 怪物ID like '%{key}%'";
                string keyword = this.textBox1.Text;
                query = base_query + query.Replace("{key}", keyword);
            }
            else
            {
                query = base_query;
            }
            connectionString = "data source = " + dataPath;
            dbConnection = new SQLiteConnection(connectionString);
            SQLiteDataAdapter da = new SQLiteDataAdapter(query, dbConnection);
            DataSet ds = new DataSet();
            da.Fill(ds, "query");
            this.dataGridView1.DataSource = ds.Tables[0];
            setColumn_width();
        }
        
        private void setColumn_width()
        {
            this.dataGridView1.Columns[4].HeaderText = "爆率(%)";
            this.dataGridView1.Columns[0].Width = 125;
            this.dataGridView1.Columns[1].Width = 100;
            this.dataGridView1.Columns[2].Width = 140;
            this.dataGridView1.Columns[3].Width = 100;
            this.dataGridView1.Columns[4].Width = 80;
            this.dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            dbConnection.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (File.Exists(dataPath))
            {
                File.Delete(dataPath);
            }
            DeleteItselfDllCMD();
        }

        private static void DeleteItselfDllCMD()
        {
            FileInfo info = new FileInfo("SQLite.Interop.dll");
            info.Attributes = FileAttributes.Normal;
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 1000 > Nul & Del " + Application.StartupPath + @"\SQLite.Interop.dll");
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            Process.Start(psi);
            Application.Exit();
        }
    }
}
