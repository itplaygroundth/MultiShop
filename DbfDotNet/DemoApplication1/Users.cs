using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoApplication1
{
    //class Program
    //{
    //    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential,
    //        Size = 0,
    //        Pack = 1)]
    //    struct none
    //    {
    //    }

    //    static void Main(string[] args)
    //    {

    //        using (var table1 = new DbfTable<test>(@"samples\t0\test.dbf", System.Text.Encoding.ASCII, DbfVersion.dBaseIV))
    //        {
    //            Console.WriteLine();
    //            Console.WriteLine("By Record");

    //            int cpt = 0;
    //            foreach (test test1 in table1) //table1)
    //            {
    //                cpt++;
    //                Console.WriteLine(string.Format("{0} {1} {2}", test1.ID, test1.NAME, test1.DATE));
    //            }

    //            Console.WriteLine();
    //            Console.WriteLine("By Index");

    //            var index1 = new DbfIndex<test>(@"samples\t0\name.ndx", table1);
    //            cpt = 0;
    //            index1.Dump();

    //            foreach (test test1 in index1) // table1)
    //            {
    //                Console.WriteLine(string.Format("{0} {1} {2}", test1.ID, test1.NAME, test1.DATE));
    //                cpt++;
    //            }

    //            var file2 = new FileInfo(@"samples\t0\test2.dbf");
    //            if (file2.Exists) file2.Delete();
    //            using (var table2 = new DbfTable<test>(file2.FullName, System.Text.Encoding.ASCII, DbfVersion.dBaseIV))
    //            {
    //                var sortOrder2 = new SortOrder<test>();
    //                sortOrder2.AddField("NAME");

    //                //var index2File = new FileInfo(@"samples\t0\name2.ndx");
    //                //if (index2File.Exists) index2File.Delete();
    //                //var index2 = new DbfIndex<test>(index2File.FullName, table2, sortOrder2);

    //                foreach (test test1 in index1) // table1)
    //                {
    //                    Console.WriteLine(string.Format("{0} {1} {2}", test1.ID, test1.NAME, test1.DATE));
    //                    var test2 = table2.NewRecord();
    //                    test2.ID = test1.ID;
    //                    test2.NAME = test1.NAME;
    //                    test2.NOTE = test1.NOTE;
    //                    test2.BOOLEAN = test1.BOOLEAN;
    //                    test2.DATE = test1.DATE;
    //                    test2.DECIMAL = test1.DECIMAL;
    //                    test2.STRING = test1.STRING;
    //                }
    //            }
    //        }





    //        //using (var table2 = new DbfTable<test>(@"samples\t0\test2.dbf", System.Text.Encoding.ASCII, DbfVersion.dBaseIV))
    //        //{
    //        //    var sortOrder2 = new SortOrder<test>();
    //        //    sortOrder2.AddField("NAME");
    //        //    var index2 = new DbfIndex<test>(@"samples\t0\name2.ndx", table2, sortOrder2);

    //        //    for (int i=0;i<10;i++)
    //        //    {
    //        //        var test2 = table2.NewRecord();
    //        //        test2.ID = i;
    //        //        test2.NAME = i.ToString();
    //        //        test2.NOTE = i.ToString();
    //        //        test2.BOOLEAN = (i & 1)==0;
    //        //        test2.DATE = DateTime.Now.AddDays(i);
    //        //        test2.DECIMAL = i;
    //        //        test2.STRING = i.ToString();
    //        //    }
    //        //}

    //        //using (var table2 = new DbfDotNet.Linq.DbfTable<test>(@"samples\t0\test.dbf", System.Text.Encoding.ASCII, DbfVersion.dBaseIV))
    //        //{
    //        //    var x2 = from test t in table2 where t.NAME == "PASCAL" select t;

    //        //    foreach (var x3 in x2)
    //        //    {
    //        //        Console.WriteLine(x3.NAME);
    //        //    }
    //        //    /* this does not work */
    //        //}

    //        // f.Value = 123;
    //    }
    //}

    class Individual : DbfDotNet.DbfRecord
    {
        [DbfDotNet.Column(Width = 20)]
        public string FIRSTNAME;
        [DbfDotNet.Column(Width = 20)]
        public string MIDDLENAME;
        [DbfDotNet.Column(Width = 20)]
        public string LASTNAME;
        public DateTime DOB;
        [DbfDotNet.Column(Width = 20)]
        public string STATE;
    }




}
