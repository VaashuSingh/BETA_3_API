using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BETA_3_BUSY_HELP.Models;
using BETA_3_API_BUSY_VCH.Models;
using BETA_3_API.Models;
using System.Web.Hosting;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using ESCommon;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Configuration;
using static BETA_3_API_BUSY_VCH.Models.BusyVoucher;
using Busy2184;
using System.Xml;
using System.Xml.Serialization;
using System.Dynamic;

namespace BETA_3_API.Controllers
{
    public class ValuesController : ApiController
    {
        int Provider = clsMain.MyInt(ConfigurationManager.AppSettings["Provider"]);
        string BusyPath = ConfigurationManager.AppSettings["BusyPath"];
        string BusyDataPath = ConfigurationManager.AppSettings["BusyDataPath"];
        string ServerName = ConfigurationManager.AppSettings["ServerName"];
        string SUserName = ConfigurationManager.AppSettings["SUserName"];
        string SPassword = ConfigurationManager.AppSettings["SPassword"];
        string CompCode = ConfigurationManager.AppSettings["CompCode"];

        [HttpGet]
        public bool ValidateURL()
        {
            return true;
        }

        [HttpGet]
        public dynamic ValidateUser(string UName, string Pass, string CompCode, string FY)
        {
            Validate_User UList = new Validate_User(); int Status = 0; string StatusStr = string.Empty; string UType = string.Empty;
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY); int UT = 0;
                string sql = string.Empty; DataTable DT1 = new DataTable(); SQLHELPER obj = new SQLHELPER(constr);


                sql = $"SELECT TOP 1 ISNULL(A.[User], '') as [Username],  ISNULL(A.[PWD], '') as [Password],  ISNULL(A.[UType], '') as [UType], ISNULL(A.[MCCode], 0) as MCCode, IsNull(B.[Name], '') as MCName FROM [ESBTUserMapping] A Left Join Master1 B On A.MCCode = B.Code WHERE A.[User] = '" + UName + "' AND A.[PWD] = '" + Pass + "' ";
                DT1 = obj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    Status = 1; StatusStr = "Valid"; UT = clsMain.MyInt(DT1.Rows[0]["UType"]);
                    UType = UT == 1 ? "Inward" : UT == 2 ? "Outward" : UT == 3 ? "Both" : "Both";

                    UList.UserName = clsMain.MyString(DT1.Rows[0]["Username"]);
                    UList.Pwd = clsMain.MyString(DT1.Rows[0]["Password"]);
                    UList.UTCode = clsMain.MyInt(DT1.Rows[0]["UType"]);
                    UList.UTName = clsMain.MyString(UType);
                    UList.MCCode = clsMain.MyInt(DT1.Rows[0]["MCCode"]);
                    UList.MCName = clsMain.MyString(DT1.Rows[0]["MCName"]);

