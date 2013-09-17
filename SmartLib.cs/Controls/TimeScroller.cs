using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmartPOS.Controls
{
    public partial class TimeScroller : DevExpress.XtraEditors.XtraUserControl
    {
        private int h = 1;
        private int m = 1;
        private int s = 1;

        public TimeScroller()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            textBox1.Text = (h += 1).ToString();
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            textBox2.Text = (m += 1).ToString();
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            textBox3.Text = (s += 1).ToString();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            textBox1.Text = (h -= 1).ToString();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            textBox2.Text = (m -= 1).ToString();
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            textBox3.Text = (s -= 1).ToString();
        }
    }
}
