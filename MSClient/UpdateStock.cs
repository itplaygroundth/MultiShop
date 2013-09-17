using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using SmartLib.Helpers;
using System.IO;
using System.Net;
using ServiceStack.Text;
using DevExpress.XtraGrid.Columns;

namespace MultiShop
{
    public partial class UpdateStock : DevExpress.XtraEditors.XtraForm
    {
        private static SQLiteDatabase db = null;

        public static string sqlite_dbname = Path.GetDirectoryName(Application.ExecutablePath)+ @"\DATA\itemstock.s3db";
        public DataTable dtup { get; set; }
        private DataTable dpup;
        public string serverip { get; set; }
        public UpdateStock()
        {
            InitializeComponent();
            dpup = new DataTable();
            dpup.Columns.Add(new DataColumn() { ColumnName = "ID", Caption = "Id", DataType = typeof(int) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "CODE", Caption = "รหัส", DataType = typeof(string) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "FULLNAME", Caption = "ชื่อ", DataType = typeof(string) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "SHORTNAME", Caption = "ชื่อย่อ", DataType = typeof(string) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "ITEMTYPECO", Caption = "รหัสชนิด", DataType = typeof(string) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "SUPPLIERCO", Caption = "รหัสกลุ่ม", DataType = typeof(string) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "SALEPRICE1", Caption = "ราคา1", DataType = typeof(double) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "SALEPRICE2", Caption = "ราคา2", DataType = typeof(double) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "SALEPRICE3", Caption = "ราคา3", DataType = typeof(double) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "SALEPRICE4", Caption = "ราคา4", DataType = typeof(double) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "SALEPRICE5", Caption = "ราคา5", DataType = typeof(double) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "SALEPRICE6", Caption = "ราคา6", DataType = typeof(double) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "SALEPRICE7", Caption = "ราคา7", DataType = typeof(double) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "SALEPRICE8", Caption = "ราคา8", DataType = typeof(double) });
            dpup.Columns.Add(new DataColumn() { ColumnName = "SALEPRICE9", Caption = "ราคา9", DataType = typeof(double) });
            gridControl1.DataSource=dpup;

        }

       

