using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Configuration;
using SmartLib.Helpers;
using System.IO;
using DevExpress.Utils;
using System.Data.SqlClient;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using System.Net;
using ServiceStack.Text;
using System.Data.OleDb;
using FastReport;
using FastReport.Utils;


namespace MultiShop
{
    public partial class SaleSide : DevExpress.XtraEditors.XtraForm
    {
        private static string instancename = ConfigurationManager.AppSettings["instanceName"].ToString();
        private static string username = ConfigurationManager.AppSettings["username"].ToString();
        private static string password = ConfigurationManager.AppSettings["password"].ToString();
        private static string dbname = ConfigurationManager.AppSettings["dbname"].ToString();
        private string strcon = string.Empty;
        private string cuscode = ConfigurationManager.AppSettings["cuscode"].ToString();
        private string nodeA = ConfigurationManager.AppSettings["nodeA"].ToString();
        private string nodeB = ConfigurationManager.AppSettings["nodeB"].ToString();
        private DataTable saleitemdt = new DataTable();
        private DataTable searchitemdt = new DataTable();
        private DataTable maindt = new DataTable();
        private DataTable MainDtreceipt;
        private SaveForm svform;
        private int rowedit = 0;
        private bool incvat = true;
        RecMoney recmon;
        Billing bill = null;
        SQLiteDatabase db = null;// = new SQLiteDatabase();
        private Boolean islip = true;
        Boolean reloadBill = false;
        Boolean ispop = false;
        Boolean savepopup = false;
        int level = 1;
        int lastindex1 = 0;
        int lastindex2 = 0;
        int lastindex3 = 0;
        int defaultSearch=0;
        Boolean defaultSrch=false;
        string _arcode = "";
        public static string sqlite_dbname = Path.GetDirectoryName(Application.ExecutablePath) + @"\DATA\invoice.s3db";
        public static string sqlite_menulist = Path.GetDirectoryName(Application.ExecutablePath) + @"\DATA\menu_list.db";
        public static string sqlite_item = Path.GetDirectoryName(Application.ExecutablePath) + @"\DATA\itemmaster.db";
        public SaleSide()
        {
            InitializeComponent();
            strcon = string.Format(@"Data Source={0}; Initial Catalog={1};User Id={2};Password={3};", instancename, dbname, username, password);
            listBoxControl1.DisplayMember = "MENUNAME";
            listBoxControl1.ValueMember = "POSITION";
            listBoxControl2.DisplayMember = "MENU_NAME";
            listBoxControl2.ValueMember = "MENU_ID";
            listBoxControl3.DisplayMember = "MENU_DTL_N";
            listBoxControl3.ValueMember = "MENU_ID";
            bill = new Billing();
            db = new SQLiteDatabase(sqlite_dbname);
            DocDate.EditValue = DateTime.Now.ToShortDateString();
            preSaleDt();
            gridPageControl1.EnterClose += new EventHandler(gridPageControl1_EnterClose);
            textEdit1.KeyDown += new KeyEventHandler(textEdit1_KeyDown);
            ArName.TextChanged += new EventHandler(ArName_TextChanged);
            //popupControlContainer2.CloseOnLostFocus = false;
            //popupControlContainer2.CloseOnOuterMouseClick = false;
            
            //textEdit1.Focus();
        }

        void ArName_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (ArName.Text.IndexOf(":") != -1)
            {
                _arcode = ArName.Text.Substring(0, ArName.Text.IndexOf(":"));
                if (getSingleRow(string.Format("select pricelevel from bcar where code='{0}'", _arcode), "pricelevel").Length > 0)
                    level = Convert.ToInt16(getSingleRow(string.Format("select pricelevel from bcar where code='{0}'", _arcode), "pricelevel"));
                if (saleitemdt.Rows.Count > 0)
                {
                    foreach (DataRow dr in saleitemdt.Rows)
                    {
                        string price = getSingleRow(string.Format("Select saleprice{0} from bcpricelist where itemcode='{1}' and getdate() between startdate and stopdate and saletype=0", level, dr[5]), "saleprice" + level);
                        dr[2] = price;
                        dr[3] = (Convert.ToDecimal(dr[0]) * Convert.ToDecimal(dr[2])).ToString();

                    }
                    upAmounLabel(Convert.ToDouble(saleitemdt.Compute("Sum(Amount)", string.Empty)).ToString("#,##0.00"));
                }
            }
           
        }

     

