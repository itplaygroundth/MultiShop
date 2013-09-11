using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.Sql;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;

namespace DbfTools
{
    public partial class DataSource : DevExpress.XtraEditors.XtraForm
    {
        public string connectionstring { get; set; }
        private string servername = "";
        private string username = "";
        private string passwd = "";
        private string dbname = "";
        private BackgroundWorker bgw;
        
        public DataSource()
        {
            InitializeComponent();
            bgw = new BackgroundWorker();
            comboBoxEdit2.Click += new EventHandler(comboBoxEdit2_Click);
        }

        void comboBoxEdit2_Click(object sender, EventArgs e)
        {
           // throw new NotImplementedException();
            if (comboBoxEdit2.Properties.Items.Count == 0)
            {
                username = textEdit1.Text;
                passwd = textEdit2.Text;
                //if (comboBoxEdit2.SelectedIndex != -1)
                //{
                    connectionstring = string.Format(@"Server={0};Database={1};User Id={2};Password={3};", servername, dbname, username, passwd);
                    SqlConnection sqlConnection = new SqlConnection(connectionstring);
                    Microsoft.SqlServer.Management.Common.ServerConnection serverConnection = new Microsoft.SqlServer.Management.Common.ServerConnection(sqlConnection);
                    try
                    {
                        Server server = new Server(serverConnection);
                        //using (var con = new SqlConnection(constring))
                        //using (var da = new SqlDataAdapter("SELECT Name FROM master.sys.databases", con))
                        //{
                        //    var ds = new DataSet();
                        //    da.Fill(ds);
                        foreach (Database db in server.Databases)
                            comboBoxEdit2.Properties.Items.Add(db.Name);

                        //}
                    }
                    catch (Microsoft.SqlServer.Management.Common.ConnectionFailureException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
               // }
            }
            else
            {
                dbname = comboBoxEdit2.SelectedText;
                connectionstring = string.Format(@"Server={0};Database={1};User Id={2};Password={3};", servername, dbname, username, passwd);

            }
        }

        private void DataSource_Load(object sender, EventArgs e)
        {
            DataTable instancedt = new DataTable();
            int i = 0,c=0;
            groupControl1.Enabled = false;
            groupControl2.Enabled = false;
            bgw.WorkerReportsProgress = true;
            bgw.DoWork += delegate(object send, DoWorkEventArgs args)
            {
                instancedt = SqlDataSourceEnumerator.Instance.GetDataSources();
                //DataTable instancedt = SmoApplication.EnumAvailableSqlServers(true);
                bgw.ReportProgress(i);
                i += 1;
            };

            bgw.ProgressChanged += delegate(object send, ProgressChangedEventArgs args)
            {
                c=args.ProgressPercentage;
            };

            bgw.RunWorkerCompleted += delegate(object send, RunWorkerCompletedEventArgs args)
            {
                foreach (DataRow dr in instancedt.Rows)
                {
                    if(dr["instanceName"].ToString().Length>0)
                    comboBoxEdit1.Properties.Items.Add(string.Concat(dr["ServerName"], "\\", dr["InstanceName"]));
                    else
                        comboBoxEdit1.Properties.Items.Add(string.Concat(dr["ServerName"]));
                }
                groupControl1.Enabled = true;
                groupControl2.Enabled = true;
            };
            bgw.RunWorkerAsync();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //connectionstring = "";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit1.SelectedIndex != -1)
            {
                servername = comboBoxEdit1.SelectedText;
                comboBoxEdit2.Properties.Items.Clear();
                username = textEdit1.Text;
                passwd = textEdit2.Text;
                dbname = "master";
                connectionstring = string.Format(@"Server={0};Database={1};User Id={2};Password={3};", servername,dbname, username,passwd);
                //Server server = new Server(servername);
                //"Server=myServerName\myInstanceName;Database=myDataBase;User Id=myUsername;Password=myPassword;"
                SqlConnection sqlConnection = new SqlConnection(connectionstring);
                Microsoft.SqlServer.Management.Common.ServerConnection serverConnection = new Microsoft.SqlServer.Management.Common.ServerConnection(sqlConnection);
                try
                {
                    Server server = new Server(serverConnection);
                    //using (var con = new SqlConnection(constring))
                    //using (var da = new SqlDataAdapter("SELECT Name FROM master.sys.databases", con))
                    //{
                    //    var ds = new DataSet();
                    //    da.Fill(ds);
                    foreach (Database db in server.Databases)
                        comboBoxEdit2.Properties.Items.Add(db.Name);

                    //}
                }
                catch (Microsoft.SqlServer.Management.Common.ConnectionFailureException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit2.Properties.Items.Count == 0)
            {
                if (comboBoxEdit2.SelectedIndex != -1)
                {
                    connectionstring = string.Format(@"Server={0};Database={1};User Id={2};Password={3};", servername, dbname, username, passwd);
                    SqlConnection sqlConnection = new SqlConnection(connectionstring);
                    Microsoft.SqlServer.Management.Common.ServerConnection serverConnection = new Microsoft.SqlServer.Management.Common.ServerConnection(sqlConnection);
                    try
                    {
                        Server server = new Server(serverConnection);
                        //using (var con = new SqlConnection(constring))
                        //using (var da = new SqlDataAdapter("SELECT Name FROM master.sys.databases", con))
                        //{
                        //    var ds = new DataSet();
                        //    da.Fill(ds);
                        foreach (Database db in server.Databases)
                            comboBoxEdit2.Properties.Items.Add(db.Name);

                        //}
                    }
                    catch (Microsoft.SqlServer.Management.Common.ConnectionFailureException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                dbname = comboBoxEdit2.SelectedText;
                connectionstring = string.Format(@"Server={0};Database={1};User Id={2};Password={3};", servername, dbname, username, passwd);
                   
            }
        }

        private void radioGroup1_EditValueChanged(object sender, EventArgs e)
        {
            if (radioGroup1.EditValue.ToString()=="0")
            {
                textEdit3.Enabled = true;
                comboBoxEdit2.Enabled = false;
            }
            else
            {
                textEdit3.Enabled = false;
                comboBoxEdit2.Enabled = true;
            }
        }

     
    }
}