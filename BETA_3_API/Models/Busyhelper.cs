using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Busy2184;
using System.Configuration;
using ESCommon;

namespace BETA_3_BUSY_HELP.Models
{
    public class Busyhelper
    {
        readonly CFixedInterface FI = new CFixedInterface();
        int Provider = clsMain.MyInt(ConfigurationManager.AppSettings["Provider"]);
        string BusyPath = ConfigurationManager.AppSettings["BusyPath"];
        string BusyDataPath = ConfigurationManager.AppSettings["BusyDataPath"];
        string ServerName = ConfigurationManager.AppSettings["ServerName"];
        string SUserName = ConfigurationManager.AppSettings["SUserName"];
        string SPassword = ConfigurationManager.AppSettings["SPassword"];

        public Busyhelper()
        {

        }
        public bool ValidateUser(string UserName, string Password, string ServerNm, string SuserNm, String Spass, int FY, string compcode, ref string UserType, ref string ErrorStr)
        {
            bool Connect;
           //string errstr = "";
            bool Uexist = false;
            Connect = true;
            //Connect = ConnectToBusy(compcode, FY, ServerNm, SuserNm, Spass, ref ErrorStr);
            if (Connect == true)
            {
                Uexist = true;//  FI.ValidUserPassword(UserName, Password);
                // SuperUser = FI.IfSuperUser(UserName);
                if (Uexist == true) { UserType = GetUserType(UserName, compcode, FY); }
             }
           return Uexist;
        }

        public string GetUserType(string UserName,string CompCode,int FY)
        {

            string UType = "";
            int UT = 0;
            string sql = "";
            DataTable table = new DataTable();
            try
            {
                string ConnectionString = GetConnectionString(Provider, CompCode, FY);
                sql = "select UType From ESUserMapping Where [User] = '" + UserName + "'";
                table = new SQLHELPER(ConnectionString).getTable(sql);
                if (table != null && table.Rows.Count > 0)
                {
                    UT = clsMain.MyInt(table.Rows[0]["UType"]);
                }
                if (UT == 1)
                {
                    UType = "Admin";
                }
                else if (UT == 2)
                {
                    UType = "SalesMan";
                }
                else if (UT == 3)
                {
                    UType = "Dispatcher";
                }
            }
            catch
            {
                return "";
            }
            return UType;
        }
        private string GetConnectionString(int Provider, string CompCode, int FY)
        {
            string DBName = "";
            string ConnectionString = "";

            if (Provider == 1)
            {
                DBName = "db1" + FY + ".bds";

                string DBFilePath = BusyDataPath + "\\" + CompCode + "\\" + DBName;
                ConnectionString = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + DBFilePath + ";Mode=ReadWrite;Persist Security Info=False;Jet OLEDB:Database Password=ILoveMyINDIA";
            }
            else
            {
                DBName = "Busy" + CompCode + "_db1" + FY;
                ConnectionString = "Data Source = " + ServerName + "; Initial catalog = " + DBName + "; Uid = " + SUserName + "; Pwd =" + SPassword + ";Max Pool Size=500";
            }

            return ConnectionString;
        }

        public static string FormatDate(string dateString)
        {
            if (dateString == null) return null;

            // Parse the received date string into a DateTime object using the specified format
            DateTime date = DateTime.ParseExact(dateString, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

            // Format the DateTime object as "dd-MMM-yyyy"
            string formattedDate = date.ToString("dd-MMM-yyyy");

            // Return the formatted date
            return formattedDate;
        }

        //private bool ConnectToBusy(string _CompCode, int _FinYear, string _SQLServerName, string _SQLUserName, string _SQLPassword, ref string ErrorStr)
        //{
        //    bool Connect = false;
        //    try
        //    {
        //        FI.CloseDB();
        //        Connect = FI.OpenCSDBForYear("C:\\BusyWin", _SQLServerName, _SQLUserName, _SQLPassword, _CompCode, Convert.ToInt16(_FinYear));
        //    }
        //    catch (Exception EX)
        //    {
        //        ErrorStr = EX.Message.ToString();
        //        return false;
        //    }

        //    if (Connect == false)
        //    {
        //        ErrorStr = "Unable to Connect to Company";
        //    }
        //    return Connect;
        //}
    }

}