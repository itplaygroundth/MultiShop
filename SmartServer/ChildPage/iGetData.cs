using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
//using DbfDotNet;
using System.Data.SQLite;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
//using System.Threading;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Linq;
using ServiceStack.Text;
using DevExpress.Utils.OAuth.Provider;
using System.Diagnostics;
using System.Net;
using System.Data.OleDb;
 

namespace SmartServer.ChildPage
{
    public partial class iGetData : DevExpress.XtraEditors.XtraForm
    {
        string mypath = @"C:\DATA";
        private List<List<string>> myList = new List<List<string>>();
        private List<string> plist=new List<string>()
        {
         "กรุงเทพมหานคร "," กระบี่ "," กาญจนบุรี "," กาฬสินธุ์ "," กำแพงเพชร "," ขอนแก่น "," จันทบุรี "," ฉะเชิงเทรา "," ชลบุรี "," ชัยนาท "," ชัยภูมิ "," ชุมพร "," เชียงราย ","เชียงใหม่",
         "ตรัง","ตราด","ตาก","นครนายก","นครปฐม","นครพนม","นครราชสีมา","นครศรีธรรมราช","นครสวรรค์","นนทบุรี","นราธิวาส","น่าน","บึงกาฬ","บุรีรัมย์","ปทุมธานี","ประจวบคีรีขันธ์","ปราจีนบุรี",
         "ปัตตานี","พระนครศรีอยุธยา","พังงา","พัทลุง","พิจิตร","พิษณุโลก","เพชรบุรี","เพชรบูรณ์","แพร่","พะเยา","ภูเก็ต","มหาสารคาม","มุกดาหาร","แม่ฮ่องสอน","ยะลา","ยโสธร","ร้อยเอ็ด","ระนอง",
         "ระยอง","ราชบุรี","ลพบุรี","ลำปาง","ลำพูน","เลย","ศรีสะเกษ","สกลนคร","สงขลา","สตูล","สมุทรปราการ","สมุทรสงคราม","สมุทรสาคร","สระแก้ว","สระบุรี","สิงห์บุรี","สุโขทัย","สุพรรณบุรี",
         "สุราษฎร์ธานี","สุรินทร์","หนองคาย","หนองบัวลำภู","อ่างทอง","อุดรธานี","อุทัยธานี","อุตรดิตถ์","อุบลราชธานี","อำนาจเจริญ"
        };
        SQLiteDatabase sqlsetting;
        SQLiteDatabase sqlar;
        SQLiteDatabase sqlserv;
        List<string> filelist = new List<string>();// {"item_master.dbf","serial_bar.dbf","stck_all.dbf","stock_do.dbf"};
        List<DateTime> filemoditiem = new List<DateTime>();
        int stepdbf = 0;
        int step1 = 0;
        int step2 = 0;
        int step3 = 0;
        int step4 = 0;
        BackgroundWorker bgwitem;
        BackgroundWorker bgwstock;
        Timer time;
        bool isbusy=false;
        Stopwatch stopwatch;
        public iGetData()
        {
            InitializeComponent();
            dateEdit1.EditValue = DateTime.Now;
            time = new Timer();
            stopwatch=new Stopwatch();

            bgwitem = new BackgroundWorker();
            bgwstock = new BackgroundWorker();
            //bgw.WorkerReportsProgress = true;
            //bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            //bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            prepareDB();
            sqlsetting = new SQLiteDatabase(@"DATA\Setting.db");
            sqlar = new SQLiteDatabase(@"DATA\AR.db");
            sqlserv = new SQLiteDatabase(@"DATA\serverconfig.db");
            openDB();
            initLookup();
            //listView1.Columns.Add("DOCNO",200);
            //dateEdit1.EditValue = DateTime.Now.ToShortDateString();

            //time.Interval = Convert.ToInt16(textEdit2.Text) * 1000;
            //time.Tick += new EventHandler(time_Tick);
           // time.Start();
            //StartTask();    
        }

        //void time_Tick(object sender, EventArgs e)
        //{
        //   if(!isbusy)
        //       {
        //            isbusy=true;
        //           // Log("StartTask");
        //           // StartTask();
        //       }
        //}

        public static void Log(string logMessage)
        {
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine("{0}:{1}:{2}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString(), logMessage);
            }   
        }

        private void StartTask()
        {
            //System.Threading.Tasks.Parallel.For(0, stepdbf, (i) =>
            //{
            //    ReadDBF(i);

            //});

            //bgw.RunWorkerAsync();
            try
            {
                System.Threading.Tasks.TaskCreationOptions atp = System.Threading.Tasks.TaskCreationOptions.AttachedToParent;
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {

                    if (filelist.Count > 0)
                    {
                        Log(string.Format("{0}", filelist[0]));
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            //ReadDBF(filelist[0]); 
                            ReadODBF(filelist[0]);
                        }, atp);
                    }
                    if (filelist.Count > 1)
                    {
                        Log(string.Format("{0}", filelist[1]));

                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            ReadODBF(filelist[1]);
                        }, atp);
                    }

