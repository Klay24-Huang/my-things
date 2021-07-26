using Domain.SP.Input.Subscription;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.BillFunc
{
    public class BuyNowNxtCommon
    {
        public string IDNO { get; set; } = "";
        public Int64 LogID { get; set; } = 0;
        /// <summary>
        /// 付款方式
        /// </summary>
        public Int64 PayTypeId { get; set; } = 0;
        /// <summary>
        /// 發票設定
        /// </summary>
        public Int64 InvoTypeId { get; set; } = 0;

        public int ApiID  = 0;
        public string ApiJson = "";
        public string errCode = "000000";
        public string errMsg = "Success";
        private MonSubsSp msp = new MonSubsSp();

        //20210714 ADD BY ADAM REASON.取流水號
        public int MonthlyRentId = 0;
        public int NowPeriod = 1;//預設第一期
        public DateTime OriSDATE;   //原本的起日
        public string MonthlyRentIds = "";
        public BuyNowNxtCommon()
        {
        }

        public bool CkApiID()
        {
            if (SiteUV.BuyNow_PayNxt.Any(x => x == ApiID))
                return true;
            else
            {
                errCode = "ERR265";//ApiID錯誤
                return false;
            }                
        }

        /// <summary>
        /// 執行付款完成後動作
        /// </summary>
        /// <returns></returns>
        public bool exeNxt(string MerchantTradeNo,string TransactionNo)
        {
            bool flag = false;
            var carRepo = new CarRentRepo();
            var trace = new TraceCom();
            trace.traceAdd("apiIfo", new { ApiID, ApiJson });
            string funNm = "exeNxt";
            int funId = SiteUV.GetFunId(funNm);

            if (!CkApiID()) return false;

            try
            {
                //月租專案群組:建立月租
                if (ApiID == 179)
                {
                    if (!string.IsNullOrEmpty(ApiJson) && !string.IsNullOrWhiteSpace(ApiJson))
                    {
                        var spIn = JsonConvert.DeserializeObject<SPInput_CreateSubsMonth>(ApiJson);
                        if(spIn != null)
                        {
                            spIn.IDNO = IDNO;
                            spIn.LogID = LogID;
                        }
                        if (!string.IsNullOrWhiteSpace(spIn.IDNO) && !string.IsNullOrWhiteSpace(spIn.MonProjID) &&
                            spIn.LogID > 0 && spIn.MonProPeriod > 0)
                        {
                            spIn.PayTypeId = PayTypeId;
                            spIn.InvoTypeId = InvoTypeId;
                            spIn.SetPayOne = 1;//首期金額已付
                            //20210709 ADD BY ADAM REASON.補上台新訂單編號
                            spIn.MerchantTradeNo = MerchantTradeNo;
                            spIn.TaishinTradeNo = TransactionNo;
                            
                            flag = msp.sp_CreateSubsMonth(spIn, ref errCode, ref MonthlyRentId);
                            trace.traceAdd("CreateSubsMonth", new { flag, errCode });
                        }
                        else
                        {
                            errCode = "ERR257";//參數遺漏
                            flag = false;
                        }
                    }
                    else
                    {
                        errCode = "ERR267";//ApiJson錯誤
                        flag = false;
                    }
                }
                //取得訂閱制升轉列表:升轉
                else if (ApiID == 188)
                {
                    if (!string.IsNullOrEmpty(ApiJson) && !string.IsNullOrWhiteSpace(ApiJson))
                    {
                        var spIn = JsonConvert.DeserializeObject<SPInput_UpSubsMonth>(ApiJson);
                        if (spIn != null)
                        {
                            spIn.IDNO = IDNO;
                            spIn.LogID = LogID;
                            spIn.MerchantTradeNo = MerchantTradeNo;
                            spIn.TaishinTradeNo = TransactionNo;
                        }
                        if (!string.IsNullOrWhiteSpace(spIn.IDNO) && spIn.LogID > 0 &&
                            !string.IsNullOrWhiteSpace(spIn.MonProjID) && spIn.MonProPeriod > 0 &&
                            !string.IsNullOrWhiteSpace(spIn.UP_MonProjID) && spIn.UP_MonProPeriod > 0 )
                        {
                            spIn.PayTypeId = PayTypeId;
                            spIn.InvoTypeId = InvoTypeId;
                            flag = msp.sp_UpSubsMonth(spIn, ref errCode, ref MonthlyRentId, ref NowPeriod, ref OriSDATE);
                            trace.traceAdd("UpSubsMonth", new { flag,errCode });
                        }
                        else
                        {
                            errCode = "ERR257";//參數遺漏
                            flag = false;
                        }
                    }
                }
                //訂閱牌卡制欠費查詢:繳費
                else if (ApiID == 190)
                {
                    if(!string.IsNullOrEmpty(ApiJson) && !string.IsNullOrWhiteSpace(ApiJson))
                    {
                        var spIn = JsonConvert.DeserializeObject<SPInput_ArrearsPaySubs>(ApiJson);
                        if (spIn != null)
                        {
                            spIn.IDNO = IDNO;
                            spIn.LogID = LogID;
                            spIn.MerchantTradeNo = MerchantTradeNo;
                            spIn.TaishinTradeNo = TransactionNo;
                            spIn.MonthlyRentIds = MonthlyRentIds;
                        }
                        if (string.IsNullOrWhiteSpace(spIn.IDNO) || spIn.LogID == 0)
                        {
                            errCode = "ERR257";//參數遺漏
                            flag = false;
                        }
                        else
                        {
                            spIn.PayTypeId = PayTypeId;
                            spIn.InvoTypeId = InvoTypeId;
                            
                            flag = msp.sp_ArrearsPaySubs(spIn, ref errCode);
                            trace.traceAdd("ArrearsPaySubs", new { flag, errCode }); 
                        }
                    }
                    else
                    {
                        errCode = "ERR267";//ApiJson錯誤
                        flag = false;
                    }
                }
                else
                   errMsg = "無對應ApiID";

                trace.traceAdd("errCode", errCode);
                trace.traceAdd("errMsg", errMsg);
                carRepo.AddTraceLog(funId, funNm, trace, flag);
            }
            catch (Exception ex)
            {
                flag = false;
                errMsg = ex.Message;
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(funId, funNm, trace, flag);
                throw ex;
            }           

            return flag;
        }
        
    }
}