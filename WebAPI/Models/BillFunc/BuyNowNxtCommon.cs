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
        public int ApiID  = 0;
        public string ApiJson = "";
        public string errCode = "000000";
        public string errMsg = "Success";
        private readonly List<int> PayNxt = new List<int> { 179,188 };
        private MonSubsSp msp = new MonSubsSp();

        public BuyNowNxtCommon()
        {
        }

        public bool CkApiID()
        {
            if (PayNxt.Any(x => x == ApiID))
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
        public bool exeNxt()
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
                if (ApiID == 179)
                {
                    if (!string.IsNullOrEmpty(ApiJson) && !string.IsNullOrWhiteSpace(ApiJson))
                    {
                        var spIn = JsonConvert.DeserializeObject<SPInput_CreateSubsMonth>(ApiJson);
                        if (!string.IsNullOrWhiteSpace(spIn.IDNO) && !string.IsNullOrWhiteSpace(spIn.MonProjID) &&
                            spIn.LogID > 0 && spIn.MonProPeriod > 0)
                        {
                            spIn.SetPayOne = 1;//首期金額已付
                            flag = msp.sp_CreateSubsMonth(spIn, ref errCode);
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
                else if(ApiID == 188)
                {
                    if (!string.IsNullOrEmpty(ApiJson) && !string.IsNullOrWhiteSpace(ApiJson))
                    {
                        var spIn = JsonConvert.DeserializeObject<SPInput_UpSubsMonth>(ApiJson);
                        if (!string.IsNullOrWhiteSpace(spIn.IDNO) && spIn.LogID > 0 &&
                            !string.IsNullOrWhiteSpace(spIn.MonProjID) && spIn.MonProPeriod > 0 &&
                            !string.IsNullOrWhiteSpace(spIn.UP_MonProjID) && spIn.UP_MonProPeriod > 0 )
                        {
                            flag = msp.sp_UpSubsMonth(spIn, ref errCode);
                            trace.traceAdd("UpSubsMonth", new { flag,errCode });
                        }
                        else
                        {
                            errCode = "ERR257";//參數遺漏
                            flag = false;
                        }
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