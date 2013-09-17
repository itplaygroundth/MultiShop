using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraNavBar;


namespace MSSale
{
    public partial class MainFrm : XtraForm
    {
        public MainFrm()
        {
            InitializeComponent();
            InitSkinGallery();

        }
        void InitSkinGallery()
        {
           // SkinHelper.InitSkinGallery("McSkin", true);
            
	    //navBarControl1.ResetStyles();

        }

        private void MainFrm_Load(object sender, EventArgs e)
        {

        }

    }

    public class SmartNavBar : NavBarControl
    {
        protected override void RegisterAvailableNavBarViews()
        {
            base.RegisterAvailableNavBarViews();
            AvailableNavBarViews.Add(new NoExpandButtonViewInfoRegistrator());
        }
        protected override DevExpress.XtraNavBar.ViewInfo.BaseViewInfoRegistrator GetExplorerView(ActiveLookAndFeelStyle activeStyle)
        {
            if (activeStyle != ActiveLookAndFeelStyle.Skin)

                return AvailableNavBarViews[NoExpandButtonViewInfoRegistrator.CustomViewName];

            return base.GetExplorerView(activeStyle);
      
    }
}