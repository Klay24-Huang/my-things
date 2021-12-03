using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.CTBCPOS
{
    public class WebAPIOutput_InquiryByLidm
    {   
        /// <summary>
        /// 查詢結果
        /// </summary>
        public int QueryCode { get; set; }
        /// <summary>
        /// 查詢錯誤原因之代碼
        /// </summary>
        public string QueryError { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 中信
        /// </summary>
        public string XID { get; set; }

        public string PAN { get; set; }

        public int ECI { get; set; }

        public int Currency { get; set; }

        public int Amount { get; set; }

        public int Exponent{ get; set; }

        public int TransactionStatus { get; set; }

        public int TransactionErrCode { get; set; }

        public string ErrorDesc { get; set; }
        /// <summary>
        /// 交易授權碼
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 調閱序號
        /// </summary>
        public int TermSeq { get; set; }
        /// <summary>
        /// 調閱編號
        /// </summary>
        public string RetrRef { get; set; }
        /// <summary>
        /// 當前交易狀態
        /// </summary>
        public int CurrentState { get; set; }
        /// <summary>
        /// 請款批次編號
        /// </summary>
        public int BatchId { get; set; }
        /// <summary>
        /// 請款批次序號
        /// </summary>
        public int BatchSeq { get; set; }
        /// <summary>
        /// 交易折抵金額(紅利專用)
        /// </summary>
        public int OffsetAmt { get; set; }
        /// <summary>
        /// 交易原始金額(紅利專用)
        /// </summary>
        public int OriginalAmt { get; set; }
        /// <summary>
        /// 交易兌換點數(紅利專用)
        /// </summary>
        public int UtilizedPoint { get; set; }
        /// <summary>
        /// 本次獲得的紅利點數(紅利專用)
        /// </summary>
        public int AwardedPoint { get; set; }
        /// <summary>
        /// 目前點數餘額(紅利專用)
        /// </summary>
        public int PointBalance{ get; set; }
        /// <summary>
        /// 電子發票載具編號
        /// </summary>
        public string CardNo { get; set; }
        
        /// <summary>
        /// 版本修訂日期
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 訊息規格版本
        /// </summary>
        public int Revision { get; set; }


        public Dictionary<string,string> Orders { get; set; }



    }
}


/*
if (ibl.QueryCode == 2)
{
    Response.WritResponse.Write("DataLen: e("DataLen: --");");
    foreach (Dictionary<String, String> dictionary in ibl.Data)foreach (Dictionary<String, String> dictionary in ibl.Data)
    {{
        Response.Write("");Response.Write("");
        Response.Write("ID: " + dictionary["id"]);Response.Write("ID: " + dictionary["id"]);
        Response.Write("LIDResponse.Write("LID--M(M(訂單編號訂單編號): "): " + dictionary["LID+ dictionary["LID--M"]);M"]);
        Response.Write("XID(Response.Write("XID(交易識別碼交易識別碼): " + dictionary["XID"]);): " + dictionary["XID"]);
        Response.Write("PAN(Response.Write("PAN(部份信用卡卡號部份信用卡卡號): " + dictionary["PAN"]);): " + dictionary["PAN"]);
        Response.Write("ECI(ECIResponse.Write("ECI(ECI值值): " + dictionary["ECI"]);): " + dictionary["ECI"]);
        Response.Write(Response.Write("Currency("Currency(授權授權//購貨交易的回傳金額的交易幣別代碼購貨交易的回傳金額的交易幣別代碼): " + dictionary["Currency"]);): " + dictionary["Currency"]);
        Response.Write("Amount(Response.Write("Amount(授權授權//購貨交易的回傳金額的幣值指數購貨交易的回傳金額的幣值指數): " + dictionary["AuthAmt"]);): " + dictionary["AuthAmt"]);
        Response.Write("Exponent(Response.Write("Exponent(授權授權//購貨交易的回傳金額的授權金額購貨交易的回傳金額的授權金額): " + dictionary["Exponent"]);): " + dictionary["Exponent"]);
        Response.Write("RResponse.Write("RespCode(espCode(交易的執行狀態交易的執行狀態): " + dictionary["RespCode"]);): " + dictionary["RespCode"]);
        Response.Write("ErrCode(Response.Write("ErrCode(交易的錯誤代碼交易的錯誤代碼): " + dictionary["ErrCode"]);): " + dictionary["ErrCode"]);
        Response.Write("ERRDESC(Response.Write("ERRDESC(錯誤訊息錯誤訊息): " + dictionary["ERRDESC"]);): " + dictionary["ERRDESC"]);
        Response.Write("AuthCode(Response.Write("AuthCode(授權交易授權碼授權交易授權碼): " ): " + dictionary["AuthCode"]);+ dictionary["AuthCode"]);
        Response.Write("TermSeq(Response.Write("TermSeq(交易之調閱序號交易之調閱序號): " + dictionary["TermSeq"]);): " + dictionary["TermSeq"]);
        Response.Write("RetrRef(Response.Write("RetrRef(調閱編號調閱編號): " + dictionary["RetrRef"]);): " + dictionary["RetrRef"]);
        Response.Write("CurrentState(Response.Write("CurrentState(訂單交易狀態訂單交易狀態): " + dictionary["Cur): " + dictionary["CurrentState"]);rentState"]);
        Response.Write("BatchID(Response.Write("BatchID(購貨交易之批次編號購貨交易之批次編號): " + dictionary["BatchID"]);): " + dictionary["BatchID"]);
        Response.Write("BatchSeq(Response.Write("BatchSeq(購貨交易之批次序號購貨交易之批次序號): " + dictionary["BatchSeq"]);): " + dictionary["BatchSeq"]);
        Response.Write("OffsetAmt(Response.Write("OffsetAmt(購貨交易之折抵金額購貨交易之折抵金額): " ): " + dictionary["OffsetAmt"]);+ dictionary["OffsetAmt"]);
        Response.Write("OriginalAmt(Response.Write("OriginalAmt(購貨交易之原始消費金額購貨交易之原始消費金額): " + dictionary["OriginalAmt"]);): " + dictionary["OriginalAmt"]);
        Response.Write("UtilizedPoint(Response.Write("UtilizedPoint(購貨交易之本次兌換點數購貨交易之本次兌換點數): " + dictionary["UtilizedPoint"]);): " + dictionary["UtilizedPoint"]);
        Response.Write("AwardedPoint(Response.Write("AwardedPoint(購貨交易之本次賺取點數購貨交易之本次賺取點數): " + dictionary["AwardedPoint"]);): " + dictionary["AwardedPoint"]);
        Response.Write("PointBalance(Response.Write("PointBalance(購貨交易之目前點數餘額購貨交易之目前點數餘額): " + dictionary["PointBalance"]);): " + dictionary["PointBalance"]);
    }}
    Response.Write("Response.Write("---------------------- 交易完成交易完成 --------------------------------------");");
 
 */