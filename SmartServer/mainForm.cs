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
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Docking2010.Views.NativeMdi;
using SmartServer.ChildPage;


namespace SmartServer
{
    public partial class mainForm : XtraForm
    {
        iGetData igDataForm = new iGetData();
         iSendData iSendForm = new iSendData();
         iTemData iTemForm = new iTemData();
         iPointData iPointForm = new iPointData();
        public mainForm()
        {
            InitializeComponent();
            //documentManager1.View.DocumentClosing += new DevExpress.XtraBars.Docking2010.Views.DocumentCancelEventHandler(View_DocumentClosing);
            
            InitSkinGallery();
            
            

        }

        //void View_DocumentClosing(object sender, DevExpress.XtraBars.Docking2010.Views.DocumentCancelEventArgs e)
        //{
        //    e.Cancel = true;
        //    if (e.Document.Form != null)
        //    {
        //        e.Document.Form.Hide();
        //        e.Document.Form.MdiParent = null;
        //    }
        //    documentManager1.View.Controller.RemoveDocument(e.Document);
        //    //throw new NotImplementedException();
        //}
        void InitSkinGallery()
        {
            SkinHelper.InitSkinGallery(rgbiSkins, true);
        }

    

        private void mainForm_Load(object sender, EventArgs e)
        {
            documentManager1.MdiParent = this;
            documentManager1.View = new NativeMdiView();
            

        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
          //  if (!documentManager1.IsDocumentSelectorVisible)
          //      igDataForm = new iGetData();

            //      iSendForm = new iSendData();
            DevExpress.XtraBars.Docking2010.Views.BaseDocument doc = documentManager1.GetDocument(igDataForm);
            if (doc == null) { igDataForm = new iGetData(); documentManager1.View.AddDocument(igDataForm); }
            igDataForm.MdiParent = this;
            //igDataForm.FormBorderStyle = FormBorderStyle.None;
            //igDataForm.WindowState = FormWindowState.Maximized;
            igDataForm.BringToFront();
           
            igDataForm.Show();
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
           
          //  if (!documentManager1.IsDocumentSelectorVisible)
          //      iSendForm = new iSendData();
            DevExpress.XtraBars.Docking2010.Views.BaseDocument doc = documentManager1.GetDocument(iSendForm);
            if (doc == null) iSendForm = new iSendData();
            iSendForm.MdiParent = this;
            //iSendForm.FormBorderStyle = FormBorderStyle.None;
           // iSendForm.WindowState = FormWindowState.Maximized;
            documentManager1.View.AddDocument(iSendForm);
            iSendForm.Show();
        }

        private void Cascade_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            (documentManager1.View.Controller as INativeMdiViewController).Cascade();
        }

        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraBars.Docking2010.Views.BaseDocument doc = documentManager1.GetDocument(iTemForm);
            if (doc == null)
            { iTemForm = new iTemData(); documentManager1.View.AddDocument(iTemForm); }
            iTemForm.MdiParent = this;
            //iTemForm.FormBorderStyle = FormBorderStyle.None;
            //iTemForm.WindowState = FormWindowState.Maximized;
            iTemForm.BringToFront();
           
            iTemForm.Show();
        }

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DevExpress.XtraBars.Docking2010.Views.BaseDocument doc = documentManager1.GetDocument(iPointForm);
            if (doc == null) iPointForm = new iPointData();
            iPointForm.MdiParent = this;
            //iPointForm.FormBorderStyle = FormBorderStyle.None;
           // iPointForm.WindowState = FormWindowState.Maximized;
            documentManager1.View.AddDocument(iPointForm);
            iPointForm.Show();

        }

   
      

        

    }
}