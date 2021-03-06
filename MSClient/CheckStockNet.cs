﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO;

namespace MultiShop
{
    public partial class CheckStockNet : DevExpress.XtraEditors.XtraForm
    {
        private DataTable dstock;
        private static DataTable dcheck;
        private static string strCon;
        private WebServer ws;
        private static string instancename = ConfigurationManager.AppSettings["instanceName"].ToString();
        private static string username = ConfigurationManager.AppSettings["username"].ToString();
        private static string password = ConfigurationManager.AppSettings["password"].ToString();
        private static string dbname = ConfigurationManager.AppSettings["dbname"].ToString();
        private string cuscode = ConfigurationManager.AppSettings["cuscode"].ToString();
        private static string ipadd = localIPAddress();
        private static string port = "8080";
        private static string docno = string.Empty;
        public CheckStockNet()
        {
            InitializeComponent();
            button2.Enabled = false;
            strCon = string.Format(@"Data Source={0}; Initial Catalog={1};User Id={2};Password={3};", instancename, dbname, username, password);
            ShopCodeLabel.Text = cuscode;
            preData();
        }

        private void CheckStock_Resize(object sender, EventArgs e)
        {
            splitContainerControl1.SplitterPosition = this.Width / 2;
        }

        private void loadData()
        {
            var offset = "1";
            var docdate = string.Format("{0}/{1}/{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var doctime = string.Format("{0}/{1}/{2} {3}:{4}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);
            using (var scon = new SqlConnection(strCon))
            {
                if (scon.State == ConnectionState.Closed)
                {
                    scon.Open();
                }
                try
                {
                    using (var da = new SqlDataAdapter(string.Format("select max(checkdocno) from bccheckstock where checkdate='{0}'", docdate), scon))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0][0].ToString() != String.Empty)
                            {
                                offset = (Convert.ToInt16(dt.Rows[0][0].ToString().Substring(dt.Rows[0][0].ToString().Length - 4, 4)) + 1).ToString();
                            }
                        }
                        for (var f = 0; f < 3; f++)
                        {
                            offset = "0" + offset;
                        }
                        offset = offset.Substring(0, 4);
                        docno = string.Format("CHK{0}{1}-{2}{3}", (DateTime.Now.Year + 543).ToString().Substring(2, 2), DateTime.Now.Month.ToString().Length < 2 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString().Length < 2 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString(), offset);
                    }
                    using (var scom = new SqlCommand(string.Format("insert into bccheckstock(cuscode,checkdate,checkdocno,checktime,iscancel)values('{0}','{1}','{2}','{3}',0)", cuscode, docdate, docno, doctime), scon))
                    {
                        scom.ExecuteNonQuery();
                    }
                    using (var scom = new SqlCommand(string.Format("insert into bccheckstocksub1(itemcode,itemname,unitcode,stockqty,checkdate,checkdocno,checktime,iscancel)select code,name1,defsaleunitcode,stockqty,'{0}','{1}','{2}',0 from bcitem where stockqty>0", docdate, docno, doctime), scon))
                    {
                        scom.ExecuteNonQuery();
                    }
                    using (var scom = new SqlCommand(string.Format("insert into bccheckstocksub2(itemcode,serialno,qty,checkindate,checkdocno,checkintime,iscancel)select bccheckstocksub1.itemcode,serialno,'0','{0}','{1}','{2}',0 from bccheckstocksub1,bcserialmaster where bccheckstocksub1.itemcode=bcserialmaster.itemcode and activestatus=1 and stockstatus=0 and checkdocno='{1}'", docdate, docno, doctime), scon))
                    {
                        scom.ExecuteNonQuery();
                    }


