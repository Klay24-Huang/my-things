using Domain.Common;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 後台使用api，取得電子柵欄
    /// </summary>
    public class GetPolygonForWebController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoGerPolygon(Dictionary<string, object> value)
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
            string funName = "GetPolygonForWebController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetPolygon apiInput = null;
            OAPI_GetPolygonForWeb outputApi = null;
            List<Domain.TB.Polygon> lstTmpData = new List<Domain.TB.Polygon>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            StationAndCarRepository _repository;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetPolygon>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (flag)
                {
                    if (flag)
                    {
                        if (string.IsNullOrWhiteSpace(apiInput.StationID))
                        {
                            flag = false;
                            errCode = "ERR900";

                        }
                    }
                }
            }
            #endregion
            #region TB
            if (flag)
            {
                _repository = new StationAndCarRepository(connetStr);

                List<GetPolygonRawData> lstData = new List<GetPolygonRawData>();
                lstData = _repository.GetPolygonRaws(apiInput.StationID);
                int DataLen = 0;
                if (lstData != null)
                {
                    DataLen = lstData.Count;
                }
                if (DataLen > 0)
                {
                    outputApi = new OAPI_GetPolygonForWeb();
                    outputApi.PolygonObj = new List<Models.Param.Output.PartOfParam.PolygonDataForWeb>();
                }
                for (int i = 0; i < DataLen; i++)
                {
                    Models.Param.Output.PartOfParam.PolygonDataForWeb obj = new Models.Param.Output.PartOfParam.PolygonDataForWeb()
                    {
                        PolygonType = lstData[i].PolygonMode
                    };
                    string[] tmpLonGroup = lstData[i].Longitude.Split('⊙');
                    string[] tmpLatGroup = lstData[i].Longitude.Split('⊙');
                    int tmpLonGroupLen = tmpLonGroup.Length;
                   
                    obj.PolygonObj = new string[tmpLonGroupLen];
                    for (int j = 0; j < tmpLonGroupLen; j++)
                    {
                        string tmpData = "";
                        string[] tmpLon = tmpLonGroup[j].Split(',');
                        string[] tmpLat = tmpLatGroup[j].Split(',');
                        int LonLen = tmpLon.Length;



                        for (int k = 0; k < LonLen; k++)
                        {
                            if (k == 0)
                            {
                                tmpData += string.Format("{0},{1}", tmpLon[k], (tmpLat[k] == null) ? "0" : tmpLat[k]);
                            }
                            else
                            {
                                tmpData += string.Format(" {0},{1}", tmpLon[k], (tmpLat[k] == null) ? "0" : tmpLat[k]);
                            }
                        }
                         obj.PolygonObj[j]= tmpData;
                      
                    }

                  
                    outputApi.PolygonObj.Add(obj);
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}