                    // if (!isbusy)
                    // {
                    //DataTable dip = (DataTable)gridControl2.DataSource;
                    //foreach (DataRow dr in dip.Rows)
                    //{
                    //    System.Threading.Tasks.Task.Factory.StartNew(() => { UploadInvoice(dr["ipaddress"].ToString()); }, atp);
                    //    System.Threading.Tasks.Task.Factory.StartNew(() => { UploadItem(dr["ipaddress"].ToString()); }, atp);
                    //}
                    //}
                    //System.Threading.Tasks.Task.Factory.StartNew(() => { ReadDBF(2); }, atp);
                }).ContinueWith(cont =>
                {
                    // itemcountLabel.Text = "OK";
                    //stepdbf = step1 + step2 + step3 + step4;
                    //if (step1 == 1) step1 = 0;
                    //if (step2 == 1) step2 = 0;
                    //if (step3 == 1) step3 = 0;
                    //if (step4 == 1) step4 = 0;
                    //using (SQLiteDatabase db = new SQLiteDatabase(@"DATA\bcapinvoicesub.db"))
                    //{
                    //    db.ExecuteNonQuery("update bcapinvoicesub set sending='1'");
                    //}
                    if (cont.IsFaulted) Log(cont.Exception.Message.ToString());
                    else
                    {
                        Log(string.Format("starting ... upload"));
                        DataTable dip = (DataTable)gridControl2.DataSource;
                        foreach (DataRow dr in dip.Rows)
                        {
                            UploadInvoice(dr["ipaddress"].ToString());
                            UploadItem(dr["ipaddress"].ToString());
                        }
                        isbusy = false;
                        filelist.Clear();
                    }
                });
            }
            catch (Exception ex)
            {

                Log(ex.Message);
            }
        }


     
        private void UploadInvoice(string serverip)
        {
            
            SQLiteDatabase db = new SQLiteDatabase(@"DATA\bcapinvoicesub.db");
            DataTable dt=db.GetDataTable("select docno,docdate,doctime,cuscode,itemcode,serialno,qty,saleprice,apcode from bcapinvoicesub where sending=0");
            foreach (DataRow dr in dt.Rows)
            {
                string[] arry = new string[5];
                var api = new BCAPINVOICESUB();
                api.CUSCODE = dr["cuscode"].ToString();
                api.DOCDATE = dr["docdate"].ToString();
                api.DOCNO = dr["docno"].ToString();
                api.DOCTIME = dr["doctime"].ToString();
                api.ITEMCODE = dr["itemcode"].ToString();
                api.QTY = dr["qty"].ToString();
                api.SALEPRICE = dr["saleprice"].ToString();
                api.SERIALNO = dr["serialno"].ToString();
                api.APCODE = dr["apcode"].ToString();
                arry[0] = api.ITEMCODE;
                arry[1] = api.SERIALNO;
                arry[2] = "";
                arry[3] = api.QTY;
                arry[4] = api.SALEPRICE;
                
                
                
                var json = JsonSerializer.SerializeToString(api);
               // Log(string.Format("http://{0}/multishop/upload", serverip));
                WebRequestJson(string.Format("http://{0}/multishop/upload",serverip), json);
                this.Invoke(new MethodInvoker(delegate
                {
                       
                    listView1.Items.Add(dr["docno"].ToString()).SubItems.AddRange(arry);
                    
                }));
               
            }
            
        
        }

        public BCITEM[] WebRequestGetJson(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            BCITEM[] cuscode=null;
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
                Log(ex.Message+":Line219");
            }
            return cuscode;
        }


      
        private void UploadItem(string serverip)
        {
            try
            {
                SQLiteDatabase db = new SQLiteDatabase(@"DATA\itemmaster.db");
                
                DataTable dt = db.GetDataTable("select CODE, FULLNAME, SHORTNAME, SPEC, ITEMTYPECO,SUPPLIERCO, BRANDCODE, SALEPRICE1, SALEPRICE2, SALEPRICE3, SALEPRICE4, SALEPRICE5, SALEPRICE6, SALEPRICE7, SALEPRICE8,SALEPRICE9, CH1, CH2, CH3, CH4, CH5 from bcitem where sending=0").AsEnumerable().CopyToDataTable();
                Log("uploadItem:"+dt.Rows.Count.ToString());
                //var qry1 = dt.AsEnumerable().Select(a => new { MobileNo = a["code"].ToString() });
                //var qry2 = cuscode.AsEnumerable().Select(b => new { MobileNo = b.CODE.ToString() });

                //var exceptAB = qry1.Except(qry2);

                //DataTable contacts = (from a in dt.AsEnumerable()
                //                      join ab in exceptAB on a["code"].ToString() equals ab.MobileNo
                //                      select a).CopyToDataTable();

                foreach (DataRow dr in dt.Rows)
                {
                    var api = new BCITEM();
                    //Log("uploadItem:Line266");
                    //Log(dr["CODE"].ToString());
                  //  WebRequestGetJson(string.Format("http://{0}/multishop/supplierco/{0}/{1}", serverip, dr["CODE"].ToString(), dr["SUPPLIERCO"].ToString()));
                    BCITEM[] cuscode = WebRequestGetJson(string.Format("http://{0}/multishop/checkitem/{1}", serverip, dr["CODE"].ToString()));

                    //Log(cuscode.Count().ToString());
                    if (cuscode == null || cuscode.Count() == 0)
                    {
                        //Log("start:Line272");
                        api.CODE = dr["CODE"].ToString();
                        api.FULLNAME = dr["FULLNAME"].ToString();
                        api.SHORTNAME = dr["SHORTNAME"].ToString();
                        api.SPEC = dr["SPEC"].ToString();
                        api.ITEMTYPECO = dr["ITEMTYPECO"].ToString();
                        api.SUPPLIERCO = dr["SUPPLIERCO"].ToString();
                        api.BRANDCODE = dr["BRANDCODE"].ToString();
                        api.SALEPRICE1 = dr["SALEPRICE1"].ToString();
                        api.SALEPRICE2 = dr["SALEPRICE2"].ToString();
                        api.SALEPRICE3 = dr["SALEPRICE3"].ToString();
                        api.SALEPRICE4 = dr["SALEPRICE4"].ToString();
                        api.SALEPRICE5 = dr["SALEPRICE5"].ToString();
                        api.SALEPRICE6 = dr["SALEPRICE6"].ToString();
                        api.SALEPRICE7 = dr["SALEPRICE7"].ToString();
                        api.SALEPRICE8 = dr["SALEPRICE8"].ToString();
                        api.SALEPRICE9 = dr["SALEPRICE9"].ToString();
                        api.CH1 = dr["CH1"].ToString();
                        api.CH2 = dr["CH2"].ToString();
                        api.CH3 = dr["CH3"].ToString();
                        api.CH4 = dr["CH4"].ToString();
                        api.CH5 = dr["CH5"].ToString();

                        var json = JsonSerializer.SerializeToString(api);
                         
                        WebRequestNO(string.Format("http://{0}/multishop/upload/bcitem", serverip), json);
                       // db.ExecuteNonQuery(string.Format("update bcitem set sending=1 where code='{0}'",dr["CODE"].ToString()));
                    }else
                        {
                        //Log("Line...305");
                        DataTable dt2 = db.GetDataTable("select CODE,SUPPLIERCO from bcitem where sending=1").AsEnumerable().CopyToDataTable();
                        //Log("Line...307");
                        foreach(DataRow drr in dt2.Rows)
                        {
                          //  Log("Line...310");
                            WebRequestGetJson(string.Format("http://{0}/multishop/supplierco/{1}/{2}", serverip,drr["CODE"],dr["SUPPLIERCO"]));
                           // Log(string.Format("http://{0}/multishop/supplierco/{1}/{2}", serverip,drr["CODE"],dr["SUPPLIERCO"]));
                            }
                
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }

        }

      

        public void WebRequestinJson(string url, string postData)
        {
            StreamWriter requestWriter;

            var webRequest = System.Net.WebRequest.Create(url) as System.Net.HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.Method = "POST";
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = 20000;

                webRequest.ContentType = "application/json";
                //POST the data.
                using (requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(postData);
                }
            }
            using (WebClient client = new WebClient())
            {
                //client.UploadFile("http://some/upload.php", "foo.bar");
                client.UploadString(url,postData);
            }
        }
        public void WebRequestJson(string url, string postData)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                streamWriter.Write(postData);
                streamWriter.Flush();
                streamWriter.Close();
                try
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                       // Log(result.ToString());
                        using (SQLiteDatabase db = new SQLiteDatabase(@"DATA\bcapinvoicesub.db"))
                        {
                            string sql = result.ToString().Replace("[", "").Replace("]", "");

                            var cuscode = JsonSerializer.DeserializeFromString<BCAPINVOICESUB>(sql);
                            db.ExecuteNonQuery(string.Format("update bcapinvoicesub set sending='1' where cuscode='{0}' and docno='{1}'", cuscode.CUSCODE, cuscode.DOCNO));
                        }
                    }

                }
                catch (WebException ex)
                {
                    Log(ex.Message);
                }
            }
        }
        public void WebRequestNO(string url, string postData)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                streamWriter.Write(postData);
                streamWriter.Flush();
                streamWriter.Close();
                try
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        // Log(result.ToString());
                        using (SQLiteDatabase db = new SQLiteDatabase(@"DATA\itemmaster.db"))
                        {
                            string sql = result.ToString().Replace("[", "").Replace("]", "");

                            var cuscode = JsonSerializer.DeserializeFromString<BCITEM>(sql);
                            Log(string.Format("UploadItem:{0}", cuscode.CODE));
                            db.ExecuteNonQuery(string.Format("update bcitem set sending='1' where code='{0}' ", cuscode.CODE));
                        }
                    }

                }
                catch (WebException ex)
                {
                    Log(ex.Message);
                }
            }
        }
        private string FileWatcher()
        {
           
            string filename = "";
            //string path = string.Format(@"{0}\{1}", mypath, filelist[i]);
           
            FileSystemWatcher fsw = new FileSystemWatcher();
            try
            {
                fsw.Path = mypath;
                fsw.NotifyFilter = NotifyFilters.LastWrite;
                fsw.Filter = "*.dbf";
                fsw.Changed += new FileSystemEventHandler(fsw_Changed);
                fsw.EnableRaisingEvents = true;
            }
            catch(Exception ex)
            {

            }
            return filename;
        }

        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                try
                {
                    if (e.FullPath.Contains("item_master.dbf") || e.FullPath.Contains("stock_do.dbf") || e.FullPath.Contains("menu_list.dbf") || e.FullPath.Contains("menu_list_dtl.dbf"))
                    {
                        filelist.Add(e.FullPath);
                        // Log("File Change");
                        StartTask();
                        //DataTable dip = (DataTable)gridControl2.DataSource;
                        //foreach (DataRow dr in dip.Rows)
                        //{
                        //     UploadInvoice(dr["ipaddress"].ToString()); 
                        //     UploadItem(dr["ipaddress"].ToString());
                        //}
                    }
                    //filelist.Clear();
                    // if(e.FullPath==string.Format(@"{0}\{1}",mypath,filelist[0]))
                    // {
                    //if()
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }
            }
            //throw new NotImplementedException();
        }

        void UpdateLabel(string s)
        {
            if (this.itemcountLabel.InvokeRequired)
            {
                // It's on a different thread, so use Invoke.
                this.BeginInvoke (new MethodInvoker(() => UpdateLabel(s)));
            }
            else
            {
                // It's on the same thread, no need for Invoke
                this.itemcountLabel.Text = s;
            }
        }
    
        private void initLookup()
        {
            DataTable dt=new DataTable();
            dt.Columns.Add(new DataColumn("Name1",typeof(string)));
            myList.Add(plist);

            foreach (string row in myList[0])
            {
                DataRow dr = dt.NewRow();
                dr["Name1"] = row;
                dt.Rows.Add(dr);
            }
            repositoryItemLookUpEdit1.DataSource = dt;
            repositoryItemLookUpEdit1.DisplayMember = "Name1";
            repositoryItemLookUpEdit1.ValueMember = "Name1";
            repositoryItemLookUpEdit1.NullText = "";
            provinceCombo.Properties.DataSource = dt;
            provinceCombo.Properties.DisplayMember = "Name1";
            provinceCombo.Properties.ValueMember = "Name1";
            provinceCombo.Properties.NullText = "";
           // textEdit2.Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month,DateTime.Now.Day,DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second);
        }
        
        private void prepareDB()
        {
        
            SplashScreenManager.ShowForm(typeof(WaitForm1));
            SplashScreenManager.Default.SetWaitFormCaption("โหลดการตั้งค่า");
        

            System.Threading.Thread.Sleep(4000);
            SplashScreenManager.CloseForm();
            
        }
        

        private void openDB()
        {
           
            DataTable dtsetting=sqlsetting.GetDataTable("select location,refresh,autodown,autoup,province from SETTING");
           
                    foreach (DataRow dr in dtsetting.Rows)
                    {
                        textEdit1.Text = dr["location"].ToString();
                        mypath = textEdit1.Text;

                        //textEdit2.Text = dr["refresh"].ToString();
                        if(Convert.ToInt16(dr["AutoDown"])==1)
                            simpleButton2.Text = "ทำงาน";
                        else
                            simpleButton2.Text = "ไม่ทำงาน";
                        if(Convert.ToInt16(dr["AutoUp"])==1)
                            simpleButton5.Text = "ทำงาน";
                        else
                            simpleButton5.Text = "ไม่ทำงาน";
                        provinceCombo.EditValue = dr["province"].ToString();
                    }
                    DataTable dtar = sqlar.GetDataTable("select RowId,ShopName,Province,ipaddress,status from customer");
                    DataTable dtserv = sqlserv.GetDataTable("select serverid,ipaddress,status from serverlist");
                    
                    
                    dtserv.Columns[2].ColumnName = "status";

                    dtar.Columns[4].ColumnName = "status";

                    gridControl2.DataSource = dtserv;
                    gridControl1.DataSource = dtar;
            

        }

   

     

        private void simpleButton1_Click(object sender, EventArgs e)
        {
          
            using (FolderBrowserDialog dial = new FolderBrowserDialog())
            {
                dial.Description = "ตำแหน่งที่เก็บ";
                dial.ShowNewFolderButton = false;
                dial.RootFolder = Environment.SpecialFolder.MyComputer;
                if (dial.ShowDialog() == DialogResult.OK)
                {
                    mypath = dial.SelectedPath;
                    textEdit1.Text = mypath;
                }
            }
        }


        private string UpdateData(List<string> txt, List<string> field, string db, string table,string where)
        {
            string sql = string.Format("update {0} set ", table);
            try
            {
                DataSet ds = new DataSet();
              
                int i = 0;
                foreach (String val in txt)
                {
                    sql += string.Format("{0}='{1}'",field[i], val);
                    if (i < txt.Count - 1)
                        sql += ",";
                    i += 1;
                }
               
                sql += " "+where;
             
            }
            catch (SQLiteException ex)
            {
                //Log(ex.Message);
            }

            return sql;
        
        }
        private List<string> UpdateData(List<List<string>> txt, List<string> field,List<List<string>> del, string db, string table, int col)
        {
            List<string> rlist = new List<string>();
            string where = "";
            string sql = "";
            try
            {
               
                if(txt.Count>0)
                {
                    foreach (List<string> inlist in txt)
                    {
                        if (inlist.Count > 0)
                        {
                            sql = string.Format("update {0} set ", table);
                            int i = 0;
                            where = string.Format(" where {0}='{1}'", field[col], inlist[col]);
                            foreach (String val in inlist)
                            {
                                sql += string.Format("{0}='{1}'", field[i], val);
                                if (i < inlist.Count - 1)
                                    sql += ",";
                                i += 1;
                            }
                            // sql.Trim().Remove(sql.Length-1);
                            sql += where;
                            rlist.Add(sql);
                        }
                        else
                        {
                            sql = string.Format("delete from {0} ", table);
                            rlist.Add(sql);
                    
                        }
                    }
                    foreach (List<string> inlist in del)
                    {
                        if (inlist.Count > 0)
                        {
                            sql = string.Format("delete from {0} ", table);
                            where = string.Format(" where {0}='{1}'", field[col], inlist[col]);
                          
                            sql += where;
                            rlist.Add(sql);
                        }
                    
                    
                    }
                }

               
            }
            catch (SQLiteException ex)
            {
                //Log(ex.Message);
            }

            return rlist;

        }
        private string InsertData(List<string> txt, List<string> field, string db, string table)
        {
            string sql = string.Format("insert into {0}(", table);
            try
            {
                DataSet ds = new DataSet();

                sql += string.Join(",", field);
                sql += ")values(";
                int i = 0;
                foreach (String val in txt)
                {
                    sql += string.Format("'{0}'", val);
                    if (i < txt.Count - 1)
                        sql += ",";
                    i += 1;
                }
               
                sql += ")";
            }
            catch (SQLiteException ex)
            {
                //Log(ex.Message);
            }

            return sql;
        }
        private List<string> InsertData(List<List<string>> txt, List<string> field, string db, string table)
        {
            List<string> rlist = new List<string>();
            string sql = "";
            try
            {
                if(txt.Count>0)
                foreach (List<string> inlist in txt)
                {
                    if (inlist.Count > 0)
                    {
                        sql = string.Format("insert into {0}(", table);



                        sql += string.Join(",", field);
                        sql += ")values(";
                        int i = 0;
                        foreach (String val in inlist)
                        {
                            sql += string.Format("'{0}'", val);
                            if (i < inlist.Count - 1)
                                sql += ",";
                            i += 1;
                        }
                        sql += ")";
                        rlist.Add(sql);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                //Log(ex.Message);
            }

            return rlist;
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            List<string> setting = new List<string>();
            setting.Add(textEdit1.Text);
            //setting.Add(textEdit2.Text);
            if (simpleButton2.Text == "ทำงาน")
                setting.Add("1");
                else
                setting.Add("0");

            if (simpleButton5.Text == "ทำงาน")
                setting.Add("1");
            else
                setting.Add("0");

            setting.Add(provinceCombo.EditValue.ToString());
            List<string> settingf = new List<string>();
            settingf.Add("location");
            //settingf.Add("refresh");
            settingf.Add("autodown");
            settingf.Add("autoup");
            settingf.Add("province");

            List<List<string>> servlist = new List<List<string>>();
            List<string> servlistf = new List<string>();
            List<List<string>> serverdel = new List<List<string>>();
            DataTable dt = (DataTable)gridControl2.DataSource;
            foreach (DataColumn dc in dt.Columns)
            {
                servlistf.Add(dc.ColumnName);
            }
            foreach (DataRow dr in dt.Rows)
            {
                List<string> nlist=new List<string>();
                if (dr.RowState != DataRowState.Deleted)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                      

                        nlist.Add(dr[dc.ColumnName].ToString());
                       
                    }
                       servlist.Add(nlist);
                    
                }
            }

            List<List<string>> arlist = new List<List<string>>();
            List<string> arlistf = new List<string>();
            DataTable dt_=(DataTable)gridControl1.DataSource;
            foreach (DataColumn dc in dt_.Columns)
            {
                arlistf.Add(dc.ColumnName);
            }
            foreach (DataRow dr in dt_.Rows)
            {
                List<string> nlist = new List<string>();
                if (dr.RowState != DataRowState.Deleted)
                {
                    foreach (DataColumn dc in dt_.Columns)
                        nlist.Add(dr[dc.ColumnName].ToString());
                    
                    arlist.Add(nlist);

                }//else
            
            }

            try
            {
                //sqlite = new SQLiteDatabase(@"DATA\Setting.db");
                if (sqlsetting.GetDataTable("Select * from SETTING").Rows.Count > 0)
                    sqlsetting.ExecuteNonQuery(UpdateData(setting, settingf, "Setting", "SETTING", ""));
                else
                    sqlsetting.ExecuteNonQuery(InsertData(setting, settingf, "Setting", "SETTING"));

                sqlserv.ExecuteNonQuery("delete from serverlist");
                if (sqlserv.GetDataTable("Select * from serverlist").Rows.Count > 0)
                {
                    List<string> ulist = UpdateData(servlist, servlistf,serverdel, "", "serverlist",0);
                    foreach (string sql in ulist)
                        sqlserv.ExecuteNonQuery(sql);
                }
                else
                {
                    List<string> ilist = InsertData(servlist, servlistf, "", "serverlist");
                    foreach(string sql in ilist)
                    sqlserv.ExecuteNonQuery(sql);
                }

                     sqlar.ExecuteNonQuery("delete from customer");
                    List<string> ailist = InsertData(arlist, arlistf, "", "customer");
                    foreach (string sql in ailist)
                        sqlar.ExecuteNonQuery(sql);
                

            }
            catch (SQLiteException ex)
            {
                Log(ex.Message);
            }
          
        }

        private DataSet OleDBF2DT(string sql)
        {
            string connString = @"Provider=VFPOLEDB.1;Data Source="+mypath;
            DataSet ds = new DataSet();
            //string wh="(";
            //int ar=0;
            //DataTable dtr = (DataTable)gridControl1.DataSource;
            //DataRow[] drar = dtr.Select("Status='ทำงาน'");
            ////foreach(DataRow dr in drar)
            //foreach (DataRow dr in drar)
            //{
            //    wh += string.Format("CUSCODE='{0}'", dr["SHOPNAME"]);
            //    if (ar < drar.Count() - 1)
            //        wh += " OR ";
            //    ar += 1;
            //}
            //wh+=")";
            using (OleDbConnection dbcon = new OleDbConnection(connString))
            {
                using (OleDbCommand Cmd = new OleDbCommand(sql))
                {
                    Cmd.Connection = dbcon;

                    //  Cmd.Parameters.Add(new OleDbParameter("?", "ZA601"));

                    dbcon.Open();
                    IDataReader reader = Cmd.ExecuteReader();
                    ds.Load(reader, LoadOption.PreserveChanges, "stock_do"); 
                   
                }
                return ds;
            }
        
        
        }
        public static DataTable CompareTwoDataTable(DataTable dt1, DataTable dt2)
        {
            dt1.Merge(dt2);
            DataTable d3 = dt2.GetChanges();
            return d3;
        }
        private void ReadODBF(string filenames)
        {

            DataTable dt_so = new DataTable();
            DataTable dt = new DataTable();

            SQLiteDatabase itemmas;
            string mytime = dateEdit1.EditValue.ToString();
            string table;
            string sql_so = "SELECT stock_sn.ordernum, stock_do.cuscode, stock_do.stockdate, stock_do.stk_time, stock_sn.code, stock_sn.sn, stock_sn.quantity, stock_sn.sale_price FROM item_master, stock_sn, stock_do WHERE item_master.code = stock_sn.code AND stock_sn.quantity>0 AND stock_sn.ordernum = stock_do.ordernum ";
            string sql_rc = "SELECT stock_sn.ordernum, stock_do.cuscode, stock_do.stockdate, stock_do.stk_time, stock_sn.code, stock_sn.sn, stock_sn.quantity, stock_sn.sale_price FROM item_master, stock_sn, stock_do WHERE item_master.code = stock_sn.code AND stock_sn.quantity<0 AND stock_sn.ordernum = stock_do.ordernum ";

            int c = 0;
            int r = 1;
            try
            {
                mypath = textEdit1.Text;



                if (filenames.Contains("item_master"))
                {

                   // Log("step1");
                    dt = OleDBF2DT("select * from item_master").Tables[0];//ParseDBF.ReadDBF(@filenames).AsEnumerable().CopyToDataTable();
                    //Log("MasterItem Count:" + dt.Rows.Count.ToString());
                    itemmas = new SQLiteDatabase(@"DATA\itemmaster.db");
                    table = "bcitem";
                    //c = itemmas.GetDataTable(string.Format("select count(*) from {0}", table)).Rows.Count;
                    //Log("step2");
                    //var contacts = itemmas.GetDataTable(string.Format("select count(*) from {0}", table)).AsEnumerable().Except(dt.AsEnumerable(), DataRowComparer.Default);
                    DataTable dtlocal = itemmas.GetDataTable("select * from bcitem");
                    //Log("step2.1");
                    var qry1 = dt.AsEnumerable().Select(a => new { MobileNo = a["code"].ToString() });
                    var qry2 = dtlocal.AsEnumerable().Select(b => new { MobileNo = b["code"].ToString() });

                    var exceptAB = qry1.Except(qry2);
                    if (exceptAB.Count() > 0)
                    {
                        DataTable contacts = (from a in dt.AsEnumerable()
                                              join ab in exceptAB on a["code"].ToString() equals ab.MobileNo
                                              select a).CopyToDataTable();

                        //DataTable contacts = CompareTwoDataTable(dt, dtlocal);
                        Log("Item_master Count:" + contacts.Rows.Count.ToString());
                        
                        //if(contacts!=null)
                        //    if (contacts.Rows.Count > 0) Log("LocalItem Count:" + contacts.Rows.Count.ToString());
                       // foreach(DataColumn dc in contacts.Columns)
                       // Log(dc.ColumnName);
                        foreach (DataRow dr in contacts.Rows)
                        {
                            //Response.Write(row["ColX"]);
                            //}
                            //foreach (DataRow dr in dt.Rows)
                            //{
                            Dictionary<string, string> dic = new Dictionary<string, string>();


                            // if (r > c)
                            // {
                            
                            dic.Add("CODE", dr["CODE"].ToString());
                            dic.Add("FULLNAME", dr["FULLNAME"].ToString().Trim());
                            dic.Add("SHORTNAME", dr["SHORTNAME"].ToString().Trim());
                            dic.Add("ITEMTYPECO", dr["ITEMTYPECODE"].ToString());
                            dic.Add("BRANDCODE", dr["BRANDCODE"].ToString());
                            dic.Add("SUPPLIERCO", dr["SUPPLIERCODE"].ToString());
                            dic.Add("SPEC", dr["SPEC"].ToString());
                            dic.Add("SALEPRICE1", dr["SALEPRICE0"].ToString());
                            dic.Add("SALEPRICE2", dr["SALEPRICE1"].ToString());
                            dic.Add("SALEPRICE3", dr["SALEPRICE2"].ToString());
                            dic.Add("SALEPRICE4", dr["SALEPRICE3"].ToString());
                            dic.Add("SALEPRICE5", dr["SALEPRICE4"].ToString());
                            dic.Add("SALEPRICE6", dr["SALEPRICE5"].ToString());
                            dic.Add("SALEPRICE7", dr["SALEPRICE6"].ToString());
                            dic.Add("SALEPRICE8", dr["SALEPRICE7"].ToString());
                            dic.Add("SALEPRICE9", dr["SALEPRICE8"].ToString());
                            dic.Add("CH1", dr["CH1"].ToString());
                            dic.Add("CH2", dr["CH2"].ToString());
                            dic.Add("CH3", dr["CH3"].ToString());
                            dic.Add("CH4", dr["CH4"].ToString());
                            dic.Add("CH5", dr["CH5"].ToString());
                            dic.Add("sending", "0");



                            if (itemmas.GetDataTable(string.Format("select code from {1} where code='{0}'", dr["CODE"].ToString(), table)).Rows.Count == 0)
                            {
                                // Log("step3");
                                itemmas.Insert(table, dic);
                                //  Log("step4");
                                //c += 1;
                                //bgwitem.ReportProgress(r);
                                // itemcountLabel.Text = c.ToString();
                            }
                            else
                            {
                                if (itemmas.GetDataTable(string.Format("select code from {1} where code='{0}' and (saleprice1<>{2} or saleprice2<>{3} or saleprice3<>{4} or saleprice4<>{5} or saleprice5<>{6} or saleprice6<>{7} or saleprice7<>{8} or saleprice8<>{9} or saleprice9<>{10} or supplierco<>'{11}' or fullname<>'{12}' or shortname<>'{13}')",
                                    dr["code"].ToString(), table, dr["SALEPRICE0"].ToString(), dr["SALEPRICE1"].ToString(), dr["SALEPRICE2"].ToString(), dr["SALEPRICE3"].ToString(), dr["SALEPRICE4"].ToString(), dr["SALEPRICE5"].ToString(), dr["SALEPRICE6"].ToString(),
                                    dr["SALEPRICE7"].ToString(), dr["SALEPRICE8"].ToString(), dr["SALEPRICE9"].ToString(), dr["SUPPLIERCODE"].ToString(), dr["FULLNAME"].ToString().Trim(), dr["SHORTNAME"].ToString().Trim())).Rows.Count > 0)
                                {
                                    itemmas.Updatestr(table, dic, string.Format(" code='{0}'",dr["code"].ToString()));
                                    c += 1;
                                    itemcountLabel.Text = c.ToString();
                                }
                            
                            }

                            // }
                            r += 1;

                        }
                    }
                }else
                if(filenames.Contains("stock_do"))
                {

                    int ar = 0;
                    string wh = " AND (";
                    DataTable dtr = (DataTable)gridControl1.DataSource;
                    DataRow[] drar = dtr.Select("Status='ทำงาน'");
                    //foreach(DataRow dr in drar)
                    foreach (DataRow dr in drar)
                    {
                        wh += string.Format("CUSCODE='{0}'", dr["SHOPNAME"]);
                        if (ar < drar.Count() - 1)
                            wh += " OR ";
                        ar += 1;
                    }
                    ar = 0;
                    
                    wh = string.Format("{0}) AND STOCKDATE=datetime({1},{2},{3})", wh, DateTime.Parse(mytime).Year, DateTime.Parse(mytime).Month, DateTime.Parse(mytime).Day);//DateTime.Now.ToShortDateString());
                    sql_so = string.Format("{0} {1} {2}", sql_so, wh, "ORDER BY stock_sn.sn, stock_do.stk_time, stock_do.ordernum");
                    sql_rc = string.Format("{0} {1} {2}", sql_rc, wh, "ORDER BY stock_sn.sn, stock_do.stk_time, stock_do.ordernum");
                    Log(sql_so);
                    
                    dt_so = OleDBF2DT(sql_so).Tables[0];
                    Log("stock_do Rows:"+dt_so.Rows.Count.ToString());
                    //DataTable dt_rc = new DataTable();
                    //dt_rc = OleDBF2DT(sql_rc).Tables[0];
                    //var dtexcep = dt_so.AsEnumerable().Except(dt_rc.AsEnumerable(), DataRowComparer.Default);
                    itemmas = new SQLiteDatabase(@"DATA\bcapinvoicesub.db");
                    table = "bcapinvoicesub";
                    foreach (DataRow dr in dt_so.Rows)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();


                        // if (r > c)
                        // {
                        dic.Add("DOCNO", dr["ordernum"].ToString());
                        dic.Add("CUSCODE", dr["cuscode"].ToString());
                        dic.Add("APCODE",provinceCombo.EditValue.ToString());
                        dic.Add("DOCDATE", dr["stockdate"].ToString());
                        dic.Add("DOCTIME", dr["stk_time"].ToString());
                        dic.Add("ITEMCODE", dr["code"].ToString());
                        dic.Add("SERIALNO", dr["sn"].ToString().Trim());
                        dic.Add("QTY", dr["quantity"].ToString().Trim());
                        dic.Add("SALEPRICE", dr["sale_price"].ToString());


                        if (itemmas.GetDataTable(string.Format("select docno,itemcode from {3} where docno='{0}' and itemcode='{1}' and serialno='{2}'", dr["ordernum"].ToString().ToString(), dr["code"].ToString(),dr["sn"].ToString(), table)).Rows.Count == 0)
                        {
                            Log(string.Format("เพิ่ม sn:{0}",dr["sn"].ToString()));
                            itemmas.Insert(table, dic);
                            //c += 1;
                            //bgwstock.ReportProgress(ar);
                            // itemcountLabel.Text = c.ToString();
                        }
                        SQLiteDatabase idb=new SQLiteDatabase(@"DATA\itemmaster.db");
                        if(idb.GetDataTable(string.Format("select count(*) from bcitem where code='{0}'",dr["code"].ToString())).Rows.Count==0)
                        {
                            Log(string.Format(string.Format("select count(*) from bcitem where code='{0}'",dr["code"].ToString())));
                            dt = OleDBF2DT(string.Format("select * from item_master where code='{0}'",dr["code"].ToString())).Tables[0];
                            var contacts = dt.AsEnumerable();//.Intersect(itemmas.GetDataTable(string.Format("select count(*) from {0}", table)).AsEnumerable(), DataRowComparer.Default);
                            

                            foreach (DataRow drr in contacts)
                            {
                                //Response.Write(row["ColX"]);
                                //}
                                //foreach (DataRow dr in dt.Rows)
                                //{
                                Dictionary<string, string> dicc = new Dictionary<string, string>();


                              //  if (r > c)
                               // {

                                    dicc.Add("CODE", drr["CODE"].ToString());
                                    dicc.Add("FULLNAME", drr["FULLNAME"].ToString().Trim());
                                    dicc.Add("SHORTNAME", drr["SHORTNAME"].ToString().Trim());
                                    dicc.Add("ITEMTYPECO", drr["ITEMTYPECODE"].ToString());
                                    dicc.Add("BRANDCODE", drr["BRANDCODE"].ToString());
                               // Log("Before_SUPPLIERCO");
                                    dicc.Add("SUPPLIERCO", drr["SUPPLIERCODE"].ToString());
                               // Log("After_SUPPLIERCO");
                                    dicc.Add("SPEC", drr["SPEC"].ToString());
                                    dicc.Add("SALEPRICE1", drr["SALEPRICE0"].ToString());
                                    dicc.Add("SALEPRICE2", drr["SALEPRICE1"].ToString());
                                    dicc.Add("SALEPRICE3", drr["SALEPRICE2"].ToString());
                                    dicc.Add("SALEPRICE4", drr["SALEPRICE3"].ToString());
                                    dicc.Add("SALEPRICE5", drr["SALEPRICE4"].ToString());
                                    dicc.Add("SALEPRICE6", drr["SALEPRICE5"].ToString());
                                    dicc.Add("SALEPRICE7", drr["SALEPRICE6"].ToString());
                                    dicc.Add("SALEPRICE8", drr["SALEPRICE7"].ToString());
                                    dicc.Add("SALEPRICE9", drr["SALEPRICE8"].ToString());
                                    dicc.Add("CH1", drr["CH1"].ToString());
                                    dicc.Add("CH2", drr["CH2"].ToString());
                                    dicc.Add("CH3", drr["CH3"].ToString());
                                    dicc.Add("CH4", drr["CH4"].ToString());
                                    dicc.Add("CH5", drr["CH5"].ToString());
                                    dicc.Add("SENDING", "0");



                                   // if (itemmas.GetDataTable(string.Format("select code from {1} where code='{0}'", dr["CODE"].ToString(), table)).Rows.Count == 0)
                                    //{
                                        idb.Insert("bcitem", dicc);
                                        //c += 1;
                                        //bgwitem.ReportProgress(r);
                                        // itemcountLabel.Text = c.ToString();
                                    //}

                              //  }
                              //  r += 1;

                            }
                        }
                        ar += 1;

                    }

                }else
                    if (filenames.Contains("menu_list"))
                    {
                        DataTable dtmenu = new DataTable();
                        dtmenu = OleDBF2DT(string.Format("select * from menu_list where assign_dat=datetime({0},{1},{2})", DateTime.Parse(mytime).Year, DateTime.Parse(mytime).Month, DateTime.Parse(mytime).Day)).Tables[0];
                    }

            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            //AND (stock_do.cuscode = '7WYZI') AND (stock_do.stockdate = datetime(2013, 5, 8))";

        }


        //private void ReadODBF(string filenames)
        //{

        //    DataTable dt_so = new DataTable();
        //    DataTable dt = new DataTable();
            
        //    SQLiteDatabase itemmas;
        //    string mytime = dateEdit1.EditValue.ToString();
        //    string table;
        //    string sql_so = "SELECT stock_sn.ordernum, stock_do.cuscode, stock_do.stockdate, stock_do.stk_time, stock_sn.code, stock_sn.sn, stock_sn.quantity, stock_sn.sale_price FROM item_master, stock_sn, stock_do WHERE item_master.code = stock_sn.code AND stock_sn.quantity>0 AND stock_sn.ordernum = stock_do.ordernum ";
        //    string sql_rc = "SELECT stock_sn.ordernum, stock_do.cuscode, stock_do.stockdate, stock_do.stk_time, stock_sn.code, stock_sn.sn, stock_sn.quantity, stock_sn.sale_price FROM item_master, stock_sn, stock_do WHERE item_master.code = stock_sn.code AND stock_sn.quantity<0 AND stock_sn.ordernum = stock_do.ordernum ";
            
        //    int c = 0;
        //    int r = 1;
        //    try
        //    {
        //        mypath = textEdit1.Text;



        //        if (filenames.Contains("item_master"))
        //        {
        //            bgwitem.WorkerReportsProgress = true;
        //            if (!bgwitem.IsBusy)
        //                bgwitem.RunWorkerAsync();
        //        }
        //        else if(filenames.Contains("stock_do"))
        //        {
        //            bgwstock.WorkerReportsProgress = true;
        //            if (!bgwstock.IsBusy)
        //                bgwstock.RunWorkerAsync();
        //        }

        //        bgwitem.DoWork += delegate(object send, DoWorkEventArgs args)
        //        {
                    
        //                 dt = OleDBF2DT("select * from item_master").Tables[0];//ParseDBF.ReadDBF(@filenames).AsEnumerable().CopyToDataTable();
        //                        itemmas = new SQLiteDatabase(@"DATA\itemmaster.db");
        //                        table = "bcitem";
        //                        c = itemmas.GetDataTable(string.Format("select count(*) from {0}", table)).Rows.Count;
        //                        var contacts = dt.AsEnumerable().Intersect(itemmas.GetDataTable(string.Format("select count(*) from {0}", table)).AsEnumerable(), DataRowComparer.Default);


        //                        foreach (DataRow dr in contacts)
        //                        {
        //                            //Response.Write(row["ColX"]);
        //                        //}
        //                        //foreach (DataRow dr in dt.Rows)
        //                        //{
        //                         Dictionary<string, string> dic = new Dictionary<string, string>();


        //                            if (r > c)
        //                            {

        //                                dic.Add("CODE", dr["CODE"].ToString());
        //                                dic.Add("FULLNAME", dr["FULLNAME"].ToString().Trim());
        //                                dic.Add("SHORTNAME", dr["SHORTNAME"].ToString().Trim());
        //                                dic.Add("ITEMTYPECO", dr["ITEMTYPECODE"].ToString());
        //                                dic.Add("BRANDCODE", dr["BRANDCODE"].ToString());
        //                                dic.Add("SPEC", dr["SPEC"].ToString());
        //                                dic.Add("SALEPRICE1", dr["SALEPRICE0"].ToString());
        //                                dic.Add("SALEPRICE2", dr["SALEPRICE1"].ToString());
        //                                dic.Add("SALEPRICE3", dr["SALEPRICE2"].ToString());
        //                                dic.Add("SALEPRICE4", dr["SALEPRICE3"].ToString());
        //                                dic.Add("SALEPRICE5", dr["SALEPRICE4"].ToString());
        //                                dic.Add("SALEPRICE6", dr["SALEPRICE5"].ToString());
        //                                dic.Add("SALEPRICE7", dr["SALEPRICE6"].ToString());
        //                                dic.Add("SALEPRICE8", dr["SALEPRICE7"].ToString());
        //                                dic.Add("SALEPRICE9", dr["SALEPRICE8"].ToString());
        //                                dic.Add("CH1", dr["CH1"].ToString());
        //                                dic.Add("CH2", dr["CH2"].ToString());
        //                                dic.Add("CH3", dr["CH3"].ToString());
        //                                dic.Add("CH4", dr["CH4"].ToString());
        //                                dic.Add("CH5", dr["CH5"].ToString());



        //                                if (itemmas.GetDataTable(string.Format("select code from {1} where code='{0}'", dr["CODE"].ToString(), table)).Rows.Count == 0)
        //                                {
        //                                    itemmas.Insert(table, dic);
        //                                    //c += 1;
        //                                    bgwitem.ReportProgress(r);
        //                                    // itemcountLabel.Text = c.ToString();
        //                                }

        //                            }
        //                            r += 1;

        //                        }

                    
                   
        //        };

        //        bgwitem.ProgressChanged += delegate(object send, ProgressChangedEventArgs args)
        //        {
        //            // itemcountLabel.Text = args.ProgressPercentage.ToString();
        //            UpdateLabel("starting...");
        //        };

        //        bgwitem.RunWorkerCompleted += delegate(object send, RunWorkerCompletedEventArgs args)
        //        {
        //            UpdateLabel("item Complete");
        //            step1 = 1;
                    

        //        };
                
        //            bgwstock.DoWork += delegate(object send, DoWorkEventArgs args)
        //            {

        //                int ar = 0;
        //                string wh = " AND (";
        //                DataTable dtr = (DataTable)gridControl1.DataSource;
        //                DataRow[] drar = dtr.Select("Status='ทำงาน'");
        //                //foreach(DataRow dr in drar)
        //                foreach (DataRow dr in drar)
        //                {
        //                    wh += string.Format("CUSCODE='{0}'", dr["SHOPNAME"]);
        //                    if (ar < drar.Count() - 1)
        //                        wh += " OR ";
        //                    ar += 1;
        //                }
        //                ar = 0;
                       
        //                wh = string.Format("{0}) AND STOCKDATE=datetime({1},{2},{3})",wh,DateTime.Parse(mytime).Year, DateTime.Parse(mytime).Month, DateTime.Parse(mytime).Day);//DateTime.Now.ToShortDateString());
        //                sql_so = string.Format("{0} {1} {2}",sql_so, wh, "ORDER BY stock_sn.sn, stock_do.stk_time, stock_do.ordernum");
        //                sql_rc = string.Format("{0} {1} {2}", sql_rc, wh, "ORDER BY stock_sn.sn, stock_do.stk_time, stock_do.ordernum");
        //                dt_so = OleDBF2DT(sql_so).Tables[0];
        //                DataTable dt_rc = new DataTable();
        //                dt_rc=OleDBF2DT(sql_rc).Tables[0];
        //                var dtexcep = dt_so.AsEnumerable().Except(dt_rc.AsEnumerable(), DataRowComparer.Default);
        //                itemmas = new SQLiteDatabase(@"DATA\bcapinvoicesub.db");
        //                table = "bcapinvoicesub";
        //                foreach (DataRow dr in dtexcep)
        //                {
        //                    Dictionary<string, string> dic = new Dictionary<string, string>();


        //                    // if (r > c)
        //                    // {
        //                    dic.Add("DOCNO", dr["ordernum"].ToString());
        //                    dic.Add("CUSCODE", dr["cuscode"].ToString());
        //                    dic.Add("DOCDATE", dr["stockdate"].ToString());
        //                    dic.Add("DOCTIME", dr["stk_time"].ToString());
        //                    dic.Add("ITEMCODE", dr["code"].ToString());
        //                    dic.Add("SERIALNO", dr["sn"].ToString().Trim());
        //                    dic.Add("QTY", dr["quantity"].ToString().Trim());
        //                    dic.Add("SALEPRICE", dr["sale_price"].ToString());


        //                    if (itemmas.GetDataTable(string.Format("select docno,itemcode from {2} where docno='{0}' and itemcode='{1}'", dr["ordernum"].ToString().ToString(), dr["code"].ToString(), table)).Rows.Count == 0)
        //                    {
        //                        itemmas.Insert(table, dic);
        //                        //c += 1;
        //                        bgwstock.ReportProgress(ar);
        //                        // itemcountLabel.Text = c.ToString();
        //                    }

        //                    //  }
        //                    ar += 1;

        //                }

        //            };
        //            bgwstock.ProgressChanged += delegate(object send, ProgressChangedEventArgs args)
        //            {
        //                // itemcountLabel.Text = args.ProgressPercentage.ToString();
        //                UpdateLabel("starting...");
        //            };

        //            bgwstock.RunWorkerCompleted += delegate(object send, RunWorkerCompletedEventArgs args)
        //            {
        //                UpdateLabel("Stock Complete");
        //                step1 = 1;

        //            };
                
        //    }
        //    catch (Exception ex)
        //    { 
            
        //    }
        //    //AND (stock_do.cuscode = '7WYZI') AND (stock_do.stockdate = datetime(2013, 5, 8))";

        //}

        //private void ReadDBF(string filenames)
        //{

        //    DataTable dt=new DataTable();
        //    int c = 0;
        //    //SQLiteDatabase itemmas = new SQLiteDatabase(@"DATA\itemmaster.db");
        //    SQLiteDatabase itemmas;
        //    string mytime = dateEdit1.EditValue.ToString();
        //    string table;
        //    int r = 1;
        //    string strcontain;
        //    try
        //    {
        //                mypath = textEdit1.Text;
                       
        //                bgw.WorkerReportsProgress = true;
        //                if(!bgw.IsBusy)
        //                bgw.RunWorkerAsync();
        //                bgw.DoWork += delegate(object send, DoWorkEventArgs args)
        //                {
        //                   // itemcountLabel.Text = c.ToString();
        //                   // foreach (var vals in bcitem)

        //                        //this.Invoke(new MethodInvoker(delegate
        //                        //{
        //                        //    Log(string.Format("Reading .... {0}", filenames));
        //                        //}));
        //                    string sql = "";
        //                    if (filenames.Contains("item_master"))
        //                    {
        //                        //Log(string.Format("{0}", filenames));
        //                        r = 1;
        //                        dt = OleDBF2DT("select * from item_master").Tables[0];//ParseDBF.ReadDBF(@filenames).AsEnumerable().CopyToDataTable();
        //                        itemmas = new SQLiteDatabase(@"DATA\itemmaster.db");
        //                        table = "bcitem";
        //                        c = itemmas.GetDataTable(string.Format("select count(*) from ", table)).Rows.Count;
        //                        foreach (DataRow dr in dt.Rows)
        //                        {

        //                            Dictionary<string, string> dic = new Dictionary<string, string>();


        //                            if (r > c)
        //                            {

        //                                dic.Add("CODE", dr["CODE"].ToString());
        //                                dic.Add("FULLNAME", dr["FULLNAME"].ToString().Trim());
        //                                dic.Add("SHORTNAME", dr["SHORTNAME"].ToString().Trim());
        //                                dic.Add("ITEMTYPECO", dr["ITEMTYPECO"].ToString());
        //                                dic.Add("BRANDCODE", dr["BRANDCODE"].ToString());
        //                                dic.Add("SPEC", dr["SPEC"].ToString());
        //                                dic.Add("SALEPRICE1", dr["SALEPRICE0"].ToString());
        //                                dic.Add("SALEPRICE2", dr["SALEPRICE1"].ToString());
        //                                dic.Add("SALEPRICE3", dr["SALEPRICE2"].ToString());
        //                                dic.Add("SALEPRICE4", dr["SALEPRICE3"].ToString());
        //                                dic.Add("SALEPRICE5", dr["SALEPRICE4"].ToString());
        //                                dic.Add("SALEPRICE6", dr["SALEPRICE5"].ToString());
        //                                dic.Add("SALEPRICE7", dr["SALEPRICE6"].ToString());
        //                                dic.Add("SALEPRICE8", dr["SALEPRICE7"].ToString());
        //                                dic.Add("SALEPRICE9", dr["SALEPRICE8"].ToString());
        //                                dic.Add("CH1", dr["CH1"].ToString());
        //                                dic.Add("CH2", dr["CH2"].ToString());
        //                                dic.Add("CH3", dr["CH3"].ToString());
        //                                dic.Add("CH4", dr["CH4"].ToString());
        //                                dic.Add("CH5", dr["CH5"].ToString());



        //                                if (itemmas.GetDataTable(string.Format("select code from {1} where code='{0}'", dr["CODE"].ToString(), table)).Rows.Count == 0)
        //                                {
        //                                    itemmas.Insert(table, dic);
        //                                    //c += 1;
        //                                    bgw.ReportProgress(r);
        //                                    // itemcountLabel.Text = c.ToString();
        //                                }

        //                            }
        //                            r += 1;

        //                        }

        //                    }
        //                    else 
        //                        if (filenames.Contains("stock_do"))
        //                        {
        //                            Log(string.Format("{0}", filenames));
        //                            int ar = 0;
        //                            string wh = "(";
        //                            DataTable dtr = (DataTable)gridControl1.DataSource;
        //                            DataRow[] drar = dtr.Select("Status='ทำงาน'");
        //                            //foreach(DataRow dr in drar)
        //                            foreach (DataRow dr in drar)
        //                            {
        //                                wh += string.Format("CUSCODE='{0}'", dr["SHOPNAME"]);
        //                                if (ar < drar.Count() - 1)
        //                                    wh += " OR ";
        //                                ar += 1;
        //                            }
        //                            ar = 0;
        //                            wh += string.Format(") AND STOCKDATE='{0}'",ConvertDateTh(mytime));//DateTime.Now.ToShortDateString());
        //                            //Log(string.Format("Where:{0} ",wh));
        //                            DataTable dt_do = new DataTable();
        //                            dt_do.Columns.Add(new DataColumn("ORDERNUM", typeof(string)));
        //                            dt_do.Columns.Add(new DataColumn("STOCKDATE", typeof(string)));
        //                            dt_do.Columns.Add(new DataColumn("STK_TIME", typeof(string)));
        //                            dt_do.Columns.Add(new DataColumn("CUSCODE", typeof(string)));

        //                            //Log(string.Format("Read:{0} Rows", ParseDBF.ReadDBF(filenames).AsEnumerable().CopyToDataTable().Rows.Count));
        //                           // if (ParseDBF.ReadDBF(filenames).AsEnumerable().CopyToDataTable().Rows.Count > 0)

        //                            if(OleDBF2DT(string.Format("select * from stock_do {0}",wh)).Tables[0].Rows.Count>0)
        //                            {
        //                                dt_do = OleDBF2DT(string.Format("select * from stock_do {0}", wh)).Tables[0];//ParseDBF.ReadDBF(filenames).AsEnumerable().CopyToDataTable();
        //                               // DataRow[] rowin=dtwh.Select(wh);
        //                                int rr = 0;
                                   
                                        
        //                               // foreach (DataRow dj in rowin)
        //                               //     dt_do.ImportRow(dj);


        //                                // DataTable dt_dtl=ParseDBF.ReadDBF("stock_dtl.dbf").AsEnumerable().CopyToDataTable();
        //                                //DataTable dt_sn = new DataTable();
        //                                //dt_sn.Columns.Add(new DataColumn("ORDERNUM", typeof(string)));
        //                                //dt_sn.Columns.Add(new DataColumn("CODE", typeof(string)));
        //                                //dt_sn.Columns.Add(new DataColumn("SN", typeof(string)));
        //                                //dt_sn.Columns.Add(new DataColumn("QUANTITY", typeof(string)));
        //                                //dt_sn.Columns.Add(new DataColumn("SALE_PRICE", typeof(string)));

        //                                //foreach (DataRow dr in rowin)
        //                                //{
        //                                //    string inwh = string.Format("ORDERNUM='{0}'", dr["ORDERNUM"]);
        //                                //    DataRow[] drr = ParseDBF.ReadDBF(string.Format(@"{0}\stock_sn.dbf", mypath)).AsEnumerable().CopyToDataTable().Select(inwh);
        //                                //    foreach (DataRow dj in drr)
        //                                //        dt_sn.ImportRow(dj);


        //                                //}
        //                                itemmas = new SQLiteDatabase(@"DATA\bcapinvoicesub.db");
        //                                table = "bcapinvoicesub";
        //                                c = itemmas.GetDataTable(string.Format("select count(*) from {0}", table)).Rows.Count;
        //                                var result = from t1 in dt_do.Rows.Cast<DataRow>()
        //                                             join t2 in dt_sn.Rows.Cast<DataRow>()
        //                                             on t1.ItemArray[0] equals t2.ItemArray[0]
        //                                             select new
        //                                             {
        //                                                 DOCNO = t1.ItemArray[0],
        //                                                 CUSCODE = t1.ItemArray[3],
        //                                                 DOCDATE = t1.ItemArray[1],
        //                                                 DOCTIME = t1.ItemArray[2],
        //                                                 ITEMCODE = t2.ItemArray[1],
        //                                                 SERIALNO = t2.ItemArray[2],
        //                                                 QTY = t2.ItemArray[3],
        //                                                 SALEPRICE = t2.ItemArray[4],
        //                                             };


        //                                foreach (var dr in result)
        //                                {
        //                                    Dictionary<string, string> dic = new Dictionary<string, string>();


        //                                    // if (r > c)
        //                                    // {
        //                                    dic.Add("DOCNO", dr.DOCNO.ToString());
        //                                    dic.Add("CUSCODE", dr.CUSCODE.ToString());
        //                                    dic.Add("DOCDATE", dr.DOCDATE.ToString());
        //                                    dic.Add("DOCTIME", dr.DOCTIME.ToString());
        //                                    dic.Add("ITEMCODE", dr.ITEMCODE.ToString());
        //                                    dic.Add("SERIALNO", dr.SERIALNO.ToString().Trim());
        //                                    dic.Add("QTY", dr.QTY.ToString().Trim());
        //                                    dic.Add("SALEPRICE", dr.SALEPRICE.ToString());


        //                                    if (itemmas.GetDataTable(string.Format("select docno,itemcode from {2} where docno='{0}' and itemcode='{1}'", dr.DOCNO.ToString(), dr.ITEMCODE.ToString(), table)).Rows.Count == 0)
        //                                    {
        //                                        itemmas.Insert(table, dic);
        //                                        //c += 1;
        //                                        bgw.ReportProgress(ar);
        //                                        // itemcountLabel.Text = c.ToString();
        //                                    }

        //                                    //  }
        //                                    ar += 1;

        //                                }
        //                            }
        //                        }
                            
                              

                          
        //                };
        //                bgw.ProgressChanged += delegate(object send, ProgressChangedEventArgs args)
        //                {
        //                    // itemcountLabel.Text = args.ProgressPercentage.ToString();
        //                    UpdateLabel("starting...");
        //                };

        //                bgw.RunWorkerCompleted += delegate(object send, RunWorkerCompletedEventArgs args)
        //                {
        //                    UpdateLabel("Complete");
        //                    step1 = 1;
                            
        //                };
                    
        //            //case 1:
        //            //break;
        //            //case 2:
        //            //break;
                
                
        //        #region ITEMTYPECO
              
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        // Do something with ex
        //        Log(ex.Message);
        //    }
            
        //}
        

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (simpleButton2.Text == "ไม่ทำงาน")
                simpleButton2.Text = "ทำงาน";
            else
                simpleButton2.Text = "ไม่ทำงาน";
            
        }

        private void iGetData_Load(object sender, EventArgs e)
        {
           // time.Start();
           // if(bgw.IsBusy!=true)
           // bgw.RunWorkerAsync();
            FileWatcher();
            simpleButton6_Click(null, null);

        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            if (simpleButton5.Text == "ไม่ทำงาน")
                simpleButton5.Text = "ทำงาน";
            else
                simpleButton5.Text = "ไม่ทำงาน";
            
        }

        private void gridControl2_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode==Keys.Tab)
            {
               
                e.Handled = false;
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();

            }
        }

    

        private void iGetData_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control)
            {
                if (MessageBox.Show("Delete row?", "Confirmation", MessageBoxButtons.YesNo) !=
                  DialogResult.Yes)
                    return;
                GridView view = sender as GridView;
                view.DeleteRow(view.FocusedRowHandle);
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control)
            {
                if (MessageBox.Show("Delete row?", "Confirmation", MessageBoxButtons.YesNo) !=
                  DialogResult.Yes)
                    return;
                GridView view = sender as GridView;
                view.DeleteRow(view.FocusedRowHandle);
            }
        }

        private void gridView2_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            ColumnView View = sender as ColumnView;
            View.SetRowCellValue(e.RowHandle, View.Columns["ServerId"],View.RowCount);
            //View.SetRowCellValue(e.RowHandle, View.Columns["CreatedDate"], DateTime.Today);
        }

        private void gridView1_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            ColumnView View = sender as ColumnView;
            View.SetRowCellValue(e.RowHandle, View.Columns["RowId"], View.RowCount);
        }

        private void iGetData_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //this.Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "Muli Shop Server Hidden";
                notifyIcon1.BalloonTipText = "Your application has been minimized to the taskbar.";
                notifyIcon1.ShowBalloonTip(3000);
                this.ShowInTaskbar = false;
            }
        }
        
        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            //this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            this.ShowInTaskbar = true;
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
           // if (simpleButton5.Text == "ไม่ทำงาน")
           // {
                filelist = new List<string>(){mypath+@"\item_master.dbf",mypath+@"\stock_do.dbf"};
                isbusy = true;
               // foreach (string filename in filelist)
               //     ReadODBF(filename);
                StartTask();
           // }
                //DataTable dip = (DataTable)gridControl2.DataSource;
                //foreach (DataRow dr in dip.Rows)
                //{
                //    UploadInvoice(dr["ipaddress"].ToString());
                //    UploadItem(dr["ipaddress"].ToString()); 
                //}
        }

        private string ConvertDateEng(string datet)
        {
            string[] temp = datet.Split('/');
            
            string day="";
            string mont="";
            string years="";
            if (temp[0].Length == 1) day = "0" + temp[0]; else day = temp[0];
            if (temp[1].Length == 1) mont = "0" + temp[1]; else mont = temp[1];
            years = (Convert.ToInt16(temp[2]) - 543).ToString(); ;
            return day + "/" + mont + "/" + years;
        }

        private string ConvertDateTh(string datet)
        {
            string[] temp = datet.Split('/');
            
            string day = "";
            string mont = "";
            string years = "";
            if (temp[0].Length == 1) day = "0" + temp[0]; else day = temp[0];
            if (temp[1].Length == 1) mont = "0" + temp[1]; else mont = temp[1];
            //mont = temp[1];
            years = temp[2];
            return day + "/" + mont + "/" + years;
        }
        private Boolean CheckDate(string date1, string date2)
        { 
            bool istrue=false;
            string[] temp1 = date1.Split('/');
            string[] temp2 = date2.Split('/');
            if (Convert.ToInt16(temp1[0]) == Convert.ToInt16(temp2[0]) && Convert.ToInt16(temp1[1]) == Convert.ToInt16(temp2[1]) && Convert.ToInt16(temp1[2]) == Convert.ToInt16(temp2[2].ToString().Substring(0, 4)))
                istrue = true;
                return istrue; 
        }

    

       
      
    }
    public class BCITEM
    { 
    public string CODE {get;set;}
    public string FULLNAME {get;set;}
    public string SHORTNAME {get;set;}
    public string ITEMTYPECO{get;set;}
    public string SUPPLIERCO {get;set;}
    public string BRANDCODE{get;set;}
    public string SPEC{get;set;}
    public string SALEPRICE1{get;set;}
    public string SALEPRICE2{get;set;}
    public string SALEPRICE3{get;set;}
    public string SALEPRICE4{get;set;}
    public string SALEPRICE5{get;set;}
    public string SALEPRICE6{get;set;}
    public string SALEPRICE7{get;set;}
    public string SALEPRICE8{get;set;}
    public string SALEPRICE9{get;set;}
    public string CH1{get;set;}
    public string CH2{get;set;}
    public string CH3{get;set;}
    public string CH4{get;set;}
    public string CH5 { get; set; }
    
    }
    public class BCAPINVOICESUB
    {
            public string DOCNO{get;set;}
            public string CUSCODE{get;set;}
            public string APCODE { get; set; }
            public string DOCDATE{get;set;}
            public string DOCTIME{get;set;}
            public string ITEMCODE{get;set;}
            public string SERIALNO{get;set;}
            public string QTY{get;set;}
            public string SALEPRICE { get; set; }


    }
}