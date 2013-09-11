using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Data.SQLite;
using System.Collections;
using System.Net;

namespace SmartCentralControl
{
    public partial class SCC : ServiceBase
    {
        SQLiteDatabase sqlsetting;
        SQLiteDatabase sqlar;
        SQLiteDatabase sqlserv;
        DataTable dsetting,dservlist,darlist;

        public SCC()
        {
            InitializeComponent();

            sqlsetting = new SQLiteDatabase(@"DATA\Setting.db");
            sqlar = new SQLiteDatabase(@"DATA\AR.db");
            sqlserv = new SQLiteDatabase(@"DATA\serverconfig.db");
            dsetting = new DataTable();
            dservlist = new DataTable();
            darlist = new DataTable();
        }

        protected override void OnStart(string[] args)
        {
            loadConfig();
        }

        protected override void OnStop()
        {

        }

        private void loadConfig()
        {
            dsetting = sqlsetting.GetDataTable("select location,downauto,upauto,refresh from setting");
            dservlist = sqlserv.GetDataTable("select ServerId,ipaddress,status from serverconfig");
            darlist = sqlar.GetDataTable("select RowId,ShopName,Province,ipaddress,status from customer");
        }
        private void loaddata()
        {
            try
            {

                DataTable dt = ParseDBF.ReadDBF(string.Format(@"{0}\stock_do.dbf",dsetting.Rows[0]["location"]));

            }
            catch (Exception ex)
            {
                // Do something with ex
                throw new Exception(ex.Message);
            }
            
        
        }
    }
}
