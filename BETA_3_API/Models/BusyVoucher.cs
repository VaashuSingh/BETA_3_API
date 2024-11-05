using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESCommon;
using System.Data;
using System.Configuration;

namespace BETA_3_API_BUSY_VCH.Models
{
    public class BusyVoucher
    {
    
        public class AccMaster
        {
            public Accounts Accounts { get; set; }
        }

        public class Account
        {
            public string Name { get; set; }
            public string PrintName { get; set; }
            public string ParentGroup { get; set; }
            public bool BillByBillBalancing { get; set; }
            public int tmpParentGrpCode { get; set; }
            public string ChequePrintName { get; set; }
            public string BrokerName { get; set; }
            public bool BrokerAssigned { get; set; }
            public Address Address { get; set; }

            public int SupplierType { get; set; }
            public string PriceLevel { get; set; }
            public string PriceLevelForPurc { get; set; }
            public string TaxType { get; set; }
            public string ReverseChargeType { get; set; }
            public string TypeOfDealerGST { get; set; }
            public string InputType { get; set; }
        }

        public class Accounts
        {
            public Account Account { get; set; }
        }

        public class Address
        {
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public string Address4 { get; set; }
            public string Email { get; set; }
        
            public string WhatsAppNo { get; set; }
            public string ITPAN { get; set; }
            public string TINNo { get; set; }
            public string GSTNo { get; set; }
            public string OF { get; set; }
            public string CountryName { get; set; }
            public string StateName { get; set; }
            public string RegionName { get; set; }
            public string AreaName { get; set; }
            public int PINCode { get; set; }
            public int TmpMasterCode { get; set; }
            public string C3 { get; set; }
            public string Mobile { get; set; }
            public string Contact { get; set; }
        }



        //public class Accounts
        //{
        //    public List<Account> AccDet { get; set; }

        //}
        //public class Account
        //{
        //    public string Name { get; set; }
        //    public string PrintName { get; set; }
        //    public string ParentGroup { get; set; }
        //    public bool BillByBillBalancing { get; set; }
        //    public List<Address> AccAddress { get; set; }
        //}
        //public class Address
        //{
        //    public string Address1 { get; set; }
        //    public string Address2 { get; set; }
        //    public string Address3 { get; set; }
        //    public string Address4 { get; set; }
        //    public string Email { get; set; }
        //    public string GSTNo { get; set; }
        //    public string CountryName { get; set; }

        //    public string StateName { get; set; }
        //    public string PinCode { get; set; }
        //    public string ITPAN { get; set; }
        //    public string Mobile { get; set; }

        //}
        public class BillingDetails
        {
            public string PartyName { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public string Address4 { get; set; }
            public string MobileNo { get; set; }
            public string ITPAN { get; set; }
            public int TypeOfDealer { get; set; }
            public string TmpVchCode { get; set; }
        }

        public class VchOtherInfoDetails
        {
            //public string OFInfo { get; set; }

            public string Narration1 { get; set; }
            public string Transport { get; set; }
            public string GRNo { get; set; }
            public string VehicleNo { get; set; }
            public string Station { get; set; }
            public string GrDate { get; set; }
            public string PurchaseBillNo { get; set; }
            public string PurchaseBillDate { get; set; }
        }

        public class ItemDescInfo
        {
            //public string OFInfo { get; set; }
            public string Description1 { get; set; }
            public string Description2 { get; set; }
            public string Description3 { get; set; }
            public string Description4 { get; set; }
            public int tmpSrNo { get; set; }
        }

        public class InvVoucher
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public string VchNo { get; set; }
            public string PartyName { get; set; }
            public string Narration { get; set; }
            public double TQty { get; set; }
            public double TAmount { get; set; }
            public double NetAmount { get; set; }
            public List<InvItemDetail> ItemEntries { get; set; }
            public List<InvBSDetail> BSEntries { get; set; }
            //public List<BSDDetail> BSDDetail { get; set; }
        }
        public class BSDDetail
        {

