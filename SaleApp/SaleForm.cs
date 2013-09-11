using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Drawing;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Data.OleDb;
 


namespace SaleApp
{
    public partial class SaleForm : XtraForm
    {
        AutoHeightHelper helper;
        DataTable dtAr;
        DataTable dtItem;
        DataTable dtGridItem;
        
        private string strconn="";
        public DBConnect[] dataSource = new DBConnect[] { new DBConnect { company_name = "", database_name = "", provider_name = "", server_dsn = "" }, };
        private string reg_path;
        private xLogin xlogin;
        private bool _loginstate=true;
        private int popupselect=0;
        private string arcode="";
        private string bfitem="";
        private int bfrows=0;
        private bool includevat = false;
        private bool zerovat = false;
        private DataTable MainDtreceipt;
        string[] wordnum=new string[]
        {
            "","หนึ่ง","สอง","สาม","สี่","ห้า","หก","เจ็ด","แปด","เก้า","สิบ"
        };
       // string[] numbers = new string[]
       // {
       //     "1","2","3","4","5","6","7","8","9","10","11","20","000","0000","000000","0000000"
       // };
        public SaleForm()
        {
            InitializeComponent();
            InitSkinGallery();
            //InitGrid();
            PriceBox.EditValueChanged += new EventHandler(PriceBox_EditValueChanged);
            DiscountBox.EditValueChanged += new EventHandler(DiscountBox_EditValueChanged);
            DiscountBox.KeyDown += new KeyEventHandler(DiscountBox_KeyDown);
            PrintOutCombo.KeyDown += new KeyEventHandler(PrintOutCombo_KeyDown);
            taxrate.KeyDown += new KeyEventHandler(taxrate_KeyDown);
            PriceBox.KeyDown += new KeyEventHandler(PriceBox_KeyDown);
            QtyBox.KeyDown += new KeyEventHandler(QtyBox_KeyDown);
            gridView1.CustomRowCellEditForEditing += new CustomRowCellEditEventHandler(gridView1_CustomRowCellEditForEditing);
            DocDateBox.Text = DateTime.Now.ToShortDateString();
            TaxDateBox.Text = DateTime.Now.ToShortDateString();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(SaleForm_KeyDown);
            DocNoBox.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(DocNoBox_Closed);
            gridView1.CustomRowCellEditForEditing+=new CustomRowCellEditEventHandler(gridView1_CustomRowCellEditForEditing);
        }

     

