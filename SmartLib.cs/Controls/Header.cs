using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using SmartLib.Helpers;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace SmartLib.Controls
{
    public partial class Header : DevExpress.XtraEditors.XtraUserControl
    {



        public Header()
        {
            InitializeComponent();
            lbl_ip.Text = Networking.GetLocalIP();
            timer1.Start();
            
           
        }

  

        [Browsable(true)]
        [DefaultValue(true)]
        public bool BackButtonVisible
        {
            get { return this.pictureBoxBack.Visible; }
            set { this.pictureBoxBack.Visible = value; }
        }

        [Browsable(true)]
        [DefaultValue("Title")]
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }


        //[Browsable(true)]
        //[DefaultValue("ipaddress")]
        public string IPAdress
        {
            get { return lbl_ip.Text; }
            set
            {
                lbl_ip.Text = value;
            }
        
        }

        [Browsable(true)]
        [DefaultValue("StatusText")]
        public string StatusText
        {
            get
            {
                return lbl_status.Text;
            }
            set
            {
                lbl_status.Text = value;
            }
        }


        [Browsable(true)]
        [DefaultValue(true)]
        public bool Status
        {
            get
            {
                return led_status.On;
            }
            set {
                led_status.On = value;
            }
        }

        public void setStatus(bool on)
        {
            led_status.On = on;
        }

        public void setStatusText(string txt)
        {
            lbl_status.Text = txt;
        }

        public event EventHandler BackRequest;

        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            if (BackRequest != null)
            {
                BackRequest(this, null);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbl_time.Text = string.Format("{0}  {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
        }
    }
}
