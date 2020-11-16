using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.BackEnd.Output;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 後台點數查詢
    /// </summary>
    public class BE_BonusQueryController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoBE_BonusQuery(Dictionary<string, object> value)
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
            string funName = "BE_BonusQueryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_BonusQuery apiInput = null;
            //List<OAPI_BonusQuery> outputApi = new List<OAPI_BonusQuery>();
            OAPI_BE_BonusQuery outputApi = new OAPI_BE_BonusQuery();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_BonusQuery>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.IDNO))
                {
                    flag = false;
                    errCode = "ERR900";
                }
                if (flag)
                {
                    //2.判斷格式
                    flag = baseVerify.checkIDNO(apiInput.IDNO);
                    if (false == flag)
                    {
                        errCode = "ERR103";
                    }
                }

            }

            #endregion
            #region TB


            //開始送短租查詢
            if (flag)
            {
                WebAPIOutput_NPR270Query wsOutput = new WebAPIOutput_NPR270Query();
                HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                flag = wsAPI.NPR270Query(apiInput.IDNO, ref wsOutput);


                if (flag)
                {
                    int giftLen = wsOutput.Data.Length;

                    if (giftLen > 0)
                    {
                        //OAPI_BonusQuery objBonus = new OAPI_BonusQuery();
                        //objBonus.BonusObj = new List<BonusData>();
                        int TotalGiftPoint = 0;
                        int TotalLastPoint = 0;
                        int TotalGiftPointCar = 0;
                        int TotalGiftPointMotor = 0;
                        int TotalLastPointCar = 0;
                        int TotalLastPointMotor = 0;
                        int TotalLastTransPointCar = 0;
                        int TotalLastTransPointMotor = 0;

                        for (int i = 0; i < giftLen; i++)
                        {
                            DateTime tmpDate;
                            int tmpPoint = 0;
                            bool DateFlag = DateTime.TryParse(wsOutput.Data[i].EDATE, out tmpDate);
                            bool PointFlag = int.TryParse(wsOutput.Data[i].GIFTPOINT, out tmpPoint);
                            if (DateFlag && (tmpDate >= DateTime.Now) && PointFlag)
                            {
                                //  totalPoint += tmpPoint;

                                BonusData objPoint = new BonusData()
                                {
                                    //20201021 ADD BY ADAM REASON.補上流水號
                                    SEQNO = wsOutput.Data[i].SEQNO,

                                    PointType = (wsOutput.Data[i].GIFTTYPE == "01") ? 0 : 1,
                                    EDATE = (wsOutput.Data[i].EDATE == "") ? "" : (wsOutput.Data[i].EDATE.Split(' ')[0]).Replace("/", "-"),
                                    GIFTNAME = wsOutput.Data[i].GIFTNAME,
                                    GIFTPOINT = string.IsNullOrEmpty(wsOutput.Data[i].GIFTPOINT) ? "0" : wsOutput.Data[i].GIFTPOINT,
                                    LASTPOINT = string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? "0" : wsOutput.Data[i].LASTPOINT,
                                    AllowSend = string.IsNullOrEmpty(wsOutput.Data[i].RCVFLG) ? 0 : ((wsOutput.Data[i].RCVFLG == "Y") ? 1 : 0)

                                };
                                if (objPoint.PointType == 0)
                                {
                                    if (!objPoint.GIFTNAME.Contains("【汽車】"))
                                    {
                                        objPoint.GIFTNAME = "【汽車】\n" + objPoint.GIFTNAME;
                                    }
                                    TotalGiftPointCar += int.Parse(objPoint.GIFTPOINT);
                                    TotalLastPointCar += int.Parse(objPoint.LASTPOINT);
                                    //20201018 ADD BY ADAM REASON.增加可轉贈的剩餘點數
                                    TotalLastTransPointCar += (objPoint.AllowSend == 1 ? int.Parse(objPoint.LASTPOINT) : 0);
                                }
                                else if (objPoint.PointType == 1)
                                {
                                    if (!objPoint.GIFTNAME.Contains("【機車】"))
                                    {
                                        objPoint.GIFTNAME = "【機車】\n" + objPoint.GIFTNAME;
                                    }
                                    TotalGiftPointMotor += int.Parse(objPoint.GIFTPOINT);
                                    TotalLastPointMotor += int.Parse(objPoint.LASTPOINT);
                                    //20201018 ADD BY ADAM REASON.增加可轉贈的剩餘點數
                                    TotalLastTransPointMotor += (objPoint.AllowSend == 1 ? int.Parse(objPoint.LASTPOINT) : 0);
                                }
                        

                                //點數加總
                                TotalGiftPoint += int.Parse(objPoint.GIFTPOINT);
                                TotalLastPoint += int.Parse(objPoint.LASTPOINT);


                            }

                        }
                        outputApi.TotalLASTPOINT = TotalLastPoint;
                        outputApi.TotalCarLASTPOINT = TotalLastPointCar;
                        outputApi.TotalMotorLASTPOINT = TotalLastPointMotor;
                    }
                }
                else
                {
                    errCode = "ERR";
                    errMsg = wsOutput.Message;
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