        void SaleForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F7) 
            {
                simpleButton1_Click(null, null);
            }
            //throw new NotImplementedException();
        }

        void gridView1_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
        {
            popupselect=e.RowHandle;
            //throw new NotImplementedException();
        }

        void QtyBox_KeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.KeyCode == Keys.Enter || e.KeyCode==Keys.Down || e.KeyCode==Keys.Right || e.KeyCode==Keys.Left || e.KeyCode==Keys.Up)
            {
                //GridRowInfo rinfo = gridView1
                int[] rsl = gridView1.GetSelectedRows();
                TextEdit txt = sender as TextEdit;
                if (txt.Text.Trim().Length > 0)
                    dtGridItem.Rows[popupselect]["amount"] = (Convert.ToDouble(dtGridItem.Rows[popupselect]["price"]) * Convert.ToDouble(txt.Text)).ToString("#,##0.00");
            
                updateSumBill();
                DataRow dr = dtGridItem.NewRow();
                dtGridItem.Rows.Add(dr);
                AmountToWord();
            }
        }

        void PriceBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                updateSumBill();
                DataRow dr = dtGridItem.NewRow();
                dtGridItem.Rows.Add(dr);
                AmountToWord();
            }
            //throw new NotImplementedException();
        }

        void taxrate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                updateSumBill();
                AmountToWord();
                DataRow dr = dtGridItem.NewRow();
                dtGridItem.Rows.Add(dr);
            }
            //throw new NotImplementedException();
        }

        void PrintOutCombo_KeyDown(object sender, KeyEventArgs e)
        {
            if (popupselect == dtGridItem.Rows.Count - 1)
            {
                DataRow dr = dtGridItem.NewRow();
                dtGridItem.Rows.Add(dr);
            }
            //updateSumBill();
                
            //throw new NotImplementedException();
        }


        void DiscountBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double total = Convert.ToDouble(dtGridItem.Rows[popupselect]["qty"]) * Convert.ToDouble(dtGridItem.Rows[popupselect]["price"]);
                int[] rsl = gridView1.GetSelectedRows();
                TextEdit txt = sender as TextEdit;
                if (txt.Text.Trim().Length > 0)
                {
                    string[] temp = txt.Text.Split(',');
                    double[] disc = new double[temp.Length];
                    
                    foreach (string val in temp)
                    {
                        if (val.IndexOf('%') != -1)
                        {
                            string vals=val.Replace("%", string.Empty);
                            total = total-((total * Convert.ToInt16(vals)) / 100);
                        }
                        else
                        {
                            total -= Convert.ToInt16(val);
                        }

                    }

                    
                }
                dtGridItem.Rows[popupselect]["amount"] = total.ToString("#,##0.00");
                AmountToWord();
            }
        }

        void DiscountBox_EditValueChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            
            
            

            //int[] rsl = gridView1.GetSelectedRows();
            //TextEdit txt = sender as TextEdit;
            //if (txt.Text.Trim().Length > 0)
            //{
            //    string[] temp=txt.Text.Split(',');
            //    double[] disc = new double[temp.Length];
            //      double total = Convert.ToDouble(dtGridItem.Rows[popupselect]["amount"]);
            //    foreach (string val in temp)
            //    {
            //        if (val.IndexOf('%')!=-1)
            //        {
            //            total = (total * Convert.ToInt16(val)) / 100;
            //        }
            //        else
            //        {
            //            total -= Convert.ToInt16(val);
            //        }
                        
            //    }
              
            //    dtGridItem.Rows[popupselect]["amount"] =  total.ToString("#,##0.00");
            //}
        }

        void PriceBox_EditValueChanged(object sender, EventArgs e)
        {
            int[] rsl = gridView1.GetSelectedRows();
            TextEdit txt = sender as TextEdit;
            if (txt.Text.Trim().Length > 0)
                dtGridItem.Rows[popupselect]["amount"] = (Convert.ToDouble(dtGridItem.Rows[popupselect]["qty"]) * Convert.ToDouble(txt.Text)).ToString("#,##0.00");
            //updateSumBill();         
            //throw new NotImplementedException();
        }

        private void loadDepartCode()
        {
            using (SqlConnection scon = new SqlConnection(strconn))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("select code,name from bcdepartment", scon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    foreach(DataRow dr in dt.Rows)
                        departmentcode.Properties.Items.Add(string.Format("{0}/{1}",dr["code"],dr["name"]));

                }
            }
        }

        private void loadSale()
        {
            using (SqlConnection scon = new SqlConnection(strconn))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("select code,name from bcsale", scon))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    
                    foreach (DataRow dr in dt.Rows)
                         cashiercode.Properties.Items.Add(string.Format("{0}/{1}", dr["code"], dr["name"]));
                }
            }
        }
    
        void InitSkinGallery()
        {
           // SkinHelper.InitSkinGallery(rgbiSkins, true);
            dtGridItem=new DataTable();
            dtGridItem.Columns.Add("ItemCode",typeof(string));
            dtGridItem.Columns.Add("SerialNo", typeof(string));
            dtGridItem.Columns.Add("ItemName",typeof(string));
            dtGridItem.Columns.Add("UnitName", typeof(string));
            dtGridItem.Columns.Add("Whcode", typeof(string));
            dtGridItem.Columns.Add("Shelfcode", typeof(string));
            dtGridItem.Columns.Add("Qty", typeof(double));
            dtGridItem.Columns.Add("Price", typeof(double));
            dtGridItem.Columns.Add("DiscountAmount", typeof(string));
            dtGridItem.Columns.Add("Amount", typeof(double));
            dtGridItem.Columns.Add("SendType", typeof(string));
            dtGridItem.Columns.Add("PrintOut", typeof(string));
            DataRow dnew = dtGridItem.NewRow();
            dtGridItem.Rows.Add(dnew);
            itemgrid.DataSource = dtGridItem;
            TimeSpan sp = new TimeSpan(Convert.ToInt16(crdays.Text), 0, 0, 0);
            duedate.Text = DateTime.Now.Add(sp).ToShortDateString();
           // loadWareHouse();
           // loadShelf();

            //gridView1.Columns.Add(new GridColumn() { Name = "ItemCode", FieldName = "ItemCode" });
            //gridView1.Columns.Add(new GridColumn() { Name = "ItemName", FieldName = "ItemName" });
        }

        private void loadWareHouse()
        {
            DataTable dt = new DataTable();
            using (SqlConnection scon = new SqlConnection(strconn)) 
            {
                using (SqlDataAdapter da = new SqlDataAdapter("select code,name from bcwarehouse", scon))
                {
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            WhCodeCombo.Items.Add(string.Format("{0}",dr["code"],dr["name"]));
                        }
                    }

                }
            }
        }
        private void loadShelf()
        {
            DataTable dt = new DataTable();
            using (SqlConnection scon = new SqlConnection(strconn))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("select code,name from bcshelf", scon))
                {
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            ShelfCodeCombo.Items.Add(string.Format("{0}", dr["code"], dr["name"]));
                        }
                    }

                }
            }
        }
        private void SaleForm_Load(object sender, EventArgs e)
        {
           
            itemgrid.ForceInitialize();
            helper = new AutoHeightHelper(gridView1);
            helper.EnableColumnPanelAutoHeight();
            xlogin = new xLogin();
            xlogin.CloseDialog += new EventHandler(xlogin_CloseDialog);
            xlogin.ShowDialog();
        }

        void xlogin_CloseDialog(object sender, EventArgs e)
        {
            try
            {
                if (xlogin._result == DialogResult.OK)
                {
                    if (xlogin.passwd.Length == 0)
                    {
                        MessageBox.Show("กรุณาใส่รหัสผ่าน");
                    }
                    else
                    {
                        loadConnection();
                        strconn = createConn(xlogin.dbname, xlogin.userid, xlogin.passwd);
                        using (SqlConnection scon = new SqlConnection(strconn))
                        {
                            if (scon.State == ConnectionState.Closed) scon.Open();
                            DataTable dt = new DataTable();

                            using (SqlDataAdapter da = new SqlDataAdapter("select goupcode,goupdesc from bcsystrdocg where TransKey='BillingTransConfig' order by linenumber ", scon))
                            {
                                da.Fill(dt);
                            }
                            foreach (DataRow dr in dt.Rows)
                                DocNoBox.Properties.Items.Add(string.Format("{0}={1}",dr["goupcode"],dr["goupdesc"]));
                            _loginstate = true;

                        }
                        loadBCAR();
                        loadBCItem();
                        loadWareHouse();
                        loadShelf();
                        //loadDepartCode();
                        //loadSale();
                        xlogin.Close();

                    }
                }
                else
                {
                    _loginstate = false;
                    this.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            //throw new NotImplementedException();
        }
    

        private void loadBCAR()
        {
            dtAr = new DataTable();
            using(SqlConnection scon=new SqlConnection(strconn))
            {
                using(SqlDataAdapter da=new SqlDataAdapter("select code,name1,billaddress,telephone,debtamount from bcar",scon))
                {
                    da.Fill(dtAr);
                    //this.gridControl1.DataSource = ds.Tables["Orders"];
                    
                    //this.repositoryItemSearchLookUpEdit1.DataSource = ds.Tables["Customers"];
                    //this.repositoryItemSearchLookUpEdit2.DisplayMember = "FirstName";
                    //this.repositoryItemSearchLookUpEdit2.DataSource = ds.Tables["Employees"];
                    ArCodeBox.Properties.DataSource = dtAr;
                     ArCodeBox.Properties.DisplayMember = "code";
                    ArCodeBox.Properties.ValueMember = "code";
                   //ItemCodeFind.DataSource=dtAr;
                   
                }

            }

        }
        private void loadBCItem()
        {
            dtItem = new DataTable();
            DataTable dconf = new DataTable();
            string sql = "select ";
            string jointable="";
            string sortby="";
            string filterby="";
            //dtItem.Columns.Add(new DataColumn() {ColumnName="ItemCode",Caption="รหัสสินค้า"});
            //dtItem.Columns.Add(new DataColumn() { ColumnName = "Name1", Caption = "ชื่อสินค้า" });
            //dtItem.Columns.Add(new DataColumn() { ColumnName = "UnitName", Caption = "หน่วยนับ" });
            //dtItem.Columns.Add(new DataColumn() { ColumnName = "stockqty", Caption = "ยอดคงเหลือ" });
            //dtItem.Columns.Add(new DataColumn() { ColumnName = "ราคา1", Caption = "ราคา1" });
            //dtItem.Columns.Add(new DataColumn() { ColumnName = "ราคา2", Caption = "รหัสสินค้า" });
            //dtItem.Columns.Add(new DataColumn() { ColumnName = "ราคา3", Caption = "รหัสสินค้า" });
            //dtItem.Columns.Add(new DataColumn() { ColumnName = "ราคา4", Caption = "รหัสสินค้า" });

            using (SqlConnection scon = new SqlConnection(strconn))
            {
                //using (SqlDataAdapter da = new SqlDataAdapter("select code as ItemCode,name1 as ItemName,defsaleunitcode,stockqty,saleprice1,saleprice2,saleprice3,saleprice4,defsalewhcode,defsaleshelf from bcitem", scon))
                using (SqlDataAdapter da = new SqlDataAdapter("select FieldName,ColText,ColWidth,ColWidth,JoinTable,FilterBy,Bcsystrformat.SortBy as SortBy from BCSysTRFind,BCSysTRFormat where BCSysTRFormat.TransKey=BCSysTRFind.TransKey and BCSysTRFind.TransKey='ItemTransConfig'", scon))
                {
                    da.Fill(dconf);
                    foreach (DataRow dr in dconf.Rows)
                    {
                        if(dr["FieldName"].ToString().Contains("ItemCode"))
                        dtItem.Columns.Add(new DataColumn() { ColumnName ="ItemCode", Caption = dr["Coltext"].ToString().Trim() });
                        else
                        if (dr["FieldName"].ToString().Contains("UnitName"))
                            dtItem.Columns.Add(new DataColumn() { ColumnName = "UnitName", Caption = dr["Coltext"].ToString().Trim() });
                        else
                            dtItem.Columns.Add(new DataColumn() { ColumnName = dr["FieldName"].ToString().Trim(), Caption = dr["Coltext"].ToString().Trim() });

                       // repositoryItemSearchLookUpEdit1View.Columns.Add(new GridColumn(){Name= dr["Coltext"].ToString().Trim(),FieldName=dr["FieldName"].ToString().Trim() });
                        sql += string.Format("{0},", dr["FieldName"].ToString().Trim());
                        jointable=dr["JoinTable"].ToString();
                        sortby=dr["SortBy"].ToString();
                        filterby=dr["FilterBy"].ToString();
                        
                    }
                    sql = sql.Remove(sql.Length-1);
                    sql += string.Format(" from {0} where {1} order by {2} ",jointable,filterby,sortby);
                    //this.gridControl1.DataSource = ds.Tables["Orders"];
                    using (SqlDataAdapter daa = new SqlDataAdapter(sql, scon))
                    {
                        daa.Fill(dtItem);
                        //this.repositoryItemSearchLookUpEdit1.DataSource = ds.Tables["Customers"];
                        //this.repositoryItemSearchLookUpEdit2.DisplayMember = "FirstName";
                        //this.repositoryItemSearchLookUpEdit2.DataSource = ds.Tables["Employees"];
                        this.ItemCodeFind.DataSource = dtItem;
                        this.ItemCodeFind.DisplayMember = "ItemCode";
                        this.ItemCodeFind.ValueMember = "ItemCode";
                        //.Properties.DataSource = dtAr;
                        //ArCodeBox.Properties.DisplayMember = "code";
                        //ArCodeBox.Properties.ValueMember = "code";
                        //ItemCodeFind.DataSource=dtAr;

                    }

                }
            }
        }


        void ItemCodeFind_Popup(object sender, System.EventArgs e)
        {
            int[] rsel=gridView1.GetSelectedRows();
            popupselect = rsel[0];
            bfitem=dtGridItem.Rows[popupselect][0].ToString();
            bfrows=dtGridItem.Rows.Count;
            //throw new System.NotImplementedException();
        }


        void ItemCodeFind_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            //GridView view = ItemCodeFind.View;
            
            SearchLookUpEdit view = sender as SearchLookUpEdit;
            DataRow drnew = dtGridItem.NewRow();
            if(dtGridItem.Rows.Count>1)
            dtGridItem.AsEnumerable().Where(row => String.IsNullOrEmpty(row.Field<string>("ItemCode"))).ToList().ForEach(row => row.Delete());
            
            dtGridItem.AcceptChanges();
            if (view.EditValue != null && view.EditValue.ToString().Trim().Length>0)
            {
                var dsel = dtItem.Select(string.Format("ITEMCODE='{0}'", view.EditValue));
                if (ArCodeBox.Text.Length == 0)
                {
                    view.EditValue = "";
                    MessageBox.Show("กรุณากำหนดรหัสลูกค้า!");

                }
                else
                    using (SqlConnection scon = new SqlConnection(strconn))
                    {
                        DataTable dprice = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(string.Format("select pricelevel from bcar where code='{0}'", ArCodeBox.Text), scon))
                        {
                            da.Fill(dprice);
                        }
                        string saleprice = "";
                        if (dprice.Rows.Count > 0)
                            saleprice = string.Format("saleprice{0}", dprice.Rows[0]["pricelevel"]);
                        else
                            saleprice = "saleprice1";
                        foreach (DataRow dr in dsel)
                            using (SqlDataAdapter da = new SqlDataAdapter(string.Format("select '{0}' as itemcode,'{1}' as itemname,'{2}' as unitname,defsalewhcode as whcode,defsaleshelf as shelfcode,{3} as price,'1' as qty,{3}*1 as amount from bcitem where code='{0}'", dr["itemcode"], dr["name1"].ToString().Replace("'", "''"), dr["unitname"], saleprice), scon))
                            {
                                DataTable dtt=new DataTable();
                                da.Fill(dtt);
                                if (popupselect <= dtGridItem.Rows.Count - 1) 
                                    {
                                     foreach(DataRow ddr in dtt.Rows)
                                        {
                                             dtGridItem.Rows[popupselect]["ItemCode"]=ddr["ItemCode"];
                                             dtGridItem.Rows[popupselect]["ItemName"] = ddr["ItemName"];
                                             dtGridItem.Rows[popupselect]["unitname"] = ddr["unitname"];
                                             dtGridItem.Rows[popupselect]["WhCode"] = ddr["WhCode"];
                                             dtGridItem.Rows[popupselect]["ShelfCode"] = ddr["ShelfCode"];
                                             dtGridItem.Rows[popupselect]["Price"] = ddr["Price"];
                                             dtGridItem.Rows[popupselect]["Qty"] = ddr["Qty"];
                                             dtGridItem.Rows[popupselect]["Amount"] = ddr["Amount"];
                                             
                                        }
                                     updateSumBill();
                                     AmountToWord();
                                        if(dtGridItem.Rows.Count<bfrows)
                                            {
                                             drnew=dtGridItem.NewRow();
                                             dtGridItem.Rows.Add(drnew);
                                            }else
                                            if (dtGridItem.Rows.Count == 1)
                                            {
                                                drnew = dtGridItem.NewRow();
                                                dtGridItem.Rows.Add(drnew);
                                            }

                                    }else
                                    {
                                        
                                        foreach(DataRow ddr in dtt.Rows)
                                        {
                                            drnew["ItemCode"]=ddr["ItemCode"];
                                            drnew["ItemName"]=ddr["ItemName"];
                                            drnew["unitname"]=ddr["unitname"];
                                            drnew["WhCode"]=ddr["WhCode"];
                                            drnew["ShelfCode"]=ddr["ShelfCode"];
                                            drnew["Price"]=ddr["Price"];
                                            drnew["Qty"]=ddr["Qty"];
                                            drnew["Amount"]=ddr["Amount"];
                                        }
                                        
                                        dtGridItem.Rows.Add(drnew);
                                        updateSumBill();
                                        AmountToWord();
                                        drnew=dtGridItem.NewRow();
                                        dtGridItem.Rows.Add(drnew);
                                       // gridView1.FocusedRowHandle = this.popupselect+1;

                                    }
                            }
                        
                        //updateSumBill();
                       // updateSumBill(sumitemamount,0.00,0.00,sumitemamount,sumitemamount,sumitemamount);
                        //itemgrid.RefreshDataSource();
                        //itemgrid.ForceInitialize();
                        //gridView1.FocusedRowHandle = dtGridItem.Rows.Count;
                       // gridView1.ShowEditor();
                    }
               
            }else
            //if (dtGridItem.Rows.Count == gridView1.RowCount)
            {
                drnew = dtGridItem.NewRow();
                dtGridItem.Rows.Add(drnew);
                
            }
            gridView1.FocusedRowHandle = popupselect+1;
           // int rowHandle = view.FocusedRowHandle;
           // string fieldName = "ItemCode"; // or other field name
           // object value = view.GetRowCellValue(rowHandle, fieldName);
            
           // 
            //DataRow dr=dtGridItem.NewRow();
            //dr["ItemCode"]=dtItem.Rows[0]["ItemCode"];
            //dtGridItem.Rows.Add(dr);
           // gridView1.DataSource = dtGridItem;

           // itemgrid.DataSource=dtGridItem;
           // dr["ItemCode"]=dtItem.Rows[0]["ItemCode"];
            //  throw new System.NotImplementedException();
        }
        void QtyBox_EditValueChanged(object sender, System.EventArgs e)
        {
            //int[] rsl=gridView1.GetSelectedRows();
            //TextEdit txt = sender as TextEdit;
            //if(txt.Text.Trim().Length>0)
            //dtGridItem.Rows[popupselect]["amount"] = (Convert.ToDouble(dtGridItem.Rows[popupselect]["price"]) * Convert.ToDouble(txt.Text)).ToString("#,##0.00");
            //updateSumBill();
            // throw new System.NotImplementedException();
        }

      


           public static string ReadReg(string Node,string KeyName)
        {
            // Opening the registry key
            //RegistryKey SUBKEY;
            var rk = RegistryKey.OpenRemoteBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, "");
            //RegistryKey rk = Registry.CurrentUser;
            // Open a subKey as read-only

            var sk1 = rk.OpenSubKey(string.Format("SOFTWARE\\BanChiang Soft\\{0}",Node));
            // If the RegistrySubKey doesn't exist -> (null)

            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value

                    // or null is returned.

                    return (string)sk1.GetValue(KeyName.ToUpper());
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!

                    MessageBox.Show(e.Message);
                    return null;
                }
            }
        }

        private void loadConnection()
        {
            var ds = new DataSet();

          
            if (ds.Tables.Count == 0)
            {
                reg_path = ReadReg("BC5APP","BC5APPData");
                ds = QuerytoOLE(String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\\BCDATA.mdb;", reg_path), "SELECT * from dbconnection ");
                
            }
            var dbconn = ds.Tables[0];
            var query3 = from o in dbconn.AsEnumerable() select new { ServerDSN = o.Field<string>("server_dsn"), DB_Name = o.Field<string>("database_name"), Company_Name = o.Field<string>("company_name"), Provider_Name = o.Field<string>("provider") };
            dataSource = new DBConnect[ds.Tables[0].Rows.Count];
            int i = 0;
            foreach (var res in query3)
            {
                //thText.Properties.Items.Add(res.Company_Name.ToString());
                var mem = new DBConnect();
                mem.company_name = res.Company_Name;
                mem.database_name = res.DB_Name;
                mem.provider_name = res.Provider_Name;
                mem.server_dsn = res.ServerDSN;

                dataSource[i] = mem;
                i += 1;
                //str_conn = "Data Source=" + res.ServerDSN + ";Initial Catalog=" + res.DB_Name + ";";
            }
        }

           private static DataSet QuerytoOLE(string strconn, string sql)
        {
            OleDbDataReader rd;
            var ds = new DataSet();
            var dt = new DataTable();
            try
            {
                using (var conn = new OleDbConnection(strconn))
                {
                    conn.Open();
                    using (var comm = new OleDbCommand(sql, conn))
                    {
                        rd = comm.ExecuteReader();
                        for (int i = 0; i < rd.FieldCount; i++)
                            dt.Columns.Add(rd.GetName(i), rd.GetFieldType(i));
                        dt.BeginLoadData();
                        Object[] values = new Object[rd.FieldCount];

                        while (rd.Read())
                        {
                            rd.GetValues(values);
                            DataRow loadDataRow = dt.LoadDataRow(values, true);
                        }
                        dt.EndLoadData();
                        ds.Tables.Add(dt);

                    }
                }
            }
            catch (OleDbException ex)
            {
                //if(ex.ErrorCode<0)

                //MessageBox.Show(ex.Message);
            }

            return ds;

        }

        private string createConn(string dbname,string userid,string password)
        {
            string str_conn = "";
            var conn = from d in dataSource where d.company_name.ToUpper() == dbname.ToUpper() select d;
            foreach (var res in conn)
            {

                str_conn = String.Format("Server={0};Database={1};user id={2};password={3}", res.server_dsn, res.database_name,userid,password);

            }
            return str_conn;

        }

        private void DocNoBox_Popup(object sender, EventArgs e)
        {
            //if (_loginstate)
            //{
            //    DataTable dt=new DataTable
            //    using (SqlConnection scon = new SqlConnection(strconn))
            //    {
            //        if (scon.State == ConnectionState.Closed) scon.Open();


            //        using (SqlDataAdapter da = new SqlDataAdapter(string.Format("select max(docno) from bcarinvoice where docno like '{0}%' ",DocNoBox.Properties.Items[DocNoBox.SelectedIndex].ToString()), scon))
            //        {
            //            da.Fill(dt);
            //            if (dt.Rows.Count > 0)
            //            {
            //                DocNoBox.Text = dt.Rows[0][1].ToString();
            //            }
            //        }
            //    }
            //}
        }
        void DocNoBox_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            //throw new NotImplementedException();
            //if (_loginstate)
            //{
                DataTable dt = new DataTable();
                ComboBoxEdit cbedit = sender as ComboBoxEdit;
                string docseltext = "";
                if (cbedit.Text.IndexOf("=") > 0) docseltext = cbedit.Text.Substring(0, cbedit.Text.IndexOf("="));
                //if (DocNoBox.Text.IndexOf("=") > 0) docseltext = DocNoBox.Text.Substring(0, DocNoBox.Text.IndexOf("="));
                else
                    docseltext = cbedit.Text;

                string docnotmp = "";
                int coun = 0;
                string docoun = "";
                using (SqlConnection scon = new SqlConnection(strconn))
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
                    scom = new SqlCommand(string.Format("select max(docno) from bcarinvoice where docno like '{0}%' ", docseltext), scon);
                    if (docseltext != string.Empty)
                        using (SqlDataAdapter da = new SqlDataAdapter(scom))
                        {
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                                if (dt.Rows[0][0].ToString() != "")
                                {
                                    coun = Convert.ToInt16(dt.Rows[0][0]) + 1;
                                    for (int i = 0; i < 5; i++) { docoun += string.Format("0{0}", coun); }
                                    DocNoBox.Text = docnotmp + docoun.Substring(0, 4);
                                    TaxNoBox.Text = docnotmp + docoun.Substring(0, 4);
                                }
                                else
                                {
                                    DocNoBox.Text = docnotmp.Replace("####", "0001");
                                    TaxNoBox.Text = docnotmp.Replace("####", "0001");
                                }
                        }

                    //docnotmp = docnotmp.Replace("####", "0001");
                    //DocNoBox.Text = docnotmp;
                    //}
                }
            //}
        }
        private void DocNoBox_QueryCloseUp(object sender, CancelEventArgs e)
        {
            //if (_loginstate)
            //{
            //    DataTable dt = new DataTable();
            //    ComboBoxEdit cbedit = sender as ComboBoxEdit;
            //    string docseltext = "";
            //    if(cbedit.Text.IndexOf("=") > 0) docseltext = cbedit.Text.Substring(0, cbedit.Text.IndexOf("="));
            //    //if (DocNoBox.Text.IndexOf("=") > 0) docseltext = DocNoBox.Text.Substring(0, DocNoBox.Text.IndexOf("="));
            //    else
            //        docseltext = cbedit.Text;

            //    string docnotmp = "";
            //    int coun=0;
            //    string docoun = "";
            //    using (SqlConnection scon = new SqlConnection(strconn))
            //    {
            //        if (scon.State == ConnectionState.Closed) scon.Open();
            //        SqlCommand scom = new SqlCommand(string.Format("select DocFormat from BCSysTRFormat where TransKey='BillingTransConfig'"), scon);
            //    //    if (DocNoBox.Text == string.Empty)
            //    //    {
            //            dt=new DataTable();
            //            scom=new SqlCommand(string.Format("select DocFormat from BCSysTRFormat where TransKey='BillingTransConfig'"),scon);
            //            using(SqlDataAdapter da=new SqlDataAdapter(scom))
            //            {
            //                da.Fill(dt);
            //                if(dt.Rows.Count>0)
            //                {
            //                    docnotmp=dt.Rows[0][0].ToString();
            //                    docnotmp=docnotmp.Replace("@@@",docseltext);
            //                    docnotmp=docnotmp.Replace("YY",(Convert.ToInt16(DateTime.Now.Year.ToString())+543).ToString().Substring(2,2));
            //                    docnotmp = docnotmp.Replace("MM", DateTime.Now.Month.ToString().Length < 2 ? "0"+DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString());
            //                    docnotmp=docnotmp.Replace("DD",DateTime.Now.Day.ToString().Length<2? "0"+DateTime.Now.Day.ToString():DateTime.Now.Day.ToString());
                                
            //                }
                        
            //            }
            //            dt = new DataTable();
            //            scom = new SqlCommand(string.Format("select max(docno) from bcarinvoice where docno like '{0}%' ", docseltext), scon);
            //            if(docseltext!=string.Empty)
            //            using (SqlDataAdapter da = new SqlDataAdapter(scom))
            //            {
            //                da.Fill(dt);
            //                if (dt.Rows.Count > 0)
            //                if(dt.Rows[0][0].ToString()!="")
            //                {
            //                    coun = Convert.ToInt16(dt.Rows[0][0]) + 1;
            //                    for (int i = 0; i < 5; i++) { docoun += string.Format("0{0}", coun); }
            //                    DocNoBox.Text = docnotmp + docoun.Substring(0, 4);
            //                    TaxNoBox.Text = docnotmp + docoun.Substring(0, 4);
            //                }
            //                else
            //                {
            //                    DocNoBox.Text = docnotmp.Replace("####","0001");
            //                    TaxNoBox.Text = docnotmp.Replace("####", "0001");
            //                }
            //            }
                    
            //            //docnotmp = docnotmp.Replace("####", "0001");
            //            //DocNoBox.Text = docnotmp;
            //        //}
            //    }
            //}
        }

        private void comboBoxEdit3_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            //int[] sid = ArSearch.GetSelectedRows();
            DataTable sid = ArCodeBox.Properties.DataSource as DataTable;
            DataRow[] drr = sid.Select(string.Format("code='{0}'",ArCodeBox.Text));
            
           // DataRow dr = ArSearch.GetDataRow(sid[0]);
            foreach (DataRow dr in drr)
            {
                arcode = dr.ItemArray[0].ToString();
                ArNameBox.Text = dr.ItemArray[1].ToString();
            }
            if (dtGridItem.Rows.Count == 0)
            {
                DataRow dr = dtGridItem.NewRow();
                dtGridItem.Rows.Add(dr);
            }

        }

        private void BillTaxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BillTaxType.SelectedIndex == 1)
                includevat = true;
            else if (BillTaxType.SelectedIndex == 2)
            {
                zerovat = true;
                TaxNoBox.EditValue = "";
                TaxDateBox.EditValue = "";
            }
            else if (BillTaxType.SelectedIndex == 0)
            {
                includevat = false; zerovat = false;
            }
            updateSumBill();
            AmountToWord();
            DataRow dr = dtGridItem.NewRow();
            dtGridItem.Rows.Add(dr);
        }


        private void updateSumBill()
        {
            double sumamount=0.00,sumexceptamount=0.00,sumdiscountamount=0.00,sumtaxamount=0.00,sumbillamount=0.00,sumnetamount=0.00;
            dtGridItem.AsEnumerable().Where(row => String.IsNullOrEmpty(row.Field<string>("ItemCode"))).ToList().ForEach(row => row.Delete());
            double sumitemamount = Convert.ToDouble(dtGridItem.AsEnumerable().Sum(x => x.Field<double>("Amount")));
            sumamount = sumitemamount;
            int rate = (100 + Convert.ToInt16(taxrate.Text));// / 100;
            double taxRate = Convert.ToDouble(rate) / 100;
            //if (!includevat && zerovat)
            //    sumitemamount = sumitemamount / taxRate;
            //else 
            sumitemamount = sumitemamount * taxRate;
            if (!includevat && !zerovat)
            {
                
                SumItemAmount.Text = sumamount.ToString("#,##0.00");
                ExceptTaxAmount.Text = sumexceptamount.ToString("#,##0.00");
                SumDiscountAmount.Text = sumdiscountamount.ToString("#,##0.00");
                SumTaxAmount.Text = (sumitemamount-sumamount).ToString("#,##0.00");
                SumBillAmount.Text = sumitemamount.ToString("#,##0.00");
                SumNetAmount.Text = sumamount.ToString("#,##0.00");
            }else
                if (zerovat)
                {
                    SumItemAmount.Text = sumamount.ToString("#,##0.00");
                    ExceptTaxAmount.Text = sumamount.ToString("#,##0.00");
                    SumDiscountAmount.Text = sumdiscountamount.ToString("#,##0.00");
                    SumTaxAmount.Text = sumtaxamount.ToString("#,##0.00");
                    SumBillAmount.Text = sumamount.ToString("#,##0.00");
                    SumNetAmount.Text = sumamount.ToString("#,##0.00");


                }
                else
                {
                    SumItemAmount.Text = sumamount.ToString("#,##0.00");
                    ExceptTaxAmount.Text = sumexceptamount.ToString("#,##0.00");
                    SumDiscountAmount.Text = sumdiscountamount.ToString("#,##0.00");
                    sumtaxamount=(sumitemamount-sumamount);
                    SumTaxAmount.Text = sumtaxamount.ToString("#,##0.00");
                    SumBillAmount.Text = sumamount.ToString("#,##0.00");
                    SumNetAmount.Text = (sumamount-sumtaxamount).ToString("#,##0.00");
                
                }
            //AmountToWord();
        }
        private void AmountToWord()
        {
            
            string [] temp = new string[SumBillAmount.Text.Substring(0,SumBillAmount.Text.IndexOf('.')).Replace(",","").Length];
            string strtemp=SumBillAmount.Text.Substring(0,SumBillAmount.Text.IndexOf('.')).Replace(",","");
            
            //int i = temp.Length-1;
            for (int k = 0; k < temp.Length; k++)
            {
                temp[k] = strtemp.Substring(k,1);
               // i -= 1;
            }
            //i = tdigi.Length-1;
            string[] tdigi = new string[SumBillAmount.Text.Substring(SumBillAmount.Text.IndexOf('.') + 1, 2).Length];
            strtemp = SumBillAmount.Text.Substring(SumBillAmount.Text.IndexOf('.') + 1, 2);
            for (int k = 0; k < tdigi.Length; k++)
            {
                tdigi[k] = strtemp.Substring(k, 1);
                // i -= 1;
            }
            string wording = "";
            int i = temp.Length-1;
            int j = 0;
            foreach(string val in temp)
            { 
                //if(temp.Length>11)
                //{
                switch(i)
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
                            if(temp.Length==2)
                                wording = "สิบ";
                                else
                                wording+="สิบ";
                            }
                            else
                            if (Convert.ToInt16(temp[j].ToString()) == 2)
                                wording += "ยี่สิบ";
                            else if (Convert.ToInt16(temp[j].ToString()) > 2)
                                wording += wordnum[Convert.ToInt16(temp[j].ToString())] + "สิบ";
                        break;
                 
                    case 2:
                        if(Convert.ToInt16(temp[j].ToString())>0)
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
                        wording += wordnum[Convert.ToInt16(temp[j].ToString())]+"ล้าน";
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
            wording = wording+"บาท";
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
                            else if (Convert.ToInt16(tdigi[j].ToString())> 2)
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
            if (Convert.ToInt16(SumBillAmount.Text.Substring(SumBillAmount.Text.IndexOf('.') + 1, 2))>0)
                wording = wording + "สตางค์";

            TotalAmountWord.Text = wording;
        }

        private void taxrate_EditValueChanged(object sender, EventArgs e)
        {
           // updateSumBill();
        }

        private void crdays_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TimeSpan sp = new TimeSpan(Convert.ToInt16(crdays.Text),0,0,0);
                duedate.Text = duedate.DateTime.Add(sp).ToShortDateString();
            }
        }

        private void departmentcode_QueryPopUp(object sender, CancelEventArgs e)
        {
            loadDepartCode();
        }

        private void cashiercode_QueryPopUp(object sender, CancelEventArgs e)
        {
            loadSale();
        }

        private void inboxItem_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            dtGridItem = new DataTable();
            DocNoBox.Text = "";
            DocDateBox.Text = "";
            TaxNoBox.Text = "";
            TaxDateBox.Text = "";
            ArCodeBox.Text = "";
            ArNameBox.Text = "";
            itemgrid.DataSource = dtGridItem;
            itemgrid.RefreshDataSource();
            updateSumBill();
            TotalAmountWord.Text = "";
            InitSkinGallery();
        }

        private void outboxItem_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            OpenForm opform = new OpenForm();
            opform.ShowDialog();
        }


        SaveForm svform;
        private void draftsItem_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (savecomplete)
            {
                PrintingForm print = new PrintingForm();
                if (print.ShowDialog() == DialogResult.OK)
                    inboxItem_LinkClicked(null, null);
            }
        }
        bool savecomplete = false;
        void svform_CloseSave(object sender, EventArgs e)
        {
            savecomplete = true;
           // MainDtreceipt.Rows.Clear();
            MainDtreceipt=svform.Dtreceipt;
            //svform.Amount;
            //throw new NotImplementedException();
        }

        private void trashItem_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (MessageBox.Show("ต้องการลบข้อมูล ใช่หรือไม่", "เตือน", MessageBoxButtons.YesNo) == DialogResult.Yes)
            { 
                
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (SumBillAmount.Text.Length > 0)
            {
                if (Convert.ToDouble(SumBillAmount.Text) > 0)
                {
                    svform = new SaveForm();
                    svform.CloseSave += new EventHandler(svform_CloseSave);
                    svform.Amount = Convert.ToDouble(SumBillAmount.Text)+Convert.ToDouble(transportAmount.EditValue);
                    svform.ArCode = ArCodeBox.Text;
                    svform.strconn = strconn;
                    if (MainDtreceipt == null || MainDtreceipt.Rows.Count==0)
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
                dr["CheqAmount"]=0.00;
                dr["CheqValue"]=0.00;
                dr["CheqBalance"]=0.00;
                dr["CouponValue"]=0.00;
                dr["DepositValue"]=0.00;
                dr["DepositBalance"]=0.00;
                dr["DepositAmount"]=0.00;
                dr["CashAmount"]=0.00;
                dr["BillAmount"] = Convert.ToDouble(SumBillAmount.Text);
                MainDtreceipt.Rows.Add(dr);
            }

        private void calendarItem_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (xlogin.userid == "sa")
            {
                ConfigForm cfgform = new ConfigForm();
                cfgform.ShowDialog();
            }
            else
                MessageBox.Show("ไม่มีสิทธิ์เข้าหน้าจอนี้!");
        }
    }

    public class AutoHeightHelper
    {
        GridView view;
        public AutoHeightHelper(GridView view)
        {
            this.view = view;
            EnableColumnPanelAutoHeight();
        }

        public void EnableColumnPanelAutoHeight()
        {
            SetColumnPanelHeight();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            view.ColumnWidthChanged += OnColumnWidthChanged;
            view.GridControl.Resize += OnGridControlResize;
            view.EndSorting += OnGridColumnEndSorting;
        }

        void OnGridColumnEndSorting(object sender, EventArgs e)
        {
            view.GridControl.BeginInvoke(new MethodInvoker(SetColumnPanelHeight));
        }

        void OnGridControlResize(object sender, EventArgs e)
        {
            SetColumnPanelHeight();
        }

        void OnColumnWidthChanged(object sender, DevExpress.XtraGrid.Views.Base.ColumnEventArgs e)
        {
            SetColumnPanelHeight();
        }

        private void SetColumnPanelHeight()
        {
            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
            int height = 0;
            for (int i = 0; i < view.VisibleColumns.Count; i++)
                height = Math.Max(GetColumnBestHeight(viewInfo, view.VisibleColumns[i]), height);
            view.ColumnPanelRowHeight = height;
        }

        private int GetColumnBestHeight(GridViewInfo viewInfo, GridColumn column)
        {
            GridColumnInfoArgs ex = viewInfo.ColumnsInfo[column];
            if (ex == null)
            {
                viewInfo.GInfo.AddGraphics(null);
                ex = new GridColumnInfoArgs(viewInfo.GInfo.Cache, null);
                try
                {
                    ex.InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(),
                                                            new GlyphElementInfoArgs(viewInfo.View.Images, 0, null),
                                                            StringAlignment.Near));
                    if (viewInfo.View.CanShowFilterButton(null))
                    {
                        ex.InnerElements.Add(viewInfo.Painter.ElementsPainter.FilterButton, new GridFilterButtonInfoArgs());
                    }
                    ex.SetAppearance(viewInfo.PaintAppearance.HeaderPanel);
                    ex.Caption = column.Caption;
                    ex.CaptionRect = new Rectangle(0, 0, column.Width - 20, 17);
                }
                finally
                {
                    viewInfo.GInfo.ReleaseGraphics();
                }
            }
            GraphicsInfo grInfo = new GraphicsInfo();
            grInfo.AddGraphics(null);
            ex.Cache = grInfo.Cache;
            bool canDrawMore = true;
            Size captionSize = CalcCaptionTextSize(grInfo.Cache, ex as HeaderObjectInfoArgs, column.GetCaption());
            Size res = ex.InnerElements.CalcMinSize(grInfo.Graphics, ref canDrawMore);
            res.Height = Math.Max(res.Height, captionSize.Height);
            res.Width += captionSize.Width;
            res = viewInfo.Painter.ElementsPainter.Column.CalcBoundsByClientRectangle(ex, new Rectangle(Point.Empty, res)).Size;
            grInfo.ReleaseGraphics();
            return res.Height;
        }

        Size CalcCaptionTextSize(GraphicsCache cache, HeaderObjectInfoArgs ee, string caption)
        {
            Size captionSize = ee.Appearance.CalcTextSize(cache, caption, ee.CaptionRect.Width).ToSize();
            captionSize.Height++; captionSize.Width++;
            return captionSize;
        }

        public void DisableColumnPanelAutoHeight()
        {
            UnsubscribeFromEvents();
        }

        private void UnsubscribeFromEvents()
        {
            view.ColumnWidthChanged -= OnColumnWidthChanged;
            view.GridControl.Resize -= OnGridControlResize;
            view.EndSorting -= OnGridColumnEndSorting;
        }
    }
}