using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using System.Data;
using WebAPI.Utils;
using Domain.SP.Output;
using System.CodeDom;
using Domain.SP.Input.Arrears;
using WebAPI.Models.BillFunc;
using Domain.SP.Output.Car;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 車型下拉選單
    /// </summary>
    public class GetCarTypeGroupListController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost()]
        public Dictionary<string, object> DoGetCarTypeGroupList([FromBody] Dictionary<string, object> value)
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
            string funName = "GetCarTypeGroupListController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            var apiInput = new IAPI_GetCarTypeGroupList();
            var outputApi = new OAPI_GetCarTypeGroupList();
            outputApi.SeatGroups = new List<GetProject_SeatGroup>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            var SeatGroups = new List<GetProject_SeatGroup>();

            string Contentjson = "";
            bool isGuest = true;

            #endregion
            #region 防呆

            //flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            //if(errCode == "ERR901")
            //{
            //    flag = true;
            //    errCode = "000000";
            //    errMsg = "Success";
            //}

            //if (flag)
            //{
            //    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetCarTypeGroupList>(Contentjson);
            //    //寫入API Log
            //    string ClientIP = baseVerify.GetClientIp(Request);
            //    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
            //}

            #endregion
            #region TB

            var spList = sp_GetCarTypeGroupList(ref errCode, ref errMsg);
            if(spList != null && spList.Count() > 0)
            {
                List<int> SeatsList = spList.GroupBy(x => x.Seat).Select(y => y.FirstOrDefault().Seat).ToList();
                if (SeatsList != null && SeatsList.Count() > 0)
                {
                    foreach (int se in SeatsList)
                    {
                        var item = new GetProject_SeatGroup();
                        item.Seat = Convert.ToInt16(se);

                        List<GetProject_CarInfo> CarInfos =
                            (from a in spList
                             where a.Seat == se
                             group new { a.Seat, a.CarBrend, a.CarTypeName, a.CarTypeImg }
                             by new { a.Seat, a.CarBrend, a.CarTypeName, a.CarTypeImg } into g
                             select new GetProject_CarInfo
                             {
                                 Seat = item.Seat,
                                 CarType = g.Key.CarBrend + ' ' + g.Key.CarTypeName,
                                 CarTypePic = g.Key.CarTypeImg,
                                 CarTypeName = g.Key.CarTypeName
                             }).ToList();
                        if (CarInfos != null && CarInfos.Count() > 0)
                            item.CarInfos = CarInfos;
                        SeatGroups.Add(item);
                    }
                }
                if (SeatGroups != null && SeatGroups.Count() > 0)
                    outputApi.SeatGroups = SeatGroups;
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

        private List<SPOut_GetCarTypeGroupList> sp_GetCarTypeGroupList(ref string errCode, ref string errMsg)
        {
            var re = new List<SPOut_GetCarTypeGroupList>();

            try
            {
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetCarTypeGroupList);
                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                object[][] objparms = {
                            new object[] {},
                            new object[] {}
                        };

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, objparms, true, ref returnMessage, ref messageLevel, ref messageType);

                if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
                {
                    if (ds1.Tables.Count >= 2)
                        re = objUti.ConvertToList<SPOut_GetCarTypeGroupList>(ds1.Tables[0]);
                    else if (ds1.Tables.Count == 1)
                    {
                        var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[0]);
                        if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                        {
                            errCode = re_db.ErrorCode;
                            errMsg = re_db.ErrorMsg;
                        }
                    }
                }
                else
                    errMsg = returnMessage;

                return re;
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
                throw ex;
            }
        }


    }
}
