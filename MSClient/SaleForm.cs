using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.Xpo;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using SmartLib.Helpers;
using System.IO;
using FastReport;
using FastReport.Utils;

namespace MultiShop
{
    public partial class SaleForm : DevExpress.XtraEditors.XtraForm
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
        Billing bill=null;
        SQLiteDatabase db=null;// = new SQLiteDatabase();
        private Boolean islip = true;
        Boolean reloadBill = false;

        public static string sqlite_dbname = Path.GetDirectoryName(Application.ExecutablePath)+ @"\DATA\invoice.s3db";
            //Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "invoice.s3db");
        string[] wordnum = new string[]
        {
            "","หนึ่ง","สอง","สาม","สี่","ห้า","หก","เจ็ด","แปด","เก้า","สิบ"
        };
        SearchForm srchForm;
        public SaleForm()
        {
            InitializeComponent();
            gridView1.OptionsBehavior.Editable = true;
            gridView1.OptionsBehavior.ReadOnly = false;
            
            gridView1.OptionsView.ShowGroupPanel = false;
            strcon = string.Format(@"Data Source={0}; Initial Catalog={1};User Id={2};Password={3};", instancename, dbname, username, password);
            srchForm = new SearchForm();
            srchForm.EnterClose += new EventHandler(srchForm_EnterClose);
            bill = new Billing();
            db = new SQLiteDatabase(sqlite_dbname);
            DocDate.EditValue = DateTime.Now.ToShortDateString();
            preData();
        }

        void srchForm_EnterClose(object sender, EventArgs e)
        {
            textSearch.Text="";
            //throw new NotImplementedException();
        }