            public int SrNo { get; set; }
            public string BSName { get; set; }
            public int tmpBSCode { get; set; }
            public double BSPer { get; set; }
            public double Amount { get; set; }
            public string PercentOperatedOn { get; set; }
        }
        public class InvItemDetail
        {
            public int SrNo { get; set; }
            public string ItemName { get; set; }
            public string UnitName { get; set; }
            public double QtyMainUnit { get; set; }
            public double QtyAltUnit { get; set; }
            public double Price { get; set; }
            public double Amount { get; set; }
        }

        public class InvBSDetail
        {
            public int SrNo { get; set; }
            public string BSName { get; set; }
            public int tmpBSCode { get; set; }
            public double BSPer { get; set; }
            public double Amount { get; set; }
            public string PercentOperatedOn { get; set; }
        }

        public class ItemDetail
        {
            public string Date { get; set; }
            public int VchType { get; set; }
            public string VchNo { get; set; }
            public int SrNo { get; set; }
            public string ItemName { get; set; }
            public string SchemeName { get; set; }
            public int SchemeParentRowNo { get; set; }
            public int SchemeType { get; set; }
            public string UnitName { get; set; }
            public string AltUnitName { get; set; }
            public double ConFactor { get; set; }
            public double Qty { get; set; }
            public double QtyMainUnit { get; set; }
            public double QtyAltUnit { get; set; }
            public ItemDescInfo ItemDescInfo { get; set; }
            public double ListPrice { get; set; }
            public double Amt { get; set; }
            public double DiscountPercent { get; set; }
            public string CompoundDiscount { get; set; }
            public double Discount { get; set; }
            public double Price { get; set; }
            public double PriceAltUnit { get; set; }
            public double STPercent { get; set; }
            public double STPercent1 { get; set; }
            public double STAmount { get; set; }
            public double NettAmount { get; set; }
            public int TmpVchCode { get; set; }
            public string MC { get; set; }
            public string AF { get; set; }

            public  int BillingUnit { get; set; }
            public List<ItemSerialNoDetail> ItemSerialNoEntries { get; set; }
            public List<ParamStockDetails> ParamStockEntries { get; set; }
        }

        public class ItemSerialNoDetail
        {
            public int ItemCode { get; set; }
            public int MCCode { get; set; }
            public string SerialNo { get; set; }
            public int VchItemSN { get; set; }
            public int GridSN { get; set; }
            public double MainQty { get; set; }
            public double AltQty { get; set; }
            public double MainUnitPrice { get; set; }
            public double AltUnitPrice { get; set; }
        }
        public class BSDetail
        {
            public int SrNo { get; set; }
            public string BSName { get; set; }
            public int tmpBSCode { get; set; }
            public double PercentVal { get; set; }
            public double Amt { get; set; }
            public string PercentOperatedOn { get; set; }
            //public string SrNo { get; set; }
            //public string BSName { get; set; }
            //public string PercentVal { get; set; }
            //public string PercentOperatedOn { get; set; }
            //public string Amt { get; set; }
            //public string Date { get; set; }
            //public string VchNo { get; set; }
            //public string VchType { get; set; }
            //public string TmpVchCode { get; set; }
        }

    
        public class Sale
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public string StockUpdationDate { get; set; }
            public string VchNo { get; set; }
            public string STPTName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public bool BrokerInvolved { get; set; }
            public string BrokerName { get; set; }
            public BillingDetails BillingDetails { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<ItemDetail> ItemEntries { get; set; }
            public List<BSDetail> BillSundries { get; set; }
            public int TmpVchCode { get; set; }
            public int TmpVchSeriesCode { get; set; }
        }

        public class SaleReturn
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public string StockUpdationDate { get; set; }
            public string VchNo { get; set; }
            public string STPTName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public bool BrokerInvolved { get; set; }
            public string BrokerName { get; set; }
            public BillingDetails BillingDetails { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<ItemDetail> ItemEntries { get; set; }
            public List<BSDetail> BillSundries { get; set; }
            public int TmpVchCode { get; set; }
            public int TmpVchSeriesCode { get; set; }
        }
        public class StockTransfer
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public string StockUpdationDate { get; set; }
            public string VchNo { get; set; }
            public string STPTName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public bool BrokerInvolved { get; set; }
            public bool Cancelled { get; set; }
            public int InputType { get; set; }
            public string BrokerName { get; set; }
            public BillingDetails BillingDetails { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<ItemDetail> ItemEntries { get; set; }
            public List<BSDetail> BillSundries { get; set; }
            public int TmpVchCode { get; set; }
            public int TmpVchSeriesCode { get; set; }
        }

