using Domain.Common;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 資料最近更新列表
    /// </summary>
    public class GetUPDListController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private CommonRepository _repository;
        [HttpGet]
        public Dictionary<string, object> DoGetUPDList()
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetUPDListController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            OAPI_GetUPDList GetUPDListAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            UPDList ObjOut = null;
            _repository = new CommonRepository(connetStr);
            #endregion
            #region 防呆

            if (flag)
            {

                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("No Input", ClientIP, funName, ref errCode, ref LogID);

            }
            #endregion

            #region TB
            if (flag)
            {
                // lstOut = new List<CityData>();
                try
                {
                    ObjOut = _repository.GetUpdList();
                    if (ObjOut!=null)
                    {
                        GetUPDListAPI = new OAPI_GetUPDList()
                        {
                             AreaList=(ObjOut.AreaList==null)?null:(ObjOut.AreaList.Value.ToString("yyyyMMddHHmmss")),
                              LoveCode = (ObjOut.LoveCode == null) ? null : (ObjOut.LoveCode.Value.ToString("yyyyMMddHHmmss")),
                               NormalRent = (ObjOut.NormalRent == null) ? null : (ObjOut.NormalRent.Value.ToString("yyyyMMddHHmmss")),
                                Polygon = (ObjOut.Polygon == null) ? null : (ObjOut.Polygon.Value.ToString("yyyyMMddHHmmss")),
                            Parking = (ObjOut.Parking == null) ? null : (ObjOut.Parking.Value.ToString("yyyyMMddHHmmss"))
                        };
                    }
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, GetUPDListAPI, token);
            return objOutput;
            #endregion
        }
    }
}