                    using (var da = new SqlDataAdapter(string.Format("select ItemCode as code,ItemName as name1,StockQty From BCCheckStockSub1 Where checkdate='{0}' and checkdocno='{1}'", docdate, docno), scon))
                    {
                        da.Fill(dstock);
                        gridControl1.DataSource = dstock;
                    }
                }
                catch (SqlException ex)
                {
                }
                finally
                {
                    if (gridView1.Columns.Count > 0)
                    {
                        gridView1.Columns[2].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        gridView2.Columns[2].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    }
                }
            }
        }
        private void preData()
        {
            dstock = new DataTable();
            dcheck = new DataTable();
            dstock.Columns.Add(new DataColumn() { ColumnName = "Code", Caption = "รหัส", DataType = typeof(string) });
            dstock.Columns.Add(new DataColumn() { ColumnName = "Name1", Caption = "ชื่อ", DataType = typeof(string) });
            dstock.Columns.Add(new DataColumn() { ColumnName = "StockQty", Caption = "จำนวน", DataType = typeof(double) });
            dcheck.Columns.Add(new DataColumn() { ColumnName = "Code", Caption = "รหัส", DataType = typeof(string) });
            dcheck.Columns.Add(new DataColumn() { ColumnName = "Name1", Caption = "ชื่อ", DataType = typeof(string) });
            dcheck.Columns.Add(new DataColumn() { ColumnName = "StockQty", Caption = "จำนวน", DataType = typeof(double) });
            gridControl2.DataSource = dcheck;
            dateLabel.Text = DateTime.Now.ToShortDateString();
            timeLabel.Text = DateTime.Now.ToShortTimeString();
            checkTable("BCCheckStock");
            checkTable("BCCheckStockSub1");
            checkTable("BCCheckStockSub2");
        }

        private void checkTable(string tablename)
        {
            using (var scon = new SqlConnection(strCon))
            {
                if (scon.State == ConnectionState.Closed)
                {
                    scon.Open();
                }
                try
                {
                    using (var scom = new SqlCommand(string.Format("select checkdocno from {0}", tablename), scon))
                    {
                        scom.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    var sr = new StreamReader(Path.GetDirectoryName(Application.ExecutablePath) + string.Format(@"\{0}.sql", tablename));
                    var sql = sr.ReadToEnd();
                    using (var scom = new SqlCommand(sql, scon))
                    {
                        scom.ExecuteNonQuery();
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ws = new WebServer(GetFormValues, string.Format("http://{0}:{1}/checkstock/", ipadd, port));
            ws.Run();
            UrlLabel.Text = string.Format("http://{0}:{1}/checkstock/", ipadd, port);
            loadData();
            button1.Enabled = false;
            button2.Enabled = true;
        }


        private void CheckStockNet_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (!checkCancel())
            {
                if (ws != null)
                {
                    ws.Stop();
                }
                e.Cancel = true;
            }
            else
            {
                if (ws != null)
                {
                    ws.Stop();
                }
                e.Cancel = false;
            }
        }

        private bool checkCancel()
        {
            var suc = false;
            var dt = new DataTable();
            if (gridControl2.DataSource != null)
            {
                dt = gridControl2.DataSource as DataTable;
            }
            if (dt.Rows.Count == 0
            )
            {
                if (MessageBox.Show("ต้องการยกเลิกการตรวจนับใช้หรือไม่!", "เตือน", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    suc = true;
                }
                else
                {
                    suc = false;
                }
            }
            return suc;
        }

        private  string GetFormValues(HttpListenerRequest request)
        {
            var formVars = new Hashtable();

            var html = @"<html><body><form action='" + string.Format("http://{0}:{1}/checkstock/", ipadd, port) + "' method='post'><input type='text' name='myvalue' value='My Input'><input type='submit'></form></body></html>";

            if (!request.HasEntityBody)
            {
                return html;
            }
            var body = request.InputStream;

            var encoding = request.ContentEncoding;

            var reader = new System.IO.StreamReader(body, encoding);

            if (request.ContentType.ToLower() == "application/x-www-form-urlencoded")
            {
                var s = reader.ReadToEnd();

                var pairs = s.Split('&');

                var item = pairs[0].Split('=');
                var docdate = string.Format("{0}/{1}/{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var doctime = string.Format("{0}/{1}/{2} {3}:{4}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute);
                formVars.Add(item[0], System.Web.HttpUtility.UrlDecode(item[1]));
                var resql = string.Format("update bccheckstocksub2 set qty=1,checkeddate='{0}',checkedtime='{1}' where checkdocno='{2}' and serialno='{3}'", docdate, doctime, docno, item[1]);
                using (var scon = new SqlConnection(strCon))
                {
                    if (scon.State == ConnectionState.Closed)
                    {
                        scon.Open();
                    }
                    using (var scom = new SqlCommand(resql, scon))
                    {
                        scom.ExecuteNonQuery();
                    }
                    this.Invoke(new MethodInvoker(delegate
                    {
                        using (var da = new SqlDataAdapter(string.Format("select ItemCode as Code,(select Name1 from BCItem where code=bccheckstocksub2.ItemCode) as Name1,SUM(Qty) as StockQty from BCCheckStockSub2 where CheckDocNo='{0}' group by ItemCode", docno), scon))
                        {
                            dcheck.Clear();
                            da.Fill(dcheck);
                        }
                    }));
                }
            }

            body.Close();

            reader.Close();



            return html;
        }

        public static string localIPAddress()
        {
            IPHostEntry host;
            var localIP = string.Empty;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                localIP = ip.ToString();

                var temp = localIP.Split('.');
                if (ip.AddressFamily == AddressFamily.InterNetwork && temp[0] == "192")
                {
                    break;
                }
                else
                {
                    localIP = null;
                }
            }
            return localIP;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ws.Stop();
            button2.Enabled = false;
        }
    }
}
