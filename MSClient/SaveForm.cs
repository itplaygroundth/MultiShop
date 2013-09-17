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

namespace MultiShop
{
    public partial class SaveForm : DevExpress.XtraEditors.XtraForm
    {
        public event EventHandler CloseSave;
        public Double Amount { get; set; }
        public string ArCode { get; set; }
        public string strconn { get; set; }
        private double sumcreditamount = 0;
        private double sumcheqamount = 0;
        private double sumcoupamount = 0;
        private double sumdepoamount = 0;
        public DataTable Dtreceipt {get;set;}
        public DialogResult dResult { get; set; }
        private DataTable dtdepos;
              
        private int crgridrowsel = 0;
        private int chqgridrowsel = 0;
        private int cougridrowsel = 0;
        private int depgridrowsel = 0;
        public SaveForm()
        {
            InitializeComponent();
            ControlBox = false;
            //Dtreceipt = new DataTable();
            //initDataTable();
            CreditAmountRepo.KeyDown += new KeyEventHandler(CreditAmountRepo_KeyDown);
            CheqValueRepo.KeyDown += new KeyEventHandler(CheqValueRepo_KeyDown);
            CouponValueRepo.KeyDown += new KeyEventHandler(CouponValueRepo_KeyDown);
            DepositValueRepo.KeyDown += new KeyEventHandler(DepositValueRepo_KeyDown);
            CreditGridView.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(CreditGridView_CustomRowCellEditForEditing);
            CheqGridView.CustomRowCellEditForEditing += new CustomRowCellEditEventHandler(CheqGridView_CustomRowCellEditForEditing);
            CouponGridView.CustomRowCellEditForEditing += new CustomRowCellEditEventHandler(CouponGridView_CustomRowCellEditForEditing);
            DepositGridView.CustomRowCellEditForEditing += new CustomRowCellEditEventHandler(DepositGridView_CustomRowCellEditForEditing);
            
        }

        private void loadDeposit()
        {
            using (SqlConnection scon = new SqlConnection(strconn))
            {
                //using(SqlDataAdapter da=new SqlDataAdapter(string.Format("select DocNo,TotalAmount,coalesce((Select Amount from BCARDepositUse where BCARDeposit.DocNo=BCARDepositUse.DepositNo),0) as Amount,BillBalance from BCARDeposit where ArCode='{0}'",ArCode),scon))
                using (SqlDataAdapter da = new SqlDataAdapter(string.Format("select DocNo,TotalAmount,BillBalance from BCARDeposit where ArCode='{0}'", ArCode), scon))
                {
                   
                    da.Fill(dtdepos);
                    if (dtdepos.Rows.Count > 0)
                    {
                        
                        DepositNoRepo.DataSource = dtdepos;
                        DepositNoRepo.DisplayMember = "DocNo";
                        DepositNoRepo.ValueMember = "DocNo";
                        //DepositNoRepo
                    }
                }
            }
        }

        void DepositNoRepo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            SearchLookUpEdit view = sender as SearchLookUpEdit;
            if (view.EditValue != null && view.EditValue.ToString().Trim().Length > 0)
            {
                var dsel = dtdepos.Select(string.Format("DOCNO='{0}'", view.EditValue));
                foreach (DataRow dr in dsel)
                {
                    Dtreceipt.Rows[depgridrowsel]["DepositAmount"] = dr["BillBalance"];

                }
            }
            //DataRow drnew = dtdepos.NewRow();

            //throw new System.NotImplementedException();
        }

        void DepositGridView_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
        {
            depgridrowsel = e.RowHandle;
            //throw new NotImplementedException();
        }

        void CouponGridView_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
        {
            cougridrowsel = e.RowHandle;
            //throw new NotImplementedException();
        }