        public class SaleQuotation
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public string StockUpdationDate { get; set; }
            public string VchNo { get; set; }
            public string STPTName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public bool BrokerInvolved { get; set; }
            public string BrokerName { get; set; }
            public BillingDetails BillingDetails { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<ItemDetail> ItemEntries { get; set; }
            public List<BSDetail> BillSundries { get; set; }
            public int TmpVchCode { get; set; }
            public int TmpVchSeriesCode { get; set; }
        }

        public class SaleOrder
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public string StockUpdationDate { get; set; }
            public string VchNo { get; set; }
            public string STPTName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public bool BrokerInvolved { get; set; }
            public string BrokerName { get; set; }
            public BillingDetails BillingDetails { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<ItemDetail> ItemEntries { get; set; }
            public List<BSDetail> BillSundries { get; set; }
            public int TmpVchCode { get; set; }
            public int TmpVchSeriesCode { get; set; }
        }

        public class Journal
        {
            public string VchNo { get; set; }
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public string PDCDate { get; set; }
            public string StockUpdationDate { get; set; }
            public int VchType { get; set; }
            public string TranCurName { get; set; }
            public int TranType { get; set; }
            public int tmpVchCode { get; set; }
            public int tmpVchSeriesCode { get; set; }
            public double tmpTotalAmt { get; set; }
            public int tmpOldVchSeriesCode { get; set; }
            public string tmpTranCurString { get; set; }
            public string tmpTranCurSubString { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<AccDetail> AccEntries { get; set; }
            public List<BillDetail> PendingBillDetails { get; set; }
        }

        public class Receipt
        {
            public string VchNo { get; set; }
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public string PDCDate { get; set; }
            public string StockUpdationDate { get; set; }
            public int VchType { get; set; }
            public string TranCurName { get; set; }
            public int TranType { get; set; }
            public int tmpVchCode { get; set; }
            public int tmpVchSeriesCode { get; set; }
            public double tmpTotalAmt { get; set; }
            public int tmpOldVchSeriesCode { get; set; }
            public string tmpTranCurString { get; set; }
            public string tmpTranCurSubString { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<AccDetail> AccEntries { get; set; }
            public List<BillDetail> PendingBillDetails { get; set; }
        }
        
        public class BillDetail
        {
            public string MasterName1 { get; set; }
            public int tmpMasterCode1 { get; set; }
            public List<BillRefs> BillRefDT { get; set; }
        }

        public class AccDetail
        {
            public string Date { get; set; }
            public int VchType { get; set; }
            public int SrNo { get; set; }
            public string AccountName { get; set; }
            public string ShortNar { get; set; }
            public int AmountType { get; set; }
            public double AmtMainCur { get; set; }
            public double CashFlow { get; set; }
            public int tmpVchCode { get; set; }
            public string tmpGroupName { get; set; }
            public int tmpAccCode { get; set; }
            //public List<BillDetails> BillRefs { get; set; }
        }

        public class BillDetails
        {
            public int Method { get; set; }
            public int SrNo { get; set; }
            public string RefNo { get; set; }
            public string Date { get; set; }
            public string DueDate { get; set; }
            public double Value1 { get; set; }
            public int ItemSrNo { get; set; }
            public int tmpRefCode { get; set; }
            public int tmpMasterCode1 { get; set; }
        }

        public class BillRefs
        {
            public int Method { get; set; }
            public int SrNo { get; set; }
            public string RefNo { get; set; }
            public string Date { get; set; }
            public string DueDate { get; set; }
            public double Value1 { get; set; }
            public int ItemSrNo { get; set; }
            public int tmpRefCode { get; set; }
            public int tmpRecType { get; set; }
            public int tmpMasterCode1 { get; set; }
        }

        public class STPTData
        {
            public int GSTType { get; set; }
            public bool MultiTax { get; set; }
            public bool TaxType { get; set; }
        }

