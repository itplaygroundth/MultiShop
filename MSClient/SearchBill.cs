using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SqlClient;
using System.Configuration;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;


namespace MultiShop
{
    public partial class SearchBill : DevExpress.XtraEditors.XtraForm
    {
        Timer timer1;
        int timerTicks = 0;
        int waitUntill = 10;
        int iAllRows = 0;
        int iPageOffset = 1;
        int iPageSize = 200;
        int currentRow = 0;
        BackgroundWorker work = new BackgroundWorker();
        public DataTable dt = new DataTable();
        private static string instancename = ConfigurationManager.AppSettings["instanceName"].ToString();
        private static string username = ConfigurationManager.AppSettings["username"].ToString();
        private static string password = ConfigurationManager.AppSettings["password"].ToString();
        private static string dbname = ConfigurationManager.AppSettings["dbname"].ToString();
        string strcon;
        private string _title = string.Empty;
        public string myTitle
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }
        private string searchresult=string.Empty;
        public string searchResult
        {
            get
            {
                return searchresult;
            }
            set
            {
                searchresult = value;
            }
        }
        public DataRow result { get; set; }
        
        public event EventHandler EnterClose;
        public string sqltext { get; set; }
        public string[] wheretxt { get; set; }
        public SearchBill()
        {
            InitializeComponent();
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Enabled = false;
            work.DoWork += new DoWorkEventHandler(work_DoWork);
            work.RunWorkerCompleted += new RunWorkerCompletedEventHandler(work_RunWorkerCompleted);
            simpleButton1.Click+=new EventHandler(simpleButton1_Click);
            simpleButton2.Click+=new EventHandler(simpleButton2_Click);
            simpleButton3.Click+=new EventHandler(simpleButton3_Click);
            firstBtn.Click+=new EventHandler(firstBtn_Click);
            strcon = string.Format(@"Data Source={0}; Initial Catalog={1};User Id={2};Password={3};", instancename, dbname, username, password);
            textSearch.KeyDown += new KeyEventHandler(textSearch_KeyDown);
        }

