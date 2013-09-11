using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SaleApp
{
    public partial class xLogin : DevExpress.XtraEditors.XtraForm
    {
        public string Connection { get; set; }
        public string userid { get; set; }
        public string passwd { get; set; }
        public string dbname { get; set; }
        public event EventHandler CloseDialog;
        string connectemp;
        public DialogResult _result { get; set; }
        public xLogin()
        {
            InitializeComponent();
            connectemp = SaleForm.ReadReg("", "LOGIN_DEFAULT");
            if (connectemp.Length > 0)
            {
                string[] temp = connectemp.Split('^');
                textEdit1.Text = temp[0];
                dbname = temp[0];
                textEdit2.Text = temp[1];
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (CloseDialog != null)
            {
                _result = DialogResult.OK;
                Connection = textEdit1.Text;
                userid = textEdit2.Text;
                passwd = textEdit3.Text;
                CloseDialog(this, null);
                
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (CloseDialog != null) 
            {
                _result = DialogResult.Cancel;
                Connection = "";
                userid = "";
                passwd = "";
                CloseDialog(this,null); 
                
            }
        }

        private void xLogin_Load(object sender, EventArgs e)
        {
            groupControl1.Left = (this.ClientSize.Width - groupControl1.Width) / 2;
            groupControl1.Top = (this.ClientSize.Height - groupControl1.Height) / 2;
        }

    }
}