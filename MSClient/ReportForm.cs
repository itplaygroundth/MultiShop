using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace MultiShop
{
    public partial class ReportForm : DevExpress.XtraEditors.XtraForm
    {
        public ReportForm()
        {
            InitializeComponent();
            initialTree();
        }
        private void initialTree()
        {
            CreateColumns(treeList1);
            CreateNodes(treeList1);
        }
        private void CreateColumns(TreeList tl)
        {
            // Create three columns.
            tl.BeginUpdate();
            tl.Columns.Add();
            tl.Columns[0].Caption = "รายงาน";
            tl.Columns[0].VisibleIndex = 0;
            //tl.Columns.Add();
            //tl.Columns[1].Caption = "Location";
            //tl.Columns[1].VisibleIndex = 1;
            //tl.Columns.Add();
            //tl.Columns[2].Caption = "Phone";
            //tl.Columns[2].VisibleIndex = 2;
            tl.EndUpdate();
        }

        private void CreateNodes(TreeList tl)
        {
            tl.BeginUnboundLoad();
            // Create a root node .
            TreeListNode parentForRootNodes = null;
            TreeListNode rootNode0 = tl.AppendNode(new object[] { "ขายสินค้า"},parentForRootNodes);
            TreeListNode rootNode1 = tl.AppendNode(new object[] { "สินค้าคงเหลือ" }, parentForRootNodes);
            
            // Create a child of the rootNode
            tl.AppendNode(new object[] { "รายงานการขายเงินสด"}, rootNode0);
            tl.AppendNode(new object[] { "รายงานการขายเงินสด-ลดหนี้" }, rootNode0);
            tl.AppendNode(new object[] { "รายงานสินค้าคงเหลือ" }, rootNode1);
            // Creating more nodes
            // ...
            tl.EndUnboundLoad();
        }

        private void treeList1_Click(object sender, EventArgs e)
        {
           DevExpress.XtraTreeList.TreeList tree = sender as DevExpress.XtraTreeList.TreeList;
           DevExpress.XtraTreeList.TreeListHitInfo info = tree.CalcHitInfo(tree.PointToClient(MousePosition));
           //if (info.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Cell) MessageBox.Show(tree.node);
        }

        private void treeList1_AfterFocusNode(object sender, NodeEventArgs e)
        {
            MessageBox.Show(e.Node.Id.ToString());
        }
    }
}