        void textSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode==Keys.Enter || e.KeyCode==Keys.Return)
            {
                
               // gridView2.MakeRowVisible(0, false);
               // gridView2.SelectRow(gridView2.FocusedRowHandle);
                SendKeys.Send("{TAB}");
                SendKeys.Send("{TAB}");
                e.Handled = true;
            }
            //throw new NotImplementedException();
        }

        void work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            iAllRows = Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(dt.Rows.Count / iPageSize))) + 1;
            if (dt.Rows.Count > 0)
            {
                
                gridControl2.DataSource = goToPage(1);
            }
            else
            {
               
                gridControl2.DataSource = null;
                simpleButton1.Enabled = false;
                simpleButton2.Enabled = false;
                simpleButton3.Enabled = false;
                firstBtn.Enabled = false;
            }
            currentRow = 0;

        }
        private static DataTable DoRowDoubleClick(GridView view, Point pt, DataTable tables)
        {
            var info = view.CalcHitInfo(pt);
            var newRow = new object[4];
            DataRow row = null;
            DataTable table = new DataTable();
            table = tables.Clone();
            if (info.InRow || info.InRowCell)
            {
                row = view.GetDataRow(info.RowHandle);

                newRow[0] = 1;
                newRow[1] = row[1].ToString();
                newRow[2] = row[4];
                newRow[3] = Convert.ToDouble(newRow[0]) * Convert.ToDouble(row[4]);

                DataRow rows = table.NewRow();
                rows.ItemArray = newRow;
                table.Rows.Add(rows);

            }
            //newRow[0] = 1;
            //newRow[1] = "ทดสอบ";
            //newRow[2] = 10;
            //newRow[3] = Convert.ToDouble(1) * Convert.ToDouble(10);

            //rows = table.NewRow();
            //rows.ItemArray = newRow;
            //table.Rows.Add(rows);
            return table;

        }
        void work_DoWork(object sender, DoWorkEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                string _where = "(";
                //dt = LoadData(string.Format("SELECT DISTINCT BCITEM.CODE,NAME1,STOCKQTY,BCITEMUNIT.NAME AS UNITNAME,SALEPRICE1 FROM BCITEM,BCITEMUNIT  WHERE  ((BCITEM.CODE  LIKE '%{0}%' ) OR (NAME1  LIKE '%{0}%' ) OR (BCITEMUNIT.NAME  LIKE '%{0}%' )) AND BCITEM.DEFSTKUNITCODE=BCITEMUNIT.CODE AND  BCITEM.ACTIVESTATUS <>2 AND BCITEM.ACTIVESTATUS <>3  AND  (BCITEM.Name1 Like '%{0}%' Or BCITEM.Code Like '%{0}%') ORDER BY BCITEM.CODE", textSearch.Text.ToString()));
                if (wheretxt.Length > 0)
                {
                    int l = wheretxt.Length;
                    foreach (string val in wheretxt)
                    {
                        _where += string.Format("({0} LIKE '%{1}%')", val, textSearch.Text.ToString());
                        if(l>1)
                        _where += " or ";
                        l -= 1;
                    }
                    _where += ")";
                }
                dt = LoadData(string.Format("{0} where {1}",sqltext,_where));//string.Format("SELECT DISTINCT BCITEM.CODE,NAME1,STOCKQTY,BCITEMUNIT.NAME AS UNITNAME,SALEPRICE1 FROM BCITEM,BCITEMUNIT  WHERE  ((BCITEM.CODE  LIKE '%{0}%' ) OR (NAME1  LIKE '%{0}%' ) OR (BCITEMUNIT.NAME  LIKE '%{0}%' )) AND BCITEM.DEFSTKUNITCODE=BCITEMUNIT.CODE AND  BCITEM.ACTIVESTATUS <>2 AND BCITEM.ACTIVESTATUS <>3  AND  (BCITEM.Name1 Like '%{0}%' Or BCITEM.Code Like '%{0}%') ORDER BY BCITEM.CODE", textSearch.Text.ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            timerTicks++;

            if (timerTicks > waitUntill && !work.IsBusy)
            {
                timer1.Stop();
                work.RunWorkerAsync();
            }
        }

        private void textSearch_EditValueChanged(object sender, EventArgs e)
        {
            if (!timer1.Enabled)
            {
                timer1.Start();
            }
            timerTicks = 0;
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
        

        public void preData()
        {
            //ArCode.Properties.DataSource = LoadData("Select code,name1,billaddress,telephone from bcar");
            //SaleCode.Properties.DataSource = LoadData("select code,name from bcsale");
            //if (dt.Columns.Count == 0)
            //{
            //    dt.Columns.Add(new DataColumn("CODE", typeof(String)));
            //    dt.Columns.Add(new DataColumn("NAME1", typeof(String)));
            //    dt.Columns.Add(new DataColumn("STOCKQTY", typeof(Double)));
            //    dt.Columns.Add(new DataColumn("UNITNAME", typeof(String)));
            //    dt.Columns.Add(new DataColumn("SALEPRICE1", typeof(Double)));
            //    dt.Columns.Add(dcol);
            //}

            dt = LoadData(sqltext);//"SELECT DISTINCT BCITEM.CODE,NAME1,STOCKQTY,BCITEMUNIT.NAME AS UNITNAME,SALEPRICE1 FROM BCITEM,BCITEMUNIT  WHERE ((BCITEM.CODE  LIKE '%%' ) OR (NAME1  LIKE '%%' ) OR (BCITEMUNIT.NAME  LIKE '%%' )) AND BCITEM.DEFSTKUNITCODE=BCITEMUNIT.CODE AND  BCITEM.ACTIVESTATUS <>2 AND BCITEM.ACTIVESTATUS <>3  ORDER BY BCITEM.CODE");
            iAllRows = Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(dt.Rows.Count / iPageSize))) + 1;
            //gridControl2.DataSource = goToPage(1);
        }
        public void setGrid(DevExpress.XtraGrid.Columns.GridColumn col, DevExpress.Utils.FormatType ftype = DevExpress.Utils.FormatType.None, string format = "")
        {
            gridView2.Columns.Add(col);
            col.DisplayFormat.FormatType = ftype;
            col.DisplayFormat.FormatString= format;

            
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (iPageOffset < iAllRows)
            {
                iPageOffset += 1;
                gridControl2.DataSource = goToPage(iPageOffset);
                labelControl11.Text = string.Format("หน้า {0} ", iPageOffset);
                simpleButton1.Enabled = true;
                if (iPageOffset == iAllRows-1)
                {
                    simpleButton2.Enabled = false;
                }
            }
            else
            {
                simpleButton2.Enabled = false;
                simpleButton1.Enabled = true;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (iPageOffset > 1)
            {
                iPageOffset -= 1;
                gridControl2.DataSource = goToPage(iPageOffset);

                labelControl11.Text = string.Format("หน้า {0} ", iPageOffset);
                if (iPageOffset == 1)
                {
                    simpleButton1.Enabled = false;
                    simpleButton2.Enabled = true;
                    simpleButton3.Enabled = true;
                }
                else
                {
                    
                    simpleButton2.Enabled = true;
                    simpleButton3.Enabled = true;
                }
            }
            else
            {
                simpleButton1.Enabled = false;
                simpleButton2.Enabled = true;
                simpleButton3.Enabled = true;
            }
        }

        private void firstBtn_Click(object sender, EventArgs e)
        {
            iPageOffset = 1;
            gridControl2.DataSource = goToPage(iPageOffset);
            labelControl11.Text = string.Format("หน้า {0} ", iPageOffset);
            simpleButton1.Enabled = false;
            simpleButton2.Enabled = true;
            simpleButton3.Enabled = true;
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            iPageOffset = Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(dt.Rows.Count / iPageSize)));
            gridControl2.DataSource = goToPage(iPageOffset);
            labelControl11.Text = string.Format("หน้า {0} ", iPageOffset);
            simpleButton2.Enabled = false;
            simpleButton3.Enabled = false;
        }
        private DataTable goToPage(int p)
        {
            return dt.AsEnumerable().OrderBy(f => f.ItemArray[0])
                   .Skip((p - 1) * iPageSize)
                   .Take(iPageSize).CopyToDataTable();
         
        }
        private DataTable goToPage(int p, DataTable dt)
        {
                return dt.AsEnumerable().OrderBy(f => f.ItemArray[0])
                      .Skip((p - 1) * iPageSize)
                      .Take(iPageSize).CopyToDataTable();
            
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {
         //   preData();
        }

        private void gridView2_DoubleClick(object sender, EventArgs e)
        {
            //EnterClose(sender, e);
            GridView view = (GridView)sender;
            Point pt = view.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo info = view.CalcHitInfo(pt);

            if (info.InRow || info.InRowCell)
            {
                result = view.GetDataRow(info.RowHandle);
                //searchresult = row[0].ToString();
                //EnterClose(this, null);
                this.Close();
                this.DialogResult=DialogResult.OK;

            }
            
        }

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GridView view = (GridView)sender;
                Point pt = view.GridControl.PointToClient(Control.MousePosition);
                GridHitInfo info = view.CalcHitInfo(pt);
                if (info.InRow || info.InRowCell)
                {
                    result = view.GetDataRow(info.RowHandle);
                    this.Close();
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    result = view.GetDataRow(currentRow);
                    this.Close();
                    this.DialogResult = DialogResult.OK;
                }
            }else
                if (e.KeyCode == Keys.Up)
                {
                    if (currentRow >0)
                        currentRow -= 1;
                  
                    gridView2.MakeRowVisible(currentRow, false);
                    gridView2.SelectRow(gridView2.FocusedRowHandle); 
                }
                else
                    if (e.KeyCode == Keys.Down)
                    {
                        if (currentRow <200)
                            currentRow += 1;

                        gridView2.MakeRowVisible(currentRow, false);
                        gridView2.SelectRow(gridView2.FocusedRowHandle); 
                    }else
                        if (e.KeyCode == Keys.F2)
                        {
                            //textSearch.Focus();
                            textSearch.Select();
                        }
                        else if (e.KeyCode == Keys.Home && e.KeyCode == Keys.Control)
                        {
                            if(iPageOffset>2)
                            firstBtn_Click(this, null);
                        }else
                            if (e.KeyCode == Keys.Home)
                            {
                                if(iPageOffset>1)
                                simpleButton1_Click(this, null); 
                            }else
                                if (e.KeyCode == Keys.End)
                                {
                                    if(iPageOffset<iAllRows-1)
                                    simpleButton2_Click(this, null);
                                }
                                else if (e.KeyCode == Keys.End && e.KeyCode == Keys.Control)
                                {
                                    if(iPageOffset<iAllRows-1)
                                    simpleButton3_Click(this, null);
                                }
                
        }

        private void ClearSearchTxt_Click(object sender, EventArgs e)
        {
            textSearch.Text = string.Empty;
            textSearch.Focus();
            firstBtn_Click(sender, e);
        }

       


     
    }
}