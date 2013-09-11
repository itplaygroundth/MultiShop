using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.OleDb;

namespace SmartServer.ChildPage
{
    public partial class iPointData : DevExpress.XtraEditors.XtraForm
    {
        BackgroundWorker wg1;
        string connString = @"Provider=VFPOLEDB.1;Data Source=E:\";
        //string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=e:\;Extended Properties=dbase IV;";
        public iPointData()
        {
            InitializeComponent();
            wg1 = new BackgroundWorker();
            DumpData();
            //CustomDbfReader();
            
        }
        private void DumpData()
        {

            wg1.RunWorkerAsync();
            wg1.DoWork += delegate(object send, DoWorkEventArgs e)
            {
                using (OleDbConnection dbcon = new OleDbConnection(connString))
                {
                    using (OleDbCommand Cmd = new OleDbCommand(string.Format("select ORDERNUM,CUSCODE,STOCKDATE from stock_do where cuscode='{0}'","Z6A01")))
                    {
                        Cmd.Connection = dbcon;

                      //  Cmd.Parameters.Add(new OleDbParameter("?", "ZA601"));
                        
                        dbcon.Open();
                        IDataReader reader = Cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Console.WriteLine(reader["ORDERNUM"]);
                        }
                    }
                }
            };


        }
        public void CustomDbfReader()
        {
            string filepath = @"E:\";

            var dataTable = new DataTable();

            string connectionString =
                "Provider=Microsoft.ACE.OLEDB.12.0;Extended Properties=dBASE IV;Data Source='" + filepath + "'";

            using (var oledbConnection = new OleDbConnection(connectionString))
            {
                oledbConnection.Open();
                string stringCommadn = string.Format(@"select * from stock_do");
                var oleDbCommand = new OleDbCommand(stringCommadn, oledbConnection);
                var oleDbDataAdapter = new OleDbDataAdapter
                {
                    SelectCommand = oleDbCommand
                };
                oleDbDataAdapter.Fill(dataTable);
            }
            //Assert.IsNotEmpty(dataTable.Rows);
        }

    }
}