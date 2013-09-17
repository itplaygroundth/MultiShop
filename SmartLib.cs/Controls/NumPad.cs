using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using DevComponents.DotNetBar.Keyboard;
using DevExpress.XtraEditors;

namespace SmartLib.Controls
{
    public partial class NumPad : DevExpress.XtraEditors.XtraUserControl
    {

        public event EventHandler EnterClose;


        private TileItem myTile;
        public TileItem TileCaller
        {
            get { return myTile; }
        }

        private string caller;
        public string Caller
        {
            get
            {
                return caller;
        	}
        }
        public string Text
        {
        	get 
            {
                return textEdit1.Text;
            }
            set {
                textEdit1.Text = value;
            }
        }

        private char charpass;
        public char charPass
        {
            set { charpass = value; }
        }
        private DialogResult dialogresult;
        public DialogResult Dialogresult
        {
        	get 
            {
                return dialogresult;
            }
        }

        public NumPad(TileItem tile)
        {
            InitializeComponent();
            myTile = tile;
            textEdit1.Properties.PasswordChar = charpass;
            keyboardControl1.Keyboard = CreateNumericKeyboard();


            keyboardControl1.Invalidate();
            keyboardControl1.IsTopBarVisible = false;


        }

        public NumPad(string calling)
        {
            InitializeComponent();
            textEdit1.Properties.PasswordChar = charpass;
            caller = calling;
            keyboardControl1.Keyboard = CreateNumericKeyboard();


            keyboardControl1.Invalidate();
            keyboardControl1.IsTopBarVisible = false;


        }

        public NumPad()
        {
            InitializeComponent();
            textEdit1.Properties.PasswordChar = charpass;
            keyboardControl1.Keyboard = CreateNumericKeyboard();


            keyboardControl1.Invalidate();
            keyboardControl1.IsTopBarVisible = false;
            
            
        }

        //private Keyboard CreateNumericKeyboard()
        //{
        //    Keyboard keyboard = new Keyboard();

        //    LinearKeyboardLayout klNumLockOn = new LinearKeyboardLayout();

        //    //klNumLockOn.AddKey("NumLock", info: null, style: KeyStyle.Pressed, layout: 1);
        //    //klNumLockOn.AddKey("/", "{DIVIDE}");
        //    //klNumLockOn.AddKey("*", "{MULTIPLY}");
        //    //klNumLockOn.AddKey("-", "{SUBTRACT}");
        //    //klNumLockOn.AddLine();

        //    //klNumLockOn.AddKey("7",style: KeyStyle.Normal,width:15,height:45);
        //    klNumLockOn.AddKey("7");
        //    klNumLockOn.AddKey("8");
        //    klNumLockOn.AddKey("9");
        //    klNumLockOn.AddKey("BACK", "{BackSpace}");

        //    //klNumLockOn.AddKey("+", "{ADD}", height: 21);
        //    klNumLockOn.AddLine();

        //    klNumLockOn.AddKey("4");
        //    klNumLockOn.AddKey("5");
        //    klNumLockOn.AddKey("6");
        //    klNumLockOn.AddKey("");
        //    klNumLockOn.AddLine();

        //    klNumLockOn.AddKey("1");
        //    klNumLockOn.AddKey("2");
        //    klNumLockOn.AddKey("3");
        //    klNumLockOn.AddKey("Enter", "{ENTER}", height: 21);
        //    klNumLockOn.AddLine();

        //    klNumLockOn.AddKey("0", width: 21);
        //    klNumLockOn.AddKey(".");


        //    LinearKeyboardLayout klNumLockOff = new LinearKeyboardLayout();

        //    klNumLockOff.AddKey("NumLock", style: KeyStyle.Normal, layout: 0);
        //    klNumLockOff.AddKey("/", "{DIVIDE}");
        //    klNumLockOff.AddKey("*", "{MULTIPLY}");
        //    klNumLockOff.AddKey("-", "{SUBTRACT}");
        //    klNumLockOff.AddLine();

        //    klNumLockOff.AddKey("Home", "{HOME}");
        //    klNumLockOff.AddKey("Up", "{UP}");
        //    klNumLockOff.AddKey("PgUp", "{PGUP}");
        //    klNumLockOff.AddKey("+", "{ADD}", height: 21);
        //    klNumLockOff.AddLine();

        //    klNumLockOff.AddKey("Left", "{LEFT}");
        //    klNumLockOff.AddKey("");
        //    klNumLockOff.AddKey("Right", "{RIGHT}");
        //    klNumLockOff.AddLine();

        //    klNumLockOff.AddKey("End", "{END}");
        //    klNumLockOff.AddKey("Down", "{DOWN}");
        //    klNumLockOff.AddKey("PgDn", "{PGDN}");
        //    klNumLockOff.AddKey("Enter", "{ENTER}", height: 21);
        //    klNumLockOff.AddLine();

        //    klNumLockOff.AddKey("Ins", "{INSERT}", width: 21);
        //    klNumLockOff.AddKey(".");


        //    keyboard.Layouts.Add(klNumLockOn);
        //    //keyboard.Layouts.Add(klNumLockOff);

        //    return keyboard;
        //}

        public void setPasswordChar()
        {
            textEdit1.Properties.PasswordChar = '*';
        }

        private void textEdit1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                if (EnterClose != null)
                {
                    EnterClose(this, null); dialogresult = DialogResult.OK;
                    //Hide();
                }
                //dialogresult = DialogResult.OK;
            }
        }

        private void NumPad_SizeChanged(object sender, EventArgs e)
        {
            dialogresult = DialogResult.OK;
        }
    }
}