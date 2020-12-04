using Domain.Common;
using Domain.TB;
using Domain.TB.BackEnd;
using Newtonsoft.Json;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 縣市列表 20200812 recheckOK
    /// </summary>
    public class GetMemberInvoiceSettingController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private CommonRepository _repository;
        /// <summary>
        /// 縣市列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doGetMemberInvoiceSetting(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetMemberInvoiceSettingController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            Int64 tmpOrder = 0;

            //OAPI_CityList CityListAPI = null;
            IAPI_BE_GetOrderModifyInfo apiInput = JsonConvert.DeserializeObject<IAPI_BE_GetOrderModifyInfo>(JsonConvert.SerializeObject(value));
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            BE_MemberInvoiceSettingData lstOut = new BE_MemberInvoiceSettingData();
            _repository = new CommonRepository(connetStr);
            #endregion
            #region 防呆
         
            if (flag)
            {
             
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(JsonConvert.SerializeObject(value), ClientIP, funName, ref errCode, ref LogID);

            }
            #endregion

            flag = Int64.TryParse(apiInput.OrderNo.Replace("H", ""), out tmpOrder);

            #region TB
            if (flag)
            {
                // lstOut = new List<CityData>();
                try
                {
                    lstOut.MemberInvoice = _repository.GetMemberDataFromOrder(tmpOrder);
                    lstOut.LoveCodeList = _repository.GetLoveCode();
                }
                catch (Exception ex)
                {
                    flag = false;
                    errMsg = ex.Message;
                }

            }
            #endregion
           
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, lstOut, token);
            return objOutput;
            #endregion
        }
    }
}
