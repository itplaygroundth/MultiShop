namespace SmartLib.Controls
{
    partial class DialogPasswordBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.numPad1 = new SmartLib.Controls.NumPad();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.AllowHtmlString = true;
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.25F);
            this.labelControl1.Location = new System.Drawing.Point(38, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(128, 26);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "labelControl1";
            // 
            // numPad1
            // 
            this.numPad1.Location = new System.Drawing.Point(38, 68);
            this.numPad1.Name = "numPad1";
            this.numPad1.Size = new System.Drawing.Size(262, 341);
            this.numPad1.TabIndex = 8;
           // this.numPad1.charPass = '*';
            // 
            // DialogPasswordBox
            // 
            this.Appearance.BackColor = System.Drawing.Color.Black;
            this.Appearance.BorderColor = System.Drawing.Color.White;
            this.Appearance.ForeColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseBorderColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 447);
            this.ControlBox = false;
            this.Controls.Add(this.numPad1);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.LookAndFeel.SkinName = "Metropolis";
            this.Name = "DialogPasswordBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

     

    

        #endregion

        public DevExpress.XtraEditors.LabelControl labelControl1;
        //private NumPad numPad1;
    }
}
