using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Output;
using Domain.WebAPI.Input.CENS;
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
    /// <summary>
    /// 【後台】設定電子柵欄
    /// </summary>
    public class BE_HandlePolygonController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】設定電子柵欄
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoHandlePolygon(Dictionary<string, object> value)
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
            string funName = "BE_HandlePolygonController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_HandlePolygon apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            DateTime SD = DateTime.Now, ED = DateTime.Now;
            string Longitude = "", Latitude = "";


            bool isGuest = true;
        
            string Contentjson = "";

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_HandlePolygon>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.StationID, apiInput.BlockName };
                string[] errList = { "ERR900", "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);

                if (flag)
                {
                    if(!string.IsNullOrEmpty(apiInput.StartDate) && !string.IsNullOrEmpty(apiInput.EndDate))
                    {
                        
                        if (flag)
                        {
                            flag = DateTime.TryParse(apiInput.StartDate + " 00:00:00", out SD);
                            if (flag)
                            {
                                flag = DateTime.TryParse(apiInput.EndDate + " 23:59:59", out ED);
                                if (flag == false)
                                {
                                    errCode = "ERR900";
                                }
                            }
                            else
                            {
                                errCode = "ERR900";
                            }
                        }
                        if (flag)
                        {
                            if (SD > ED)
                            {
                                flag = false;
                                errCode = "ERR900";
                            }
                        }
                  
                    }
                  
                }
                if (flag)
                {
                    if (apiInput.polygon != null)
                    {
                        int count = apiInput.polygon.Count;
                        for(int i = 0; i < count; i++)
                        {
                            string tmpLat = "",tmpLng = "";
                            if (apiInput.polygon[i].RawData != null)
                            {
                                int RawCount = apiInput.polygon[i].RawData.Count;
                                for(int j = 0; j < RawCount; j++)
                                {
                                    if (j > 0)
                                    {
                                        tmpLat += ",";
                                        tmpLng += ",";
                                    }
                                    tmpLat += apiInput.polygon[i].RawData[j].lat;
                                    tmpLng += apiInput.polygon[i].RawData[j].lng;
                                }
                                if (i > 0)
                                {
                                    Latitude += "⊙";
                                    Longitude += "⊙";
                                }
                                Latitude += tmpLat;
                                Longitude += tmpLng;
                            }
                         
                        }
                    }
                }

            }
            #endregion

            #region TB

            if (flag)
            {

                string spName = new ObjType().GetSPName(ObjType.SPType.BE_HandlePolygon);
                SPInput_BE_HandlePolygon spInput = new SPInput_BE_HandlePolygon()
                {
                    LogID = LogID,
                    UserID = apiInput.UserID,
                    StartDate = SD,
                    BlockID = apiInput.BlockID,
                    BlockName = apiInput.BlockName,
                    EndDate = ED,
                    MAPColor = apiInput.MAPColor,
                    Mode = apiInput.Mode,
                    StationID = apiInput.StationID,
                    Latitude = Latitude,
                    Longitude = Longitude

                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_BE_HandlePolygon, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_HandlePolygon, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
  

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