        public class TaxCData
        {
            public double CGSTPer { get; set; }
            public double SGSTPer { get; set; }
            public double IGSTPer { get; set; }
        }

        public class BillByBill
        {
            public bool BillByBillApplicable { get; set; }
            public int CreditDays { get; set; }
        }

        public class ReferenceDT
        {
            public int RefCode { get; set; }
            public string RefNo { get; set; }
            public DateTime VchDT { get; set; }
            public double Amount { get; set; }
        }

        public class POrder
        {

            public string OrderNo { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public int PartyCode { get; set; }
            public int SaleType { get; set; }
            public string OrderType { get; set; }
            public int SOType { get; set; }
            public int PType { get; set; }
            public int Brand { get; set; }

            public float Qty { get; set; }
            public float Area { get; set; }
            public float Amount { get; set; }
            public float NetAmount { get; set; }
            public string PartyName { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public string Address4 { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public string GST { get; set; }

            public string PAN { get; set; }
            public string PinCode { get; set; }
            public int State { get; set; }
    
            public List<PItem> PItem { get; set; }


            public List<BSPDetails> BSPDetails { get; set; }

        }
        public class BSPDetails
        {
            public int SrNo { get; set; }
            public int BSCode { get; set; }
            public double BsValue { get; set; }
            public string Narration { get; set; }
            public string BSPercent { get; set; }
            public double OperatedOn { get; set; }
            public double Bsamount { get; set; }

        }
        public class PItem
        {

            public int Vchcode { get; set; }
            public int SrNo { get; set; }
            public string OrderId { get; set; }
            public int Rectype { get; set; }
            public int Method { get; set; }
            public string Vchno { get; set; }
            public string Date { get; set; }
            public string ProcessType { get; set; }
            public int Size { get; set; }
            public int Type { get; set; }
            public int Color { get; set; }
            public int Variety { get; set; }
            public int Shape { get; set; }
            public int RefCode { get; set; }
            public string Path { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public int Chargeable { get; set; }
            public float Qty { get; set; }
            public float Area { get; set; }
            public float Price { get; set; }
            public int Item { get; set; }
            public int OrganGas { get; set; }
            public int Proces { get; set; }
            public float ProcesPrice { get; set; }
            public float ProcesAmt { get; set; }
            public int Polish { get; set; }
            public float PolishPrice { get; set; }
            public float PolishAmt { get; set; }
            public int SplWork { get; set; }
            public float SplWorkPrice { get; set; }
            public float SplWorkAmt { get; set; }
            public int Block { get; set; }
            public int CSKQty { get; set; }
            public float CSKPrice { get; set; }
            public float CSKAmt { get; set; }
            public float WithoutDiscAmt { get; set; }
            public float Disc { get; set; }
            public float DistAmt { get; set; }
            public float FinalAmount { get; set; }
            public string Remark { get; set; }
            public int OrderType { get; set; }
            public float CutoutQty { get; set; }
            public float CutoutAmt { get; set; }

            public float HoleQty { get; set; }
            public float HoleAmt { get; set; }
            public float ItemPrice { get; set; }

            public float ItemPrice1 { get; set; }
            public float SpacerPrice { get; set; }
            public float DiscAmount { get; set; }
            public List<CutOut> CutOut {get; set;}
            public List<Hole> Hole { get; set; }
        }
        public class CutOut
        {
            public int Vchcode { get; set; }
            public int Srno { get; set; }
            public int CItemCode { get; set; }
            public int Qty { get; set; }
            public int Price { get; set; }
            public int Amt { get; set; }
        }
        public class Hole
        {
            public int Vchcode { get; set; }
            public int Srno { get; set; }
            public int CItemCode { get; set; }
            public int Qty { get; set; }
            public int Price { get; set; }
            public int Amt { get; set; }
        }
        public class StockJournal
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public string VchType { get; set; }
            public string VchNo { get; set; }
            public string StockUpdationDate { get; set; }
            public string ConMCName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public string OriginalID { get; set; }
          
            public string tmpVchCode { get; set; }
            public string tmpVchSeriesCode { get; set; }
            public string tmpMasterCode1 { get; set; }
            public string tmpMasterCode2 { get; set; }
            public string tmpTotalQty { get; set; }
            public string tmpTotalAltQty { get; set; }
            public string tmpTotalAmt { get; set; }
            public string tmpCostOfGoods { get; set; }
            public string tmpTotalQtyRM { get; set; }
            public string tmpTotalAltQtyRM { get; set; }
            public string tmpTotalAmtRM { get; set; }
            public string OldIdentity { get; set; }
            public string tmpBillDateStr { get; set; }


            public List<ItemDetail> ItemEntries { get; set; }
          
            public List<ItemDetail> ItemEntries1 { get; set; }

        }

        public class CRParty
        {
            public string UserId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string MobileNo { get; set; }
            public string GSTNO { get; set; }
            public string CityCode { get; set; }
            public string AreaCode { get; set; }
            public string PinCode { get; set; }
            public string StateCode { get; set; }
            public string StateName { get; set; }
            public string CountryName { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public string OwnerName { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string ConfmReg { get; set; }
            public int AgrpCode { get; set; }
            public string PAN { get; set; }
            public int CRDays { get; set; }
            public double CreditLimit { get; set; }
            public string Base64Str { get; set; }
            public int ImgType { get; set; }

            public string Exte { get; set; }
            public string AdharNo { get; set; }
            public string Pass { get; set; }
            public string Deviceid { get; set; }
            public int Itype { get; set; }
        }

        //public class ItemDetail
        //{
        //    public string Date { get; set; }
        //    public string VchType { get; set; }
        //    public string SrNo { get; set; }
        //    public string ItemName { get; set; }
        //    public string UnitName { get; set; }
        //    public string AltUnitName { get; set; }
        //    public string ConFactor { get; set; }
        //    public string Qty { get; set; }
        //    public string QtyMainUnit { get; set; }
        //    public string QtyAltUnit { get; set; }
        //    public string ItemTaxCategory { get; set; }
        //    public string ItemDescInfo { get; set; }
        //    public string Price { get; set; }
        //    public string PriceAltUnit { get; set; }
        //    public string ListPrice { get; set; }
        //    public string Amt { get; set; }
        //    public string NettAmount { get; set; }
        //    public string tmpNettPrice { get; set; }
        //    public string tmpVchCode { get; set; }
        //    public string tmpMainUCFactor { get; set; }
        //    public string MC { get; set; }
        //    public string tmpGroupName { get; set; }
        //    public string tmpMainUnitName { get; set; }
        //    public string tmpAltUnitName { get; set; }
        //    public string tmpConFactorType { get; set; }
        //    public string tmpItemMastConFactor { get; set; }
        //    public string tmpPackingUnitName { get; set; }
        //    public string tmpItemMastPUConFactor { get; set; }
        //    public string ItemSerialNoEntries { get; set; }
        //    public List<ParamStockDetails> ParamStockEntries { get; set; }
        //    public string BatchEntries { get; set; }
        //    public string tmpNettPriceAfterDisc { get; set; }
        //    public string tmpItemCode { get; set; }
        //    public string tmpMCCode { get; set; }
        //    public string tmpVchSeriesCode { get; set; }
        //    public string tmpPartyCode { get; set; }
        //    public string tmpMainUnitCode { get; set; }
        //    public string tmpUnitCode { get; set; }
        //    public string RecType { get; set; }
        //}

        public class ParamStockDetails
        {
            public string Date { get; set; }
            public string VchType { get; set; }
            public string VchCode { get; set; }
            public string ItemCode { get; set; }
            public string MCCode { get; set; }
            public string VchItemGridSN { get; set; }
            public string SrNo { get; set; }
            public string Param1 { get; set; }
            public string Param2 { get; set; }
            public string MainQty { get; set; }
            public string AltQty { get; set; }
            public string MainUnitPrice { get; set; }
            public string AltUnitPrice { get; set; }
            public string Amount { get; set; }
            public string BCN { get; set; }
            public string Party { get; set; }
            public string Discount { get; set; }
            public string Markup { get; set; }
            public string NettAltPrice { get; set; }
            public string NettAmount { get; set; }
            public string NettPrice { get; set; }
            public string ConFactor { get; set; }
            public string BCNMode { get; set; }
        }

        public string GetMasterCodeToName(string ConStr, int Code)
        {
            try
            {
                DataTable dt = new DataTable(); string Name = string.Empty;

                string str = "Select Name From Master1 Where Code = " + Code + "";
                dt = new SQLHELPER(ConStr).getTable(str);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Name = clsMain.MyString(dt.Rows[0]["Name"]);
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

        public STPTData GetSTPTData(int Provider, string ConStr, String Name, int MasterType)
        {
            STPTData Stptdata = new STPTData();
            try
            {
                DataTable dt = new DataTable();

                string str = "Select I1 as GSTType,B3 as MultiTax,B13 as TaxType from Master1 Where Name = '" + Name.Replace("'", "''") + "' And MasterType = " + MasterType + "";
                if (Provider == 1)
                {
                    dt = new OLEDBHELPER(ConStr).getTable(str);
                }
                else
                {
                    dt = new SQLHELPER(ConStr).getTable(str);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    Stptdata.GSTType = clsMain.MyInt(dt.Rows[0]["GSTType"]);
                    Stptdata.MultiTax = clsMain.MyBool(dt.Rows[0]["MultiTax"]);
                    Stptdata.TaxType = clsMain.MyBool(dt.Rows[0]["TaxType"]);
                }
            }
            catch
            {
                return Stptdata;
            }
            return Stptdata;
        }

        public TaxCData GetTaxCategoryData(int Provider, string ConStr, int ItemCode)
        {
            TaxCData taxcdata = new TaxCData();
            try
            {

                DataTable dt = new DataTable();

                string str = "";
                if (Provider == 1)
                {
                    str = "Select B.D1 as CGSTPer,B.D8 as SGSTPer,B.D2 as IGSTPer,B.I3 fROM Master1 A Inner Join MasterSupport B On A.CM8 = B.MasterCode Where A.Code = " + ItemCode + " And B.CM1 = 0 And B.Date <= Date() Order By B.Date Desc";
                    dt = new OLEDBHELPER(ConStr).getTable(str);
                }
                else
                {
                    str = "Select B.D1 as CGSTPer,B.D8 as SGSTPer,B.D2 as IGSTPer,B.I3 fROM Master1 A Inner Join MasterSupport B On A.CM8 = B.MasterCode Where A.Code = " + ItemCode + " And B.CM1 = 0 And B.Date <= getDate() Order By B.Date Desc";
                    dt = new SQLHELPER(ConStr).getTable(str);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (clsMain.MyInt(dt.Rows[0]["I3"]) == 1)
                    {
                        taxcdata.CGSTPer = clsMain.MyDouble(dt.Rows[0]["CGSTPer"]);
                        taxcdata.SGSTPer = clsMain.MyDouble(dt.Rows[0]["SGSTPer"]);
                        taxcdata.IGSTPer = clsMain.MyDouble(dt.Rows[0]["IGSTPer"]);
                    }
                    else
                    {
                        taxcdata.CGSTPer = clsMain.MyDouble(dt.Rows[0]["CGSTPer"]);
                        taxcdata.SGSTPer = 0;
                        taxcdata.IGSTPer = clsMain.MyDouble(dt.Rows[0]["CGSTPer"]);
                    }
                }
            }
            catch
            {
                return taxcdata;
            }
            return taxcdata;
        }

        public string GetItemMainUnitName(int Provider, string ConStr, int ItemCode,int Itype)
        {
            string Name = "";
            string str = "";
            try
            {
                DataTable dt = new DataTable();
                if (Itype == 1)
                {
                     str = "Select B.Name From Master1 A Inner Join Master1 B On A.CM1 = B.Code Where A.Code = " + ItemCode + "";
                }
                else
                {
                     str = "Select B.Name From Master1 A Inner Join Master1 B On A.CM2 = B.Code Where A.Code = " + ItemCode + "";
                }
                if (Provider == 1)
                {
                    dt = new OLEDBHELPER(ConStr).getTable(str);
                }
                else
                {
                    dt = new SQLHELPER(ConStr).getTable(str);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    Name = clsMain.MyString(dt.Rows[0]["Name"]);
                }
            }
            catch
            {
                return "";
            }
            return Name;
        }

        public string GetAccDefaultSTPTName(int Provider, string ConStr, string AccName)
        {
            string Name = "";
            try
            {
                DataTable dt = new DataTable();
                string str = "";

                int AccCode = GetMasterNameToCode(Provider, ConStr, AccName, 2);

                if (Provider == 1)
                {
                    str = "Select IIF(IsNull(B.Name),'',B.Name) as StPtName from Master1 A Left Join Master1 B On A.CM6 = B.Code Where A.Code = " + AccCode + "";
                    dt = new OLEDBHELPER(ConStr).getTable(str);
                }
                else
                {
                    str = "Select IsNull(B.Name,'') as StPtName from Master1 A Left Join Master1 B On A.CM6 = B.Code Where A.Code = " + AccCode + "";
                    dt = new SQLHELPER(ConStr).getTable(str);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    Name = clsMain.MyString(dt.Rows[0]["StPtName"]);
                }
            }
            catch
            {
                return "";
            }
            return Name;
        }

        public int GetMasterNameToCode(int Provider, string ConStr, string Name, int MasterType)
        {
            int Code = 0;
            try
            {
                DataTable dt = new DataTable();
                string str = "Select Code From Master1 Where Name = '" + Name.Replace("'", "''") + "' And MasterType = " + MasterType + "";
                if (Provider == 1)
                {
                    dt = new OLEDBHELPER(ConStr).getTable(str);
                }
                else
                {
                    dt = new SQLHELPER(ConStr).getTable(str);
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    Code = clsMain.MyInt(dt.Rows[0]["Code"]);
                }
            }
            catch
            {
                return 0;
            }
            return Code;
        }

        public ReferenceDT GetVchNewRefDet(int Provider, string ConStr, int VchCode, int AccCode)
        {
            ReferenceDT RefDT = new ReferenceDT();
            try
            {
                DataTable dt = new DataTable();
                string str = "";

                if (Provider == 1)
                {
                    str = "Select Top 1 LTrim(VchNo) as RefNo,Date,Value1,(Select IIF(IsNull(Max(RefCode)),0,Max(RefCode)) + 1 as R From Tran3) as NRefCode From Tran2 A Where A.VchCode = " + VchCode + " And A.RecType = 1 And A.MasterCode1 = " + AccCode + "";
                    dt = new OLEDBHELPER(ConStr).getTable(str);
                }
                else
                {
                    str = "Select Top 1 LTrim(VchNo) as RefNo,Date,Value1,IsNull((Select IsNull(Max(RefCode),0) + 1 as R From Tran3),1) as NRefCode From Tran2 A Where A.VchCode = " + VchCode + " And A.RecType = 1 And A.MasterCode1 = " + AccCode + "";
                    dt = new SQLHELPER(ConStr).getTable(str);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    RefDT.RefCode = clsMain.MyInt(dt.Rows[0]["NRefCode"]);
                    RefDT.RefNo = clsMain.MyString(dt.Rows[0]["RefNo"]);
                    RefDT.VchDT = clsMain.MyDate(dt.Rows[0]["Date"]);
                    RefDT.Amount = clsMain.MyDouble(dt.Rows[0]["Value1"]);
                }
            }
            catch
            {
                return RefDT;
            }
            return RefDT;
        }



        public BillByBill IfBillByBillApplicableInAccMast(int Provider, string ConStr, int Code)
        {
            BillByBill billBybill = new BillByBill();
            try
            {
                DataTable dt = new DataTable();

                string str = "Select B2, I2 From Master1 Where Mastertype = 2 And Code = " + Code + "";
                if (Provider == 1)
                {
                    dt = new OLEDBHELPER(ConStr).getTable(str);
                }
                else
                {
                    dt = new SQLHELPER(ConStr).getTable(str);
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    billBybill.BillByBillApplicable = clsMain.MyBool(dt.Rows[0]["B2"]);
                    billBybill.CreditDays = clsMain.MyInt(dt.Rows[0]["I2"]);
                }
            }
            catch
            {
                return billBybill;
            }
            return billBybill;
        }
    }
}