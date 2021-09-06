using Domain.CarMachine;
using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.OrderList;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.OrderList;
using Domain.TB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 完成的訂單查詢
    /// </summary>
    public class BookingFinishQueryController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> GetCancelOrderList(Dictionary<string, object> value)
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
            string funName = "BookingFinishQueryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BookingFisishQuery apiInput = null;
            OAPI_BookingFinishQuery outputApi = new OAPI_BookingFinishQuery();
            outputApi.OrderFinishObjs = new List<OrderFinishObj>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            MotorInfo motorinfo = new MotorInfo();
            CarInfo carInfo = new CarInfo();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            int NowPage = 0;
            int pageSize = 10;
            List<CardList> lstCardList = new List<CardList>();
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BookingFisishQuery>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (apiInput.NowPage.HasValue)
                {
                    if (apiInput.NowPage.Value < 0)
                    {
                        NowPage = 1;
                    }
                    else
                    {
                        NowPage = apiInput.NowPage.Value;
                    }
                }
                else
                {
                    NowPage = 1;
                }
            }
            //不開放訪客
            if (flag)
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
            }
            #endregion

            #region TB
            //Token判斷
            if (flag && isGuest == false)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }
            //Token判斷
            if (flag)
            {
                string CheckTokenName = "usp_GetFinishOrderList_20210901";
                SPInput_GetFinishOrder spCheckTokenInput = new SPInput_GetFinishOrder()
                {
                    LogID = LogID,
                    Token = Access_Token,
                    ShowYear = apiInput.ShowOneYear,
                    IDNO = IDNO,
                    pageNo = NowPage,
                    pageSize = 10
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetFinishOrder, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetFinishOrder, SPOutput_Base>(connetStr);
                List<OrderFinishDataList> orderFinishDataLists = new List<OrderFinishDataList>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(CheckTokenName, spCheckTokenInput, ref spOut, ref orderFinishDataLists, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    BillCommon billCommon = new BillCommon();
                    int DataLen = orderFinishDataLists.Count;
                    if (DataLen > 0)
                    {
                        outputApi.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(orderFinishDataLists[0].TotalCount / pageSize))) + ((orderFinishDataLists[0].TotalCount % pageSize > 0) ? 1 : 0);

                        for (int i = 0; i < DataLen; i++)
                        {
                            BillCommon billComm = new BillCommon();
                            int td = 0, th = 0, tm = 0;
                            var xre = billComm.GetTimePart(orderFinishDataLists[i].final_start_time, orderFinishDataLists[i].final_stop_time, orderFinishDataLists[i].ProjType);
                            if (xre != null)
                            {
                                td = Convert.ToInt32(xre.Item1);
                                th = Convert.ToInt32(xre.Item2);
                                tm = Convert.ToInt32(xre.Item3);
                            }

                            OrderFinishObj obj = new OrderFinishObj()
                            {
                                RentYear = orderFinishDataLists[i].RentYear,        //20201029 ADD BY ADAM 年份移到下面
                                Bill = orderFinishDataLists[i].final_price,
                                CarOfArea = orderFinishDataLists[i].Area,
                                CarTypePic = orderFinishDataLists[i].CarTypeImg,
                                OrderNo = string.Format("H{0}", orderFinishDataLists[i].OrderNo.ToString().PadLeft(7, '0')),
                                ProjType = orderFinishDataLists[i].ProjType,
                                RentDateTime = orderFinishDataLists[i].final_start_time.ToString("MM月dd日 HH:mm"),
                                StationName = orderFinishDataLists[i].StationName,
                                UniCode = orderFinishDataLists[i].UniCode,
                                TotalRentTime = string.Format("{0}天{1}時{2}分", td, th, tm),
                                CarNo = orderFinishDataLists[i].CarNo.Replace(" ", ""),
                                IsMotor = orderFinishDataLists[i].ProjType == 4 ? 1 : 0 ,    //增加IsMotor
                                IsJointOrder = orderFinishDataLists[i].IsJointOrder
                            };
                            outputApi.OrderFinishObjs.Add(obj);
                        }
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