using Domain.Common;
using Domain.SP.Input.Bill;
using Domain.SP.Input.Discount;
using Domain.SP.Output;
using Domain.SP.Output.Bill;
using Domain.TB;
using Newtonsoft.Json;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 機車預估金額
    /// </summary>
    public class GetEstimateMotorController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoGetEstimateMotor(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = httpContext.Request.Headers["Authorization"] ?? "";
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetEstimateMotorController";
            Int64 LogID = 0;
            IAPI_GetEstimateMotor apiInput = null;
            OAPI_GetEstimateMotor apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            DateTime SDate = DateTime.Now.AddHours(-1);
            DateTime EDate = DateTime.Now;
            string IDNO = "";
            List<SPOutput_GetEstimateMotor_Q01> PriceBase = new List<SPOutput_GetEstimateMotor_Q01>();
            BillCommon billCommon = new BillCommon();
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_GetEstimateMotor>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (flag)
                {
                    string[] checkList = { apiInput.ProjID, apiInput.SDate, apiInput.EDate };
                    string[] errList = { "ERR900", "ERR900", "ERR900" };
                    //1.判斷必填
                    flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                    //判斷日期
                    if (flag)
                    {
                        if (!string.IsNullOrWhiteSpace(apiInput.SDate) && !string.IsNullOrWhiteSpace(apiInput.EDate))
                        {
                            flag = DateTime.TryParse(apiInput.SDate, out SDate);
                            if (flag)
                            {
                                flag = DateTime.TryParse(apiInput.EDate, out EDate);
                                if (flag)
                                {
                                    if (SDate >= EDate)
                                    {
                                        flag = false;
                                        errCode = "ERR153";
                                    }
                                }
                                else
                                {
                                    errCode = "ERR152";
                                }
                            }
                            else
                            {
                                errCode = "ERR151";
                            }
                        }
                    }
                }
            }
            #endregion
            #region TB
            #region Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            #endregion
            if (flag)
            {
                string spName = "usp_GetEstimateMotor_Q01";
                SPInput_GetEstimateMotor_Q01 spInput = new SPInput_GetEstimateMotor_Q01()
                {
                    ProjID = apiInput.ProjID,
                    CarNo = apiInput.CarNo,
                    CarType = apiInput.CarType,
                    IDNO = IDNO,
                    MonId = apiInput.MonId,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetEstimateMotor_Q01, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetEstimateMotor_Q01, SPOutput_Base>(connetStr);
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref PriceBase, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }

            if (flag)
            {
                if (PriceBase.Count != 0)
                {

                    double DayMaxMinute = 600;
                    List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));

                    /*以車號取得當前優惠標籤*/
                    SPInput_GetDiscountLabelByCarNo spInputDiscountLabel = new SPInput_GetDiscountLabelByCarNo()
                    {
                        CarNo = apiInput.CarNo,
                        LogID = LogID
                    };

                    var reDiscountLabel = new CarRentCommon().GetDiscountLabelByCar(spInputDiscountLabel);

                    if (reDiscountLabel != null)
                    {
                        apiOutput.DiscountLabel = new EstimateDiscountLabel();
                        apiOutput.DiscountLabel.LabelType = reDiscountLabel.LabelType;
                        apiOutput.DiscountLabel.GiveMinute = reDiscountLabel.GiveMinute;
                        apiOutput.DiscountLabel.Price = 0;
                        apiOutput.DiscountLabel.Describe = $"優惠標籤折抵";
                    }

                    var InsurancePrice = billCommon.MotorInsurancePrice(SDate, EDate, PriceBase[0].BaseMinutes, PriceBase[0].BaseMotoRate, PriceBase[0].InsuranceMotoMin, PriceBase[0].InsuranceMotoRate, DayMaxMinute, lstHoliday);

                    int CarRentBill = 0;
                    var RentBill = billCommon.MotoRentMonthComp(SDate, EDate, PriceBase[0].MinuteOfPrice, PriceBase[0].MinuteOfPriceH, PriceBase[0].BaseMinutes, DayMaxMinute, lstHoliday, new List<MonthlyRentData>(), 0, Convert.ToInt32(DayMaxMinute), PriceBase[0].MaxPrice, PriceBase[0].BaseMinutesPrice, PriceBase[0].FirstFreeMins, apiOutput.DiscountLabel?.GiveMinute ?? 0);
                    if (RentBill != null)
                    {
                        CarRentBill = RentBill.RentInPay;
                    }

                    apiOutput = new OAPI_GetEstimateMotor()
                    {
                        CarRentBill = CarRentBill,
                        InsuranceBaseMinute = PriceBase[0].BaseMinutes,
                        InsuranceBaseRate = PriceBase[0].BaseMotoRate,
                        InsuranceMotoMin = PriceBase[0].InsuranceMotoMin,
                        InsuranceMotoRate = PriceBase[0].InsuranceMotoRate,
                        InsuranceBill = apiInput.Insurance == 1 ? InsurancePrice : 0,

                    };
                }
            }
            #endregion
            #region 寫入錯誤Log
            if (!flag)
            {
                baseVerify.InsErrorLog(funName, errCode, 0, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}