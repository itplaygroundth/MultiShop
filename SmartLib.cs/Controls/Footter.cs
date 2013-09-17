using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SmartLib.Controls
{
    public partial class Footter : DevExpress.XtraEditors.XtraUserControl
    {
        private int newindex = 0;
        private int searchindex = 1;
        private int deleteindex = 2;
        private int saveindex = 3;
        private int cancelindex = 4;

        public Footter()
        {
            InitializeComponent();
        }

        [Browsable(true)]
        [DefaultValue(true)]
        public bool NewButton
        {
            get
            {
                return pictureBox2.Visible;
                
            }
            set
            {
                pictureBox2.Visible = value;
            }
        }
        [Browsable(true)]
        [DefaultValue("New")]
        public string NewBtnText
        {
            get
            {
                return label1.Text;
            }
            set
            {
                label1.Text = value;
            }
        }
        [Browsable(true)]
        [DefaultValue(0)]
        public int NewBtnIndex
        {
        	get 
            {
                return newindex;
            }
            set
            {
                newindex=setBackGround(value, pictureBox2);
            }
        }

        [Browsable(true)]
        [DefaultValue(true)]
        public bool FindButton
        {
            get
            {
               return pictureBox1.Visible;
            }
            set
            {
                pictureBox1.Visible = value;
            }
        }
        [Browsable(true)]
        [DefaultValue("Find")]
        public string FindBtnText
        {
            get
            {
                return label2.Text;
            }
            set
            {
                label2.Text = value;
            }
        }
        [Browsable(true)]
        [DefaultValue(1)]
        public int findBtnIndex
        {
            get
            {
                return searchindex;
            }
            set
            {
                searchindex = setBackGround(value, pictureBox1);
            }
        }
        [Browsable(true)]
        [DefaultValue(true)]
        public bool CancelButton
        {
            get
            {
                return pictureBox4.Visible;
            }
            set
            {
                pictureBox4.Visible = value;
            }
        }
        [Browsable(true)]
        [DefaultValue("Cancel")]
        public string CancelBtnText
        {
            get
            {
                return label5.Text;
            }
            set
            {
                label5.Text = value;
            }
        }
        [Browsable(true)]
        [DefaultValue(4)]
        public int cancelBtnIndex
        {
            get
            {
                return cancelindex;
            }
            set
            {
                cancelindex = setBackGround(value, pictureBox4);
            }
        }
        [Browsable(true)]
        [DefaultValue(true)]
        public bool SaveButton
        {
            get
            {
                return pictureBox5.Visible;
            }
            set 
            {
                pictureBox5.Visible = value;
            }
        }
        [Browsable(true)]
        [DefaultValue("Save")]
        public string SaveBtnText
        {
            get
            {
                return label4.Text;
            }
            set
            {
                label4.Text = value;
            }
        }
        [Browsable(true)]
        [DefaultValue(3)]
        public int SaveBtnIndex
        {
            get
            {
                return saveindex;
            }
            set
            {
                saveindex = setBackGround(value, pictureBox5);
            }
        }
        [Browsable(true)]
        [DefaultValue(true)]
        public bool DelButton
        {
            get
            {
                return pictureBox6.Visible;
        	}
            set
            {
                pictureBox6.Visible = value;
            }
        }

        [Browsable(true)]
        [DefaultValue("Delete")]
        public string DeleteBtnText
        {
            get
            {
                return label3.Text;
            }
            set
            {
                label3.Text = value;
            }
        }
        [Browsable(true)]
        [DefaultValue(2)]
        public int DeleteBtnIndex
        {
            get
            {
                return deleteindex;
            }
            set
            {
                deleteindex = setBackGround(value, pictureBox6);
            }
        }
       
        public event EventHandler saveBtn;
        public event EventHandler newBtn;
        public event EventHandler findBtn;
        public event EventHandler commitBtn;
        public event EventHandler cancelBtn;
        public event EventHandler delBtn;

        private int setBackGround(int index,PictureBox picbox)
        {
            int choice = 0;
            switch (index)
            {
                case 0:
                    picbox.Image = global::SmartPOS.Properties.Resources.add1;
                    choice = 0;
                    break;
                case 1:
                    picbox.Image = global::SmartPOS.Properties.Resources.search1;
                    choice = 1;
                    break;
                case 2:
                    picbox.Image = global::SmartPOS.Properties.Resources.delete1;
                    choice = 2;
                    break;
                case 3:
                    picbox.Image = global::SmartPOS.Properties.Resources.save1;
                    choice = 3;
                    break;
                case 4:
                    picbox.Image = global::SmartPOS.Properties.Resources.cancel1;
                    choice = 4;
                    break;
            }

            return choice;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if(cancelBtn!=null)
            {
                pictureBox4.Image = global::SmartPOS.Properties.Resources.cancel1;
                cancelBtn(this, null);
            }
        }
        private void pictureBox4_MouseLeave(object sender, EventArgs e)
        {
            //pictureBox4.Image = global::SmartPOS.Properties.Resources.cancel;
        }

        //private void pictureBox4_MouseHover(object sender, EventArgs e)
        //{
        //    pictureBox4.Image = global::SmartPOS.Properties.Resources.cancel1;
        //}

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(findBtn!=null)
            {
                findBtn(this, null);
                pictureBox1.Image = global::SmartPOS.Properties.Resources.search1;
            }
        }

        //private void pictureBox1_MouseLeave(object sender,EventArgs e){}
        //private void pictureBox1_MouseHover(object sender,EventArgs e){}
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if(delBtn!=null)
            {
                pictureBox6.Image = global::SmartPOS.Properties.Resources.delete1;
            	delBtn(this,null);
            }
        }
        private void pictureBox6_MouseLeave(object sender, EventArgs e)
        {
            //pictureBox6.Image = global::SmartPOS.Properties.Resources.delete ;
        }

        private void pictureBox6_MouseHover(object sender, EventArgs e) { }//pictureBox6.Image = global::SmartPOS.Properties.Resources.delete1; }

        //private void pictureBox3_Click(object sender, EventArgs e)
        //{
        //    if(commitBtn!=null)
        //    {
        //        pictureBox3.Image = global::SmartPOS.Properties.Resources.check1;
        //        commitBtn(this, null);
        //    }
        //}
        //private void pictureBox3_MouseLeave(object sender,EventArgs e)
        //{
        //    pictureBox3.Image = global::SmartPOS.Properties.Resources.check;
        //}
        //private void pictureBox3_MouseHover(object sender,EventArgs e)
        //{
        //    pictureBox3.Image = global::SmartPOS.Properties.Resources.check1;
        //}
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (newBtn != null)
            {
                pictureBox2.Image = global::SmartPOS.Properties.Resources.add1;
                newBtn(this, null);
            }
        }
        void pictureBox2_MouseLeave(object sender, System.EventArgs e)
        {
            //pictureBox2.Image = global::SmartPOS.Properties.Resources.add;
        }

        void pictureBox2_MouseHover(object sender, System.EventArgs e)
        {
            //pictureBox2.Image = global::SmartPOS.Properties.Resources.add1;
        }


        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if(saveBtn!=null)
            {
                pictureBox5.Image = global::SmartPOS.Properties.Resources.save1;
                saveBtn(this, null);
            }
        }

        private void pictureBox5_MouseLeave(object sender, EventArgs e) {}// pictureBox5.Image = global::SmartPOS.Properties.Resources.save; }
        //private void pictureBox5_MouseHover(object sender, EventArgs e) { pictureBox5.Image = global::SmartPOS.Properties.Resources.save1; }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            //pictureBox1.Image = global::SmartPOS.Properties.Resources.search1;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            //pictureBox1.Image = global::SmartPOS.Properties.Resources.search;
        }

   
        
    }

}
