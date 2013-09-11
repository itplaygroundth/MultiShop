using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraBars.Helpers;
using DevExpress.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;
using ServiceStack.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using SmartLib.Helpers;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace MSClient
{
    public partial class MSForm : DevExpress.XtraEditors.XtraForm
    {
        int lockTimerCounter = 0;
        string scanbar = "";
        static string instancename = ConfigurationManager.AppSettings["instanceName"].ToString();
        static string username = ConfigurationManager.AppSettings["username"].ToString();
        static string password = ConfigurationManager.AppSettings["password"].ToString();
        static string dbname = ConfigurationManager.AppSettings["dbname"].ToString();
        string strcon = "";
        string cuscode = ConfigurationManager.AppSettings["cuscode"].ToString();
        string nodeA = ConfigurationManager.AppSettings["nodeA"].ToString();
        string nodeB = ConfigurationManager.AppSettings["nodeB"].ToString();
        string _apcode = "";
        public static string sqlite_dbname = Path.GetDirectoryName(Application.ExecutablePath)+ @"\DATA\itemstock.s3db";
        SQLiteDatabase db = null;
        public MSForm()
        {
            InitializeComponent();
            //InitSkinGallery();
            // InitGrid();
            timer.Start();
            OnTimerTick(null, null);
            UpdateDate();
            UpdateIP();
            ShopCodeLabel.Text = cuscode;
            loadData();
            //strcon = string.Format(@"Server={0};Database={1};User Id={2};Password={3};", instancename, dbname, username, password);
            strcon = string.Format(@"Data Source={0}; Initial Catalog={1};User Id={2};Password={3};", instancename, dbname, username, password);
            db = new SQLiteDatabase(sqlite_dbname);
        }
        //void InitSkinGallery()
        //{
        //    SkinHelper.InitSkinGallery(rgbiSkins, true);
        //}
        BindingList<Person> gridDataList = new BindingList<Person>();
        void InitGrid()
        {
            //gridDataList.Add(new Person("John", "Smith"));
            //gridDataList.Add(new Person("Gabriel", "Smith"));
            //gridDataList.Add(new Person("Ashley", "Smith", "some comment"));
            //gridDataList.Add(new Person("Adrian", "Smith", "some comment"));
            //gridDataList.Add(new Person("Gabriella", "Smith", "some comment"));
            //gridControl.DataSource = gridDataList;
        }

        private void loadData()
        {
            try
            {
                WebRequestGetJson(string.Format("http://{0}/multishop/download/{1}", nodeA, ShopCodeLabel.Text));
            }
            catch (HttpListenerException ex)
            {
                WebRequestGetJson(string.Format("http://{0}/multishop/download/{1}", nodeB, ShopCodeLabel.Text));
                //MessageBox.Show(ex.Message);
            }
        }

        public void WebRequestGetJson(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    string sql = result.ToString().Replace("[", "").Replace("]", "");

                    BCAPINVOICESUB [] cuscode = JsonSerializer.DeserializeFromString<BCAPINVOICESUB[]>(sql);
                    gridControl1.DataSource = cuscode;
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void WebRequestPostJson(string url, string postData)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string sql = "";
                string apcode = "AP-0000";
                streamWriter.Write(postData);
                streamWriter.Flush();
                streamWriter.Close();
                try
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        // MessageBox.Show(result.ToString());
                        //using (SQLiteDatabase db = new SQLiteDatabase(@"DATA\bcapinvoicesub.db"))
                        //{
                        if (result.ToString().StartsWith("["))
                            sql = result.ToString().Replace("[", "").Replace("]", "");
                        else
                            sql = result.ToString();
                        // string strcon=@"Server=MACAIR\SQLEXPRESS;Database=minismart;User Id=sa;Password=password;";
                        if (sql != string.Empty)
                        {
                            var cuscode = JsonSerializer.DeserializeFromString<BCAPINVOICESUB>(sql);
                            if (getMSSQLRows(string.Format("select docno from bcapinvoice where docno='{0}'", cuscode.DOCNO.Trim()), strcon).Rows.Count == 0)
                            {
                                DataTable dtap = getMSSQLRows(string.Format("select code from bcap where name1 like '{0}'", cuscode.APCODE),strcon);
                                if (dtap.Rows.Count > 0)
                                    apcode = dtap.Rows[0][0].ToString();
                                else
                                {
                                    DataTable dtt=getMSSQLRows("select max(code) from bcap",strcon);
                                        if(dtt.Rows.Count>0)
                                        {
                                            apcode = (Convert.ToInt16(dtt.Rows[0][0].ToString().Substring(dtt.Rows[0][0].ToString().IndexOf("-")+1,4)) + 1).ToString();
                                            for (int i = 0; i < 5; i++)
                                                apcode = "0" + apcode;
                                                
                                                apcode = "AP-"+apcode.Substring(apcode.Length-4,4);
                                            
                                        }
                                    insertMSSQL(string.Format("insert into bcap(code,name1,address)values('{0}','{1}','{2}')",apcode,_apcode,"..."), strcon);
                                }
                                sql = string.Format("insert into bcapinvoice(docno,docdate,apcode,iscompletesave,taxtype,billtype,grirbillstatus)values('{0}','{1}','{2}',1,1,1,1)", cuscode.DOCNO.Trim(), ConvertToDate(cuscode.DOCDATE),apcode,cuscode.ID);
                                insertMSSQL(sql, strcon);
                                
                            }
                            sql = string.Format("select itemcode,sum(qty) as qty from bcapinvoicesub where docno='{0}' and itemcode='{1}' group by docno,itemcode", cuscode.DOCNO, cuscode.ITEMCODE);
                            if (getMSSQLRows(sql, strcon).Rows.Count > 0)
                            {
                                int _qty = Convert.ToInt16(getMSSQLRows(sql, strcon).Rows[0]["qty"])+1;
                                sql = string.Format("update bcapinvoicesub set qty={0},cnqty={0},grremainqty={0},amount=price*({0}),netamount=amount/1.05,homeamount=amount/1.07 where docno='{1}' and itemcode='{2}'", _qty, cuscode.DOCNO, cuscode.ITEMCODE);
                                insertMSSQL(sql, strcon);
                            }
                            else
                            {
                                sql = string.Format("insert into bcapinvoicesub(docno,docdate,apcode,itemcode,itemname,qty,cnqty,grremainqty,stocktype,unitcode,shelfcode,whcode,price,amount,netamount,homeamount)values('{5}','{6}','{8}','{0}','{1}',{2},{2},{2},2,'001','001','001',{3},{4},{7},{7})", cuscode.ITEMCODE, cuscode.ITEMNAME.ToString().Replace("'", "นิ้ว"), cuscode.QTY, cuscode.SALEPRICE, (Convert.ToDouble(cuscode.QTY) * Convert.ToDouble(cuscode.SALEPRICE)).ToString(), cuscode.DOCNO, ConvertToDate(cuscode.DOCDATE), ((Convert.ToDouble(cuscode.QTY) * Convert.ToDouble(cuscode.SALEPRICE)) / 1.07).ToString(),apcode);
                                insertMSSQL(sql, strcon);
                                db.ExecuteNonQuery(string.Format("insert into bcapinvoicesub (docno,docdate,apcode,itemcode,price,qty,amount,cost)values('{0}','{1}','{2}','{3}',{4},{5},{6},{7})", cuscode.DOCNO,ConvertToDate(cuscode.DOCDATE),"AP-0001",cuscode.ITEMCODE,cuscode.SALEPRICE,cuscode.QTY,(Convert.ToDouble(cuscode.QTY) * Convert.ToDouble(cuscode.SALEPRICE)),cuscode.SALEPRICE));
                            }
                           
                                sql = string.Format("update bcitem set lastbuyprice={0} where code='{1}'", cuscode.SALEPRICE, cuscode.ITEMCODE);
                                db.ExecuteNonQuery(sql);
                          
                            if (getMSSQLRows(string.Format("select code from bcitem where code='{0}'",cuscode.ITEMCODE), strcon).Rows.Count == 0)
                            {
                                sql = string.Format("insert into bcitem(id,code,name1,defstkunitcode,vendorcode)values('{4}','{0}','{1}','{2}','{3}')",cuscode.ITEMCODE,cuscode.ITEMNAME,"001",cuscode.SUPPLIERCO);
                                insertMSSQL(sql, strcon);
                            }
                            sql = string.Format("update bcapinvoice set AfterDiscount={0},BeforeTaxAmount={0}/1.07,TaxAmount={0}-({0}/1.07),TotalAmount={0},BillBalance={0},NetDebtAmount={0},HomeAmount={0}/1.07,SumOfItemAmount={0}", string.Format("(select sum(amount) from bcapinvoicesub where docno='{0}')", cuscode.DOCNO));
                            insertMSSQL(sql, strcon);
                            sql = string.Format("insert into bcserialmaster(CtrlNo,serialno,ItemCode,ReceiveDate,RegisDate,ReceiveDocNo,ApCode,ItemStatus,RegisterNo,ShelfCode,WHCode,ActiveStatus)values('{0}','{0}','{1}','{2}','{2}','{3}','AP-0001',0,'{0}','001','001',1)", cuscode.SERIALNO, cuscode.ITEMCODE, ConvertToDate(cuscode.DOCDATE), cuscode.DOCNO);
                            insertMSSQL(sql, strcon);

                            sql = string.Format("insert into bcserialtrans(CtrlNo,serialno,ItemCode,DocDate,DocNo,ShelfCode,WHCode,savefrom)values('{0}','{0}','{1}','{2}','{3}','001','001',2)", cuscode.SERIALNO, cuscode.ITEMCODE, ConvertToDate(cuscode.DOCDATE), cuscode.DOCNO);
                            insertMSSQL(sql, strcon);

                            sql = string.Format("insert into bcserialprintform(StartCtrlNo,StopCtrlNo,serialtext,ItemCode,DocNo,savefrom)values('{0}','{0}','{0}','{1}','{2}',2)", cuscode.SERIALNO, cuscode.ITEMCODE, cuscode.DOCNO);
                            insertMSSQL(sql, strcon);

                            sql = string.Format("INSERT INTO ProcessStock (ItemCode,DocDate,ProcessFlag,FlowStatus,ProcessType,ProcessCase)  VALUES('{0}','{1}',0,1,5,2)", cuscode.ITEMCODE, ConvertToDate(cuscode.DOCDATE));
                            insertMSSQL(sql, strcon);
                            //  db.ExecuteNonQuery(string.Format("update bcapinvoicesub set sending='1' where cuscode='{0}' and docno='{1}'", cuscode.CUSCODE, cuscode.DOCNO));
                        }
                        else
                            MessageBox.Show("ไม่พบหมายเลขซีเรียลนี้!โปรดตรวจสอบ");
                    }
                }
                catch (WebException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private string ConvertToDate(string docdate)
        {
            if (docdate.Length > 10)
            {
                string[] temp = docdate.Split('/');
                string day = temp[0];
                string mon = temp[1];
                string year = temp[2].Substring(0, 4);

                if (Convert.ToInt16(year) > DateTime.Now.Year)
                    year = (Convert.ToInt16(year) - 543).ToString();
                docdate = string.Format("{0}/{1}/{2}", year, mon, day);
            }
            return docdate;
        }

        private DataTable getMSSQLRows(string sql, string connstr)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection scon = new SqlConnection(connstr))
                {
                    if (scon.State == ConnectionState.Closed)
                        scon.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, scon))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return dt;
        }
        private void insertMSSQL(string sql, string connstr)
        {
            try
            {
                using (SqlConnection scon = new SqlConnection(connstr))
                {
                    if (scon.State == ConnectionState.Closed)
                        scon.Open();
                    using (SqlCommand scom = new SqlCommand(sql, scon))
                    {
                        scom.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MClient_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                //MessageBox.Show(scanbar.Replace("D",""));
                UpdateBcapinvoice(scanbar.Replace("D", ""));
                scanbar = String.Empty;
            }
            else
                if (e.KeyCode == Keys.F5)
                    loadData();
                else
                    scanbar += e.KeyCode.ToString();
        }


        private void UpdateBcapinvoice(string serialno)
        {
            try
            {
                BCAPINVOICESUB[] inv = gridControl1.DataSource as BCAPINVOICESUB[];
                DataTable dt = ConvertToDT(inv);
                DataRow[] result = dt.Select(string.Format("SERIALNO='{0}'", serialno));
                List<string> gridstr = new List<string>();
                foreach (DataRow dr in result)
                {
                    gridstr.Add(dr["DOCNO"].ToString());
                    gridstr.Add(dr["SERIALNO"].ToString());
                }
                //if(serialno=="1202089925")
                //   return;
                WebRequestPostJson(string.Format("http://{0}/multishop/update", nodeA), JsonSerializer.SerializeToString(gridstr.ToArray()));
                loadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private DataTable ConvertToDT(BCAPINVOICESUB[] inv)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("DOCNO", typeof(string)));
            dt.Columns.Add(new DataColumn("SERIALNO", typeof(string)));

            foreach (BCAPINVOICESUB api in inv)
            {
                DataRow dr = dt.NewRow();
                dr["DOCNO"] = api.DOCNO;
                dr["SERIALNO"] = api.SERIALNO;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        int GetStringLength(string str)
        {
            int counter = 0;
            int pos = 0;
            while (pos < str.Length)
            {
                if (str[pos] != ':')
                    counter++;
                pos++;
            }
            return counter;
        }
        void OnTimerTick(object sender, EventArgs e)
        {
            if (lockTimerCounter == 0)
            {
                lockTimerCounter++;
                UpdateTime();
                lockTimerCounter--;
            }
        }
        void UpdateTime()
        {
            string time = DateTime.Now.ToLongTimeString();
            //if (GetStringLength(time) > digitalGauge1.DigitCount)
            //    digitalGauge1.DigitCount = GetStringLength(time);
            //digitalGauge1.Text = time;
            timeLabel.Text = time;
        }
        void UpdateDate()
        {
            string date = DateTime.Now.ToShortDateString();
            dateLabel.Text = date;
        }

        void UpdateIP()
        {
            ShopIpLabel.Text = getIP().Length == 0 ? "0.0.0.0" : getIP();
        }
        string getIP()
        {
            try
            {
                WebClient client = new WebClient();
                return client.DownloadString("http://icanhazip.com/");
            }
            catch (WebException wx)
            {
                return string.Empty;
            }
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
           // DataRow rows;// = new ArrayList();
           // string serial = "";
           // GridView view = (GridView)sender;
           // GridHitInfo info = view.CalcHitInfo(e.Location);
           //  if (info.InRow || info.InRowCell)
           // {
           //     rows = view.GetDataRow(info.RowHandle);
                //foreach (BCAPINVOICESUB val in rows)
                //{
             //       serial = rows["SERIALNO"].ToString();
                //}
            
            // Add the selected rows to the list.
            //if (view.FocusedRowHandle < 0)
            //    return;
            //foreach (int index in gridView1.GetSelectedRows())
            //{
            //    rows.Add(gridView1.GetRow(index));
            //}
            //if (rows.Count > 0)
            //{
            //    foreach (BCAPINVOICESUB val in rows)
            //    {
            //        serial = val.SERIALNO;
            //    }
            //}
            //UpdateBcapinvoice(serial);
            //scanbar = String.Empty;
            //}
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            DataRow result;
            GridView view = (GridView)sender;
            BCAPINVOICESUB[] invoice = (BCAPINVOICESUB[])gridControl1.DataSource;
            Point pt = view.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo info = view.CalcHitInfo(pt);

            if (info.InRow || info.InRowCell)
            {
                //result = invoice[info.RowHandle].SERIALNO;
                UpdateBcapinvoice(invoice[info.RowHandle].SERIALNO);
                scanbar = String.Empty;

            }
        }
    }

    public class BCAPINVOICESUB
    {
        public string ID { get; set; }
        public string DOCNO { get; set; }
        public string CUSCODE { get; set; }
        public string DOCDATE { get; set; }
        public string DOCTIME { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMNAME { get; set; }
        public string SERIALNO { get; set; }
        public string QTY { get; set; }
        public string SALEPRICE { get; set; }
        public string APCODE { get; set; }
        public string SUPPLIERCO { get; set; }
    }
}
