using Domain.Common;
using Domain.SP.Input.Common;
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
using WebAPI.Models.Enum;
using Domain.SP.Output.Common;
using Domain.SP.Input.PolygonList;
using Domain.SP.Output.PolygonList;
using Domain.SP.Output;
using System.Data;
using System.Linq;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得電子柵欄資料
    /// </summary>
    public class GetPolygonController : ApiController
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
            string funName = "GetPolygonController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetPolygon apiInput = null;
            OAPI_GetPolygon outputApi = null;
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
            }

            #endregion
            #region TB

            if (flag)
            {
                SPInput_PolygonListQuery spInput = new SPInput_PolygonListQuery()
                {
                    StationID = apiInput.StationID,
                    IsMotor = apiInput.IsMotor,
                    LogID = LogID                    
                };

                string SPName = new ObjType().GetSPName(ObjType.SPType.PolygonListQuery);
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_PolygonListQuery, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_PolygonListQuery, SPOutput_Base>(connetStr);
                List<SPOutput_PolygonListQuery> PolygonList = new List<SPOutput_PolygonListQuery>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref PolygonList, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    var lstData = PolygonList.Select(x => new GetPolygonRawData() {
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        PolygonMode = x.PolygonMode
                    }).ToList();

                    int DataLen = 0;
                    if (lstData != null)
                    {
                        DataLen = lstData.Count;
                    }
                    if (DataLen > 0)
                    {
                        outputApi = new OAPI_GetPolygon();
                        outputApi.PolygonObj = new List<Models.Param.Output.PartOfParam.PolygonData>();
                    }
                    for (int i = 0; i < DataLen; i++)
                    {
                        Models.Param.Output.PartOfParam.PolygonData obj = new Models.Param.Output.PartOfParam.PolygonData()
                        {
                            PolygonType = lstData[i].PolygonMode
                        };
                        string[] tmpLonGroup = lstData[i].Longitude.Split('⊙');
                        string[] tmpLatGroup = lstData[i].Latitude.Split('⊙');
                        int tmpLonGroupLen = tmpLonGroup.Length;


                        obj.PolygonObj = new string[tmpLonGroupLen];
                        for (int j = 0; j < tmpLonGroupLen; j++)
                        {
                            string tmpData = "";

                            string[] tmpLon = tmpLonGroup[j].Split(',');
                            string[] tmpLat = tmpLatGroup[j].Split(',');
                            int LonLen = tmpLon.Length;

                            //20200930 ADD BY ADAM REASON.增加POLYGON字串
                            tmpData += "POLYGON((";

                            for (int k = 0; k < LonLen; k++)
                            {
                                //if (j == 0)
                                if (k == 0)
                                {
                                    tmpData += string.Format("{0} {1}", tmpLon[k], (tmpLat[k] == null) ? "0" : tmpLat[k]);
                                }
                                else
                                {
                                    tmpData += string.Format(",{0} {1}", tmpLon[k], (tmpLat[k] == null) ? "0" : tmpLat[k]);
                                }
                            }
                            //判斷最後一點是否回來
                            if (tmpLon[0].ToString() != tmpLon[LonLen - 1].ToString())
                            {
                                tmpData += string.Format(",{0} {1}", tmpLon[0], (tmpLat[0] == null) ? "0" : tmpLat[0]);
                            }


                            //20200930 ADD BY ADAM REASON.增加POLYGON字串
                            tmpData += "))";

                            obj.PolygonObj[j] = tmpData;
                        }

                        //obj.PolygonObj = tmpData;
                        outputApi.PolygonObj.Add(obj);
                    }
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
