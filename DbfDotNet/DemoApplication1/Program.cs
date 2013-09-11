using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DemoApplication1
{
    static class Program
    {
 
        //public static string HexDump2(byte[] bytes)
        //{
        //    var sb = new System.Text.StringBuilder();
        //    for (int line = 0; line < bytes.Length; line += 16)
        //    {
        //        byte[] lineBytes = bytes.Skip(line).Take(16).ToArray();
        //        sb.AppendFormat("{0:x8} ", line);
        //        sb.Append(string.Join(" ", lineBytes.Select(b => b.ToString("x2")).ToArray()).PadRight(16 * 3));
        //        sb.Append(" ");
        //        sb.Append(new string(lineBytes.Select(b => b < 32 ? '.' : (char)b).ToArray()));
        //    }
        //    return sb.ToString();
        //}

        [STAThread]
        static void Main()
        {
            //var bytes = new byte[1024 * 5];
            //var sw = System.Diagnostics.Stopwatch.StartNew();
            //for (int i=0;i<1000;i++) DbfDotNet.Core.Utils.HexDump(bytes);
            //System.Diagnostics.Debug.WriteLine("Mine:" + sw.ElapsedMilliseconds);
            //sw = System.Diagnostics.Stopwatch.StartNew();
            //for (int i=0;i<1000;i++) HexDump2(bytes);
            //System.Diagnostics.Debug.WriteLine("Yours:" + sw.ElapsedMilliseconds);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
