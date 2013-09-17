namespace MSClient
{
    partial class MSForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lockTimerCounter++;
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
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
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSForm));
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.ShopIpLabel = new DevExpress.XtraEditors.LabelControl();
            this.ShopNameLabel = new DevExpress.XtraEditors.LabelControl();
            this.ShopCodeLabel = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.dateLabel = new DevExpress.XtraEditors.LabelControl();
            this.timeLabel = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.digitalGauge2 = new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalGauge();
            this.digitalBackgroundLayerComponent2 = new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalGauge2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 4.25F);
            this.groupControl1.AppearanceCaption.Options.UseFont = true;
            this.groupControl1.Controls.Add(this.ShopIpLabel);
            this.groupControl1.Controls.Add(this.ShopNameLabel);
            this.groupControl1.Controls.Add(this.ShopCodeLabel);
            this.groupControl1.Controls.Add(this.labelControl5);
            this.groupControl1.Controls.Add(this.labelControl4);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Location = new System.Drawing.Point(3, 6);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(601, 98);
            this.groupControl1.TabIndex = 0;
            // 
            // ShopIpLabel
            // 
            this.ShopIpLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.ShopIpLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.ShopIpLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.ShopIpLabel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.ShopIpLabel.Location = new System.Drawing.Point(138, 59);
            this.ShopIpLabel.Name = "ShopIpLabel";
            this.ShopIpLabel.Size = new System.Drawing.Size(176, 25);
            this.ShopIpLabel.TabIndex = 8;
            // 
            // ShopNameLabel
            // 
            this.ShopNameLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.ShopNameLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.ShopNameLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.ShopNameLabel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.ShopNameLabel.Location = new System.Drawing.Point(314, 28);
            this.ShopNameLabel.Name = "ShopNameLabel";
            this.ShopNameLabel.Size = new System.Drawing.Size(252, 25);
            this.ShopNameLabel.TabIndex = 7;
            // 
            // ShopCodeLabel
            // 
            this.ShopCodeLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.ShopCodeLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.ShopCodeLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.ShopCodeLabel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.ShopCodeLabel.Location = new System.Drawing.Point(112, 28);
            this.ShopCodeLabel.Name = "ShopCodeLabel";
            this.ShopCodeLabel.Size = new System.Drawing.Size(118, 25);
            this.ShopCodeLabel.TabIndex = 6;
            this.ShopCodeLabel.Text = "90000913";
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.labelControl5.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.labelControl5.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl5.Location = new System.Drawing.Point(34, 59);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(98, 25);
            this.labelControl5.TabIndex = 5;
            this.labelControl5.Text = "ไอพีแอดเดรส :";
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.labelControl4.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.labelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl4.Location = new System.Drawing.Point(251, 28);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(57, 25);
            this.labelControl4.TabIndex = 4;
            this.labelControl4.Text = "ชื่อร้าน :";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.labelControl3.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl3.Location = new System.Drawing.Point(34, 28);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(72, 25);
            this.labelControl3.TabIndex = 3;
            this.labelControl3.Text = "รหัสร้าน :";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.panelControl2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1292, 112);
            this.panelControl1.TabIndex = 1;
            // 
            // panelControl2
            // 
            this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl2.Controls.Add(this.groupControl2);
            this.panelControl2.Controls.Add(this.groupControl1);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControl2.Location = new System.Drawing.Point(456, 2);
            this.panelControl2.Margin = new System.Windows.Forms.Padding(4);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(834, 108);
            this.panelControl2.TabIndex = 1;
            // 
            // groupControl2
            // 
            this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 4.25F);
            this.groupControl2.AppearanceCaption.Options.UseFont = true;
            this.groupControl2.Controls.Add(this.dateLabel);
            this.groupControl2.Controls.Add(this.timeLabel);
            this.groupControl2.Controls.Add(this.labelControl2);
            this.groupControl2.Controls.Add(this.labelControl1);
            this.groupControl2.Location = new System.Drawing.Point(610, 6);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(221, 98);
            this.groupControl2.TabIndex = 7;
            // 
            // dateLabel
            // 
            this.dateLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.dateLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.dateLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.dateLabel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.dateLabel.Location = new System.Drawing.Point(93, 24);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(104, 25);
            this.dateLabel.TabIndex = 3;
            // 
            // timeLabel
            // 
            this.timeLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.timeLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.timeLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.timeLabel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.timeLabel.Location = new System.Drawing.Point(93, 61);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(104, 25);
            this.timeLabel.TabIndex = 5;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.labelControl2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.Location = new System.Drawing.Point(30, 61);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(57, 25);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "เวลา :";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25F);
            this.labelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Location = new System.Drawing.Point(24, 24);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(57, 25);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "วันที่";
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.RelationName = "Level1";
            this.gridControl1.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1284, 485);
            this.gridControl1.TabIndex = 2;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridView1_RowClick);
            this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // timer
            // 
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // digitalGauge2
            // 
            this.digitalGauge2.AppearanceOff.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#34000000");
            this.digitalGauge2.AppearanceOn.ContentBrush = new DevExpress.XtraGauges.Core.Drawing.SolidBrushObject("Color:#7F93C6");
            this.digitalGauge2.BackgroundLayers.AddRange(new DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent[] {
            this.digitalBackgroundLayerComponent2});
            this.digitalGauge2.Bounds = new System.Drawing.Rectangle(306, 5, 142, 72);
            this.digitalGauge2.DigitCount = 6;
            this.digitalGauge2.Name = "digitalGauge2";
            this.digitalGauge2.Text = "00,000";
            // 
            // digitalBackgroundLayerComponent2
            // 
            this.digitalBackgroundLayerComponent2.BottomRight = new DevExpress.XtraGauges.Core.Base.PointF2D(307.775F, 99.9625F);
            this.digitalBackgroundLayerComponent2.Name = "digitalBackgroundLayerComponent1";
            this.digitalBackgroundLayerComponent2.ShapeType = DevExpress.XtraGauges.Core.Model.DigitalBackgroundShapeSetType.Style11;
            this.digitalBackgroundLayerComponent2.TopLeft = new DevExpress.XtraGauges.Core.Base.PointF2D(20F, 0F);
            this.digitalBackgroundLayerComponent2.ZOrder = 1000;
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.xtraTabControl1.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 112);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(1292, 525);
            this.xtraTabControl1.TabIndex = 3;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Appearance.Header.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.xtraTabPage1.Appearance.Header.Options.UseFont = true;
            this.xtraTabPage1.Controls.Add(this.gridControl1);
            this.xtraTabPage1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(1284, 485);
            this.xtraTabPage1.Text = "รับสินค้า";
            // 
            // MSForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1292, 637);
            this.Controls.Add(this.xtraTabControl1);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MSForm";
            this.Text = "MultiShop Client";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MClient_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalGauge2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.digitalBackgroundLayerComponent2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

     

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.Timer timer;
        private DevExpress.XtraGauges.Win.Gauges.Digital.DigitalGauge digitalGauge2;
        private DevExpress.XtraGauges.Win.Gauges.Digital.DigitalBackgroundLayerComponent digitalBackgroundLayerComponent2;
        private DevExpress.XtraEditors.LabelControl timeLabel;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl dateLabel;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl ShopIpLabel;
        private DevExpress.XtraEditors.LabelControl ShopNameLabel;
        private DevExpress.XtraEditors.LabelControl ShopCodeLabel;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;


    }
}
