using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using System.Data.SqlClient;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace SmartPOS.Controls
{
    public partial class GridPageControl : DevExpress.XtraEditors.XtraUserControl
    {

        public DataTable dt { get; set; }
        private int index = 0;
        Timer timer1;
        int timerTicks = 0;
        int waitUntill = 10;
        int iAllRows = 0;
        int iPageOffset = 1;
        int iPageSize = 200;
        int currentRow = 0;
        BackgroundWorker work = new BackgroundWorker();
        public string strcon { get; set; }
        private string _title = string.Empty;
        public string textsearch { get; set; }
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
        private string searchresult = string.Empty;
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

        public string[] conditionf { get; set; }
        public string[] conditionv { get; set; }

        public DataRow result { get; set; }

        public event EventHandler EnterClose;
        public string sqltext { get; set; }
        public string sqltext2 { get; set; }
        public string[] wheretxt { get; set; }
        public string[] wheretxt2 { get; set; }
        public string[] jointable { get; set; }
        public Boolean isserial { get; set; }
        public string serialno { get; set; }
        public GridPageControl()
        {
            InitializeComponent();
            dt = new DataTable();
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Enabled = false;
            work.DoWork += new DoWorkEventHandler(work_DoWork);
            work.RunWorkerCompleted += new RunWorkerCompletedEventHandler(work_RunWorkerCompleted);
            firstBtn.Click += new EventHandler(firstBtn_Click);
            prevBtn.Click += new EventHandler(prevBtn_Click);
            nextBtn.Click += new EventHandler(nextBtn_Click);
            lastBtn.Click += new EventHandler(lastBtn_Click);
            gridView1.DoubleClick += new EventHandler(gridView1_DoubleClick);
            gridView1.KeyDown += new KeyEventHandler(gridView1_KeyDown);
            gridView1.OptionsBehavior.Editable = false;
            isserial = false;
        }

        public void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.KeyCode == Keys.Enter)
            {
                //GridView view = (GridView)sender;
                Point pt = gridView1.GridControl.PointToClient(Control.MousePosition);
                GridHitInfo info = gridView1.CalcHitInfo(pt);
                if (dt.Rows.Count == 1 && sender.GetType() == typeof(DevExpress.XtraEditors.TextEdit))
                {
                    result = gridView1.GetDataRow(0);
                    EnterClose(sender, e);
                    return;
                }
                else
                {
                    if (info.InRow || info.InRowCell)
                    {
                        result = gridView1.GetDataRow(info.RowHandle);
                        //this.Close();
                        //this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        result = gridView1.GetDataRow(currentRow);
                        //this.Close();
                        //this.DialogResult = DialogResult.OK;
                    }
                }
            }
            else
                if (e.KeyCode == Keys.Up)
                {
                    if (currentRow > 0)
                        currentRow -= 1;

                    gridView1.MakeRowVisible(currentRow, false);
                    gridView1.SelectRow(gridView1.FocusedRowHandle);
                }
                else
                    if (e.KeyCode == Keys.Down)
                    {
                        if (currentRow < 200)
                            currentRow += 1;

                        gridView1.MakeRowVisible(currentRow, false);
                        gridView1.SelectRow(gridView1.FocusedRowHandle);
                    }
                    else
                        if (e.KeyCode == Keys.F2)
                        {
                            //textSearch.Focus();
                           // textSearch.Select();
                        }
                        else if (e.KeyCode == Keys.Home && e.KeyCode == Keys.Control)
                        {
                            if (iPageOffset > 2)
                                firstBtn_Click(this, null);
                        }
                        else
                            if (e.KeyCode == Keys.Home)
                            {
                                if (iPageOffset > 1)
                                    prevBtn_Click(this, null);
                            }
                            else
                                if (e.KeyCode == Keys.End)
                                {
                                    if (iPageOffset < iAllRows - 1)
                                        nextBtn_Click(this, null);
                                }
                                else if (e.KeyCode == Keys.End && e.KeyCode == Keys.Control)
                                {
                                    if (iPageOffset < iAllRows - 1)
                                        lastBtn_Click(this, null);
                                }
        }

        void gridView1_DoubleClick(object sender, EventArgs e)
        {
                //throw new NotImplementedException();
            GridView view = (GridView)sender;
            Point pt = view.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo info = view.CalcHitInfo(pt);

            if (info.InRow || info.InRowCell)
            {
                result = view.GetDataRow(info.RowHandle);
                //if(isserial)
                    
                //searchresult = row[0].ToString();
                EnterClose(this, null);
                //this.Close();
                //this.DialogResult = DialogResult.OK;

            }
        }

        void lastBtn_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            iPageOffset = Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(dt.Rows.Count / iPageSize)));
            gridControl1.DataSource = goToPage(iPageOffset);
            lblPage.Text = string.Format("หน้า {0} ", iPageOffset);
            nextBtn.Enabled = false;
            lastBtn.Enabled = false;
        }

        void nextBtn_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (iPageOffset < iAllRows)
            {
                iPageOffset += 1;
                gridControl1.DataSource = goToPage(iPageOffset);
                lblPage.Text = string.Format("หน้า {0} ", iPageOffset);
                prevBtn.Enabled = true;
                if (iPageOffset == iAllRows - 1)
                {
                    nextBtn.Enabled = false;
                }
            }
            else
            {
                nextBtn.Enabled = false;
                prevBtn.Enabled = true;
            }
        }

        void prevBtn_Click(object sender, EventArgs e)
        {
            if (iPageOffset > 1)
            {
                iPageOffset -= 1;
                gridControl1.DataSource = goToPage(iPageOffset);

                lblPage.Text = string.Format("หน้า {0} ", iPageOffset);
                if (iPageOffset == 1)
                {
                    prevBtn.Enabled = false;
                    nextBtn.Enabled = true;
                    lastBtn.Enabled = true;
                }
                else
                {

                    nextBtn.Enabled = true;
                    lastBtn.Enabled = true;
                }
            }
            else
            {
               prevBtn.Enabled = false;
                nextBtn.Enabled = true;
                lastBtn.Enabled = true;
            }
            //throw new NotImplementedException();
        }

        void firstBtn_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            iPageOffset = 1;
            gridControl1.DataSource = goToPage(iPageOffset);
            lblPage.Text = string.Format("หน้า {0} ", iPageOffset);
            prevBtn.Enabled = false;
            nextBtn.Enabled = true;
            lastBtn.Enabled = true;
        }

        void work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            iAllRows = Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(dt.Rows.Count / iPageSize))) + 1;
            if (dt.Rows.Count > 0)
            {

                gridControl1.DataSource = goToPage(1);
            }
            else
            {

                gridControl1.DataSource = null;
                firstBtn.Enabled = false;
                prevBtn.Enabled = false;
                nextBtn.Enabled = false;
                lastBtn.Enabled = false;
            }
            currentRow = 0;
        }

        void work_DoWork(object sender, DoWorkEventArgs e)
        {
           // throw new NotImplementedException();
            try
            {
                string _where = "(";
                //dt = LoadData(string.Format("SELECT DISTINCT BCITEM.CODE,NAME1,STOCKQTY,BCITEMUNIT.NAME AS UNITNAME,SALEPRICE1 FROM BCITEM,BCITEMUNIT  WHERE  ((BCITEM.CODE  LIKE '%{0}%' ) OR (NAME1  LIKE '%{0}%' ) OR (BCITEMUNIT.NAME  LIKE '%{0}%' )) AND BCITEM.DEFSTKUNITCODE=BCITEMUNIT.CODE AND  BCITEM.ACTIVESTATUS <>2 AND BCITEM.ACTIVESTATUS <>3  AND  (BCITEM.Name1 Like '%{0}%' Or BCITEM.Code Like '%{0}%') ORDER BY BCITEM.CODE", textSearch.Text.ToString()));
                if (wheretxt.Length > 0)
                {
                    int l = wheretxt.Length;
                    foreach (string val in wheretxt)
                    {
                        _where += string.Format("({0} LIKE '%{1}%')", val,textsearch);
                        if (l > 1)
                            _where += " or ";
                        l -= 1;
                    }
                    _where += ")";
                }
                if (conditionf.Length > 0 && conditionv.Length>0) 
                {
                    _where += " AND ";
                    for (int j = 0; j < conditionf.Length; j++)
                    {
                        _where += string.Format("{0} LIKE '{1}'", conditionf[j], conditionv[j]);
                        if (j < conditionf.Length - 1)
                            _where += " OR ";
                    }
                }
                dt = LoadData(string.Format("{0} where {1}", sqltext, _where));//string.Format("SELECT DISTINCT BCITEM.CODE,NAME1,STOCKQTY,BCITEMUNIT.NAME AS UNITNAME,SALEPRICE1 FROM BCITEM,BCITEMUNIT  WHERE  ((BCITEM.CODE  LIKE '%{0}%' ) OR (NAME1  LIKE '%{0}%' ) OR (BCITEMUNIT.NAME  LIKE '%{0}%' )) AND BCITEM.DEFSTKUNITCODE=BCITEMUNIT.CODE AND  BCITEM.ACTIVESTATUS <>2 AND BCITEM.ACTIVESTATUS <>3  AND  (BCITEM.Name1 Like '%{0}%' Or BCITEM.Code Like '%{0}%') ORDER BY BCITEM.CODE", textSearch.Text.ToString()));
                
                _where = "(";
                if (wheretxt2.Length > 0)
                {
                    int l = wheretxt2.Length;
                    foreach (string val in wheretxt2)
                    {
                        _where += string.Format("({0} LIKE '%{1}%')", val, textsearch);
                        if (l > 1)
                            _where += " or ";
                        l -= 1;
                    }
                    _where += ")";
                }
                if (conditionf.Length > 0 && conditionv.Length > 0)
                {
                    _where += " AND ";
                    for (int j = 0; j < conditionf.Length; j++)
                    {
                        _where += string.Format("{0} LIKE '{1}'", conditionf[j], conditionv[j]);
                        if (j < conditionf.Length - 1)
                            _where += " OR ";
                    }
                }
                if (dt.Rows.Count == 0 && sqltext2.Length>0 && _where.Length>2)
                {
                    if(jointable.Length==2)
                        dt = LoadData(string.Format("{0} where {1}={2} and  {3}", sqltext2,jointable[0],jointable[1], _where));
                    else
                    dt = LoadData(string.Format("{0} where {1}", sqltext2, _where));
                    if (dt.Rows.Count ==1) isserial = true;
                }
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

        public void Loading()
        {
            //dt = LoadData(sqltext);//"SELECT DISTINCT BCITEM.CODE,NAME1,STOCKQTY,BCITEMUNIT.NAME AS UNITNAME,SALEPRICE1 FROM BCITEM,BCITEMUNIT  WHERE ((BCITEM.CODE  LIKE '%%' ) OR (NAME1  LIKE '%%' ) OR (BCITEMUNIT.NAME  LIKE '%%' )) AND BCITEM.DEFSTKUNITCODE=BCITEMUNIT.CODE AND  BCITEM.ACTIVESTATUS <>2 AND BCITEM.ACTIVESTATUS <>3  ORDER BY BCITEM.CODE");
            if (!timer1.Enabled)
            {
                timer1.Start();
            }
            timerTicks = 0;
            
        
        }

   

        public void Clear()
        {
            dt = new DataTable();
            gridView1.Columns.Clear();
            gridControl1.RefreshDataSource();
        
        }

        public void preData()
        {
            iAllRows = Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(dt.Rows.Count / iPageSize))) + 1; 
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

        public void AddColumn(string caption, string fieldname, int width, bool edited,FormatType type,String formatstr)
        {
            index++;
            DevExpress.XtraGrid.Columns.GridColumn column = new DevExpress.XtraGrid.Columns.GridColumn()
            {
                Caption = caption,
                FieldName = fieldname,
                Width = width,

            };

                column.DisplayFormat.FormatType = type;//FormatType.Numeric;
                column.DisplayFormat.FormatString = formatstr;
                column.AppearanceCell.Font = new Font(gridView1.Appearance.Row.Font, FontStyle.Bold);
          
            gridView1.Columns.Add(column);
            column.VisibleIndex = 0;
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
        public void setGrid(DevExpress.XtraGrid.Columns.GridColumn col, DevExpress.Utils.FormatType ftype = DevExpress.Utils.FormatType.None, string format = "")
        {
            gridView1.Columns.Add(col);
            col.DisplayFormat.FormatType = ftype;
            col.DisplayFormat.FormatString = format;


        }
    }
}
