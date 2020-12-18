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
    /// 取得取消訂單列表
    /// </summary>
    public class GetCancelOrderListController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());
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
            string funName = "GetCancelOrderListController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetCancelOrderList apiInput = null;
            OAPI_GetCancelOrderList outputApi = new OAPI_GetCancelOrderList();
            outputApi.CancelObj = new List<OrderCancelObj>();
            Int64 tmpOrder = -1;
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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetCancelOrderList>(Contentjson);
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
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.GetCancelOrder);
                SPInput_GetCancelOrder spCheckTokenInput = new SPInput_GetCancelOrder()
                {
                    LogID = LogID,
                    Token = Access_Token,
                    IDNO = IDNO,
                    pageNo = NowPage,
                    pageSize = 10
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetCancelOrder, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetCancelOrder, SPOutput_Base>(connetStr);
                List<OrderCancelDataList> orderCancelDataLists = new List<OrderCancelDataList>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(CheckTokenName, spCheckTokenInput, ref spOut, ref orderCancelDataLists, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    BillCommon billCommon = new BillCommon();
                    int DataLen = orderCancelDataLists.Count;
                    if (DataLen > 0)
                    {
                        outputApi.TotalPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(orderCancelDataLists[0].TotalCount / pageSize))) + ((orderCancelDataLists[0].TotalCount % pageSize > 0) ? 1 : 0);
                        for (int i = 0; i < DataLen; i++)
                        {
                            OrderCancelObj obj = new OrderCancelObj()
                            {
                                CarBrend = orderCancelDataLists[i].CarBrend,
                                CarNo = orderCancelDataLists[i].CarNo.Replace(" ", ""),
                                CarRentBill = orderCancelDataLists[i].init_price,
                                CarTypeImg = orderCancelDataLists[i].CarTypeImg,
                                CarTypeName = orderCancelDataLists[i].CarTypeName,
                                ED = orderCancelDataLists[i].stop_time,
                                Milage = billCommon.CarMilageCompute(orderCancelDataLists[i].start_time, orderCancelDataLists[i].stop_time, orderCancelDataLists[i].MilageUnit, Mildef, 20, new List<Holiday>()),
                                MilageUnit = (orderCancelDataLists[i].MilageUnit < 0) ? Mildef : orderCancelDataLists[i].MilageUnit,
                                MilOfHours = (orderCancelDataLists[i].MilageUnit == 0) ? 0 : 20,
                                OperatorICon = orderCancelDataLists[i].OperatorICon,
                                order_number = string.Format("H{0}", orderCancelDataLists[i].order_number.ToString().PadLeft(7, '0')),
                                Price = orderCancelDataLists[i].init_price,
                                ProjID = orderCancelDataLists[i].ProjID,
                                PRONAME = orderCancelDataLists[i].PRONAME,
                                Score = orderCancelDataLists[i].Score,
                                SD = orderCancelDataLists[i].start_time,
                                Seat = orderCancelDataLists[i].Seat,
                                CarOfArea = orderCancelDataLists[i].CarOfArea,
                                StationName = orderCancelDataLists[i].StationName,
                                IsMotor = orderCancelDataLists[i].IsMotor,
                                WeekdayPrice = orderCancelDataLists[i].WeekdayPrice,
                                HoildayPrice = orderCancelDataLists[i].HoildayPrice,
                                WeekdayPriceByMinutes = orderCancelDataLists[i].WeekdayPriceByMinutes,
                                HoildayPriceByMinutes = orderCancelDataLists[i].HoildayPriceByMinutes,
                                InsuranceBill = orderCancelDataLists[i].InsurancePurePrice,
                                TransDiscount = (orderCancelDataLists[i].init_TransDiscount < 0) ? 0 : orderCancelDataLists[i].init_TransDiscount,
                                MileageBill = billCommon.CarMilageCompute(orderCancelDataLists[i].start_time, orderCancelDataLists[i].stop_time, orderCancelDataLists[i].MilageUnit, Mildef, 20, new List<Holiday>()),
                            };
                            obj.Bill = obj.CarRentBill + obj.InsuranceBill + obj.MileageBill - obj.TransDiscount;
                            outputApi.CancelObj.Add(obj);
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