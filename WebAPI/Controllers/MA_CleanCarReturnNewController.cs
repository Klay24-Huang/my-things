using Domain.Common;
using Domain.SP.MA.Input;
using Domain.SP.MA.Output;
using Domain.SP.Output;
using Domain.TB.Maintain;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Maintain.Input;
using WebAPI.Models.Param.Maintain.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【整備人員】還車
    /// </summary>
    public class MA_CleanCarReturnNewController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        string MaintainContainer = (System.Configuration.ConfigurationManager.AppSettings["MaintainContainer"] == null) ? "" : System.Configuration.ConfigurationManager.AppSettings["MaintainContainer"].ToString();
        [HttpPost]
        public Dictionary<string, object> DoMA_CleanCarReturn(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "MA_CleanCarReturnNewController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_MA_CleanCarReturnNew apiInput = null;
            NullOutput outputApi = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            CarRepository _repository = new CarRepository(connetStr);
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            DateTime SD = DateTime.Now;
            DateTime ED = DateTime.Now;
            string ManageStation = "";
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string CID = "";

            string IDNO = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MA_CleanCarReturnNew>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

            }
            if (flag)
            {
                string[] checkList = { apiInput.UserID, apiInput.CarNo };
                string[] errList = { "ERR900", "ERR900" };
                //判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
            }
            #endregion
            #region TB
            if (flag)
            {
                //usp_SettingClearMemberData_202003
                string spName = "usp_MA_CleanCarEnd";
                string incarPic = "";
                string outcarPic = "";
                if (apiInput.incarPic != "")
                {
                    
                    if(apiInput.incarPicType== "image/jpeg;base64")
                    {
                        incarPic = string.Format("{0}_incarPic_{1}.jpg", apiInput.OrderNum, DateTime.Today.ToString("yyyyMMddHHmmss"));
                      
                    }
                    else
                    {
                        incarPic = string.Format("{0}_incarPic_{1}.png", apiInput.OrderNum, DateTime.Today.ToString("yyyyMMddHHmmss"));
                    }
                    flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.incarPic, incarPic, MaintainContainer);
                    if (flag) {
                        apiInput.incarPic = incarPic;
                    }
                   
                }
                if (apiInput.outcarPic != "")
                {

                    if (apiInput.outcarPicType == "image/jpeg;base64")
                    {
                        outcarPic = string.Format("{0}_outcarPic_{1}.jpg", apiInput.OrderNum, DateTime.Today.ToString("yyyyMMddHHmmss"));

                    }
                    else
                    {
                        outcarPic = string.Format("{0}_outcarPic_{1}.png", apiInput.OrderNum, DateTime.Today.ToString("yyyyMMddHHmmss"));
                    }
                    flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.outcarPic, incarPic, MaintainContainer);
                    if (flag)
                    {
                        apiInput.outcarPic = outcarPic;
                    }
                    
                }


                SPInput_MA_CleanCarEndNew spInput = new SPInput_MA_CleanCarEndNew()
                {
                    OrderNum = apiInput.OrderNum,
                    CarNo = apiInput.CarNo,
                    UserID = apiInput.UserID,
                    Anydispatch = apiInput.Anydispatch,
                    Maintenance = apiInput.Maintenance,
                    dispatch = apiInput.dispatch,
                    insideClean = apiInput.insideClean,
                    outsideClean = apiInput.outsideClean,
                    rescue = apiInput.rescue,
                    remark = apiInput.remark,
                    incarPic = apiInput.incarPic,
                    outcarPic = apiInput.outcarPic,
                    incarPicType = apiInput.incarPicType,
                    outcarPicType = apiInput.outcarPicType,
                    isCar = apiInput.IsCar,
                    LogID = LogID
                };
                SPOutput_MA_ClearCarReturn spOut = new SPOutput_MA_ClearCarReturn();

                flag = new SQLHelper<SPInput_MA_CleanCarEndNew, SPOutput_MA_ClearCarReturn>(connetStr).ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag,  spOut.Error,spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    CID = spOut.CID;
                }
            }

            #endregion
            #region 車機
            if (flag)
            {
                new CarCommonFunc().DoMACloseRent(apiInput.OrderNum, apiInput.UserID, LogID, apiInput.UserID, ref errCode);
            }
            #endregion
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}
