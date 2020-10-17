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
    public class BE_HandleTransParkingController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】修改調度停車場
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doBE_HandleTransParking(Dictionary<string, object> value)
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
            string funName = "BE_HandleTransParkingController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_HandleTransParking apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            DateTime SD=DateTime.Now, ED=DateTime.Now;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_HandleTransParking>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID,apiInput.ParkingAddress,apiInput.ParkingName,apiInput.OpenTime,apiInput.CloseTime,apiInput.UserID };
                string[] errList = { "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if(apiInput.Latitude<=0 || apiInput.Longitude <= 0)
                {
                    flag = false;
                    errCode = "ERR900";
                }
                if (flag)
                {
                    flag = DateTime.TryParse(apiInput.OpenTime, out SD);
                    if (flag == false)
                    {
                        errCode = "ERR900";
                    }
                    else
                    {
                        flag = DateTime.TryParse(apiInput.CloseTime, out ED);
                        if (flag == false)
                        {
                            errCode = "ERR900";
                        }
                    }
                }

            }
            #endregion

            #region TB

            if (flag)
            {

                string spName = new ObjType().GetSPName(ObjType.SPType.BE_HandleTransParking);
                SPInput_BE_HandleTransParking spInput = new SPInput_BE_HandleTransParking()
                {
                    LogID = LogID,
                     CloseTime=ED,
                     OpenTime=SD,
                      Latitude=apiInput.Latitude,
                       Longitude=apiInput.Longitude,
                        ParkingAddress=apiInput.ParkingAddress,
                         ParkingID=apiInput.ParkingID,
                          ParkingName=apiInput.ParkingName,
                           UserID=apiInput.UserID


                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_BE_HandleTransParking, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_HandleTransParking, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);

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