                    //sql = $"SELECT Top 1  0 as Status, '' as Msg, * FROM [ESUserMapping] WHERE [User] = '" + UName + "' ";
                    //DT1 = obj.getTable(sql);
                    //if (DT1 == null )
                    //{
                    //    sql = $"SELECT Top 1 0 as Status, '' as Msg, * FROM [ESUserMapping] WHERE [PWD] = '" + Pass + "'";
                    //    DT3 = obj.getTable(sql);
                    //    if (DT3 == null)
                    //    {
                    //        Status = 0; StatusStr = "Wrong password. Try again or click ‘Forgot password’ to reset it. !";
                    //    }
                    //}
                    //else
                    //{
                    //    Status = 0; StatusStr = "Couldn't find your username and password. !";
                    //}
                }
                else
                {
                    Status = 0; StatusStr = "Couldn't find your username and password. !";
                }

            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), Data = UList, };
            }
            return new { Status = Status, Msg = StatusStr, Data = UList };
        }

        [HttpGet]
        public dynamic GetDashboardCardSummaries(string CompCode, string FY, string Users)
        {
            List<Dashboard> dash = new List<Dashboard>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER con = new SQLHELPER(constr);

                string sql = $"Select TOP 1 IsNull(UType, 0) as UTCode, (CASE WHEN UType = 1 THEN 'Inward' WHEN UType = 2 THEN 'Outward' WHEN UType = 3 THEN 'Both' ELSE 'Both' END) as UTName, IsNull(MCCode, 0) as MCCode From ESBTUserMapping Where [user] = '{Users.Replace("'", "''")}' Group By UType, MCCode";
                DataTable DT = new SQLHELPER(constr).getTable(sql);

                if (DT != null && DT.Rows.Count > 0)
                {
                    int UTCode = Convert.ToInt32(DT.Rows[0]["UTCode"]);
                    string UName = clsMain.MyString(DT.Rows[0]["UTName"]);
                    int MCCode = Convert.ToInt32(DT.Rows[0]["MCCode"]);

                    switch (UTCode)
                    {
                        case 1:
                            sql = $"SELECT 'Pending' AS Name, COUNT(VchCode) AS Value FROM ESBTTRAN1 WHERE MasterCode2 = {MCCode} And (Status Is Null Or Status = 0) And VchType = 108 UNION ALL SELECT 'Completed', COUNT(*) FROM ESBTTRAN1 WHERE [Status] IN (1, 3) And MasterCode2 = {MCCode} And VchType = 108";
                            break;
                        case 2:
                            sql = $"SELECT 'Pending' AS Name, COUNT(VchCode) AS Value FROM TRAN1 WHERE MasterCode2 = {MCCode} And [Flag] = 1 And [VchType] = 11 UNION ALL SELECT 'Completed', COUNT(*) FROM TRAN1 WHERE [MasterCode2] = {MCCode} And [Flag] = 2 And [VchType] = 11";
                            break;
                        case 3:
                            sql = $"SELECT 'Pending' AS Name, COUNT(VchCode) AS Value FROM ESBTTRAN1 WHERE MasterCode2 = {MCCode} And (Status Is Null Or Status = 0) And VchType = 108 UNION ALL SELECT 'Completed', COUNT(*) FROM ESBTTRAN1 WHERE [Status] = 1 And MasterCode2 = {MCCode} And VchType = 108";

                            break;
                        case 4:
                            sql = $"WITH Totals AS (SELECT A.VchCode, A.RefNo, ISNULL(T.TotQty, 0) AS TotQty FROM (SELECT DISTINCT A.RefCode AS VchCode, ISNULL(A.[RefNo], '') AS RefNo FROM ESJSLRefTran A INNER JOIN ESJSLTran1 B ON A.RefCode = B.VchCode WHERE A.RecType = 1 AND A.Method IN (1,2) AND B.[QStatus] = 1 AND B.Cancelled <> 1) A INNER JOIN (SELECT A.VchCode, A.RefNo, SUM(A.TQty) AS TotQty FROM (SELECT A.RefCode as VchCode, A.RefNo, A.MasterCode2, SUM(A.Value1) AS TQty FROM ESJSLRefTran A INNER JOIN Master1 M ON A.MasterCode2 = M.Code AND M.MasterType = 6 WHERE A.RecType = 1 And A.Method = 1 AND M.CM6 = {MCCode} GROUP BY A.RefCode, A.RefNo, A.MasterCode2) A GROUP BY A.VchCode, A.RefNo) T ON A.VchCode = T.VchCode AND A.RefNo = T.RefNo), " +
                            $"Transfers AS (SELECT A.VchCode, A.RefNo, ISNULL(TR.TransQty, 0) AS TransQty FROM (SELECT DISTINCT A.VchCode, ISNULL(A.[RefNo], '') AS RefNo FROM ESJSLRefTran A INNER JOIN ESJSLTran1 B ON A.RefCode = B.VchCode WHERE A.RecType = 1 AND A.Method IN (1, 2) AND B.[QStatus] = 1 AND B.Cancelled <> 1) A INNER JOIN (SELECT A.VchCode, A.RefNo, SUM(A.TRQty) AS TransQty FROM (SELECT A.RefCode as VchCode, A.RefNo, A.MasterCode2, (IsNull(SUM(A.Value1), 0) * (-1)) AS TRQty FROM ESJSLRefTran A INNER JOIN Master1 M ON A.MasterCode2 = M.Code AND M.MasterType = 6 WHERE A.RecType = 1 And A.Method = 2 AND M.CM6 = {MCCode} GROUP BY A.RefCode, A.RefNo, A.MasterCode2) A GROUP BY A.VchCode, A.RefNo) TR ON A.VchCode = TR.VchCode AND A.RefNo = TR.RefNo)" +
                            $"SELECT CASE WHEN T.TotQty = ISNULL(Tr.TransQty, 0) THEN 'Completed' ELSE 'Pending' END AS [Name], COUNT(*) As [Value] FROM Totals T LEFT JOIN Transfers Tr ON T.VchCode = Tr.VchCode AND T.RefNo = Tr.RefNo GROUP BY CASE WHEN T.TotQty = (ISNULL(Tr.TransQty, 0)) THEN 'Completed' ELSE 'Pending' END";

                            break;
                        case 5:
                            sql = $"SELECT 'Quotation' AS Name, COUNT(VchCode) AS Value FROM ESJSLTRAN1 WHERE CREATEDBY = '{Users}' UNION ALL SELECT 'Pending Orders', COUNT(*) FROM ESJSLTRAN1 WHERE QStatus = 1 And CREATEDBY = '{Users}' UNION ALL SELECT 'Completed Orders', COUNT(*) FROM ESJSLTRAN1 WHERE QStatus = 111 UNION ALL SELECT 'Replacement', COUNT(*) FROM ESJSLTRAN1 WHERE QStatus = 112 UNION ALL SELECT 'Invoice' , Count(*) FROM ESJSLTRAN1 WHERE QStatus = 112";
                            break;
                    }
                    DataTable DT1 = con.getTable(sql);

                    if (DT1 != null && DT1.Rows.Count > 0)
                    {
                        foreach (DataRow item in DT1.Rows)
                        {
                            dash.Add(new Dashboard
                            {
                                Name = clsMain.MyString(item["Name"]),
                                Values = clsMain.MyInt(item["Value"])
                            });
                        }
                    }
                    else
                    {
                        return new { Status = 0, Msg = "Data Not Found !!!" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), Data = dash };
            }
            return new { Status = 1, Msg = "Success", Data = dash };
        }

        [HttpGet]
        public dynamic GetVoucherList(string CompCode, string FY, string StartDate, string EndDate, int TranType, int MCCode, int Status)
        {
            List<GetVoucherList> V_List = new List<GetVoucherList>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER obj = new SQLHELPER(constr);
                string formattedStartDate = string.IsNullOrEmpty(StartDate) ? "" : Busyhelper.FormatDate(StartDate);
                string formattedEndDate = string.IsNullOrEmpty(EndDate) ? "" : Busyhelper.FormatDate(EndDate);
                string sql = string.Empty;
                DataTable DT1 = new DataTable();

                switch (TranType)
                {
                    case 1:
                        sql = $"Select A.[VchCode], IsNull(A.[VchNo], '') as VchNo, Convert(Varchar, A.[Date], 105) as VchDate, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(M.[Name], '') as AccName, IsNull(A.[MasterCode2], 0) as MCCode, IsNull(M1.[Name], '') as MCName, IsNull(A.[Container], '') as Container, IsNull(A.[D1], 0) as TotCart, IsNull(A.[D2], 0) as TotQty, IsNull(A.[D3], 0) as TotPcs, (CASE WHEN A.[Status] = 0 THEN 'Pending' WHEN A.[Status] IN (1, 3) Then 'Completed' WHEN A.[Status] = 2 Then 'Cancelled' ELSE 'Panding' END) as [Status] From ESBTTRAN1 A Left Join Master1 M On A.MasterCode1 = M.Code Left Join Master1 M1 On A.MasterCode2 = M1.Code Where A.[MasterCode2] = {MCCode} And A.VchType = 108 And A.[Status] <> 2 ";
                        if (Status > 0) sql += (Status == 1) ? " And (A.[Status] Is Null OR A.[Status] = 0)" : " And (A.[Status] IN (1, 3))";
                        if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And A.[Date] >= '{formattedStartDate}' And A.[Date] <= '{formattedEndDate}'";

                        break;
                    case 2:

                        sql = $"Select A.[VchCode], IsNull(LTrim(A.[VchNo]), '') as VchNo, Convert(Varchar, A.[Date], 105) as VchDate, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(A.[MasterCode2], 0) as MCCode, IsNull(M1.[Name], '') as AccName, IsNull(M2.[Name], '') as MCName, '' as Container, 0 as TotCart, IsNull((Sum(B.[Value1]) * (-1)), 0) as TotQty, 0 as TotPcs, (CASE WHEN A.[Flag] = 1 THEN 'Pending' WHEN A.[Flag] = 2 Then 'Completed' END) as [Status] From Tran1 A INNER JOIN TRAN2 B ON A.VchCode = B.VchCode Left Join Master1 M1 On A.[MasterCode1] = M1.Code Left Join Master1 M2 On A.[MasterCode2] = M2.Code Where A.VchType = 11 ";
                        sql += Status == 0 ? "And Flag IN (1, 2)" : Status == 1 ? " And A.[Flag] = 1" : " And A.[Flag] = 2 ";
                        sql += "Group By A.[VchCode], A.[VchNo], A.[Date], A.[MasterCode1], A.[MasterCode2], M1.[Name], M2.[Name], A.[Flag]";
                        if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And A.[Date] >= '{formattedStartDate}' And A.[Date] <= '{formattedEndDate}'"; 
                        break;
                }
                sql += " Order By A.[VchCode] DESC, A.[Date] DESC";
                DT1 = obj.getTable(sql);
                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        V_List.Add(new GetVoucherList
                        {
                            VchCode = Convert.ToInt32(item["VchCode"]),
                            VchNo = clsMain.MyString(item["VchNo"]),
                            VchDate = Convert.ToString(item["VchDate"]),
                            AccCode = Convert.ToInt32(item["AccCode"]),
                            AccName = clsMain.MyString(item["AccName"]),
                            MCCode = Convert.ToInt32(item["MCCode"]),
                            MCName = clsMain.MyString(item["MCName"]),
                            Container = clsMain.MyString(item["Container"]),
                            TotCart = Convert.ToDecimal(item["TotCart"]),
                            TotQty = Convert.ToDecimal(item["TotQty"]),
                            TotPcs = Convert.ToDecimal(item["TotPcs"]),
                            Status = Convert.ToString(item["Status"])
                        });
                    }
                }
                else
                {
                    throw new Exception("Data Not Found !!!");
                }
            }
            catch (Exception err)
            {
                return new { Status = 0, Msg = err.Message.ToString(), Data = V_List };
            }
            return new { Status = 1, Msg = "Success", Data = V_List };
        }

        [HttpGet]
        public dynamic GetVoucherItemsList(string CompCode, string FY, int TranType, int VchCode)
        {
            List<GetVoucherItemsList> I_List = new List<GetVoucherItemsList>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER obj = new SQLHELPER(constr); string sql = string.Empty;

                switch (TranType)
                {
                    case 1:
                        sql = $"Select A.SNo, A.[VchCode], IsNull(A.[ItemCode], 0) as ItemCode, IsNull(M.[Name], '') as ItemName, IsNull(A.[Value1], 0) as Carton, IsNull(A.[Value2], 0) as Qty, IsNull(A.[Value3], 0) as Pcs, IsNull((Select COUNT(BCN) as VAL From ESBTBCN B Where A.VchCode = B.VchCode And A.SNo = B.ISrNo And A.ItemCode = B.MasterCode2 And [RecType] = 1 And [Status] = 1 And Len(B.[BCN]) > 0), 0) as ScanQty From ESBTTRAN2 A INNER JOIN Master1 M On A.[ItemCode] = M.[Code] And M.[MasterType] = 6 Where A.[VchCode] = {VchCode} And A.[VchType] = 108 And A.RecType = 1 Order By A.[SNo], M.[Name]";
                        break;
                    case 2:
                        sql = $"Select A.SrNo as SNo, A.[VchCode], IsNull(A.[MasterCode1], 0) as ItemCode, IsNull(M1.[Name], '') as ItemName, 0 as Carton, (IsNull(A.[Value1], 0) * (-1)) as Qty, 0 as Pcs, IsNull((Select COUNT(BCN) as VAL From ESBTBCN B Where A.[VchCode] = B.[VchCode] And A.[SrNo] = B.[ISrNo] And A.[MasterCode1] = B.[MasterCode2] And B.[RecType] = 2 And [Status] = 1 And Len(B.[BCN]) > 0), 0) as ScanQty From TRAN2 A INNER JOIN Master1 M1 On A.[MasterCode1] = M1.[Code] And M1.[MasterType] = 6 Where A.[VchCode] = {VchCode} And A.[VchType] = 11 And A.RecType = 2 Order By A.[SrNo], M1.[Name]";
                        break;
                }
                DataTable DT1 = obj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        var GetBCNListDT = GetBCNDetailsList(CompCode, FY, TranType, Convert.ToInt32(item["VchCode"]), Convert.ToInt32(item["SNo"]), Convert.ToInt32(item["ItemCode"]));
                        I_List.Add(new GetVoucherItemsList
                        {
                            SNo = Convert.ToInt32(item["SNo"]),
                            VchCode = Convert.ToInt32(item["VchCode"]),
                            ItemCode = Convert.ToInt32(item["ItemCode"]),
                            ItemName = clsMain.MyString(item["ItemName"]),
                            Carton = Convert.ToDecimal(item["Carton"]),
                            Qty = Convert.ToDecimal(item["Qty"]),
                            Pcs = Convert.ToDecimal(item["Pcs"]),
                            ScanQty = Convert.ToDecimal(item["ScanQty"]),
                            BCNDetails = GetBCNListDT?.Data ?? new List<GetBCNListDT>() 
                        });
                    }
                }
                else
                {
                    throw new Exception("Data Not Found !!!");
                }
            }
            catch (Exception err)
            {
                return new { Status = 0, Msg = err.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = I_List };
        }

        private dynamic GetBCNDetailsList(string CompCode, string FY, int TranType, int VchCode, int ISNo, int ItemCode)
        {
            List<GetBCNListDT> B_List = new List<GetBCNListDT>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);

                string sql = $"Select VchCode, IsNull(ISrNo, 0) as ISNo, IsNull(MasterCode2, 0) as ItemCode, IsNull(BCNSrNo, 0) as SNo, IsNull(BCN, '') as BCN, IsNull(Value1, 0) as Qty From ESBTBCN Where [VchCode] = {VchCode} And [ISrNo] = {ISNo} And [MasterCode2] = {ItemCode} And [RecType] = {TranType} And [Status] = 1 Order By [ISrNo], [BCNSrNo], [MasterCode2]";
                DataTable DT1 = new SQLHELPER(constr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach(DataRow item in DT1.Rows)
                    {
                        B_List.Add(new GetBCNListDT
                        {
                            VchCode = Convert.ToInt32(item["VchCode"]),
                            ISNo = Convert.ToInt32(item["ISNo"]),
                            ItemCode = Convert.ToInt32(item["ItemCode"]),
                            SNo = Convert.ToInt32(item["SNo"]),
                            BCN = Convert.ToString(item["BCN"]),
                            Qty = Convert.ToDecimal(item["Qty"])
                        });
                    }
                }
                else
                {
                    throw new Exception("BCN Not Found !!!");
                }
            } 
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), Data = B_List };
            }
            return new { Status = 1, Msg = "Success", Data = B_List };
        }

        [HttpGet]
        public dynamic GetBCNDetailsValidate(string CompCode, string FY, int TranType, int VchCode, string BCN)
        {
            GetBCNListDT B_List = new GetBCNListDT(); string sql = string.Empty;
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);

                switch (TranType)
                {
                    case 1:
                        sql = $"Select VchCode, IsNull(ISrNo, 0) as ISNo, IsNull(MasterCode2, 0) as ItemCode, IsNull(BCNSrNo, 0) as SNo, IsNull(BCN, '') as BCN, IsNull(Value1, 0) as Qty From ESBTBCN Where VchCode = {VchCode} And BCN = '{BCN.Replace("'", "''")}' And ([Status] Is Null OR [Status] IN (0, 1)) Order By ISrNo, BCNSrNo, MasterCode2";
                        break;

                    case 2:
                        sql = $"Select VchCode, IsNull(CM1, 0) as AccCode, IsNull(VchItemSN, 0) as ISNo,  IsNull(ItemCode, 0) as ItemCode, IsNull(SrNo, 0) as SNo, IsNull(BCN, '') as BCN, (IsNull(Value1, 0) * (-1)) as Qty From ItemParamDet Where VchCode = {VchCode} And BCN = '{BCN.Replace("'", "''")}' Order By VchItemSN, SrNo, ItemCode";
                        break;
                }
                DataTable DT1 = new SQLHELPER(constr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    B_List.VchCode = Convert.ToInt32(DT1.Rows[0]["VchCode"]);
                    B_List.ISNo = Convert.ToInt32(DT1.Rows[0]["ISNo"]);
                    B_List.ItemCode = Convert.ToInt32(DT1.Rows[0]["ItemCode"]);
                    B_List.SNo = Convert.ToInt32(DT1.Rows[0]["SNo"]);
                    B_List.BCN = Convert.ToString(DT1.Rows[0]["BCN"]);
                    B_List.Qty = Convert.ToDecimal(DT1.Rows[0]["Qty"]);

                    if (TranType == 1)
                    {
                        sql = $"Update ESBTBCN Set [Status] = 1 Where [VchCode] = {VchCode} And [ISrNo] = {Convert.ToInt32(DT1.Rows[0]["ISNo"])} And [MasterCode2] = {Convert.ToInt32(DT1.Rows[0]["ItemCode"])} And [BCN] = '{BCN}' ";
                    }
                    else
                    {
                        sql = $"Delete From ESBTBCN Where [VchCode] = {VchCode} And [ISrNo] = {Convert.ToInt32(DT1.Rows[0]["ISNo"])} And [MasterCode2] = {Convert.ToInt32(DT1.Rows[0]["ItemCode"])} And [BCN] = '{BCN}'" ;
                        new SQLHELPER(constr).ExecuteSQL(sql);

                        sql = $"INSERT INTO ESBTBCN ([VchCode], [RecType], [MasterCode1], [MasterCode2], [ISRNO], [BCNSRNO], [BCN], [AutoBCNNO], [Value1], [Value2], [Status]) Values ({Convert.ToInt32(DT1.Rows[0]["VchCode"])}, 2, {Convert.ToInt32(DT1.Rows[0]["AccCode"])}, {Convert.ToInt32(DT1.Rows[0]["ItemCode"])}, {Convert.ToInt32(DT1.Rows[0]["ISNo"])}, {Convert.ToInt32(DT1.Rows[0]["SNo"])}, '{Convert.ToString(DT1.Rows[0]["BCN"])}', 0,  {Convert.ToDouble(DT1.Rows[0]["Qty"])},  {Convert.ToDouble(DT1.Rows[0]["Qty"])}, 1)";
                    }
                    int DT2 = new SQLHELPER(constr).ExecuteSQL(sql);
                }
                else
                {
                    throw new Exception("BCN Not Found !!!");
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "BCN Valid", Data = B_List };
        }

        [HttpPost]
        public dynamic SaveMaterialInwardBCN(S_Mat_Vch obj, string CompCode, string FY)
        {
            int Status = 0; string StatusStr = string.Empty; 
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                string XML = CreateXML(obj.MItemsDetails);

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@VchCode", SqlDbType.Int) { Value = obj.VchCode },
                    new SqlParameter("@VchType", SqlDbType.Int) { Value = 108 },
                    new SqlParameter("@Status", SqlDbType.Int) { Value = 1 },
                    new SqlParameter("@MItemsDetails", SqlDbType.NVarChar, -1) { Value =  XML},
                };

                DataTable DT1 = new SQLHELPER(constr).getTable("Sp_SaveMaterialInwardBCN", parameters);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    Status = Convert.ToInt32(DT1.Rows[0]["Status"]);
                    StatusStr = Convert.ToString(DT1.Rows[0]["Msg"]);
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = Status, Msg = StatusStr };
        }

        [HttpPost]
        public dynamic SaveMaterialOutwardBCN(S_Mat_Vch obj, string CompCode, string FY)
        {
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                string XML = CreateXML(obj.MItemsDetails);

                string sql = $"Update Tran1 Set [Flag] = 2 Where VchCode = {obj.VchCode} And VchType = 11";
                int DT1 = new SQLHELPER(constr).ExecuteSQL(sql);

                if (DT1 == 1)
                {
                    return new { Status = 1, Msg = "Material Outward BCN saved successfully" };
                }
                else
                {
                    return new { Status = 1, Msg = "Material Outward Data Not Saved Check The Voucher" };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
        }

        [HttpGet]
        public dynamic GetMasterList(string CompCode, string FY, int TranType, int Type, int Code)
        {
            List<UnknowList> UNList = new List<UnknowList>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr); string sql = string.Empty;

                switch (TranType)
                {
                    case 1:
                        sql = $"Select IsNull([Code], 0) as Code, IsNull([Name], '') as Name From ESJSLCountryMaster Where MasterType = {Type} ";
                        if (Code > 0) sql += $"And ParentGrp = {Code} "; sql += "Order By [Name]";

                        break;
                    case 2:
                        sql = $"Select IsNull([Code], 0) as Code, IsNull([Name], '') as Name From ESJSLCUSTOMER Group By Code, [Name] Order By [Name]";
                        break;
                }

                DataTable DT1 = conobj.getTable(sql);
                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        UNList.Add(new UnknowList
                        {
                            Code = Convert.ToInt32(item["Code"]),
                            Name = clsMain.MyString(item["Name"])
                        });
                    }
                }
                else
                {
                    throw new Exception("Data Not Found !!!");
                }

            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = UNList };
        }

        [HttpGet]
        public dynamic GetDashboardVchUpdateList(string CompCode, string FY, int TranType, int MCCode, string StartDate, string EndDate)
        {
            List<VchUpdateList> PORD = new List<VchUpdateList>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr);
                string formattedStartDate = string.IsNullOrEmpty(StartDate) ? "" : Busyhelper.FormatDate(StartDate);
                string formattedEndDate = string.IsNullOrEmpty(EndDate) ? "" : Busyhelper.FormatDate(EndDate); string sql = string.Empty;

                switch (TranType)
                {
                    case 1:
                        //string sql = $"SELECT A.VchCode, A.RefNo, A.VchDate, A.AccCode, A.AccName, A.Mobile, ISNULL(T.TotQty, 0) AS TotQty, (ISNULL(TR.TransQty, 0) * (-1)) AS TransQty, (IsNull(T.TotQty, 0) - (IsNull(TR.TransQty, 0) * (-1))) as BQty, (CASE WHEN IsNull(T.TotQty, 0) - (IsNull(TR.TransQty, 0) * (-1)) = 0 THEN 'Completed' ELSE 'Pending' END) as [Status] FROM " +
                        //        $"(SELECT DISTINCT A.OrderId as VchCode, IsNull(A.[RefNo], '') as RefNo, CONVERT(VARCHAR, B.Date, 105) as VchDate, IsNull(B.MasterCode1, 0) as AccCode, IsNull(B.CustName, '') as AccName, IsNull(B.CMobile, '') as Mobile FROM ESJSLRefTran A Inner Join ESJSLTran1 B On A.OrderId = B.VchCode And A.VchType = B.VchType WHERE A.VchType = 108 AND A.Method = 2 And B.[QStatus] = 1 And B.Cancelled <> 1";
                        //        if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And B.[Date] >= '{formattedStartDate}' And B.[Date] <= '{formattedEndDate}' " ;
                        //        sql += ") A LEFT JOIN (SELECT A.VchCode, A.RefNo, SUM(A.TQty) AS TotQty FROM (SELECT A.VchCode, A.RefNo, A.ItemCode, SUM(A.Qty) AS TQty FROM ESJSLRefTran A INNER JOIN Master1 M ON A.ItemCode = M.Code AND M.MasterType = 6 Where A.Method = 1 AND A.VchType = 108 AND M.CM6 = {MCCode} Group By A.VchCode, A.RefNo, A.ItemCode) A Group By A.VchCode, A.RefNo) T ON A.VchCode = T.VchCode AND A.RefNo = T.RefNo LEFT JOIN (SELECT A.OrderId AS VchCode, A.RefNo, SUM(A.TRQty) AS TransQty FROM (SELECT A.OrderId, A.RefNo, A.ItemCode, SUM(A.Qty) AS TRQty FROM ESJSLRefTran A INNER JOIN  Master1 M ON A.ItemCode = M.Code AND M.MasterType = 6 Where A.Method = 2 AND A.VchType = 108 AND M.CM6 = {MCCode} Group By A.OrderId, A.RefNo, A.ItemCode) A Group By A.OrderId, A.RefNo) TR ON A.VchCode = TR.VchCode AND A.RefNo = TR.RefNo";

                        sql = $"WITH Totals AS (SELECT A.VchCode, A.RefNo, ISNULL(T.TotQty, 0) AS TotQty FROM (SELECT DISTINCT A.RefCode AS VchCode, ISNULL(A.[RefNo], '') AS RefNo FROM ESJSLRefTran A INNER JOIN ESJSLTran1 B ON A.RefCode = B.VchCode WHERE A.RecType = 1 And A.Method IN (1,2) AND B.[QStatus] = 1 AND B.Cancelled <> 1";
                        if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And B.[Date] >= '{formattedStartDate}' And B.[Date] <= '{formattedEndDate}' ";
                        sql += $") A INNER JOIN (SELECT A.VchCode, A.RefNo, SUM(A.TQty) AS TotQty FROM (SELECT A.RefCode as VchCode, A.RefNo, A.MasterCode2, SUM(A.Value1) AS TQty FROM ESJSLRefTran A INNER JOIN Master1 M ON A.MasterCode2 = M.Code AND M.MasterType = 6 WHERE A.RecType = 1 And A.Method = 1 AND M.CM6 = {MCCode} GROUP BY A.RefCode, A.RefNo, A.MasterCode2) A GROUP BY A.VchCode, A.RefNo) T ON A.VchCode = T.VchCode AND A.RefNo = T.RefNo), " +
                        $"Transfers AS(SELECT A.VchCode, A.RefNo, ISNULL(TR.TransQty, 0) AS TransQty FROM (SELECT DISTINCT A.RefCode AS VchCode, ISNULL(A.[RefNo], '') AS RefNo FROM ESJSLRefTran A INNER JOIN ESJSLTran1 B ON A.RefCode = B.VchCode WHERE A.RecType = 1 AND A.Method IN (1, 2) AND B.[QStatus] = 1 AND B.Cancelled <> 1 ";
                        if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And B.[Date] >= '{formattedStartDate}' And B.[Date] <= '{formattedEndDate}' ";
                        sql += $") A INNER JOIN (SELECT A.VchCode, A.RefNo, SUM(A.TRQty) AS TransQty FROM (SELECT A.RefCode as VchCode, A.RefNo, A.MasterCode2, SUM(A.Value1) AS TRQty FROM ESJSLRefTran A INNER JOIN Master1 M ON A.MasterCode2 = M.Code AND M.MasterType = 6 WHERE A.RecType = 1 And A.Method = 2 AND M.CM6 = {MCCode} GROUP BY A.RefCode, A.RefNo, A.MasterCode2) A GROUP BY A.VchCode, A.RefNo) TR ON A.VchCode = TR.VchCode AND A.RefNo = TR.RefNo) " +
                        $"SELECT T.VchCode, CONVERT(VARCHAR, T1.Date, 105) as VchDate, IsNull(T.RefNo, '') as RefNo, IsNull(T1.MasterCode1, 0) as AccCode, ISNULL(T2.Name, '') as AccName, ISNULL(T2.Mobile, '') as Mobile, IsNull(T2.[Email], '') as Email, IsNull(T2.[GSTIN], '') as GSTIN, IsNull(T2.[Address], '') as Address, ISNULL(T.TotQty, 0) as TotQty, (ISNULL(Tr.TransQty, 0) * (-1)) AS TransQty, (ISNULL(T.TotQty, 0) - (ISNULL(Tr.TransQty, 0) * (-1))) AS BalQty, CASE WHEN T.TotQty = (ISNULL(Tr.TransQty, 0) * (-1)) THEN 'Completed' ELSE 'Pending' END AS VoucherStatus FROM Totals T LEFT JOIN Transfers Tr ON T.VchCode = Tr.VchCode AND T.RefNo = Tr.RefNo LEFT JOIN ESJSLTRAN1 T1 ON T.VchCode = T1.VchCode LEFT JOIN ESJSLCustomer T2 ON T1.MasterCode1 = T2.Code ORDER BY T.VchCode";

                        break;
                    case 2:
                        //Verification Pending And Completed List 
                        sql = $"Select VchCode, IsNull(A.[VchNo], '') as RefNo, IsNull(A.[Date], '') as [VchDate], IsNull(A.[MasterCode1], 0) as AccCode, IsNull(M1.[Name], '') as AccName, IsNull(M1.[Mobile], '') as Mobile,  IsNull(M1.[Email], '') as Email, IsNull(M1.[GSTIN], '') as GSTIN, IsNull(M1.[Address], '') as Address, IsNull(M1.[Email], '') as Mobile, IsNull(A.[TotQty], 0) as TotQty, 0 as TransQty, 0 as BalQty, IsNull(A.[Verification], 0) as [Status], (CASE WHEN A.[Verification] = 1 THEN 'Verified' ELSE 'Pending For Verification' END) as VoucherStatus From ESJSLTRAN1 A INNER JOIN ESJSLCustomer M1 On A.MasterCode1 = M1.Code Where VchType = 109 And Cancelled <> 1";
                        if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And A.[Date] >= '{formattedStartDate}' And A.[Date] <= '{formattedEndDate}' ";

                        break;
                    case 3:
                        //sql = $"WITH Totals AS (SELECT A.VchCode, A.RefNo, ISNULL(T.TotQty, 0) AS TotQty FROM (SELECT DISTINCT A.OrderId1 AS VchCode, ISNULL(A.[RefNo], '') AS RefNo FROM ESJSLRefTran A INNER JOIN ESJSLTran1 B ON A.OrderId1 = B.VchCode AND A.VchType = B.VchType WHERE A.VchType = 109 AND A.Method IN (1,2) AND B.[Verification] = 1 AND B.Cancelled <> 1";
                        //if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And B.[Date] >= '{formattedStartDate}' And B.[Date] <= '{formattedEndDate}' ";
                        //sql += $") A INNER JOIN (SELECT A.VchCode, A.RefNo, SUM(A.TQty) AS TotQty FROM (SELECT A.VchCode, A.RefNo, A.ItemCode, SUM(A.Qty) AS TQty FROM ESJSLRefTran A INNER JOIN Master1 M ON A.ItemCode = M.Code AND M.MasterType = 6 WHERE A.Method = 1 AND A.VchType = 109 GROUP BY A.VchCode, A.RefNo, A.ItemCode) A GROUP BY A.VchCode, A.RefNo) T ON A.VchCode = T.VchCode AND A.RefNo = T.RefNo), " +
                        //$"Transfers AS(SELECT A.VchCode, A.RefNo, ISNULL(TR.TransQty, 0) AS TransQty FROM (SELECT DISTINCT A.OrderId1 AS VchCode, ISNULL(A.[RefNo], '') AS RefNo FROM ESJSLRefTran A INNER JOIN ESJSLTran1 B ON A.OrderId1 = B.VchCode AND A.VchType = B.VchType WHERE A.VchType = 109 AND A.Method IN (1, 2) AND B.[Verification] = 1 AND B.Cancelled <> 1 ";
                        //if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And B.[Date] >= '{formattedStartDate}' And B.[Date] <= '{formattedEndDate}' ";
                        //sql += $") A INNER JOIN (SELECT A.VchCode, A.RefNo, SUM(A.TRQty) AS TransQty FROM (SELECT A.OrderId1 as VchCode, A.RefNo, A.ItemCode, SUM(A.Qty) AS TRQty FROM ESJSLRefTran A INNER JOIN Master1 M ON A.ItemCode = M.Code AND M.MasterType = 6 WHERE A.Method = 2 AND A.VchType = 109 GROUP BY A.OrderId1, A.RefNo, A.ItemCode) A GROUP BY A.VchCode, A.RefNo) TR ON A.VchCode = TR.VchCode AND A.RefNo = TR.RefNo) " +
                        //$"SELECT T.VchCode, CONVERT(VARCHAR, T1.Date, 105) as VchDate, IsNull(T.RefNo, '') as RefNo, IsNull(T1.MasterCode1, 0) as AccCode, ISNULL(T2.Name, '') as AccName, ISNULL(T2.Mobile, '') as Mobile, IsNull(T2.[Email], '') as Email, IsNull(T2.[GSTIN], '') as GSTIN, IsNull(T2.[Address], '') as Address, ISNULL(T.TotQty, 0) as TotQty, (ISNULL(Tr.TransQty, 0) * (-1)) AS TransQty, (ISNULL(T.TotQty, 0) - (ISNULL(Tr.TransQty, 0) * (-1))) AS BalQty, CASE WHEN T.TotQty = (ISNULL(Tr.TransQty, 0) * (-1)) THEN 'Completed' ELSE 'Pending' END AS VoucherStatus FROM Totals T LEFT JOIN Transfers Tr ON T.VchCode = Tr.VchCode AND T.RefNo = Tr.RefNo LEFT JOIN ESJSLTRAN1 T1 ON T.VchCode = T1.VchCode LEFT JOIN ESJSLCustomer T2 ON T1.MasterCode1 = T2.Code ORDER BY T.VchCode";

                        // Packing Pendign And Complated List
                        sql = $"WITH Totals AS (SELECT A.VchCode, A.RefNo, ISNULL(T.TotQty, 0) AS TotQty FROM (SELECT DISTINCT ISNULL(A.RefCode, 0) AS VchCode, ISNULL(A.[RefNo], '') AS RefNo FROM ESJSLRefTran A INNER JOIN ESJSLTran1 B ON A.RefCode = B.OrderId AND B.VchType = 109 WHERE A.RecType = 2 And A.Method IN (1,2) AND B.[Verification] = 1 AND B.Cancelled <> 1";
                        if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And B.[Date] >= '{formattedStartDate}' And B.[Date] <= '{formattedEndDate}' ";
                        sql += $") A INNER JOIN (SELECT A.VchCode, A.RefNo, SUM(A.TQty) AS TotQty FROM (SELECT A.RefCode as VchCode, A.RefNo, A.MasterCode2, SUM(A.Value1) AS TQty FROM ESJSLRefTran A INNER JOIN Master1 M ON A.MasterCode2 = M.Code AND M.MasterType = 6 WHERE A.RecType = 2 And A.Method = 1 GROUP BY A.RefCode, A.RefNo, A.MasterCode2) A GROUP BY A.VchCode, A.RefNo) T ON A.VchCode = T.VchCode AND A.RefNo = T.RefNo), " +
                        $"Transfers AS(SELECT A.VchCode, A.RefNo, ISNULL(TR.TransQty, 0) AS TransQty FROM (SELECT DISTINCT A.RefCode AS VchCode, ISNULL(A.[RefNo], '') AS RefNo FROM ESJSLRefTran A INNER JOIN ESJSLTran1 B ON A.RefCode = B.OrderId And A.RefNo = B.OrderNo And B.VchType = 109 WHERE A.RecType = 2 AND A.Method IN (1, 2) AND B.[Verification] = 1 AND B.Cancelled <> 1 ";
                        if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And B.[Date] >= '{formattedStartDate}' And B.[Date] <= '{formattedEndDate}' ";
                        sql += $") A INNER JOIN (SELECT A.VchCode, A.RefNo, SUM(A.TRQty) AS TransQty FROM (SELECT A.RefCode as VchCode, A.RefNo, A.MasterCode2, SUM(A.Value1) AS TRQty FROM ESJSLRefTran A INNER JOIN Master1 M ON A.MasterCode2 = M.Code AND M.MasterType = 6 WHERE A.RecType = 2 And A.Method = 2 GROUP BY A.RefCode, A.RefNo, A.MasterCode2) A GROUP BY A.VchCode, A.RefNo) TR ON A.VchCode = TR.VchCode AND A.RefNo = TR.RefNo) " +
                        $"SELECT T.VchCode, CONVERT(VARCHAR, T1.Date, 105) as VchDate, IsNull(T.RefNo, '') as RefNo, IsNull(T1.MasterCode1, 0) as AccCode, ISNULL(T2.Name, '') as AccName, ISNULL(T2.Mobile, '') as Mobile, IsNull(T2.[Email], '') as Email, IsNull(T2.[GSTIN], '') as GSTIN, IsNull(T2.[Address], '') as Address, ISNULL(T.TotQty, 0) as TotQty, (ISNULL(Tr.TransQty, 0) * (-1)) AS TransQty, (ISNULL(T.TotQty, 0) - (ISNULL(Tr.TransQty, 0) * (-1))) AS BalQty, CASE WHEN T.TotQty = (ISNULL(Tr.TransQty, 0) * (-1)) THEN 'Completed' ELSE 'Pending' END AS VoucherStatus FROM Totals T LEFT JOIN Transfers Tr ON T.VchCode = Tr.VchCode AND T.RefNo = Tr.RefNo LEFT JOIN ESJSLTRAN1 T1 ON T.VchCode = T1.VchCode LEFT JOIN ESJSLCustomer T2 ON T1.MasterCode1 = T2.Code ORDER BY T.VchCode";

                        break;
                }

                DataTable DT1 = conobj.getTable(sql);
                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        PORD.Add(new VchUpdateList
                        {
                            VchCode = Convert.ToInt32(item["VchCode"]),
                            VchNo = clsMain.MyString(item["RefNo"]),
                            Date = clsMain.MyString(item["VchDate"]),
                            AccCode = Convert.ToInt32(item["AccCode"]),
                            AccName = clsMain.MyString(item["AccName"]),
                            Mobile = clsMain.MyString(item["Mobile"]),
                            Email = clsMain.MyString(item["Email"]),
                            GSTIN = clsMain.MyString(item["GSTIN"]),
                            Address = clsMain.MyString(item["Address"]),
                            TotQty = Convert.ToDecimal(item["TotQty"]),
                            TransQty = Convert.ToDecimal(item["TransQty"]),
                            BQty = Convert.ToDecimal(item["BalQty"]),
                            Status = clsMain.MyString(item["VoucherStatus"])
                        });
                    }
                }
                else
                {
                    throw new Exception("Data Not Found !!!");
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = PORD };
        }

        [HttpGet]
        public dynamic GetCategory(string CompCode, string FY)
        {
            List<Category> SPT = new List<Category>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                string sql = ""; //string img = "https://random.imagecdn.app/500/500";

                sql = $"Select Distinct T.I1 as [Code], M1.[Name] From ESAttributesMappingConfig T left Join Master1 M1 On T.I1 = M1.Code Where T.MasterType = 1 And M1.ParentGrp = 0 Order by M1.Name";
                DataTable DT1 = Provider == 1 ? new OLEDBHELPER(constr).getTable(sql) : new SQLHELPER(constr).getTable(sql);
                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        Category TM = new Category();
                        TM.Code = clsMain.MyInt(item["Code"]);
                        TM.Name = clsMain.MyString(item["Name"]);
                        SPT.Add(TM);
                    }
                }
            }
            catch (Exception err)
            {
                return new { Status = 0, Msg = err.Message.ToString(), Data = SPT };
            }
            return new { Status = 1, Msg = "Success", Data = SPT };
        }

        [HttpGet]
        public dynamic GetProducts(int GrpCode, string CompCode, string FY)
        {
            List<GetProduct> PList = new List<GetProduct>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY); //string img = "https://random.imagecdn.app/500/500";
                SQLHELPER obj = new SQLHELPER(constr);
                string SubGrpStr = GrpCode > 0 ? GetAllItemSubGroups(GrpCode, constr) : "";

                string sql = $"Select M1.ParentGrp as Code,M2.Name FROM MASTER1 M1 Inner Join Master1 M2 On M1.ParentGrp = M2.Code WHERE M1.MASTERTYPE = 6";
                if (SubGrpStr.Length > 0) sql += $" AND M1.ParentGrp In (" + SubGrpStr + ")";
                sql += $" Group By M1.ParentGrp,M2.Name Order By M2.Name";
                DataTable DT1 = obj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow grp in DT1.Rows)
                    {
                        int rootCode = Convert.ToInt32(grp["Code"]);
                        string rootName = clsMain.MyString(grp["Name"]);
                        object price = GetProductMinAndMaxPrice(constr, rootCode);
                        JObject priceObject = JObject.FromObject(price);

                        // Find or create the root data object
                        GetProduct rootData = PList.FirstOrDefault(rd => rd.Code == rootCode);
                        if (rootData == null)
                        {
                            rootData = new GetProduct
                            {
                                Code = rootCode,
                                Name = rootName,
                                MinPrice = priceObject["MinPrice"].ToObject<decimal>(),
                                MaxPrice = priceObject["MaxPrice"].ToObject<decimal>(),
                                //DataItem = new List<DataItem>()
                                Mendatories = new List<Mendatory>(),
                                NonMendatories = new List<NonMendatory>()
                            };
                            PList.Add(rootData);
                        }

                        SQLHELPER obj1 = new SQLHELPER(constr); string sql1 = string.Empty;
                        //string IGrpStr = GrpCode > 0 ? GrpCode.ToString() + "," + Convert.ToInt32(grp["Code"]).ToString() : Convert.ToInt32(grp["Code"]).ToString(); SubGrpStr = GetAllItemSubGroups(Convert.ToInt32(grp["Code"]), constr); //SubGrpStr.ToString() + "," +
                        string IgrpStr = GetAllParentGroups(Convert.ToInt32(grp["Code"]), constr);

                        sql1 = $"Select A.[SrNo],A.[I2] as ATCode,M1.[Name] as Attribute,A.[I3] as SATCode,M2.[Name] as AttributeVal,A.I4 as AMendatory From (Select B.SrNo,B.I2,E.I3,B.I4 From (Select Top 1 I1 as MIgrpCode," + Convert.ToInt32(grp["Code"]).ToString() + " as IgrpCode from ESAttributesMappingConfig Where I1 in (" + IgrpStr + ") Group By I1) A Inner Join ESAttributesMappingConfig B On A.MIgrpCode = B.I1 Inner Join Master1 M1 On A.IgrpCode = M1.ParentGrp Inner Join ESAttributesMappingConfig E On M1.Code = E.I1 And E.MasterType = 101 And B.[I2] = E.[I2] Where B.[I4] = 1 Group By A.MIgrpCode,B.SrNo,B.I2,E.I3,B.I4 Union All Select B.SrNo,B.I2,M1.Code as I3,B.I4 From (Select Top 1 I1 as MIgrpCode," + Convert.ToInt32(grp["Code"]).ToString() + " as IgrpCode from ESAttributesMappingConfig Where I1 in (" + IgrpStr + ") Group By I1) A Inner Join ESAttributesMappingConfig B On A.MIgrpCode = B.I1 Inner Join ESMaster1 M1 On B.I2 = M1.I1 Where B.I4 = 0 Group By B.SrNo,B.I2,M1.Code,B.I4) A Inner Join ESMaster1 M1 On A.I2 = M1.Code Inner Join ESMaster1 M2 On A.I3 = M2.Code Order By A.SrNo";
                        DataTable DT2 = obj1.getTable(sql1);
                        if (DT2 != null && DT2.Rows.Count > 0)
                        {
                            foreach (DataRow row in DT2.Rows)
                            {
                                string dataItemName = row["Attribute"].ToString();
                                int dataItemCode = Convert.ToInt32(row["ATCode"]);
                                int subDataCode = Convert.ToInt32(row["SATCode"]);
                                string subDataName = row["AttributeVal"].ToString();
                                int AMendatory = clsMain.MyInt(row["AMendatory"]);

                                //// Find or create the data item object
                                //DataItem dataItem = rootData.DataItem.FirstOrDefault(di => di.Code == dataItemCode);
                                //if (dataItem == null)
                                //{
                                //    dataItem = new DataItem
                                //    {
                                //        Code = dataItemCode,
                                //        Name = dataItemName,
                                //        Mendatory = AMendatory,
                                //        SubData = new List<SubData>()
                                //    };
                                //    rootData.DataItem.Add(dataItem);
                                //}

                                if (AMendatory == 1)
                                {
                                    Mendatory mendatory = rootData.Mendatories.FirstOrDefault(di => di.Code == dataItemCode);
                                    if (mendatory == null)
                                    {
                                        mendatory = new Mendatory
                                        {
                                            Code = dataItemCode,
                                            Name = dataItemName,
                                            SubData = new List<SubData>()
                                        };
                                        rootData.Mendatories.Add(mendatory);
                                    }

                                    // Add the sub data object
                                    if (!mendatory.SubData.Any(sd => sd.Code == subDataCode))
                                    {
                                        mendatory.SubData.Add(new SubData
                                        {
                                            Code = subDataCode,
                                            Name = subDataName
                                        });
                                    }
                                }
                                else
                                {
                                    NonMendatory nonMendatory = rootData.NonMendatories.FirstOrDefault(di => di.Code == dataItemCode);
                                    if (nonMendatory == null)
                                    {
                                        nonMendatory = new NonMendatory
                                        {
                                            Code = dataItemCode,
                                            Name = dataItemName,
                                            SubData = new List<SubData>()
                                        };
                                        rootData.NonMendatories.Add(nonMendatory);
                                    }

                                    // Add the sub data object
                                    if (!nonMendatory.SubData.Any(sd => sd.Code == subDataCode))
                                    {
                                        nonMendatory.SubData.Add(new SubData
                                        {
                                            Code = subDataCode,
                                            Name = subDataName
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception err)
            {
                return new { Status = 0, Msg = err.Message.ToString(), Data = PList };
            }
            return new { Status = 1, Msg = "Success", Data = PList };
        }

        [HttpGet]
        public dynamic GetSearchAllProducts(string CompCode, string FY)
        {
            List<GetSerchProducts> SPLIST = new List<GetSerchProducts>();
            try
            {
                string sql = string.Empty;
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER obj = new SQLHELPER(constr);

                sql = "Select M1.ParentGrp as Code,M2.Name FROM MASTER1 M1 Inner Join Master1 M2 On M1.ParentGrp = M2.Code WHERE M1.MASTERTYPE = 6 Group By M1.ParentGrp,M2.Name Order By M2.Name";
                DataTable DT1 = obj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow row in DT1.Rows)
                    {
                        //GetSerchProducts SP = new GetSerchProducts();
                        SPLIST.Add(new GetSerchProducts
                        {
                            Code = clsMain.MyInt(row["Code"]),
                            Name = clsMain.MyString(row["Name"])
                        });
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found !!!", Data = SPLIST };
                }
            }
            catch (Exception ERR)
            {
                return new { Status = 0, Msg = ERR.Message.ToString(), Data = SPLIST };
            }
            return new { Status = 1, Msg = "Success", Data = SPLIST };
        }

        [HttpGet]
        public dynamic GetProductsFilterWise(string CompCode, string FY, int IGrpCode)
        {
            List<GetProductsFiltWise> PList = new List<GetProductsFiltWise>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER obj = new SQLHELPER(constr);
                string IgrpStr = GetAllParentGroups(IGrpCode, constr);
                object price = GetProductMinAndMaxPrice(constr, IGrpCode);
                JObject priceObject = JObject.FromObject(price);
                string igrpname = GetBusyMasterCode2NameIfExist(constr, IGrpCode, 5);
                var sizeChartData = GetProductSubAttribute(constr, IGrpCode);
                var sizeCharts = sizeChartData?.Data as List<SizeChart> ?? new List<SizeChart>();

                string sql = $"Select A.[SrNo],A.[I2] as ATCode,M1.[Name] as Attribute,A.[I3] as SATCode,M2.[Name] as AttributeVal,A.I4 as AMendatory From (Select B.SrNo,B.I2,E.I3,B.I4 From (Select Top 1 I1 as MIgrpCode," + IGrpCode + " as IgrpCode from ESAttributesMappingConfig Where I1 in (" + IgrpStr + ") Group By I1) A Inner Join ESAttributesMappingConfig B On A.MIgrpCode = B.I1 Inner Join Master1 M1 On A.IgrpCode = M1.ParentGrp Inner Join ESAttributesMappingConfig E On M1.Code = E.I1 And E.MasterType = 101 And B.[I2] = E.[I2] Where B.[I4] = 1 Group By A.MIgrpCode,B.SrNo,B.I2,E.I3,B.I4 Union All Select B.SrNo,B.I2,M1.Code as I3,B.I4 From (Select Top 1 I1 as MIgrpCode," + IGrpCode + " as IgrpCode from ESAttributesMappingConfig Where I1 in (" + IgrpStr + ") Group By I1) A Inner Join ESAttributesMappingConfig B On A.MIgrpCode = B.I1 Inner Join ESMaster1 M1 On B.I2 = M1.I1 Where B.I4 = 0 Group By B.SrNo,B.I2,M1.Code,B.I4) A Inner Join ESMaster1 M1 On A.I2 = M1.Code Inner Join ESMaster1 M2 On A.I3 = M2.Code Order By A.SrNo";
                //string sql = $"Select A.[SrNo],A.[I2] as ATCode,M1.[Name] as Attribute,A.[I3] as SATCode,M2.[Name] as AttributeVal,A.I4 as AMendatory From (Select B.SrNo,B.I2,E.I3,B.I4 From (Select Top 1 I1 as IgrpCode from ESAttributesMappingConfig Where I1 = {IGrpCode} Group By I1) A Inner Join ESAttributesMappingConfig B On A.IgrpCode = B.I1 Inner Join Master1 M1 On A.IgrpCode = M1.ParentGrp Inner Join ESAttributesMappingConfig E On M1.Code = E.I1 And E.MasterType = 101 And B.[I2] = E.[I2] Where B.[I4] = 1 Group By A.IgrpCode,B.SrNo,B.I2,E.I3,B.I4 Union All Select B.SrNo,B.I2,M1.Code as I3,B.I4 From (Select Top 1 I1 as IgrpCode from ESAttributesMappingConfig Where I1 = {IGrpCode} Group By I1) A Inner Join ESAttributesMappingConfig B On A.IgrpCode = B.I1 Inner Join ESMaster1 M1 On B.I2 = M1.I1 Where B.I4 = 0 Group By B.SrNo,B.I2,M1.Code,B.I4) A Inner Join ESMaster1 M1 On A.I2 = M1.Code Inner Join ESMaster1 M2 On A.I3 = M2.Code Order By A.SrNo";
                DataTable DT2 = obj.getTable(sql);
                if (DT2 != null && DT2.Rows.Count > 0)
                {
                    foreach (DataRow row in DT2.Rows)
                    {
                        int rootCode = Convert.ToInt32(IGrpCode);
                        string dataItemName = row["Attribute"].ToString();
                        int dataItemCode = Convert.ToInt32(row["ATCode"]);
                        int subDataCode = Convert.ToInt32(row["SATCode"]);
                        string subDataName = row["AttributeVal"].ToString();
                        int AMendatory = clsMain.MyInt(row["AMendatory"]);

                        // Find or create the root data object
                        GetProductsFiltWise rootData = PList.FirstOrDefault(rd => rd.Code == rootCode);
                        if (rootData == null)
                        {
                            rootData = new GetProductsFiltWise
                            {
                                Code = rootCode,
                                Name = igrpname,
                                MinPrice = priceObject["MinPrice"].ToObject<decimal>(),
                                MaxPrice = priceObject["MaxPrice"].ToObject<decimal>(),
                                SizeCharts = sizeCharts,
                                //DataItem = new List<DataItem>()
                                Mendatories = new List<Mendatory>(),
                                NonMendatories = new List<NonMendatory>()
                            };
                            PList.Add(rootData);
                        }

                        // Find or create the data item object
                        //DataItem dataItem = rootData.DataItem.FirstOrDefault(di => di.Code == dataItemCode);
                        //if (dataItem == null)
                        //{
                        //    dataItem = new DataItem
                        //    {
                        //        Code = dataItemCode,
                        //        Name = dataItemName,
                        //        Mendatory = AMendatory,
                        //        SubData = new List<SubData>()
                        //    };
                        //    rootData.DataItem.Add(dataItem);
                        //}

                        if (AMendatory == 1)
                        {
                            // Find or create the data item object
                            Mendatory mendatories = rootData.Mendatories.FirstOrDefault(di => di.Code == dataItemCode);
                            if (mendatories == null)
                            {
                                mendatories = new Mendatory
                                {
                                    Code = dataItemCode,
                                    Name = dataItemName,
                                    SubData = new List<SubData>()
                                };
                                rootData.Mendatories.Add(mendatories);
                            }

                            // Add the sub data object
                            if (!mendatories.SubData.Any(sd => sd.Code == subDataCode))
                            {
                                mendatories.SubData.Add(new SubData
                                {
                                    Code = subDataCode,
                                    Name = subDataName
                                });
                            }
                        }
                        else
                        {
                            // Find or create the data item object
                            NonMendatory nonMendatory = rootData.NonMendatories.FirstOrDefault(di => di.Code == dataItemCode);
                            if (nonMendatory == null)
                            {
                                nonMendatory = new NonMendatory
                                {
                                    Code = dataItemCode,
                                    Name = dataItemName,
                                    SubData = new List<SubData>()
                                };
                                rootData.NonMendatories.Add(nonMendatory);
                            }

                            // Add the sub data object
                            if (!nonMendatory.SubData.Any(sd => sd.Code == subDataCode))
                            {
                                nonMendatory.SubData.Add(new SubData
                                {
                                    Code = subDataCode,
                                    Name = subDataName
                                });
                            }
                        }
                    }
                }
                else
                {
                    PList.Add(new GetProductsFiltWise
                    {
                        Code = IGrpCode,
                        Name = igrpname,
                        MinPrice = priceObject["MinPrice"].ToObject<decimal>(),
                        MaxPrice = priceObject["MaxPrice"].ToObject<decimal>(),
                        SizeCharts = sizeCharts,
                        //DataItem = new List<DataItem>()
                        Mendatories = new List<Mendatory>(),
                        NonMendatories = new List<NonMendatory>()
                    });
                }

            }
            catch (Exception err)
            {
                return new { Status = 0, Msg = err.Message.ToString(), Data = PList };
            }
            return new { Status = 1, Msg = "Success", Data = PList };
        }

        [HttpGet]
        public dynamic GetProductSubAttribute(string constr, int IGrpCode)
        {
            var lsize = new List<SizeChart>();
            try
            {
                string sql = $"select IsNull(A.[I2], 0) as UCode, IsNull(B.[Name], '') as UName, IsNull(A.[M1], '') as Size, IsNull(A.[M2], '') as Box From ESAttributesMappingConfig A Inner Join ESMaster1 B On A.I3 = B.Code Where A.I1 = {IGrpCode} And A.MasterType = 2 Group By A.[I2], B.[Name], A.[M1], A.[M2] Order By B.[Name]";
                DataTable DT1 = new SQLHELPER(constr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        lsize.Add(new SizeChart
                        {
                            WCode = Convert.ToInt32(item["UCode"]),
                            WName = Convert.ToString(item["UName"]).Trim(),
                            Size = Convert.ToString(item["Size"]).Trim(),
                            Box = Convert.ToString(item["Box"]).Trim()
                        });
                    }
                }
            }
            catch
            {

            }
            return new { Data = lsize };
        }

        [HttpPost]
        public dynamic GetProductItemsStock(dynamic filters)
        {
            List<GetItemsDTStock> productList = new List<GetItemsDTStock>();
            try
            {
                JObject filterObject = JObject.FromObject(filters);
                var CompCode = filterObject["CompCode"]?.ToObject<string>();
                var FY = filterObject["FY"]?.ToObject<string>();
                var AccCode = filterObject["AccCode"]?.ToObject<int>();
                string formattedDate = DateTime.Now.ToString("dd-MMM-yyyy");
                //string img = "https://random.imagecdn.app/500/500";
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER obj = new SQLHELPER(constr); int R = 0;
                string OptFld = GetConfigeOptionalField(constr, 1);

                // Get GrpCode from filters
                var grpCode = filterObject["GrpCode"]?.ToObject<int>();
                if (grpCode == null || grpCode == 0) { throw new Exception("GrpCode is required in filters."); }

                // Get HCode and ATCode from filters
                var items = filterObject["Items"]?.ToObject<List<JObject>>();
                if (items == null || items.Count == 0) { throw new Exception("Items are required in filters."); }

                var hCodes = new List<int>();
                var atCodes = new List<int>();

                foreach (var item in items)
                {
                    var hCode = item["HCode"]?.ToObject<int>() ?? 0;
                    var atCode = item["ATCode"]?.ToObject<int>() ?? 0;
                    var mendatory = item["Mendatory"]?.ToObject<int>() ?? 0;

#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                    if (hCode != null && atCode != null && mendatory != 0)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                    {
                        hCodes.Add(Convert.ToInt32(hCode));
                        atCodes.Add(Convert.ToInt32(atCode));
                    }
                }

                if (hCodes.Count == 0 || atCodes.Count == 0) { throw new Exception("Attribute And Subattribute Like 'HCode And ATCode are required' in Items."); }
                string optFldCase = string.IsNullOrEmpty(OptFld) ? "''" : $"CASE WHEN LEN({OptFld}) > 0 THEN M1.{OptFld} ELSE '' END";

                // Build the SQL query
                string sql = $"Select Top 1 A.MasterCode as ItemCode, M.[Name] as ItemName, IsNull(M3.[Name], '') as GSTSlabs, IsNull(M4.[D2], 0) as GSTRate, M.[CM1] as UCode, M2.[Name] as UName, M.[D2] as Price, {optFldCase} as Brand, Sum(A.Qty) as Stock From (Select A.MasterCode, A.D1 as Qty From Folio1 A Where A.MasterType = 6 Union All Select MasterCode1, IsNull(Sum(Value1),0) as Qty From Tran2 A inner Join Master1 B On A.MasterCode1 = B.Code And A.MasterCode2 = B.CM6 Where A.Date <= '{formattedDate}' And A.RecType = 2 Group By MasterCode1) A inner Join Master1 M On A.MasterCode = M.Code Left Join MasterAddressInfo M1 On A.MasterCode = M1.MasterCode LEFT JOIN Master1 M2 ON M.[CM1] = M2.Code LEFT JOIN Master1 M3 ON M.[CM8] = M3.[Code] LEFT JOIN MasterSupport M4 on M.[CM8] = M4.[MasterCode] And M4.[I3] = 1 ";
                for (int i = 0; i < atCodes.Count; i++) { R++; sql += $" Inner Join ESAttributesMappingConfig E{R} On M.Code = E{R}.I1 AND E{R}.MasterType = 101"; }
                sql += $" Where M.[ParentGrp] = {grpCode}"; R = 0;
                for (int i = 0; i < atCodes.Count; i++) { R++; sql += $" And E{R}.I3 = {atCodes[i]}"; }                                     //sql += $"AND E.I2 IN ({string.Join(",", hCodes)}) AND E.I3 IN ({string.Join(",", atCodes)}) " +
                sql += string.IsNullOrEmpty(OptFld) ? $" Group By A.[MasterCode], M.[Name], M3.[Name], M4.[D2], M.[CM1], M2.[Name], M.[D2] Order By M.[Name]" : $" Group By A.[MasterCode], M3.[Name], M4.[D2], M.[Name], M.[CM1],M2.[Name], M.[D2], {OptFld} Order By M.[Name]";

                DataTable DT1 = obj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow row in DT1.Rows)
                    {
                        var priceDisc = new ItemDiscount(); priceDisc = GetItemCategoryWisePriceAndDiscount(constr, Convert.ToInt32(AccCode), Convert.ToInt32(row["ItemCode"]));

                        productList.Add(new GetItemsDTStock
                        {
                            ItemName = row["ItemName"].ToString(),
                            ItemCode = Convert.ToInt32(row["ItemCode"]),
                            GSTSlabs = clsMain.MyString(row["GSTSlabs"]),
                            GSTRate = Convert.ToInt32(row["GSTRate"]),
                            UName = row["UName"].ToString(),
                            UCode = Convert.ToInt32(row["UCode"]),
                            Stock = Convert.ToDecimal(row["Stock"]),
                            Qty = Convert.ToDecimal(1),
                            Brand = Convert.ToString(row["Brand"]),
                            ItemDiscounts = priceDisc
                        });
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Item Not Found !!!", Data = productList };
                }
            }
            catch (Exception err)
            {
                return new { Status = 0, Msg = err.Message.ToString(), Data = productList };
            }
            return new { Status = 1, Msg = "Success", Data = productList };
        }

        private ItemDiscount GetItemCategoryWisePriceAndDiscount(string Constr, int AccCode, int ItemCode)
        {
            var distPrice = new ItemDiscount();
            try
            {
                string sql = string.Empty;
                if (AccCode == 0)
                {
                    sql = $"Select ISNULL(M1.[D2], 0) as MRP, (CASE WHEN IsNull(M2.[D2], 0) = 0 THEN M1.[D2] ELSE IsNull((M1.[D2] - (((M1.[D2]) * (M2.[D2])) / 100)), 0) END) as Price, ISNULL(M2.[D2], 0) as Discount From Master1 M1 LEFT JOIN MasterSupport M2 ON M1.Code = M2.MasterCode And M2.[I1] = 103 Where M1.MasterType = 6 And M1.Code = {ItemCode} Group By M1.D2, M2.D2";
                }
                else
                {
                    sql = $"SELECT ISNULL(M1.[D2], 0) as MRP, (CASE WHEN IsNull(M2.[D2], 0) = 0 THEN M1.[D2] ELSE IsNull((M1.[D2] - (((M1.[D2]) * (M2.[D2])) / 100)), 0) END) as Price, ISNULL(M2.[D2], 0) as Discount FROM ESJSLCustomer A LEFT JOIN Master1 M1 On M1.Code = {ItemCode} And M1.MasterType = 6 LEFT JOIN MasterSupport M2 ON M2.MasterCode = M1.Code And M2.I1 = CASE WHEN ISNULL(A.CustType, 0) = 1 THEN 101 WHEN ISNULL(A.CustType, 0) = 2 THEN 102 WHEN ISNULL(A.CustType, 0) = 3 THEN 103 ELSE 103 END WHERE A.[Code] = {AccCode} Group By M1.[D2], M2.[D2]";
                }
                DataTable DT1 = new SQLHELPER(Constr).getTable(sql);
                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    distPrice.Price = Convert.ToDecimal(DT1.Rows[0]["Price"]);
                    distPrice.MRP = Convert.ToDecimal(DT1.Rows[0]["MRP"]);
                    distPrice.Discount = Convert.ToDouble(DT1.Rows[0]["Discount"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Item Category Wise Price And Discount: " + ex.Message);
            }
            return distPrice;
        }

        [HttpPost]
        public dynamic GetItemCategoryWisePriceAndDiscountList(ListOfItem obj, string CompCode, string FY, int AccCode)
        {
            List<ItemWithDiscount> items = new List<ItemWithDiscount>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                foreach (var item in obj.ItemDetails)
                {
                    var ItemCode = item.ItemCode;

                    if (ItemCode != null)
                    {
                        var priceDisc = new ItemDiscount();
                        priceDisc = GetItemCategoryWisePriceAndDiscount(constr, Convert.ToInt32(AccCode), Convert.ToInt32(ItemCode));

                        items.Add(new ItemWithDiscount
                        {
                            ItemCode = Convert.ToInt32(ItemCode),
                            ItemName = clsMain.MyString(item?.ItemName),
                            ItemDiscount = priceDisc,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = items };
        }

        [HttpGet]
        public dynamic GetBusyBillSundryList(string CompCode, string FY, int Code)
        {
            List<BillSundrylist> lst = new List<BillSundrylist>();
            int IGCode = 0; double IGST = 0; double CGST = 0; double SGST = 0;
            try
            {
                string ConnectionString = GetConnectionString(Provider, CompCode, FY);
                string queryStr = "";
                DataTable table = new DataTable();
                if (Provider == 1)
                {
                    queryStr = "Select iif(isnull(D1),0,D1) as IGST,iif(isnull(D4),0,D4) as CGST,iif(isnull(D5),0,D5) as SGST from Master1 Where Code = " + Code + "";
                    table = new OLEDBHELPER(ConnectionString).getTable(queryStr);
                }
                else
                {
                    queryStr = "Select isnull(D1,01) as IGST,isnull(D4,0) as CGST,isnull(D5,0) as SGST from Master1 Where Code = " + Code + "";
                    table = new SQLHELPER(ConnectionString).getTable(queryStr);
                }
                if (table != null && table.Rows.Count > 0)
                {
                    IGST = clsMain.MyDouble(table.Rows[0]["IGST"]);
                    CGST = clsMain.MyDouble(table.Rows[0]["CGST"]);
                    SGST = clsMain.MyDouble(table.Rows[0]["SGST"]);
                }
                if (Provider == 1)
                {
                    queryStr = "Select Name, Code,0 as D1,I1 AS BSType ,IIF(I1 = 1, 'Additive', IIF(i1 <> 1, 'Subtractive')) as BSTypeName ,D2 as BSPercent, I2 As  CalBasis,IIF(I2 = 1, 'Percentage', IIF(I2 = 0, 'Absoulte Amt', IIF(I2 = 2, 'Per Main Qty', IIF(I2 = 3, 'Per Alt Qty')))) as CalBasisName,I3 as AppliedOn, I5 as BSNature from Master1 where Mastertype = 9 order By Name";
                    table = new OLEDBHELPER(ConnectionString).getTable(queryStr);
                }
                else
                {
                    queryStr = "Select Name,Code,I1 BSType ,(case when I1 = 1 then 'Additive' else 'Subtractive' end) as BSTypeName ,D2 as BSPercent, I2 As  CalBasis,(case when I2 = 1 then 'Percentage' When I2 = 0 then 'Absoulte Amt' When I2 = 2 Then 'Per Main Qty' when I2 = 3 then 'Per Alt Qty' end) as CalBasisName,I3 as AppliedOn, I5 as BSNature from Master1 where Mastertype = 9 order By Name";
                    table = new SQLHELPER(ConnectionString).getTable(queryStr);
                }
                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in table.Rows)
                    {
                        BillSundrylist lstObj = new BillSundrylist();

                        lstObj.Code = clsMain.MyInt(dr1["Code"]);
                        lstObj.Name = clsMain.MyString(dr1["Name"]);
                        lstObj.BSType = clsMain.MyInt(dr1["BSType"]);
                        lstObj.BSTypeName = clsMain.MyString(dr1["BSTypeName"]);
                        lstObj.CalBasis = clsMain.MyInt(dr1["CalBasis"]);
                        lstObj.CalBasisName = clsMain.MyString(dr1["CalBasisName"]);
                        lstObj.AppliedOn = clsMain.MyInt(dr1["AppliedOn"]);
                        lstObj.BSNature = clsMain.MyInt(dr1["BSNature"]);
                        lstObj.BSAmount = 0;
                        lstObj.PercentOperatedOn = 0;
                        if (lstObj.BSNature == 51 || lstObj.BSNature == 52 || lstObj.BSNature == 53)
                        {
                            if (lstObj.BSNature == 51)
                            {
                                if (SGST > 0) { lstObj.BSPercent = SGST; } else { lstObj.BSPercent = clsMain.MyDouble(dr1["BSPercent"]); }
                            }
                            if (lstObj.BSNature == 52)
                            {
                                if (CGST > 0) { lstObj.BSPercent = CGST; } else { lstObj.BSPercent = clsMain.MyDouble(dr1["BSPercent"]); }
                            }
                            if (lstObj.BSNature == 53)
                            {
                                if (IGST > 0) { lstObj.BSPercent = IGST; } else { lstObj.BSPercent = clsMain.MyDouble(dr1["BSPercent"]); }
                            }
                        }

                        else
                        {
                            lstObj.BSPercent = clsMain.MyDouble(dr1["BSPercent"]);
                        }

                        lst.Add(lstObj);
                    }
                }
            }
            catch
            {
                return lst;
            }
            return lst;
        }

        [HttpGet]
        public dynamic GetBusyBillSundryList1(string CompCode, string FY, int Vchtype, int SeriesCode, int Code)
        {
            List<BillSundrylist> lst = new List<BillSundrylist>();
            int IGCode = 0;
            double IGST = 0;
            double CGST = 0;
            double SGST = 0;

            try
            {
                string ConnectionString = GetConnectionString(Provider, CompCode, FY);
                string queryStr = "";
                DataTable table = new DataTable();
                if (Provider == 1)
                {
                    queryStr = "Select iif(isnull(D1),0,D1) as IGST,iif(isnull(D4),0,D4) as CGST,iif(isnull(D5),0,D5) as SGST from Master1 Where Code = " + Code + "";
                    table = new OLEDBHELPER(ConnectionString).getTable(queryStr);
                }
                else
                {
                    queryStr = "Select isnull(D1,01) as IGST,isnull(D4,0) as CGST,isnull(D5,0) as SGST from Master1 Where Code = " + Code + "";
                    table = new SQLHELPER(ConnectionString).getTable(queryStr);
                }
                if (table != null && table.Rows.Count > 0)
                {
                    IGST = clsMain.MyDouble(table.Rows[0]["IGST"]);
                    CGST = clsMain.MyDouble(table.Rows[0]["CGST"]);
                    SGST = clsMain.MyDouble(table.Rows[0]["SGST"]);
                }

                if (Provider == 1)
                {
                    queryStr = "Select B.Code,B.Name,B.D1,B.BSType,B.BSTypeName,A.D1 as BSPercent,B.CalBasis,B.CalBasisName,B.AppliedOn,B.BSNature From(Select SrNo, MasterCode3, D1 from Tran10 Where Mastercode1 = " + SeriesCode + " And Mastercode2 = " + Code + " And Vchtype = " + Vchtype + ") A Left Join (Select Name, Code,0 as D1,I1 AS BSType ,IIF(I1 = 1, 'Additive', IIF(i1 <> 1, 'Subtractive')) as BSTypeName ,D2 as BSPercent, I2 As  CalBasis,IIF(I2 = 1, 'Percentage', IIF(I2 = 0, 'Absoulte Amt', IIF(I2 = 2, 'Per Main Qty', IIF(I2 = 3, 'Per Alt Qty')))) as CalBasisName,I3 as AppliedOn, I5 as BSNature from Master1 where Mastertype = 9) B On A.MasterCode3 = B.Code Order By A.SrNo";
                    table = new OLEDBHELPER(ConnectionString).getTable(queryStr);
                }
                else
                {
                    queryStr = "Select B.Code,B.Name,B.D1,B.BSType,B.BSTypeName,A.D1 as BSPercent,B.CalBasis,B.CalBasisName,B.AppliedOn,B.BSNature From(Select SrNo, MasterCode3, D1 from Tran10 Where Mastercode1 = " + SeriesCode + " And Mastercode2 = " + Code + " And Vchtype = " + Vchtype + ") A Left Join (Select Name, Code, 0 as D1, I1 BSType, (case when I1 = 1 then 'Additive' else 'Subtractive' end) as BSTypeName ,D2 as BSPercent, I2 As  CalBasis,(case when I2 = 1 then 'Percentage' When I2 = 0 then 'Absoulte Amt' When I2 = 2 Then 'Per Main Qty' when I2 = 3 then 'Per Alt Qty' end) as CalBasisName,I3 as AppliedOn, I5 as BSNature from Master1 where Mastertype = 9) B On A.MasterCode3 = B.Code Order By A.SrNo";
                    table = new SQLHELPER(ConnectionString).getTable(queryStr);
                }
                //if (Provider == 1)
                //{
                //    queryStr = "Select Name,Code,I1 BSType ,(case when I1 = 1 then 'Additive' else 'Subtractive' end) as BSTypeName ,D2 as BSPercent, I2 As  CalBasis,(case when I2 = 1 then 'Percentage' When I2 = 0 then 'Absoulte Amt' When I2 = 2 Then 'Per Main Qty' when I2 = 3 then 'Per Alt Qty' end) as CalBasisName,I3 as AppliedOn, I5 as BSNature from Master1 where Mastertype = 9 order By Name";
                //    table = new OLEDBHELPER(ConnectionString).getTable(queryStr);
                //}
                //else
                //{
                //    queryStr = "Select Name,Code,I1 BSType ,(case when I1 = 1 then 'Additive' else 'Subtractive' end) as BSTypeName ,D2 as BSPercent, I2 As  CalBasis,(case when I2 = 1 then 'Percentage' When I2 = 0 then 'Absoulte Amt' When I2 = 2 Then 'Per Main Qty' when I2 = 3 then 'Per Alt Qty' end) as CalBasisName,I3 as AppliedOn, I5 as BSNature from Master1 where Mastertype = 9 order By Name";
                //    table = new SQLHELPER(ConnectionString).getTable(queryStr);
                //}
                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow dr1 in table.Rows)
                    {
                        BillSundrylist lstObj = new BillSundrylist();

                        lstObj.Code = clsMain.MyInt(dr1["Code"]);
                        lstObj.Name = clsMain.MyString(dr1["Name"]);
                        lstObj.BSType = clsMain.MyInt(dr1["BSType"]);
                        lstObj.BSTypeName = clsMain.MyString(dr1["BSTypeName"]);
                        lstObj.CalBasis = clsMain.MyInt(dr1["CalBasis"]);
                        lstObj.CalBasisName = clsMain.MyString(dr1["CalBasisName"]);
                        lstObj.AppliedOn = clsMain.MyInt(dr1["AppliedOn"]);
                        lstObj.BSNature = clsMain.MyInt(dr1["BSNature"]);
                        lstObj.BSAmount = 0;
                        lstObj.PercentOperatedOn = 0;
                        if (lstObj.BSNature == 51 || lstObj.BSNature == 52 || lstObj.BSNature == 53)
                        {
                            if (lstObj.BSNature == 51)
                            {
                                if (SGST > 0) { lstObj.BSPercent = SGST; } else { lstObj.BSPercent = clsMain.MyDouble(dr1["BSPercent"]); }
                            }
                            if (lstObj.BSNature == 52)
                            {
                                if (CGST > 0) { lstObj.BSPercent = CGST; } else { lstObj.BSPercent = clsMain.MyDouble(dr1["BSPercent"]); }
                            }
                            if (lstObj.BSNature == 53)
                            {
                                if (IGST > 0) { lstObj.BSPercent = IGST; } else { lstObj.BSPercent = clsMain.MyDouble(dr1["BSPercent"]); }
                            }
                        }

                        else
                        {
                            if (lstObj.CalBasis == 0)
                            {
                                lstObj.BSAmount = clsMain.MyDouble(dr1["BSPercent"]);
                            }
                            else
                            {
                                lstObj.BSPercent = clsMain.MyDouble(dr1["BSPercent"]);
                            }

                        }


                        lst.Add(lstObj);
                    }
                }
            }
            catch
            {
                return lst;
            }
            return lst;
        }

        [HttpPost]
        public dynamic SaveQuatationData(TransactionData obj, string CompCode, string FY)
        {
            int Status = 0; string StatusStr = string.Empty;
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER objcon = new SQLHELPER(constr);

                string XmlData = CreateXML(obj.ItemDetails);
                //string XmlBSData = CreateXML(obj.BSDetails);
                string vchNo = GetVchNo(constr, Convert.ToInt32(108), Convert.ToInt32(1), out int autoNo);

                // Define parameters with @ prefix
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@VchCode", SqlDbType.Int) { Value = obj.VchCode },
                new SqlParameter("@VchNo", SqlDbType.VarChar, 40) { Value = vchNo },
                new SqlParameter("@AutoVchNo", SqlDbType.Int) { Value = autoNo },
                new SqlParameter("@CustId", SqlDbType.Int) { Value = obj.CustId },
                new SqlParameter("@CMobile", SqlDbType.VarChar, 50) { Value = obj.CMobile },
                new SqlParameter("@TQty", SqlDbType.Float) { Value = obj.TQty },
                new SqlParameter("@TAmt", SqlDbType.Float) { Value = obj.TAmt },
                new SqlParameter("@Discount", SqlDbType.Float) { Value = obj.Discount },
                new SqlParameter("@TaxType", SqlDbType.Float) { Value = obj.TaxType },
                new SqlParameter("@TaxAmt1", SqlDbType.Float) { Value = obj.TaxAmt1 },
                new SqlParameter("@TaxAmt2", SqlDbType.Float) { Value = obj.TaxAmt2 },
                new SqlParameter("@NetAmt", SqlDbType.Float) { Value = obj.NetAmt },
                new SqlParameter("@Users", SqlDbType.VarChar, 50) { Value = obj.Users },
                new SqlParameter("@ItemDetails", SqlDbType.NVarChar, -1) { Value = XmlData },
                    //new SqlParameter("@BSDetails", SqlDbType.NVarChar, -1) { Value = XmlBSData }
                };

                // Execute the stored procedure
                DataTable DT1 = objcon.getTable("sp_SaveQuatationTransactionData", parameters);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    Status = Convert.ToInt32(DT1.Rows[0]["Status"]);
                    StatusStr = DT1.Rows[0]["Msg"].ToString();
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
#pragma warning disable IDE0037 // Use inferred member name
            return new { Status = Status, Msg = StatusStr };
#pragma warning restore IDE0037 // Use inferred member name
        }

        public dynamic GetSubAttributes(string Constr, int VchCode, int ItemCode)
        {
            List<GetAttributes> List = new List<GetAttributes>();
            try
            {
                string sql = string.Empty;

                for (int i = 1; i <= 10; i++)  // Iterate from CM1 to CM10
                {
                    // Concatenate each query and use the proper CM column reference
                    sql += $"SELECT IsNull(T.[CM{i}], 0) AS ATCode, IsNull(M.Name, '') as ATName, IsNull(M.[I1], 0) as HCode, IsNull(M1.[Name], '') as HName FROM ESJSLTRAN2 T INNER JOIN ESMaster1 M ON T.CM{i} = M.Code LEFT JOIN ESMaster1 M1 ON M.I1 = M1.Code And M1.MasterType = 1 WHERE T.ItemCode = {Convert.ToInt32(ItemCode)} AND T.VchCode = {VchCode} AND T.VchType = 108 AND T.RecType = 1 AND T.CM{i} > 0";

                    // Add UNION ALL between queries except for the last one
                    if (i < 10)
                    {
                        sql += " UNION ALL ";
                    }
                }
                DataTable DT1 = new SQLHELPER(Constr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        List.Add(new GetAttributes
                        {
                            HCode = Convert.ToInt32(item["HCode"]),
                            HName = clsMain.MyString(item["HName"]),
                            ATCode = Convert.ToInt32(item["ATCode"]),
                            ATName = clsMain.MyString(item["ATName"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = List };
        }

        [HttpGet]
        public dynamic GetPendingItemListForCancellection(string CompCode, string FY, int TranType, int MCCode, int VchType, int VchCode)
        {
            List<STQuotItemsDt> list = new List<STQuotItemsDt>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr); string sql = string.Empty;
                DataTable DT1 = new DataTable();
                switch (TranType)
                {
                    case 1:
                        sql = $"Select ISNULL(A.[MasterCode2], 0) as ItemCode, ISNULL(B.[Name], '') as ItemName, ISNULL(B.[CM1], 0) as UCode, ISNULL(C.[Name], '') as UName, ISNULL(SUM(A.[Value1]), 0) as Qty, IsNull((Select Top 1 Price From ESJSLTran2 Where VchType = 108 And A.MasterCode2 = ItemCode And RecType = 1), 0) as Price from ESJSLRefTran A Inner join Master1 B On A.MasterCode2 = B.Code Inner Join ESJSLTran1 T1 On A.RefCode = T1.VchCode Left Join Master1 C On B.CM1 = C.Code Where B.MasterType = 6 And A.[RecType] In (1, 2) And T1.[VchType] = 108 And T1.VchCode = {VchCode} And T1.QStatus = 1 Group by A.MasterCode2, B.[Name], B.[CM1], C.[Name] Having Sum(A.Value1) >= 0.01";
                        break;
                    case 2:
                        break;
                }
                DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        list.Add(new STQuotItemsDt
                        {
                            ItemCode = Convert.ToInt32(item["ItemCode"]),
                            ItemName = clsMain.MyString(item["ItemName"]).Trim(),
                            UCode = Convert.ToInt32(item["UCode"]),
                            UName = clsMain.MyString(item["UName"]).Trim(),
                            Qty = Convert.ToDecimal(item["Qty"]),
                            Price = Convert.ToDecimal(item["Price"]),
                            Amount = (Convert.ToDecimal(item["Qty"]) * Convert.ToDecimal(item["Price"]))
                        });
                    }
                }
                else
                {
                    throw new Exception("Data Not Found !!!");
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), Data = list };
            }
            return new { Status = 1, Msg = "Success", Data = list };
        }

        [HttpPost]
        public dynamic SaveOrderFollowUp(FollowUp obj, string CompCode, string FY)
        {
            int Status = 0; string StatusStr = string.Empty;
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr);
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@VchCode", SqlDbType.Int) { Value = obj.VchCode },
                new SqlParameter("@VchType", SqlDbType.Int) { Value = obj.VchType },
                new SqlParameter("@Remarks", SqlDbType.VarChar, 255) { Value = obj.Remarks },
                new SqlParameter("@FollowdBy", SqlDbType.VarChar, 40) { Value = obj.FollowdBy },
                new SqlParameter("@Users", SqlDbType.VarChar, 40) { Value = obj.Users }
                };

                DataTable DT1 = conobj.getTable("dbo.[sp_SaveOrderFollowUp]", parameters);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    Status = Convert.ToInt32(DT1.Rows[0]["Status"]);
                    StatusStr = clsMain.MyString(DT1.Rows[0]["Msg"]);
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), };
            }
            return new { Status = Status, Msg = StatusStr };
        }

        [HttpGet]
        public dynamic GetOrderFollowUp(string CompCode, string FY, int VchCode, int VchType)
        {
            List<GetFollowUp> flist = new List<GetFollowUp>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY); int SNo = 1;
                SQLHELPER conobj = new SQLHELPER(constr);

                string sql = $"Select ISNULL(B.[SNo], 0) as SNo, CONVERT(VARCHAR, B.[FollowdOn],105) as FDate, IsNull(B.Person, '') as FPerson, ISNULL(B.[Remarks], '') as Remarks From ESJSLTRAN1 A Inner Join ESJSLFOLLOWUP B ON A.VCHCODE = B.VCHCODE AND A.VCHTYPE = B.VCHTYPE WHERE A.[VCHCODE] = {VchCode} AND A.[VCHTYPE] = {VchType} ORDER BY B.[SNO] Desc";
                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        flist.Add(new GetFollowUp
                        {
                            SNo = SNo++,
                            FDate = clsMain.MyString(item["FDate"]),
                            Person = clsMain.MyString(item["FPerson"]),
                            Remarks = clsMain.MyString(item["Remarks"])
                        });
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found !!!", Data = flist };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), Data = flist };
            }
            return new { Status = 1, Msg = "Success", Data = flist };
        }

        [HttpGet]
        public dynamic GetFollowUpHistoryRpt(string CompCode, string FY, int VchType, int Status, string StartDate, string EndDate)
        {
            List<FollowUpRpt> RPTL = new List<FollowUpRpt>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr);
                string formattedStartDate = string.IsNullOrEmpty(StartDate) ? "" : Busyhelper.FormatDate(StartDate);
                string formattedEndDate = string.IsNullOrEmpty(EndDate) ? "" : Busyhelper.FormatDate(EndDate);

                string sql = $"Select A.VchCode, IsNull(A.[VchNo], '') as VchNo, CONVERT(VARCHAR, A.Date, 105) as Date, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(C.[Name], '') as AccName, IsNull(B.Remarks, '') as Status, IsNull(B.[FollowdBy], '') as FollowedBy, IsNull(CONVERT(VARCHAR, B.FollowdOn, 105), '') as FollowedOn From ESJSLTran1 A Inner Join ESJSLFollowUp B On A.VchCode = B.VchCode And A.[VchType] = B.[VchType] LEFT JOIN ESJSLCustomer C On A.MasterCode1 = C.Code Where A.[VchType] = {VchType}";
                if (Status != 0) sql += $" And A.[QStatus] = {Status}";
                if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And Date >= '{formattedStartDate}' And Date <= '{formattedEndDate}'";
                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        RPTL.Add(new FollowUpRpt
                        {
                            VchCode = Convert.ToInt32(item["VchCode"]),
                            VchNo = item["VchNo"].ToString().Trim(),
                            Date = item["Date"].ToString().Trim(),
                            AccCode = Convert.ToInt32(item["AccCode"]),
                            AccName = clsMain.MyString(item["AccName"]).Trim(),
                            Status = clsMain.MyString(item["Status"]).Trim(),
                            FollowedBy = clsMain.MyString(item["FollowedBy"]).Trim(),
                            FollowedOn = clsMain.MyString(item["FollowedOn"]).Trim(),
                        });
                    }
                }
                else
                {
                    return new { Status = 1, Msg = "Data Not Found !!!", Data = RPTL };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), Data = RPTL };
            }
            return new { Status = 1, Msg = "Success", Data = RPTL };
        }

        [HttpGet]
        public dynamic GetOrderPrintingDetails(string CompCode, string FY, int VchCode, int VchType)
        {
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr); int sno = 1;

                string sql = $"Select A.[VchCode], IsNull(A.[VchNo], '') as VchNo, Convert(Varchar, A.[Date], 105) as VchDate, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(D.[Name], '') as AccName, IsNull(D.[Mobile], '') as Mobile,IsNull(A.[TotQty], 0) as TotQty,IsNull(A.[TotAmt], 0) as TotAmt,IsNull(A.[NetAmount], 0) as NetAmt,IsNull(A.[QStatus], 0) as QStatus, (CASE WHEN [QStatus] = 1 Then 'Approved' WHEN [QStatus] = 2 Then 'Rejected' ELSE 'Pending' END) as QName, IsNull(B.[ItemCode], 0) as ItemCode, IsNull(C.[Name], '') as ItemName, IsNull(B.[Qty], 0) as Qty, IsNull(B.[Price], 0) as Price, IsNull(B.[Amount], 0) as Amount From ESJSLTRAN1 A Inner Join ESJSLTRAN2 B On A.VchCode = B.VchCode And A.VchType = B.VchType And B.RecType = 1 Left Join Master1 C On B.ItemCode = C.Code And C.MasterType = 6 INNER JOIN ESJSLCustomer D ON A.MasterCode1 = D.Code Where A.VchType = {VchType} And A.VchCode = {VchCode}";
                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    var head = new GetOrderPrinting
                    {
                        VchCode = Convert.ToInt32(DT1.Rows[0]["VchCode"]),
                        VchNo = DT1.Rows[0]["VchNo"].ToString().Trim(),
                        Date = DT1.Rows[0]["VchDate"].ToString().Trim(),
                        AccCode = Convert.ToInt32(DT1.Rows[0]["AccCode"]),
                        AccName = clsMain.MyString(DT1.Rows[0]["AccName"]).Trim(),
                        Mobile = clsMain.MyString(DT1.Rows[0]["Mobile"]).Trim(),
                        TQty = Convert.ToDecimal(DT1.Rows[0]["TotQty"]),
                        TAmt = Convert.ToDecimal(DT1.Rows[0]["TotAmt"]),
                        NetAmt = Convert.ToDecimal(DT1.Rows[0]["NetAmt"]),
                        QStatus = Convert.ToInt32(DT1.Rows[0]["QStatus"]),
                        QName = clsMain.MyString(DT1.Rows[0]["QName"]),
                        ItemsDetails = new List<PrintItemsDT>()
                    };

                    foreach (DataRow item in DT1.Rows)
                    {
                        head.ItemsDetails.Add(new PrintItemsDT
                        {
                            SNo = clsMain.MyInt(sno++),
                            ItemCode = Convert.ToInt32(item["ItemCode"]),
                            ItemName = clsMain.MyString(item["ItemName"]).Trim(),
                            Qty = Convert.ToDecimal(item["Qty"]),
                            Price = Convert.ToDecimal(item["Price"]),
                            Amount = Convert.ToDecimal(item["Amount"])
                        });
                    }
                    return new { Status = 1, Msg = "Success", Data = head };
                }
                else
                {
                    return new { Status = 1, Msg = "Data Not Found !!!" };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }

        }

        [HttpPost]
        public dynamic AutoPostReceiptAndVchApproved(PaymentDetails obj, string CompCode, string FY)
        {
            try
            {
                string ConStr = GetConnectionString(Provider, CompCode, FY);
                VchApprovel Inv_Ap = GetOrderApprovelData(obj);

                //if (Inv_Ap.SCode == 1 && Inv_Ap.ReceiptYesNo == 1)
                //{
                //    SaveReceiptOrderPayment(obj, Inv_Ap, CompCode, FY);
                //}
                //else
                //{
                UpdateInvoiceOtherDetails(Inv_Ap, obj, 108, 0, obj.OrderId, ConStr);
                //}
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = obj.SCode == 1 ? "Approved Successfully !!!" : "Rejection Successfully !!!" };
        }

        [HttpGet]
        public dynamic GetOrderPaymentReceiptList(string CompCode, string FY, string StartDate, string EndDate)
        {
            List<PaymentReceipt> R_List = new List<PaymentReceipt>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);

                string sql = "Select A.[VchCode], IsNull((CONVERT(VARCHAR, A.[Date], 105)), '') as VchDate, IsNull(A.[VchNo], 0) as VchNo, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(M.[Name], '') as AccName, IsNull(M.[Mobile], '') as Mobile, IsNull(A.NetAmount, 0) as NetAmt, IsNull((CONVERT(VARCHAR, B.[Date], 105)), '') as RDate, IsNull(LTrim(B.[VchNo]), '') as RVchNo, IsNull(B.[OrgVchAmtBaseCur], 0) as RAmt From ESJSLTran1 A LEFT JOIN ESJSLCustomer M ON A.MasterCode1 = M.Code INNER JOIN Tran1 B ON A.[BusyVchCode] = B.VchCode And B.VchType = 14 Where A.[VchType] = 108 Order By A.[VchCode]";
                DataTable DT1 = new SQLHELPER(constr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    R_List.Add(new PaymentReceipt
                    {
                        VchCode = Convert.ToInt32(DT1.Rows[0]["VchCode"]),
                        VchNo = Convert.ToString(DT1.Rows[0]["VchNo"]),
                        VchDate = Convert.ToString(DT1.Rows[0]["VchDate"]),
                        AccCode = Convert.ToInt32(DT1.Rows[0]["AccCode"]),
                        AccName = clsMain.MyString(DT1.Rows[0]["AccName"]),
                        Mobile = clsMain.MyString(DT1.Rows[0]["Mobile"]),
                        Amount = Convert.ToDecimal(DT1.Rows[0]["NetAmt"]),
                        RVchDate = Convert.ToString(DT1.Rows[0]["RDate"]),
                        RVchNo = Convert.ToString(DT1.Rows[0]["RVchNo"]),
                        RAmount = Convert.ToDecimal(DT1.Rows[0]["RAmt"])
                    });
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found !!!", Data = R_List };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = R_List };
        }

        public void UpdateInvoiceOtherDetails(VchApprovel obj, PaymentDetails R_Obj, int VchType, int BusyCode, int VchCode, string constr)
        {
            try
            {
                SQLHELPER conobj = new SQLHELPER(constr);
                string formattedDate = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                string sql = "";

                sql = $"UPDATE ESJSLTRAN1 SET [QStatus] = {obj.SCode}, [Remarks] = '{obj.Remarks.Replace("'", "''")}', [ApprovedBy] = '{obj.ApprovedBy}', [ApprovedOn] = '{formattedDate}', [BusyVchCode] = {BusyCode} Where VchCode = {VchCode} And [VchType] = {VchType}";
                int DT1 = conobj.ExecuteSQL(sql);

                if (DT1 > 0)
                {
                    if (R_Obj.SCode == 1)
                    {
                        if (R_Obj.PaymentReceipt == 1)
                        {
                            sql = $"INSERT INTO ESJSLRECEIPT ([VchCode], [RecType], [PaymentType], [ChequeDate], [ChequeNo], [Images], [Amount], [Status], [BusyCode]) Values ({clsMain.MyInt(VchCode)}, 1, {Convert.ToInt32(R_Obj.PaymentType)}, '{Busyhelper.FormatDate(Convert.ToString(R_Obj.ChequeDate))}', '{R_Obj.ChequeNo}', '{R_Obj.ChequeImage}',  {Convert.ToDouble(R_Obj.Amount)}, 0, 0)";
                            int DT2 = new SQLHELPER(constr).ExecuteSQL(sql);
                        }

                        sql = $"Delete ESJSLREFTRAN Where VchCode = " + VchCode + " And Vchtype = " + VchType + " ";
                        int DT3 = new SQLHELPER(constr).ExecuteSQL(sql);

                        sql = $"Select A.VchCode, ISNULL(A.[VchNo], '') as RefNo, CONVERT(VARCHAR, A.[Date], 105) as Date, ISNULL(A.[MasterCode1], 0) as AccCode, B.[SNo], ISNULL(B.[ItemCode], 0) as ItemCode, ISNULL(B.[Qty], 0) as Qty, ISNULL(B.[Price], 0) as Price, ISNULL(B.[Amount], 0) as Amount From ESJSLTRAN1 A Inner Join ESJSLTRAN2 B On A.VchCode = B.VchCode And A.VchType = B.VchType And B.RecType = 1 Where A.[VchCode] = {VchCode} And A.[Vchtype] = {VchType} Order By B.[SNo]";
                        DataTable DT4 = new SQLHELPER(constr).getTable(sql);

                        if (DT4 != null && DT4.Rows.Count > 0)
                        {
                            foreach (DataRow item in DT4.Rows)
                            {
                                string formattedDt = Busyhelper.FormatDate(Convert.ToString(item["Date"]));
                                sql = $"INSERT INTO ESJSLREFTRAN ([RefCode], [RecType], [Method], [VchCode], [VchType], [Date],  [RefNo], [MasterCode1], [MasterCode2], [Value1], [Value2], [Value3], [NewRefVchCode], [NewRefVchNo]) Values ({clsMain.MyInt(item["VchCode"])}, 1, 1,{clsMain.MyInt(item["VchCode"])}, 108, '{formattedDt}', '{clsMain.MyString(item["RefNo"])}', {clsMain.MyInt(item["AccCode"])}, {clsMain.MyInt(item["ItemCode"])}, {Convert.ToDecimal(item["Qty"])}, {Convert.ToDecimal(item["Price"])}, {Convert.ToDecimal(item["Amount"])}, 0, '{clsMain.MyString(item["RefNo"])}')";
                                int DT5 = new SQLHELPER(constr).ExecuteSQL(sql);

                                if (DT5 == 0)
                                {
                                    throw new Exception("Unable To Connect To Company !!!");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Unable To Connect To Company !!!");
                        }
                    }
                }
                else
                {
                    throw new Exception("Unable To Connect To Company !!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private void SaveReceiptOrderPayment(PaymentDetails Obj, VchApprovel Inv_Ap, string CompCode, string FY)
        {
            try
            {
                string ConStr = GetConnectionString(Provider, CompCode, FY);

                ValidateOrCreateBusyAccount(Obj, CompCode, FY);

                AutoOrderDT AutoOrderDT = GetVoucherConfigDetails(ConStr, 2);

                VchReceipt Inv = GetOrderReceiptData(AutoOrderDT, Obj);

                var AlertOrder = SaveAccInvoiceAuto(Inv, Inv_Ap, 14, Obj.OrderId, CompCode, FY);

                if (AlertOrder?.BusyCode > 0) UpdateInvoiceOtherDetails(Inv_Ap, Obj, 108, clsMain.MyInt(AlertOrder?.BusyCode), Obj.OrderId, ConStr);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        private void ValidateOrCreateBusyAccount(PaymentDetails Obj, string CompCode, string FY)
        {
            try
            {
                string sql = string.Empty; DataTable DT1 = new DataTable();
                CRParty CR_PARTY = new CRParty();
                string ConStr = GetConnectionString(Provider, CompCode, FY);

                sql = $" Select M.* From Master1 M INNER JOIN MasterAddressInfo M1 On M.Code = M1.MasterCode Where M.MasterType = 2 And M1.Mobile = '{Obj.Mobile.Replace("'", "''")}'";
                DT1 = new SQLHELPER(ConStr).getTable(sql);

                if (DT1 == null || DT1.Rows.Count == 0)
                {
                    sql = $" Select M.* From Master1 M INNER JOIN MasterAddressInfo M1 On M.Code = M1.MasterCode Where M.MasterType = 2 And M.[Name] = '{Obj.PartyName}'";
                    DT1 = new SQLHELPER(ConStr).getTable(sql);

                    if (DT1 == null || DT1.Rows.Count == 0)
                    {
                        sql = $"Select IsNull(A.[Code], 0) as Code, IsNull(A.[Name], '') as Name, IsNull(A.[Mobile], '') as Mobile, IsNull(A.[Email], '') as Email, IsNull(A.[GSTIN], '') as GSTIN, IsNull(A.[OrgName], '') as FirmName, IsNull(A.[CM1], 0) as CNCode, IsNull(B.[Name], '') as CNName, IsNull(A.[CM2], 0) as STCode, IsNull(C.[Name], '') as STName, IsNull(A.[CM3], 0) as CTCode, IsNull(D.[Name], '') as CTName, IsNull(A.[Address], '') as [Address],IsNull(A.[Deactive], 0) as Deactive From ESJSLCUSTOMER A Left Join ESJSLCountryMaster B On A.CM1 = B.Code And B.MasterType = 1 Left Join ESJSLCountryMaster C On A.CM2 = C.Code And C.MasterType = 2 Left Join ESJSLCountryMaster D On A.[CM3] = D.[Code] And D.[MasterType] = 3 Where 1=1 And A.Mobile = '{Obj?.Mobile}' And A.Code = {Obj?.PartyCode}";
                        DT1 = new SQLHELPER(ConStr).getTable(sql);

                        if (DT1 != null && DT1.Rows.Count > 0)
                        {
                            CR_PARTY.Name = clsMain.MyString(DT1.Rows[0]["Name"]);
                            CR_PARTY.MobileNo = clsMain.MyString(DT1.Rows[0]["Mobile"]);
                            CR_PARTY.Email = clsMain.MyString(DT1.Rows[0]["Email"]);
                            CR_PARTY.GSTNO = clsMain.MyString(DT1.Rows[0]["GSTIN"]);
                            CR_PARTY.Address1 = clsMain.MyString(DT1.Rows[0]["Address"]);
                            CR_PARTY.Address2 = clsMain.MyString(DT1.Rows[0]["CTName"]);
                            CR_PARTY.StateCode = Convert.ToString(DT1.Rows[0]["STCode"]);
                            CR_PARTY.StateName = clsMain.MyString(DT1.Rows[0]["STName"]);
                            CR_PARTY.CountryName = clsMain.MyString(DT1.Rows[0]["CTName"]);

                            SaveAccAuto(CR_PARTY, CompCode, FY);
                        }
                        else
                        {
                            throw new Exception("Party Details Not Found. !!!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        private void SaveAccAuto(CRParty Obj, string CompCode, string FY)
        {
            try
            {
                BusyVoucher BVch = new BusyVoucher();
                string ConnectionString = GetConnectionString(Provider, CompCode, FY);
                CFixedInterface FI = new CFixedInterface();
                string XMLStr = "";

                XMLStr = GetAccountXML(Obj, ConnectionString);
                bool Connect = false;

                FI.CloseDB();

                if (Provider == 1)
                {
                    Connect = FI.OpenDBForYear(BusyPath, BusyDataPath, CompCode, Convert.ToInt16(FY));
                }
                else
                {
                    Connect = FI.OpenCSDBForYear(BusyPath, ServerName, SUserName, SPassword, CompCode, Convert.ToInt16(FY));
                }

                if (Connect == true)
                {
                    object Err = "";
                    int Return = FI.AddMasterFromXML(Convert.ToInt32(2), XMLStr, ref Err);   //SaveMasterFromXML(MType, XMLStr, ref Err, false, 0, ref Code);
                    if (Return == 0)
                    {
                        throw new Exception("Unable To Create a Customer," + Err.ToString());
                    }
                }
                else
                {
                    throw new Exception("Unable To Connect To Company");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public string GetAccountXML(CRParty Obj, string ConStr)
        {
            string XMLStr = "";
            try
            {
                BusyVoucher BVch = new BusyVoucher();
                List<BusyVoucher.Address> Acc3 = new List<BusyVoucher.Address>();
                AccMaster AccObj = new AccMaster();
                Accounts Acc1Obj = new Accounts();
                Account Acc2Obj = new Account();
                BusyVoucher.Address Acc3Obj = new BusyVoucher.Address();

                Acc2Obj.Name = Obj.Name;
                Acc2Obj.PrintName = Obj.Name;
                Acc2Obj.ParentGroup = BVch.GetMasterCodeToName(ConStr, 116);
                Acc2Obj.tmpParentGrpCode = 116;
                Acc2Obj.ChequePrintName = Obj.Name;
                Acc2Obj.BrokerAssigned = false;
                Acc2Obj.BrokerName = "";
                /////Address -------------------

                Acc3Obj.Address1 = Obj.Address1;
                Acc3Obj.Address2 = Obj.Address2;
                Acc3Obj.Address3 = Obj.Address3;
                // Acc3Obj.Address4 = Obj.OwnerName;
                Acc3Obj.CountryName = Obj.CountryName;
                Acc3Obj.StateName = Obj.StateName;
                Acc3Obj.PINCode = int.Parse(Obj.StateCode);
                Acc3Obj.Mobile = Obj.MobileNo;
                Acc3Obj.GSTNo = Obj.GSTNO;
                Acc3Obj.ITPAN = Obj.PAN;
                Acc3Obj.C3 = Obj.AdharNo;
                Acc3Obj.Contact = Obj.OwnerName;
                Acc3Obj.Email = Obj.Email;
                Acc3Obj.RegionName = "---Others---";
                Acc3Obj.AreaName = "---Others---";
                Acc2Obj.Address = Acc3Obj;
                Acc2Obj.SupplierType = 1;
                Acc2Obj.PriceLevel = "@";
                Acc2Obj.PriceLevelForPurc = "@";
                Acc2Obj.TaxType = "Others";
                Acc2Obj.TypeOfDealerGST = "Un-Registered";
                Acc2Obj.ReverseChargeType = "Not Applicable";
                Acc2Obj.InputType = "Section 17(5)-ITC None";

                //Acc1.BillByBillBalancing = "True";
                //Acc2.Add(Acc2Obj);
                Acc1Obj.Account = Acc2Obj;
                // Acc.AccDet = Acc1;
                AccObj.Accounts = Acc1Obj;

                XMLStr = CreateXML(Acc2Obj).Replace("<?xml version=\"1.0\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "").Replace("<BillRefDT>", "").Replace("</BillRefDT>", "");
            }
            catch
            {
                return "";
            }

            return XMLStr;
        }

        private VchReceipt GetOrderReceiptData(AutoOrderDT S_Obj, PaymentDetails Obj)
        {
            VchReceipt Inv = new VchReceipt();

            Inv.SeriesCode = Convert.ToInt32(S_Obj?.SeriesCode);
            Inv.SeriesName = S_Obj?.SeriesName;
            Inv.PartyName = Obj.PartyName;
            Inv.CashAcc = S_Obj?.Name1;
            Inv.BankAcc = S_Obj?.Name2;
            Inv.PDC = Obj.PDCReq;
            Inv.PDCDate = Obj.PDCDate;
            Inv.Mode = clsMain.MyInt(Obj.PaymentType);
            Inv.Amount = clsMain.MyDouble(Obj.Amount);
            Inv.ChequeNo = Obj.ChequeNo;
            Inv.ChequeDt = Obj.ChequeDate;
            Inv.Image = clsMain.MyString(Obj.ChequeImage);
            Inv.Remarks = Obj.Remarks;
            Inv.BillByBill = Obj.BillByBill;
            return Inv;
        }

        private VchApprovel GetOrderApprovelData(PaymentDetails Obj)
        {
            VchApprovel Inv_Ap = new VchApprovel();
            Inv_Ap.VchCode = Obj.OrderId;
            Inv_Ap.SCode = Obj.SCode;
            Inv_Ap.Remarks = Obj.Remarks;
            Inv_Ap.ApprovedBy = Obj.ApprovedBy;
            Inv_Ap.ReceiptYesNo = Obj.PaymentReceipt;
            return Inv_Ap;
        }

        private dynamic SaveAccInvoiceAuto(VchReceipt Inv, VchApprovel Inv_Ap, int VchType, int VchCode, string CompCode, string FY)
        {
            int Status = 0; string Msg = ""; object BusyVchCode = 0; object Err = "";
            CFixedInterface FI = new CFixedInterface();
            try
            {
                string XMLStr = ""; string ConStr = GetConnectionString(Provider, CompCode, FY);
                XMLStr = GetReceiptVoucherXML(VchType, Inv, ConStr);
                bool Connect = false;

                FI.CloseDB();

                Connect = FI.OpenCSDBForYear(BusyPath, ServerName, SUserName, SPassword, CompCode, Convert.ToInt16(FY));

                if (Connect == true)
                {
                    bool Return = FI.SaveVchFromXML(VchType, XMLStr, ref Err, false, 0, ref BusyVchCode);
                    if (Return == true)
                    {
                        Status = 1; Msg = "Success";
                    }
                    else
                    {
                        throw new Exception(Err.ToString());
                    }
                }
                else
                {
                    throw new Exception("Unable To Connect To The Company. !!!");
                }
            }
            catch (Exception ex)
            {
                if (clsMain.MyInt(BusyVchCode) > 0)
                {
                    FI.DeleteVchByCode(clsMain.MyInt(BusyVchCode), ref Err);
                }
                throw new Exception(ex.Message + Err);
            }
            return new { Status = Status, Msg = Msg, BusyCode = clsMain.MyInt(BusyVchCode) };
        }

        [HttpGet]
        public dynamic GetStockTransferQuotataionDt(string CompCode, string Fy, int MCCode)
        {
            List<GetStockTransferQuotDt> qlist = new List<GetStockTransferQuotDt>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, Fy);
                SQLHELPER conobj = new SQLHELPER(constr);

                //string sql = $"Select A.[VchCode], A.[VchNo], A.[VchDate], A.[AccCode], A.[AccName] From (Select A.VchCode, A.[VchNo], A.[VchDate], A.[AccCode], A.[AccName], A.[CustMobile], A.[ItemCode], ISNULL(SUM(B.[Qty]), 0) as STK From (Select A.[VchCode], IsNull(A.[VchNo], '') as VchNo, CONVERT(VARCHAR, A.[Date], 105) as VchDate,IsNull(A.[MasterCode1], 0) as AccCode, ISNULL(A.[CustName], '') as AccName, ISNULL(A.[CMobile], '') as CustMobile, ISNULL(B.[ItemCode], 0) as ItemCode From ESJSLTran1 A Inner Join ESJSLTran2 B ON A.VchCode = B.VchCode And B.VchType = 108 Left Join Master1 C On B.ItemCode = C.Code Where A.QStatus = 1 And C.CM6 = {MCCode}) A Inner Join ESJSLRefTran B On A.VchCode = B.VchCode And A.ItemCode = B.ItemCode And B.RecType = 1 Left Join Master1 C On A.ItemCode = C.Code And C.MasterType = 6 Group By A.VchCode, A.[VchNo], A.[VchDate], A.[AccCode], A.[AccName], A.[CustMobile], A.[ItemCode], C.[Name] Having Sum(Qty) >= 0.01) A Group By A.[VchCode], A.[VchNo], A.[VchDate], A.[AccCode], A.[AccName]";
                string sql = $"Select A.VchCode, ISNULL(A.[VchNo], '') as VchNo, CONVERT(VARCHAR, A.[Date], 105) as VchDate, ISNULL(A.[MasterCode1], 0) as AccCode, ISNULL(M.[Name], '') as AccName, ISNULL(M.[Mobile], '') As CMobile, ISNULL(M.[Email],'') as CEmail, ISNULL(M.[Address], '') as CAddress, ISNULL(M.[GSTIN], '') As CGSTIN From (Select IsNull(A.[RefCode], 0) As OrderId from ESJSLRefTran A Inner join Master1 B On A.MasterCode2 = B.Code Inner Join ESJSLTran1 T1 On A.RefCode = T1.VchCode Where B.MasterType = 6 And B.CM6 = {MCCode} And A.RecType = 1 And T1.QStatus = 1 And T1.Cancelled <> 1 Group by A.RefCode Having Sum(A.Value1) >= 0.01) O Inner join ESJSLTran1 A On O.OrderID = A.VchCode Left Join ESJSLCustomer M On A.MasterCode1 = M.Code Group by A.VchCode, A.[VchNo], A.[Date], A.[MasterCode1], M.[Name], M.[Mobile], M.[Email], M.[Address], M.[GSTIN] Order By A.Date,A.VchCode";
                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        qlist.Add(new GetStockTransferQuotDt
                        {
                            VchCode = Convert.ToInt32(item["VchCode"]),
                            VchNo = clsMain.MyString(item["VchNo"]).Trim(),
                            VchDate = Convert.ToString(item["VchDate"]).Trim(),
                            AccCode = Convert.ToInt32(item["AccCode"]),
                            AccName = clsMain.MyString(item["AccName"]).Trim(),
                            CMobile = clsMain.MyString(item["CMobile"]).Trim(),
                            CEmail = clsMain.MyString(item["CEmail"]).Trim(),
                            CGSTIN = clsMain.MyString(item["CGSTIN"]).Trim(),
                            CAddress = clsMain.MyString(item["CAddress"]).Trim(),
                        });
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found !!!", Data = qlist };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), Data = qlist };
            }
            return new { Status = 1, Msg = "Success", Data = qlist };
        }

        [HttpGet]
        public dynamic GetStockTransferQuotataionItemsDt(string CompCode, string Fy, int VchCode, int MCCode)
        {
            List<STQuotItemsDt> list = new List<STQuotItemsDt>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, Fy);
                SQLHELPER conobj = new SQLHELPER(constr);

                string sql = $"Select A.ItemCode, A.ItemName, A.UCode, UName, A.Q as Qty,A.Price,STK.Stock From (Select ISNULL(A.[MasterCode2], 0) as ItemCode, ISNULL(B.[Name], '') as ItemName, ISNULL(B.[CM1], 0) as UCode, ISNULL(C.[Name], '') as UName, ISNULL(SUM(A.[Value1]), 0) as Q, IsNull((Select Top 1 Price From ESJSLTran2 Where VchType = 108 And A.MasterCode2 = ItemCode And RecType = 1), 0) as Price from ESJSLRefTran A Inner join Master1 B On A.MasterCode2 = B.Code Inner Join ESJSLTran1 T1 On A.RefCode = T1.VchCode Left Join Master1 C On B.CM1 = C.Code Where B.MasterType = 6 And B.CM6 = {MCCode} And A.RecType = 1 And T1.VchCode = {VchCode} And T1.QStatus = 1 Group by A.MasterCode2, B.[Name], B.[CM1], C.[Name] Having Sum(A.Value1) >= 0.01) A Outer Apply(Select ISNULL(Sum(S.Qty),0) as Stock From (Select S.D1 as Qty From Tran4 S Where S.MasterCode1 = A.ItemCode And S.MasterCode2 = {MCCode} Union All Select IsNull(Sum(S.Value1), 0) as Qty From Tran2 S Where S.MasterCode1 = A.ItemCode And S.MasterCode2 = {MCCode} And S.RecType = 2 And S.[Date] <= GetDate()) S) STK";
                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        list.Add(new STQuotItemsDt
                        {
                            ItemCode = Convert.ToInt32(item["ItemCode"]),
                            ItemName = clsMain.MyString(item["ItemName"]).Trim(),
                            UCode = Convert.ToInt32(item["UCode"]),
                            UName = clsMain.MyString(item["UName"]).Trim(),
                            Qty = Convert.ToDecimal(item["Qty"]),
                            Price = Convert.ToDecimal(item["Price"]),
                            Stock = Convert.ToDecimal(item["Stock"]),
                        });
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found !!!", Data = list };
                }

            }
            catch (Exception ex)
            {
                return new { Status = -1, Msg = ex.Message.ToString(), Data = list };
            }
            return new { Status = 1, Msg = "Success", Data = list };
        }

        [HttpGet]
        public dynamic GetPackerList(string CompCode, string Fy)
        {
            List<UnknowList> plist = new List<UnknowList>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, Fy);
                SQLHELPER conobj = new SQLHELPER(constr); int sno = 1;

                string sql = $"Select IsNull([User], 0) as Name From ESUserMapping Where [UType] = 5 Group By [User] Order By [User]";
                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        plist.Add(new UnknowList
                        {
                            Code = Convert.ToInt32(sno++),
                            Name = clsMain.MyString(item["Name"])
                        });
                    }
                }
                else
                {
                    throw new Exception("Data Not Found !!!");
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), Data = plist };
            }
            return new { Status = 1, Msg = "Success", Data = plist };
        }

        [HttpPost]
        public dynamic SaveStockTransferData(STVchDetail obj, string CompCode, string Fy)
        {
            object VchCode = 0;
            int STBusyCode = 0;
            int Status = 0;
            string StatusStr = string.Empty;
            string errMsg = string.Empty;
            CFixedInterface FI = new CFixedInterface();

            try
            {
                BusyVoucher BusyVch = new BusyVoucher();
                string constr = GetConnectionString(Provider, CompCode, Fy);
                SQLHELPER conobj = new SQLHELPER(constr);
                AutoOrderDT S_DT = GetVoucherConfigDetails(constr, 1);
                string seriesname = S_DT?.SeriesName; string tmcname = S_DT?.Name1;
                string vchNo = GetVchNo(constr, Convert.ToInt32(109), Convert.ToInt32(2), out int autoNo);
                //string formattedDate = DateTime.Now.ToString("dd-MM-yyyy"); // Format the date as 'YYYY-MM-DD hh:mm:ss'
                double InvAmount = 0; string xmlstr = ""; STVchDetail NewInv = obj;

                // Generate Stock Tranfer XML
                xmlstr = GetStockTransferXML(5, NewInv, clsMain.MyString(vchNo), clsMain.MyString(seriesname), clsMain.MyString(tmcname), ref InvAmount, constr);

                bool Connect = false; FI.CloseDB();
                Connect = FI.OpenCSDBForYear(BusyPath, ServerName, SUserName, SPassword, CompCode, Convert.ToInt16(Fy));

                if (!Connect)
                {
                    throw new Exception("Unable To Connect To Company");
                }

                // Save Stock Transfer Invoice
                if (!SaveVoucherFromXML(5, false, xmlstr, ref VchCode, FI, out errMsg))
                {
                    throw new Exception(errMsg);
                }

                STBusyCode = clsMain.MyInt(VchCode);

                string XmlData = CreateXML(obj.STItemDetails);

                // Define parameters with @ prefix
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@BusyVchCode", SqlDbType.Int) { Value = STBusyCode },
                new SqlParameter("@VchCode", SqlDbType.Int) { Value = obj.VchCode },
                new SqlParameter("@OrderId", SqlDbType.Int) { Value = obj.OrderId },
                new SqlParameter("@OrderNo", SqlDbType.VarChar, 40) { Value = obj.OrderNo },
                new SqlParameter("@VchNo", SqlDbType.VarChar, 40) { Value = vchNo },
                new SqlParameter("@AutoVchNo", SqlDbType.Int) { Value = autoNo },
                new SqlParameter("@AccCode", SqlDbType.Int) { Value = obj.AccCode },
                new SqlParameter("@AccName", SqlDbType.VarChar, 100) { Value = obj.AccName },
                new SqlParameter("@MCCode1", SqlDbType.Int) { Value = obj.MCCode },
                new SqlParameter("@MCCode2", SqlDbType.Int) { Value = S_DT?.MasterCode1 },
                new SqlParameter("@Series", SqlDbType.Int) { Value = S_DT?.SeriesCode },
                new SqlParameter("@Mobile", SqlDbType.VarChar, 50) { Value = obj.Mobile },
                new SqlParameter("@Remarks", SqlDbType.VarChar, -1) { Value = obj.Remarks},
                new SqlParameter("@TQty", SqlDbType.Float) { Value = obj.TQty },
                new SqlParameter("@TAmt", SqlDbType.Float) { Value = obj.TAmt },
                new SqlParameter("@NetAmt", SqlDbType.Float) { Value = obj.NetAmt },
                new SqlParameter("@Users", SqlDbType.VarChar, 50) { Value = obj.Users },
                new SqlParameter("@STItemDetails", SqlDbType.NVarChar, -1) { Value = XmlData }
                };

                // Execute the stored procedure
                DataTable result = conobj.getTable("[sp_SaveStockTransferTran]", parameters);

                if (result != null && result.Rows.Count > 0)
                {
                    Status = Convert.ToInt32(result.Rows[0]["Status"]);
                    StatusStr = result.Rows[0]["Msg"].ToString();
                }

                // Check the status returned by the stored procedure
                if (Status == 0)
                {
                    if (STBusyCode > 0)
                    {
                        DeleteVoucher(FI, STBusyCode, out errMsg);
                    }
                    throw new Exception(StatusStr + "," + errMsg);
                }
            }
            catch (Exception ex)
            {
                if (STBusyCode > 0) { DeleteVoucher(FI, STBusyCode, out errMsg); };
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = Status, Msg = StatusStr, VchCode = STBusyCode };
        }

        [HttpGet]
        public dynamic GetStockTranferVchDetails(string CompCode, string FY, string MCCode, string StartDate, string EndDate)
        {
            List<GetSTVchDT> slist = new List<GetSTVchDT>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr);
                string formattedStartDate = string.IsNullOrEmpty(StartDate) ? "" : Busyhelper.FormatDate(StartDate);
                string formattedEndDate = string.IsNullOrEmpty(EndDate) ? "" : Busyhelper.FormatDate(EndDate);

                string sql = $"Select A.[VchCode], ISNULL(A.[VCHNO], '') as VchNo, CONVERT(VARCHAR, A.[DATE], 105) as VchDate, ISNULL(A.[MasterCode1], 0) as AccCode, ISNULL(B.[Name], '') as AccName, ISNULL(B.[Mobile], '') as Mobile, ISNULL(A.[TotQty], 0) as TotQTy, ISNULL(A.[TotAmt], 0) as TotAmt, ISNULL(A.[NetAmount], 0) as NetAmt, ISNULL(A.[BusyVchCode], 0) as BusyVchCode, ISNULL(A.[Remarks], '') as Remarks From ESJSLTRAN1 A INNER JOIN ESJSLCustomer B ON A.MasterCode1 = B.Code Where A.[VchType] = 109 And [MasterCode2] = {MCCode}";
                if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And [Date] >= '{formattedStartDate}' And [Date] <= '{formattedEndDate}'";
                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        slist.Add(new GetSTVchDT
                        {
                            VchCode = Convert.ToInt32(item["VchCode"]),
                            VchNo = item["VchNo"].ToString().Trim(),
                            Date = item["VchDate"].ToString().Trim(),
                            AccCode = Convert.ToInt32(item["AccCode"]),
                            AccName = clsMain.MyString(item["AccName"]).Trim(),
                            Mobile = clsMain.MyString(item["Mobile"]).Trim(),
                            TQty = Convert.ToDecimal(item["TotQty"]),
                            TAmt = Convert.ToDecimal(item["TotAmt"]),
                            NetAmt = Convert.ToDecimal(item["NetAmt"]),
                            Remarks = clsMain.MyString(item["Remarks"]),
                            BusyVchCode = Convert.ToInt32(item["BusyVchCode"]),
                        });
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found !!!", Data = slist };
                }
            }
            catch (Exception ex)
            {
                return new { Status = -1, Msg = ex.Message.ToString(), Data = slist };
            }
            return new { Status = 1, Msg = "Success", Data = slist };
        }

        private bool SaveVoucherFromXML(int VchType, bool Modify, string xmlStr, ref object VchCode, CFixedInterface fi, out string errMsg)
        {
            object err = "";
            bool result = fi.SaveVchFromXML(VchType, xmlStr, ref err, Modify, 0, ref VchCode);
            errMsg = err?.ToString();
            return errMsg == "" ? result : false;
        }

        private void DeleteVoucher(CFixedInterface Fi, int VchCode, out string ErrMsg)
        {
            ErrMsg = "";
            object err = null;
            object BusyVchCode = (object)VchCode;

            try
            {
                Fi.DeleteVchByCode(BusyVchCode, ref err);
                if (err != null)
                {
                    ErrMsg = err?.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
            }
        }

        public string GetStockTransferXML(int VchType, STVchDetail Inv, string VchNo, string VchSeriesName, string TMCName, ref double InvAmount, string ConnectionString)
        {
            string XMLStr = string.Empty;
            try
            {
                BusyVoucher BVch = new BusyVoucher();
                BusyVoucher.StockTransfer ORD = new BusyVoucher.StockTransfer();
                ORD.VchSeriesName = VchSeriesName; //Inv.SeriesName; //BVch.GetMasterCodeToName(ConnStr, SeriesCode).Replace("12", "");
                ORD.Date = DateTime.UtcNow.ToString("dd-MM-yyyy");
                //ORD.Date = DateTime.UtcNow.ToString("10-04-2024")
                ORD.VchNo = VchNo.ToString();
                ORD.VchType = VchType;
                ORD.StockUpdationDate = ORD.Date;
                ORD.MasterName1 = Inv.MCName;
                ORD.MasterName2 = TMCName.ToString();
                ORD.TranCurName = "";
                ORD.TmpVchCode = 0;
                //ORD.TmpVchSeriesCode = 253;
                ORD.ItemEntries = new List<BusyVoucher.ItemDetail>();
                //ORD.BillSundries = new List<BusyVoucher.BSDetail>();
                //ORD.VchOtherInfoDetails = new BusyVoucher.VchOtherInfoDetails();
                //ORD.VchOtherInfoDetails.Narration1 = clsMain.MyString(Inv.Remarks);
                //ORD.VchOtherInfoDetails.Transport = clsMain.MyString(Inv.Transport);
                //ORD.VchOtherInfoDetails.Station = clsMain.MyString(Inv.Station);

                int SrNo = 0;
                foreach (var item in Inv.STItemDetails)
                {
                    BusyVoucher.ItemDetail ID = new BusyVoucher.ItemDetail();
                    SrNo = SrNo + 1;
                    ID.SrNo = SrNo;
                    ID.VchType = ORD.VchType;
                    ID.Date = ORD.Date;
                    ID.VchNo = VchNo.ToString();
                    ID.ItemName = item.ItemName.ToString();
                    ID.UnitName = item.UName.ToString();
                    ID.ConFactor = 1;
                    ID.Qty = clsMain.MyDouble(item.Qty);
                    ID.QtyMainUnit = clsMain.MyDouble(ID.Qty);
                    ID.QtyAltUnit = 0;
                    ID.ConFactor = 0;
                    ID.ListPrice = 0;
                    ID.Price = (double)item.Price;
                    ID.Amt = clsMain.MyDouble(ID.Price * ID.Qty);
                    ID.PriceAltUnit = 0;
                    if (ID.QtyAltUnit != 0) { ID.PriceAltUnit = clsMain.MyDouble((clsMain.MyDouble(ID.Amt) / clsMain.MyDouble(ID.QtyAltUnit)).ToString("0.00")); }
                    InvAmount = InvAmount + clsMain.MyDouble(ID.Amt);
                    ID.TmpVchCode = 0;
                    ID.MC = ORD.MasterName2;
                    ID.ItemDescInfo = new BusyVoucher.ItemDescInfo();
                    ORD.ItemEntries.Add(ID);
                }
                //ORD.TmpTotalAmt = InvAmount;
                XMLStr = CreateXML(ORD).Replace("<?xml version=\"1.0\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            }
            catch
            {
                return "";
            }
            return XMLStr;
        }

        public string GetReceiptVoucherXML(int VchType, VchReceipt Inv, string ConStr)
        {
            string XMLStr = "";
            try
            {
                int SrNo = 0;
                int RefSrNo = 0;
                BusyVoucher BVch = new BusyVoucher();
                Receipt ORD = new Receipt();

                ORD.VchSeriesName = Inv.SeriesName;
                ORD.Date = DateTime.UtcNow.ToString("dd-MM-yyyy");
                ORD.VchNo = "";
                ORD.VchType = VchType;
                ORD.StockUpdationDate = ORD.Date;

                ORD.TranCurName = "Rs.";
                ORD.tmpVchCode = 0;
                ORD.tmpVchSeriesCode = Inv.SeriesCode;

                ORD.AccEntries = new List<AccDetail>();

                ORD.VchOtherInfoDetails = new VchOtherInfoDetails();
                ORD.TranType = 0;

                if (Inv.PDC == 1)
                {
                    ORD.TranType = 1;
                    ORD.PDCDate = Inv.PDCDate;
                }

                ORD.VchOtherInfoDetails.Narration1 = Inv.Remarks;
                SrNo = 0;

                SrNo = SrNo + 1;
                AccDetail ID = new AccDetail();
                ID.SrNo = SrNo;
                ID.AccountName = Inv.PartyName;
                ID.AmountType = 2;
                ID.AmtMainCur = Inv.Amount;
                ID.CashFlow = Inv.Amount;
                ID.ShortNar = "";
                ID.Date = ORD.Date;
                if (Inv.Mode == 2)
                {
                    ID.ShortNar = Inv.ChequeNo + "|" + Inv.ChequeDt;
                }
                ID.VchType = ORD.VchType;
                //ID.BillRefs = new List<BillDetails>();
                //RefSrNo = 0;
                //foreach (var item in Inv.BillByBill)
                //{
                //    RefSrNo = RefSrNo + 1;
                //    BillDetails billRefs = new BillDetails();
                //    billRefs.ItemSrNo = ID.SrNo;
                //    billRefs.SrNo = RefSrNo;
                //    billRefs.Method = 2;
                //    billRefs.RefNo = item.RefNo;
                //    billRefs.Date = ORD.Date;
                //    billRefs.DueDate = item.DueDate;
                //    billRefs.Value1 = item.Amount;
                //    billRefs.tmpRefCode = item.RefCode;
                //    billRefs.tmpMasterCode1 = BVch.GetMasterNameToCode(ConStr, ID.AccountName, 2);
                //    ID.BillRefs.Add(billRefs);
                //}
                ORD.AccEntries.Add(ID);

                SrNo = SrNo + 1;
                ID = new AccDetail();
                ID.SrNo = SrNo;
                if (Inv.Mode == 1)
                {
                    ID.AccountName = Inv.CashAcc;
                }
                else if (Inv.Mode == 2)
                {
                    ID.AccountName = Inv.BankAcc;
                }
                ID.AmountType = 1;
                ID.AmtMainCur = Inv.Amount;
                ID.CashFlow = Inv.Amount;

                ID.Date = ORD.Date;
                ID.ShortNar = "";
                if (Inv.Mode == 2)
                {
                    ID.ShortNar = Inv.ChequeNo + "|" + Inv.ChequeDt;
                }
                ID.VchType = ORD.VchType;

                ORD.AccEntries.Add(ID);

                ORD.PendingBillDetails = new List<BillDetail>();
                if (Inv.BillByBill.Count > 0)
                {
                    RefSrNo = 0;
                    BillDetail billDT = new BillDetail();

                    billDT.MasterName1 = Inv.PartyName;
                    billDT.tmpMasterCode1 = BVch.GetMasterNameToCode(2, ConStr, Inv.PartyName, 2);
                    billDT.BillRefDT = new List<BillRefs>();
                    foreach (var item in Inv.BillByBill)
                    {
                        RefSrNo = RefSrNo + 1;
                        BillRefs billRefs = new BillRefs();
                        billRefs.ItemSrNo = 1;
                        billRefs.SrNo = RefSrNo;
                        billRefs.Method = 2;
                        billRefs.RefNo = item.RefNo;
                        billRefs.Date = ORD.Date;
                        billRefs.DueDate = item.DueDate;
                        billRefs.Value1 = item.Amount;
                        billRefs.tmpRefCode = item.RefCode;
                        billRefs.tmpRecType = 1;
                        billRefs.tmpMasterCode1 = billDT.tmpMasterCode1;
                        billDT.BillRefDT.Add(billRefs);
                    }
                    ORD.PendingBillDetails.Add(billDT);
                }
                XMLStr = CreateXML(ORD).Replace("<?xml version=\"1.0\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "").Replace("<BillRefDT>", "").Replace("</BillRefDT>", "");
            }
            catch
            {
                return "";
            }

            return XMLStr;
        }

        [HttpGet]
        public dynamic GetGodownVerificationVchList(string CompCode, string FY)
        {
            List<GetStockTransferQuotDt> VList = new List<GetStockTransferQuotDt>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr);

                string sql = $"Select A.[VchCode], IsNull(A.[VchNo], '') as VchNo, CONVERT(VARCHAR, A.[Date], 105) as VchDate, IsNull(A.[MasterCode1], 0) as AccCode,  IsNull(B.[Name], '') as AccName, IsNull(B.[Mobile], '') as Mobile, IsNull(B.[Email], '') as Email, IsNull(B.[GSTIN], '') as GSTIN, IsNull(B.[Address], '') as Address From ESJSLTran1 A Left Join ESJSLCustomer B On A.MasterCode1 = B.Code Where ([Verification] Is Null OR [Verification] = 0) And [VchType] = 109 And A.Cancelled <> 1 Order By A.[VchCode], A.[Date]"; //And [I1] = {MCCode}
                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        VList.Add(new GetStockTransferQuotDt
                        {
                            VchCode = Convert.ToInt32(item["VchCode"]),
                            VchNo = clsMain.MyString(item["VchNo"]).Trim(),
                            VchDate = Convert.ToString(item["VchDate"]).Trim(),
                            AccCode = Convert.ToInt32(item["AccCode"]),
                            AccName = clsMain.MyString(item["AccName"]).Trim(),
                            CMobile = clsMain.MyString(item["Mobile"]).Trim(),
                            CEmail = clsMain.MyString(item["Email"]).Trim(),
                            CGSTIN = clsMain.MyString(item["GSTIN"]).Trim(),
                            CAddress = clsMain.MyString(item["Address"]).Trim(),
                        });
                    }
                }
                else
                {
                    throw new Exception("Data Not Found !!!");
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), };
            }
            return new { Status = 1, Msg = "Success", Data = VList };
        }

        [HttpGet]
        public dynamic GetGodownVerificationVchItemDetails(string CompCode, string FY, int VchCode)
        {
            List<STQuotItemsDt> VIList = new List<STQuotItemsDt>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr);

                string sql = $"Select IsNull(B.[ItemCode], 0) as ItemCode, IsNull(M.[Name], '') as ItemName, IsNull(M.[CM1], 0) as UCode, IsNull(M1.[Name], '') as UName, B.[Qty], B.[Price], B.[Amount] From ESJSLTran1 A Inner Join ESJSLTran2 B On A.VchCode = B.VchCode And A.VchType = B.VchType And B.RecType = 1 Left Join Master1 M On B.ItemCode = M.Code And M.MasterType = 6 Left Join Master1 M1 On M.CM1 = M1.Code And M1.MasterType = 8 Where A.Cancelled <> 1 And (A.[Verification] Is Null Or A.[Verification] = 0) And A.[VchType] = 109 And A.[VchCode] = {VchCode}";
                DataTable DT1 = conobj.getTable(sql);
                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        VIList.Add(new STQuotItemsDt
                        {
                            ItemCode = Convert.ToInt32(item["ItemCode"]),
                            ItemName = clsMain.MyString(item["ItemName"]).Trim(),
                            UCode = Convert.ToInt32(item["UCode"]),
                            UName = clsMain.MyString(item["UName"]).Trim(),
                            Qty = Convert.ToDecimal(item["Qty"]),
                            Price = Convert.ToDecimal(item["Price"]),
                            Amount = Convert.ToDecimal(item["Amount"])
                        });
                    }
                }
                else
                {
                    throw new Exception("Data Not Found !!!");
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = VIList };
        }

        [HttpPost]
        public dynamic SaveGodownVerification(STVerificationVchDT obj, string CompCode, string FY)
        {
            int Status = 0; string StatusStr = string.Empty;
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr);
                string xml = CreateXML(obj?.STVItemDetails);

                // Define parameters with @ prefix
                SqlParameter[] parameter = new SqlParameter[]
                {
                new SqlParameter("@VchCode", SqlDbType.Int) { Value = obj?.VchCode },
                new SqlParameter("@VchType", SqlDbType.Int) { Value = 109 },
                new SqlParameter("@Status", SqlDbType.Int) { Value = 1 },
                new SqlParameter("@STVItemDetails", SqlDbType.NVarChar, -1) { Value = xml }
                };

                DataTable DT1 = conobj.getTable("[sp_SaveGodownTransferVerification]", parameter);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    Status = Convert.ToInt32(DT1.Rows[0]["Status"]);
                    StatusStr = DT1.Rows[0]["Msg"].ToString();
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), };
            }
            return new { Status = Status, Msg = StatusStr };
        }

        [HttpGet]
        public dynamic GetGodownVerificationDashboardVchList(string CompCode, string FY)
        {
            List<VerificationVchList> VList = new List<VerificationVchList>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                string sql = $"Select VchCode, IsNull(A.[VchNo], '') as VchNo, IsNull(A.[Date], '') as [Date], IsNull(A.[MasterCode1], 0) as AccCode, IsNull(M1.[Name], '') as AccName, IsNull(M1.[Mobile], '') as Mobile,  IsNull(M1.[Email], '') as Email, IsNull(M1.[GSTIN], '') as GSTIN, IsNull(M1.[Address], '') as Address, IsNull(M1.[Email], '') as Mobile, IsNull(A.[Verification], 0) as [Status], (CASE WHEN A.[Verification] = 1 THEN 'Verified' ELSE 'Pending For Verification' END) as SName From ESJSLTRAN1 A INNER JOIN ESJSLCustomer M1 On A.MasterCode1 = M1.Code Where VchType = 109 And Cancelled <> 1";

                DataTable DT1 = new SQLHELPER(constr).getTable(sql);
                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {

                        VList.Add(new VerificationVchList
                        {
                            VchCode = Convert.ToInt32(item["VchCode"]),
                            VchNo = clsMain.MyString(item["VchNo"]),
                            Date = clsMain.MyString(item["Date"]),
                            AccCode = Convert.ToInt32(item["AccCode"]),
                            AccName = clsMain.MyString(item["AccName"]),
                            Mobile = clsMain.MyString(item["Mobile"]),
                            Email = clsMain.MyString(item["Email"]),
                            GSTIN = clsMain.MyString(item["GSTIN"]),
                            Address = clsMain.MyString(item["Address"]),
                            Status = Convert.ToInt32(item["Status"]),
                            SName = clsMain.MyString(item["SName"]),
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = VList };
        }

        [HttpGet]
        public dynamic GetPendingPackingList(string CompCode, string FY)
        {
            List<PendingPackingDT> clist = new List<PendingPackingDT>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr);

                //string sql = $"Select IsNull(A.MasterCode1, 0) As AccCode, IsNull(C.[Name], '') as AccName, IsNull(C.Mobile, '') as Mobile, IsNull(C.Email, '') as Email, IsNull(C.GSTIN, '') as GSTIN,  IsNull(C.[Address], '') as Address, IsNull(Sum(A.Value1), 0) as TQty from ESJSLRefTran A inner Join ESJSLCustomer C On A.MasterCode1 = C.Code Inner Join ESJSLTran1 T1 On A.RefCode = T1.OrderId And A.MasterCode1 = T1.MasterCode1 Where A.RecType = 2 And T1.VchType = 109 And T1.Verification = 1 And T1.Cancelled <> 1 Group by A.MasterCode1, C.[Name], C.Mobile, C.Email, C.GSTIN, C.[Address] Having Sum(A.Value1) >= 0.01";
                string sql = $"Select IsNull(B.MasterCode1, 0) as AccCode, IsNull(C.[Name], '') as AccName, IsNull(C.Mobile, '') as Mobile, IsNull(C.Email, '') as Email, IsNull(C.GSTIN, '') as GSTIN,  IsNull(C.[Address], '') as Address, Sum(B.Value1) as TQty From (Select OrderId, OrderNo, MasterCode1 From ESJSLTRAN1 T1 Where T1.VchType = 109 And T1.Verification = 1 And T1.Cancelled <> 1 Group By OrderId, OrderNo, MasterCode1) A INNER JOIN ESJSLRefTran B ON A.OrderId = B.RefCode And B.RecType = 2 And A.MasterCode1 = B.MasterCode1 INNER JOIN ESJSLCustomer C On B.MasterCode1 = C.Code GROUP BY B.MasterCode1, C.[Name], C.Mobile, C.Email, C.GSTIN, C.[Address] Having Sum(B.Value1) >= 0.01 Order By B.MasterCode1";
                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        clist.Add(new PendingPackingDT
                        {
                            AccCode = Convert.ToInt32(item["AccCode"]),
                            AccName = clsMain.MyString(item["AccName"]),
                            Mobile = clsMain.MyString(item["Mobile"]),
                            Email = clsMain.MyString(item["Email"]),
                            Address = clsMain.MyString(item["Address"]),
                            GSTIN = clsMain.MyString(item["GSTIN"]),
                            TQty = Convert.ToDouble(item["TQty"])
                        });
                    }
                }
                else
                {
                    throw new Exception("Data Not Found !!!");
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), Data = clist };
            }
            return new { Status = 1, Msg = "Success", Data = clist };
        }

        [HttpGet]
        public dynamic GetPendingPackingItemsDetails(string CompCode, string FY, int AccCode)
        {
            List<PendingPackingItemsDT> PList = new List<PendingPackingItemsDT>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY); int SNo = 1;
                //ROW_NUMBER() OVER(ORDER BY RefCode) as SNo,
                //string sql = $"Select A.RefCode, A.RefNo, A.MasterCode2 as ItemCode, IsNull(M.[Name], '') as ItemName, Sum(A.Value1) as Qty, (Select Top 1 Price From ESJSLTran2 Where VchCode = T1.VchCode And ItemCode = A.MasterCode2 And RecType = 1) as Price From ESJSLRefTran A Inner Join ESJSLTran1 T1 On A.NewRefVchCode = T1.VchCode Left Join Master1 M On A.MasterCode2 = M.Code And M.MasterType = 6 Where T1.VchType = 109 And T1.MasterCode1 = {AccCode} And T1.Verification = 1 And T1.Cancelled <> 1 And A.RecType = 2 Group By  A.RefCode, A.RefNo, A.MasterCode2, M.[Name] Having Sum(A.Value1) >= 0.01 Order By A.RefNo, A.MasterCode2";
                string sql = $"Select B.RefCode as OrderId, B.RefNo as OrderNo, B.MasterCode2 as ItemCode, IsNull(M.[Name], '') as ItemName, Sum(B.Value1) as Qty, IsNull((Select Top 1 Price From ESJSLTran2 Where B.RefCode = VchCode And VchType = 108 And B.MasterCode2 = ItemCode And RecType = 1), 0) as Price From (Select OrderId, OrderNo, MasterCode1 From ESJSLTRAN1 T1 Where T1.VchType = 109 And T1.Verification = 1 And T1.Cancelled <> 1 And T1.MasterCode1 = {AccCode} Group By OrderId, OrderNo, MasterCode1) A INNER JOIN ESJSLRefTran B ON A.OrderId = B.RefCode And B.RecType = 2 And A.MasterCode1 = B.MasterCode1 Left Join Master1 M On B.MasterCode2 = M.Code And M.MasterType = 6 GROUP BY B.RefCode, B.RefNo, B.MasterCode2, M.[Name] Having Sum(B.Value1) >= 0.01 Order By B.RefCode, B.RefNo";
                DataTable DT1 = new SQLHELPER(constr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        PList.Add(new PendingPackingItemsDT
                        {
                            SNo = Convert.ToInt32(SNo++),
                            //VchCode = Convert.ToInt32(item["VchCode"]),
                            //VchNo = clsMain.MyString(item["VchNo"]),
                            ItemCode = Convert.ToInt32(item["ItemCode"]),
                            ItemName = clsMain.MyString(item["ItemName"]),
                            Qty = Convert.ToDecimal(item["Qty"]),
                            Price = Convert.ToDecimal(item["Price"]),
                            Amount = Convert.ToDecimal(Convert.ToDecimal(item["Qty"]) * Convert.ToDecimal(item["Price"])),
                            OrderId = Convert.ToInt32(item["OrderId"]),
                            OrderNo = clsMain.MyString(item["OrderNo"]),
                        });
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found !!!", Data = PList };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = PList };
        }

        [HttpPost]
        public dynamic SavePackingData(PackingSaved Obj, string CompCode, string FY)
        {
            int Status = 0; string StatusStr = string.Empty; string BoxNumber = string.Empty;
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                string XML = CreateXML(Obj?.BItemDetails);
                string VchNo = GetVchNo(constr, Convert.ToInt32(110), Convert.ToInt32(3), out int AutoNo);

                SqlParameter[] parameter = new SqlParameter[]
                {
                new SqlParameter("@VchCode", SqlDbType.Int) {Value = Obj.VchCode},
                new SqlParameter("@VchNo", SqlDbType.VarChar, 40) { Value = VchNo },
                new SqlParameter("@VchType", SqlDbType.Int) {  Value = 110},
                new SqlParameter("@AutoVchNo", SqlDbType.Int) { Value = AutoNo },
                new SqlParameter("@MasterCode1", SqlDbType.Int) { Value = Obj.AccCode },
                new SqlParameter("@Mobile", SqlDbType.VarChar, 20) { Value = Obj.Mobile},
                new SqlParameter("@TQty", SqlDbType.Float) { Value = Obj.TotQty },
                new SqlParameter("@TAmt", SqlDbType.Float) { Value = Obj.TotAmt },
                new SqlParameter("@NetAmt", SqlDbType.Float) { Value = Obj.NetAmt },
                new SqlParameter("@Users", SqlDbType.VarChar, 50) { Value = Obj.Users },
                new SqlParameter("@ItemDetails", SqlDbType.NVarChar, -1) { Value = XML }
                };

                DataTable DT1 = new SQLHELPER(constr).getTable("[sp_SaveBoxPacking]", parameter);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    Status = Convert.ToInt32(DT1.Rows[0]["Status"]);
                    StatusStr = clsMain.MyString(DT1.Rows[0]["Msg"]);
                    BoxNumber = clsMain.MyString(DT1.Rows[0]["BoxNumber"]);
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = Status, Msg = StatusStr, BoxNumber = BoxNumber };
        }

        [HttpGet]
        public dynamic GetPackingVchListDetails(string CompCode, string FY, string StartDate, string EndDate, int AccCode, int Status)
        {
            List<GetPackingVch> PList = new List<GetPackingVch>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER obj = new SQLHELPER(constr);
                string formattedStartDate = string.IsNullOrEmpty(StartDate) ? "" : Busyhelper.FormatDate(StartDate);
                string formattedEndDate = string.IsNullOrEmpty(EndDate) ? "" : Busyhelper.FormatDate(EndDate);
                int QStatus = Status == 1 ? 0 : Status == 2 ? 1 : Status == 3 ? 2 : 0;

                string sql = $"Select A.[VchCode], IsNull(A.[VchNo], '') as VchNo, IsNull(CONVERT(VARCHAR, A.[Date], 105), '') as VchDate, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(M.[Name], '') as AccName, IsNull(M.[Mobile], '') as Mobile, IsNull(A.[TotQty], 0) as TotQty, IsNull(A.TotAmount, 0) as TotAmt, IsNull(A.NetAmount, 0) as NetAmt, IsNull(A.[Status], 0) as [DStatus], CASE WHEN A.[Status] = 0 THEN 'Pending' WHEN A.[Status] = 1 THEN 'Dispatched' WHEN A.[Status] = 2 THEN 'Cancelled' ELSE 'Pending' END As [Status] , IsNull(A.[DispatchedCode], 0) as InvVchCode,  IsNull(CONVERT(VARCHAR, A.[DispatchedDate], 105), '') as InvVchDate, IsNull(A.[DispatchedNo], '') as InvVchNo From ESJSLPacking A INNER JOIN ESJSLCustomer M ON A.MasterCode1 = M.Code Where A.VchType = 110";
                if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And A.[Date] >= '{formattedStartDate}' And A.[Date] <= '{formattedEndDate}'";
                if (AccCode != 0) sql += $"A.[MasterCode1] = {AccCode}"; if (Status != 0) sql += $"A.[Status] = {QStatus}"; sql += " Order By A.[VchCode] DESC, A.[Date] DESC";
                DataTable DT1 = obj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        PList.Add(new GetPackingVch
                        {
                            VchCode = Convert.ToInt32(item["VchCode"]),
                            VchDate = item["VchDate"].ToString(),
                            VchNo = item["VchNo"].ToString(),
                            AccCode = Convert.ToInt32(item["AccCode"]),
                            AccName = clsMain.MyString(item["AccName"]),
                            Mobile = clsMain.MyString(item["Mobile"]),
                            TQty = Convert.ToDecimal(item["TotQty"]),
                            TAmt = Convert.ToDecimal(item["TotAmt"]),
                            NetAmt = Convert.ToDecimal(item["NetAmt"]),
                            Status = clsMain.MyString(item["Status"]),
                            InvVchCode = Convert.ToInt32(item["InvVchCode"]),
                            InvVchDate = clsMain.MyString(item["InvVchDate"]),
                            InvVchNo = clsMain.MyString(item["InvVchNo"])
                        });
                    }
                }
                else
                {
                    throw new Exception("Data Not Found !!!");
                }
            }
            catch (Exception err)
            {
                return new { Status = 0, Msg = err.Message.ToString(), Data = PList };
            }
            return new { Status = 1, Msg = "Success", Data = PList };
        }

        [HttpPost]
        public dynamic CreateParty(CCustomer obj, string CompCode, string FY)
        {
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                int Code = Convert.ToInt32(obj.Code); string sql = string.Empty;
                string formattedDate = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");

                if (Code == 0)
                {
                    sql = $"Select Top 1 * From ESJSLCustomer Where Mobile = '{obj?.Mobile.Replace("'", "''")}'";
                }
                else
                {
                    sql = $"Select Top 1 * From ESJSLCustomer Where Mobile = '{obj?.Mobile.Replace("'", "''")}' And Code <> {Code}";
                }
                DataTable DT1 = new SQLHELPER(constr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    throw new Exception("Mobile No. Already Exists For The Some Customer !!!");
                }
                else
                {
                    if (Code == 0)
                    {
                        sql = $"Insert Into ESJSLCustomer ([Name], [Email], [Mobile], [GSTIN], [CM1], [CM2], [CM3], [CM4], [CM5], [Address], [OrgName], [CustType], [CreatedBy], [CreationTime]) Values ('{obj.Name.Replace("'", "''")}', '{obj.Email.Replace("'", "''")}', '{obj.Mobile.Replace("'", "''")}', '{obj.GSTNO.Replace("'", "''")}', {obj.CNCode}, {obj.STCode}, {obj.CTCode}, 0, 0, '{obj.Address.Replace("'", "''")}', '{obj.OrgName}', {obj.CustType}, '{obj.Users}', '{formattedDate}')";
                    }
                    else
                    {
                        sql = $"Update ESJSLCustomer Set [Name] = '{obj.Name.Replace("'", "''")}', [Mobile] = '{obj.Mobile.Replace("'", "''")}', [Email] = '{obj.Email.Replace("'", "''")}', [GSTIN] = '{obj.GSTNO.Replace("'", "''")}', [OrgName] = '{obj.OrgName.Replace("'", "''")}', [CM1] = {obj.CNCode}, [CM2] = {obj.STCode}, [CM3] = {obj.CTCode}, [Address] = '{obj.Address.Replace("'", "''")}', [CustType] = {obj.CustType}, [ModifiedBy] = '{obj.Users}', [ModificationTime] = '{formattedDate}' Where Code = {Code}";
                    }
                    int r = new SQLHELPER(constr).ExecuteSQL(sql);
                    if (r > 0)
                    {
                        var CustList = GetCustomerDetails(obj?.Mobile, CompCode, FY);
                        var CustDet = CustList?.Data as List<GetCustomerDT> ?? new List<GetCustomerDT>();
                        return new { Status = 1, Msg = Code == 0 ? "Customer Successfully Created !!!" : "Customer Successfully Modified !!!", Data = CustDet };
                    }
                    else
                    {
                        throw new Exception("Unable To Connect To Company !!!");
                    }
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString().Trim() };
            }
        }

        [HttpGet]
        public dynamic GetCustomerDetails(string CompCode, string FY, string Mobile)
        {
            List<GetCustomerDT> CList = new List<GetCustomerDT>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY); string sql = string.Empty; DataTable DT1 = new DataTable();

                sql = $"Select * From ESJSLCUSTOMER WHERE [MOBILE] = '{Mobile.Replace("'", "''").ToString().Trim()}' AND DEACTIVE = 1";
                DT1 = new SQLHELPER(constr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    throw new Exception("Customer account is deactivated. Please contact admin to reactivate your account.");
                }

                sql = $"Select IsNull(A.[Code], 0) as Code, IsNull(A.[Name], '') as Name, IsNull(A.[Mobile], '') as Mobile, CASE WHEN A.CM2 = B.StateCode THEN 2 ELSE 1 END As TaxType From ESJSLCUSTOMER A LEFT JOIN ESJSLCompanyRegistration B ON A.CM2 = B.StateCode WHERE A.[MOBILE] = '{Mobile.Replace("'", "''").ToString().Trim()}'";
                DT1 = new SQLHELPER(constr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        CList.Add(new GetCustomerDT
                        {
                            Code = Convert.ToInt32(item["Code"]),
                            Name = item["Name"].ToString().Trim(),
                            Mobile = item["Mobile"].ToString().Trim(),
                            GstType = Convert.ToInt32(item["TaxType"]),
                        });
                    }
                }
                else
                {
                    throw new Exception("Customer Not Found !!!");
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), Data = CList };
            }
            return new { Status = 1, Msg = "Success", Data = CList };
        }

        [HttpGet]
        public dynamic GetCustomerDetailsList(string CompCode, string FY, int Code, int CType)
        {
            List<Customers> CList = new List<Customers>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                string sql = $"Select IsNull(A.[Code], 0) as Code, IsNull(A.[Name], '') as Name, IsNull(A.[Mobile], '') as Mobile, IsNull(A.[Email], '') as Email, IsNull(A.[GSTIN], '') as GSTIN, IsNull(A.[OrgName], '') as FirmName, IsNull(A.[CM1], 0) as CNCode, IsNull(B.[Name], '') as CNName, IsNull(A.[CM2], 0) as STCode, IsNull(C.[Name], '') as STName, IsNull(A.[CM3], 0) as CTCode, IsNull(D.[Name], '') as CTName, IsNull(A.[Address], '') as [Address], IsNull(A.[CustType], 0) as CCode, (CASE WHEN IsNull(A.[CustType], 0) = 1 THEN 'B2B A' WHEN IsNull(A.[CustType], 0) = 2 THEN 'B2B B' WHEN IsNull(A.[CustType], 0) = 3 THEN 'B2C' END) as CName, IsNull(A.[Deactive], 0) as Deactive From ESJSLCUSTOMER A Left Join ESJSLCountryMaster B On A.CM1 = B.Code And B.MasterType = 1 Left Join ESJSLCountryMaster C On A.CM2 = C.Code And C.MasterType = 2 Left Join ESJSLCountryMaster D On A.[CM3] = D.[Code] And D.[MasterType] = 3 Where 1=1";
                if (CType != 0) sql += $" And A.[CustType] = {CType} ";
                if (Code != 0) sql += $" And A.[Code] = {Code} ";
                sql += "Order By A.[Name]";
                DataTable DT1 = new SQLHELPER(constr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        CList.Add(new Customers
                        {
                            Code = Convert.ToInt32(item["Code"]),
                            Name = item["Name"].ToString().Trim(),
                            Mobile = item["Mobile"].ToString().Trim(),
                            Email = item["Email"].ToString().Trim(),
                            GSTIN = item["GSTIN"].ToString().Trim(),
                            FirmName = item["FirmName"].ToString().Trim(),
                            CNCode = Convert.ToInt32(item["CNCode"]),
                            CNName = item["CNName"].ToString().Trim(),
                            STCode = Convert.ToInt32(item["STCode"]),
                            STName = item["STName"].ToString().Trim(),
                            CTCode = Convert.ToInt32(item["CTCode"]),
                            CTName = item["CTName"].ToString().Trim(),
                            Address = item["Address"].ToString().Trim(),
                            CCode = Convert.ToInt32(item["CCode"]),
                            CName = item["CName"].ToString().Trim(),
                            Deactive = Convert.ToInt32(item["Deactive"])
                        });
                    }
                }
                else
                {
                    throw new Exception("Customer Details Not Found !!!");
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString(), Data = CList };
            }
            return new { Status = 1, Msg = "Success", Data = CList };
        }

        [HttpGet]
        public dynamic CancelCustomer(string CompCode, string FY, int Code)
        {
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);

                string sql = $"Update ESJSLCustomer SET [DEACTIVE] = 1 WHERE Code = {Code}";
                int DT1 = new SQLHELPER(constr).ExecuteSQL(sql);

                if (DT1 == 1)
                {
                    return new { Status = 1, Msg = "Your Customer Has Been Successfully Deactivated !!!" };
                }
                else
                {
                    throw new Exception("An error occurred while deactivated customer to the database. Please try again later. !!!");
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
        }

        [HttpPost]
        public dynamic VoucherCancellation(dynamic obj, string CompCode, string FY, int TranType, int VchType, int VchCode)
        {
            int Status = 0; string StatusStr = string.Empty;
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY); string sql = string.Empty;
                int BusyVchCode = 0; int Cancelled = 0; int Verification = 0;
                DataTable DT1 = new DataTable(); int DT2 = 0;
                switch (TranType)
                {
                    case 1:
                        // Fully Order Cancellection
                        sql = $"Select Top 1 * From ESJSLRefTran Where VchType = 109 And RefCode = {VchCode} And Method = 2";
                        DT1 = new SQLHELPER(constr).getTable(sql);

                        if (DT1 != null && DT1.Rows.Count > 0)
                        {
                            throw new Exception("This voucher cannot be fully canceled as it has already been adjusted. Please check the voucher status. !!!");
                        }
                        else
                        {
                            sql = $"Update ESJSLTRAN1 SET [CANCELLED] = 1 WHERE VchCode = {VchCode} And VchType = {VchType}";
                            DT2 = new SQLHELPER(constr).ExecuteSQL(sql);

                            if (DT2 == 1)
                            {
                                Status = 1; StatusStr = "Order Is Successfully Cancelled !!!";
                            }
                            else
                            {
                                throw new Exception("An error occurred while canceled voucher. Please try again later !!!");
                            }
                        }
                        break;

                    case 2:
                        // Particial Order cancellection
                        string formattedDate = DateTime.Now.ToString("dd-MMM-yyyy");
                        JObject filterObject = JObject.FromObject(obj);
                        var CancelItems = filterObject["CancelItems"]?.ToObject<List<JObject>>();

                        foreach (var item in CancelItems)
                        {
                            var OrderId = item["OrderId"]?.ToObject<int>() ?? 0;
                            var OrderNo = item["OrderNo"]?.ToObject<string>() ?? "";
                            var AccCode = item["AccCode"]?.ToObject<int>() ?? 0;
                            var ItemCode = item["ItemCode"]?.ToObject<int>() ?? 0;
                            var Qty = item["Qty"]?.ToObject<double>() ?? 0;
                            var Price = item["Price"]?.ToObject<double>() ?? 0;
                            var Amount = item["Amount"]?.ToObject<double>() ?? 0;

                            sql = $"INSERT INTO ESJSLREFTRAN ([RefCode], [RecType], [Method], [VchCode], [VchType], [Date], [RefNo], [MasterCode1], [MasterCode2], [Value1], [Value2], [Value3], [NewRefVchCode], [NewRefVchNo]) VALUES ({OrderId}, 1, 2, -1300, {VchType}, '{formattedDate}', '{OrderNo}', {AccCode}, {ItemCode}, {(Qty * (-1))}, {Price}, {Amount}, -1300, '{OrderNo}')";
                            DT2 = new SQLHELPER(constr).ExecuteSQL(sql);

                            if (DT2 == 0)
                            {
                                throw new Exception("An error occurred while canceled voucher. Please try again later !!!");
                            }
                            else
                            {
                                Status = 1; StatusStr = "Order is successfully cancelled !!!";
                            }
                        }

                        break;

                    case 3:
                        // Stock Tranfer/Godown Trander Cancel Fully
                        JObject CancelVchObj = JObject.FromObject(obj);
                        var OrderId_1 = Convert.ToInt32(CancelVchObj["OrderId"]?.ToObject<int>() ?? 0);
                        var OrderNo_1 = CancelVchObj["OrderNo"]?.ToObject<string>() ?? "";
                        var Order_VchType_1 = Convert.ToInt32(CancelVchObj["OrderVchType"]?.ToObject<int>());
                        dynamic Invobj = new ExpandoObject();

                        //sql = $"Select * From ESJSLTran1 Where VchCode = {VchCode} And VchType = {VchType} And Cancelled <> 1 And ((VchType = 109 AND Verification <> 1) OR (VchType <> 109)) ";
                        sql = $"Select  IsNull(A.Cancelled, 0) as Cancelled, IsNull(A.Verification, 0) as Verification, IsNull(A.VchCode, 0) as VchCode, IsNull(A.VchNo, '') as VchNo, CONVERT(VARCHAR, A.[Date], 105) as VchDate, IsNull(A.BusyVchCode, 0) as BusyVchCode, IsNull(B.[VchType], 0) as BusyVchType, LTRIM(IsNull(B.VchNo, '')) as BusyVchNo, CONVERT(VARCHAR, B.[Date], 105) as BusyVchDate, IsNull(A.MasterCode1, 0) as AccCode, IsNull(M.[Name], 0) as AccName, IsNull(A.MasterCode2, 0) as MCCode1, IsNull(M1.[Name], '') as MCName1, IsNull(A.MasterCode3, 0) as MCCode2, IsNull(M2.[Name], '') as MCName2, IsNull(A.MasterCode4, 0) as SeriesCode, IsNull(SUBSTRING(M3.[Name], 3, 25), '') as SeriesName From ESJSLTran1 A LEFT JOIN Tran1 B On A.BusyVchCode = B.VchCode LEFT JOIN ESJSLCustomer M ON A.MasterCode1 = M.Code LEFT JOIN MASTER1 M1 ON A.MasterCode2 = M1.Code LEFT JOIN MASTER1 M2 ON A.MasterCode3 = M2.Code LEFT JOIN MASTER1 M3 ON A.MasterCode4 = M3.Code Where A.VchCode = {VchCode} And A.VchType = {VchType}";
                        DT1 = new SQLHELPER(constr).getTable(sql);

                        if (DT1 != null && DT1.Rows.Count > 0)
                        {
                            Cancelled = Convert.ToInt32(DT1.Rows[0]["Cancelled"]);
                            Verification = Convert.ToInt32(DT1.Rows[0]["Verification"]);
                            BusyVchCode = Convert.ToInt32(DT1.Rows[0]["BusyVchCode"]);
                            Invobj.MCName1 = clsMain.MyString(DT1.Rows[0]["MCName1"]);
                            Invobj.MCName2 = clsMain.MyString(DT1.Rows[0]["MCName2"]);
                            Invobj.SeriesCode = Convert.ToInt32(DT1.Rows[0]["SeriesCode"]);
                            Invobj.SeriesName = clsMain.MyString(DT1.Rows[0]["SeriesName"]);
                            Invobj.VchDate = clsMain.MyString(DT1.Rows[0]["BusyVchDate"]);
                            Invobj.VchNo = clsMain.MyString(DT1.Rows[0]["BusyVchNo"]);
                            Invobj.VchType = Convert.ToInt32(DT1.Rows[0]["BusyVchType"]);
                            Invobj.ItemDetails = new JArray();
                        }

                        if (Verification == 1) throw new Exception("This voucher cannot canceled as it has already been verified. Please check the voucher status. !!!");
                        if (Cancelled == 1) throw new Exception("This voucher cannot canceled as it has already been canceled. Please check the voucher status. !!!");

                        if (BusyVchCode != 0)
                        {
                            var VoucherCancel = BusyVoucherCancellation(Invobj, CompCode, FY, BusyVchCode);

                            // Check if the status is 0 (error case)
                            if (VoucherCancel.Status == 0)
                            {
                                throw new Exception("Error in Voucher Cancellation:" + VoucherCancel.Msg);
                            }
                        }

                        sql = $"Delete From ESJSLRefTran Where VchType = {Order_VchType_1} And VchCode = {VchCode} And OrderId = {OrderId_1}";
                        DT2 = new SQLHELPER(constr).ExecuteSQL(sql);

                        if (DT2 == 1)
                        {
                            sql = $"Update ESJSLTran1 Set Cancelled = 1 Where VchCode = {VchCode} And Vchtype = {VchType}";
                            DT2 = new SQLHELPER(constr).ExecuteSQL(sql);

                            if (DT2 == 0)
                            {
                                throw new Exception("An error occurred while canceled voucher. Please try again later !!!");
                            }
                            else
                            {
                                Status = 1; StatusStr = "Stock Transfer Is Successfully Cancelled !!!";
                            }
                        }
                        break;

                    case 4:

                        // Packing Order Cancellection
                        sql = $"Select Top 1 * From ESJSLPacking Where VchCode = {VchCode} And VchType = {VchType}";
                        DT1 = new SQLHELPER(constr).getTable(sql);

                        if (DT1 != null && DT1.Rows.Count > 0)
                        {
                            throw new Exception("This voucher cannot be canceled as it has already been canceled. Please check the voucher status. !!!");
                        }
                        else
                        {
                            sql = $"Update ESJSLPacking SET [Status] = 2, [CANCELLED] = 1 WHERE VchCode = {VchCode} And VchType = {VchType}";
                            DT2 = new SQLHELPER(constr).ExecuteSQL(sql);

                            if (DT2 == 1)
                            {
                                sql = $"Delete From ESJSLRefTran Where NewRefVchCode = {VchCode} And VchType = {VchType}";
                                DT2 = new SQLHELPER(constr).ExecuteSQL(sql);

                                if (DT2 == 1)
                                {
                                    Status = 1; StatusStr = "Packing Box Is Successfully Cancelled !!!";
                                }
                            }
                            else
                            {
                                throw new Exception("An error occurred while canceled voucher. Please try again later !!!");
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = Status, Msg = StatusStr.ToString() };
        }

        private dynamic BusyVoucherCancellation(dynamic obj, string CompCode, string FY, int BusyCode)
        {
            try
            {
                object VchCode = BusyCode;
                string constr = GetConnectionString(Provider, CompCode, FY);
                string StatusStr = string.Empty; string errMsg = string.Empty;
                CFixedInterface FI = new CFixedInterface();
                BusyVoucher BusyVch = new BusyVoucher();
                double InvAmount = 0; dynamic NewInv = obj;

                // Generate Stock Tranfer Cancellection XML
                string xmlstr = GetBusyCancellectionXML(NewInv, BusyCode, ref InvAmount, constr);

                bool Connect = false; FI.CloseDB();
                Connect = FI.OpenCSDBForYear(BusyPath, ServerName, SUserName, SPassword, CompCode, Convert.ToInt16(FY));

                if (!Connect)
                {
                    throw new Exception("Unable To Connect To Company");
                }

                // Save Stock Transfer Invoice
                if (!SaveVoucherFromXML(5, true, xmlstr, ref VchCode, FI, out errMsg))
                {
                    throw new Exception(errMsg);
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success" };
        }

        public string GetBusyCancellectionXML(dynamic Inv, int BusyCode, ref double InvAmount, string Connectionstr)
        {
            string XMLStr = string.Empty;
            try
            {
                BusyVoucher BVch = new BusyVoucher();
                BusyVoucher.StockTransfer ORD = new BusyVoucher.StockTransfer();
                ORD.VchSeriesName = Inv.SeriesName;
                ORD.Date = Inv.VchDate.ToString();
                ORD.VchNo = Inv.VchNo.ToString();
                ORD.VchType = Inv.VchType;
                ORD.StockUpdationDate = ORD.Date;
                ORD.MasterName1 = Inv.MCName1.ToString();
                ORD.MasterName2 = Inv.MCName2.ToString();
                ORD.TranCurName = "";
                ORD.Cancelled = true;
                ORD.InputType = 1;
                ORD.TmpVchCode = BusyCode;
                ORD.TmpVchSeriesCode = Inv.SeriesCode;
                ORD.ItemEntries = new List<BusyVoucher.ItemDetail>();
                //ORD.BillSundries = new List<BusyVoucher.BSDetail>();
                //ORD.VchOtherInfoDetails = new BusyVoucher.VchOtherInfoDetails();
                //ORD.VchOtherInfoDetails.Narration1 = clsMain.MyString(Inv.Remarks);
                //ORD.VchOtherInfoDetails.Transport = clsMain.MyString(Inv.Transport);
                //ORD.VchOtherInfoDetails.Station = clsMain.MyString(Inv.Station);

                int SrNo = 0;
                foreach (var item in Inv.ItemDetails)
                {
                    BusyVoucher.ItemDetail ID = new BusyVoucher.ItemDetail();
                    SrNo = SrNo + 1;
                    ID.SrNo = SrNo;
                    ID.VchType = ORD.VchType;
                    ID.Date = ORD.Date;
                    ID.VchNo = Inv.VchNo.ToString();
                    ID.ItemName = item.ItemName.ToString();
                    ID.UnitName = item.UName.ToString();
                    ID.ConFactor = 1;
                    ID.Qty = clsMain.MyDouble(item.Qty);
                    ID.QtyMainUnit = clsMain.MyDouble(ID.Qty);
                    ID.QtyAltUnit = 0;
                    ID.ConFactor = 0;
                    ID.ListPrice = 0;
                    ID.Price = (double)item.Price;
                    ID.Amt = clsMain.MyDouble(ID.Price * ID.Qty);
                    ID.PriceAltUnit = 0;
                    if (ID.QtyAltUnit != 0) { ID.PriceAltUnit = clsMain.MyDouble((clsMain.MyDouble(ID.Amt) / clsMain.MyDouble(ID.QtyAltUnit)).ToString("0.00")); }
                    InvAmount = InvAmount + clsMain.MyDouble(ID.Amt);
                    ID.TmpVchCode = 0;
                    ID.MC = ORD.MasterName2;
                    ID.ItemDescInfo = new BusyVoucher.ItemDescInfo();
                    ORD.ItemEntries.Add(ID);
                }
                //ORD.TmpTotalAmt = InvAmount;
                XMLStr = CreateXML(ORD).Replace("<?xml version=\"1.0\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            }
            catch
            {
                return "";
            }
            return XMLStr;
        }

        [HttpGet]
        public dynamic GetSalesmanWiseCustomerList(string CompCode, string FY, string Users, int VchType, string startDate, string endDate, int CType)
        {
            List<CustomerList> clist = new List<CustomerList>();
            try
            {
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr);
                string formattedStartDate = string.IsNullOrEmpty(startDate) ? "" : Busyhelper.FormatDate(startDate);
                string formattedEndDate = string.IsNullOrEmpty(endDate) ? "" : Busyhelper.FormatDate(endDate);

                string sql = $"Select A.Code as AccCode, ISNULL(A.[Name], '') As AccName, ISNULL(A.[Mobile], '') as Mobile, IsNull(A.[Email], '') as Email, IsNull(A.[OrgName], '') as FirmName, IsNull(A.[Address], '') as [Address], IsNull(A.[CustType], 0) as CTCode, IsNull(A.[Deactive], 0) as Deactive, (CASE WHEN A.[CustType] = 1 THEN 'B2B A' WHEN A.[CustType] = 2 THEN 'B2B B' WHEN  A.[CustType] = 3 THEN 'B2C' ELSE '' END) as CTName From ESJSLCustomer A LEFT JOIN ESJSLTRAN1 B ON A.Code = B.MasterCode1 And B.VchType = {VchType}"; //Where A.CreatedBy = '{Users.Replace("'", "''").Trim()}'
                if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And B.[Date] >= '{formattedStartDate}' And B.[Date] <= '{formattedEndDate}' ";
                if (CType != 0) sql += $"And A.[CustType] = {CType} ";
                sql += "Group By A.[Code], A.[Name], A.[Mobile], A.[Email], A.[OrgName], A.[Address],A.CustType, A.[Deactive]  Order By A.[Name]";

                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        clist.Add(new CustomerList
                        {
                            AccCode = Convert.ToInt32(item["AccCode"]),
                            AccName = clsMain.MyString(item["AccName"]),
                            Mobile = clsMain.MyString(item["Mobile"]),
                            Email = clsMain.MyString(item["Email"]),
                            FirmName = clsMain.MyString(item["FirmName"]),
                            Address = clsMain.MyString(item["Address"]),
                            CTCode = Convert.ToInt32(item["CTCode"]),
                            CTName = clsMain.MyString(item["CTName"]),
                            Deactive = Convert.ToInt32(item["Deactive"])
                        });
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found !!!", Data = clist };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = clist };
        }

        [HttpGet]
        public dynamic GetSalesManWiseOrderGraffDetails(string CompCode, string FY, string Users, int AccCode, int VchType, string StartDate, string EndDate)
        {
            List<SalesManOrdersDt> slist = new List<SalesManOrdersDt>();
            List<GraffYear> dlist = new List<GraffYear>();
            try
            {
                string formattedStartDate = string.IsNullOrEmpty(StartDate) ? "" : Busyhelper.FormatDate(StartDate);
                string formattedEndDate = string.IsNullOrEmpty(EndDate) ? "" : Busyhelper.FormatDate(EndDate);
                string constr = GetConnectionString(Provider, CompCode, FY);
                SQLHELPER conobj = new SQLHELPER(constr);

                // Fetch Salesman Order Amount using the new function
                dlist = GetSalesmanOrderAmount(constr, Users, AccCode, VchType, formattedStartDate, formattedEndDate);

                string sql = $"Select A.[VchCode], IsNull(A.[VchNo], '') as VchNo, Convert(Varchar, A.[Date], 105) as Date, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(C.[Name], '') as AccName, IsNull(C.[Mobile], '') as Mobile,IsNull(A.[TotQty], 0) as TotQty,IsNull(A.[TotAmt], 0) as TotAmt,IsNull(A.[NetAmount], 0) as NetAmt,IsNull(A.[QStatus], 0) as QStatus,(CASE WHEN A.[QStatus] = 1 Then 'Approved' WHEN A.[QStatus] = 2 Then 'Rejected' ELSE 'Pending' END) as QName, IsNull(C.[CustType], 0) as CTCode, (CASE WHEN C.[CustType] = 1 THEN 'B2B A' WHEN C.[CustType] = 2 THEN 'B2B B' WHEN C.[CustType] = 3 THEN 'B2C' ELSE '' END) as CTName, IsNull(A.[Remarks], '') as Remarks From ESJSLTRAN1 A Inner Join ESUserMapping B On A.CreatedBy = B.[User] Inner Join ESJSLCustomer C On A.MasterCode1 = C.Code Where B.UType in (1,2) And B.[User] = '{Users.Replace("'", "''").Trim()}' And A.[MasterCode1] = {AccCode} And A.[VchType] = {VchType}";
                if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And Date >= '{formattedStartDate}' And Date <= '{formattedEndDate}'";
                sql += "ORDER BY A.[VchCode] DESC, [Date] DESC";
                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        slist.Add(new SalesManOrdersDt
                        {
                            VchCode = Convert.ToInt32(item["VchCode"]),
                            VchNo = item["VchNo"].ToString().Trim(),
                            Date = item["Date"].ToString().Trim(),
                            AccCode = Convert.ToInt32(item["AccCode"]),
                            AccName = clsMain.MyString(item["AccName"]).Trim(),
                            Mobile = clsMain.MyString(item["Mobile"]).Trim(),
                            TQty = Convert.ToDecimal(item["TotQty"]),
                            TAmt = Convert.ToDecimal(item["TotAmt"]),
                            NetAmt = Convert.ToDecimal(item["NetAmt"]),
                            SCode = Convert.ToInt32(item["QStatus"]),
                            SName = clsMain.MyString(item["QName"]),
                            CTCode = Convert.ToInt32(item["CTCode"]),
                            CTName = clsMain.MyString(item["CTName"]),
                            Remarks = clsMain.MyString(item["Remarks"])
                        });
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found !!!" };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = new { DiscountDetails = dlist, ItemDetails = slist } };
        }

        [HttpGet]
        public dynamic GetSalesmanOrderAmount(string constr, string Users, int AccCode, int VchType, string StartDate, string EndDate)
        {
            List<GraffYear> Grafyear = new List<GraffYear>();
            try
            {
                SQLHELPER conobj = new SQLHELPER(constr);
                string sql = $"Select YEAR(A.[DATE]) as [YEAR], MONTH(A.[DATE]) as [MONTH], CONVERT(CHAR(3), DATENAME(MONTH, A.[DATE])) As MName, IsNull(SUM(A.[NetAmount]), 0) as QutAmt, IsNull((SUM(A.[NetAmount]) - 100), 0) as InvAmt From ESJSLTRAN1 A Inner Join ESUserMapping B On A.CreatedBy = B.[User] Where B.UType in (1,2) And B.[User] = '{Users}' And A.[MasterCode1] = {AccCode} And A.[VchType] = {VchType}";
                if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate)) sql += $" And A.[Date] >= '{StartDate}' And A.[Date] <= '{EndDate}'"; sql += $"Group By YEAR(A.[DATE]), MONTH(A.[DATE]),  CONVERT(CHAR(3), DATENAME(MONTH, A.[DATE]))";
                DataTable DT1 = conobj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item in DT1.Rows)
                    {
                        int rootCode = Convert.ToInt32(item["YEAR"]);
                        int dataItemCode = Convert.ToInt32(item["MONTH"]);

                        // Find or create the root data object
                        GraffYear rootData = Grafyear.FirstOrDefault(rd => rd.Year == rootCode);
                        if (rootData == null)
                        {
                            rootData = new GraffYear
                            {
                                Year = rootCode,
                                Months = new List<Months>()
                            };
                            Grafyear.Add(rootData);
                        }

                        Months dataItem = rootData.Months.FirstOrDefault(di => di.MonthsCode == dataItemCode);
                        if (dataItem == null)
                        {
                            dataItem = new Months
                            {
                                MonthsCode = dataItemCode,
                                MonthsName = clsMain.MyString(item["MName"]),
                                QuotaionAmt = Convert.ToDecimal(item["QutAmt"]),
                                InvoiceAmt = Convert.ToDecimal(item["InvAmt"]),
                            };
                            rootData.Months.Add(dataItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching salesman order amount: " + ex.Message);
            }
            return Grafyear;
        }

        public GetPrice GetProductMinAndMaxPrice(string constr, int ParentGrp)
        {
            GetPrice price = new GetPrice();
            try
            {
                SQLHELPER objcon = new SQLHELPER(constr);
                string sql = $"Select IsNull(Min(A.[D2]), 0) as MinPrice, IsNull(Max(A.[D2]), 0) as MaxPrice From (Select Code, isNull([D2], 0) as D2 From Master1 Where ParentGrp = {ParentGrp} And MasterType  = 6 And [D2] > 0) A";
                DataTable DT1 = objcon.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    price.MinPrice = Convert.ToDecimal(DT1.Rows[0]["MinPrice"]);
                    price.MaxPrice = Convert.ToDecimal(DT1.Rows[0]["MaxPrice"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Price : " + ex.Message);
            }
            return price;
        }

        [HttpGet]
        public string GetAllParentGroups(int ParentGrp, string ConStr)
        {
            string GroupStr = Convert.ToString(ParentGrp);
            try
            {
                GetItemAllParentQueryStr(ParentGrp, ref GroupStr, ConStr);
            }
            catch
            {
                return GroupStr;
            }
            return GroupStr;
        }

        public void GetItemAllParentQueryStr(int ParentGrp, ref string GroupStr, string ConStr)
        {
            try
            {
                string sql = "";
                // sqlhelper Obj = new sqlhelper();
                DataTable dt = new DataTable();
                sql = "Select A.ParentGrp as Code From Master1 A Where A.Code = " + ParentGrp + " And A.MasterType = 5 And A.ParentGrp > 0";
                if (Provider == 1)
                {
                    dt = new OLEDBHELPER(ConStr).getTable(sql);
                }
                else
                {
                    dt = new SQLHELPER(ConStr).getTable(sql);
                }
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    foreach (DataRow Row in dt.Rows)
                    {
                        if (GroupStr.Length > 0) { GroupStr = GroupStr + "," + Convert.ToString(Row["Code"]); } else { GroupStr = Convert.ToString(Row["Code"]); }
                        GetItemAllParentQueryStr(Convert.ToInt32(Row["Code"]), ref GroupStr, ConStr);
                    }
                }
            }
            catch
            {

            }
        }

        [HttpGet]
        public string GetAllItemSubGroups(int ParentGrp, string ConStr)
            {
                string GroupStr = Convert.ToString(ParentGrp);
                try
                {
                    GetItemAllChildQueryStr(ParentGrp, ref GroupStr, ConStr);
                }
                catch
                {
                    return GroupStr;
                }
                return GroupStr;
            }

        public void GetItemAllChildQueryStr(int ParentGrp, ref string GroupStr, string ConStr)
            {
                try
                {
                    string sql = "";
                    // sqlhelper Obj = new sqlhelper();
                    DataTable dt = new DataTable();
                    sql = "Select A.Code From Master1 A Where A.ParentGrp = " + ParentGrp + " And A.MasterType = 5 Order By A.Code";
                    if (Provider == 1)
                    {
                        dt = new OLEDBHELPER(ConStr).getTable(sql);
                    }
                    else
                    {
                        dt = new SQLHELPER(ConStr).getTable(sql);
                    }
                    if ((dt != null) && (dt.Rows.Count > 0))
                    {
                        foreach (DataRow Row in dt.Rows)
                        {
                            if (GroupStr.Length > 0) { GroupStr = GroupStr + "," + Convert.ToString(Row["Code"]); } else { GroupStr = Convert.ToString(Row["Code"]); }
                            GetItemAllChildQueryStr(Convert.ToInt32(Row["Code"]), ref GroupStr, ConStr);
                        }
                    }
                }
                catch
                {

                }
            }

        public static string CreateXML(Object YourClassObject)
        {
            XmlDocument xmlDoc = new XmlDocument();   //Represents an XML document, 
                                                        // Initializes a new instance of the XmlDocument class.          
            XmlSerializer xmlSerializer = new XmlSerializer(YourClassObject.GetType());
            // Creates a stream whose backing store is memory. 
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, YourClassObject);
                xmlStream.Position = 0;
                //Loads the XML document from the specified string.
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }

        private string GetConnectionString(int Provider, string CompCode, string FY)
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

        public string GetInvoiceXML(int VchType, VchInvoice Inv, ref double InvAmount, string ConnectionString)
        {
            string XMLStr = "";
            double TaxableAmount = 0;
            int CompoundDiscount = 0;
            try
            {
                int SN = 0;
                double Disc1 = 0;
                double Disc2 = 0;
                BusyVoucher BVch = new BusyVoucher();
                BusyVoucher.STPTData stptdata = BVch.GetSTPTData(Provider, ConnectionString, Inv.StPtName, 13);
                int MCCode = BVch.GetMasterNameToCode(Provider, ConnectionString, Inv.MCName, 11);
                BusyVoucher.Sale ORD = new BusyVoucher.Sale();
                ORD.VchSeriesName = Inv.SeriesName; //BVch.GetMasterCodeToName(ConnStr, SeriesCode).Replace("12", "");
                ORD.Date = DateTime.UtcNow.ToString("dd-MM-yyyy");
                ORD.VchNo = "";
                ORD.VchType = VchType;
                ORD.StockUpdationDate = ORD.Date;
                ORD.STPTName = Inv.StPtName; //BVch.GetMasterCodeToName(ConnStr,Inv.StPtCode,pCmpCode,pFY);
                ORD.MasterName1 = Inv.AccName; //BVch.GetMasterCodeToName(ConnStr, clsMain.MyInt(PartyId));
                ORD.MasterName2 = Inv.MCName; //BVch.GetMasterCodeToName(ConnStr, MCCode);
                if (Inv.Salesman.Length > 0)
                {
                    ORD.BrokerInvolved = true;
                    ORD.BrokerName = Inv.Salesman;
                }
                ORD.TranCurName = "";
                ORD.TmpVchCode = 0;
                ORD.TmpVchSeriesCode = Inv.SeriesCode;

                ORD.ItemEntries = new List<BusyVoucher.ItemDetail>();
                ORD.BillSundries = new List<BusyVoucher.BSDetail>();
                ORD.VchOtherInfoDetails = new BusyVoucher.VchOtherInfoDetails();

                ORD.VchOtherInfoDetails.Narration1 = clsMain.MyString(Inv.Remarks);
                ORD.VchOtherInfoDetails.Transport = clsMain.MyString(Inv.Transport);
                ORD.VchOtherInfoDetails.Station = clsMain.MyString(Inv.Station);


                BusyVoucher.ItemDetail ID = new BusyVoucher.ItemDetail();
                int SrNo = 0;
                int ISSrNo = 0;
                foreach (var item in Inv.ItemDT)
                {
                    ID = new BusyVoucher.ItemDetail();
                    SrNo = SrNo + 1;
                    SN = SrNo;
                    ID.SrNo = SrNo;
                    ID.VchType = ORD.VchType;
                    ID.Date = ORD.Date;
                    ID.VchNo = ORD.VchNo;
                    ID.ItemName = item.ItemName; //BVch.GetMasterCodeToName(ConnectionString, clsMain.MyInt(ItemId[i]));

                    ID.ConFactor = 1;
                    if (item.BillingUnit == 1)
                    {
                        ID.UnitName = BVch.GetItemMainUnitName(Provider, ConnectionString, clsMain.MyInt(item.ItemCode), 1);
                        ID.Qty = clsMain.MyDouble(item.Qty);
                        ID.QtyMainUnit = clsMain.MyDouble(item.Qty);
                        ID.QtyAltUnit = clsMain.MyDouble(item.AltQty);
                    }
                    else
                    {
                        ID.UnitName = BVch.GetItemMainUnitName(Provider, ConnectionString, clsMain.MyInt(item.ItemCode), 2);
                        ID.Qty = clsMain.MyDouble(item.AltQty);
                        ID.QtyMainUnit = clsMain.MyDouble(item.Qty);
                        ID.QtyAltUnit = clsMain.MyDouble(item.AltQty);

                    }
                    ID.ConFactor = clsMain.MyDouble(item.ConFactor);
                    ID.ListPrice = clsMain.MyDouble(item.ListPrice);
                    ID.Amt = clsMain.MyDouble(ID.ListPrice * ID.Qty);
                    if (CompoundDiscount == 1)
                    {
                        ID.CompoundDiscount = clsMain.MyDouble(item.DiscPerent) + "+" + clsMain.MyDouble(item.AddDiscount);// "10+5";
                        Disc1 = clsMain.MyDouble(((ID.Amt * clsMain.MyDouble(item.DiscPerent)) / 100).ToString("0.00"));
                        ID.Amt = clsMain.MyDouble(ID.Amt - Disc1);
                        Disc2 = clsMain.MyDouble(((ID.Amt * clsMain.MyDouble(item.AddDiscount)) / 100).ToString("0.00"));
                        ID.Discount = Disc1 + Disc2;
                        ID.Amt = clsMain.MyDouble(ID.Amt - Disc2);
                    }
                    else
                    {
                        ID.DiscountPercent = clsMain.MyDouble(item.DiscPerent);
                        ID.Discount = clsMain.MyDouble(((ID.Amt * ID.DiscountPercent) / 100).ToString("0.00"));
                        ID.Amt = clsMain.MyDouble(ID.Amt - ID.Discount);
                    }

                    ID.Price = clsMain.MyDouble(item.Price);
                    ID.PriceAltUnit = 0;
                    if (ID.QtyAltUnit != 0) { ID.PriceAltUnit = clsMain.MyDouble((clsMain.MyDouble(ID.Amt) / clsMain.MyDouble(ID.QtyAltUnit)).ToString("0.00")); }
                    if (item.SchemeCode > 0)
                    {
                        ID.SchemeName = BVch.GetMasterCodeToName(ConnectionString, item.SchemeCode);
                        ID.SchemeType = item.SchemeType;
                        ID.SchemeParentRowNo = 0;
                    }
                    else
                    {
                        ID.SchemeName = null;
                        ID.SchemeType = 0;
                        ID.SchemeParentRowNo = 0;

                    }
                    ID.STPercent = 0;
                    ID.STPercent1 = 0;
                    ID.STAmount = 0;
                    if (stptdata.MultiTax == true)
                    {
                        BusyVoucher.TaxCData taxcdata = BVch.GetTaxCategoryData(Provider, ConnectionString, item.ItemCode);
                        if (stptdata.GSTType == 1)
                        {
                            ID.STPercent = clsMain.MyDouble(taxcdata.IGSTPer);
                            ID.STPercent1 = clsMain.MyDouble(0);
                        }
                        else
                        {
                            ID.STPercent = clsMain.MyDouble(taxcdata.CGSTPer);
                            ID.STPercent1 = clsMain.MyDouble(taxcdata.SGSTPer);
                        }
                        if (stptdata.TaxType == true)
                        {
                            TaxableAmount = ((ID.Amt * 100) / (100 + (ID.STPercent + ID.STPercent1)));
                        }
                        else
                        {
                            TaxableAmount = ID.Amt;
                        }
                        ID.STAmount = clsMain.MyDouble(((TaxableAmount * ID.STPercent) / 100).ToString("0.00"));
                        ID.STAmount = ID.STAmount + clsMain.MyDouble(((TaxableAmount * ID.STPercent1) / 100).ToString("0.00"));
                        if (stptdata.TaxType == false) { ID.Amt = clsMain.MyDouble(ID.Amt + ID.STAmount); }
                    }
                    InvAmount = InvAmount + clsMain.MyDouble(ID.Amt);
                    ID.TmpVchCode = 0;
                    ID.MC = ORD.MasterName2;
                    ID.AF = item.IRemarks;
                    ID.ItemDescInfo = new BusyVoucher.ItemDescInfo();
                    ID.ItemDescInfo.Description1 = item.IDescription1;
                    ID.ItemDescInfo.Description2 = item.IDescription2;
                    ID.ItemDescInfo.Description3 = item.IDescription3;
                    ID.ItemDescInfo.Description4 = item.IDescription4;
                    ID.ItemDescInfo.tmpSrNo = SrNo;
                    ID.ItemSerialNoEntries = new List<BusyVoucher.ItemSerialNoDetail>();
                    ISSrNo = 0;
                    foreach (var SerialItem in item.ItemSerailDT)
                    {
                        ISSrNo = ISSrNo + 1;
                        BusyVoucher.ItemSerialNoDetail serialNoDetail = new BusyVoucher.ItemSerialNoDetail();
                        serialNoDetail.ItemCode = item.ItemCode;
                        serialNoDetail.MCCode = MCCode;
                        serialNoDetail.VchItemSN = ID.SrNo;
                        serialNoDetail.GridSN = ISSrNo;
                        serialNoDetail.SerialNo = SerialItem.SerailNo;
                        serialNoDetail.MainQty = -1;
                        serialNoDetail.AltQty = -1;
                        serialNoDetail.MainUnitPrice = ID.Price;
                        serialNoDetail.AltUnitPrice = ID.Price;
                        ID.ItemSerialNoEntries.Add(serialNoDetail);
                    }

                    ORD.ItemEntries.Add(ID);
                    if (item.FItemCode > 0 || item.FreeQty > 0)
                    {
                        ID = new BusyVoucher.ItemDetail();
                        SrNo = SrNo + 1;
                        ID.SrNo = SrNo;
                        ID.VchType = ORD.VchType;
                        ID.Date = ORD.Date;
                        ID.VchNo = ORD.VchNo;
                        ID.ItemName = BVch.GetMasterCodeToName(ConnectionString, item.FItemCode);

                        ID.ConFactor = 1;
                        //if (item.BillingUnit == 1)
                        //{
                        ID.UnitName = BVch.GetItemMainUnitName(Provider, ConnectionString, clsMain.MyInt(item.FItemCode), 1);
                        ID.Qty = clsMain.MyDouble(item.FreeQty);
                        ID.QtyMainUnit = clsMain.MyDouble(item.FreeQty);
                        ID.QtyAltUnit = clsMain.MyDouble(item.FreeQty);
                        //}
                        //else
                        //{
                        //    ID.UnitName = BVch.GetItemMainUnitName(Provider, ConnectionString, clsMain.MyInt(item.FItemCode), 2);
                        //    ID.Qty = clsMain.MyDouble(item.FreeQty);
                        //    ID.QtyMainUnit = clsMain.MyDouble(item.FreeQty);
                        //    ID.QtyAltUnit = clsMain.MyDouble(item.FreeQty);

                        //}
                        ID.ConFactor = clsMain.MyDouble(item.ConFactor);
                        ID.ListPrice = clsMain.MyDouble(item.ListPrice);
                        ID.Amt = clsMain.MyDouble(ID.ListPrice * ID.Qty);
                        //if (CompoundDiscount == 1)
                        //{
                        //    ID.CompoundDiscount = clsMain.MyDouble(item.DiscPerent) + "+" + clsMain.MyDouble(item.AddDiscount);// "10+5";
                        //    Disc1 = clsMain.MyDouble(((ID.Amt * clsMain.MyDouble(item.DiscPerent)) / 100).ToString("0.00"));
                        //    ID.Amt = clsMain.MyDouble(ID.Amt - Disc1);
                        //    Disc2 = clsMain.MyDouble(((ID.Amt * clsMain.MyDouble(item.AddDiscount)) / 100).ToString("0.00"));
                        //    ID.Discount = Disc1 + Disc2;
                        //    ID.Amt = clsMain.MyDouble(ID.Amt - Disc2);
                        //}
                        //else
                        //{
                        ID.DiscountPercent = 0;
                        ID.Discount = 0;//clsMain.MyDouble(((ID.Amt * ID.DiscountPercent) / 100).ToString("0.00"));
                                        //ID.Amt = clsMain.MyDouble(ID.Amt - ID.Discount);
                                        //}
                                        //ID.DiscountPercent = clsMain.MyDouble(item.DiscPerent);
                                        //ID.Discount = clsMain.MyDouble(((ID.Amt * ID.DiscountPercent) / 100).ToString("0.00"));
                                        //ID.Amt = clsMain.MyDouble(ID.Amt - ID.Discount);
                        ID.Price = clsMain.MyDouble(item.Price);
                        ID.PriceAltUnit = 0;
                        if (ID.QtyAltUnit != 0) { ID.PriceAltUnit = clsMain.MyDouble((clsMain.MyDouble(ID.Amt) / clsMain.MyDouble(ID.QtyAltUnit)).ToString("0.00")); }
                        if (item.SchemeCode != 0)
                        {
                            ID.SchemeName = BVch.GetMasterCodeToName(ConnectionString, item.SchemeCode);
                            ID.SchemeType = item.SchemeType;
                            ID.SchemeParentRowNo = SN;
                        }
                        ID.STPercent = 0;
                        ID.STPercent1 = 0;
                        ID.STAmount = 0;
                        if (stptdata.MultiTax == true)
                        {
                            BusyVoucher.TaxCData taxcdata = BVch.GetTaxCategoryData(Provider, ConnectionString, item.FItemCode);
                            if (stptdata.GSTType == 1)
                            {
                                ID.STPercent = clsMain.MyDouble(taxcdata.IGSTPer);
                                ID.STPercent1 = clsMain.MyDouble(0);
                            }
                            else
                            {
                                ID.STPercent = clsMain.MyDouble(taxcdata.CGSTPer);
                                ID.STPercent1 = clsMain.MyDouble(taxcdata.SGSTPer);
                            }
                            if (stptdata.TaxType == true)
                            {
                                TaxableAmount = ((ID.Amt * 100) / (100 + (ID.STPercent + ID.STPercent1)));
                            }
                            else
                            {
                                TaxableAmount = ID.Amt;
                            }
                            ID.STAmount = clsMain.MyDouble(((TaxableAmount * ID.STPercent) / 100).ToString("0.00"));
                            ID.STAmount = ID.STAmount + clsMain.MyDouble(((TaxableAmount * ID.STPercent1) / 100).ToString("0.00"));
                            if (stptdata.TaxType == false) { ID.Amt = clsMain.MyDouble(ID.Amt + ID.STAmount); }
                        }
                        ORD.ItemEntries.Add(ID);
                    }
                }
                SrNo = 0;

                foreach (var BS in Inv.BSDetail)
                {
                    SrNo = SrNo + 1;
                    BusyVoucher.BSDetail BSDet = new BusyVoucher.BSDetail();
                    BSDet.BSName = BS.BSName;
                    BSDet.SrNo = SrNo;
                    BSDet.tmpBSCode = BS.BSCode;
                    BSDet.PercentVal = BS.BSPer;
                    BSDet.PercentOperatedOn = BS.PercentOperatedOn;
                    BSDet.Amt = BS.Amount;
                    ORD.BillSundries.Add(BSDet);

                }
                XMLStr = CreateXML(ORD).Replace("<?xml version=\"1.0\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            }
            catch
            {
                return "";
            }

            return XMLStr;
        }

        public int GetDefaultMaterialCenter(string constr, int ItemCode)
        {
            try
            {
                string sql = $"Select [CM6] As MCCode From Master1 Where Code = {ItemCode} And MasterType = 6 ";
                DataTable DT1 = new SQLHELPER(constr).getTable(sql);

                return DT1 != null ? clsMain.MyInt(DT1.Rows[0]["MCCode"]) : 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Item Default Material Center:" + ex.Message.ToString());
            }
        }

        [HttpGet]
        public string GetVchNo(string constr, int vchtype, int tranType, out int autoNo)
        {
            int autoVchNo = GetAutoVchNo(constr, vchtype);
            autoNo = Convert.ToInt32(autoVchNo);
#pragma warning disable IDE0018 // Inline variable declaration
            string prefix, suffix, padStr;
#pragma warning restore IDE0018 // Inline variable declaration
            LoadSuffixPrefix(constr, tranType, out prefix, out suffix, out padStr, ref autoVchNo);

            return $"{prefix}{padStr}{autoVchNo}{suffix}";
        }

        private int GetAutoVchNo(string constr, int Vchtype)
        {
            SQLHELPER obj = new SQLHELPER(constr);

            string sql = Vchtype == 110 ? $"Select IsNull(Max(AutoVchNo),0) as AutoVchNo From ESJSLPacking Where VchType = {Vchtype}" : $"Select IsNull(Max(AutoVchNo),0) as AutoVchNo From ESJSLTran1 Where VchType = {Vchtype}";
            DataTable DT1 = obj.getTable(sql);

            if (DT1 != null && DT1.Rows.Count > 0)
            {
                return Convert.ToInt32(DT1.Rows[0]["AutoVchNo"]) + 1;
            }
            return 1;
        }

        private void LoadSuffixPrefix(string constr, int tranType, out string prefix, out string suffix, out string padStr, ref int vchNo)
        {
            prefix = string.Empty;
            suffix = string.Empty;
            padStr = string.Empty;
            SQLHELPER obj = new SQLHELPER(constr);

            string sql = $"SELECT * FROM ESJSLVchConfig WHERE TranType = {tranType}";
            DataTable DT1 = obj.getTable(sql);

            if (DT1 != null && DT1.Rows.Count > 0)
            {
                prefix = DT1.Rows[0]["Prefix"]?.ToString().Trim() ?? string.Empty;
                suffix = DT1.Rows[0]["Suffix"]?.ToString().Trim() ?? string.Empty;

                if (!DBNull.Value.Equals(DT1.Rows[0]["StartNo"]) && Convert.ToInt32(DT1.Rows[0]["StartNo"]) > vchNo)
                {
                    vchNo = Convert.ToInt32(DT1.Rows[0]["StartNo"]);
                }

                string vchStr = vchNo.ToString();
                if (!DBNull.Value.Equals(DT1.Rows[0]["Padding"]) && Convert.ToInt32(DT1.Rows[0]["Padding"]) != 0)
                {
                    int padLength = DT1.Rows[0]["PaddLength"] != DBNull.Value ? Convert.ToInt32(DT1.Rows[0]["PaddLength"]) : 0;
                    char padChar = DT1.Rows[0]["PaddChar"] != DBNull.Value ? Convert.ToChar(DT1.Rows[0]["PaddChar"]) : ' ';

                    if (padLength > vchStr.Length)
                    {
                        padStr = new string(padChar, padLength - vchStr.Length);
                    }
                }
            }
        }

        public string GetBusyMasterCode2NameIfExist(string Constr, int Code, int MasterType)
        {
            try
            {
                string sql = $"Select IsNull([Name], '') as Name From Master1 Where Code = {Code} And MasterType = {MasterType}";
                DataTable dt1 = new SQLHELPER(Constr).getTable(sql);

                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    return clsMain.MyString(dt1.Rows[0]["Name"]).Trim();
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        private AutoOrderDT GetVoucherConfigDetails(string constr, int TranType)
        {
            AutoOrderDT S_DT = new AutoOrderDT();
            try
            {
                string sql = string.Empty;

                sql = $"Select Top 1 ISNULL(A.[MasterCode1], 0) as SeriesCode, IsNull(SUBSTRING(B.[Name], 3, 25), '') as SeriesName, ISNULL(A.[MasterCode2], 0) as [MasterCode1],  ISNULL(M1.[Name], '') as Name1, ISNULL(A.[MasterCode3], 0) as MasterCode2, ISNULL(M2.[Name], '') as Name2 From ESJSLSeriesConfig A Left Join Master1 B ON A.MasterCode1 = B.Code Left Join Master1 M1 ON A.MasterCode2 = M1.Code Left Join Master1 M2 ON A.MasterCode3 = M2.Code Where A.RecType = {TranType}";
                DataTable DT1 = new SQLHELPER(constr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    S_DT.SeriesCode = Convert.ToInt32(DT1.Rows[0]["SeriesCode"]);
                    S_DT.SeriesName = clsMain.MyString(DT1.Rows[0]["SeriesName"]);
                    S_DT.MasterCode1 = Convert.ToInt32(DT1.Rows[0]["MasterCode1"]);
                    S_DT.Name1 = clsMain.MyString(DT1.Rows[0]["Name1"]).Trim();
                    S_DT.MasterCode2 = Convert.ToInt32(DT1.Rows[0]["MasterCode2"]);
                    S_DT.Name2 = clsMain.MyString(DT1.Rows[0]["Name2"]).Trim();
                }
                else
                {
                    throw new Exception("Voucher Can't Save, Due to Series And Material Center Can't Configure");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Configuration Series Details" + ex.Message.ToString());
            }
            return S_DT;
        }

        private string GetConfigeOptionalField(string constr, int RecType)
            {
                try
                {
                    string sql = $"Select IsNull(Code, 0) as OptFld From ESJSLOFConfig Where RecType = {RecType}";
                    DataTable DT1 = new SQLHELPER(constr).getTable(sql);

                    if (DT1 != null && DT1.Rows.Count > 0)
                    {
                        return "OF" + Convert.ToInt32(DT1.Rows[0]["OptFld"]);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching Configuration Item Optional Filed:" + ex.Message.ToString());
                }
                return "";
            }
    }
}
