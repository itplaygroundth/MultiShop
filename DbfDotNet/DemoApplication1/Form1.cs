using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DemoApplication1
{
    using DbfDotNet;

    public partial class Form1 : Form
    {
        DbfTable<Individual> individuals;
        string[] male_firstnames = "Michael,Christopher,Joshua,Matthew,Daniel,David,Andrew,Justin,Ryan,Robert,James,Nicholas,Joseph,John,Jonathan,Kevin,Kyle,Brandon,William,Eric,Jose,Steven,Jacob,Brian,Tyler,Zachary,Aaron,Alexander,Adam,Thomas,Richard,Timothy,Benjamin,Jason,Jeffrey,Sean,Jordan,Jeremy,Travis,Cody,Nathan,Mark,Jesse,Charles,Juan,Samuel,Patrick,Dustin,Scott,Stephen,Paul,Bryan,Luis,Derek,Austin,Kenneth,Carlos,Gregory,Alex,Cameron,Jared,Jesus,Bradley,Christian,Corey,Victor,Cory,Miguel,Tylor,Edward,Francisco,Trevor,Adrian,Jorge,Ian,Antonio,Shawn,Ricardo,Vincent,Edgar,Erik,Peter,Shane,Evan,Chad,Alejandro,Brett,Gabriel,Eduardo,Raymond,Phillip,Mario,Marcus,Manuel,George,Martin,Spencer,Garrett,Casey".Split(',');
        string[] female_firstnames = "Jessica,Ashley,Amanda,Brittany,Sarrah,Jennifer,Stephanie,Samantha,Elizabeth,Megan,Nicole,Lauren,Melissa,Amber,Michelle,Heather,Christina,Rachel,Tiffany,Kayla,Danielle,Vanessa,Rebecca,Laura,Courtney,Katherine,Chelsea,Kimberly,Sara,Kelsey,Andrea,Alyssa,Crystal,Maria,Amy,Alexandra,Erica,Jasmine,Natalie,Hanna,Angela,Kelly,Brittney,Mary,Cassandra,Erin,Victoria,Jacqueline,Jamie,Lindsey,Alicia,Lisa,Katie,Allison,Kristen,Cynthia,Anna,Caitlin,Monica,Christine,Diana,Erika,Veronica,Kathryn,Whitney,Brianna,Nancy,Shannon,Kristina,Lindsay,Kristin,Marissa,Patricia,Brooke,Brenda,Angelica,Morgan,Adriana,April,Ana,Taylor,Tara,Jordan,Jenna,Catherine,Alexis,Karen,Melanie,Natasha,Sandra,Julie,Bianca,Krystal,Mayra,Holly,Alexandria,Monique,Leslie,Katelyn".Split(',');
        string[] lastnames = "Smith,Johnson,Williams,Jones,Brown,Davis,Miller,Wilson,Moore,Taylor,Anderson,Thomas,Jackson,White,Harris,Martin,Thompson,Garcia,Martinez,Robinson,Clark,Rodriguez,Lewis,Lee,Walker,Hall,Allen,Young,Hernandez,King,Wright,Lopez,Hill,Scott,Green,Adams,Baker,Gonzalez,Nelson,Carter,Mitchell,Perez,Roberts,Turner,Phillips,Campbell,Parker,Evans,Edwards,Collins,Stewart,Sanchez,Morris,Rogers,Reed,Cook,Morgan,Bell,Murphy,Bailey,Rivera,Cooper,Richardson,Cox,Howard,Ward,Torres,Peterson,Gray,Ramirez,James,Watson,Brooks,Kelly,Sanders,Price,Bennett,Wood,Barnes,Ross,Henderson,Coleman,Jenkins,Perry,Powell,Long,Patterson,Hughes,Flores,Washington,Butler,Simmons,Foster,Gonzales,Bryant,Alexander,Russell,Griffin,Diaz,Hayes".Split(',');
        string[] states = "Alabama,Alaska,Arizona,Arkansas,California,Colorado,Connecticut,Delaware,Florida,Georgia,Hawaii,Idaho,Illinois,Indiana,Iowa,Kansas,Kentucky,Louisiana,Maine,Maryland,Massachusetts,Michigan,Minnesota,Mississippi,Missouri,Montana,Nebraska,Nevada,New Hampshire,New Jersey,New Mexico,New York,North Carolina,North Dakota,Ohio,Oklahoma,Oregon,Pennsylvania,Rhode Island,South Carolina,South Dakota,Tennessee,Texas,Utah,Vermont,Virginia,Washington,West Virginia,Wisconsin".Split(',');
        const int ADD_INDIVIDUALS_COUNT = 100;
            
        Random ran = new Random(0);

        private string[] Split(ref string s)
        {
            var names = new List<string>();
            var len = s.Length;
            for (int i = 0; i < len; i++)
            {
                var c = s[i];
                if (char.IsLetter(c))
                {
                    int from = i;
                    while (char.IsLetter(c) && i < len)
                    {
                        i++;
                        c = s[i];
                    }
                    names.Add(s.Substring(from, i - from));
                }
            }
            return names.ToArray();
        }


        public Form1()
        {
            foreach (var fi in new System.IO.DirectoryInfo(".").GetFiles("*.ndx"))
            {
                fi.Delete();
            }

            InitializeComponent();
            individuals = new DbfTable<Individual>(@"individuals.dbf", Encoding.ASCII, DbfVersion.dBaseIV);


            listView1.View = View.Details;
            listView1.Columns.Add("Record Number", 80);
            for (int i = 1; i < individuals.Columns.Count; i++)
            {
                var c = individuals.Columns[i];
                listView1.Columns.Add(c.mFieldInfo.Name, 120);
            }

            listView1.VirtualMode = true;
            listView1.VirtualListSize = individuals.RecordCount;
            
            this.addToolStripMenuItem.Text = string.Format("Add {0:0,0} individuals", ADD_INDIVIDUALS_COUNT);
            RefreshBar();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        private string RandomEntry(string[] strings)
        {
            return strings[ran.Next(strings.Length)];
        }

        private Individual GetIndividualByRow(UInt32 rowNo)
        {
            Individual individual = null;
            if (mIndex == null)
            {
                individual = individuals.GetRecord(rowNo);
            }
            else
            {
                individual = mIndex.GetRecord(rowNo);
            }
            return individual;
        }

        void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var listviewItem = new ListViewItem();
            Individual individual = GetIndividualByRow((UInt32)e.ItemIndex);
            if (individual == null)
            {
                listviewItem.Text = "#null";
                listviewItem.SubItems.Add("#null");
                listviewItem.SubItems.Add("#null");
                listviewItem.SubItems.Add("#null");
                listviewItem.SubItems.Add("#null");
                listviewItem.SubItems.Add("#null");
            }
            else
            {
                listviewItem.Text = "#" + (e.ItemIndex + 1);
                listviewItem.SubItems.Add(individual.FIRSTNAME);
                listviewItem.SubItems.Add(individual.MIDDLENAME);
                listviewItem.SubItems.Add(individual.LASTNAME);
                listviewItem.SubItems.Add(individual.DOB.ToShortDateString());
                listviewItem.SubItems.Add(individual.STATE);
            }
            e.Item = listviewItem;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        DateTime mLastRefresh;

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (DateTime.Now.Subtract(mLastRefresh).TotalSeconds >= 5)
            {
                RefreshGrid();
                mLastRefresh = DateTime.Now;
            }
            toolStripStatusLabel1.Text = string.Format("{0}% ({1} records)", e.ProgressPercentage, individuals.RecordCount);
        }

        private void RefreshBar()
        {
            toolStripStatusLabel1.Text = string.Format("Ready ({0} records in database).", individuals.RecordCount);
        }

        private void RefreshGrid()
        {
            listView1.VirtualListSize = 0;
            listView1.VirtualListSize = individuals.RecordCount;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            individuals.Close();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int k = individuals.RecordCount;
            for (int i = 0; i < ADD_INDIVIDUALS_COUNT; i++)
            {
                var newIndiv = individuals.NewRecord();
                bool male = (ran.NextDouble() > 0.5);
                string[] firsntnames = (male ? male_firstnames : female_firstnames);
                newIndiv.FIRSTNAME = RandomEntry(firsntnames);
                string middleName = RandomEntry(firsntnames);
                if (middleName != newIndiv.FIRSTNAME)
                    newIndiv.MIDDLENAME = middleName;
                newIndiv.LASTNAME = RandomEntry(lastnames);
                newIndiv.STATE = RandomEntry(states);
                newIndiv.DOB = DateTime.Today.Subtract(new TimeSpan(ran.Next(36500), 0, 0, 0));
                k++;
                newIndiv.SaveChanges();
            }
            RefreshBar();
            RefreshGrid();
        }

        private void clearFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            individuals.EmptyTable();
            listView1.BeginUpdate();
            RefreshGrid();
            listView1.EndUpdate();
            RefreshBar();
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {

        }

        private void listView1_SearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e)
        {

        }

        private void listView1_Click(object sender, EventArgs e)
        {

        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == 0) mIndex = null;
            else SetIndex(individuals.Columns[e.Column].mFieldInfo.Name);
        }

        private DbfIndex<Individual> mIndex;

        private void SetIndex(string columnName)
        {
            var sortOrder = new SortOrder<Individual>(false);
            sortOrder.AddField(columnName);
            mIndex = individuals.GetIndex(columnName + ".ndx", sortOrder);
            RefreshGrid();
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var individual = GetIndividualByRow((UInt32)listView1.FocusedItem.Index);
            var detailForm = new DetailForm();
            detailForm.ReadIndividual(individual);
            if (detailForm.ShowDialog() == DialogResult.OK)
            {
                detailForm.WriteIndividual(individual);
                individual.SaveChanges();
            }
        }

    }
}
