using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace BETA_3_API.Models
{
    public class UnknowList
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class Validate_User
    {
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public int UTCode { get; set; }
        public string UTName { get; set; }
        public int MCCode { get; set; }
        public string MCName { get; set; }
    }

    public class Dashboard
    {
        public string Name { get; set; }
        public int Values { get; set; }
    }

    public class GetVoucherList
    {
        public int VchCode { get; set; }
        public string VchDate { get; set; }
        public string VchNo { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public int MCCode { get; set; }
        public string MCName { get; set; }
        public string Container { get; set; }
        public decimal TotCart { get; set; }
        public decimal TotQty { get; set; }
        public decimal TotPcs { get; set; }
        public string Status { get; set; }
    }

    public class GetVoucherItemsList
    {
        public int VchCode { get; set; }
        public int SNo { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Carton { get; set; }
        public decimal Qty { get; set; }
        public decimal Pcs { get; set; }
        public decimal ScanQty { get; set; }
        public List<GetBCNListDT> BCNDetails { get; set; }
    }

    public class GetBCNListDT
    {
        public int VchCode { get; set; }
        public int ISNo { get; set; }
        public int ItemCode { get; set; }
        public int SNo { get; set; }
        public string BCN { get; set; }
        public decimal Qty { get; set; }
    }


    public class S_Mat_Vch
    {
        public int VchCode { get; set; }
        public int VchType { get; set; }
        public int Status { get; set; }
        public List<MItemDetails> MItemsDetails { get; set; }
    }
    public class MItemDetails
    {
        public int VchCode { get; set; }
        public int SNo { get; set; }
        public int ItemCode { get; set; }
        public decimal Carton { get; set; }
        public decimal Qty { get; set; }
        public decimal ScanQty { get; set; }
        public List<BCNDetails> BCNDetails { get; set; }
    }

    public class BCNDetails
    {
        public int VchCode { get; set; }
        public int ISNo { get; set; }
        public int ItemCode { get; set; }
        public string BCN { get; set; }
    }

    //public class MBCNList
    //{
    //    public int VchCode { get; set; }
    //    public int ISNo { get; set; }
    //    public int ItemCode { get; set; }
    //    public string BCN { get; set; }
    //}

    public class CCustomer
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string GSTNO { get; set; }
        public int CNCode { get; set; }
        public int STCode { get; set; }
        public int CTCode { get; set; }
        public string Address { get; set; }
        public string OrgName { get; set; }
        public string PAN { get; set; }
        public int CustType { get; set; }
        public string Users { get; set; }
    }

    public class GetCustomerDT
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public int GstType { get; set; }
    }
    public class CustomerList
    {
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string FirmName { get; set; }
        public string Address { get; set; }
        public int CTCode { get; set; }
        public string CTName { get; set; }
        public int Deactive { get; set; }
    }

    public class Customers
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string FirmName { get; set; }
        public string GSTIN { get; set; }
        public int CNCode { get; set; }
        public string CNName { get; set; }
        public int STCode { get; set; }
        public string STName { get; set; }
        public int CTCode { get; set; }
        public string CTName { get; set; }
        public string Address { get; set; }
        public int CCode { get; set; }
        public string CName { get; set; }
        public int Deactive { get; set; }
    }

    public class Category
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public string Images { get; set; }

    }

    public class GetProduct
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Images { get; set; } = "";

        //public List<DataItem> DataItem { get; set; }
        public List<Mendatory> Mendatories { get; set; }
        public List<NonMendatory> NonMendatories { get; set; }
    }

    public class ItemDiscount
    {
        public decimal MRP { get; set; }
        public double Discount { get; set; }
        public decimal Price { get; set; }
    }

    public class ItemWithDiscount
    {
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public ItemDiscount ItemDiscount { get; set; }
    }

    public class ListOfItem
    {
        public List<ListOfItemDT> ItemDetails { get; set; }
    }

    public class ListOfItemDT
    {
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
    }

    public class GetProductsFiltWise
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<string> Images { get; set; }
        public List<SizeChart> SizeCharts { get; set; }

        //public List<DataItem> DataItem { get; set; }
        public List<Mendatory> Mendatories { get; set; }
        public List<NonMendatory> NonMendatories { get; set; }
    }


    public class DataItem
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int Mendatory { get; set; }
        public List<SubData> SubData { get; set; }
    }

    public class Mendatory
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int Selected { get; set; }
        public List<SubData> SubData { get; set; }
    }

    public class NonMendatory
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int Selected { get; set; }
        public List<SubData> SubData { get; set; }
    }

    public class SubData
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class SizeChart
    {
        public int WCode { get; set; }
        public string WName { get; set; }
        public string Size { get; set; }
        public string Box { get; set; }
    }


    public class GetSerchProducts
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class GetItemsDTStock
    {
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public string GSTSlabs { get; set; }
        public int GSTRate { get; set; }
        public string Brand { get; set; }
        public int UCode { get; set; }
        public string UName { get; set; }
        public decimal Qty { get; set; }
        public decimal Stock { get; set; }
        public List<string> Images { get; set; }
        public ItemDiscount ItemDiscounts { get; set; }
    }

    public class GetItemsStock
    {
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public int UCode { get; set; }
        public string UName { get; set; }
        public decimal Stock { get; set; }
        public decimal Price { get; set; }
        public string Images { get; set; }
    }

    public class TransactionData
    {
        public int VchCode { get; set; }
        public int CustId { get; set; }
        public string CustName { get; set; }
        public string CMobile { get; set; }
        public double TQty { get; set; }
        public double TAmt { get; set; }
        public double Discount { get; set; }
        public double TaxType { get; set; }
        public double TaxAmt1 { get; set; }
        public double TaxAmt2 { get; set; }
        public double NetAmt { get; set; }
        public string Users { get; set; }

        [XmlArray("ItemDetail")]
        [XmlArrayItem("ItemDetail")]
        public List<ItemDetail> ItemDetails { get; set; }
        //public List<BSDetail> BSDetails { get; set; }
    }

    public class ItemDetail
    {
        public int PCode { get; set; }
        public int ItemCode { get; set; }
        public double Qty { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public double MRP { get; set; }
        public double ItemDisc { get; set; }
        public double GstType { get; set; }
        public double GstAmt { get; set; }
        public int UCode { get; set; }

        [XmlArray("Attribute")]
        [XmlArrayItem("Attribute")]
        public List<Attribute> Attributes { get; set; }
    }

    public class BSDetail
    {
        public int SrNo { get; set; }
        public int BSCode { get; set; }
        public string BSName { get; set; }
        public double BSPer { get; set; }
        public double Amount { get; set; }
        public string PercentOperatedOn { get; set; }

        //    public string SrNo { get; set; }
        //    public string BSName { get; set; }
        //    public int BSCode { get; set; }
        //    public string PercentVal { get; set; }
        //    public string PercentOperatedOn { get; set; }
        //    public string Amt { get; set; }
        //    public string Date { get; set; }
        //    public string VchNo { get; set; }
        //    public string VchType { get; set; }
        //    public string TmpVchCode { get; set; }
    }

    public class Attribute
    {
        public int UCode { get; set; }
        public int ATCode { get; set; }
    }

    public class VchConfig
    {
        public int TranType { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int StartNo { get; set; }
        public int Padding { get; set; }
        public int PaddLength { get; set; }
        public char PaddChar { get; set; }
    }

    public class GetVchDT
    {
        public int VchCode { get; set; }
        public string Date { get; set; }
        public string VchNo { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Mobile { get; set; }
        public decimal TQty { get; set; }
        public decimal TAmt { get; set; }
        public decimal NetAmt { get; set; }
        public int Status { get; set; }
        public string SName { get; set; }
    }

    public class GetSTVchDT
    {
        public int VchCode { get; set; }
        public string Date { get; set; }
        public string VchNo { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Mobile { get; set; }
        public decimal TQty { get; set; }
        public decimal TAmt { get; set; }
        public decimal NetAmt { get; set; }
        public string Remarks { get; set; }
        public int BusyVchCode { get; set; }
    }

    public class GetItemsDT
    {
        public int VchCode { get; set; }
        public int SNo { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string Images { get; set; }
    }

    public class GetAttributes
    {
        public int HCode { get; set; }
        public string HName { get; set; }
        public int ATCode { get; set; }
        public string ATName { get; set; }
    }

    public class FollowUp
    {
        public int VchCode { get; set; }
        public int VchType { get; set; }
        public string Remarks { get; set; }
        public string FollowdBy { get; set; }
        public string Users { get; set; }
    }

    public class GetFollowUp
    {
        public int SNo { get; set; }
        public string FDate { get; set; }
        public string Person { get; set; }
        public string Remarks { get; set; }
    }

    public class GetOrderPrinting
    {
        public int VchCode { get; set; }
        public string Date { get; set; }
        public string VchNo { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Mobile { get; set; }
        public decimal TQty { get; set; }
        public decimal TAmt { get; set; }
        public decimal NetAmt { get; set; }
        public int QStatus { get; set; }
        public string QName { get; set; }
        public List<PrintItemsDT> ItemsDetails { get; set; }
    }

    public class PrintItemsDT
    {
        public int SNo { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string Images { get; set; }
    }

    public class VchApprovel
    {
        public int VchCode { get; set; }
        public int SCode { get; set; }
        public string Remarks { get; set; }
        public string ApprovedBy { get; set; }
        public int ReceiptYesNo { get; set; }
    }

    public class VchUpdateList
    {
        public int VchCode { get; set; }
        public string VchNo { get; set; }
        public string Date { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string GSTIN { get; set; }
        public string Address { get; set; }
        public decimal TotQty { get; set; }
        public decimal TransQty { get; set; }
        public decimal BQty { get; set; }
        public string Status { get; set; }
    }

    public class SalesManOrdersDt
    {
        public int VchCode { get; set; }
        public string Date { get; set; }
        public string VchNo { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Mobile { get; set; }
        public decimal TQty { get; set; }
        public decimal TAmt { get; set; }
        public decimal NetAmt { get; set; }
        public int SCode { get; set; }
        public string SName { get; set; }
        public int CTCode { get; set; }
        public string CTName { get; set; }
        public string Remarks { get; set; }
    }

    public class SalesmanOrderAmount
    {
        public decimal QuotaionAmt { get; set; }
        public decimal InvoiceAmt { get; set; }
    }

    public class GraffYear
    {
        public int Year { get; set; }
        public List<Months> Months { get; set; }
    }
    public class Months
    {
        public int MonthsCode { get; set; }
        public string MonthsName { get; set; }
        public decimal QuotaionAmt { get; set; }
        public decimal InvoiceAmt { get; set; }
    }

    public class GetPrice
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }

    public class GetStockTransferQuotDt
    {
        public int VchCode { get; set; }
        public string VchNo { get; set; }
        public string VchDate { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string CMobile { get; set; }
        public string CEmail { get; set; }
        public string CGSTIN { get; set; }
        public string CAddress { get; set; }
    }

    public class STQuotWiseDt
    {
        public int VchCode { get; set; }
        public string VchNo { get; set; }
        public string VchDate { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string CMobile { get; set; }
        public string CEmail { get; set; }
        public string CGSTIN { get; set; }
        public string CAddress { get; set; }
        public List<STQuotItemsDt> QuotationItems { get; set; }
    }

    public class STQuotItemsDt
    {
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public int UCode { get; set; }
        public string UName { get; set; }
        public decimal Qty { get; set; }
        public decimal Stock { get; set; }
        public decimal TransQty { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }

    public class STVchDetail
    {
        public int VchCode { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public int MCCode { get; set; }
        public string MCName { get; set; }
        public string Mobile { get; set; }
        public string Remarks { get; set; }
        public double TQty { get; set; }
        public double TAmt { get; set; }
        public double NetAmt { get; set; }
        public string Users { get; set; }

        //[XmlArray("STItemDetail")]
        //[XmlArrayItem("STItemDetail")]
        public List<STItemDetail> STItemDetails { get; set; }
    }

    public class STItemDetail
    {
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public int UCode { get; set; }
        public string UName { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }

    public class STVerificationVchDT
    {
        public int VchCode { get; set; }
        public List<STVItemDetail> STVItemDetails { get; set; }
    }

    public class STVItemDetail
    {
        public int VchCode { get; set; }
        public string VchNo { get; set; }
        public string Date { get; set; }
        public int AccCode { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public int UCode { get; set; }
        public string UName { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }

    public class PendingPackingDT
    {
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string GSTIN { get; set; }
        public string Address { get; set; }
        public double TQty { get; set; }
    }
    public class PendingPackingItemsDT
    {
        public int SNo { get; set; }
        //public int VchCode { get; set; }
        //public string VchNo { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public decimal PQty { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
    }

    public class PackingSaved
    {
        public int VchCode { get; set; }
        public string VchNo { get; set; }
        public string VchDate { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Mobile { get; set; }
        public double TotQty { get; set; }
        public double TotAmt { get; set; }
        public double NetAmt { get; set; }
        public string Users { get; set; }
        public List<BItemDetail> BItemDetails { get; set; }
    }

    public class BItemDetail
    {
        public int SNo { get; set; }
        public int RefCode { get; set; }
        public string RefNo { get; set; }
        public int AccCode { get; set; }
        public int ItemCode { get; set; }
        public double Qty { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
    }
    public class GetPackingVch
    {
        public int VchCode { get; set; }
        public string VchDate { get; set; }
        public string VchNo { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Mobile { get; set; }
        public decimal TQty { get; set; }
        public decimal TAmt { get; set; }
        public decimal NetAmt { get; set; }
        public string Status { get; set; }
        public int InvVchCode { get; set; }
        public string InvVchDate { get; set; }
        public string InvVchNo { get; set; }
    }

    public class AutoOrderDT
    {
        public int SeriesCode { get; set; }
        public string SeriesName { get; set; }
        public int MasterCode1 { get; set; }
        public string Name1 { get; set; }
        public int MasterCode2 { get; set; }
        public string Name2 { get; set; }
    }

    public class FollowUpRpt
    {
        public int VchCode { get; set; }
        public string VchNo { get; set; }
        public string Date { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Status { get; set; }
        public string FollowedBy { get; set; }
        public string FollowedOn { get; set; }
    }

    public class CancelVoucher
    {
        public int VchCode { get; set; }
        public List<CancelItem> CancelItems { get; set; }
    }

    public class CancelItem
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public int ItemCode { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }

    public class VerificationVchList
    {
        public int VchCode { get; set; }
        public string VchNo { get; set; }
        public string Date { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string GSTIN { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public string SName { get; set; }
    }

    public class PaymentReceipt
    {
        public int VchCode { get; set; }
        public string VchNo { get; set; }
        public string VchDate { get; set; }
        public int AccCode { get; set; }
        public string AccName { get; set; }
        public string Mobile { get; set; }
        public decimal Amount { get; set; }
        public string RVchDate { get; set; }
        public string RVchNo { get; set; }
        public decimal RAmount { get; set; }
    }

    public class SLogo
    {
        public string CompCode { get; set; }
        public string Fy { get; set; }
        public string ImgStr { get; set; }
    }
    public class Banner
    {
        public string URL { get; set; }    
    }

    public class UserRegister
    {
        public string Message { get; set; }
        public int Status { get; set; }
    }
    public class PartySaleType
    {
        public string SType { get; set; }
        public int STypeCode { get; set; }
    }
    public class UserWiseData
    {
        public string CreateDate { get; set; }
        public int UCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
    }
    public class UserData
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string PWD { get; set; }
    }
    public class Data
    {
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public string UserType { get; set; }
      
      
    }
    public class OTPVerifyData
    {
        public int ModeSMS { get; set; }
        public string SMSFormat { get; set; }
        public int ModeWA { get; set; }
        public string WAType { get; set; }
        public string WAURL { get; set; }
        public string WAUserName { get; set; }
        public string WAPassword { get; set; }
        public string SMSBody { get; set; }
    }
    public class SMSAPI
    {
        public string SMSAPIFormat { get; set; }
        public string SMSAPIUNameP { get; set; }
        public string SMSAPIUNameV { get; set; }
        public string SMSAPIUPassP { get; set; }
        public string SMSAPIUPassV { get; set; }
        public string SMSAPISenderIDP { get; set; }
        public string SMSAPISenderIDV { get; set; }
        public string SMSAPIMobileP { get; set; }
        public string SMSAPIMsgP { get; set; }
    }
    public class BeatDetails
    {
        public int BeatId { get; set; }
        public string Beat { get; set; }
        public string TotalParties { get; set; }
        public string Visited { get; set; }
        public string Target { get; set; }
        public string Achived { get; set; }

    }
    public class BeatList
    {
        public string BeatId { get; set; }
        public string BeatName { get; set; }
        public string BType { get;  set; }
    }
    public class Location
    {
        public int PartyCode { get; set; }
        public int UserId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CAddress { get; set; }
        public string Message { get; set; }
        public int Sucess { get; set; }
    }

    public class Alert
    {
        public string Message { get; set; }
        public int Sucess { get; set; }

    }
    public class AlertOrder
    {
        public int Status { get; set; }
        public string Msg { get; set; }
        public int OrderId { get; set; }
    }
    public class AlertPayment
    {
        public string Message { get; set; }
        public int Sucess { get; set; }
        public int PaymentId { get; set; }
    }
    public class PardoshMList
    {

        public string MCCode {get;set;}
        public string Code { get; set; }
        public string Name { get; set; }

        public string Unit { get; set; }
    }
    public class GroupList
    {
        public int Code { get; set; }
        public string Name { get; set; }

    }
    public class PardoshItemList
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }

    }
    public class BillSundrylist
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int BSType { get; set; }
        public string BSTypeName { get; set; }
        public double BSAmount { get; set; }
        public int CalBasis { get; set; }
        public string CalBasisName { get; set; }
        public double BSPercent { get; set; }
        public int AppliedOn { get; set; }
        public int BSNature { get; set; }
        public int PercentOperatedOn { get; set; }
    }
    public class PardoshAccountList
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string GSTNo { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
         public string PAN { get; set; }
        public string Email { get; set; }
        public string PINCode { get; set; }
        public string State { get; set; }
        public int StateCode { get; set; }
        public double  Balance { get; set; }

    }
    public class DefaultMaster
    {
        public int SJSeries { get; set; }
        public int Series { get; set; }
        public int ThicknessCode { get; set; }
        public string ThicknessName { get; set; }
        public int GlassTypeCode { get; set; }
        public string GlassTypeName { get; set; }
        public int ColorCode { get; set; }
        public string ColorName { get; set; }
        public int VarietyCode { get; set; }
        public string VarietyName { get; set; }
        public int ChargeableCode { get; set; }
        public string ChargeableName { get; set; }
        public int OrderType { get; set; }
        public string OrderTypeName { get; set; }
        public int ProcessType { get; set; }
        public string ProcessTypeName { get; set; }
        public int Brand { get; set; }
        public string BrandName { get; set; }
        public int Process { get; set; }
        public string ProcessName { get; set; }

        public int Polish { get; set; }
        public string PolishName { get; set; }

        public int SplWork { get; set; }
        public string SplWorkName { get; set; }
        public int ThicknessItem { get; set; }
        public string ThicknessItemName { get; set; }
        public int Organgas { get; set; }
        public string OrgangasName { get; set; }

        public double ThicknessItemPrice { get; set; }
        public double SplWorkItemPrice { get; set; }
        public double ProcessItemPrice { get; set; }
        public double PolishItemPrice { get; set; }
        public string ThicknessItemUnit { get; set; }
        public string SplWorkItemUnit { get; set; }
        public string ProcessItemUnit { get; set; }
        public string PolishItemUnit { get; set; }
    }

    public class AccList
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
    public class VistedParty
    {
        public string SalesMan { get; set; }
        public string Party { get; set; }
        public string VistedDate { get; set; }
    }
    public class OrdDT
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public double OrderQty { get; set; }
        public double OrderAmt { get; set; }
        public double POrderQty { get; set; }
        public double POrderAmt { get; set; }
    }

    public class Accountdt
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int StPtCode { get; set; }
        public string StPtName { get; set; }
        public string Transport { get; set; }
        public string Station { get; set; }

        public string Mobile { get; set; }
        public string Address { get; set; }
    }
    public class RItem
    {
        public int ReVali { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public string NameAlias { get; set; }
        public double Price { get; set; }
        public double MinPrice { get; set; }
        public double AltPrice { get; set; }
        public string MainUnit { get; set; }
        public string AltUnit { get; set; }
        public int CFMode { get; set; }
        public double Confactor { get; set; }
        public double Stock { get; set; }
        public double DiscPer { get; set; }
    }


    public class Item
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string NameAlias { get; set; }
        public double Price { get; set; }
        public double MinPrice { get; set; }
        public double AltPrice { get; set; }
        public string MainUnit { get; set; }
        public string AltUnit { get; set; }
        public int  CFMode { get; set; }
        public double Confactor { get; set; }
        public double Stock { get; set; }
        public double DiscPer { get; set; }
    }

    public class PartyDetails
    {
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string EMail { get; set; }
        public int Visited { get; set; }
        public string ClosingBalance { get; set; }
        public string LastRecipt { get; set; }
        public string LastOrderItem { get; set; }
        public double Points { get; set; }
        public int CheckIn { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CAddress { get; set; }
        public int Approval { get; set; }
        public int CheckInstatus { get; set; }
        public int Revisit { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

    }
    public class PartyDtal
    {

        public string Message { get; set; }
        public int Sucess { get; set; }

    }
    public class CategoryList
    {

        public string Name { get; set; }
        public int Code { get; set; }
        public string CategoryImage { get; set; }

    }
    public class SubCategoryList
    {

        public string Name { get; set; }
        public int Code { get; set; }
        public int CategoryCode { get; set; }
        public string CategoryImage { get; set; }

    }
    public class BillByBillRef
    {
        public int RefCode { get; set; }
        public string RefNo { get; set; }
        public string Date { get; set; }
        public double Amount { get; set; }
        public string DueDate { get; set; }
        public double AdjAmount { get; set; }
    }


    public class MultipalProductImages
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public List<Image1> ProductImage { get; set; }

    }
    public class ProductList
    {

        public string SubCategoryCode { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public double MRP { get; set; }
        public double MinPrice { get; set; }
  
        public double AltPrice { get; set; }
        public double Stock { get; set; }
        public string Unit { get; set; }
        public string MinQTY { get; set; }
        public string MaxQTY { get; set; }
        public double DiscPer { get; set; }
        public double Confactor { get;set;}

        public int CFMode { get; set; }
        public int Code { get; set; }

        public string AltUnit { get; set; }
        public int Fevourite { get; set; }
        public string imgURL { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
    }

    public class CheckIn
    {
        public int PartyCode { get; set; }
        public string PartyName { get; set; }
        public int UserId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Message { get; set; }
        public int Sucess { get; set; }
        public int CheckedIn { get; set; }
        public int Visited { get; set; }
    }

    public class CheckInSFA
    {
        public int PartyCode { get; set; }
        public int UserId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CheckOut { get; set; }
        public string Revisit { get; set; }
        public string Distance { get; set; }
        public string Distance2 { get; set; }
        public string Description { get; set; }
        public string Image64 { get; set; }
        public string Address { get; set; }
        public string Exte { get; set; }
    }

    public class Country
    {
        public int CountryCode { get; set; }
        public string CountryName { get; set; }

    }
    public class State
    {
        public int StateCode { get; set; }
        public string StateName { get; set; }
    }
    public class SalesManList
    {
        public int SCode { get; set; }
        public string SName { get; set; }
    }

    public class AccGroup
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class ExpenseHead
    {
        public string Expense { get; set; }
    }
    public class ExpenseVch
    {
        public int SalesmanCode { get; set; }
        public string Salesman { get; set; }
        public double ExpAmount { get; set; }
        public List<Expense> Expenses { get; set; }
    }

    public class Expense
    {
        public string ExpenseH { get; set; }
        public string Transport { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string DocAttach { get; set; }
        public string DocBase64 { get; set; }
        public string FileExt { get; set; }
    }
    
    public class ProductPrice
    {
        public int ProductCode { get; set; }
        public List<MRPList> MRPList { get; set; }

    }
    public class MRPList
    {
        public double Price { get; set; }
        public double MRP { get; set; }
    }
    public class City
    {
        public int CityCode { get; set; }
        public string CityName { get; set; }
    }
    public class APIResponse
    {
        public string msg { get; set; }
        public int Sucess { get; set; }
        public int Value { get; set; }
    }

    public class SalesMantag
    {
        public int SCode { get; set; }
        public int PartyCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
    }
    public class Response
    {
        public string msg { get; set; }
        public int Sucess { get; set; }
        public List<CartDetails> CartList { get; set; }
    }



    public class  SFACatalougeOrder
    {
        public int SeriesCode { get; set; }
        public int STPTCode { get; set; }
        public int MCCode { get; set; }
        public string Transport { get; set; }
        public string Station { get; set; }
        public string Narration { get; set; }
        public List<addcart> Cart { get; set; }

        public List<BSDDetailCart> BSDDetailCart { get; set; }

    }
    //int SeriesCode,int STPTCode,int MCCode
    public class addcart
    {
        public int CartId { get; set; }
        public string ProductId { get; set; }
        public string SalesManId { get; set; }
        public string PartyId { get; set; }
        public string MinQty { get; set; }
        public string AltQty { get; set; }
        public string Confactor { get; set; }
        public string Unit { get; set; }
        public string AltUnit { get; set; }
        public string Price { get; set; }
        public string Disc { get; set; }
        public double DiscAmt { get; set; }
        public double SalePrice { get; set; }
        public string TotalPrice { get; set; }
        public string Status { get; set; }
        public int FItemCode { get; set; }
        public double FreeQty { get; set; }
        public double FItemPrice { get; set; }
        public int SchemeCode { get; set; }
        public int SchemeType { get; set; }
        public string Remarks { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }

        public int BillingUnit { get; set; }
    }

    public class AppLogInfo
    {
        public int UserType { get; set; }
        public int UserID { get; set; }
        public int MsgID { get; set; }
        public string MSG { get; set; }
        public string MasterName { get; set; }
    }

    public class AddFavourite
    {
        public int ProductId { get; set; }
        public int PartyId { get; set; }
        public int FType { get; set; }
    }

    public class Salesmencart
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int SalesManId { get; set; }
        public int PartyId { get; set; }
        public int MinQty { get; set; }
        public string AltQty { get; set; }
        public string Unit { get; set; }
        public string AltUnit { get; set; }
        public string Confactor { get; set; }
        public string Price { get; set; }
        public string Disc { get; set; }
        public double DiscAmt { get; set; }
        public double SalePrice { get; set; }
        public string TotalPrice { get; set; }
        public string PartyName { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int FItemCode { get; set; }
        public string FItemName { get; set; }
        public double FreeQty { get; set; }
        public double FItemPrice { get; set; }
        public int SchemeCode { get; set; }
        public int SchemeType { get; set; }
        public string Remarks { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public int BillingUnit { get; set; }

        //public List<Image1> ProductImage { get; set; }
    }
    public class CartDetails
    {
        public string ProductId { get; set; }
        public string MinQty { get; set; }
        public string SalesManId { get; set; }
        public string PartyId { get; set; }
        public string Price { get; set; }
        public string Status { get; set; }
        public string TotalPrice { get; set; }
        public string Disc { get; set; }
        public double DiscAmt { get; set; }
        public int FItemCode { get; set; }
        public double FreeQty { get; set; }
        public double FItemPrice { get; set; }
        public double SchemeCode { get; set; }
        public double SchemeType { get; set; }
        public int BillingUnit { get; set; }

    }
    public class CompDetail
    {
        public string CompName { get; set; }
        public string CompPrintNm { get; set; }
        public string Add1 { get; set; }
        public String Add2 { get; set; }
        public string Add3 { get; set; }
        public string Add4 { get; set; }
        public string pincode { get; set; }
        public string telno { get; set; }
        public String GSTNO { get; set; }
        public string TinNo { get; set; }


    }
    public class UpdateCart
    {

        public string Qty { get; set; }
        public string SalesManId { get; set; }
        public string PartyId { get; set; }
        public string Price { get; set; }
        public string CartId { get; set; }
        public string TotalPrice { get; set; }
        public string Disc { get; set; }
        public string DiscAmt { get; set; }
        public string FItemCode { get; set; }
        public string FreeQty { get; set; }
        public string FItemPrice { get; set; }
        public string SchemeCode { get; set; }
        public string SchemeType { get; set; }
    }
    public class Image1
    {

        public string ProductImage { get; set; }
    }


    //public partial class ImageDbEntities : DbContext
    //{
    //    public ImageDbEntities()
    //        : base("name=ImageDbEntities")
    //    {
    //    }

    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        throw new UnintentionalCodeFirstException();
    //    }

    //    public DbSet<Image> Images { get; set; }
    //}

    public partial class Image
    {
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
    }

    public class PaymentDetails
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public int SCode { get; set; }
        public int PartyCode { get; set; }
        public string PartyName { get; set; }
        public string Mobile { get; set; }
        public string ChequeDate { get; set; }
        public string ChequeNo { get; set; }
        public int PDCReq { get; set; }
        public string PDCDate { get; set; }
        public string ChequeImage { get; set; }
        public string Amount { get; set; }
        public string PaymentType { get; set; }
        public int PaymentReceipt { get; set; }
        public string Remarks { get; set; }
        public string ApprovedBy { get; set; }
        public List<BillByBillRef> BillByBill { get; set; }
    }
    public class PendingBillPayment
    {
        public string PartyName { get; set; }
        public string RefNo { get; set; }
        public string RefCode { get; set; }
        public string PendingAmount { get; set; }
        public string ChequeDate { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeImage { get; set; }
        public string Amount { get; set; }
        public string PaymentType { get; set; }
        public string Remarks { get; set; }
        public string RefDate { get; set; }
        public string CreatedBy { get; set; }
    }
    public class LedgerReport
    {
        public int vchcode { get; set; }
        public string date { get; set; }
        public string dtorder { get; set; }
        public string Type { get; set; }
        public string BillNo { get; set; }
        public string Account { get; set; }
        public double Value { get; set; }
        public string Credit { get; set; }
        public string debit { get; set; }
        public string Balance { get; set; }
        public string Shortnar { get; set; }
        public string DueDate { get; set; }
        public int OverDueDays { get; set; }
    }

    public class AccountWiseStatement
    {
        public string date { get; set; }
        public string Type { get; set; }
        public string VchNo { get; set; }
        public double BillAmt { get; set; }
        public double PAmtBillDr { get; set; }
        public double PAmtBillCr { get; set; }
        public double Balance { get; set; }
        public string DueDate { get; set; }
        public string Due { get; set; }
        public int PDays { get; set; }
    }
    public class SaleHeaderinv
    {
        public int Vchcode { get; set; }
        public string VchNo { get; set; }
        public string GrNo { get; set; }
        public string Station { get; set; }
        public string Transport { get; set; }
        public string PartyName { get; set; }

        public string InvDate { get; set; }
        public double Qty { get; set; }
    }
    public class ReceiptSum
    {
        
        public string VchNo { get; set; }
        public string PartyName { get; set; }
        public string InvDate { get; set; }
        public string Num { get; set; }
        public string type { get; set; }
        public double DebitAmt { get; set; }
        public double Amount { get; set; }

        public string Narration { get; set; }
    }
    public class SaleItemDet
    {
        public int SrNo { get; set; }
        public string ItemName { get; set; }
        public string IUnit { get; set; }
        public double Qty { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
     
    }
    public class OpenClsBlns
    {

        public string opening { get; set; }
        public string closing { get; set; }
        public List<LedgerReport> LedgerReport { get; set; }
    }

    public class PendingPayments
    {
        public int RefCode { get; set; }
        public string RefNo { get; set; }
        public string RefDate { get; set; }
        public int VchCode { get; set; }
        public string Type { get; set; }
        public int Days { get; set; }
        public string DueDate { get; set; }
        public double TotalAmount { get; set; }
        public double Payable { get; set; }
        public double Reciveable { get; set; }
    }
    public class Company    
    {
        public string CompName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string TelNo { get; set; }
        public string EMail { get; set; }
        public string ITPAN { get; set; }
        public string GSTNo { get; set; }
        public string TinNo { get; set; }
        public string PinCode { get; set; }
    }

    public class TargetReport
    {
        public double MTarget { get; set; }
        public double YTarget { get; set; }
        public double MAchived { get; set; }
        public double YAchived { get; set; }

        public int SCode { get; set; }
        public string SName { get; set; }
    }

    public class SalesmanTarget
    {
        public int MonthVal { get; set; }
        public double MTarget { get; set; }
        public double MAchived { get; set; }
        public int SCode { get; set; }
        public string SName { get; set; }
    }
    public class TargetMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MasterType { get; set; }
        public int MasterRangeType { get; set; }
        public int MasterApplyOn { get; set; }
    
    }
    public class TargetMasterDetail
    {
        public int Month { get; set; }
        public double Target { get; set; }
        public double Achivement { get; set; }
        public double Diffrence { get; set; }
        public double Percent { get; set; }

    }


    public class RecItem
    {
        public string ProductName { get; set; }

        public string Name { get; set; }
        public string Alias { get; set; }
        public int ProductCode { get; set; }
        public int Code { get; set; }
        public double Price { get; set; }
        public string MinQTY { get; set; }
        public string Description { get; set; }
        public int Faourite { get; set; }
        public string imgURL { get; set; }
        public double MinPrice { get; set; }
        public double Stock { get; set; }
        public double DiscPer { get; set; }
        public double Confactor { get; set; }
        public double MRP { get; set; }
        public int CFMode { get; set; }

        public string Unit { get; set; }
        public string AltUnit { get; set; }
        public double AltPrice { get; set; }

    }

    public class Stock
    {
        public string ItemName = "";
        public string Code { get; set; }
        public string UnitName = "";
        //public double OPStock = 0;
        //public double OpAmt = 0;
        //public double OpTran = 0;
        //public double QtyIn = 0;
        //public double QtyOut = 0;
        public double CloseStock = 0;
    }

    public class SalesParty
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public int Beat { get; set; }
        public string fromDate { get; set; }
        public double Points { get; set; }
    }


    public class SalesOrderHistory
    {
        public string OrderDate { get; set; }
        public string PartyName { get; set; }
        public double OrderAmount { get; set;}
    }

    public class SalesNewParty
    {
        public string CDate { get; set; }
        public string PartyName { get; set; }
        public string Mobile { get; set; }
        public string EMail { get; set; }
        public string Status { get; set; }
    }

    public class SalesmanVisit
    {
        public string VisitDate { get; set; }
        public string PartyName { get; set; }
        public string Distance { get; set; }
    }

    public class SalesmanLocation
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class SalesPoints
    {
        
        public double Points { get; set; }
    }
    public class AutoCamera
    {

        public int CameraOnOff { get; set; }
        public double CheckinDist { get; set; }
        public int OtherDet { get; set; }
        public int Scheme { get; set; }
        public int RG { get; set; }
        public int MEBA { get; set; }

    }
    public class ReValidate 
    { 
     
        public int ReVali { get; set; }
    
    }

    public class PartyBalanceDetails
    {
        public string PartyName { get; set; }
        public string Balance { get; set; }
    }

    public class CraeteVisit
    {
        public int BeatId { get; set; }
        public int SalesManId { get; set; }
        public int PartyCode { get; set; }
    }
    public class Tracking
    {         
        public int SalesManId { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int EType { get; set; }

        public string Distance { get; set; }
    }

    public class STracking
    {
        public string Date { get; set; }
        public string STime { get; set; }
        public string SAddress { get; set; }
        public string ETime { get; set; }
        public string EAddress { get; set; }
    }

    public class AccOrder
    {
        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public double OrderQty { get; set; }
        public double OrderAmount { get; set; }
        public string Status { get; set; }
    }

    public class OrderWiseStatement
    {
        public int OrderID { get; set; }
        public string OrderDate { get; set; }
        public int SalesmanID { get; set; }
        public string Salesman { get; set; }
        public int PartyID { get; set; }
        public string PartyName { get; set; }
        public string OStatus { get; set; }
        public int BQuotationVchCode { get; set; }
        public string BQuotaionDate { get; set; }
        public string BQuotationNo { get; set; }
        public int BOrderVchCode { get; set; }
        public string BOrderDate { get; set; }
        public string BOrderNo { get; set; }
        public int BInvVchCode { get; set; }
        public string BInvDate { get; set; }
        public string BInvVchNo { get; set; }
        public int FVchCode { get; set; }
    }

    public class OrderItemData
    {
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public string UnitName { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }

    public class VItemData
    {
        public int SrNo { get; set; }
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }

    public class Scheme
    {
        public int SchemeCode { get; set; }
        public string SchemeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SPartyType { get; set; }
        public string PartyType { get; set; }
        public int PartyCode { get; set; }
        public string PartyName { get; set; }
        public int SchemeAppType { get; set; }
        public string SchemeAppOn { get; set; }
        public string ApplicableOn { get; set; }
        public string SchemeType { get; set; }
        public List<SchemeAccData> SchAccData { get; set; }
        public List<SchemeData> SchData { get; set; }
    }

    public class SchemeAccData
    {
        public int SchemeCode { get; set; }
        public int PartyCode { get; set; }
        public string PartyName { get; set; }
    }

    public class SchemeData
    {
        public int SchemeCode { get; set; }
        public string SchemeName { get; set; }
        public int SrNo { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public int UCode { get; set; }
        public string UnitName { get; set; }
        public int FItemCode { get; set; }
        public string FItemName { get; set; }
        public double FromQty { get; set; }
        public double ToQty { get; set; }
        public double FreeQty { get; set; }
        public double VAmount { get; set; }
        public double VDisc { get; set; }
        public string Disc { get; set; }
        public double Price { get; set; }
        public int SBasis { get; set; }
    }

    public class OrderItem
    {
        public string ItemName { get; set; }
        public double ItemQty { get; set; }
        public double OrderPrice { get; set; }
        public string Status { get; set; }
    }

    public class WebLink
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }

    public class ExpVchConfigDT
    {
        public int SeriesCode { get; set; }
        public string SeriesName { get; set; }
        public int SaveDoc { get; set; }
        public int SaveDocInDB { get; set; }
        public string DocPath { get; set; }
    }

    public class VchInvoice
    {
        public int SeriesCode { get; set; }
        public string SeriesName { get; set; }
        public string StPtName { get; set; }
        public string AccName { get; set; }
        public string MCName { get; set; }
        public string Transport { get; set; }
        public string Station { get; set; }
        public string Salesman { get; set; }
        public string Remarks { get; set; }
        public List<InvoiceItemDT> ItemDT { get; set; }
        public List<BSDetail> BSDetail { get; set; }
    }
    public class BSDDetail
    {
        public int SrNo { get; set; }
        public string BSName { get; set; }
        public int BSCode { get; set; }
        public double BSPer { get; set; }
        public double Amount { get; set; }
        public string PercentOperatedOn { get; set; }
    }

    public class BSDDetailCart
    {

        public int SrNo { get; set; }
        public string BSName { get; set; }
        public int BSCode { get; set; }
        public double BSPer { get; set; }
        public double Amount { get; set; }
        public string PercentOperatedOn { get; set; }
    }
    public class VchReceipt
    {
        public int SeriesCode { get; set; }
        public string SeriesName { get; set; }
        public string PartyName { get; set; }
        public string CashAcc { get; set; }
        public string BankAcc { get; set; }
        public int PDC { get; set; }
        public string PDCDate { get; set; }
        public string Remarks { get; set; }
        public int Mode { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeDt { get; set; }
        public double Amount { get; set; }
        public string Image { get; set; }
        public string Exte { get; set; }
        public List<BillByBillRef> BillByBill { get; set; }
    }

    public class TiketInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EMail { get; set; }
        public string AreaCode { get; set; }
        public string Mobile { get; set; }
        public int TicketType { get; set; }
        public int Priority { get; set; }
        public string base64 { get; set; }
        public string FileExt { get; set; }
        public string AddInfo { get; set; }
    }
    public class InvoiceItemDT
    {
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public double ListPrice { get; set; }
        public string MainUnit { get; set; }
        public string AltUnit { get; set; }
        public double ConFactor { get; set; }
        public double AltQty { get; set; }

        public string DiscPerent { get; set; }
        public double Discount { get; set; }

        public string AddDiscount { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public string IRemarks { get; set; }
        public string IDescription1 { get; set; }
        public string IDescription2 { get; set; }
        public string IDescription3 { get; set; }
        public string IDescription4 { get; set; }
        public int FItemCode { get; set; }
        public string FItemName { get; set; }

        public double FreeQty { get; set; }
        public double FItemPrice { get; set; }
        public int SchemeCode { get; set; }
        public int SchemeType { get; set; }

        public int BillingUnit { get; set; }
        public List<InvoiceItemSerailDT> ItemSerailDT { get; set; }
        public List<InvoiceItemParamDT> ItemParamDT { get; set; }

    }
    public class InvoiceItemSerailDT
    {
        public string SerailNo { get; set; }
    }

    public class InvoiceItemParamDT
    {
        public string BCN { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public double Qty { get; set; }
    }


    public class Address
    {
        public string road { get; set; }
        public string suburb { get; set; }        
        public string city { get; set; }        
        public string state_district { get; set; }        
        public string state { get; set; }        
        public string postcode { get; set; }        
        public string country { get; set; }        
        public string country_code { get; set; }
    }
    public class RootObject
    {
        public string place_id { get; set; }        
        public string licence { get; set; }        
        public string osm_type { get; set; }        
        public string osm_id { get; set; }        
        public string lat { get; set; }        
        public string lon { get; set; }        
        public string display_name { get; set; }        
        public Address address { get; set; }    
    }

    public class VchStockJ
    {
        public int SeriesCode { get; set; }
        public string SeriesName { get; set; }
        public string StPtName { get; set; }
        public string MCName1 { get; set; }
        public string MCName2 { get; set; }
        public int MCCode1 { get; set; }
        public int MCCode2 { get; set; }
        public double ScrabQty { get; set; }
        public List<InvoiceItemDT> ItemDT { get; set; }
        public List<Halfitemdt> Halfitemdt { get; set; }

        public List<InvoiceItemDT> ItemDT1 { get; set; }
       
    }
    public class Halfitemdt
    {
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public double ListPrice { get; set; }
        public string MainUnit { get; set; }
        public string AltUnit { get; set; }
        public double ConFactor { get; set; }
        public double AltQty { get; set; }

        public string DiscPerent { get; set; }
        public double Discount { get; set; }

        public string AddDiscount { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public string IRemarks { get; set; }
        public string IDescription1 { get; set; }
        public string IDescription2 { get; set; }
        public string IDescription3 { get; set; }
        public string IDescription4 { get; set; }
        public int FItemCode { get; set; }
        public string FItemName { get; set; }

        public double FreeQty { get; set; }
        public double FItemPrice { get; set; }
        public int SchemeCode { get; set; }
        public int SchemeType { get; set; }

        public int BillingUnit { get; set; }
        public List<InvoiceItemSerailDT> ItemSerailDT { get; set; }
        public List<InvoiceItemParamDT> ItemParamDT { get; set; }

    }
    public class Employee
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public int Department { get; set; }
        public string DepartmentName { get; set; }
        public int MCCode { get; set; }
        public string MCName { get; set; }
    }
    public class DepartmentCount
    {
        public int Department { get; set; }
        public int Pending { get; set; }
        public int Upcoming { get; set; }
        public int Complete { get; set; }
    }
    public class BCNItemDet
    {
        public int ItemCode { get; set; }
        public string BCN { get; set; }
        public int IRCount { get; set; }
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }


    }
    public class DepartCount
    {
        public int NoofCount { get; set; }
        public int Squence { get; set; }
        public string SquenceName { get; set; }
    }
    public class DepartmentSts
    {
        public string ItemName { get; set; }
        public int ItemCode { get; set; }
        public string Unit { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Variety { get; set; }
        public int TaskID {get;set;}
        public string ProcessType { get; set; }
        public string Date { get; set; }
        public string VchNo { get; set; }
        public string OrderId { get; set; }
        public string Shape { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Qty { get; set; }
        public double Area { get; set; }
        public double Status { get; set; }
    }
    public class PSaleOrde
    {
        public int VCode { get; set; }
        public string Date { get; set; }
        public string VchNo { get; set; }
        public string PartyName { get; set; }
        public string OrderType { get; set; }
        public double Qty { get; set; }
        public double Area { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }

    }
    public class SaleOrdeWiseDet
    {
        public string ProcesType { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Variety { get; set; }
        public string Shape { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Chargeable { get; set; }
        public double Qty { get; set; }
        public double HoleQty { get; set; }
        public double CSKQty { get; set; }
        public double CutoutQty { get; set; }
        public double Area { get; set; }
        public double FinalAmt { get; set; }
        public string Proces { get; set; }
        public string Polish { get; set; }
        public string SPLWork { get; set; }
    }

}