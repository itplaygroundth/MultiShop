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
using System.IO;


namespace DbfTools
{
    public partial class DbfForm : XtraForm
    {

        string path = "";
        string filename = "";
        string strconnection = "";
        public DbfForm()
        {
            InitializeComponent();
            InitGrid();
           // InitTreeListControl();

        }
        BindingList<Person> gridDataList = new BindingList<Person>();
        void InitGrid()
        {
            gridDataList.Add(new Person("John", "Smith"));
            gridDataList.Add(new Person("Gabriel", "Smith"));
            gridDataList.Add(new Person("Ashley", "Smith", "some comment"));
            gridDataList.Add(new Person("Adrian", "Smith", "some comment"));
            gridDataList.Add(new Person("Gabriella", "Smith", "some comment"));
            gridControl.DataSource = gridDataList;
        }
        void InitTreeListControl(DataTable dt)
        {
            Projects projects = InitData(dt);
            DataBinding(projects);
        }
        Projects InitData(DataTable dt)
        {
            Projects projects = new Projects();
            projects.Add(new Project(filename, false));
            foreach(DataColumn dc in dt.Columns)
            projects[0].Projects.Add(new Project(dc.ColumnName, true));
            //projects.Add(new Project("Project A", false));
            //projects.Add(new Project("Project B", false));
            //projects[0].Projects.Add(new Project("Task 1", true));
            //projects[0].Projects.Add(new Project("Task 2", true));
            //projects[0].Projects.Add(new Project("Task 3", true));
            //projects[0].Projects.Add(new Project("Task 4", true));
            //projects[1].Projects.Add(new Project("Task 1", true));
            //projects[1].Projects.Add(new Project("Task 2", true));
            return projects;
        }
        void DataBinding(Projects projects)
        {
            treeList.ExpandAll();
            treeList.DataSource = projects;
            treeList.BestFitColumns();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "C# Corner Open File Dialog";
            fdlg.InitialDirectory = @"c:\";
            fdlg.Filter = "dbf files (*.dbf)|*.dbf|dbf files (*.dbf)|*.dbf";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                path=fdlg.FileName;
                filename = fdlg.SafeFileName;
                labelControl1.Text += path + "]" ;
                ReadDBF();


            
            }
            
        }

        private void ReadDBF()
        {

            try
            {

                DataTable dt = ParseDBF.ReadDBF(path);
                InitTreeListControl(dt);
               

            }
            catch (Exception ex)
            {
                // Do something with ex
                MessageBox.Show(ex.Message);
            }

        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            DataSource datasoruce = new DataSource();
            if (datasoruce.ShowDialog() == DialogResult.OK)
            {
                strconnection = datasoruce.connectionstring;
            
            }
        }

    }
}