        void textEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();
            if(e.KeyCode==Keys.Enter)
            gridPageControl1.gridView1_KeyDown(sender, e);
            else
            if(e.KeyCode==Keys.Delete || e.KeyCode==Keys.Back)
                if(textEdit1.EditValue.ToString().Length>=0 && textEdit1.EditValue.ToString().Length>0)
                    defaultSearch+=1;
                else
                    if(defaultSearch>=2 && textEdit1.EditValue.ToString().Length==0)
                        {
                        defaultSrch=true;
                        defaultSearch=0;
                        }else if(defaultSrch)itemLoad(defaultSrch);
                
        }

        void gridPageControl1_EnterClose(object sender, EventArgs e)
        {
            DataRow dr=null;
            if (gridPageControl1.myTitle.Contains("ลูกค้า"))
            {
                dr = gridPageControl1.result;
                if (dr.ItemArray.Length > 0)
                { 
                    ArName.Text = string.Format("{0}:{1}", dr.ItemArray[0].ToString(), dr.ItemArray[1].ToString()); //_arcode = dr.ItemArray[0].ToString();
                    textEdit1.Text = string.Empty;
                    defaultSrch=true;
                    this.ActiveControl = textEdit1;
                }
            }
            else  if (gridPageControl1.myTitle.Contains("สินค้า"))
            {
                dr = gridPageControl1.result;
                string serailnumber="";
                if (dr.ItemArray.Length > 0)
                {
                    if (gridPageControl1.isserial)
                    {
                        for (int r = 6; r < dr.ItemArray.Length; r++)
                            if (dr[r].ToString() != string.Empty)
                                serailnumber = dr[r].ToString();
                                insertItem(dr,serailnumber);
                    }
                    else
                        insertItem(dr);
                    textEdit1.Text = string.Empty;
                    this.ActiveControl = textEdit1;
                }
                  //  MessageBox.Show(dr.ItemArray[0].ToString());
                    //ArName.Text = string.Format("{0} {1}", dr.ItemArray[0].ToString(), dr.ItemArray[1].ToString());
            }
            //MessageBox.Show(dr.ItemArray.Length.ToString());
            //throw new NotImplementedException();
        }

        private string getSingleRow(string sql, string field)
        {
            string result = "";
            using (SqlConnection scon = new SqlConnection(strcon))
            {
                if (scon.State == ConnectionState.Closed) scon.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, scon))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    result = ds.Tables[0].Rows[0][field].ToString();
                }
            }
            return result;
        }
        private void insertItem(DataRow dr)
        {
            
            var sel = saleitemdt.Select(string.Format("SaleItemName='{0}'", dr[1]));
            if (sel.Length > 0)
            {
                sel[0][0] = (Convert.ToDecimal(sel[0][0].ToString()) + 1);
                sel[0][3] = String.Format("{0:0,0.0}", (Convert.ToDecimal(sel[0][0].ToString()) * Convert.ToDecimal(sel[0][2].ToString())));

            }
            else
            {
                DataRow rows = saleitemdt.NewRow();
                rows[0] = 1;
                rows[1] = dr[1];
                if (getSingleRow(string.Format("select pricelevel from bcar where code='{0}'", _arcode), "pricelevel").Length > 0)
                    level = Convert.ToInt16(getSingleRow(string.Format("select pricelevel from bcar where code='{0}'", _arcode), "pricelevel"));
                else
                    level = MainForm._appconf.saleprice;
                
                switch (level)
                { 
                    case 1:
                         rows[2] = dr[5];
                         rows[3] = 1 * Convert.ToDouble(dr[5]);
                        break;
                    case 2:
                        rows[2] = dr[6];
                        rows[3] = 1 * Convert.ToDouble(dr[6]);
                        break;
                    case 3:
                        rows[2] = dr[7];
                        rows[3] = 1 * Convert.ToDouble(dr[7]);
                        break;
                    case 4:
                        rows[2] = dr[8];
                        rows[3] = 1 * Convert.ToDouble(dr[8]);
                        break;
                }
                if (checkPriceCost(dr.ItemArray[0].ToString(),rows[3].ToString()) && MainForm._appconf.islowcost == 1)
                { 
                    MessageBox.Show("สินค้าราคาขายต๋ำกว่าหรือเท่ากับทุน");
                    return;
                }
                
                rows[4] = dr[0];
                rows[5] = dr[0];
                rows[6] = dr[4];
                saleitemdt.Rows.Add(rows);


            }
            saleitemdt.AcceptChanges();
            upAmounLabel(Convert.ToDouble(saleitemdt.Compute("Sum(Amount)", string.Empty)).ToString("#,##0.00"));
        }
        private void loaderItem(DataRow dr)
        {

            var sel = saleitemdt.Select(string.Format("SaleItemName='{0}'", dr[1]));
            if (sel.Length > 0)
            {
                sel[0][0] = (Convert.ToDecimal(sel[0][0].ToString()) + 1);
                sel[0][3] = String.Format("{0:0,0.0}", (Convert.ToDecimal(sel[0][0].ToString()) * Convert.ToDecimal(sel[0][2].ToString())));

            }
            else
            {
                DataRow rows = saleitemdt.NewRow();
                rows[0] = dr[2];
                rows[1] = dr[1];
                rows[2] = dr[4];
                rows[3] = Convert.ToInt16(dr[2]) * Convert.ToDouble(dr[4]);
                rows[4] = dr[0];
                rows[5] = dr[0];
                rows[6] = dr[3];
                saleitemdt.Rows.Add(rows);


            }
            saleitemdt.AcceptChanges();
            upAmounLabel(Convert.ToDouble(saleitemdt.Compute("Sum(Amount)", string.Empty)).ToString("#,##0.00"));
        }
        private void insertItem(DataRow dr, string serialno)
        {
            int level = 1;
            var sel = saleitemdt.Select(string.Format("SaleItemName='{0}'", dr[1]));
            if (sel.Length > 0)
            {
                sel[0][0] = (Convert.ToDecimal(sel[0][0].ToString()) + 1);
                sel[0][3] = String.Format("{0:0,0.0}", (Convert.ToDecimal(sel[0][0].ToString()) * Convert.ToDecimal(sel[0][2].ToString())));

            }
            else
            {
                DataRow rows = saleitemdt.NewRow();
                rows[0] = 1;
                rows[1] = dr[1];
                if (getSingleRow(string.Format("select pricelevel from bcar where code='{0}'", _arcode), "pricelevel").Length > 0)
                    level = Convert.ToInt16(getSingleRow(string.Format("select pricelevel from bcar where code='{0}'", _arcode), "pricelevel"));
                else
                    level = MainForm._appconf.saleprice;

                switch (level)
                {
                    case 1:
                        rows[2] = dr[5];
                        rows[3] = 1 * Convert.ToDouble(dr[5]);
                        break;
                    case 2:
                        rows[2] = dr[6];
                        rows[3] = 1 * Convert.ToDouble(dr[6]);
                        break;
                    case 3:
                        rows[2] = dr[7];
                        rows[3] = 1 * Convert.ToDouble(dr[7]);
                        break;
                    case 4:
                        rows[2] = dr[8];
                        rows[3] = 1 * Convert.ToDouble(dr[8]);
                        break;
                }
                if (checkPriceCost(dr.ItemArray[0].ToString(), rows[3].ToString()) && MainForm._appconf.islowcost == 1)
                {
                    MessageBox.Show("สินค้าราคาขายต๋ำกว่าหรือเท่ากับทุน");
                    return;
                }
               
                rows[4] = serialno;
                rows[5] = dr[0];
                rows[6] = dr[4];
                saleitemdt.Rows.Add(rows);


            }
            saleitemdt.AcceptChanges();
            upAmounLabel(Convert.ToDouble(saleitemdt.Compute("Sum(Amount)", string.Empty)).ToString("#,##0.00"));
        }
        private bool checkPriceCost(string itemcode,string saleprice)
        {
            Boolean _istrue = false;
            if (saleprice == string.Empty) saleprice = "0.00";
            //string saleprice = getSingleRow(string.Format("select SalePrice{0} from BCITEM WHERE CODE='{1}'", MainForm._appconf.saleprice, itemcode), string.Format("SalePrice{0}", MainForm._appconf.saleprice));
            string costprice = getSingleRow(string.Format("select LastBuyPrice from BCITEM WHERE CODE='{1}'", MainForm._appconf.costprice, itemcode), string.Format("LastBuyPrice", MainForm._appconf.costprice));
            if (Convert.ToDouble(saleprice) - Convert.ToDouble(costprice) <= 0)
                _istrue = true;

            return _istrue;
        }
        private void upAmounLabel(string txt)
        {
            NumberAmount.Text = txt;
            //TotalAmount.Text = txt;
            //if (incvat)
            //{
            //    BeforeTaxAmount.Text = (Convert.ToDouble(txt) / 1.07).ToString("#,##0.00");
            //    TaxAmount.Text = (Convert.ToDouble(txt) - (Convert.ToDouble(txt) / 1.07)).ToString("#,##0.00");
            //}
            //else
            //{
            //    BeforeTaxAmount.Text = txt;
            //    TaxAmount.Text = "0.00";
            //}
            //AmountToWord();
            //SumCashAmount.Text = "0.00";
            //textSearch.Focus();
            ChangeAmount.Text = txt;
            RecieptAmount.Text = (Convert.ToDouble(txt) - (Convert.ToDouble(txt) / 1.07)).ToString("#,##0.00");
            TotalAmount.Text = (Convert.ToDouble(txt) / 1.07).ToString("#,##0.00"); 
            

        }
        private void ExitBar_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            this.Close();
        }

        private void preSaleDt()
        {
            saleitemdt.Columns.Add(new DataColumn("Qty", typeof(Int16)));
            saleitemdt.Columns.Add(new DataColumn("SaleItemName", typeof(String)));
            saleitemdt.Columns.Add(new DataColumn("Price", typeof(Double)));
            saleitemdt.Columns.Add(new DataColumn("Amount", typeof(Double)));
            saleitemdt.Columns.Add(new DataColumn("SerialNo", typeof(string)));
            saleitemdt.Columns.Add(new DataColumn("ItemCode", typeof(string)));
            saleitemdt.Columns.Add(new DataColumn("UnitCode", typeof(string)));
            AddColumn("รวม", "Amount", 25, false,gridView1);
            //AddColumn("ราคา", "Price", 25, false);
            AddColumn("รายการ", "SaleItemName", 50, false,gridView1);
            AddColumn("จำนวน", "Qty", 25, true,gridView1);

            gridControl1.DataSource = saleitemdt;
            preDocNoCombo();
            newDocno(MainForm._appconf.defdocno);
            preCustomer();
            /// load docno
            /// load default customer
            
        }
     
        //// custom function
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
                    DocNo.Properties.Items.Add(string.Format("{0}={1}", dr["goupcode"], dr["goupdesc"]));
            }
        }
        private void preCustomer()
        {
            using (SqlConnection scon = new SqlConnection(strcon))
            {
                if (scon.State == ConnectionState.Closed) scon.Open();
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(string.Format("select code,name1 from bcar where (name1 like '%{0}%') order by code ",MainForm._appconf.defcustomer), scon))
                {
                    da.Fill(dt);
                }
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    { ArName.Text = string.Format("{0} {1}", dr["code"], dr["name1"]); _arcode = dr["code"].ToString(); }
                }
                else
                {
                     using (SqlDataAdapter da = new SqlDataAdapter(string.Format("select code from bcar where code='AR-000' order by code "), scon))
                        {
                            da.Fill(dt);
                        }
                    if(dt.Rows.Count>0)
                        using (SqlCommand scom = new SqlCommand(string.Format("Update BCAr set Name1='{0}' where code='AR-000'", MainForm._appconf.defcustomer), scon))
                        {
                            scom.ExecuteNonQuery();
                        }
                    else
                    using (SqlCommand scom = new SqlCommand(string.Format("insert into bcar(code,name1)values('AR-000','{0}')",MainForm._appconf.defcustomer), scon))
                    {
                        scom.ExecuteNonQuery();
                    }
                    ArName.Text = string.Format("Ar-000 {0}",MainForm._appconf.defcustomer);
                    _arcode = "Ar-000";
                }
            }
        }
        int index;
        private void AddColumn(string caption, string fieldname, int width, bool edited,GridView gridview)
        {
            index++;
            DevExpress.XtraGrid.Columns.GridColumn column = new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Caption = caption,
                FieldName = fieldname,
                Width = width,

            };
          
            if (fieldname != "SaleItemName")
            {
                column.DisplayFormat.FormatType = FormatType.Numeric;
                column.DisplayFormat.FormatString = "#,##0.00";
                column.AppearanceCell.Font = new Font(gridView1.Appearance.Row.Font, FontStyle.Bold);
            }
            gridview.Columns.Add(column);
            column.VisibleIndex = 0;
        }
        private void newDocno(string docseltext)
        {
            DataTable dt = new DataTable();
            //ComboBoxEdit cbedit = sender as ComboBoxEdit;
           // string docseltext = "";

           // if (DocNo.Text.IndexOf("=") > 0)
           //     docseltext = DocNo.Text.Substring(0, DocNo.Text.IndexOf("="));
           // else
           //     docseltext = DocNo.Text;

            string docnotmp = "";
            int coun = 0;
            string docoun = "";
            using (SqlConnection scon = new SqlConnection(strcon))
            {
                if (scon.State == ConnectionState.Closed) scon.Open();
                SqlCommand scom = new SqlCommand(string.Format("select DocFormat from BCSysTRFormat where TransKey='BillingTransConfig'"), scon);
                dt = new DataTable();
                scom = new SqlCommand(string.Format("select DocFormat from BCSysTRFormat where TransKey='BillingTransConfig'"), scon);
                using (SqlDataAdapter da = new SqlDataAdapter(scom))
                {
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        docnotmp = dt.Rows[0][0].ToString();
                        docnotmp = docnotmp.Replace("@@@", docseltext);
                        docnotmp = docnotmp.Replace("YY", (Convert.ToInt16(DateTime.Now.Year.ToString()) + 543).ToString().Substring(2, 2));
                        docnotmp = docnotmp.Replace("MM", DateTime.Now.Month.ToString().Length < 2 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString());
                        docnotmp = docnotmp.Replace("DD", DateTime.Now.Day.ToString().Length < 2 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());

                    }

                }
                dt = new DataTable();
                scom = new SqlCommand(string.Format("select max(docno) from bcarinvoice where docno like '{0}%' and Year(Docdate)=Year(GetDate()) and Month(Docdate)=Month(GetDate()) and day(Docdate)=day(GetDate())", docseltext), scon);
                if (docseltext != string.Empty)
                    using (SqlDataAdapter da = new SqlDataAdapter(scom))
                    {
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                            if (dt.Rows[0][0].ToString() != "")
                            {
                                coun = Convert.ToInt16(dt.Rows[0][0].ToString().Substring(dt.Rows[0][0].ToString().Length - 4, 4)) + 1;
                                for (int i = 0; i < 5; i++) { docoun += "0"; }
                                docoun = string.Format("{0}{1}", docoun, coun);
                                DocNo.Text = docnotmp.Replace("####", docoun.Substring(docoun.Length - 4));

                            }
                            else
                            {
                                DocNo.Text = docnotmp.Replace("####", "0001");

                            }
                    }

            }

        }
        private void newDocno(Object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            DataTable dt = new DataTable();
            ComboBoxEdit cbedit = sender as ComboBoxEdit;
            string docseltext = "";

            if (DocNo.Text.IndexOf("=") > 0)
                docseltext = DocNo.Text.Substring(0, DocNo.Text.IndexOf("="));
            else
                docseltext = DocNo.Text;
            
            string docnotmp = "";
            int coun = 0;
            string docoun = "";
            using (SqlConnection scon = new SqlConnection(strcon))
            {
                if (scon.State == ConnectionState.Closed) scon.Open();
                SqlCommand scom = new SqlCommand(string.Format("select DocFormat from BCSysTRFormat where TransKey='BillingTransConfig'"), scon);
                dt = new DataTable();
                scom = new SqlCommand(string.Format("select DocFormat from BCSysTRFormat where TransKey='BillingTransConfig'"), scon);
                using (SqlDataAdapter da = new SqlDataAdapter(scom))
                {
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        docnotmp = dt.Rows[0][0].ToString();
                        docnotmp = docnotmp.Replace("@@@", docseltext);
                        docnotmp = docnotmp.Replace("YY", (Convert.ToInt16(DateTime.Now.Year.ToString()) + 543).ToString().Substring(2, 2));
                        docnotmp = docnotmp.Replace("MM", DateTime.Now.Month.ToString().Length < 2 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString());
                        docnotmp = docnotmp.Replace("DD", DateTime.Now.Day.ToString().Length < 2 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());

                    }

                }
                dt = new DataTable();
                scom = new SqlCommand(string.Format("select max(docno) from bcarinvoice where docno like '{0}%' and Year(Docdate)=Year(GetDate()) and Month(Docdate)=Month(GetDate()) and day(Docdate)=day(GetDate())", docseltext), scon);
                if (docseltext != string.Empty)
                    using (SqlDataAdapter da = new SqlDataAdapter(scom))
                    {
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                            if (dt.Rows[0][0].ToString() != "")
                            {
                                coun = Convert.ToInt16(dt.Rows[0][0].ToString().Substring(dt.Rows[0][0].ToString().Length - 4, 4)) + 1;
                                for (int i = 0; i < 5; i++) { docoun += "0"; }
                                docoun = string.Format("{0}{1}", docoun, coun);
                                DocNo.Text = docnotmp.Replace("####", docoun.Substring(docoun.Length - 4));
                              
                            }
                            else
                            {
                                DocNo.Text = docnotmp.Replace("####", "0001");
                                
                            }
                    }

            }

        }

        private void DocNo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            newDocno(sender, e);
            
        }

        private void CustomerBar_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            defaultSrch=false;
           customLoad();
        }

        private void customLoad()
        {
            gridPageControl1.Clear();
            gridPageControl1.myTitle = "ค้นหาลูกค้า";
            gridPageControl1.sqltext = "SELECT CODE,NAME1,BILLADDRESS FROM BCAR ";
            gridPageControl1.dt = new DataTable();
            gridPageControl1.dt.Columns.Add(new DataColumn("CODE", typeof(String)));
            gridPageControl1.dt.Columns.Add(new DataColumn("NAME1", typeof(String)));
            gridPageControl1.dt.Columns.Add(new DataColumn("BILLADDRESS", typeof(Double)));
            //gridPageControl1.dt.Columns.Add(new DataColumn("ARCODE", typeof(String)));
            //gridPageControl1.dt.Columns.Add(new DataColumn("TOTALAMOUNT", typeof(Double)));
            gridPageControl1.setGrid(new GridColumn() { FieldName = "CODE", Caption = "รหัส", Name = "CODE", Visible = true, VisibleIndex = 0 }, FormatType.None);
            gridPageControl1.setGrid(new GridColumn() { FieldName = "NAME1", Caption = "ชื่อ", Name = "NAME1", Visible = true, VisibleIndex = 1 }, FormatType.None);
            gridPageControl1.setGrid(new GridColumn() { FieldName = "BILLADDRESS", Caption = "ที่อยู่", Name = "BILLADDRESS", Visible = true, VisibleIndex = 2 }, FormatType.None);
            //gridPageControl1.setGrid(new GridColumn() { FieldName = "ARCODE", Caption = "ลูกค้า", Name = "ARCODE", Visible = true, VisibleIndex = 0 }, FormatType.None);
            //gridPageControl1.setGrid(new GridColumn() { FieldName = "TOTALAMOUNT", Caption = "ยอดรวม", Name = "TOTALAMOUNT", Visible = true, VisibleIndex = 0 }, FormatType.Numeric, "n");
            gridPageControl1.wheretxt = new string[] { "CODE", "NAME1" };
            gridPageControl1.conditionf = new string[0];
            gridPageControl1.conditionv = new string[0];
            gridPageControl1.strcon = strcon;
            gridPageControl1.preData();
            gridPageControl1.Loading();
        
        }
        private void itemLoad(bool search)
        {
            gridPageControl1.Clear();
            gridPageControl1.myTitle = "ค้นหาสินค้า";
            //gridPageControl1.sqltext = string.Format("SELECT DISTINCT BCITEM.CODE,NAME1,STOCKQTY,BCITEMUNIT.NAME AS UNITNAME,SALEPRICE{1}*((100+{2})/100) AS SALEPRICE FROM BCITEM,BCITEMUNIT  WHERE  ((BCITEM.CODE  LIKE '%{0}%' ) OR (NAME1  LIKE '%{0}%' ) OR (BCITEMUNIT.NAME  LIKE '%{0}%' )) AND BCITEM.DEFSTKUNITCODE=BCITEMUNIT.CODE AND  BCITEM.ACTIVESTATUS <>2 AND BCITEM.ACTIVESTATUS <>3  AND  (BCITEM.Name1 Like '%{0}%' Or BCITEM.Code Like '%{0}%') ORDER BY BCITEM.CODE", textSearch.Text.ToString(),MainForm._appconf.saleprice,MainForm._appconf.gp)
            gridPageControl1.sqltext = string.Format("select code,name1,stockqty,remainoutqty,(select name from bcitemunit where bcitemunit.code=deffixunitcode) AS unitname,saleprice1*((100+{0})/100) as saleprice1,saleprice2*((100+{0})/100) as saleprice2,saleprice3*((100+{0})/100) as saleprice3,saleprice4*((100+{0})/100) as saleprice4 from bcitem", MainForm._appconf.gp);
            gridPageControl1.sqltext2 = string.Format("select code,name1,stockqty,remainoutqty,(select name from bcitemunit where bcitemunit.code=deffixunitcode) AS unitname,saleprice1*((100+{0})/100) as saleprice1,saleprice2*((100+{0})/100) as saleprice2,saleprice3*((100+{0})/100) as saleprice3,saleprice4*((100+{0})/100) as saleprice4", MainForm._appconf.gp);
            gridPageControl1.dt = new DataTable();
            gridPageControl1.dt.Columns.Add(new DataColumn("code", typeof(String)));
            gridPageControl1.dt.Columns.Add(new DataColumn("name1", typeof(String)));
            gridPageControl1.dt.Columns.Add(new DataColumn("stockqty", typeof(Double)));
            gridPageControl1.dt.Columns.Add(new DataColumn("remainoutqty", typeof(Double)));
            gridPageControl1.dt.Columns.Add(new DataColumn("unitname", typeof(String)));
            gridPageControl1.dt.Columns.Add(new DataColumn("saleprice1", typeof(Double)));
            gridPageControl1.dt.Columns.Add(new DataColumn("saleprice2", typeof(Double)));
            gridPageControl1.dt.Columns.Add(new DataColumn("saleprice3", typeof(Double)));
            gridPageControl1.dt.Columns.Add(new DataColumn("saleprice4", typeof(Double)));
                    

            //gridPageControl1.dt.Columns.Add(new DataColumn("ARCODE", typeof(String)));
            //gridPageControl1.dt.Columns.Add(new DataColumn("TOTALAMOUNT", typeof(Double)));
            gridPageControl1.setGrid(new GridColumn() { FieldName = "code", Caption = "รหัส", Name = "CODE", Visible = true, VisibleIndex = 0 }, FormatType.None);
            gridPageControl1.setGrid(new GridColumn() { FieldName = "name1", Caption = "ชื่อ", Name = "NAME1", Visible = true, VisibleIndex = 1 }, FormatType.None);
            gridPageControl1.setGrid(new GridColumn() { FieldName = "stockqty", Caption = "จำนวนคงเหลือ", Name = "STOCKQTY", Visible = true, VisibleIndex = 2 }, FormatType.Numeric,"#,##0.00");
            gridPageControl1.setGrid(new GridColumn() { FieldName = "remainoutqty", Caption = "จอง", Name = "REMAINOUTQTY", Visible = true, VisibleIndex = 3 }, FormatType.Numeric, "#,##0.00");
            gridPageControl1.setGrid(new GridColumn() { FieldName = "unitname", Caption = "หน่วย", Name = "UNITNAME", Visible = true, VisibleIndex = 4 }, FormatType.None);
            gridPageControl1.setGrid(new GridColumn() { FieldName = "saleprice1", Caption = "ราคา1", Name = "SALEPRICE1", Visible = true, VisibleIndex = 5 }, FormatType.Numeric, "#,##0.00");
            gridPageControl1.setGrid(new GridColumn() { FieldName = "saleprice2", Caption = "ราคา2", Name = "SALEPRICE2", Visible = true, VisibleIndex = 6 }, FormatType.Numeric, "#,##0.00");
            gridPageControl1.setGrid(new GridColumn() { FieldName = "saleprice3", Caption = "ราคา3", Name = "SALEPRICE3", Visible = true, VisibleIndex = 7 }, FormatType.Numeric, "#,##0.00");
            gridPageControl1.setGrid(new GridColumn() { FieldName = "saleprice4", Caption = "ราคา4", Name = "SALEPRICE4", Visible = true, VisibleIndex = 8 }, FormatType.Numeric, "#,##0.00");
                    
            //gridPageControl1.setGrid(new GridColumn() { FieldName = "ARCODE", Caption = "ลูกค้า", Name = "ARCODE", Visible = true, VisibleIndex = 0 }, FormatType.None);
            //gridPageControl1.setGrid(new GridColumn() { FieldName = "TOTALAMOUNT", Caption = "ยอดรวม", Name = "TOTALAMOUNT", Visible = true, VisibleIndex = 0 }, FormatType.Numeric, "n");
            gridPageControl1.wheretxt = new string[] { "code", "name1" };
            gridPageControl1.wheretxt2 = new string[] { "ctrlno", "serialno", "registerno" };
            foreach(string val in gridPageControl1.wheretxt2)
                gridPageControl1.sqltext2+=string.Format(",{0}",val);
            gridPageControl1.sqltext2 += " from bcitem,bcserialmaster";

            gridPageControl1.jointable = new string[] { "code", "itemcode" };
            if(!search)
            {
            gridPageControl1.conditionf = new string[] { "vendercode" };
            gridPageControl1.conditionv = new string[] { listBoxControl3.SelectedValue.ToString() };
            }else
            {
                gridPageControl1.conditionf = new string[0];
                gridPageControl1.conditionv = new string[0];
            }


            gridPageControl1.strcon = strcon;
            gridPageControl1.preData();
            gridPageControl1.Loading();
            popupControlContainer1.HidePopup();
            ispop = false;
        
        }
        private void preItemGroup()
        {
            //gridView2.Columns.Clear();
            //DataTable dtcust=new DataTable();
            SQLiteDatabase dbs = new SQLiteDatabase(sqlite_menulist);
            //using (SqlConnection scon = new SqlConnection(strcon))
           // {
           
            DataTable dtcust = dbs.GetDataTable("select menuid,menuname,position from main_menu");
              
              // DataTable dtcust = dbs.GetDataTable("select menu_id,menu_name,position from menu_list where menu_alive='true' and menu_prove='true'");
              
                    if (dtcust.Rows.Count > 0)
                    {
                        this.listBoxControl1.DataSource = dtcust;
                        // listBoxControl1.Items.AddColumn("กลุ่มสินค้า");
                    }
                
          //  }

        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            
            gridPageControl1.textsearch = textEdit1.EditValue.ToString();
            if(!defaultSrch)
                gridPageControl1.Loading();
            else
                {
                    itemLoad(defaultSrch);
                }
        }

        private void ItemGroup_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            Control button = sender as Control;
            //Close the dropdown accepting the user's choice 

            if (!ispop)
            {
                preItemGroup();
                popupControlContainer1.ShowPopup(barManager1, this.PointToClient(new Point(((this.splitContainerControl1.Width/6)),
                          (this.splitContainerControl1.Height/4))));
                //popupControlContainer1.ShowPopup(barManager1, this.PointToClient(new Point(this.splitContainerControl1.Left+250,this.splitContainerControl1.Top+200)));
                ispop = true;
            }
            else
            {
                popupControlContainer1.HidePopup();
                ispop = false;
                //(button.Parent as PopupContainerControl).OwnerEdit.ClosePopup();
            }
                
            

        }

     

        private void exitBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaleSide_Load(object sender, EventArgs e)
        {
            itemLoad(true);
            this.ActiveControl = textEdit1;

        }

        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            SQLiteDatabase dbs = new SQLiteDatabase(sqlite_menulist);
            //using (SqlConnection scon = new SqlConnection(strcon))
            // {

            // DataTable dtdtl = dbs.GetDataTable(string.Format("select menu_list_dtl.menu_id,menu_dtl_n from menu_list_dtl,menu_list where menu_list.menu_id=menu_list_dtl.menu_id_i and menu_list.position='{0}' and menu_list.menu_alive='true' and menu_dtl_a='true' and menu_dtl_p='true'", listBoxControl1.SelectedValue));

            // DataTable dtcust = dbs.GetDataTable("select menu_id,menu_name,position from menu_list where menu_alive='true' and menu_prove='true'");
            if (listBoxControl1.SelectedValue != null)
            {
                DataTable dtdtl = dbs.GetDataTable(string.Format("select menu_id,menu_name from menu_list where main_id='{0}' order by show_id", listBoxControl1.SelectedValue));
                if (dtdtl.Rows.Count > 0)
                {
                    this.listBoxControl2.DataSource = dtdtl;
                    // listBoxControl1.Items.AddColumn("กลุ่มสินค้า");
                }
                // MessageBox.Show(listBoxControl1.SelectedValue.ToString());

                lastindex1 = listBoxControl1.SelectedIndex;
            }
        }

        private void listBoxControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SQLiteDatabase dbs = new SQLiteDatabase(sqlite_menulist);
            //using (SqlConnection scon = new SqlConnection(strcon))
            // {

            // DataTable dtdtl = dbs.GetDataTable(string.Format("select menu_list_dtl.menu_id,menu_dtl_n from menu_list_dtl,menu_list where menu_list.menu_id=menu_list_dtl.menu_id_i and menu_list.position='{0}' and menu_list.menu_alive='true' and menu_dtl_a='true' and menu_dtl_p='true'", listBoxControl1.SelectedValue));

            // DataTable dtcust = dbs.GetDataTable("select menu_id,menu_name,position from menu_list where menu_alive='true' and menu_prove='true'");
            if (listBoxControl2.SelectedValue != null)
            {
                DataTable dtdtl = dbs.GetDataTable(string.Format("select menu_id,menu_dtl_n from menu_list_dtl where menu_id_i='{0}' and menu_dtl_a='true' order by menu_id", listBoxControl2.SelectedValue));
                if (dtdtl.Rows.Count > 0)
                {
                    this.listBoxControl3.DataSource = dtdtl;
                    // listBoxControl1.Items.AddColumn("กลุ่มสินค้า");
                }
                // MessageBox.Show(listBoxControl1.SelectedValue.ToString());
                lastindex2 = listBoxControl2.SelectedIndex;
            }

        }

        private void listBoxControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            lastindex3 = listBoxControl3.SelectedIndex;
        }
       
      
        public BCITEM[] WebRequestGetJson(string url)
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
               // Log(ex.Message + ":Line219");
            }
            return cuscode;
        }

        private void BrowseBill_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            //updateSupplier();
            insertItem();
        }

        private void HoldBillBar_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            updateSupplier();
        }
        private void insertItem()
        {
            SQLiteDatabase db=new SQLiteDatabase(sqlite_item);
            try
            {
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.RunWorkerAsync();
                bgw.WorkerReportsProgress = true;
                bgw.DoWork += delegate(object send, DoWorkEventArgs es)
                {

                    BCITEM[] upsup = null;
                    int i = 0;
                    List<BCITEM> source = new List<BCITEM>();
                    DataTable dtsource = db.GetDataTable("select code from bcitem where code is not null");
                    foreach (DataRow dr in dtsource.Rows)
                    { 
                        BCITEM nrow=new BCITEM
                        {
                            CODE=(string)dr["code"]
                        };
                        source.Add(nrow);
                    }
                    List<BCITEM> dest = new List<BCITEM>();
                    
                    using (SqlConnection scon = new SqlConnection(strcon))
                    {
                        if (scon.State == ConnectionState.Closed) scon.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter("select code from bcitem", scon))
                        {
                            DataTable dts = new DataTable();
                            da.Fill(dts);
                            if (dts.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dts.Rows)
                                {
                                    BCITEM nrow = new BCITEM
                                    {
                                        CODE = (string)dr["code"]
                                    };
                                    dest.Add(nrow);
                                }

                            }
                        }
                    }
                    var result = source.Except(dest);
                    
                    
                    //if (dr["code"].ToString().Trim().Length > 0)
                    //{
                    //    upsup = WebRequestGetJson(string.Format("http://www.udon-it.com/multishop/vendorcode/{0}", dr["code"].ToString()));
                    //    if (upsup != null)
                    //        if (upsup[0].SUPPLIERCO.ToString().Trim().Length > 0)
                    //        {
                    //            SqlCommand comm = da.SelectCommand as SqlCommand;
                    //            comm.CommandText = string.Format("update bcitem set vendercode='{0}' where code='{1}'", upsup[0].SUPPLIERCO, dr["code"].ToString());
                    //            comm.ExecuteNonQuery();
                    //            //using (SqlCommand comm = new SqlCommand(string.Format("update bcitem set vendercode='{0}' where code='{1}'", upsup[0].SUPPLIERCO, dr["code"].ToString())))
                    //            //{
                    //            //comm.ExecuteNonQuery();
                    //            //}

                    //            bgw.ReportProgress(i);
                    //            i += 1;
                    //        }
                    //}
                };
                bgw.ProgressChanged += delegate(object send, ProgressChangedEventArgs es)
                {
                    NumberAmount.Text = es.ProgressPercentage.ToString();
                };
                bgw.RunWorkerCompleted += delegate(object send, RunWorkerCompletedEventArgs es)
                {
                    NumberAmount.Text = "Completeed...";
                };
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void updateSupplier()
        {
            try
            {
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.RunWorkerAsync();
                bgw.WorkerReportsProgress = true;
                bgw.DoWork += delegate(object send, DoWorkEventArgs es)
                {

                    BCITEM[] upsup = null;
                    int i = 0;
                    using (SqlConnection scon = new SqlConnection(strcon))
                    {
                        if (scon.State == ConnectionState.Closed) scon.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter("select code from bcitem where vendercode is null", scon))
                        {
                            DataTable dts = new DataTable();
                            da.Fill(dts);
                            if (dts.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dts.Rows)
                                {
                                    if (dr["code"].ToString().Trim().Length > 0)
                                    {
                                        upsup = WebRequestGetJson(string.Format("http://www.udon-it.com/multishop/vendorcode/{0}", dr["code"].ToString()));
                                        if (upsup != null)
                                            if (upsup[0].SUPPLIERCO.ToString().Trim().Length > 0)
                                            {
                                                SqlCommand comm = da.SelectCommand as SqlCommand;
                                                comm.CommandText = string.Format("update bcitem set vendercode='{0}' where code='{1}'", upsup[0].SUPPLIERCO, dr["code"].ToString());
                                                comm.ExecuteNonQuery();
                                                //using (SqlCommand comm = new SqlCommand(string.Format("update bcitem set vendercode='{0}' where code='{1}'", upsup[0].SUPPLIERCO, dr["code"].ToString())))
                                                //{
                                                //comm.ExecuteNonQuery();
                                                //}

                                                bgw.ReportProgress(i);
                                                i += 1;
                                            }
                                    }
                                }

                            }
                        }
                    }
                };
                bgw.ProgressChanged += delegate(object send, ProgressChangedEventArgs es)
                {
                    NumberAmount.Text = es.ProgressPercentage.ToString();
                };
                bgw.RunWorkerCompleted += delegate(object send, RunWorkerCompletedEventArgs es)
                {
                    NumberAmount.Text = "Completeed...";
                };
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listBoxControl3_Click(object sender, EventArgs e)
        {
            if (listBoxControl3.SelectedValue != null)
                if (lastindex3 != listBoxControl3.SelectedIndex)
                {
                    defaultSrch=false;
                    itemLoad(defaultSrch);
                  
                }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (NumberAmount.Text.Length > 0)
            {
                if (Convert.ToDouble(NumberAmount.Text) > 0)
                {
                    recmon = new RecMoney();
                    recmon.Amount = Convert.ToDouble(NumberAmount.Text);
                    recmon.init();
                    recmon.FormClose += new EventHandler(recmon_FormClose);
                    initDataTable();
                    recmon.ShowDialog();

                  //  textEditAmount.Text = NumberAmount.Text;
                  //  if (!savepopup)
                  //  {
                  //      savepopup = true;
                  //      popupControlContainer2.ShowPopup(barManager1, this.PointToClient(new Point(this.splitContainerControl1.Left + 450, this.splitContainerControl1.Top + 200)));
                       
                  //      textEditCash.Focus();
                  //  }
                    //svform = new SaveForm();
                    //svform.CloseSave += new EventHandler(svform_CloseSave);
                    //svform.Amount = Convert.ToDouble(NumberAmount.Text);
                    //svform.ArCode = _arcode;
                    //svform.strconn = strcon;
                    //if (MainDtreceipt == null || MainDtreceipt.Rows.Count == 0)
                    //{
                       

                    //}

                    //svform.initDataTable(MainDtreceipt);

                    
                    //if (svform.Amount > 0)
                    //    svform.ShowDialog();

                }
                else
                    MessageBox.Show("ยอดจ่ายไม่เท่ากับยอดรับเงิน!", "เตือน", MessageBoxButtons.OK);
            }
            else
                MessageBox.Show("ยอดจ่ายไม่เท่ากับยอดรับเงิน!", "เตือน", MessageBoxButtons.OK);
        }
        private void PrintingBill(string formname = "slip.frx")
        {
            try
            {
                Report fr = new Report();//{ AutoFillDataSet = true, FileName = "slip.frx"};
                DataTable dtreport = new DataTable();
                //dtreport = dataTable.Clone();

                dtreport.Columns.Clear();
                dtreport = saleitemdt;
                string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                //string pathReport=string.Format(@"{0}lip.frx",AppDomain.CurrentDomain.BaseDirectory);
                foreach (DataRow dr in dtreport.Rows)
                {
                    if (dr.Field<string>("SaleItemName").Length < 25)
                    {
                        dr.BeginEdit();
                        dr.SetField<string>("SaleItemName", dr.Field<string>("SaleItemName"));
                        dr.EndEdit();
                    }
                }
                if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ") formname = "slip.frx";
                fr.Load(appPath + "\\" + formname);
                if (!saleitemdt.Columns.Contains("ArCode"))
                {
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "ArCode" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "DocNo" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "DocDate" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "DocTime" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "TaxId" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "TotalAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "TaxAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "MachineNo" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "NetAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "DiscountAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "CreditAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "CouponAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "CashAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "ChangeAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "CompanyName" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "Address" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "Telephone" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "Province" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "Taxid" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "rows" });

                }
                //fr.Pages[0].Height = Units.Centimeters * 10f;

                ReportPage page1 = fr.Pages[0] as ReportPage;
                PageFooterBand pfb;
                //if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                pfb = page1.PageFooter as PageFooterBand;
                //ReportSummaryBand rsb = fr.Pages[0] as ReportSummaryBand; 
                DataBand data1 = new DataBand();
                DataBand data2 = new DataBand();
                // if (islip)
                // {
                // if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                // {
                page1.ReportSummary = new ReportSummaryBand();

                page1.ReportSummary.CreateUniqueName();
                page1.ReportSummary.Height = 2f * Units.Millimeters;
                //  }
                data2.Name = "Data2";
                data1.Name = "Data1";
                data1.MaxRows = 0;
                data1.KeepTogether = true;

                //data1.DataSource = fr.GetDataSource("Reciept");
                data2.DataSource = fr.GetDataSource("Reciept");
                //data2.Height = Units.Centimeters * 2f;
                //page1.Bands.Add();


                //TextObject text1 = new TextObject();
                //text1.Name = "text1";
                //text1.Text = "[Reciept.qty]";
                //text1.HorzAlign = HorzAlign.Center;
                //text1.Font = new Font("Tahoma", 8);

                //text1.Bounds = new RectangleF(Units.Centimeters * 0.25f, Units.Centimeters * 0.25f, Units.Centimeters * 1, Units.Centimeters * 0.5f);
                //data1.Objects.Add(text1);

                //TextObject text2 = new TextObject();
                //text2.Name = "text2";
                //text2.Bounds = new RectangleF(Units.Centimeters * 1.25f, Units.Centimeters * 0.25f, Units.Centimeters * 5, Units.Centimeters * 0.5f);
                //text2.Text = "[[Reciept.Name1].Substring(0,15)]....   @[FormatNumber([Reciept.SalePrice],2)]";
                //text2.HorzAlign = HorzAlign.Left;
                //text2.Font = new Font("Tahoma", 8);
                //data1.Objects.Add(text2);


                //TextObject text3 = new TextObject();
                //text3.Name = "text1";
                //text3.Bounds = new RectangleF(Units.Centimeters * 6f, Units.Centimeters * 0.25f, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                //text3.Text = "[FormatNumber([Reciept.amount],2)]";
                //text3.HorzAlign = HorzAlign.Right;
                //text3.Font = new Font("Tahoma", 8);
                //data1.Objects.Add(text3);



                //data2.Height = (Units.Centimeters * 5f);


                //GroupFooterBand footer1 = new GroupFooterBand();
                fr.RegisterData(dtreport, "Reciept");
                dtreport.Rows[0]["ArCode"] = _arcode;
                dtreport.Rows[0]["DocNo"] = DocNo.Text;
                dtreport.Rows[0]["DocDate"] = DocDate.Text;
                dtreport.Rows[0]["DocTime"] = DateTime.Now.ToShortTimeString();
                dtreport.Rows[0]["TaxId"] = DocNo.Text;
                dtreport.Rows[0]["TotalAmount"] = TotalAmount.Text;
                dtreport.Rows[0]["CashAmount"] = Convert.ToDouble(bill.cashReciept).ToString("#,##0.00");
                dtreport.Rows[0]["ChangeAmount"] = bill.cashChange;
                dtreport.Rows[0]["CreditAmount"] = bill.creditReciept;
                dtreport.Rows[0]["CouponAmount"] = bill.couponReciept;
                dtreport.Rows[0]["DiscountAmount"] = bill.sumDiscount;
                dtreport.Rows[0]["MachineNo"] = Environment.MachineName;
                dtreport.Rows[0]["Companyname"] = MainForm._appconf.companyname;
                dtreport.Rows[0]["Address"] = MainForm._appconf.address;
                dtreport.Rows[0]["Telephone"] = MainForm._appconf.telephone;
                dtreport.Rows[0]["Province"] = MainForm._appconf.province;
                dtreport.Rows[0]["Taxid"] = MainForm._appconf.taxid;
                dtreport.Rows[0]["rows"] = dtreport.Rows.Count;
                fr.AutoFillDataSet = true;


                LineObject line1 = new LineObject();
                line1.Width = Units.Centimeters * 7.5f;
                line1.Top = Units.Centimeters * 0.45f;
                line1.Left = Units.Centimeters * 0.25f;
                //data2.Objects.Add(line1);
                if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                    page1.ReportSummary.Objects.Add(line1);

                float ytop = 0.55f;
                float xleft1 = 0.25f;
                float xleft2 = 5.8f;
                TextObject text4 = new TextObject();
                text4.Name = "text4";
                text4.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                text4.Text = "ยอดสุทธิ";
                text4.HorzAlign = HorzAlign.Right;
                text4.Font = new Font("Tahoma", 8);
                //data2.Objects.Add(text4);
                if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                    page1.ReportSummary.Objects.Add(text4);

                TextObject text5 = new TextObject();
                text5.Name = "text5";
                text5.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                text5.Text = dtreport.Rows[0]["TotalAmount"].ToString();//[FormatNumber([Reciept.TotalAmount],2)]";
                text5.HorzAlign = HorzAlign.Right;
                text5.Font = new Font("Tahoma", 8);
                //data2.Objects.Add(text5);
                if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                    page1.ReportSummary.Objects.Add(text5);
                ytop += 0.40f;

                TextObject text6 = new TextObject();
                text6.Name = "text6";
                text6.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                text6.Text = "รับเงินสด";
                text6.HorzAlign = HorzAlign.Right;
                text6.Font = new Font("Tahoma", 8);
                //data2.Objects.Add(text6);
                if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                    page1.ReportSummary.Objects.Add(text6);
                TextObject text7 = new TextObject();
                text7.Name = "text7";
                text7.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                text7.Text = dtreport.Rows[0]["CashAmount"].ToString();// "[FormatNumber([Reciept.CashAmount],2)]";
                text7.HorzAlign = HorzAlign.Right;
                text7.Font = new Font("Tahoma", 8);
                //data2.Objects.Add(text7);
                if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                    page1.ReportSummary.Objects.Add(text7);
                ytop += 0.40f;

                TextObject text11 = new TextObject();
                text11.Name = "text11";
                text11.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                text11.Text = "ส่วนลด";
                text11.HorzAlign = HorzAlign.Right;
                text11.Font = new Font("Tahoma", 8);
                //data2.Objects.Add(text11);
                if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                    page1.ReportSummary.Objects.Add(text11);

                TextObject text12 = new TextObject();
                text12.Name = "text12";
                text12.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                text12.Text = dtreport.Rows[0]["DiscountAmount"].ToString();// "[FormatNumber([Reciept.DiscountAmount],2)]";
                text12.HorzAlign = HorzAlign.Right;
                text12.Font = new Font("Tahoma", 8);
                //data2.Objects.Add(text12);
                if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                    page1.ReportSummary.Objects.Add(text12);

                if (Convert.ToDecimal(bill.couponReciept) > 0)
                {
                    ytop += 0.40f;
                    TextObject text13 = new TextObject();
                    text13.Name = "text13";
                    text13.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text13.Text = "บัตรเครดิต";
                    text13.HorzAlign = HorzAlign.Right;
                    text13.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text13);
                    if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                        page1.ReportSummary.Objects.Add(text13);
                    TextObject text14 = new TextObject();
                    text14.Name = "text14";
                    text14.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text14.Text = dtreport.Rows[0]["CreditAmount"].ToString();// "[FormatNumber([Reciept.CreditAmount],2)]";
                    text14.HorzAlign = HorzAlign.Right;
                    text14.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text14);
                    if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                        page1.ReportSummary.Objects.Add(text14);
                }
                if (Convert.ToDecimal(bill.couponReciept) > 0)
                {
                    ytop += 0.40f;
                    TextObject text15 = new TextObject();
                    text15.Name = "text15";
                    text15.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text15.Text = "คูปอง";
                    text15.HorzAlign = HorzAlign.Right;
                    text15.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text15);
                    if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                        page1.ReportSummary.Objects.Add(text15);
                    TextObject text16 = new TextObject();
                    text16.Name = "text16";
                    text16.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text16.Text = dtreport.Rows[0]["CouponAmount"].ToString();// "[FormatNumber([Reciept.CouponAmount],2)]";
                    text16.HorzAlign = HorzAlign.Right;
                    text16.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text16);
                    if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                        page1.ReportSummary.Objects.Add(text16);
                }

                ytop += 0.40f;
                TextObject text8 = new TextObject();
                text8.Name = "text8";
                text8.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                text8.Text = "เงินทอน";
                text8.HorzAlign = HorzAlign.Right;
                text8.Font = new Font("Tahoma", 8);
                //data2.Objects.Add(text8);
                if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                    page1.ReportSummary.Objects.Add(text8);
                TextObject text9 = new TextObject();
                text9.Name = "text9";
                text9.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                text9.Text = dtreport.Rows[0]["ChangeAmount"].ToString();// "[FormatNumber([Reciept.ChangeAmount],2)]";
                text9.HorzAlign = HorzAlign.Right;
                text9.Font = new Font("Tahoma", 8);
                //data2.Objects.Add(text9);
                if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                    page1.ReportSummary.Objects.Add(text9);
                ytop += 0.40f;
                TextObject text10 = new TextObject();
                text10.Name = "text10";
                text10.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 7f, Units.Centimeters * 0.5f);
                text10.Text = "ราคาสินค้ารวมภาษีมูลค่าเพิ่มแล้ว ";
                text10.HorzAlign = HorzAlign.Center;
                text10.Font = new Font("Tahoma", 9, FontStyle.Bold);
                //data2.Objects.Add(text10);
                if (MainForm._appconf.formtype == "ใบเสร็จอย่างย่อ")
                    page1.ReportSummary.Objects.Add(text10);
                //data2.Height = Units.Centimeters * ytop;

                //}
                //else
                //{
                //    fr.RegisterData(dtreport, "Reciept");
                //    dtreport.Rows[0]["ArCode"] = ArCode.Text;
                //    dtreport.Rows[0]["DocNo"] = DocNo.Text;
                //    dtreport.Rows[0]["DocDate"] = DocDate.Text;
                //    dtreport.Rows[0]["DocTime"] = DateTime.Now.ToShortTimeString();
                //    dtreport.Rows[0]["TaxId"] = DocNo.Text;
                //    dtreport.Rows[0]["TotalAmount"] = TotalAmount.Text;
                //    dtreport.Rows[0]["CashAmount"] = Convert.ToDouble(bill.cashReciept).ToString("#,##0.00");
                //    dtreport.Rows[0]["ChangeAmount"] = bill.cashChange;
                //    dtreport.Rows[0]["CreditAmount"] = bill.creditReciept;
                //    dtreport.Rows[0]["CouponAmount"] = bill.couponReciept;
                //    dtreport.Rows[0]["DiscountAmount"] = bill.sumDiscount;
                //    dtreport.Rows[0]["MachineNo"] = Environment.MachineName;
                //    fr.AutoFillDataSet = true;
                //}

                //page1.PaperHeight = data1.Height + page1.PageHeader.Height + page1.PageFooter.Height;
                //page1.ReportTitle = new ReportTitleBand();
                //page1.ReportTitle.Name = "ReportTitle1";
                //page1.ReportTitle.Height = Units.Centimeters * 1.5f;

                //TextObject text1 = new TextObject();
                //text1.Name = "Text1";
                //text1.Bounds = new RectangleF(0, 0, Units.Centimeters * 19, Units.Centimeters * 1);

                //text1.Text = "PRODUCTS";
                //text1.Font = new Font("Tahoma", 14, FontStyle.Bold);

                //page1.ReportTitle.Objects.Add(text1);
                //page1.Bands.Add(data1);
                //page1.Height = data1.Height + data2.Height + Units.Centimeters * 5f;
                //data2.PrintOn = PrintOn.LastPage;
                //page1.Bands.Add(pfb);


                // page1.ReportSummary.Objects.Add(text10);

                fr.PrintSettings.Copies = 1;
                fr.PrintSettings.ShowDialog = false;
                //fr.PrintSettings.Printer = GlobalClass.printerDefault;
                //fr.PrintSettings.Printer = "Foxit Phantom Printer";// GlobalClass.printerDefault;

                //fr.Refresh();
                //if (GlobalClass.printerDefault != "ไม่กำหนด")
                fr.Show();
                ////else
                //fr.Print();
                //fr.Design();

            }
            catch (Exception ex)
            {
                SmartLib.Controls.DialogMessageBox.ShowBox(ex.Message, "<color=red>ผิดพลาด</color>", "ตกลง");
            }
        }
        private void PrintingBills(string formname = "slip.frx")
        {
            try
            {
                Report fr = new Report();//{ AutoFillDataSet = true, FileName = "slip.frx"};
                DataTable dtreport = new DataTable();
                //dtreport = dataTable.Clone();

                dtreport.Columns.Clear();
                dtreport = saleitemdt;//dataTable;
                string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                //string pathReport=string.Format(@"{0}lip.frx",AppDomain.CurrentDomain.BaseDirectory);
                foreach (DataRow dr in dtreport.Rows)
                {
                    if (dr.Field<string>("SaleItemName").Length < 25)
                    {
                        dr.BeginEdit();
                        dr.SetField<string>("SaleItemName", dr.Field<string>("SaleItemName"));
                        dr.EndEdit();
                    }
                }
                if (islip) formname = "slip.frx";
                fr.Load(appPath + "\\" + formname);
                //if (!dataTable.Columns.Contains("ArCode"))
                if (!saleitemdt.Columns.Contains("ArCode"))
                {
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "ArCode" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "DocNo" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "DocDate" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "DocTime" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "TaxId" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "TotalAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "TaxAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "MachineNo" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "NetAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "DiscountAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "CreditAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "CouponAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "CashAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "ChangeAmount" });
                    dtreport.Columns.Add(new DataColumn() { ColumnName = "CompanyName" });
                }
                DataBand data1 = new DataBand();
                DataBand data2 = new DataBand();
                dtreport.Rows[0]["ArCode"] = _arcode;
                dtreport.Rows[0]["DocNo"] = DocNo.Text;
                dtreport.Rows[0]["DocDate"] = DocDate.Text;
                dtreport.Rows[0]["DocTime"] = DateTime.Now.ToShortTimeString();
                dtreport.Rows[0]["TaxId"] = DocNo.Text;
                dtreport.Rows[0]["TotalAmount"] = TotalAmount.Text;
                dtreport.Rows[0]["CashAmount"] = Convert.ToDouble(bill.cashReciept).ToString("#,##0.00");
                dtreport.Rows[0]["ChangeAmount"] = bill.cashChange;
                dtreport.Rows[0]["CreditAmount"] = bill.creditReciept;
                dtreport.Rows[0]["CouponAmount"] = bill.couponReciept;
                dtreport.Rows[0]["DiscountAmount"] = bill.sumDiscount;
                dtreport.Rows[0]["MachineNo"] = Environment.MachineName;
                fr.AutoFillDataSet = true;


                //fr.Pages[0].Height = Units.Centimeters * 10f;
                if (islip)
                {
                    ReportPage page1 = fr.Pages[0] as ReportPage;
                    PageFooterBand pfb = page1.PageFooter as PageFooterBand;
                    //ReportSummaryBand rsb = fr.Pages[0] as ReportSummaryBand; 

                    page1.ReportSummary = new ReportSummaryBand();

                    page1.ReportSummary.CreateUniqueName();
                    page1.ReportSummary.Height = 4.0f * Units.Centimeters;

                    //data2.Name = "Data2";
                    //data1.Name = "Data1";
                    //data1.MaxRows = 0;
                    //data1.KeepTogether = true;

                    //data1.DataSource = fr.GetDataSource("Reciept");
                    //data2.DataSource = fr.GetDataSource("Reciept");
                    //data2.Height = Units.Centimeters * 2f;
                    //page1.Bands.Add();


                    //TextObject text1 = new TextObject();
                    //text1.Name = "text1";
                    //text1.Text = "[Reciept.qty]";
                    //text1.HorzAlign = HorzAlign.Center;
                    //text1.Font = new Font("Tahoma", 8);

                    //text1.Bounds = new RectangleF(Units.Centimeters * 0.25f, Units.Centimeters * 0.25f, Units.Centimeters * 1, Units.Centimeters * 0.5f);
                    //data1.Objects.Add(text1);

                    //TextObject text2 = new TextObject();
                    //text2.Name = "text2";
                    //text2.Bounds = new RectangleF(Units.Centimeters * 1.25f, Units.Centimeters * 0.25f, Units.Centimeters * 5, Units.Centimeters * 0.5f);
                    //text2.Text = "[[Reciept.Name1].Substring(0,15)]....   @[FormatNumber([Reciept.SalePrice],2)]";
                    //text2.HorzAlign = HorzAlign.Left;
                    //text2.Font = new Font("Tahoma", 8);
                    //data1.Objects.Add(text2);


                    //TextObject text3 = new TextObject();
                    //text3.Name = "text1";
                    //text3.Bounds = new RectangleF(Units.Centimeters * 6f, Units.Centimeters * 0.25f, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    //text3.Text = "[FormatNumber([Reciept.amount],2)]";
                    //text3.HorzAlign = HorzAlign.Right;
                    //text3.Font = new Font("Tahoma", 8);
                    //data1.Objects.Add(text3);



                    //data2.Height = (Units.Centimeters * 5f);


                    //GroupFooterBand footer1 = new GroupFooterBand();

                    fr.RegisterData(dtreport, "Reciept");


                    LineObject line1 = new LineObject();
                    line1.Width = Units.Centimeters * 7.5f;
                    line1.Top = Units.Centimeters * 0.45f;
                    line1.Left = Units.Centimeters * 0.25f;
                    //data2.Objects.Add(line1);
                    page1.ReportSummary.Objects.Add(line1);
                    float ytop = 0.55f;
                    float xleft1 = 0.25f;
                    float xleft2 = 5.8f;
                    TextObject text4 = new TextObject();
                    text4.Name = "text4";
                    text4.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text4.Text = "ยอดสุทธิ";
                    text4.HorzAlign = HorzAlign.Right;
                    text4.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text4);
                    page1.ReportSummary.Objects.Add(text4);

                    TextObject text5 = new TextObject();
                    text5.Name = "text5";
                    text5.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text5.Text = dtreport.Rows[0]["TotalAmount"].ToString();//[FormatNumber([Reciept.TotalAmount],2)]";
                    text5.HorzAlign = HorzAlign.Right;
                    text5.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text5);
                    page1.ReportSummary.Objects.Add(text5);
                    ytop += 0.40f;

                    TextObject text6 = new TextObject();
                    text6.Name = "text6";
                    text6.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text6.Text = "รับเงินสด";
                    text6.HorzAlign = HorzAlign.Right;
                    text6.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text6);
                    page1.ReportSummary.Objects.Add(text6);
                    TextObject text7 = new TextObject();
                    text7.Name = "text7";
                    text7.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text7.Text = dtreport.Rows[0]["CashAmount"].ToString();// "[FormatNumber([Reciept.CashAmount],2)]";
                    text7.HorzAlign = HorzAlign.Right;
                    text7.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text7);
                    page1.ReportSummary.Objects.Add(text7);
                    ytop += 0.40f;

                    TextObject text11 = new TextObject();
                    text11.Name = "text11";
                    text11.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text11.Text = "ส่วนลด";
                    text11.HorzAlign = HorzAlign.Right;
                    text11.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text11);
                    page1.ReportSummary.Objects.Add(text11);

                    TextObject text12 = new TextObject();
                    text12.Name = "text12";
                    text12.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text12.Text = dtreport.Rows[0]["DiscountAmount"].ToString();// "[FormatNumber([Reciept.DiscountAmount],2)]";
                    text12.HorzAlign = HorzAlign.Right;
                    text12.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text12);
                    page1.ReportSummary.Objects.Add(text12);

                    if (Convert.ToDecimal(bill.couponReciept) > 0)
                    {
                        ytop += 0.40f;
                        TextObject text13 = new TextObject();
                        text13.Name = "text13";
                        text13.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                        text13.Text = "บัตรเครดิต";
                        text13.HorzAlign = HorzAlign.Right;
                        text13.Font = new Font("Tahoma", 8);
                        //data2.Objects.Add(text13);
                        page1.ReportSummary.Objects.Add(text13);
                        TextObject text14 = new TextObject();
                        text14.Name = "text14";
                        text14.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                        text14.Text = dtreport.Rows[0]["CreditAmount"].ToString();// "[FormatNumber([Reciept.CreditAmount],2)]";
                        text14.HorzAlign = HorzAlign.Right;
                        text14.Font = new Font("Tahoma", 8);
                        //data2.Objects.Add(text14);
                        page1.ReportSummary.Objects.Add(text14);
                    }
                    if (Convert.ToDecimal(bill.couponReciept) > 0)
                    {
                        ytop += 0.40f;
                        TextObject text15 = new TextObject();
                        text15.Name = "text15";
                        text15.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                        text15.Text = "คูปอง";
                        text15.HorzAlign = HorzAlign.Right;
                        text15.Font = new Font("Tahoma", 8);
                        //data2.Objects.Add(text15);
                        page1.ReportSummary.Objects.Add(text15);
                        TextObject text16 = new TextObject();
                        text16.Name = "text16";
                        text16.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                        text16.Text = dtreport.Rows[0]["CouponAmount"].ToString();// "[FormatNumber([Reciept.CouponAmount],2)]";
                        text16.HorzAlign = HorzAlign.Right;
                        text16.Font = new Font("Tahoma", 8);
                        //data2.Objects.Add(text16);
                        page1.ReportSummary.Objects.Add(text16);
                    }

                    ytop += 0.40f;
                    TextObject text8 = new TextObject();
                    text8.Name = "text8";
                    text8.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text8.Text = "เงินทอน";
                    text8.HorzAlign = HorzAlign.Right;
                    text8.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text8);
                    page1.ReportSummary.Objects.Add(text8);
                    TextObject text9 = new TextObject();
                    text9.Name = "text9";
                    text9.Bounds = new RectangleF(Units.Centimeters * xleft2, Units.Centimeters * ytop, Units.Centimeters * 1.75f, Units.Centimeters * 0.5f);
                    text9.Text = dtreport.Rows[0]["ChangeAmount"].ToString();// "[FormatNumber([Reciept.ChangeAmount],2)]";
                    text9.HorzAlign = HorzAlign.Right;
                    text9.Font = new Font("Tahoma", 8);
                    //data2.Objects.Add(text9);
                    page1.ReportSummary.Objects.Add(text9);
                    ytop += 0.40f;
                    TextObject text10 = new TextObject();
                    text10.Name = "text10";
                    text10.Bounds = new RectangleF(Units.Centimeters * xleft1, Units.Centimeters * ytop, Units.Centimeters * 7f, Units.Centimeters * 0.5f);
                    text10.Text = "ราคาสินค้ารวมภาษีมูลค่าเพิ่มแล้ว ";
                    text10.HorzAlign = HorzAlign.Center;
                    text10.Font = new Font("Tahoma", 9, FontStyle.Bold);
                    //data2.Objects.Add(text10);
                    page1.ReportSummary.Objects.Add(text10);
                    //data2.Height = Units.Centimeters * ytop;



                    //page1.PaperHeight = data1.Height + page1.PageHeader.Height + page1.PageFooter.Height;
                    //page1.ReportTitle = new ReportTitleBand();
                    //page1.ReportTitle.Name = "ReportTitle1";
                    //page1.ReportTitle.Height = Units.Centimeters * 1.5f;

                    //TextObject text1 = new TextObject();
                    //text1.Name = "Text1";
                    //text1.Bounds = new RectangleF(0, 0, Units.Centimeters * 19, Units.Centimeters * 1);

                    //text1.Text = "PRODUCTS";
                    //text1.Font = new Font("Tahoma", 14, FontStyle.Bold);

                    //page1.ReportTitle.Objects.Add(text1);
                    //page1.Bands.Add(data1);
                    //page1.Height = data1.Height + data2.Height + Units.Centimeters * 5f;
                    //data2.PrintOn = PrintOn.LastPage;
                    //page1.Bands.Add(pfb);


                    // page1.ReportSummary.Objects.Add(text10);

                    fr.PrintSettings.Copies = 1;
                    fr.PrintSettings.ShowDialog = false;
                    //fr.PrintSettings.Printer = GlobalClass.printerDefault;
                    //fr.PrintSettings.Printer = "Foxit Phantom Printer";// GlobalClass.printerDefault;

                    //fr.Refresh();
                    //if (GlobalClass.printerDefault != "ไม่กำหนด")
                    //  fr.Show();
                    ////else
                    //    fr.Print();
                }
                fr.Design();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // SmartLib.Controls.DialogMessageBox.ShowBox(ex.Message, "<color=red>ผิดพลาด</color>", "ตกลง");
            }
        }
        void recmon_FormClose(object sender, EventArgs e)
        {
            if ( recmon.dtAmount.Rows.Count>0 && (double)recmon.dtAmount.Rows[0]["ChangeAmount"]>=0 && (double)recmon.dtAmount.Rows[0]["CashAmount"]>0)
            {
                if (MainDtreceipt != null)
                {
                RecieptAmount.Text = recmon.dtAmount.Rows[0]["CashAmount"].ToString();
                ChangeAmount.Text = recmon.dtAmount.Rows[0]["ChangeAmount"].ToString();
               // MainDtreceipt.Rows[0]["CashAmount"] = RecieptAmount.Text;
               // MainDtreceipt.Rows[0]["ChangeAmount"] = ChangeAmount.Text;
                saveComplete();
                }
                //PrintingBill();
                //PrintingBill("ใบเสร็จรับเงิน.frx");
                //clearScreen();
            }
            //else
            //    MessageBox.Show("ยอดจ่ายไม่เท่ากับยอดรับเงิน!", "เตือน", MessageBoxButtons.OK);
            //throw new NotImplementedException();
        }
        private void clearScreen()
        {
            DocNo.Text = string.Empty;
            SaleCode.EditValue = string.Empty;
            //DocDate.Text=string.Empty;
            ArName.Text = "AR-0001";
            //ArCode.Text = string.Empty;
            //BillAddress.Text = string.Empty;
            //Telephone.Text = string.Empty;
            //Department.Text = string.Empty;
            //ArCode.Text = string.Empty;
            //ArName.Text = string.Empty;
            NumberAmount.Text = "0.00";
            //thaiwordamount.Text = "ศูนย์บาท";
            TotalAmount.Text = "0.00";
            RecieptAmount.Text = "0.00";
            ChangeAmount.Text = "0.00";
            //TaxAmount.Text = "0.00";
            //BeforeTaxAmount.Text = "0.00";
            //SumCashAmount.Text = "0.00";
            //MyDescription.Text = string.Empty;

            saleitemdt.Rows.Clear();
            if (MainDtreceipt != null)
                if (MainDtreceipt.Rows.Count > 0)
                    MainDtreceipt.Rows.Clear();
            newDocno(MainForm._appconf.defdocno);
            
        }
        void saveComplete()
        {

          
                if (reloadBill)
                {
                    if (MainForm._appconf.billlock == 0)
                    {
                        //browseBar_LinkClicked(sender, e);
                        // delData();
                        insertHeadBill("0", "0", "1", DocNo.Text);
                        insertSubBill("0", "0", "1", DocNo.Text);

                        readsend();
                        PrintingBill("ใบเสร็จรับเงิน.frx");
                        clearScreen();
                    }
                }
                else
                {
                    insertHeadBill("0", "0", "1", DocNo.Text);
                    insertSubBill("0", "0", "1", DocNo.Text);

                    readsend();
                    PrintingBill("ใบเสร็จรับเงิน.frx");
                    clearScreen();

                }
           
        }

        public void readsend()
        {
            string sqlhead = "";
            string sqlsub = "";
            int c = 0;
            int recs = 0;
            string docno = "";
            string docdate = "";
            string amount = "";
            string netamount = "";
            string taxamount = "";
            string cashamount = "";
            string changeamount = "";
            string beforetaxamount = "";
            int total = 0;
            string itemcode = "";
            bool onrun = false;
            DataTable drcheck = new DataTable();
            string year, month, days;
            BackgroundWorker backwork = new BackgroundWorker();
            backwork.WorkerSupportsCancellation = true;
            SQLiteDatabase db = new SQLiteDatabase(sqlite_dbname);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("sending", "2");
            DataTable dread = db.GetDataTable("select docno,docno,docdate,arcode,couponno,cashiercode,cashiercode,amount,amount,amount,vatamount,changeamount,sumcreditamount,couponamount,sumcashamount,netamount from bcarinvoice where holdingstatus=0 and sending<2");

            backwork.WorkerReportsProgress = true;
            backwork.RunWorkerAsync();
            backwork.DoWork += delegate(object obj, DoWorkEventArgs args)
            {
                try
                {

                    if (dread.Rows.Count > 0)
                    {

                        recs = dread.Rows.Count;
                        foreach (DataRow dr in dread.Rows)
                        {
                            docno = dr[0].ToString();
                            using (SqlConnection scon = new SqlConnection(strcon))
                            {
                                if (scon.State == ConnectionState.Closed)
                                    scon.Open();
                                using (SqlDataAdapter da = new SqlDataAdapter("select docno from bcarinvoice where docno='" + docno + "'", scon))
                                {
                                    drcheck = new DataTable();
                                    da.Fill(drcheck);
                                }
                            }
                            if (drcheck.Rows.Count == 0)
                            {
                                sqlhead = "insert into bcarinvoice(docno,taxno,docdate,arcode,coupongdesc,cashiercode,salecode,totalamount,sumofitemamount,netdebtamount,taxamount,changeamount,sumcreditamount,coupongamount,sumcashamount,afterdiscount,beforetaxamount,homeamount,glformat,taxtype,iscompletesave)";
                                sqlhead += "values(";
                                for (int i = 0; i < dread.Columns.Count-2; i++)
                                {

                                    if (i==2)
                                    {
                                        dr[i] = DateTime.Parse(dr[i].ToString()).ToShortDateString();
                                        if (dr[i].ToString().IndexOf("-") != -1)
                                            dr[i] = (Convert.ToInt16(dr[i].ToString().Split('-')[2].ToString()) - 543).ToString() + "/" + dr[i].ToString().Split('-')[1].ToString() + "/" + dr[i].ToString().Split('-')[0].ToString();
                                        else
                                            dr[i] = (Convert.ToInt16(dr[i].ToString().Split('/')[2].ToString()) - 543).ToString() + "/" + dr[i].ToString().Split('/')[1].ToString() + "/" + dr[i].ToString().Split('/')[0].ToString();

                                        docdate = dr[i].ToString();
                                    }
                                    if (i > 6)
                                    {
                                        if (dr[i].ToString().Length == 0) dr[i] = "NULL";

                                        sqlhead += dr[i].ToString();
                                    }
                                    else
                                        sqlhead += "'" + dr[i].ToString() + "'";

                                    if (i < dread.Columns.Count - 3) sqlhead += ",";

                                }
                                
                                docno = dr["docno"].ToString();
                                amount = dr["amount"].ToString();
                                taxamount = dr["vatamount"].ToString();
                                cashamount = dr["amount"].ToString();
                                beforetaxamount = dr["netamount"].ToString().Replace(",", "");
                                changeamount = "0";// dr["changeamount"].ToString();
                                sqlhead += string.Format(",{0},{0},{1},{0},'B01','1',1)",amount,beforetaxamount);
                                // MessageBox.Show(sqlhead);
                                using (SqlConnection scon = new SqlConnection(strcon))
                                {
                                    if (scon.State == ConnectionState.Closed)
                                        scon.Open();
                                    using (SqlCommand scom = new SqlCommand(sqlhead, scon))
                                    {
                                        //  MessageBox.Show(scon.ConnectionString);
                                        scom.ExecuteNonQuery();

                                    }
                                    sqlhead = "insert into bcoutputtax(docno,taxno,docdate,taxdate,arcode,shorttaxdesc,bookcode,savefrom,source,beforetaxamount,taxamount)values(";
                                    sqlhead += string.Format("'{0}','{0}','{1}','{1}','{2}','ขายสินค้าเงินสด','03',1,6,{3},{4})",docno,docdate,_arcode,beforetaxamount,taxamount);
                                    using (SqlCommand scom = new SqlCommand(sqlhead, scon))
                                    {
                                        //  MessageBox.Show(scon.ConnectionString);
                                        scom.ExecuteNonQuery();

                                    }
                                    sqlhead = "insert into BCRecmoney(DocNo,DocDate,ArCode,MyDescription,PaymentType,SaveFrom,PayAmount,ChangeAmount)values(";
                                    sqlhead += string.Format("'{0}','{1}','{2}','ขายสินค้าเงินสด',0,1,{3},{4})", docno, docdate, _arcode,cashamount,changeamount);
                                    db.Update("bcarinvoice", dic, "docno='" + docno + "'");
                                    using (SqlCommand scom = new SqlCommand(sqlhead, scon))
                                    {
                                        //  MessageBox.Show(scon.ConnectionString);
                                        scom.ExecuteNonQuery();

                                    }
                                }

                            }
                        }
                    }
                    DataTable dreadsub = new DataTable();
                    dreadsub = db.GetDataTable("select docno,docdate,arcode,itemcode,itemname,qty,price,unitcode,amount,discountamount from bcarinvoicesub where   docno not like 'Bill%' and sending<2 ");
                    if (dreadsub.Rows.Count > 0)
                    {
                        foreach (DataRow drr in dreadsub.Rows)
                        {

                            sqlsub = "insert into bcarinvoicesub(docno,docdate,arcode,itemcode,itemname,qty,price,unitcode,amount,discountamount,whcode,shelfcode,taxtype,taxrate)";
                            sqlsub += "values(";
                            for (int i = 0; i < dreadsub.Columns.Count; i++)
                            {

                                itemcode = drr[3].ToString();
                                docno = drr[0].ToString();
                                if (i == 1)
                                {
                                    //drr[i] = drr[i].ToString().Substring(0, 10).Trim();
                                    if (drr[i].ToString().IndexOf("-") != -1)
                                    {
                                        year = (Convert.ToInt16(drr[i].ToString().Split('-')[2].ToString()) - 543).ToString();
                                        month = drr[i].ToString().Split('-')[1].ToString();
                                        days = drr[i].ToString().Split('-')[0].ToString();
                                        drr[i] = year + "/" + month + "/" + days;
                                    }
                                    else
                                    {
                                        if (drr[i].ToString().Length > 10)
                                            drr[i] = drr[i].ToString().Substring(0, 10).Trim();
                                        drr[i] = (Convert.ToInt16(drr[i].ToString().Split('/')[2].ToString()) - 543).ToString() + "/" + drr[i].ToString().Split('/')[1].ToString() + "/" + drr[i].ToString().Split('/')[0].ToString();

                                    }

                                    //dr[i] = (Convert.ToInt16(dr[i].ToString().Split('-')[2].ToString()) - 543).ToString() + "/" + dr[i].ToString().Split('-')[1].ToString() + "/" + dr[i].ToString().Split('-')[0].ToString(); 
                                }
                                if (i > 4 && i != 7)
                                {
                                    if (drr[i].ToString().Trim().Length == 0) drr[i] = "0";
                                    sqlsub += drr[i].ToString();

                                }
                                else
                                    sqlsub += "'" + drr[i].ToString() + "'";
                                if (i < dreadsub.Columns.Count - 1) sqlsub += ",";

                            }
                            sqlsub += ",'001','001','1',7)";
                            using (SqlConnection scon = new SqlConnection(strcon))
                            {
                                if (scon.State == ConnectionState.Closed)
                                    scon.Open();
                                using (SqlCommand scom = new SqlCommand(sqlsub, scon))
                                {
                                    scom.ExecuteNonQuery();
                                    db.Update("bcarinvoicesub", dic, "docno='" + docno + "' and itemcode='" + itemcode + "'");
                                }
                                //sqlsub = "update bcarinvoicesub set unitcode=(select defsaleunitcode from bcitem where code='" + itemcode + "') where docno='" + docno + "'";
                                //using (SqlCommand scom = new SqlCommand(sqlsub, scon))
                                //{
                                //    scom.ExecuteNonQuery();
                                //}
                                sqlsub = "INSERT INTO ProcessStock (ItemCode,DocDate,ProcessFlag,FlowStatus,ProcessType,ProcessCase) SELECT Code,NULL,1,0,0,0 FROM BCItem WHERE  Code='" + itemcode + "' AND Code NOT IN (SELECT ItemCode FROM ProcessStock)";
                                using (SqlCommand scom = new SqlCommand(sqlsub, scon))
                                {
                                    scom.ExecuteNonQuery();
                                }
                                // dic.Add("sending", "2");



                            }
                        }
                    }

                    // }
                    backwork.ReportProgress(c);
                    c += 1;
                }




                    //else onrun = false;
                // backwork.CancelAsync();


                    // onrun = true;



                catch (Exception ex)
                {
                    // LogMessageToFile(ex.Message+"==>readsend");
                    //backwork.CancelAsync();
                    MessageBox.Show(ex.Message);
                    //SmartLib.Controls.DialogMessageBox.ShowBox("ไม่มีข้อมูลนำส่ง", "ผิดพลาด", "ตกลง");


                }


            };


            backwork.RunWorkerCompleted += delegate(object obj, RunWorkerCompletedEventArgs args)
            {
                //backwork.CancelAsync();
                //LogMessageToFile("complete==>readsend");

            };

            backwork.ProgressChanged += delegate(object obj, ProgressChangedEventArgs args)
            {
                total = args.ProgressPercentage;
            };

        }
        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
          

        }
        private void insertHeadBill(string hold, string cancel, string iscomplete, string hdoc)
        {
            //string hdoc = "";
            Dictionary<string, string> data = new Dictionary<string, string>();
            if (hold == "1")
            {

                data.Add("DocNo", hdoc);
            }
            else
                data.Add("DocNo", DocNo.Text);

            data.Add("DocDate", DocDate.Text);
            //data.Add("DocTime", DocTime.Text);
            data.Add("Amount", NumberAmount.Text.Replace(",", ""));
            data.Add("ArCode", _arcode);
            data.Add("MemberCode", _arcode);
            data.Add("NetAmount", ((Convert.ToDecimal(NumberAmount.Text)-((Convert.ToDecimal(NumberAmount.Text)/107)*7)).ToString("#,##0.00")).Replace(",", ""));
            data.Add("VatAmount", (((Convert.ToDecimal(NumberAmount.Text)/107) * 7).ToString("#,##0.00")).Replace(",", ""));
            //if(MainDtreceipt.Rows[0]["CashAmount"] == null) bill.cashReciept = "0.00";
            if (bill.creditReciept == null) bill.creditReciept = "0.00";
            if (bill.billPercDisc == null) bill.billPercDisc = "0.00";
            // if(Discount.Text.Trim().Length>0)bill.
            if (bill.billCashDisc == null) bill.billCashDisc = "0.00";
            if (bill.cashChange == null) bill.cashChange = "0.00";
            if (bill.couponReciept == null) bill.couponReciept = "0.00";
            if (MainDtreceipt.Rows.Count > 0)
            {
                bill.cashReciept = MainDtreceipt.Rows[0]["CashAmount"].ToString();
                bill.creditReciept = MainDtreceipt.Rows[0]["CreditAmount"].ToString();
                bill.couponReciept = MainDtreceipt.Rows[0]["CouponValue"].ToString();
                bill.cashChange = MainDtreceipt.Rows[0]["ChangeAmount"].ToString();
            }
            data.Add("SumCashAmount", bill.cashReciept);
            data.Add("SumCreditAmount", bill.creditReciept);
            data.Add("CouponAmount", bill.couponReciept);
            data.Add("ChangeAmount", bill.cashChange);
            data.Add("RoundNo", "0");
            data.Add("CASHIERCODE", SaleCode.Text);

            //if (bill.billCashDisc.Length > 0)
            //    data.Add("DiscountAmount", bill.billCashDisc);
            //else
            //    if (bill.billPercDisc.Length > 0)
            data.Add("DiscountAmount", "0.00");
            //data.Add("SumOfItemAmount", NumberAmount.Text);
            //data.Add("NetDebtAmount", NumberAmount.Text);
            //data.Add("TotalAmount", NumberAmount.Text);
            //data.Add("CouponNo", bill.couponNo);
            data.Add("DiscountWord", "");// ((Convert.ToDecimal(Amount.Text)-((Convert.ToDecimal(Amount) / 107) * 7)).ToString("#,##0.00")));
            data.Add("IsCancel", cancel);
            data.Add("IsComplete", iscomplete);
            data.Add("HoldingStatus", hold);

            // if (header1.Status)
            //     data.Add("Sending", "1");
            // else
            data.Add("Sending", "0");

            string internalsql = db.Insertstr("BCArinvoice", data);
            data.Clear();
            data.Add("DocNo", DocNo.Text);

            data.Add("DocDate", DocDate.Text);
            //data.Add("DocTime", DocTime.Text);
            data.Add("Amount", NumberAmount.Text.Replace(",", ""));
            data.Add("ArCode", _arcode);

            //if (header1.Status)
            // chanel.SendMessage(GlobalClass.GlobalUser, internalsql);
            //newBill();
        }
        private void insertSubBill(string hold, string cancel, string iscomplete, string hdoc)
        {
            //string hdoc = "";
            Dictionary<string, string> data = new Dictionary<string, string>();

            // if (hold == "1")
            //     data.Add("DocNo", "BillHold_" + DocNo.Text.Substring(DocNo.Text.IndexOf('-') + 1, DocNo.Text.Length - DocNo.Text.IndexOf('-') - 1));
            // else
            foreach (DataRow dr in this.saleitemdt.Rows)
            {
                data.Clear();
                if (hold == "1")
                {
                    //    DataTable hdt = db.GetDataTable("select docno from bcarinvoice where holdingstatus=1 order by docno limit 1");
                    //    if (hdt.Rows.Count > 0)
                    //        hdoc = makeDoc("BillHold_",Convert.ToInt16(hdt.Rows[0][0].ToString().Substring(hdt.Rows[0][0].ToString().IndexOf('_') + 1, hdt.Rows[0][0].ToString().Length - hdt.Rows[0][0].ToString().IndexOf('_') - 1)).ToString(),"");
                    //    else
                    //        hdoc = "BillHold_00001";      
                    //   data.Add("DocNo", "BillHold_" + DocNo.Text.Substring(DocNo.Text.IndexOf('-') + 1, DocNo.Text.Length - DocNo.Text.IndexOf('-') - 1));
                    data.Add("DocNo", hdoc);
                }
                else
                    data.Add("DocNo", DocNo.Text);

                data.Add("DocDate", DocDate.Text);
                data.Add("DocTime", DateTime.Now.ToShortTimeString());
                data.Add("ItemCode", dr[5].ToString());
                data.Add("BarCode", dr[4].ToString());
                data.Add("ItemName", dr[1].ToString());
                data.Add("Qty", dr[0].ToString());
                data.Add("Price", dr[2].ToString());
                data.Add("Amount", dr[3].ToString().Replace(",", ""));
                data.Add("Unitcode", getSingleRow(string.Format("select top 1 code from bcitemunit where (name='{0}' or code='{0}')", dr["Unitcode"]), "code"));
                data.Add("ArCode", _arcode);
                data.Add("MemberCode", _arcode);

                // data.Add("NetAmount", SumOfItem.Text.Replace(",", ""));//(((Convert.ToDecimal(Amount)/107)*7).ToString("#,##0.00")));
                // data.Add("VatAmount", TaxAmount.Text.Replace(",", ""));
                //if (bill.cashReciept == null) bill.cashReciept = "0.00";
                //if (bill.creditReciept == null) bill.creditReciept = "0.00";
                //if (bill.billPercDisc == null) bill.billPercDisc = "0.00";
                //if (bill.billCashDisc == null) bill.billCashDisc = "0.00";
                //if (bill.cashChange == null) bill.cashChange = "0.00";
                //if (bill.couponReciept == null) bill.couponReciept = "0.00";
                //data.Add("SumCashAmount", bill.cashReciept);
                //data.Add("SumCreditAmount", bill.creditReciept);
                //data.Add("CouponAmount", bill.couponReciept);
                //data.Add("ChangeAmount", bill.cashChange);

                //if (bill.billCashDisc.Length > 0)
                //    data.Add("DiscountAmount", bill.billCashDisc);
                //else
                //    if (bill.billPercDisc.Length > 0)
                //        data.Add("DiscountAmount", bill.billPercDisc);
                //data.Add("CouponNo", bill.couponNo);
                //data.Add("DiscountWord", Discount.Text);// ((Convert.ToDecimal(Amount.Text)-((Convert.ToDecimal(Amount) / 107) * 7)).ToString("#,##0.00")));
                data.Add("IsCancel", cancel);
                //data.Add("IsComplete", iscomplete);
                // data.Add("HoldingStatus", hold);
                // if (header1.Status)
                //     data.Add("Sending", "1");
                // else
                data.Add("Sending", "0");

                string internalsql = db.Insertstr("BCArinvoiceSub", data);

                //if (header1.Status)
                //   chanel.SendMessage(GlobalClass.GlobalUser, internalsql);


                //chanel.SendMessage(GlobalClass.GlobalUser, internalsql);
            }
            //newBill();
        }

        void svform_CloseSave(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void initDataTable()
        {
            MainDtreceipt = new DataTable();
            MainDtreceipt.Columns.Add("DocNo", typeof(string));
            MainDtreceipt.Columns.Add("BillAmount", typeof(double));
            MainDtreceipt.Columns.Add("CashAmount", typeof(double));
            MainDtreceipt.Columns.Add("ChangeAmount", typeof(double));
            MainDtreceipt.Columns.Add("CreditDate", typeof(DateTime));
            MainDtreceipt.Columns.Add("CreditNo", typeof(string));
            MainDtreceipt.Columns.Add("BankCode", typeof(string));
            MainDtreceipt.Columns.Add("SecurityCode", typeof(string));
            MainDtreceipt.Columns.Add("CreditAmount", typeof(double));
            MainDtreceipt.Columns.Add("BookNo", typeof(string));
            MainDtreceipt.Columns.Add("CheqNo", typeof(string));
            MainDtreceipt.Columns.Add("CheqAmount", typeof(double));
            MainDtreceipt.Columns.Add("CheqValue", typeof(double));
            MainDtreceipt.Columns.Add("CheqBalance", typeof(double));
            MainDtreceipt.Columns.Add("CouponNo", typeof(string));
            MainDtreceipt.Columns.Add("CouponValue", typeof(double));
            MainDtreceipt.Columns.Add("DepositNo", typeof(string));
            MainDtreceipt.Columns.Add("DepositValue", typeof(double));
            MainDtreceipt.Columns.Add("DepositBalance", typeof(double));
            MainDtreceipt.Columns.Add("DepositAmount", typeof(double));


            DataRow dr = MainDtreceipt.NewRow();

            dr["CreditAmount"] = 0.00;
            dr["CheqAmount"] = 0.00;
            dr["CheqValue"] = 0.00;
            dr["CheqBalance"] = 0.00;
            dr["CouponValue"] = 0.00;
            dr["DepositValue"] = 0.00;
            dr["DepositBalance"] = 0.00;
            dr["DepositAmount"] = 0.00;
            dr["CashAmount"] = 0.00;
            dr["BillAmount"] = Convert.ToDouble(NumberAmount.Text);
            MainDtreceipt.Rows.Add(dr);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //if (Convert.ToDouble(textEditChange.Text) >= 0)
            //{
            //    popupControlContainer2.HidePopup();
            //    savepopup = false;
            //}
            //else
            //    MessageBox.Show("ยอดรับเงินไม่ครบ!");
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
           // popupControlContainer2.HidePopup();
           // savepopup = false;
        }

        private void textEditCash_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    ChangeAmount.Text = (Convert.ToDouble(textEditAmount.Text) - Convert.ToDouble(textEditCash.Text)).ToString();
            //}
        }
        //protected override void OnLostFocus(EventArgs e)
        //{
        ////    if(popupControlContainer2.on)
        //    if (savepopup)
        //        popupControlContainer2.ShowPopup(barManager1, this.PointToClient(new Point(this.splitContainerControl1.Left + 450, this.splitContainerControl1.Top + 200)));
        //        //ClosePopup(PopupCloseMode.Immediate);
        //    base.OnLostFocus(e);
        //}
    }
    public class BCITEM
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
        public string LASTROW { get; set; }

    }
    public class BCARINVOICESUB
    {
        public string DOCNO { get; set; }
        public string CUSCODE { get; set; }
        public string ARCODE { get; set; }
        public string DOCDATE { get; set; }
        public string DOCTIME { get; set; }
        public string ITEMCODE { get; set; }
        public string SERIALNO { get; set; }
        public string QTY { get; set; }
        public string SALEPRICE { get; set; }


    }
}