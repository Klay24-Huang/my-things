using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.TB;
using Domain.TB.BackEnd;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.ComboFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.BackEnd.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】儲存合約修改變更(點數折抵模式)
    /// </summary>
    public class BE_HandleOrderModifyByDiscountController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】儲存合約修改變更(點數折抵模式)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> BE_HandleOrderModifyByDiscount(Dictionary<string, object> value)
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
            string funName = "BE_HandleOrderModifyByDiscountController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_HandleOrderModifyByDiscount apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            Int64 tmpOrder = 0;
            DateTime SD = DateTime.Now, ReturnDate = DateTime.Now;
            BE_GetOrderModifyDataNew obj = null;
            ContactComm contact = new ContactComm();
            int NewFinalPrice = 0;
            int DiffFinalPrice = 0;

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_HandleOrderModifyByDiscount>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.OrderNo };
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);

                if (flag)
                {
                    if (apiInput.OrderNo.IndexOf("H") < 0)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    if (flag)
                    {
                        flag = Int64.TryParse(apiInput.OrderNo.Replace("H", ""), out tmpOrder);
                        if (flag)
                        {
                            if (tmpOrder <= 0)
                            {
                                flag = false;
                                errCode = "ERR900";
                            }

                        }
                    }
                }

            }
            #endregion

            #region TB

            if (flag)
            {

                obj = new ContactRepository(connetStr).GetModifyData(tmpOrder);
                BillCommon billCommon = new BillCommon();
                int totalPointer = 0;
                if (obj == null)
                {
                    flag = false;
                }
                else
                {

                    IDNO = obj.IDNO;
                    PointerComm pointer = new PointerComm();
                    int TotalLastPoint = 0, TotalLastPointCar = 0, TotalLastPointMotor = 0, CanUseTotalCarPoint = 0, CanUseTotalMotorPoint = 0;
                    flag = pointer.GetPointer(IDNO, obj.FS, obj.ED, obj.FE, obj.FT, obj.PROJTYPE, ref TotalLastPoint, ref TotalLastPointCar, ref TotalLastPointMotor, ref CanUseTotalCarPoint, ref CanUseTotalMotorPoint);
                    if (flag)
                    {
                        if (apiInput.CarPoint > CanUseTotalCarPoint || apiInput.MotorPoint > CanUseTotalMotorPoint)
                        {
                            flag = false;
                            errCode = "ERR207";
                        }
                    }
                    if (flag)
                    {
                        int days = 0, hours = 0, minutes = 0;
                        if (obj.PROJTYPE == 4)
                        {
                            totalPointer = apiInput.CarPoint + apiInput.MotorPoint;
                            billCommon.CalPointerToDayHourMin(totalPointer, ref days, ref hours, ref minutes);
                            int discount = Convert.ToInt32(Math.Round((obj.MaxPrice * days) + (obj.WeekdayPriceByMinutes * 60 * hours) + (obj.WeekdayPriceByMinutes * minutes), 0));

                            NewFinalPrice = ((obj.final_price - discount) > 0) ? (obj.final_price - discount) : 0;

                        
                           DiffFinalPrice = obj.final_price - NewFinalPrice;
                        }
                        else
                        {
                            totalPointer = apiInput.CarPoint;
                            billCommon.CalPointerToDayHourMin(totalPointer, ref days, ref hours, ref minutes);
                            double hour = hours + Convert.ToDouble(minutes / 60.0);
                            int discount = (obj.WeekdayPrice * days) + Convert.ToInt32((obj.WeekdayPrice / 10) * hour);

                            NewFinalPrice = ((obj.final_price - discount) > 0) ? (obj.final_price - discount) : 0;

                           DiffFinalPrice = obj.final_price - NewFinalPrice;
                        }
                    }
                    if (flag)
                    {
                        if (DiffFinalPrice != apiInput.DiffPrice)
                        {
                            flag = false;
                            errCode = "ERR758";
                        }

                    }
                    if (flag)
                    {
                        if (NewFinalPrice != apiInput.FinalPrice)
                        {
                            flag = false;
                            errCode = "ERR759";
                        }
                    }
                    /*查詢短租訂單狀態*/
                    if (flag)
                    {
                        string STATUS = "", CNTRNO = "", INVSTATUS = "";
                       flag = contact.DoNPR135(apiInput.OrderNo, ref errCode, ref errMsg, ref STATUS, ref CNTRNO, ref INVSTATUS);
                        if (flag)
                        {
                            if(INVSTATUS != "Y")
                            {
                                flag = false;
                                errCode = "ERR760";
                            }else if (Convert.ToInt32(STATUS) < 3)
                            {
                                flag = false;
                                errCode = "ERR761";
                            }
                        }
                    }
                    /*判斷是否要取款或是刷退*/
                    if (apiInput.DiffPrice == 0 || obj.Paid==0)
                    {
                        //直接更新
                    }else 
                    {
                        //查詢有無綁卡
                        if (apiInput.DiffPrice > 0) //刷退，
                        {
                            int hasBind = 0;
                            List<CreditCardBindList> lstBind = new List<CreditCardBindList>();
                            CreditAuthComm Credit = new CreditAuthComm();
                            flag = Credit.DoQueryCardList(obj.IDNO, ref hasBind, ref lstBind, ref errCode, ref errMsg);
                            if (flag)
                            {
                                if (hasBind == 0)
                                {
                                    flag = false;
                                    errCode = "ERR762";
                                }
                                else
                                {
                                     //flag = Credit.DoAuth(apiInput.OrderNo, apiInput.DiffPrice, lstBind[0].CardToken,3, ref errCode, ref errMsg);
                                }
                            }
                        }
                        else  //取款從1.0開始統一從欠款查詢中直接取款，不由此處理
                        {

                        }
                        

                    }
                    
                    /*傳送短租136*/



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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}
