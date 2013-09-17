using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
namespace SmartServer
{
    //public interface IDisposable
    //{ void Dispose();
    //}

    public class SQLiteDatabase : IDisposable
    {
        String dbConnection;

        /// <summary>
        ///     Default Constructor for SQLiteDatabase Class.
        /// </summary>
        public SQLiteDatabase()
        {
            dbConnection = "Data Source=recipes.s3db";
        }

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="inputFile">The File containing the DB</param>
        public SQLiteDatabase(String inputFile)
        {
            dbConnection = String.Format("Data Source={0}", inputFile);
        }


        /// <summary>
        ///     Single Param Constructor for specifying advanced connection options.
        /// </summary>
        /// <param name="connectionOpts">A dictionary containing all desired options and their values</param>
        public SQLiteDatabase(Dictionary<String, String> connectionOpts)
        {
            String str = "";
            foreach (KeyValuePair<String, String> row in connectionOpts)
            {
                str += String.Format("{0}={1}; ", row.Key, row.Value);
            }
            str = str.Trim().Substring(0, str.Length - 1);
            dbConnection = str;
        }

        public void Dispose()
        { 
        
        }


        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(dbConnection);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        public DataTable GetByDataAdapter(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(dbConnection);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                SQLiteDataAdapter Da = new SQLiteDataAdapter(sql, cnn);
                Da.Fill(dt);
                //SQLiteDataReader reader = mycommand.ExecuteReader();
                //dt.Load(reader);
                //reader.Close();
                cnn.Close();


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            cnn.Close();

            return rowsUpdated;
        }

