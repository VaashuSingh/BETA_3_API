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


                sql = $"SELECT TOP 1 ISNULL(A.[User], '') as [Username],  ISNULL(A.[PWD], '') as [Password],  ISNULL(A.[UType], '') as [UType], ISNULL(A.[MCCode], 0) as MCCode, IsNull(A.[MCName], '') as MCName FROM [ESBTUserMapping] A WHERE A.[User] = '" + UName + "' AND A.[PWD] = '" + Pass + "' ";
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
            string U_TRAN_TYPE = string.Empty;
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
                    U_TRAN_TYPE = clsMain.MyString(DT.Rows[0]["UTName"]);

                    switch (UTCode)
                    {
                        // Inward
                        case 1:
                            sql = $"SELECT 'Inward' as UType, 'Pending' AS Name, COUNT([VchCode]) AS Value FROM ESBTTRAN1 WHERE [MasterCode2] = {MCCode} And ([Status] Is Null Or [Status] = 0) And VchType = 108 UNION ALL SELECT 'Inward' as UType, 'Completed', COUNT(*) FROM ESBTTRAN1 WHERE [Status] IN (1, 3) And MasterCode2 = {MCCode} And VchType = 108 And ValidateBy = '{Users.Replace("'", "''")}'";
                            break;

                        // Outward
                        case 2:
                            sql = $"SELECT 'Outward' as UType, 'Pending' AS Name, COUNT([VchCode]) AS Value FROM ESBZTRAN1 WHERE [MasterCode2] = {MCCode} And [Status] = 1 And [VchType] = 11 UNION ALL SELECT 'Outward' as UType, 'Completed', COUNT(*) FROM ESBZTRAN1 WHERE [MasterCode2] = {MCCode} And [Status] IN (2, 3) And [VchType] = 11 And ValidateBy = '{Users.Replace("'", "''")}'";
                            break;

                        // Both
                        case 3:
                            sql = $"SELECT 'Inward' as UType, 'Pending' AS Name, COUNT([VchCode]) AS Value FROM ESBTTRAN1 WHERE [MasterCode2] = {MCCode} And ([Status] Is Null Or [Status] = 0) And VchType = 108 UNION ALL SELECT 'Inward' as UType, 'Completed', COUNT(*) FROM ESBTTRAN1 WHERE [Status] IN (1, 3) And MasterCode2 = {MCCode} And VchType = 108 And ValidateBy = '{Users.Replace("'", "''")}' UNION ALL SELECT 'Outward' as UType, 'Pending' AS Name, COUNT(VchCode) AS Value FROM ESBZTRAN1 WHERE MasterCode2 = { MCCode } And [Status] = 1 And[VchType] = 11 UNION ALL SELECT 'Outward' as UType, 'Completed', COUNT(*) FROM ESBZTRAN1 WHERE[MasterCode2] = { MCCode} And[Status] IN(2, 3) And[VchType] = 11 And ValidateBy = '{Users.Replace("'", "''")}'";
                            break;

                        default:
                            return new { Status = 0, Msg = "Invalid User Type" };
                    }
                    DataTable DT1 = con.getTable(sql);

                    if (DT1 != null && DT1.Rows.Count > 0)
                    {

                        foreach (DataRow item in DT1.Rows)
                        {
                            Dashboard dash1 = new Dashboard();

                            dash.Add(new Dashboard
                            {
                                UType = clsMain.MyString(item["UType"]),
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
            return new { Status = 1, Msg = "Success", Data = dash};
            // Return Response with Dynamic U_TRAN_TYPE Key
            //var response = new
            //{  
            //    Status = 1, Msg = "Success", Data = new Dictionary<string, object> 
            //    { 
            //        { U_TRAN_TYPE.ToUpper(), dash } 
            //    }
            //};

            //return response;

        }

        [HttpGet]
        public dynamic GetVoucherList(string CompCode, string FY, string StartDate, string EndDate, int TranType, int MCCode, int Status, string Users)
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
                        //sql = $"Select A.[VchCode], IsNull(A.[VchNo], '') as VchNo, Convert(Varchar, A.[Date], 105) as VchDate, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(M.[Name], '') as AccName, IsNull(A.[MasterCode2], 0) as MCCode, IsNull(M1.[Name], '') as MCName, IsNull(A.[Container], '') as Container, IsNull(A.[D1], 0) as TotCart, IsNull(A.[D2], 0) as TotQty, IsNull(A.[D3], 0) as TotPcs, (CASE WHEN A.[Status] = 0 THEN 'Pending' WHEN A.[Status] IN (1, 3) Then 'Completed' WHEN A.[Status] = 2 Then 'Cancelled' ELSE 'Panding' END) as [Status] From ESBTTRAN1 A Left Join Master1 M On A.MasterCode1 = M.Code Left Join Master1 M1 On A.MasterCode2 = M1.Code Where A.[MasterCode2] = {MCCode} And A.VchType = 108 And A.[Status] <> 2 ";
                        
                        sql = $"Select A.[VchCode], IsNull(A.[VchNo], '') as VchNo, Convert(Varchar, A.[Date], 105) as VchDate, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(A.[Name1], '') as AccName, IsNull(A.[MasterCode2], 0) as MCCode, IsNull(A.[Name2], '') as MCName, IsNull(A.[Container], '') as Container, IsNull(A.[D1], 0) as TotCart, IsNull(A.[D2], 0) as TotQty, IsNull(A.[D3], 0) as TotPcs, (CASE WHEN A.[Status] = 0 THEN 'Pending' WHEN A.[Status] IN (1, 3) Then 'Completed' WHEN A.[Status] = 2 Then 'Cancelled' ELSE 'Panding' END) as [Status] From ESBTTRAN1 A Where A.[MasterCode2] = {MCCode} And A.[VchType] = 108 And A.[Status] <> 2 ";
                        if (Status > 0) sql += (Status == 1) ? " And (A.[Status] Is Null OR A.[Status] = 0)" : " And (A.[Status] IN (1, 3))";
                        if (Status == 2) sql += $" And ValidateBy = '{Users}'" ;
                        if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And A.[Date] >= '{formattedStartDate}' And A.[Date] <= '{formattedEndDate}'";

                        break;
                    case 2:

                        //sql = $"Select A.[VchCode], IsNull(LTrim(A.[VchNo]), '') as VchNo, Convert(Varchar, A.[Date], 105) as VchDate, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(A.[MasterCode2], 0) as MCCode, IsNull(M1.[Name], '') as AccName, IsNull(M2.[Name], '') as MCName, '' as Container, IsNull((select COUNT(BCN) as TotCart From ItemParamDet Where VchCode = A.VchCode And VchType = 11), 0) as TotCart, IsNull((Sum(B.[Value1]) * (-1)), 0) as TotQty, 0 as TotPcs, (CASE WHEN A.[Flag] = 1 THEN 'Pending' WHEN A.[Flag] = 2 Then 'Completed' END) as [Status] From Tran1 A INNER JOIN TRAN2 B ON A.VchCode = B.VchCode Left Join Master1 M1 On A.[MasterCode1] = M1.Code Left Join Master1 M2 On A.[MasterCode2] = M2.Code Where A.VchType = 11 ";

                        sql = $"Select A.[VchCode], IsNull(LTrim(A.[VchNo]), '') as VchNo, Convert(Varchar, A.[Date], 105) as VchDate, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(A.[AccName], '') as AccName, IsNull(A.[MasterCode2], 0) as MCCode, IsNull(A.[MCName], '') as MCName, '' as Container, IsNull((select COUNT(BCN) as TotCart From ESBZItemParamDet Where VchCode = A.VchCode And VchType = 11), 0) as TotCart, IsNull(Sum(B.[Value1]), 0) as TotQty, 0 as TotPcs, (CASE WHEN A.[Status] = 1 THEN 'Pending' WHEN A.[Status] IN (2, 3) Then 'Completed' END) as [Status] From ESBZTran1 A INNER JOIN ESBZTRAN2 B ON A.[VchCode] = B.[VchCode] Where A.[VchType] = 11 ";
                        sql += Status == 0 ? " And A.[Status] IN (1, 2, 3)" : Status == 1 ? " And A.[Status] = 1" : " And A.[Status] IN (2, 3) ";
                        if (Status == 2) sql += $" And ValidateBy = '{Users}'" ;
                        if (!string.IsNullOrEmpty(formattedStartDate) && !string.IsNullOrEmpty(formattedEndDate)) sql += $" And A.[Date] >= '{formattedStartDate}' And A.[Date] <= '{formattedEndDate}'"; 
                        sql += "Group By A.[VchCode], A.[VchNo], A.[Date], A.[MasterCode1], A.[AccName], A.[MasterCode2], A.[MCName], A.[Status]";
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

                        //sql = $"Select A.SNo, A.[VchCode], IsNull(A.[ItemCode], 0) as ItemCode, IsNull(M.[Name], '') as ItemName, IsNull(A.[Value1], 0) as Carton, IsNull(A.[Value2], 0) as Qty, IsNull(A.[Value3], 0) as Pcs, IsNull((Select COUNT(BCN) as VAL From ESBTBCN B Where A.VchCode = B.VchCode And A.SNo = B.ISrNo And A.ItemCode = B.MasterCode2 And [RecType] = 1 And [Status] = 1 And Len(B.[BCN]) > 0), 0) as ScanQty From ESBTTRAN2 A INNER JOIN Master1 M On A.[ItemCode] = M.[Code] And M.[MasterType] = 6 Where A.[VchCode] = {VchCode} And A.[VchType] = 108 And A.RecType = 1 Order By A.[SNo], M.[Name]";

                        sql = $"Select A.SNo, A.[VchCode], IsNull(A.[ItemCode], 0) as ItemCode, IsNull(A.[ItemName], '') as ItemName, IsNull(A.[Value1], 0) as Carton, IsNull(A.[Value2], 0) as Qty, IsNull(A.[Value3], 0) as Pcs, IsNull((Select COUNT(BCN) as VAL From ESBTBCN B Where A.VchCode = B.VchCode And A.SNo = B.ISrNo And A.ItemCode = B.MasterCode2 And [RecType] = 1 And [Status] = 1 And Len(B.[BCN]) > 0), 0) as ScanQty From ESBTTRAN2 A Where A.[VchCode] = {VchCode} And A.[VchType] = 108 And A.RecType = 1 Order By A.[SNo], A.[ItemName]";
                        break;
                    case 2:
                        //sql = $"Select A.SrNo as SNo, A.[VchCode], IsNull(A.[MasterCode1], 0) as ItemCode, IsNull(M1.[Name], '') as ItemName, 0 as Carton, (IsNull(A.[Value1], 0) * (-1)) as Qty, 0 as Pcs, IsNull((Select COUNT(BCN) as VAL From ESBTBCN B Where A.[VchCode] = B.[VchCode] And A.[SrNo] = B.[ISrNo] And A.[MasterCode1] = B.[MasterCode2] And B.[RecType] = 2 And [Status] = 1 And Len(B.[BCN]) > 0), 0) as ScanQty From TRAN2 A INNER JOIN Master1 M1 On A.[MasterCode1] = M1.[Code] And M1.[MasterType] = 6 Where A.[VchCode] = {VchCode} And A.[VchType] = 11 And A.RecType = 2 Order By A.[SrNo], M1.[Name]";

                        sql = $"Select A.SNo as SNo, A.[VchCode], IsNull(A.[ItemCode], 0) as ItemCode, IsNull(A.[ItemName], '') as ItemName, IsNull((Select COUNT(BCN) as VAL From ESBZItemParamDet Where VchCode = A.VchCode And ItemCode = A.ItemCode ANd VchItemSN = A.SNo And RecType = 1), 0) as Carton, IsNull(A.[Value1], 0) as Qty, 0 as Pcs, IsNull((Select COUNT(BCN) as VAL From ESBTBCN B Where A.[VchCode] = B.[VchCode] And A.[SNo] = B.[ISrNo] And A.[ItemCode] = B.[MasterCode2] And B.[RecType] = 2 And [Status] = 1 And Len(B.[BCN]) > 0), 0) as ScanQty From ESBZTRAN2 A Where A.[VchCode] = {VchCode} And A.[VchType] = 11 And A.RecType = 2 Order By A.[SNo], A.[ItemName]";
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

                string sql = $"Select VchCode, IsNull(ISrNo, 0) as ISNo, IsNull(MasterCode2, 0) as ItemCode, IsNull(BCNSrNo, 0) as SNo, IsNull(BCN, '') as BCN, IsNull(Value1, 0) as Qty From ESBTBCN Where [VchCode] = {VchCode} And [ISrNo] = {ISNo} And [MasterCode2] = {ItemCode} And [RecType] = {TranType} And [Status] IN (1, 2) Order By [ISrNo], [BCNSrNo], [MasterCode2]";
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

        //[HttpGet]
        //public dynamic GetBCNDetailsValidate(string CompCode, string FY, int TranType, int VchCode, string BCN)
        //{
        //    GetBCNListDT B_List = new GetBCNListDT(); string sql = string.Empty;
        //    try
        //    {
        //        string constr = GetConnectionString(Provider, CompCode, FY);

        //        switch (TranType)
        //        {
        //            case 1:
        //                sql = $"Select VchCode, IsNull(ISrNo, 0) as ISNo, IsNull(MasterCode2, 0) as ItemCode, IsNull(BCNSrNo, 0) as SNo, IsNull(BCN, '') as BCN, IsNull(Value1, 0) as Qty From ESBTBCN Where VchCode = {VchCode} And BCN = '{BCN.Replace("'", "''")}' And ([Status] Is Null OR [Status] IN (0, 1)) Order By ISrNo, BCNSrNo, MasterCode2";
        //                break;

        //            case 2:
        //                sql = $"Select VchCode, IsNull(CM1, 0) as AccCode, IsNull(VchItemSN, 0) as ISNo,  IsNull(ItemCode, 0) as ItemCode, IsNull(SrNo, 0) as SNo, IsNull(BCN, '') as BCN, IsNull(Value1, 0) as Qty From ESBZItemParamDet Where VchCode = {VchCode} And BCN = '{BCN.Replace("'", "''")}' Order By VchItemSN, SrNo, ItemCode";
        //                break;
        //        }
        //        DataTable DT1 = new SQLHELPER(constr).getTable(sql);

        //        if (DT1 != null && DT1.Rows.Count > 0)
        //        {
        //            B_List.VchCode = Convert.ToInt32(DT1.Rows[0]["VchCode"]);
        //            B_List.ISNo = Convert.ToInt32(DT1.Rows[0]["ISNo"]);
        //            B_List.ItemCode = Convert.ToInt32(DT1.Rows[0]["ItemCode"]);
        //            B_List.SNo = Convert.ToInt32(DT1.Rows[0]["SNo"]);
        //            B_List.BCN = Convert.ToString(DT1.Rows[0]["BCN"]);
        //            B_List.Qty = Convert.ToDecimal(DT1.Rows[0]["Qty"]);

        //            if (TranType == 1)
        //            {
        //                sql = $"Update ESBTBCN Set [Status] = 1 Where [VchCode] = {VchCode} And [ISrNo] = {Convert.ToInt32(DT1.Rows[0]["ISNo"])} And [MasterCode2] = {Convert.ToInt32(DT1.Rows[0]["ItemCode"])} And [BCN] = '{BCN}' And RecType = 1";
        //            }
        //            else
        //            {
        //                sql = $"Delete From ESBTBCN Where [VchCode] = {VchCode} And [ISrNo] = {Convert.ToInt32(DT1.Rows[0]["ISNo"])} And [MasterCode2] = {Convert.ToInt32(DT1.Rows[0]["ItemCode"])} And [BCN] = '{BCN}' And RecType = 2" ;
        //                new SQLHELPER(constr).ExecuteSQL(sql);

        //                sql = $"INSERT INTO ESBTBCN ([VchCode], [RecType], [MasterCode1], [MasterCode2], [ISRNO], [BCNSRNO], [BCN], [AutoBCNNO], [Value1], [Value2], [Status]) Values ({Convert.ToInt32(DT1.Rows[0]["VchCode"])}, 2, {Convert.ToInt32(DT1.Rows[0]["AccCode"])}, {Convert.ToInt32(DT1.Rows[0]["ItemCode"])}, {Convert.ToInt32(DT1.Rows[0]["ISNo"])}, {Convert.ToInt32(DT1.Rows[0]["SNo"])}, '{Convert.ToString(DT1.Rows[0]["BCN"])}', 0,  {Convert.ToDouble(DT1.Rows[0]["Qty"])},  {Convert.ToDouble(DT1.Rows[0]["Qty"])}, 1)";
        //            }
        //            int DT2 = new SQLHELPER(constr).ExecuteSQL(sql);
        //        }
        //        else
        //        {
        //            throw new Exception("BCN Not Found !!!");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new { Status = 0, Msg = ex.Message.ToString() };
        //    }
        //    return new { Status = 1, Msg = "BCN Valid", Data = B_List };
        //}


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

                        BCN = BCN.Replace('；', ';');
                        string[] bcnParts = BCN.Split(';').Select(b => b.Trim()).ToArray();

                        if (bcnParts.Length != 2)
                        {
                            throw new Exception($"Invalid BCNSTR format: {bcnParts}");
                        }

                        string itemAliasOrName = bcnParts[0].Trim();
                        string bcn = Convert.ToString(bcnParts[1].Trim());

                        var itemDetails = ValidateItemNameInBusy(VchCode, bcnParts[0].Trim(), constr);

                        //sql = $"Select VchCode, IsNull(ISrNo, 0) as ISNo, IsNull(MasterCode2, 0) as ItemCode, IsNull(BCNSrNo, 0) as SNo, IsNull(BCN, '') as BCN, IsNull(Value1, 0) as Qty From ESBTBCN Where VchCode = {VchCode} And BCN = '{BCN.Replace("'", "''")}' And ([Status] Is Null OR [Status] IN (0, 1)) Order By ISrNo, BCNSrNo, MasterCode2";

                        sql = $"Select A.VchCode, IsNull(A.[MasterCode1], 0) as AccCode, IsNull(B.[SNo], 0) as ISNo, IsNull(B.[ItemCode], 0) as ItemCode, IsNull(B.[Value2], 0) as Qty, (IsNull((Select MAX(B.BcnSrNo) From ESBTBCN B Where B.VchCode = A.VchCode), 0) + 1) as SNo, '{bcn}' as BCN From ESBTTran1 A INNER JOIN ESBTTran2 B ON A.[VchCode] = B.[VchCode] Where A.[VchCode] = {VchCode} And B.[ItemCode] = {itemDetails?.Code} Order By B.[SNo], B.ItemCode";
                        break;

                    case 2:
                        sql = $"Select VchCode, IsNull(CM1, 0) as AccCode, IsNull(VchItemSN, 0) as ISNo,  IsNull(ItemCode, 0) as ItemCode, IsNull(SrNo, 0) as SNo, IsNull(BCN, '') as BCN, IsNull(Value1, 0) as Qty From ESBZItemParamDet Where VchCode = {VchCode} And BCN = '{BCN.Replace("'", "''")}' Order By VchItemSN, SrNo, ItemCode";
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
                        //sql = $"Update ESBTBCN Set [Status] = 1 Where [VchCode] = {VchCode} And [ISrNo] = {Convert.ToInt32(DT1.Rows[0]["ISNo"])} And [MasterCode2] = {Convert.ToInt32(DT1.Rows[0]["ItemCode"])} And [BCN] = '{BCN}' And RecType = 1";

                        new SQLHELPER(constr).getTable($"Delete From ESBTBCN Where [RecType] = 1 And [VchCode] = {Convert.ToInt32(DT1.Rows[0]["VchCode"])} And [MasterCode2] = {Convert.ToInt32(DT1.Rows[0]["ItemCode"])} And [BCN] = '{Convert.ToString(DT1.Rows[0]["BCN"])}'");

                        sql = $"INSERT INTO ESBTBCN ([VchCode], [RecType], [MasterCode1], [MasterCode2], [ISRNO], [BCNSRNO], [BCN], [AutoBCNNO], [Value1], [Value2], [Status]) Values ({Convert.ToInt32(DT1.Rows[0]["VchCode"])}, 1, {Convert.ToInt32(DT1.Rows[0]["AccCode"])}, {Convert.ToInt32(DT1.Rows[0]["ItemCode"])}, {Convert.ToInt32(DT1.Rows[0]["ISNo"])}, {Convert.ToInt32(DT1.Rows[0]["SNo"])}, '{Convert.ToString(DT1.Rows[0]["BCN"])}', 0,  {Convert.ToDouble(DT1.Rows[0]["Qty"])},  {Convert.ToDouble(DT1.Rows[0]["Qty"])}, 1)";
                        int DT2 = new SQLHELPER(constr).ExecuteSQL(sql);
                    }
                    else
                    {
                        sql = $"Delete From ESBTBCN Where [VchCode] = {VchCode} And [ISrNo] = {Convert.ToInt32(DT1.Rows[0]["ISNo"])} And [MasterCode2] = {Convert.ToInt32(DT1.Rows[0]["ItemCode"])} And [BCN] = '{BCN}' And RecType = 2";
                        new SQLHELPER(constr).ExecuteSQL(sql);

                        sql = $"INSERT INTO ESBTBCN ([VchCode], [RecType], [MasterCode1], [MasterCode2], [ISRNO], [BCNSRNO], [BCN], [AutoBCNNO], [Value1], [Value2], [Status]) Values ({Convert.ToInt32(DT1.Rows[0]["VchCode"])}, 2, {Convert.ToInt32(DT1.Rows[0]["AccCode"])}, {Convert.ToInt32(DT1.Rows[0]["ItemCode"])}, {Convert.ToInt32(DT1.Rows[0]["ISNo"])}, {Convert.ToInt32(DT1.Rows[0]["SNo"])}, '{Convert.ToString(DT1.Rows[0]["BCN"])}', 0,  {Convert.ToDouble(DT1.Rows[0]["Qty"])},  {Convert.ToDouble(DT1.Rows[0]["Qty"])}, 1)";
                        int DT2 = new SQLHELPER(constr).ExecuteSQL(sql);
                    }
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

        private UnknowList ValidateItemNameInBusy(int VchCode, string ItemName, string ConStr)
        {
            // Ensure input validation
            if (string.IsNullOrWhiteSpace(ItemName))
                throw new ArgumentException("Item name cannot be null or empty.", nameof(ItemName));

            if (string.IsNullOrWhiteSpace(ConStr))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(ConStr));

            UnknowList itemDetails = null;
            try
            {

                string sql = $"Select Top 1 IsNull([ItemCode], 0) as ItemCode, IsNull([ItemName], '') as ItemName From ESBTTran2 Where VchCode = {VchCode} And ([ItemName] = '{ItemName}' Or ItemAlias = '{ItemName}')";
                DataTable DT1 = new SQLHELPER(ConStr).getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    itemDetails = new UnknowList
                    {
                        Code = Convert.ToInt32(DT1.Rows[0]["ItemCode"]),
                        Name = Convert.ToString(DT1.Rows[0]["ItemName"])
                    };
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"Error validating item name: {ex.Message}", ex);
            }
            return itemDetails ?? throw new Exception($"Item name : '{ItemName}' not belong to this voucher.");
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
                    new SqlParameter("@Users", SqlDbType.VarChar, 90) { Value = obj.Users },
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
                string XML = CreateXML(obj.MItemsDetails); string sql = string.Empty;
                string CurrentDate = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");

                sql = $"Update ESBZTran1 Set [Status] = 2, [ValidateBy] = '{obj.Users}', [ValidateTime] = '{CurrentDate}' Where VchCode = {obj.VchCode} And VchType = 11";
                int DT1 = new SQLHELPER(constr).ExecuteSQL(sql);

                new SQLHELPER(constr).ExecuteSQL($"Update ESBTBCN Set [Status] = 2 Where [VchCode] = {obj.VchCode} And [RecType] = 2");

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
                //DBName = "Busy" + CompCode + "_db1" + FY;
                DBName = "ESBT" + CompCode + "_" + FY;
                ConnectionString = "Data Source = " + ServerName + "; Initial catalog = " + DBName + "; Uid = " + SUserName + "; Pwd =" + SPassword + ";Max Pool Size=500";
            }

            return ConnectionString;
        }
    }
}
