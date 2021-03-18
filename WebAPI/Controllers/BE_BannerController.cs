using System.Linq;
using System.Net;
using System.Net.Http;
using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class BE_BannerController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        string bannerContainer = (System.Configuration.ConfigurationManager.AppSettings["bannerContainer"] == null) ? "" : System.Configuration.ConfigurationManager.AppSettings["bannerContainer"].ToString();

        [HttpPost]
        public Dictionary<string, object> doBE_Banner(Dictionary<string, object> value)
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
            string funName = "BE_BannerController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_Banner apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            DateTime SD = DateTime.Now, ED = DateTime.Now;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_Banner>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                //string[] checkList = { apiInput.UserID, apiInput.StationID, apiInput.StationName, apiInput.ManagerStationID, apiInput.FCode, apiInput.Addr, apiInput.show_description };
                //string[] errList = { "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900" };
                ////1.判斷必填
                //flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);

                if (flag)
                {
                    if (apiInput.SDate > apiInput.EDate)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else
                    {
                        ED = apiInput.EDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                }

            }
            #endregion
            #region 處理azure
            if (flag)
            {
                if (apiInput.fileData1 != "")
                {
                    string FileName = string.Format("{0}_1_{1}.png", apiInput.fileName1, DateTime.Today.ToString("yyyyMMddHHmmss"));
                    flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.fileData1, FileName, bannerContainer);
                    if (flag)
                    {
                        apiInput.fileName1 = FileName;
                    }
                }
            }
            #endregion
            #region TB

            if (flag)
            {

                string spName = new ObjType().GetSPName(ObjType.SPType.BE_Banner);
                SPInput_Banner spInput = new SPInput_Banner()
                {
                    SDate = apiInput.SDate,
                    EDate = ED,
                    fileName1 = apiInput.fileName1,
                    StationType = apiInput.StationType,
                    URL = apiInput.URL,
                    RunHorse = apiInput.RunHorse,
                    UserID = apiInput.UserID,
                    SEQNO = apiInput.SEQNO
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_Banner, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_Banner, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                //baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);

            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}
