using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmartLib.Controls
{
    public partial class DialogPasswordBox : DevExpress.XtraEditors.XtraForm
    {

        static DialogPasswordBox newMessageBox;
        static string Button_id;


        private string passcode;
        public string passCode
        {
            set { passcode = value; }
            get { return passcode; }
        }
        public DialogPasswordBox()
        {
            InitializeComponent();
            numPad1.setPasswordChar();
            numPad1.Text = "";
            numPad1.EnterClose += new EventHandler(numPad1_EnterClose);
            this.TopMost = true;
            this.ShowInTaskbar = false;
           
        }

        void numPad1_EnterClose(object sender, EventArgs e)
        {
            
          //  MessageBox.Show(numPad1.Text);
            passcode = numPad1.Text;
            if(string.Compare(numPad1.Text,"1211975")==0)
                Button_id="OK";
            else
                Button_id="CANCEL";
            this.Close();
            //throw new NotImplementedException();
        }

     
        //public static string GetString(string title)
        //{
        //    var box = new DialogMessageBox { Text = title };

        //    if (box.ShowDialog() == DialogResult.OK)
        //    {
        //        return box.ResultText;
        //    }

        //    return string.Empty;
        //}

        //private void okButton_Click(object sender, EventArgs e)
        //{
        //    this.ResultText = txtUserInput.Text;
        //    this.DialogResult = DialogResult.OK;
        //}

        public static string ShowBox(string txtMessage)
        {
            newMessageBox = new DialogPasswordBox();
            newMessageBox.labelControl1.Text = txtMessage;
            newMessageBox.ShowDialog();
            return Button_id;
        }

        public static DialogResult ShowBox(string txtMessage,string caption, string btnOk)
        {
            newMessageBox = new DialogPasswordBox();
            newMessageBox.labelControl1.Text = caption;
          //  newMessageBox.labelControl2.Text = txtMessage;
          //  newMessageBox.simpleButton1.Text = btnOk;

           // newMessageBox.simpleButton1.Left = newMessageBox.simpleButton2.Left;//(newMessageBox.Width - newMessageBox.simpleButton1.Width) / 2;
            //newMessageBox.simpleButton1.Top = (newMessageBox.Height - newMessageBox.simpleButton1.Height) / 2;
           // newMessageBox.simpleButton2.Visible = false;
            return newMessageBox.ShowDialog();
            //return Button_id;
        }
        public static DialogResult ShowBox(string txtMessage, string caption, string btnOk, string btnCancel)
        {
            newMessageBox = new DialogPasswordBox();
            newMessageBox.labelControl1.Text = caption;
         //   newMessageBox.labelControl2.Text = txtMessage;
         //   newMessageBox.simpleButton1.Text = btnOk;
         //   newMessageBox.simpleButton2.Text = btnCancel;
            return newMessageBox.ShowDialog();
            //return Button_id;
        } 

        //void simpleButton1_Click(object sender, System.EventArgs e)
        //{
        //    this.DialogResult = DialogResult.OK;
        //    Button_id = "1";
        //    newMessageBox.Dispose(); 
        //    //throw new System.NotImplementedException();
        //}

        //void simpleButton2_Click(object sender, System.EventArgs e)
        //{
        //    this.DialogResult = DialogResult.Cancel;
        //    Button_id = "1";
        //    newMessageBox.Dispose(); 
        //    //throw new System.NotImplementedException();
        //}

     
    }
}
