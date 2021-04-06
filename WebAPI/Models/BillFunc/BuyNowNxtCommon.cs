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
        private readonly List<int> PayNxt = new List<int> { 179 };
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
                            return msp.sp_CreateSubsMonth(spIn, ref errCode);
                        }
                        else
                        {
                            errCode = "ERR257";//參數遺漏
                            return false;
                        }
                    }
                    else
                    {
                        errCode = "ERR267";//ApiJson錯誤
                        return false;
                    }
                }

                errMsg = "無對應ApiID";
                return false;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                throw ex;
            }
        }
    
    }
}