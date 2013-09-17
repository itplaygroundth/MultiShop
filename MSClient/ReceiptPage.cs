using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Skins;

namespace MultiShop
{
    public partial class ReceiptPage : DevExpress.XtraEditors.XtraForm
    {
        public ReceiptPage()
        {
            InitializeComponent();
            Skin tabSkin = TabSkins.GetSkin(xtraTabControl1.LookAndFeel);
            SkinElement element = tabSkin[TabSkins.SkinTabPane];
            Image image = element.Image.Image;
            simpleButton1.BackColor = Color.Gray;
            simpleButton2.BackColor = Color.FromArgb(235, 236, 239);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

            xtraTabControl1.SelectedTabPage = xtraTabPage1;
            panelControl3.BackColor = Color.Gray;
            panelControl6.BackColor = Color.LightGray;

        }

     

        private void simpleButton2_Click_1(object sender, EventArgs e)
        {
            xtraTabControl1.SelectedTabPage = xtraTabPage2;
            panelControl6.BackColor = Color.Gray;
            panelControl3.BackColor = Color.LightGray;
        }

   
       
     
      

     
        
    }
}