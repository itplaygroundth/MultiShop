using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace MultiShop
{
    public partial class RecMoney : DevExpress.XtraEditors.XtraForm
    {
        public event EventHandler FormClose;
        public double Amount { get; set; }
        public DataTable dtAmount { get; set; }
        private DataRow dr;

        public RecMoney()
        {
            InitializeComponent();
            FormClose += new EventHandler(RecMoney_FormClose);
            textEdit2.KeyDown += new KeyEventHandler(textEdit2_KeyDown);
        }

        void textEdit2_KeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.KeyCode == Keys.Enter)
            {
                textEdit3.Text = (Convert.ToDouble(textEdit2.Text)-Amount).ToString("#,##0.00");
            }
        }

        void RecMoney_FormClose(object sender, EventArgs e)
        {
            this.Close();
            //throw new NotImplementedException();
        }

        public void init()
        {
            dtAmount = new DataTable();
            dtAmount.Columns.Add(new DataColumn("Amount",typeof(double)));
            dtAmount.Columns.Add(new DataColumn("CashAmount",typeof(double)));
            dtAmount.Columns.Add(new DataColumn("ChangeAmount",typeof(double)));
            dr=dtAmount.NewRow();
            dr["Amount"] = Amount;
            dr["CashAmount"] = 0;
            dr["ChangeAmount"] = 0;
            dtAmount.Rows.Add(dr);
            textEdit1.Text = Amount.ToString("#,##0.00");
            textEdit2.Text = "";
            textEdit3.Text = "0.00";
            

        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            if (Convert.ToDouble(textEdit2.Text) > 0 && Convert.ToDouble(textEdit2.Text) >= Amount)
            {
                dtAmount.Rows[0]["CashAmount"] = Convert.ToDouble(textEdit2.Text);
                dtAmount.Rows[0]["ChangeAmount"] = Convert.ToDouble(textEdit3.Text);
                dtAmount.AcceptChanges();
                FormClose(null, e);
            }
            else
            { MessageBox.Show("ยอดจ่ายไม่เท่ากับยอดรับเงิน!", "เตือน", MessageBoxButtons.OK);
            textEdit2.Focus();
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            dtAmount.RejectChanges();
            FormClose(null, e);
        }

    }
}