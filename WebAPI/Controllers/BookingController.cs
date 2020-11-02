﻿using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Common;
using Domain.SP.Input.Bill;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;
using Domain.SP.Output.Bill;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 預約
    /// </summary>
    public class BookingController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoBooking(Dictionary<string, object> value)
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
            string funName = "BookingController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_Booking apiInput = null;
            OAPI_Booking outputApi = null;
    
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository=new StationAndCarRepository(connetStr);
            ProjectRepository projectRepository = new ProjectRepository(connetStr);
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            DateTime SDate = DateTime.Now;
            DateTime EDate = DateTime.Now.AddHours(1);
            DateTime LastPickCarTime = SDate.AddMinutes(15);
            string CarType = "", StationID = "";
            Int16 PayMode = 0;
            int QueryMode = 0;
            Int16 ProjType = 5;
            int NormalRentDefaultPickTime = 15;
            int AnyRentDefaultPickTime = 30;
            int MotorRentDefaultPickTime = 30;
            int price = 0, InsurancePurePrice=0;
            int InsurancePerHours = 0;
            string IDNO = "";
            
            List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));
            BillCommon billCommon = new BillCommon();
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
           
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_Booking>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.ProjID))
                {
                    flag = false;
                    errCode = "ERR900";
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
            #region 防呆第二段
            if (flag)
            {
               
                ProjectInfo obj = projectRepository.GetProjectInfo(apiInput.ProjID);
                if (obj == null)
                {
                    flag = false;
                    errCode = "ERR164";
                }
                ProjType = Convert.ToInt16(obj.PROJTYPE);
                PayMode = Convert.ToInt16(obj.PayMode);
                if (ProjType > 0)
                {
                    if (string.IsNullOrWhiteSpace(apiInput.CarNo)) //路邊及機車
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    if (flag)
                    {
                    
                        CarData CarObj = _repository.GetCarData(apiInput.CarNo);
                        if (CarObj == null)
                        {
                            flag = false;
                            errCode = "ERR165";
                        }
                        else
                        {
                            CarType = CarObj.CarType;
                            StationID = CarObj.StationID;
                        }
                    }
                    if (flag)
                    {
                        if (ProjType == 3)
                        {
                            EDate = SDate.AddHours(1);
                        }
                        else
                        {
                            EDate = SDate.AddDays(7);
                        }
                    }
                }
                else
                {
                    flag = baseVerify.CheckDate(apiInput.SDate, apiInput.EDate, ref errCode, ref SDate, ref EDate);//同站
                
                    if (flag)
                    {
                        if(string.IsNullOrWhiteSpace(apiInput.StationID) || string.IsNullOrWhiteSpace(apiInput.CarType))
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                        if (flag)
                        {
                            CarType = apiInput.CarType;
                            StationID = apiInput.StationID;
                        }
                    }
                }

                //20201103 ADD BY ADAM REASON.取得安心服務每小時價格
                if (flag)
                {
                    string GetInsurancePriceName = new ObjType().GetSPName(ObjType.SPType.GetInsurancePrice);
                    SPInput_GetInsurancePrice spGetInsurancePrice = new SPInput_GetInsurancePrice()
                    {
                        IDNO = IDNO,
                        CarType = CarType,
                        LogID = LogID
                    };
                    SPOutput_GetInsurancePrice spOut = new SPOutput_GetInsurancePrice();
                    SQLHelper<SPInput_GetInsurancePrice, SPOutput_GetInsurancePrice> sqlHelp = new SQLHelper<SPInput_GetInsurancePrice, SPOutput_GetInsurancePrice>(connetStr);
                    flag = sqlHelp.ExecuteSPNonQuery(GetInsurancePriceName, spGetInsurancePrice, ref spOut, ref lstError);
                    baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                    if (flag)
                    {
                        InsurancePerHours =int.Parse(spOut.InsurancePerHours.ToString());
                    }
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
            if (flag)
            {
                //判斷專案限制及取得專案設定
                if (ProjType == 3)
                {
                    LastPickCarTime = SDate.AddMinutes(AnyRentDefaultPickTime);
                }else if (ProjType == 4)
                {
                    LastPickCarTime = SDate.AddMinutes(MotorRentDefaultPickTime);
                }
                else
                {
                    LastPickCarTime = SDate.AddMinutes(NormalRentDefaultPickTime);
                }
            }
            if (flag)
            {
                //InsurancePurePrice = (apiInput.Insurance == 1) ? Convert.ToInt32(billCommon.CalSpread(SDate, EDate, 200, 200, lstHoliday)) : 0;
                //20201103 ADD BY SS ADAM REASON.計算安心服務價格
                InsurancePurePrice = (apiInput.Insurance == 1) ? Convert.ToInt32(billCommon.CalSpread(SDate, EDate, InsurancePerHours*10 , InsurancePerHours*10, lstHoliday)) : 0;
                //計算初始租金
                if (ProjType < 4)
                {
                    ProjectPriceBase priceBase = null;
                    if (ProjType == 0)
                    {
                         priceBase = projectRepository.GetProjectPriceBase(apiInput.ProjID, CarType, ProjType);
                    }
                    else
                    {
                        priceBase = projectRepository.GetProjectPriceBase(apiInput.ProjID, apiInput.CarNo, ProjType);
                    }
                   
                    price = Convert.ToInt32(billCommon.CalSpread(SDate, EDate, priceBase.PRICE, priceBase.PRICE_H, lstHoliday));
        
                }
                else
                {
                    ProjectPriceOfMinuteBase priceBase = projectRepository.GetProjectPriceBaseByMinute(apiInput.ProjID, apiInput.CarNo);
                    price = Convert.ToInt32(priceBase.Price);
                }
               

            }
            //開始做預約
            if (flag)
            {
                SPInput_Booking spInput = new SPInput_Booking()
                {
                    CarNo = (string.IsNullOrWhiteSpace(apiInput.CarNo)) ? "" : apiInput.CarNo,
                    Price = price,
                    Insurance = apiInput.Insurance,
                    CarType = CarType,
                    ED = EDate,
                     StopPickTime=LastPickCarTime,
                    IDNO = IDNO,
                    InsurancePurePrice = InsurancePurePrice,
                    LogID = LogID,
                    PayMode = PayMode,
                    ProjID = apiInput.ProjID,
                    ProjType = ProjType,
                    RStationID = StationID,
                    StationID = StationID,
                    Token = Access_Token,
                    SD = SDate
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.Booking);
                SPOutput_Booking spOut = new SPOutput_Booking();
                SQLHelper<SPInput_Booking, SPOutput_Booking> sqlHelp = new SQLHelper<SPInput_Booking, SPOutput_Booking>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag && spOut.haveCar==1)
                {
                    outputApi = new OAPI_Booking()
                    {
                        OrderNo = "H" + spOut.OrderNum.ToString().PadLeft(7, '0'),
                        LastPickTime = LastPickCarTime.ToString("yyyyMMddHHmmss")
                    };

                }
                else
                {
                    errCode = "ERR161";
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