        /// <summary>
        ///     Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="sql">The query to run.</param>
        /// <returns>A string.</returns>
        public string ExecuteScalar(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            object value = mycommand.ExecuteScalar();
            cnn.Close();
            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(String tableName, Dictionary<String, String> data, String where)
        {
            String vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            try
            {
                this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch
            {
                returnCode = false;
            }
            return returnCode;
        }

        public String Updatestr(String tableName, Dictionary<String, String> data, String where)
        {
            String vals = "";
            string sql = "";
            
            if (data.Count >= 1)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            try
            {
                sql = String.Format("update {0} set {1} where {2};", tableName, vals, where);
                this.ExecuteNonQuery(sql);
            }
            catch
            {
            }
            return sql;
        }


        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Insert(String tableName, Dictionary<String, String> data)
        {
            string sql = "";
            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", val.Value.Replace("'", ""));
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                sql = String.Format("insert into {0}({1}) values({2});", tableName, columns, values);
                this.ExecuteNonQuery(sql);
            }
            catch (Exception fail)
            {
               // MessageBox.Show(fail.Message + ":" + sql);
                returnCode = false;
            }
            return returnCode;
        }
        public string Insertstr(String tableName, Dictionary<String, String> data)
        {
            string sql = "";
            String columns = "";
            String values = "";
            
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", val.Value.Replace("'", ""));
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                sql = String.Format("insert into {0}({1}) values({2});", tableName, columns, values);
                this.ExecuteNonQuery(sql);
            }
            catch (Exception fail)
            {
                // MessageBox.Show(fail.Message + ":" + sql);
               
            }
            return sql;
        }
        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearDB()
        {
            DataTable tables;
            try
            {
                tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows)
                {
                    this.ClearTable(table["NAME"].ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearTable(String table)
        {
            try
            {

                this.ExecuteNonQuery(String.Format("delete from {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string queryFormCtrl(Control ctl, string querytype, string table)
        {
            string sql = "";
            int i = 0;
            List<string> sql1 = new List<string>();
            List<string> field = new List<string>();
            List<string> value = new List<string>();
            try
            {
                foreach (Control ctrl in ctl.Controls)
                {
                    if (ctrl is TextBox || ctrl is DevExpress.XtraEditors.ComboBoxEdit || ctrl is DevExpress.XtraEditors.TextEdit || ctrl is DevExpress.XtraEditors.DataNavigator)
                    {
                        field.Add(ctrl.Name);
                        value.Add(string.Format("'{0}'", ctrl.Text));
                    }
                }
                switch (querytype)
                {
                    case "insert":
                        sql = string.Format("insert into {0} ({1})values({2})", table, string.Join(",", field.ToArray()), string.Join(",", value.ToArray()));
                        break;
                    case "update":
                        string[] f = field.ToArray();
                        string[] v = value.ToArray();

                        foreach (string fields in f)
                        {
                            sql1.Add(string.Format("{0}={1}", fields, v[i]));
                            i += 1;
                        }
                        sql = string.Format("update {0} set {1}", table, string.Join(",", sql1.ToArray()));
                        break;
                }
            }
            catch
            {

            }

            return sql;
        }

        public string queryFormMultiCtrl(Control[] ctl, string querytype, string table)
        {
            string sql = "";
            int i = 0;
            List<string> sql1 = new List<string>();
            List<string> field = new List<string>();
            List<string> value = new List<string>();
            try
            {
                foreach (Control clt in ctl)
                    foreach (Control ctrl in clt.Controls)
                    {
                        if (ctrl is TextBox || ctrl is DevExpress.XtraEditors.ComboBoxEdit || ctrl is DevExpress.XtraEditors.TextEdit || ctrl is DevExpress.XtraEditors.DataNavigator)
                        {
                            field.Add(ctrl.Name);
                            value.Add(string.Format("'{0}'", ctrl.Text));
                        }
                    }
                switch (querytype)
                {
                    case "insert":
                        sql = string.Format("insert into {0} ({1})values({2})", table, string.Join(",", field.ToArray()), string.Join(",", value.ToArray()));
                        break;
                    case "update":
                        string[] f = field.ToArray();
                        string[] v = value.ToArray();

                        foreach (string fields in f)
                        {
                            if (i > 0)
                                sql1.Add(string.Format("{0}={1}", fields, v[i]));
                            i += 1;
                        }
                        sql = string.Format("update {0} set {1} where {2}={3}", table, string.Join(",", sql1.ToArray()), f[0], v[0]);
                        break;
                }
            }
            catch
            {

            }

            return sql;
        }


        public string queryFormCtrl(Control ctl, string querytype, string table, string[] exfield, string[] exvalue)
        {
            string sql = "";
            int i = 0;
            List<string> sql1 = new List<string>();
            List<string> field = new List<string>();
            List<string> value = new List<string>();
            try
            {
                foreach (Control ctrl in ctl.Controls)
                {
                    if (ctrl is TextBox || ctrl is DevExpress.XtraEditors.ComboBoxEdit)
                    {
                        field.Add(ctrl.Name);
                        value.Add(string.Format("'{0}'", ctrl.Text));
                    }
                }
                switch (querytype)
                {
                    case "insert":
                        if (exfield.Length > 1)
                            sql = string.Format("insert into {0} ({1}{3})values({2}{4})", table, string.Join(",", field.ToArray()), string.Join(",", value.ToArray()), string.Format(",{0}", string.Join(",", exfield)), string.Format(",{0}", string.Join(",", exvalue)));
                        else
                            sql = string.Format("insert into {0} ({1}{3})values({2}{4})", table, string.Join(",", field.ToArray()), string.Join(",", value.ToArray()), string.Format(",{0}", exfield[0]), string.Format(",{0}", exvalue[0]));
                        break;
                    case "update":
                        string[] f = field.ToArray();
                        string[] v = value.ToArray();

                        foreach (string fields in f)
                        {
                            sql1.Add(string.Format("{0}={1}", fields, v[i]));
                            i += 1;
                        }
                        i = 0;
                        foreach (string fields in exfield)
                        {
                            sql1.Add(string.Format("{0}={1}", fields, exvalue[i]));
                            i += 1;
                        }
                        sql = string.Format("update {0} set {1}", table, string.Join(",", sql1.ToArray()));
                        break;
                }
            }
            catch
            {

            }

            return sql;
        }

    }    

}