        private void preData()
        {
            ArCode.Properties.DataSource = LoadData("Select code,name1,billaddress,telephone from bcar");
            ArCode.Properties.DisplayMember = "code";
            ArCode.Properties.ValueMember = "code";
            SaleCode.Properties.DataSource = LoadData("select code,name from bcsale");
            SaleCode.Properties.DisplayMember = "code";
            SaleCode.Properties.ValueMember = "code";
            preDocNoCombo();
            saleitemdt.Columns.Add(new DataColumn("Qty", typeof(Int16)));
            saleitemdt.Columns.Add(new DataColumn("SaleItemName", typeof(String)));
            saleitemdt.Columns.Add(new DataColumn("Price", typeof(Double)));
            saleitemdt.Columns.Add(new DataColumn("Amount", typeof(Double)));
            saleitemdt.Columns.Add(new DataColumn("SerialNo", typeof(string)));
            saleitemdt.Columns.Add(new DataColumn("ItemCode",typeof(string)));
            saleitemdt.Columns.Add(new DataColumn("UnitCode", typeof(string)));
            AddColumn("รวม", "Amount", 25,false);
            AddColumn("ราคา", "Price", 25,false);
            AddColumn("รายการ","SaleItemName",50,false);
            AddColumn("จำนวน", "Qty", 25,true);
            
            gridControl1.DataSource = saleitemdt;
           
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
                    DocNo.Properties.Items.Add(string.Format("{0}={1}", dr["goupcode"], dr["goupdesc"]));
            }            
        }

        int index;
        private void AddColumn(string caption,string fieldname,int width,bool edited)
        {
            index++;
            DevExpress.XtraGrid.Columns.GridColumn column = new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Caption = caption,
                FieldName = fieldname,
                Width = width,
                
            };
            //if (edited)
            //{
            //    RepositoryItemCalcEdit repo_text = new RepositoryItemCalcEdit();
            //    repo_text.Name = "repo" + fieldname;
            //    repo_text.DisplayFormat.FormatType=FormatType.Numeric;
            //    repo_text.DisplayFormat.FormatString="#,##0.00";
            //    gridControl1.RepositoryItems.Add(repo_text);
            //    column.ColumnEdit = repo_text;
            //}
            if (fieldname != "SaleItemName")
            {
                column.DisplayFormat.FormatType = FormatType.Numeric;
                column.DisplayFormat.FormatString = "#,##0.00";
                column.AppearanceCell.Font = new Font(gridView1.Appearance.Row.Font, FontStyle.Bold);
            }
            gridView1.Columns.Add(column);
            column.VisibleIndex = 0;
        }
        private DataTable LoadData(string sql)
        {
            var dt = new DataTable();
            using (var scon = new SqlConnection(strcon))
            {
                if (scon.State == ConnectionState.Closed)
                {
                    scon.Open();
                }
                using (var da = new SqlDataAdapter(sql, scon))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private void newDocno(Object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            DataTable dt = new DataTable();
            ComboBoxEdit cbedit = sender as ComboBoxEdit;
            string docseltext = "";
            if (DocNo.Text.IndexOf("=") > 0) docseltext = DocNo.Text.Substring(0, DocNo.Text.IndexOf("="));
            //if (DocNoBox.Text.IndexOf("=") > 0) docseltext = DocNoBox.Text.Substring(0, DocNoBox.Text.IndexOf("="));
            else
                docseltext = DocNo.Text;

            string docnotmp = "";
            int coun = 0;
            string docoun = "";
            using (SqlConnection scon = new SqlConnection(strcon))
            {
                if (scon.State == ConnectionState.Closed) scon.Open();
                SqlCommand scom = new SqlCommand(string.Format("select DocFormat from BCSysTRFormat where TransKey='BillingTransConfig'"), scon);
                //    if (DocNoBox.Text == string.Empty)
                //    {
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
                                DocNo.Text=docnotmp.Replace("####", docoun.Substring(docoun.Length-4));
                                //DocNo.Text = docnotmp + docoun.Substring(0, 4);
                                //TaxNoBox.Text = docnotmp + docoun.Substring(0, 4);
                            }
                            else
                            {
                                DocNo.Text = docnotmp.Replace("####", "0001");
                                //TaxNoBox.Text = docnotmp.Replace("####", "0001");
                            }
                    }

                //docnotmp = docnotmp.Replace("####", "0001");
                //DocNoBox.Text = docnotmp;
                //}
            }
        
        }

        private void ClearSearchTxt_Click(object sender, EventArgs e)
        {
            if (srchForm.ShowDialog() == DialogResult.OK)
                if (srchForm.result != null)
                {
                    if (checkPriceCost(srchForm.result.ItemArray[0].ToString()) && MainForm._appconf.islowcost == 1)
                        MessageBox.Show("สินค้าราคาขายต๋ำกว่าหรือเท่ากับทุน");
                    insertItem(srchForm.result);
                }
            this.textSearch.Text = "";
        }

        private void textSearch_MouseClick(object sender, MouseEventArgs e)
        {
            if (srchForm.ShowDialog() == DialogResult.OK)
                if (srchForm.result != null)
                {
                    if (checkPriceCost(srchForm.result.ItemArray[0].ToString()) && MainForm._appconf.islowcost == 1)
                        MessageBox.Show("สินค้าราคาขายต๋ำกว่าหรือเท่ากับทุน");
                    insertItem(srchForm.result);
                }
            this.textSearch.Text = "";
            
        }

        private void textSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
           // e.Handled = true;
           // if(srchForm.ShowDialog()==DialogResult.OK)
           //     if(srchForm.result!=null)
           //         insertItem(srchForm.result);
           // this.textSearch.Text = "";
                    //textSearch.EditValue=srchForm.searchResult;
            
        }

        private void insertItem(DataRow dr)
        {
            
            var sel=saleitemdt.Select(string.Format("SaleItemName='{0}'",dr[1]));
            if (sel.Length>0)
            {
                sel[0][0] = (Convert.ToDecimal(sel[0][0].ToString()) + 1);
                sel[0][3] = String.Format("{0:0,0.0}", (Convert.ToDecimal(sel[0][0].ToString()) * Convert.ToDecimal(sel[0][2].ToString())));

            }
            else
            {
                DataRow rows = saleitemdt.NewRow();
                rows[0] = 1;
                rows[1] = dr[1];
                rows[2] = dr[4];
                rows[3] = 1 * Convert.ToDouble(dr[4]);
                rows[4] = dr[0];
                rows[5] = dr[0];
                rows[6] = dr[3];
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
        private void insertItem(DataRow dr,string serialno)
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
                rows[2] = dr[4];
                rows[3] = 1 * Convert.ToDouble(dr[4]);
                rows[4] = serialno;
                rows[5] = dr[0];
                rows[6] = dr[3];
                saleitemdt.Rows.Add(rows);


            }
            saleitemdt.AcceptChanges();
            upAmounLabel(Convert.ToDouble(saleitemdt.Compute("Sum(Amount)", string.Empty)).ToString("#,##0.00"));
        }
     

     
        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Delete)
            {

                GridView view = (GridView)sender;
                Point pt = view.GridControl.PointToClient(Control.MousePosition);
                GridHitInfo info = view.CalcHitInfo(pt);
                if (info.InRow && info.InRowCell)
                {
                    DataRow dcol = saleitemdt.Rows[info.RowHandle];
                    dcol.Delete();
                    saleitemdt.AcceptChanges();
                    gridControl1.RefreshDataSource();
                    if (saleitemdt.Rows.Count > 0)
                        upAmounLabel(Convert.ToDouble(saleitemdt.Compute("Sum(Amount)", string.Empty)).ToString("#,##0.00"));
                    else
                        upAmounLabel("0.00");
                }
                
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName != "Qty") return;
            GridView view = (GridView)sender;
            string fieldvalue = view.GetRowCellValue(e.RowHandle, view.Columns["Qty"]).ToString();
             RepositoryItemTextEdit repo_text = new RepositoryItemTextEdit();
                repo_text.Name = "repoQty";
                repo_text.DisplayFormat.FormatType=FormatType.Numeric;
                repo_text.DisplayFormat.FormatString="#,##0.00";
                e.RepositoryItem = repo_text;
                repo_text.EditValueChanged += new EventHandler(repo_text_EditValueChanged);
                repo_text.KeyDown += new KeyEventHandler(repo_text_KeyDown);
                
            
                rowedit = e.RowHandle;
        }

        void repo_text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DevExpress.XtraEditors.TextEdit text = (DevExpress.XtraEditors.TextEdit)sender;
                if (text.Text != string.Empty)
                {
                    saleitemdt.Rows[rowedit][3] = String.Format("{0:0,0.0}", (Convert.ToDecimal(text.Text) * Convert.ToDecimal(saleitemdt.Rows[rowedit][2].ToString())));
                    saleitemdt.AcceptChanges();
                    gridControl1.RefreshDataSource();
                    upAmounLabel(Convert.ToDouble(saleitemdt.Compute("Sum(Amount)", string.Empty)).ToString("#,##0.00"));
                    textSearch.Focus();
                    e.Handled = true;
                }
                
            }
           
            //throw new NotImplementedException();
        }

        void repo_text_EditValueChanged(object sender, EventArgs e)
        {
            
            //throw new NotImplementedException();
             //var sel=saleitemdt.Select(string.Format("SaleItemName='{0}'",dr[1]));
             //if (sel.Length > 0)
            // {
                 //saleitemdt.Rows[rowedit][0] = (Convert.ToDecimal(sel[0][0].ToString()) + 1);
                 //DevExpress.XtraEditors.TextEdit text = (DevExpress.XtraEditors.TextEdit)sender;
                 //if (text.Text != string.Empty)
                 //{
                 //    saleitemdt.Rows[rowedit][3] = String.Format("{0:0,0.0}", (Convert.ToDecimal(text.Text) * Convert.ToDecimal(saleitemdt.Rows[rowedit][2].ToString())));
                 //    saleitemdt.AcceptChanges();
                 //    gridControl1.RefreshDataSource();
                 //    upAmounLabel(Convert.ToDouble(saleitemdt.Compute("Sum(Amount)",string.Empty)).ToString("#,##0.00"));
                 //}

                // gridView1.RefreshRow(rowedit);

            // }
        }

        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (gridView1.FocusedColumn.FieldName == "Qty")
                e.Cancel = false;
            else
                e.Cancel = true;
                 
        }

        private void upAmounLabel(string txt)
        {
            NumberAmount.Text = txt;
            TotalAmount.Text = txt;
            if (incvat)
            {
                BeforeTaxAmount.Text = (Convert.ToDouble(txt) / 1.07).ToString("#,##0.00");
                TaxAmount.Text = (Convert.ToDouble(txt) - (Convert.ToDouble(txt) / 1.07)).ToString("#,##0.00");
            }
            else
            {
                BeforeTaxAmount.Text = txt;
                TaxAmount.Text = "0.00";
            }
            AmountToWord();
            SumCashAmount.Text = "0.00";
            textSearch.Focus();

        }
        private void AmountToWord()
        {

            string[] temp = new string[TotalAmount.Text.Substring(0, TotalAmount.Text.IndexOf('.')).Replace(",", "").Length];
            string strtemp = TotalAmount.Text.Substring(0, TotalAmount.Text.IndexOf('.')).Replace(",", "");

            //int i = temp.Length-1;
            for (int k = 0; k < temp.Length; k++)
            {
                temp[k] = strtemp.Substring(k, 1);
                // i -= 1;
            }
            //i = tdigi.Length-1;
            string[] tdigi = new string[TotalAmount.Text.Substring(TotalAmount.Text.IndexOf('.') + 1, 2).Length];
            strtemp = TotalAmount.Text.Substring(TotalAmount.Text.IndexOf('.') + 1, 2);
            for (int k = 0; k < tdigi.Length; k++)
            {
                tdigi[k] = strtemp.Substring(k, 1);
                // i -= 1;
            }
            string wording = "";
            int i = temp.Length - 1;
            int j = 0;
            foreach (string val in temp)
            {
                //if(temp.Length>11)
                //{
                switch (i)
                {
                    case 0:
                        //if (Convert.ToInt16(temp[j-1].ToString()) == 2 && Convert.ToInt16(temp[j].ToString()) == 1)
                        //    wording += "ยี่สิบเอ็ด";
                        //else
                        // if (Convert.ToInt16(temp[j-1].ToString()) == 1 && Convert.ToInt16(temp[j].ToString()) == 1)
                        //     wording += "สิบเอ็ด";
                        // else
                        if (Convert.ToInt16(temp[j].ToString()) == 1)
                            wording += "เอ็ด";
                        else
                            wording += wordnum[Convert.ToInt16(temp[j].ToString())];
                        break;


                    case 1:
                        if (Convert.ToInt16(temp[j].ToString()) == 1)
                        {
                            if (temp.Length == 2)
                                wording = "สิบ";
                            else
                                wording += "สิบ";
                        }
                        else
                            if (Convert.ToInt16(temp[j].ToString()) == 2)
                                wording += "ยี่สิบ";
                            else if (Convert.ToInt16(temp[j].ToString()) > 2)
                                wording += wordnum[Convert.ToInt16(temp[j].ToString())] + "สิบ";
                        break;

                    case 2:
                        if (Convert.ToInt16(temp[j].ToString()) > 0)
                            wording += wordnum[Convert.ToInt16(temp[j].ToString())] + "ร้อย";

                        break;

                    case 3:
                        if (Convert.ToInt16(temp[j].ToString()) > 0)
                            wording += wordnum[Convert.ToInt16(temp[j].ToString())] + "พัน";
                        break;

                    case 4:
                        if (Convert.ToInt16(temp[j].ToString()) > 0)
                            wording += wordnum[Convert.ToInt16(temp[j].ToString())] + "หมื่น";
                        break;

                    case 5:
                        if (Convert.ToInt16(temp[j].ToString()) > 0)
                            wording += wordnum[Convert.ToInt16(temp[j].ToString())] + "แสน";
                        break;
                    case 6:
                        if (Convert.ToInt16(temp[j].ToString()) > 0)
                            wording += wordnum[Convert.ToInt16(temp[j].ToString())] + "ล้าน";
                        break;
                    default:
                        if (Convert.ToInt16(temp[j].ToString()) > 0)
                            wording += wordnum[Convert.ToInt16(temp[j].ToString())];
                        break;
                }
                j += 1;
                i -= 1;

                //}
            }
            wording = wording + "บาท";
            i = tdigi.Length - 1;
            j = 0;
            foreach (string val in tdigi)
            {
                //if(tdigi.Length>11)
                //{
                switch (i)
                {
                    case 0:
                        // if (Convert.ToInt16(tdigi[j - 1].ToString()) == 2 && Convert.ToInt16(tdigi[j].ToString()) == 1)
                        //     wording += "ยี่สิบเอ็ด";
                        //else
                        // if (Convert.ToInt16(tdigi[j - 1].ToString()) == 1 && Convert.ToInt16(tdigi[j].ToString()) == 1)
                        //     wording += "สิบเอ็ด";
                        // else
                        if (Convert.ToInt16(tdigi[j].ToString()) == 1)
                            wording += "เอ็ด";
                        else
                            wording += wordnum[Convert.ToInt16(tdigi[j].ToString())];
                        break;


                    case 1:
                        if (Convert.ToInt16(tdigi[j].ToString()) == 1)
                            wording = "สิบ";
                        else
                            if (Convert.ToInt16(tdigi[j].ToString()) == 2)
                                wording += "ยี่สิบ";
                            else if (Convert.ToInt16(tdigi[j].ToString()) > 2)
                                wording += wordnum[Convert.ToInt16(tdigi[j].ToString())] + "สิบ";
                        break;

                    case 2:
                        if (Convert.ToInt16(tdigi[j].ToString()) > 0)
                            wording += wordnum[Convert.ToInt16(tdigi[j].ToString())] + "ร้อย";
                        break;

                    case 3:
                        if (Convert.ToInt16(tdigi[j].ToString()) > 0)
                            wording += wordnum[Convert.ToInt16(tdigi[j].ToString())] + "พัน";
                        break;

                    case 4:
                        if (Convert.ToInt16(tdigi[j].ToString()) > 0)
                            wording += wordnum[Convert.ToInt16(tdigi[j].ToString())] + "หมื่น";
                        break;

                    case 5:
                        if (Convert.ToInt16(tdigi[j].ToString()) > 0)
                            wording += wordnum[Convert.ToInt16(tdigi[j].ToString())] + "แสน";
                        break;
                    case 6:
                        if (Convert.ToInt16(tdigi[j].ToString()) > 0)
                            wording += wordnum[Convert.ToInt16(tdigi[j].ToString())] + "ล้าน";
                        break;
                    default:
                        wording += wordnum[Convert.ToInt16(tdigi[j].ToString())];
                        break;
                }
                j += 1;
                i -= 1;

                //}
            }
            if (Convert.ToInt16(TotalAmount.Text.Substring(TotalAmount.Text.IndexOf('.') + 1, 2)) > 0)
                wording = wording + "สตางค์";
            thaiwordamount.Text = wording + "  "; ;
             
        }

        private void SaleForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                //if (!gridView1.OptionsBehavior.Editable) gridView1.OptionsBehavior.Editable = true;
                if (srchForm.ShowDialog() == DialogResult.OK)
                    if (srchForm.result != null)
                    {
                        if (checkPriceCost(srchForm.result.ItemArray[0].ToString()) && MainForm._appconf.islowcost == 1)
                            MessageBox.Show("สินค้าราคาขายต๋ำกว่าหรือเท่ากับทุน");
                        insertItem(srchForm.result);
                    }
                this.textSearch.Text = "";

            }
            else
                if (e.KeyCode == Keys.Enter)
                {
                    //updateCheckStock(scanbar.Replace("D", string.Empty));
                  //  if (textSearch.Text.Length > 0)
                   // {
                    //if (!gridView1.OptionsBehavior.Editable) gridView1.OptionsBehavior.Editable = true;

                        checkSerial(scanbar.Replace("D", string.Empty));
                        scanbar = String.Empty;
                    //}
                    //else
                    //{
                      //  MessageBox.Show("กรุณาป้อนรหัสสินค้า หรือ ซีเรียลนัมเบอร์!");
                     //   textSearch.SelectAll();
                    //}
                }
                else
                    scanbar += e.KeyCode.ToString();
        }

        private void textSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //if (!gridView1.OptionsBehavior.Editable) gridView1.OptionsBehavior.Editable = true;
                //updateCheckStock(scanbar.Replace("D", string.Empty));
                if (textSearch.Text.Length > 0)
                {
                    scanbar = textSearch.Text;
                    textSearch.Text = string.Empty;
                    textSearch.Focus();

                    checkSerial(scanbar.Replace("D", string.Empty));
                    scanbar = String.Empty;
                }
                else
                {
                    MessageBox.Show("กรุณาป้อนรหัสสินค้า หรือ ซีเรียลนัมเบอร์!");
                    textSearch.SelectAll();
                }
            }
            else
                scanbar += e.KeyCode.ToString();
            
        }

        private bool checkPriceCost(string itemcode)
        {
            Boolean _istrue = false;
            string saleprice=getSingleRow(string.Format("select SalePrice{0} from BCITEM WHERE CODE='{1}'",MainForm._appconf.saleprice,itemcode),string.Format("SalePrice{0}",MainForm._appconf.saleprice));
            string costprice = getSingleRow(string.Format("select SalePrice{0} from BCITEM WHERE CODE='{1}'", MainForm._appconf.costprice, itemcode), string.Format("SalePrice{0}",MainForm._appconf.costprice));
            if(Convert.ToDouble(saleprice)-Convert.ToDouble(costprice)<=0)
            _istrue = true;

            return _istrue;
        }

        private bool checkSerial(string barcode)
        {
            bool _found = false;
            using (SqlConnection scon = new SqlConnection(strcon))
            {
                if (scon.State == ConnectionState.Closed) scon.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(string.Format("select DISTINCT BCITEM.CODE,NAME1,STOCKQTY,BCITEMUNIT.NAME AS UNITNAME,SALEPRICE{1} FROM BCITEM,BCITEMUNIT,bcserialmaster where defstkunitcode=bcitemunit.code and bcitem.code=itemcode and bcserialmaster.activestatus=1 and bcserialmaster.stockstatus=0 and (serialno='{0}' or registerno='{0}' or ctrlno='{0}') ", barcode,MainForm._appconf.saleprice), scon))
                { 
                    DataTable ds=new DataTable();
                    da.Fill(ds);
                    if (ds.Rows.Count > 0)
                    {
                        if(checkPriceCost(ds.Rows[0]["CODE"].ToString()) && MainForm._appconf.islowcost==1)
                        MessageBox.Show("สินค้าราคาขายต๋ำกว่าหรือเท่ากับทุน");
                        foreach (DataRow dr in ds.Rows)
                            insertItem(dr,barcode);
                        _found = true;
                    }
                    else
                    {
                        SqlCommand com = da.SelectCommand as SqlCommand;
                        com.CommandText = string.Format("select DISTINCT BCITEM.CODE,NAME1,STOCKQTY,BCITEMUNIT.NAME AS UNITNAME,SALEPRICE{1} FROM BCITEM,BCITEMUNIT where defstkunitcode=bcitemunit.code and bcitem.code='{0}' and stocktype<>2", barcode,MainForm._appconf.saleprice);
                        ds = new DataTable();
                        da.Fill(ds);
                        if (ds.Rows.Count > 0)
                        {
                            if (checkPriceCost(ds.Rows[0]["CODE"].ToString()) && MainForm._appconf.islowcost == 1)
                                MessageBox.Show("สินค้าราคาขายต๋ำกว่าหรือเท่ากับทุน");
                            foreach (DataRow dr in ds.Rows)
                                insertItem(dr);
                            _found = true;
                            
                        }
                        
                            
                    }
                }
            }
            return _found;
        
        }

        public string scanbar { get; set; }

        private void SaleForm_Load(object sender, EventArgs e)
        {
           // newDocno(sender,e);
            textSearch.Focus();
            //textSearch.SelectAll();
        }

        //private void DocNo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
       // {
         //   newDocno(sender, e);
       // }

        private void SaleCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            LookUpEdit look = (LookUpEdit)sender;
           
        }

        private void ArCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            using (SqlConnection scon = new SqlConnection(strcon))
            {
                if (scon.State == ConnectionState.Closed) scon.Open();
                
                using (SqlDataAdapter da = new SqlDataAdapter(string.Format("select name1,billaddress,telephone from bcar where code='{0}'",ArCode.Text), scon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        ArName.Text = dt.Rows[0][0].ToString();
                        BillAddress.Text = dt.Rows[0][1].ToString();
                        Telephone.Text = dt.Rows[0][2].ToString();
                    }
                }
            }
        }

        private void saveBar_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            //ReceiptPage rec = new ReceiptPage();
            //rec.ShowDialog();
            if (NumberAmount.Text.Length > 0)
            {
                if (Convert.ToDouble(NumberAmount.Text) > 0)
                {
                    svform = new SaveForm();
                    svform.CloseSave += new EventHandler(svform_CloseSave);
                    svform.Amount = Convert.ToDouble(NumberAmount.Text);// +Convert.ToDouble(transportAmount.EditValue);
                    svform.ArCode = ArCode.Text;
                    svform.strconn = strcon;
                    if (MainDtreceipt == null || MainDtreceipt.Rows.Count == 0)
                    {
                        initDataTable();

                    }

                    svform.initDataTable(MainDtreceipt);

                    //svform.Dtreceipt = MainDtreceipt;
                    if (svform.Amount > 0)
                        svform.ShowDialog();
                }
                else
                    MessageBox.Show("ยอดจ่ายไม่เท่ากับยอดรับเงิน!", "เตือน", MessageBoxButtons.OK);
            }
            else
                MessageBox.Show("ยอดจ่ายไม่เท่ากับยอดรับเงิน!", "เตือน", MessageBoxButtons.OK);
        }

        void svform_CloseSave(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            savecomplete = true;
            // MainDtreceipt.Rows.Clear();
            MainDtreceipt = svform.Dtreceipt;
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
        public bool savecomplete { get; set; }
        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (MainDtreceipt != null)
            {
                if (reloadBill)
                {
                    if (MainForm._appconf.billlock == 0)
                    {
                        //browseBar_LinkClicked(sender, e);
                        delData();
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
            else
                MessageBox.Show("ยอดรับเงินไม่ตรงยอดขาย!");
                
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
            data.Add("ArCode", ArCode.Text);
            data.Add("MemberCode", ArCode.Text);
            data.Add("NetAmount",BeforeTaxAmount.Text.Replace(",", ""));//(((Convert.ToDecimal(Amount)/107)*7).ToString("#,##0.00")));
            data.Add("VatAmount", TaxAmount.Text.Replace(",", ""));
            //if(MainDtreceipt.Rows[0]["CashAmount"] == null) bill.cashReciept = "0.00";
            if (bill.creditReciept == null) bill.creditReciept = "0.00";
            if (bill.billPercDisc == null) bill.billPercDisc = "0.00";
            // if(Discount.Text.Trim().Length>0)bill.
            if (bill.billCashDisc == null) bill.billCashDisc = "0.00";
            if (bill.cashChange == null) bill.cashChange = "0.00";
            if (bill.couponReciept == null) bill.couponReciept = "0.00";
            if(MainDtreceipt.Rows.Count>0)
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
                data.Add("Unitcode", getSingleRow(string.Format("select top 1 code from bcitemunit where (name='{0}' or code='{0}')",dr["Unitcode"]),"code"));
                data.Add("ArCode", ArCode.Text);
                data.Add("MemberCode", ArCode.Text);

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
        private string getSingleRow(string sql, string field)
        {
            string result = "";
            using (SqlConnection scon = new SqlConnection(strcon))
            {
                if (scon.State == ConnectionState.Closed) scon.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, scon))
                { 
                    DataSet ds=new DataSet();
                    da.Fill(ds);
                    result = ds.Tables[0].Rows[0][field].ToString();
                }
            }
            return result;
        }
        private void DocNo_Closed_1(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            newDocno(sender, e);
        }
        private void clearScreen()
        {
            DocNo.Text = string.Empty;
            SaleCode.EditValue=string.Empty;
            //DocDate.Text=string.Empty;
            ArCode.Text=string.Empty;
            BillAddress.Text = string.Empty;
            Telephone.Text = string.Empty;
            Department.Text = string.Empty;
            ArCode.Text = string.Empty;
            ArName.Text = string.Empty;
            NumberAmount.Text = "0.00";
            thaiwordamount.Text = "ศูนย์บาท";
            TotalAmount.Text = "0.00";
            TaxAmount.Text = "0.00";
            BeforeTaxAmount.Text = "0.00";
            SumCashAmount.Text = "0.00";
            MyDescription.Text = string.Empty;
            
            saleitemdt.Rows.Clear();
            if(MainDtreceipt!=null)
            if(MainDtreceipt.Rows.Count>0)
            MainDtreceipt.Rows.Clear();
        }
        public  void readsend()
        {
            string sqlhead = "";
            string sqlsub = "";
            int c = 0;
            int recs = 0;
            string docno = "";
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
            DataTable dread = db.GetDataTable("select docno,docdate,arcode,couponno,cashiercode,cashiercode,amount,netamount,vatamount,sumcashamount,changeamount,sumcreditamount,couponamount from bcarinvoice where holdingstatus=0 and sending<2");

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
                                sqlhead = "insert into bcarinvoice(docno,docdate,arcode,coupongdesc,cashiercode,salecode,totalamount,netdebtamount,taxamount,sumcashamount,changeamount,sumcreditamount,coupongamount,taxtype,iscompletesave)";
                                sqlhead += "values(";
                                for (int i = 0; i < dread.Columns.Count; i++)
                                {

                                    if (i == 1) 
                                    {
                                        dr[i] = DateTime.Parse(dr[i].ToString()).ToShortDateString();
                                        if(dr[i].ToString().IndexOf("-")!=-1)
                                        dr[i] = (Convert.ToInt16(dr[i].ToString().Split('-')[2].ToString()) - 543).ToString() + "/" + dr[i].ToString().Split('-')[1].ToString() + "/" + dr[i].ToString().Split('-')[0].ToString(); 
                                        else
                                            dr[i] = (Convert.ToInt16(dr[i].ToString().Split('/')[2].ToString()) - 543).ToString() + "/" + dr[i].ToString().Split('/')[1].ToString() + "/" + dr[i].ToString().Split('/')[0].ToString(); 
                                       

                                    }
                                    if (i > 5)
                                    {
                                        if (dr[i].ToString().Length == 0) dr[i] = "NULL";
                                        
                                        sqlhead += dr[i].ToString();
                                    }
                                    else
                                        sqlhead += "'" + dr[i].ToString() + "'";

                                    if (i < dread.Columns.Count - 1) sqlhead += ",";

                                }
                                sqlhead += ",'1',1)";
                                docno = docno;

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


                                    db.Update("bcarinvoice", dic, "docno='" + docno + "'");

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
                                        if(drr[i].ToString().Length>10)
                                        drr[i] = drr[i].ToString().Substring(0, 10).Trim();
                                        drr[i] = (Convert.ToInt16(drr[i].ToString().Split('/')[2].ToString()) - 543).ToString() + "/" + drr[i].ToString().Split('/')[1].ToString() + "/" + drr[i].ToString().Split('/')[0].ToString(); 
                                       
                                    }
                                    
                                    //dr[i] = (Convert.ToInt16(dr[i].ToString().Split('-')[2].ToString()) - 543).ToString() + "/" + dr[i].ToString().Split('-')[1].ToString() + "/" + dr[i].ToString().Split('-')[0].ToString(); 
                                }
                                if (i > 4 && i!=7)
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
                if (MainForm._appconf.formtype=="ใบเสร็จอย่างย่อ") formname = "slip.frx";
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
                    dtreport.Rows[0]["ArCode"] = ArCode.Text;
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
                    //fr.Show();
                    ////else
                    fr.Print();
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
                  dtreport.Rows[0]["ArCode"] = ArCode.Text;
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

        private void newNavBar_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            SearchBill serchbill = new SearchBill();
            serchbill.myTitle = "ค้นหาเอกสาร";
            serchbill.sqltext = "SELECT DOCDATE,DOCNO,TAXNO,(SELECT NAME1 FROM BCAR WHERE BCAR.CODE=ARCODE) as ARCODE,TOTALAMOUNT FROM BCARINVOICE ";

            serchbill.dt.Columns.Add(new DataColumn("DOCDATE", typeof(String)));
            serchbill.dt.Columns.Add(new DataColumn("DOCNO", typeof(String)));
            serchbill.dt.Columns.Add(new DataColumn("TAXNO", typeof(Double)));
            serchbill.dt.Columns.Add(new DataColumn("ARCODE", typeof(String)));
            serchbill.dt.Columns.Add(new DataColumn("TOTALAMOUNT", typeof(Double)));
            serchbill.setGrid(new GridColumn() {FieldName="DOCDATE",Caption="วันที่",Name="DOCDATE",Visible=true,VisibleIndex=0},FormatType.DateTime);
            serchbill.setGrid(new GridColumn() { FieldName = "DOCNO", Caption = "เอกสาร", Name = "DOCNO", Visible = true, VisibleIndex = 0 },FormatType.None);
            serchbill.setGrid(new GridColumn() { FieldName = "TAXNO", Caption = "ใบกำกับภาษี", Name = "TAXNO", Visible = true, VisibleIndex = 0 },FormatType.None);
            serchbill.setGrid(new GridColumn() { FieldName = "ARCODE", Caption = "ลูกค้า", Name = "ARCODE", Visible = true, VisibleIndex = 0 },FormatType.None);
            serchbill.setGrid(new GridColumn() { FieldName = "TOTALAMOUNT", Caption = "ยอดรวม", Name = "TOTALAMOUNT", Visible = true, VisibleIndex = 0},FormatType.Numeric,"n");
            serchbill.preData();
            
            serchbill.wheretxt=new string[]{"DocDate","DocNo","ArCode"};
            if (serchbill.ShowDialog() == DialogResult.OK)
                if (serchbill.result != null)
                    LoadBill(serchbill.result);
            reloadBill = true;
                    //insertItem(serchbill.result);
            //this.textSearch.Text = "";
            
        }

        private void LoadBill(DataRow billno)
        {
            saleitemdt.Rows.Clear();
            using(SqlConnection scon=new SqlConnection(strcon))
            {
            if(scon.State==ConnectionState.Closed)scon.Open();
            using (SqlDataAdapter da = new SqlDataAdapter(string.Format("SELECT DOCNO,DOCDATE,ARCODE,TOTALAMOUNT,SALECODE,SUMCASHAMOUNT FROM BCARINVOICE WHERE DOCNO='{0}'",billno["DOCNO"]),scon))
            {
                DataTable dbill = new DataTable();
                da.Fill(dbill);
                if (dbill.Rows.Count > 0)
                {
                    DocNo.Text = dbill.Rows[0]["DOCNO"].ToString();
                    DocDate.Text = dbill.Rows[0]["DOCDATE"].ToString().Substring(0,10).Trim();
                    ArCode.Text = dbill.Rows[0]["ARCODE"].ToString();
                    ArName.Text = getSingleRow(string.Format("select name1 from bcar where code='{0}'", dbill.Rows[0]["ARCODE"].ToString()), "Name1");//dbill.Rows[0]["ARCODE"].ToString();
                    BillAddress.Text = getSingleRow(string.Format("select billaddress from bcar where code='{0}'", dbill.Rows[0]["ARCODE"].ToString()), "BillAddress");//dbill.Rows[0]["ARCODE"].ToString();
                    Telephone.Text = getSingleRow(string.Format("select telephone from bcar where code='{0}'", dbill.Rows[0]["ARCODE"].ToString()), "Telephone");//dbill.Rows[0]["ARCODE"].ToString();
                    SaleCode.EditValue = dbill.Rows[0]["SALECODE"].ToString();
                    //Department.Text = dbill.Rows[0]["DEPARTMENT"].ToString();
                    TotalAmount.Text = dbill.Rows[0]["TOTALAMOUNT"].ToString();
                    SumCashAmount.Text = dbill.Rows[0]["SUMCASHAMOUNT"].ToString();
                    SqlCommand scom = da.SelectCommand as SqlCommand;
                   // SELECT DISTINCT BCITEM.CODE,NAME1,STOCKQTY,BCITEMUNIT.NAME AS UNITNAME,SALEPRICE1
                    scom.CommandText =string.Format("SELECT ITEMCODE,ITEMNAME,QTY,UNITCODE,PRICE FROM BCARINVOICESUB WHERE DOCNO='{0}' order by RowOrder",dbill.Rows[0]["DOCNO"]);
                    DataTable ditbill = new DataTable();
                    da.Fill(ditbill);
                        foreach(DataRow dr in ditbill.Rows)
                            loaderItem(dr);
                
                }
            }
            if (MainForm._appconf.billlock == 1)
                gridView1.OptionsBehavior.Editable = false;
    
            
            }
        }

        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            PrintingBill("ใบเสร็จรับเงิน.frx");
            clearScreen();
        }

        private void delBar_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (!gridView1.OptionsBehavior.Editable) gridView1.OptionsBehavior.Editable = true;
            clearScreen();
            reloadBill = false;
           
        }

        private void delData() 
        {
            string sqlhead = string.Format("delete from bcarinvoice where docno='{0}'", DocNo.Text);
            string sqlsub = string.Format("delete from bcarinvoicesub where docno='{0}'", DocNo.Text);
            string sqltax = string.Format("delete from bcoutputtax where docno='{0}'", DocNo.Text);
            string sqltrans = string.Format("delete from bctrans where docno='{0}'", DocNo.Text);
            string sqltranssub = string.Format("delete from bctranssub where docno='{0}'", DocNo.Text);
            string sqlrecmoney = string.Format("delete from bcrecmoney where docno='{0}'", DocNo.Text);
            string sqlreceipt = string.Format("delete from bcreceipt1 where docno='{0}'", DocNo.Text);
            db.ExecuteNonQuery(sqlhead);
            db.ExecuteNonQuery(sqlsub);
            using (SqlConnection scon = new SqlConnection(strcon))
            {
                if (scon.State == ConnectionState.Closed) scon.Open();
                using (SqlCommand comm = new SqlCommand(sqlhead, scon))
                {
                    comm.ExecuteNonQuery();
                }
                using (SqlCommand comm = new SqlCommand(sqlsub, scon))
                {
                    comm.ExecuteNonQuery();
                }
                using (SqlCommand comm = new SqlCommand(sqltax, scon))
                {
                    comm.ExecuteNonQuery();
                }
                using (SqlCommand comm = new SqlCommand(sqltrans, scon))
                {
                    comm.ExecuteNonQuery();
                }
                using (SqlCommand comm = new SqlCommand(sqltranssub, scon))
                {
                    comm.ExecuteNonQuery();
                }
                using (SqlCommand comm = new SqlCommand(sqlrecmoney, scon))
                {
                    comm.ExecuteNonQuery();
                }
                using (SqlCommand comm = new SqlCommand(sqlreceipt, scon))
                {
                    comm.ExecuteNonQuery();
                }
            }
        
        }
        private void browseBar_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (saleitemdt.Rows.Count > 0)
            {
                if (MainForm._appconf.billlock != 1)
                {
                    if (MessageBox.Show("ต้องการลบข้อมูล ใช่หรือไม่?", "เตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        delData();
                        clearScreen();
                    }
                }
            }

        }
      


    }
        

    

    [Persistent("SearchItem")]
    public class SearchItem : XPLiteObject
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public double StockQty { get; set; }
        public double SalePrice1 { get; set; }
    }
    #region Billing
    public class Billing
    {

        private string billdoc;
        public string billDoc
        {
            get
            {
                return billdoc;
            }
            set
            {
                billdoc = value;
            }
        }
        private string billdate;
        public string billDate
        {
            get
            {
                return billdate;
            }
            set
            {
                billdate = value;
            }
        }
        private string arcode;
        public string arCode
        {
            get
            {
                return arcode;
            }
            set
            {
                arcode = value;
            }
        }

        private string billtemp;
        public string billTemp
        {
            get
            {
                return billtemp;
            }
            set
            {
                billtemp = value;
            }
        }
        private string billcashdisc;
        public string billCashDisc
        {
            get
            {
                return billcashdisc;
            }
            set
            {
                billcashdisc = value;
            }
        }
        private string billpercdisc;
        public string billPercDisc
        {
            get
            {
                return billpercdisc;
            }
            set
            {
                billpercdisc = value;
            }
        }
        private string billtotal;
        public string billTotal
        {
            get
            {
                return billtotal;
            }
            set
            {
                billtotal = value;
            }
        }
        private string billbalance;
        public string billBalance
        {
            get
            {
                return billbalance;
            }
            set
            {
                billbalance = value;
            }
        }
        private string creditno;
        public string creditNo
        {
            get
            {
                return creditno;
            }
            set
            {
                creditno = value;
            }
        }
        private string credittotal;
        public string creditTotal
        {
            get
            {
                return credittotal;
            }
            set
            {
                credittotal = value;
            }
        }
        private string creditreciept;
        public string creditReciept
        {
            get
            {
                return creditreciept;
            }
            set
            {
                creditreciept = value;
            }
        }
        private string creditbalance;
        public string creditBalance
        {
            get
            {
                return creditbalance;
            }
            set
            {
                creditbalance = value;
            }
        }
        private string creditbrand;
        public string creditBrand
        {
            get
            {
                return creditbrand;
            }
            set
            {
                creditbrand = value;
            }
        }
        private string cashtotal;
        public string cashTotal
        {
            get
            {
                return cashtotal;
            }
            set
            {
                cashtotal = value;
            }
        }
        private string cashreciept;
        public string cashReciept
        {
            get
            {
                return cashreciept;
            }
            set
            {
                cashreciept = value;
            }
        }
        private string cashbalance;
        public string cashBalance
        {
            get
            {
                return cashbalance;
            }
            set
            {
                cashbalance = value;
            }
        }
        private string cashchange;
        public string cashChange
        {
            get
            {
                return cashchange;
            }
            set
            {
                cashchange = value;
            }
        }
        private string couponno;
        public string couponNo
        {
            get
            {
                return couponno;
            }
            set
            {
                couponno = value;
            }
        }
        private string coupontotal;
        public string couponTotal
        {
            get
            {
                return coupontotal;
            }
            set
            {
                coupontotal = value;
            }
        }
        private string couponreciept;
        public string couponReciept
        {
            get
            {
                return couponreciept;
            }
            set
            {
                couponreciept = value;
            }
        }
        private string couponbalance;
        public string couponBalance
        {
            get
            {
                return couponbalance;
            }
            set
            {
                couponbalance = value;
            }
        }
        private string discountword;
        public string discountWord
        {
            get
            {
                return discountword;
            }
            set
            {
                discountword = value;
            }
        }
        private string sumdiscount;
        public string sumDiscount
        {
            get
            {
                return sumdiscount;
            }
            set
            {
                sumdiscount = value;
            }
        }
        private string machineno;
        public string MachineNo
        {
            get
            {
                return machineno;
            }
            set
            {
                machineno = value;
            }
        }


    }
    #endregion
}
