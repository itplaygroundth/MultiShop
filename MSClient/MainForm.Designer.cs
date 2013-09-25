namespace MultiShop
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.XtraEditors.TileItemElement tileItemElement1 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement3 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement4 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement5 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement6 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement7 = new DevExpress.XtraEditors.TileItemElement();
            this.tileControl1 = new DevExpress.XtraEditors.TileControl();
            this.tileGroup2 = new DevExpress.XtraEditors.TileGroup();
            this.Stocking = new DevExpress.XtraEditors.TileItem();
            this.tileItem3 = new DevExpress.XtraEditors.TileItem();
            this.tileGroup1 = new DevExpress.XtraEditors.TileGroup();
            this.tileItem4 = new DevExpress.XtraEditors.TileItem();
            this.tileItem2 = new DevExpress.XtraEditors.TileItem();
            this.tileGroup4 = new DevExpress.XtraEditors.TileGroup();
            this.tileItem5 = new DevExpress.XtraEditors.TileItem();
            this.tileItem6 = new DevExpress.XtraEditors.TileItem();
            this.tileItem1 = new DevExpress.XtraEditors.TileItem();
            this.SuspendLayout();
            // 
            // tileControl1
            // 
            this.tileControl1.BackColor = System.Drawing.Color.LightSlateGray;
            this.tileControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileControl1.Groups.Add(this.tileGroup2);
            this.tileControl1.Groups.Add(this.tileGroup1);
            this.tileControl1.Groups.Add(this.tileGroup4);
            this.tileControl1.Location = new System.Drawing.Point(0, 0);
            this.tileControl1.MaxId = 8;
            this.tileControl1.Name = "tileControl1";
            this.tileControl1.Size = new System.Drawing.Size(915, 463);
            this.tileControl1.TabIndex = 0;
            this.tileControl1.Text = "tileControl1";
            // 
            // tileGroup2
            // 
            this.tileGroup2.Items.Add(this.Stocking);
            this.tileGroup2.Items.Add(this.tileItem3);
            this.tileGroup2.Name = "tileGroup2";
            this.tileGroup2.Text = null;
            // 
            // Stocking
            // 
            this.Stocking.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 16.25F);
            this.Stocking.AppearanceItem.Normal.Options.UseFont = true;
            this.Stocking.ContentAnimation = DevExpress.XtraEditors.TileItemContentAnimationType.Fade;
            tileItemElement1.Text = "รับสินค้า";
            this.Stocking.Elements.Add(tileItemElement1);
            this.Stocking.Id = 0;
            this.Stocking.IsLarge = true;
            this.Stocking.Name = "Stocking";
            this.Stocking.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.Stocking_ItemClick);
            // 
            // tileItem3
            // 
            this.tileItem3.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 16.25F);
            this.tileItem3.AppearanceItem.Normal.Options.UseFont = true;
            tileItemElement2.Text = "ขายสินค้า";
            this.tileItem3.Elements.Add(tileItemElement2);
            this.tileItem3.Id = 3;
            this.tileItem3.IsLarge = true;
            this.tileItem3.Name = "tileItem3";
            this.tileItem3.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem3_ItemClick);
            // 
            // tileGroup1
            // 
            this.tileGroup1.Items.Add(this.tileItem4);
            this.tileGroup1.Items.Add(this.tileItem2);
            this.tileGroup1.Name = "tileGroup1";
            this.tileGroup1.Text = null;
            // 
            // tileItem4
            // 
            this.tileItem4.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 16.25F);
            this.tileItem4.AppearanceItem.Normal.Options.UseFont = true;
            tileItemElement3.Text = "ตรวจนับสินค้า (Network)";
            this.tileItem4.Elements.Add(tileItemElement3);
            this.tileItem4.Id = 4;
            this.tileItem4.IsLarge = true;
            this.tileItem4.Name = "tileItem4";
            this.tileItem4.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem4_ItemClick);
            // 
            // tileItem2
            // 
            this.tileItem2.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 16.25F);
            this.tileItem2.AppearanceItem.Normal.Options.UseFont = true;
            tileItemElement4.Text = "ตรวจนับสืนค้า (StandAlone)";
            this.tileItem2.Elements.Add(tileItemElement4);
            this.tileItem2.Id = 2;
            this.tileItem2.IsLarge = true;
            this.tileItem2.Name = "tileItem2";
            this.tileItem2.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem2_ItemClick);
            // 
            // tileGroup4
            // 
            this.tileGroup4.Items.Add(this.tileItem5);
            this.tileGroup4.Items.Add(this.tileItem6);
            this.tileGroup4.Items.Add(this.tileItem1);
            this.tileGroup4.Name = "tileGroup4";
            this.tileGroup4.Text = null;
            // 
            // tileItem5
            // 
            this.tileItem5.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 22.25F);
            this.tileItem5.AppearanceItem.Normal.Options.UseFont = true;
            tileItemElement5.Text = "รายงาน";
            this.tileItem5.Elements.Add(tileItemElement5);
            this.tileItem5.Id = 5;
            this.tileItem5.IsLarge = true;
            this.tileItem5.Name = "tileItem5";
            // 
            // tileItem6
            // 
            this.tileItem6.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 18.25F);
            this.tileItem6.AppearanceItem.Normal.Options.UseFont = true;
            tileItemElement6.Text = "ตั้งค่า";
            this.tileItem6.Elements.Add(tileItemElement6);
            this.tileItem6.Id = 6;
            this.tileItem6.Name = "tileItem6";
            this.tileItem6.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem6_ItemClick);
            // 
            // tileItem1
            // 
            this.tileItem1.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 26.25F);
            this.tileItem1.AppearanceItem.Normal.Options.UseFont = true;
            tileItemElement7.Text = "ออก";
            this.tileItem1.Elements.Add(tileItemElement7);
            this.tileItem1.Id = 1;
            this.tileItem1.Name = "tileItem1";
            this.tileItem1.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem1_ItemClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 463);
            this.Controls.Add(this.tileControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Multi Shop";
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TileControl tileControl1;
        private DevExpress.XtraEditors.TileGroup tileGroup2;
        private DevExpress.XtraEditors.TileItem Stocking;
        private DevExpress.XtraEditors.TileItem tileItem2;
        private DevExpress.XtraEditors.TileItem tileItem1;
        private DevExpress.XtraEditors.TileItem tileItem3;
        private DevExpress.XtraEditors.TileGroup tileGroup1;
        private DevExpress.XtraEditors.TileItem tileItem4;
        private DevExpress.XtraEditors.TileItem tileItem5;
        private DevExpress.XtraEditors.TileGroup tileGroup4;
        private DevExpress.XtraEditors.TileItem tileItem6;
    }
}