using Domain.Common;
using Domain.TB;
using Domain.TB.BackEnd;
using Newtonsoft.Json;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using WebAPI.Models;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.BackEnd.Input;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 發票類型查詢
    /// </summary>
    public class GetMemberInvoiceSettingController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private CommonRepository _repository;
        /// <summary>
        /// 發票類型查詢
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
                try
                {
                    lstOut.MemberInvoice = _repository.GetMemberDataFromOrder(tmpOrder);
                    lstOut.LoveCodeList = _repository.GetLoveCode();

                    if (flag)
                    {
                        var CID = lstOut.MemberInvoice.FirstOrDefault().CID;
                        var StationID = lstOut.MemberInvoice.FirstOrDefault().lend_place;

                        CarInfo info = new CarStatusCommon(connetStr).GetInfoByCar(CID);

                        Domain.Common.Polygon Nowlatlng = new Domain.Common.Polygon()
                        {
                            Latitude = info.Latitude,
                            Longitude = info.Longitude
                        };
                        var Carflag = CheckInPolygon(Nowlatlng, StationID);
                        if (Carflag == false)
                        {
                            lstOut.MemberInvoice.FirstOrDefault().IsArea = "N";
                        }
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
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, lstOut, token);
            return objOutput;
            #endregion
        }

        private bool CheckInPolygon(Domain.Common.Polygon latlng, string StationID)
        {
            bool flag = false;
            StationAndCarRepository _repository = new StationAndCarRepository(connetStr);

            List<GetPolygonRawData> lstData = new List<GetPolygonRawData>();
            lstData = _repository.GetPolygonRaws(StationID);
            bool polygonFlag = false;
            int DataLen = lstData.Count;
            PolygonModel pm = new PolygonModel();
            for (int i = 0; i < DataLen; i++)
            {
                string[] tmpLonGroup = lstData[i].Longitude.Split('⊙');
                string[] tmpLatGroup = lstData[i].Latitude.Split('⊙');
                int tmpLonGroupLen = tmpLonGroup.Length;

                for (int j = 0; j < tmpLonGroupLen; j++)
                {
                    string[] tmpLon = tmpLonGroup[j].Split(',');
                    string[] tmpLat = tmpLatGroup[j].Split(',');
                    int LonLen = tmpLon.Length;
                    List<Domain.Common.Polygon> polygonGroups = new List<Domain.Common.Polygon>();
                    for (int k = 0; k < LonLen; k++)
                    {
                        polygonGroups.Add(new Domain.Common.Polygon()
                        {
                            Latitude = Convert.ToDouble(tmpLat[k]),
                            Longitude = Convert.ToDouble(tmpLon[k])
                        });
                    }

                    polygonFlag = pm.isInPolygonNew(ref polygonGroups, latlng);
                    if (polygonFlag)
                    {
                        if (lstData[i].PolygonMode == 0)
                        {
                            break;
                        }
                        else
                        {
                            polygonFlag = false;
                            break;
                        }
                    }
                }
            }
            flag = polygonFlag;
            return flag;
        }
    }
}