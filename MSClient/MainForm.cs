using System;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using SmartLib.Helpers;
using System.Data;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;


namespace MultiShop
{
    public partial class MainForm : XtraForm
    {
        SQLiteDatabase db = null;
        public static appConfig _appconf;
        BackgroundWorker worker;
        System.Timers.Timer timmer;
        int _update = 0;
        DataTable dtup = new DataTable();
        TileItemFrame badge = new TileItemFrame();
        TileItemElement badgele = new TileItemElement();
        public MainForm()
        {
            InitializeComponent();
            this.Text = " Multi Shop V.1 BETA Build 20130820";
            loadConfig();
            badge.Elements.Add(badgele);
            //tileItem7.Frames.Add(badge);
            //tileItem7.FrameAnimationInterval = 2500;
            //tileItem7.StartAnimation();
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            timmer = new System.Timers.Timer(1000);
            timmer.Elapsed += new System.Timers.ElapsedEventHandler(timmer_Elapsed);
            timmer.Start();
            preLoad();

        }

        void timmer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
            if (!worker.IsBusy)
                worker.RunWorkerAsync();
        }
        private void preLoad()
        {

            SplashScreenManager.ShowForm(typeof(WaitForm1));
            dtup = UpdateStock.UploadItem("www.udon-it.com");
            if (dtup.Rows.Count > 0)
            {
               
                var upstock = new UpdateStock();
                upstock.dtup = dtup;
                upstock.serverip = "www.udon-it.com";
                for (int i = 0; i < dtup.Rows.Count - 1; i++)
                {
                    SplashScreenManager.Default.SetWaitFormCaption(string.Format("อัพเดตข้อมูล {0} รายการ",i));
                    upstock.updateItem(i);
                    System.Threading.Thread.Sleep(4000);
                }
            }
            else
            {
                SplashScreenManager.Default.SetWaitFormCaption("อัพเดตข้อมูล");
                System.Threading.Thread.Sleep(4000);
            }
            SplashScreenManager.CloseForm();
          
        }

        private Boolean preUpdate()
        {
            bool succ = false;

            return succ;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                // Log("Running...");
               // refreshItem();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // Log(ex.Message);
            }
        }

        private void Stocking_ItemClick(object sender, TileItemEventArgs e)
        {
            var msform = new MSClient.MSForm();
            msform.ShowDialog();
        }

        private void tileItem2_ItemClick(object sender, TileItemEventArgs e)
        {
            var chkstock = new CheckStock();
            chkstock.ShowDialog();
        }

        private void tileItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            this.Close();
        }

        private void tileItem4_ItemClick(object sender, TileItemEventArgs e)
        {
            var chkstockn = new CheckStockNet();
            chkstockn.ShowDialog();
        }

        private void tileItem3_ItemClick(object sender, TileItemEventArgs e)
        {
            //var saleform = new SaleForm();
            //saleform.ShowDialog();
            var saleform = new SaleSide();
            saleform.ShowDialog();
        }

        private void tileItem6_ItemClick(object sender, TileItemEventArgs e)
        {
            var setform = new Setting();
            setform.ShowDialog();
        }
        private void loadConfig()
        {
            db = new SQLiteDatabase(Setting.sqlite_dbname);
            _appconf = new appConfig();
            DataTable dconfig = db.GetDataTable("select companyname,address,telephone,province,taxid,taxrate from configuration");
            DataTable dsale = db.GetDataTable("select formtype,billlock,saleprice,costprice,gp,autopurch,docno,customer from saleconfig");
            DataTable ditem = db.GetDataTable("select iscost,ismin,islowcost,ispricechange from itemconfig");
            
            
            if (ditem.Rows.Count > 0)
            {
                _appconf.iscost = Convert.ToInt16(ditem.Rows[0]["iscost"]);
                _appconf.ismin =  Convert.ToInt16(ditem.Rows[0]["ismin"]);
                _appconf.islowcost = Convert.ToInt16(ditem.Rows[0]["islowcost"]);
                _appconf.ispricechange = Convert.ToInt16(ditem.Rows[0]["ispricechange"]);
            }
            if (dsale.Rows.Count > 0)
            {
                _appconf.formtype=dsale.Rows[0]["formtype"].ToString();
                _appconf.billlock = Convert.ToInt16(dsale.Rows[0]["billlock"]);
                _appconf.saleprice = Convert.ToInt16(dsale.Rows[0]["saleprice"]);
                _appconf.costprice = Convert.ToInt16(dsale.Rows[0]["costprice"]);
                _appconf.gp = Convert.ToInt16(dsale.Rows[0]["gp"]);
                _appconf.autopurch = Convert.ToInt16(dsale.Rows[0]["autopurch"]);
                _appconf.defdocno = dsale.Rows[0]["docno"].ToString();
                _appconf.defcustomer = dsale.Rows[0]["customer"].ToString();

            }
            if (dconfig.Rows.Count > 0)
            {
                _appconf.companyname=dconfig.Rows[0]["companyname"].ToString();
                _appconf.address = dconfig.Rows[0]["address"].ToString();
                _appconf.telephone = dconfig.Rows[0]["telephone"].ToString();
                _appconf.province= dconfig.Rows[0]["province"].ToString();
                _appconf.taxid = dconfig.Rows[0]["taxid"].ToString();
                _appconf.taxrate = dconfig.Rows[0]["taxrate"].ToString();

            }
            //vGridControl1.DataSource = dconfig;
            //vGridControl2.DataSource = dsale;
            //vGridControl3.DataSource = ditem;


        }

        private void tileItem7_ItemClick(object sender, TileItemEventArgs e)
        {
            //var upstock = new UpdateStock();
            //if(dtup.Rows.Count>0)
            //upstock.dtup = dtup;
            //upstock.serverip = "www.udon-it.com";
            //upstock.ShowDialog();
        }
      

        private void refreshItem()
        { 
            //tileItem7
            dtup = UpdateStock.UploadItem("www.udon-it.com");
            
            if (dtup.Rows.Count > 0)
                _update = dtup.Rows.Count;
            else
                _update = 0;
            //if (dt.Rows.Count.ToString().Length > 3)
            //    tileItem7.AppearanceItem.Normal.Font.Size -= 10;
            badgele.Text = string.Format("อัพเดต {0}",_update);
           
            
        }
    }
    public class appConfig
    {
       public string companyname {get;set;}
       public string address {get;set;}
       public string telephone{get;set;}
       public string province {get;set;}
       public string taxid {get;set;}
       public string taxrate { get; set; }
       public int iscost{get;set;}
       public int ismin {get;set;}
       public int islowcost {get;set;}
       public int ispricechange{get;set;}
       public string formtype{get;set;}
       public int billlock{get;set;}
       public int saleprice {get;set;}
       public int costprice{get;set;}
       public int gp{get;set;}
       public int autopurch { get; set; }
       public string defdocno { get; set; }
       public string defcustomer { get; set; }
    
    }
}