        public static DataTable UploadItem(string serverip)
        {
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            string limit = "";
            string itemcode = "";
            string msql = "select ID, CODE, FULLNAME, SHORTNAME, SPEC, ITEMTYPECO,SUPPLIERCO, BRANDCODE, SALEPRICE1, SALEPRICE2, SALEPRICE3, SALEPRICE4, SALEPRICE5, SALEPRICE6, SALEPRICE7, SALEPRICE8,SALEPRICE9, CH1, CH2, CH3, CH4, CH5 from bcitem where id is NULL and code is not NULL";
            try
            {
                
                SQLiteDatabase db = new SQLiteDatabase(@"DATA\itemmaster.db");

                DataTable dt = db.GetDataTable("select max(id) as last from bcitem");
                BCITEM[] lastrow = WebRequestGetJson(string.Format("http://{0}/multishop/lastrow", serverip));
                if (dt.Rows.Count > 0 && lastrow.Length > 0)
                    if (Convert.ToInt16(dt.Rows[0][0]) < Convert.ToInt16(lastrow[0].LASTROW))
                    {
                        limit = string.Format("limit {0},{1}", dt.Rows[0][0], lastrow[0].LASTROW);
                        BCITEM[] result = WebRequestGetJson(string.Format("http://{0}/multishop/loading/{1},{2}", serverip, dt.Rows[0][0], lastrow[0].LASTROW));
                        DataRow drr = dt3.NewRow();
                        drr["id"] = result[0].ID;
                        drr["code"] = result[0].CODE;
                        drr["fullname"] = result[0].FULLNAME;
                        drr["shortname"] = result[0].SHORTNAME;
                        drr["saleprice1"] = result[0].SALEPRICE1;
                        drr["saleprice2"] = result[0].SALEPRICE2;
                        drr["saleprice3"] = result[0].SALEPRICE3;
                        drr["saleprice4"] = result[0].SALEPRICE4;
                        drr["saleprice5"] = result[0].SALEPRICE5;
                        drr["saleprice6"] = result[0].SALEPRICE6;
                        drr["saleprice7"] = result[0].SALEPRICE7;
                        drr["saleprice8"] = result[0].SALEPRICE8;
                        drr["saleprice9"] = result[0].SALEPRICE9;
                        drr["supplierco"] = result[0].SUPPLIERCO;
                        drr["itemtypeco"] = result[0].ITEMTYPECO;
                        dt3.Rows.Add(drr);
                    }
                    else
                    {

                        dt = db.GetDataTable(msql);//.AsEnumerable().CopyToDataTable();
                        dt3 = dt.Clone();
                        foreach (DataRow dr in dt.Rows)
                        {
                           
                            BCITEM[] cuscode = WebRequestGetJson(string.Format("http://{0}/multishop/checkitem/{1}", serverip, dr["CODE"].ToString()));

                            //Log(cuscode.Count().ToString());
                            if (cuscode != null || cuscode.Count() > 0)
                            {
                        

                                dt2 = db.GetDataTable(string.Format("select * from {1} where code='{0}' and (saleprice1<>{2} or saleprice2<>{3} or saleprice3<>{4} or saleprice4<>{5} or saleprice5<>{6} or saleprice6<>{7} or saleprice7<>{8} or saleprice8<>{9} or saleprice9<>{10} or supplierco<>'{11}' or id is null)",
                                           cuscode[0].CODE.ToString(), "bcitem", cuscode[0].SALEPRICE1.ToString(), cuscode[0].SALEPRICE2.ToString(), cuscode[0].SALEPRICE3.ToString(), cuscode[0].SALEPRICE4.ToString(), cuscode[0].SALEPRICE5.ToString(), cuscode[0].SALEPRICE6.ToString(), cuscode[0].SALEPRICE7.ToString(),
                                           cuscode[0].SALEPRICE8.ToString(), cuscode[0].SALEPRICE9.ToString(), cuscode[0].SUPPLIERCO.ToString(), cuscode[0].FULLNAME.ToString().Trim(), cuscode[0].SHORTNAME.ToString().Trim(), cuscode[0].ID));
                                if (dt2.Rows.Count > 0)
                                {
                                    foreach (DataRow drrr in dt2.Rows)
                                    {
                                        DataRow drr = dt3.NewRow();
                                        drr["id"] = drrr["id"];
                                        drr["code"] = drrr["code"];
                                        drr["fullname"] = drrr["fullname"];
                                        drr["shortname"] = drrr["shortname"];
                                        drr["saleprice1"] = drrr["saleprice1"];
                                        drr["saleprice2"] = drrr["saleprice2"];
                                        drr["saleprice3"] = drrr["saleprice3"];
                                        drr["saleprice4"] = drrr["saleprice4"];
                                        drr["saleprice5"] = drrr["saleprice5"];
                                        drr["saleprice6"] = drrr["saleprice6"];
                                        drr["saleprice7"] = drrr["saleprice7"];
                                        drr["saleprice8"] = drrr["saleprice8"];
                                        drr["saleprice9"] = drrr["saleprice9"];
                                        drr["supplierco"] = drrr["supplierco"];
                                        drr["itemtypeco"] = drrr["itemtypeco"];
                                        dt3.Rows.Add(drr);
                                    }
                                }

                                //Log("Line...307");
                                //foreach (DataRow drr in dt2.Rows)
                                //{
                                //    //  Log("Line...310");
                                //    WebRequestGetJson(string.Format("http://{0}/multishop/supplierco/{1}/{2}", serverip, drr["CODE"], dr["SUPPLIERCO"]));
                                //    // Log(string.Format("http://{0}/multishop/supplierco/{1}/{2}", serverip,drr["CODE"],dr["SUPPLIERCO"]));
                                //}

                            }
                        }
                    }
                    
            }
            catch (Exception ex)
            {
                //Log(ex.Message);
            }
            return dt3;
        }

