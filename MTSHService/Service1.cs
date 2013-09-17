using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Data.OleDb;
using System.Web;
using System.Configuration;

namespace MTSHService
{
    public partial class Service1 : ServiceBase
    {
        BackgroundWorker wg1;
        string connString = @"Provider=vfpoledb.1;Data Source=e:\;Persist Security Info=False;";

        public Service1()
        {
            InitializeComponent();
            wg1 = new BackgroundWorker();
        }

        private void DumpData()
        {
            
            wg1.RunWorkerAsync();
            wg1.DoWork += delegate(object send, DoWorkEventArgs e)
            {
                using (OleDbConnection dbcon = new OleDbConnection(connString))
                {
                   using(OleDbCommand Cmd = new OleDbCommand("select * from stock_do.dbf where cuscode='Z6A01'"))
                   {
                    Cmd.Connection = dbcon;

                    Cmd.Parameters.Add(new OleDbParameter("?", "D"));
                    Cmd.Parameters.Add(new OleDbParameter("?", "M"));

                    dbcon.Open();
                    IDataReader reader = Cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine(reader["Company"]);
                    }
                    }
                }
            };
            
        
        }

     

        protected override void OnStart(string[] args)
        {
            DumpData();
        }

        protected override void OnStop()
        {
        }
    }
}
