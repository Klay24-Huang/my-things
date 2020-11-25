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
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】合約修改重新計價
    /// </summary>
    public class BE_ReCalculateByDiscountController : ApiController
    {


            private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
            /// <summary>
            /// 【後台】修改合約前取得資料
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            [HttpPost]
            public Dictionary<string, object> DoBE_GetOrderModifyInfo(Dictionary<string, object> value)
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
                string funName = "BE_ReCalculateByDiscountController";
                Int64 LogID = 0;
                Int16 ErrType = 0;
            IAPI_BE_ReCalculateByDiscount apiInput = null;
            OAPI_BE_ReCalculateByDiscount apiOutput = null;
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

                #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_ReCalculateByDiscount>(Contentjson);
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
                int oldTotalPointer = 0;
                    if (obj == null)
                    {
                        flag = false;
                    }
                    else
                    {

                        IDNO = obj.IDNO;
                        PointerComm pointer = new PointerComm();
                        int TotalLastPoint=0,   TotalLastPointCar=0,   TotalLastPointMotor=0,  CanUseTotalCarPoint=0,  CanUseTotalMotorPoint = 0;
                        flag = pointer.GetPointer(IDNO, obj.FS, obj.ED, obj.FE, obj.FT, obj.PROJTYPE, ref TotalLastPoint, ref TotalLastPointCar, ref TotalLastPointMotor, ref CanUseTotalCarPoint, ref CanUseTotalMotorPoint);
                        if (flag)
                        {
                             if(apiInput.CarPoint>CanUseTotalCarPoint || apiInput.MotorPoint > CanUseTotalMotorPoint)
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
                                oldTotalPointer = obj.MotorPoint + obj.CarPoint; //舊折抵點數
                                totalPointer = apiInput.CarPoint + apiInput.MotorPoint;
                            //billCommon.CalPointerToDayHourMin(totalPointer, ref days, ref hours, ref minutes);
                            //int discount = Convert.ToInt32(Math.Round((obj.MaxPrice * days) + (obj.WeekdayPriceByMinutes * 60 * hours) + (obj.WeekdayPriceByMinutes * minutes), 0));
                            int oldPrice = calDiscountPrice(oldTotalPointer, 1, obj);
                            int discount= calDiscountPrice(totalPointer, 1, obj);
                            int tmpFinalPrice = obj.final_price;
                         //   obj.pure_price += oldPrice;
                            obj.final_price += oldPrice;
                            apiOutput = new OAPI_BE_ReCalculateByDiscount()
                                {
                                    RentPrice = ((obj.pure_price - discount) > 0) ? (obj.pure_price - discount) : 0,
                                    NewFinalPrice = ((obj.final_price - discount) > 0) ? (obj.final_price - discount) : 0,

                                };
                                apiOutput.DiffFinalPrice = tmpFinalPrice - apiOutput.NewFinalPrice;
                        }
                            else
                            {
                                 oldTotalPointer = obj.CarPoint;//舊折抵點數
                                 totalPointer = apiInput.CarPoint;
                            //billCommon.CalPointerToDayHourMin(totalPointer, ref days, ref hours, ref minutes);
                            //double hour =hours+Convert.ToDouble( minutes / 60.0);
                            //int discount = (obj.WeekdayPrice * days) + Convert.ToInt32((obj.WeekdayPrice/10) * hour);
                            int oldPrice = calDiscountPrice(oldTotalPointer, 0, obj);
                            int discount = calDiscountPrice(totalPointer, 0, obj);
                            int tmpFinalPrice = obj.final_price;
                          //  obj.pure_price += oldPrice;
                            obj.final_price += oldPrice;
                            apiOutput = new OAPI_BE_ReCalculateByDiscount()
                                {
                                    RentPrice = ((obj.pure_price - discount) > 0) ? (obj.pure_price - discount) : 0,
                                    NewFinalPrice = ((obj.final_price - discount) > 0) ? (obj.final_price - discount) : 0,

                                };
                                
                                apiOutput.DiffFinalPrice = tmpFinalPrice- apiOutput.NewFinalPrice  ;
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
                baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
                return objOutput;
                #endregion
            }
        private int calDiscountPrice(int pointer,int type, BE_GetOrderModifyDataNew obj)
        {
            int totalDiscountPrice = 0;
            int days = 0, hours = 0, minutes = 0;
            new BillCommon().CalPointerToDayHourMin(pointer, ref days, ref hours, ref minutes);
            if (type == 0)
            {
                double hour = hours + Convert.ToDouble(minutes / 60.0);
                totalDiscountPrice = (obj.WeekdayPrice * days) + Convert.ToInt32((obj.WeekdayPrice / 10) * hour);
            }
            else
            {
                totalDiscountPrice = Convert.ToInt32(Math.Round((obj.MaxPrice * days) + (obj.WeekdayPriceByMinutes * 60 * hours) + (obj.WeekdayPriceByMinutes * minutes), 0));
            }
            return totalDiscountPrice;
        }
        }
       
    }