        public static BCITEM[] WebRequestGetJson(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            BCITEM[] cuscode = null;
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    string sql = result.ToString().Replace("[", "").Replace("]", "");

                    cuscode = JsonSerializer.DeserializeFromString<BCITEM[]>(sql);
                    //gridControl1.DataSource = cuscode;
                }

            }
            catch (WebException ex)
            {
                //Log(ex.Message + ":Line219");
            }
            return cuscode;
        }
        public static String[] WebRequestGetJson2String(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            String[] cuscode = null;
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    string sql = result.ToString().Replace("[", "").Replace("]", "");

                    cuscode = JsonSerializer.DeserializeFromString<String[]>(sql);
                    //gridControl1.DataSource = cuscode;
                }

            }
            catch (WebException ex)
            {
                //Log(ex.Message + ":Line219");
            }
            return cuscode;
        }
        private void UpdateStock_Load(object sender, EventArgs e)
        {
            if (dtup.Rows.Count > 0)
            {
                foreach (DataRow dr in dtup.Rows)
                {
                    BCITEM[] cuscode = WebRequestGetJson(string.Format("http://{0}/multishop/checkitem/{1}", serverip, dr["CODE"].ToString()));
                    if (cuscode != null || cuscode.Count() > 0)
                    {

                        DataRow drr = dpup.NewRow();
                        drr["id"] = cuscode[0].ID;
                        drr["code"] = cuscode[0].CODE;
                        drr["fullname"] = cuscode[0].FULLNAME;
                        drr["shortname"] = cuscode[0].SHORTNAME;
                        drr["saleprice1"] = cuscode[0].SALEPRICE1;
                        drr["saleprice2"] = cuscode[0].SALEPRICE2;
                        drr["saleprice3"] = cuscode[0].SALEPRICE3;
                        drr["saleprice4"] = cuscode[0].SALEPRICE4;
                        drr["saleprice5"] = cuscode[0].SALEPRICE5;
                        drr["saleprice6"] = cuscode[0].SALEPRICE6;
                        drr["saleprice7"] = cuscode[0].SALEPRICE7;
                        drr["saleprice8"] = cuscode[0].SALEPRICE8;
                        drr["saleprice9"] = cuscode[0].SALEPRICE9;
                        drr["supplierco"] = cuscode[0].SUPPLIERCO;
                        drr["itemtypeco"] = cuscode[0].ITEMTYPECO;
                        dpup.Rows.Add(drr);

                        //gridControl1.DataSource = dtup;
                        //    foreach(DataColumn dc in dtup.Columns)
                        //    gridView1.Columns.Add(new GridColumn() { Caption = dc.ColumnName, FieldName = dc.ColumnName,Name=dc.ColumnName });

                        // gridControl1.DataSource = dtup;
                         
                    }
                }
                gridControl1.RefreshDataSource();
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            
            SQLiteDatabase db = new SQLiteDatabase(@"DATA\itemmaster.db");
            try
            {
                if (dpup.Rows.Count > 0)
                {
                    foreach (DataRow dr in dpup.Rows)
                    {
                        Dictionary<string, string> api = new Dictionary<string, string>();
                        api.Add("ID", dr["ID"].ToString());
                        api.Add("FULLNAME ", dr["FULLNAME"].ToString());
                        api.Add("SHORTNAME ", dr["SHORTNAME"].ToString());
                        api.Add("ITEMTYPECO ", dr["ITEMTYPECO"].ToString());
                        api.Add("SUPPLIERCO ", dr["SUPPLIERCO"].ToString());
                        api.Add("SALEPRICE1 ", dr["SALEPRICE1"].ToString());
                        api.Add("SALEPRICE2 ", dr["SALEPRICE2"].ToString());
                        api.Add("SALEPRICE3 ", dr["SALEPRICE3"].ToString());
                        api.Add("SALEPRICE4 ", dr["SALEPRICE4"].ToString());
                        api.Add("SALEPRICE5 ", dr["SALEPRICE5"].ToString());
                        api.Add("SALEPRICE6 ", dr["SALEPRICE6"].ToString());
                        api.Add("SALEPRICE7 ", dr["SALEPRICE7"].ToString());
                        api.Add("SALEPRICE8 ", dr["SALEPRICE8"].ToString());
                        api.Add("SALEPRICE9 ", dr["SALEPRICE9"].ToString());
                       string sql=db.Updatestr("bcitem", api, string.Format(" code='{0}'", dr["CODE"]));
                      // Log(sql);
                    }
                    
                }
                MessageBox.Show("Update Completed");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public static void Log(string logMessage)
        {
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine("{0}:{1}:{2}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString(), logMessage);
            }
        }
    }

    public class SBCITEM
    {
        public string ID { get; set; }
        public string CODE { get; set; }
        public string FULLNAME { get; set; }
        public string SHORTNAME { get; set; }
        public string ITEMTYPECO { get; set; }
        public string SUPPLIERCO { get; set; }
        public string BRANDCODE { get; set; }
        public string SPEC { get; set; }
        public string SALEPRICE1 { get; set; }
        public string SALEPRICE2 { get; set; }
        public string SALEPRICE3 { get; set; }
        public string SALEPRICE4 { get; set; }
        public string SALEPRICE5 { get; set; }
        public string SALEPRICE6 { get; set; }
        public string SALEPRICE7 { get; set; }
        public string SALEPRICE8 { get; set; }
        public string SALEPRICE9 { get; set; }
        public string CH1 { get; set; }
        public string CH2 { get; set; }
        public string CH3 { get; set; }
        public string CH4 { get; set; }
        public string CH5 { get; set; }
        public string lastrow { get; set; }

    }
}