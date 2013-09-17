using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;

namespace SaleApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.UserSkins.BonusSkins.Register();
            UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");

            Application.Run(new SaleForm());
        }
    }
    public class DBConnect
    {
        public string server_dsn { get; set; }
        public string database_name { get; set; }
        public string company_name { get; set; }
        public string provider_name { get; set; }
        public string usr_name { get; set; }
        public string pwd_name { get; set; }

    }
}