        void CheqGridView_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
        {
            chqgridrowsel = e.RowHandle;
            //throw new NotImplementedException();
        }
        void DepositValueRepo_KeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
            {
                CalcEdit cedit = sender as CalcEdit;
                if (cedit.Text == string.Empty) cedit.Text = "0.00";

                if (Convert.ToDouble(cedit.Text) <= Math.Abs(Convert.ToDouble(DiffAmount.Text)) && Convert.ToDouble(cedit.Text)<=Convert.ToDouble(Dtreceipt.Rows[depgridrowsel]["DepositAmount"]))
                {
                    Dtreceipt.Rows[depgridrowsel]["DepositValue"] = Convert.ToDouble(cedit.Text);
                    Dtreceipt.Rows[depgridrowsel]["DepositBalance"] = (Convert.ToDouble(Dtreceipt.Rows[depgridrowsel]["DepositAmount"]) - Convert.ToDouble(cedit.Text)).ToString("#,##0.00");
                    sumdepoamount = Convert.ToDouble(Dtreceipt.AsEnumerable().Sum(x => x.Field<double>("DepositValue")));

                    DepositBox.Text = sumdepoamount.ToString("#,##0.00");
                    UpdateSumBill();
                }
                else
                {
                    //Dtreceipt.Rows[depgridrowsel]["DepositValue"] = 0;
                    cedit.Value = 0;
                    MessageBox.Show("ยอดชำระไม่ถูกต้อง!");
                }
                

            }
        }

        void CouponValueRepo_KeyDown(object sender, KeyEventArgs e)
        {
            // throw new NotImplementedException();
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
            {
                CalcEdit cedit = sender as CalcEdit;
                if (cedit.Text == string.Empty) cedit.Text = "0.00";

                if (Convert.ToDouble(cedit.Text) <= Math.Abs(Convert.ToDouble(DiffAmount.Text)))
                {
                    Dtreceipt.Rows[cougridrowsel]["CouponValue"] = Convert.ToDouble(cedit.Text);
                    //Dtreceipt.Rows[cougridrowsel]["CouponBalance"] = (Convert.ToDouble(Dtreceipt.Rows[cougridrowsel]["CouponAmount"]) - Convert.ToDouble(cedit.Text)).ToString("#,##0.00");
                    sumcoupamount = Convert.ToDouble(Dtreceipt.AsEnumerable().Sum(x => x.Field<double>("CouponValue")));
                    CouponBox.Text = sumcoupamount.ToString("#,##0.00");
                    UpdateSumBill();
                }
                else
                {
                    cedit.Value = 0;
                    MessageBox.Show("ยอดชำระไม่ถูกต้อง!");
                }
            }
        }
        void CheqValueRepo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
            {
                CalcEdit cedit = sender as CalcEdit;
                if (cedit.Text == string.Empty) cedit.Text = "0.00";

                if (Convert.ToDouble(cedit.Text) <= Math.Abs(Convert.ToDouble(DiffAmount.Text)) && Convert.ToDouble(cedit.Text)<=Convert.ToDouble(Dtreceipt.Rows[chqgridrowsel]["CheqAmount"]))
                {
                    Dtreceipt.Rows[chqgridrowsel]["CheqValue"] = Convert.ToDouble(cedit.Text);
                    Dtreceipt.Rows[chqgridrowsel]["CheqBalance"] = (Convert.ToDouble(Dtreceipt.Rows[chqgridrowsel]["CheqAmount"]) - Convert.ToDouble(cedit.Text)).ToString("#,##0.00");
                    sumcheqamount = Convert.ToDouble(Dtreceipt.AsEnumerable().Sum(x => x.Field<double>("CheqValue")));
                    CheqBox.Text = sumcheqamount.ToString("#,##0.00");
                    UpdateSumBill();
                }
                else
                {
                   cedit.Value = 0;
                    MessageBox.Show("ยอดชำระไม่ถูกต้อง!");
                }

            }
            //throw new NotImplementedException();
        }

        void CreditGridView_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            //throw new NotImplementedException();
            crgridrowsel = e.RowHandle;
        }

        void CreditAmountRepo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
            {
                CalcEdit cedit=sender as CalcEdit;
                if (cedit.Text == string.Empty) cedit.Text = "0.00";

                if (Convert.ToDouble(cedit.Text) <= Math.Abs(Convert.ToDouble(DiffAmount.Text)))
                {
                    Dtreceipt.Rows[crgridrowsel]["CreditAmount"] = Convert.ToDouble(cedit.Text);
                    sumcreditamount = Convert.ToDouble(Dtreceipt.AsEnumerable().Sum(x => x.Field<double>("CreditAmount")));
                    CreditBox.Text = sumcreditamount.ToString("#,##0.00");
                    UpdateSumBill();
                }
                else
                {
                    //Dtreceipt.Rows[crgridrowsel]["CreditAmount"] = 0;
                    cedit.Value = 0;
                    MessageBox.Show("ยอดชำระไม่ถูกต้อง!");
                }
            
            }
            //throw new NotImplementedException();
        }

        private void loadSumBill()
        {
            if (Dtreceipt.Rows.Count > 0)
            if (Convert.ToDouble(Dtreceipt.Rows[0]["BillAmount"]) > 0)
            {
                CashBox.Text = Convert.ToDouble(Dtreceipt.Rows[0]["CashAmount"]).ToString("#,##0.00");
                CashTextBox.Text = CashBox.Text;
                CreditBox.Text = Convert.ToDouble(Dtreceipt.AsEnumerable().Sum(x => x.Field<double>("CreditAmount"))).ToString("#,##0.00");
                CheqBox.Text = Convert.ToDouble(Dtreceipt.AsEnumerable().Sum(x => x.Field<double>("CheqValue"))).ToString("#,##0.00");
                CouponBox.Text = Convert.ToDouble(Dtreceipt.AsEnumerable().Sum(x => x.Field<double>("CouponValue"))).ToString("#,##0.00");
                DepositBox.Text =  Convert.ToDouble(Dtreceipt.AsEnumerable().Sum(x => x.Field<double>("DepositValue"))).ToString("#,##0.00");
                       
                ReceiptAmount.Text = (Convert.ToDouble(CashTextBox.Text) + Convert.ToDouble(CreditBox.Text) + Convert.ToDouble(CheqBox.Text) + Convert.ToDouble(CouponBox.Text) + Convert.ToDouble(DepositBox.Text)).ToString("#,##0.00");

                DiffAmount.Text = (Convert.ToDouble(CashTextBox.Text) + Convert.ToDouble(CreditBox.Text) + Convert.ToDouble(CheqBox.Text) + Convert.ToDouble(CouponBox.Text) + Convert.ToDouble(DepositBox.Text) - Amount).ToString("#,##0.00");
                if (Convert.ToDouble(CashTextBox.Text)>Amount)
                    CashChangeText.Text = (Convert.ToDouble(CashTextBox.Text) - Amount).ToString("#,##0.00");
                else
                    CashChangeText.Text = (Convert.ToDouble(CashTextBox.Text) + Convert.ToDouble(CreditBox.Text) + Convert.ToDouble(CheqBox.Text) + Convert.ToDouble(CouponBox.Text) + Convert.ToDouble(DepositBox.Text) - Amount).ToString("#,##0.00");
                
                //UpdateSumBill();
            }
        }

        public void initDataTable(DataTable dt)
        {
          if (Dtreceipt != null)
              Dtreceipt.Rows.Clear();
                Dtreceipt = dt;
                loadSumBill();
          //  }
            CreditGrid.DataSource = Dtreceipt;
               
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

            if (Convert.ToDouble(DiffAmount.Text) >= 0)
            {
                CloseSave(sender, e);
                dResult = DialogResult.OK;
                Dtreceipt.AcceptChanges();
                this.Close();
            }
            else
                MessageBox.Show("มียอดค้างชำระคงเหลือ!");
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            CloseSave(sender, e);
            Dtreceipt.RejectChanges();  
            dResult = DialogResult.Cancel;   
            this.Close();
        }

        private void SaveForm_Load(object sender, EventArgs e)
        {
            BillAmount.Text = Amount.ToString("#,##0.00");
            //DiffAmount.Text = (0 - Amount).ToString("#,##0.00");
            CashTextBox.Select();
            dtdepos = new DataTable();
            dtdepos.Columns.Add(new DataColumn() { ColumnName = "DocNo", Caption = "เลขที่เอกสาร" });
            dtdepos.Columns.Add(new DataColumn() { ColumnName = "TotalAmount", Caption = "ยอดเงินมัดจำ" });
            //dpos.Columns.Add(new DataColumn() { ColumnName = "Amount",Caption="ยอดตัดจ่าย" });
            dtdepos.Columns.Add(new DataColumn() { ColumnName = "BillBalance", Caption = "ยอดคงเหลือ" });
            loadGrid();
            loadDeposit();
        }
        public void loadGrid()
        {
            //DataTable dt = new DataTable();
           
            CreditGrid.DataSource = Dtreceipt;
            CheqGrid.DataSource = Dtreceipt;
            CouponGrid.DataSource = Dtreceipt;
            DepositGrid.DataSource = Dtreceipt;
        }

        private void CashTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateSumBill();
            }
        }

        private void UpdateSumBill()
        {
            if (CashTextBox.Text == string.Empty) CashTextBox.Text = "0.00";
            Dtreceipt.Rows[0]["BillAmount"] = Amount;
            CashBox.Text = Convert.ToDouble(CashTextBox.Text).ToString("#,##0.00");
            Dtreceipt.Rows[0]["CashAmount"] = Convert.ToDouble(CashTextBox.Text).ToString("#,##0.00");
            ReceiptAmount.Text = (Convert.ToDouble(CashTextBox.Text)+Convert.ToDouble(CreditBox.Text)+Convert.ToDouble(CheqBox.Text)+Convert.ToDouble(CouponBox.Text)+Convert.ToDouble(DepositBox.Text)).ToString("#,##0.00");
            
            if ((Convert.ToDouble(CashTextBox.Text) - Amount) < 0 || Convert.ToDouble(CashTextBox.Text)<Amount)
            {
                DiffAmount.Text = (Convert.ToDouble(CashTextBox.Text)+Convert.ToDouble(CreditBox.Text)+Convert.ToDouble(CheqBox.Text)+Convert.ToDouble(CouponBox.Text)+Convert.ToDouble(DepositBox.Text) - Amount).ToString("#,##0.00");
            }
            else if ((Convert.ToDouble(CashTextBox.Text) - Amount) >= 0)
            {
                ReceiptAmount.Text = Convert.ToDouble(CashTextBox.Text).ToString("#,##0.00");
                DiffAmount.Text = (Convert.ToDouble(CashTextBox.Text) - Amount).ToString("#,##0.00");
            }
            Dtreceipt.Rows[0]["ChangeAmount"] = Convert.ToDouble(DiffAmount.Text);
            //if (Convert.ToDouble(CashTextBox.Text) < Math.Abs(Convert.ToDouble(DiffAmount.Text)))
            //    CashChangeText.Text = "0.00";
            if (Convert.ToDouble(CreditBox.Text) > 0 || Convert.ToDouble(CheqBox.Text) > 0 || Convert.ToDouble(CouponBox.Text) > 0 || Convert.ToDouble(DepositBox.Text) > 0)
            {
                if (Convert.ToDouble(CashTextBox.Text) > 0)
                {
                    CashChangeText.Text = (Convert.ToDouble(CashTextBox.Text) - Math.Abs(Convert.ToDouble(CashChangeText.Text))).ToString("#,##0.00");
                    DiffAmount.Text = (Convert.ToDouble(CashTextBox.Text) + Convert.ToDouble(CreditBox.Text) + Convert.ToDouble(CheqBox.Text) + Convert.ToDouble(CouponBox.Text) + Convert.ToDouble(DepositBox.Text) - Amount).ToString("#,##0.00");
                }
                else
                    CashChangeText.Text = (Convert.ToDouble(DiffAmount.Text)).ToString("#,##0.00");
            }
            else if (Convert.ToDouble(CashTextBox.Text) == Amount)
                CashChangeText.Text = "0.00";
            else
                CashChangeText.Text = (Convert.ToDouble(CashTextBox.Text) - Amount).ToString("#,##0.00");
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            
            switch (xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    break;
                case 1:
                    if (Convert.ToDouble(DiffAmount.Text) == 0)
                    {
                        
                        xtraTabPage1.Select();
                    }
                    //else
                    //    xtraTabPage2.Select();
                    break;
                case 2:
                    if (Convert.ToDouble(DiffAmount.Text) == 0)
                    {
                        xtraTabPage1.Select();
                    }
                    break;
                case 3:
                    if (Convert.ToDouble(DiffAmount.Text) == 0)
                    {
                        xtraTabPage1.Select();
                    }
                    break;
                case 4:
                    if (Convert.ToDouble(DiffAmount.Text) == 0)
                    {
                        xtraTabPage1.Select();
                    }
                    break;
            }
        }

    
         
    }
}