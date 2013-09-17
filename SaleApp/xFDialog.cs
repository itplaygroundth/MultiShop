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
    public partial class xFDialog : DevExpress.XtraEditors.XtraForm
    {
        public string Connection { get; set; }
        public string userid { get; set; }
        public string passwd { get; set; }
        public event EventHandler CloseDialog;
        xLogin xlog;
        public xFDialog()
        {
            InitializeComponent();

        }

        private void xFDialog_Load(object sender, EventArgs e)
        {
            xlog = new xLogin();
            Controls.Add(xlog);
        }
    }
}