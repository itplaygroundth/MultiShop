using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using SmartLib.Helpers;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;


namespace MultiShop
{
    public partial class Setting : DevExpress.XtraEditors.XtraForm
    {
        public DataRow result { get; set; }
        private DataTable dt = new DataTable();
        private SQLiteDatabase db=null;
        public static string sqlite_dbname = Path.GetDirectoryName(Application.ExecutablePath)+ @"\DATA\config.s3db";
        DataTable dconfig;
        DataTable dsale;
        DataTable ditem;
        string strcon;
        private static string instancename = ConfigurationManager.AppSettings["instanceName"].ToString();
        private static string username = ConfigurationManager.AppSettings["username"].ToString();
        private static string password = ConfigurationManager.AppSettings["password"].ToString();
        private static string dbname = ConfigurationManager.AppSettings["dbname"].ToString();
        public Setting()
        {
            InitializeComponent();
            initDataRow();
            strcon = string.Format(@"Data Source={0}; Initial Catalog={1};User Id={2};Password={3};", instancename, dbname, username, password);
            preDocNoCombo();
        }

        private void initDataRow()
        {
            db = new SQLiteDatabase(sqlite_dbname);

            dconfig = db.GetDataTable("select companyname,address,telephone,province,taxid,taxrate from configuration");
            dsale = db.GetDataTable("select formtype,billlock,saleprice,costprice,gp,autopurch,docno,customer from saleconfig");
            ditem = db.GetDataTable("select iscost,ismin,islowcost,ispricechange from itemconfig");
            if(ditem.Rows.Count>0)
                {
                    vGridControl3.Rows["editorRow6"].Properties.Value = Convert.ToInt16(ditem.Rows[0]["iscost"]) == 1 ? "เตือน" : "ไม่เตือน";
                    vGridControl3.Rows["editorRow7"].Properties.Value = Convert.ToInt16(ditem.Rows[0]["ismin"])==1? "เตือน" : "ไม่เตือน";
                    vGridControl3.Rows["editorRow8"].Properties.Value = Convert.ToInt16(ditem.Rows[0]["islowcost"])==1?"เตือน" : "ไม่เตือน";
                    vGridControl3.Rows["editorRow9"].Properties.Value = Convert.ToInt16(ditem.Rows[0]["ispricechange"])==1?"เตือน" : "ไม่เตือน";
                }
            if (dsale.Rows.Count > 0)
            {
                vGridControl2.Rows["editorRow1"].Properties.Value = dsale.Rows[0]["formtype"].ToString();
                vGridControl2.Rows["editorRow2"].Properties.Value = dsale.Rows[0]["billlock"].ToString()=="1"?"ทำงาน":"ไม่ทำงาน";
                vGridControl2.Rows["editorRow3"].Properties.Value = dsale.Rows[0]["saleprice"].ToString();
                vGridControl2.Rows["editorRow4"].Properties.Value = dsale.Rows[0]["costprice"].ToString();
                vGridControl2.Rows["editorRow5"].Properties.Value = dsale.Rows[0]["gp"].ToString();
                vGridControl2.Rows["row5"].Properties.Value = dsale.Rows[0]["autopurch"].ToString() == "1" ? "ทำงาน" : "ไม่ทำงาน";
                vGridControl2.Rows["row7"].Properties.Value = dsale.Rows[0]["docno"].ToString();
                vGridControl2.Rows["row8"].Properties.Value = dsale.Rows[0]["customer"].ToString();


            }
            if (dconfig.Rows.Count > 0)
            {
                vGridControl1.Rows["row"].Properties.Value = dconfig.Rows[0]["companyname"].ToString();
                vGridControl1.Rows["row1"].Properties.Value = dconfig.Rows[0]["address"].ToString();
                vGridControl1.Rows["row2"].Properties.Value=dconfig.Rows[0]["telephone"].ToString();
                vGridControl1.Rows["row3"].Properties.Value = dconfig.Rows[0]["province"].ToString();
                vGridControl1.Rows["row4"].Properties.Value = dconfig.Rows[0]["taxid"].ToString();
                vGridControl1.Rows["row6"].Properties.Value = dconfig.Rows[0]["taxrate"].ToString();
            
            }
            //vGridControl1.DataSource = dconfig;
            //vGridControl2.DataSource = dsale;
            //vGridControl3.DataSource = ditem;


        }

        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            xtraTabControl1.SelectedTabPageIndex = 0;
        }

        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            xtraTabControl1.SelectedTabPageIndex = 1;
        }
        
        private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            xtraTabControl1.SelectedTabPageIndex = 2;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Dictionary<string,string> dic= new Dictionary<string, string>();
            if (vGridControl3.Rows["editorRow6"].Properties.Value == null) dic.Add("iscost", "0");else
            dic.Add("iscost",vGridControl3.Rows["editorRow6"].Properties.Value.ToString()=="เตือน"?"1":"0");
            if (vGridControl3.Rows["editorRow7"].Properties.Value == null) dic.Add("ismin", "0");else
            dic.Add("ismin", vGridControl3.Rows["editorRow7"].Properties.Value.ToString() == "เตือน" ? "1" : "0");
            if (vGridControl3.Rows["editorRow8"].Properties.Value == null) dic.Add("islowcost", "0");else
            dic.Add("islowcost", vGridControl3.Rows["editorRow8"].Properties.Value.ToString() == "เตือน" ? "1" : "0");
            if (vGridControl3.Rows["editorRow9"].Properties.Value == null) dic.Add("ispricechange", "0");
            dic.Add("ispricechange", vGridControl3.Rows["editorRow9"].Properties.Value.ToString() == "เตือน" ? "1" : "0");

            if (ditem.Rows.Count > 0)
                db.ExecuteNonQuery("delete from itemconfig");
                db.Insert("itemconfig", dic);
                dic = new Dictionary<string, string>();

                
               dic.Add("formtype",vGridControl2.Rows["editorRow1"].Properties.Value.ToString());
               dic.Add("billlock", vGridControl2.Rows["editorRow2"].Properties.Value.ToString() == "ทำงาน" ? "1" : "0");
               dic.Add("saleprice",vGridControl2.Rows["editorRow3"].Properties.Value.ToString());
               dic.Add("costprice",vGridControl2.Rows["editorRow4"].Properties.Value.ToString());
               dic.Add("gp",vGridControl2.Rows["editorRow5"].Properties.Value.ToString());
               dic.Add("autopurch", vGridControl2.Rows["row5"].Properties.Value.ToString() == "ทำงาน" ? "1" : "0");
               dic.Add("docno", vGridControl2.Rows["row7"].Properties.Value.ToString());
               dic.Add("customer", vGridControl2.Rows["row8"].Properties.Value.ToString());
               
               if (dsale.Rows.Count > 0)
                   db.ExecuteNonQuery("delete from saleconfig");
               db.Insert("saleconfig", dic);
               dic = new Dictionary<string, string>();
               if (vGridControl1.Rows["row"].Properties.Value == null) dic.Add("companyname", "");else
               dic.Add("companyname",vGridControl1.Rows["row"].Properties.Value.ToString());
               if (vGridControl1.Rows["row1"].Properties.Value == null) dic.Add("address", "");else
               dic.Add("address",vGridControl1.Rows["row1"].Properties.Value.ToString());
                // = dconfig.Rows[0]["address"].ToString();
               if (vGridControl1.Rows["row2"].Properties.Value == null) dic.Add("telephone", "");else
               dic.Add("telephone",vGridControl1.Rows["row2"].Properties.Value.ToString());
               if (vGridControl1.Rows["row3"].Properties.Value == null) dic.Add("province", ""); else
               dic.Add("province",vGridControl1.Rows["row3"].Properties.Value.ToString());
               if (vGridControl1.Rows["row4"].Properties.Value == null) dic.Add("taxid", "");else
               dic.Add("taxid",vGridControl1.Rows["row4"].Properties.Value.ToString());
               if (vGridControl1.Rows["row6"].Properties.Value == null) dic.Add("taxrate", "");
               else
                   dic.Add("taxrate", vGridControl1.Rows["row6"].Properties.Value.ToString());
               if (dconfig.Rows.Count > 0) db.ExecuteNonQuery("delete from configuration");
                
               db.Insert("configuration", dic);
               this.Close();
               this.DialogResult = DialogResult.OK;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.Cancel;
        }

        private void preDocNoCombo()
        {
            using (SqlConnection scon = new SqlConnection(strcon))
            {
                if (scon.State == ConnectionState.Closed) scon.Open();
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter("select goupcode,goupdesc from bcsystrdocg where TransKey='BillingTransConfig' order by linenumber ", scon))
                {
                    da.Fill(dt);
                }
                
                foreach (DataRow dr in dt.Rows)
                    configDocno.Properties.Items.Add(string.Format("{0}", dr["goupcode"], dr["goupdesc"]));
            }
        }


      
    }
}