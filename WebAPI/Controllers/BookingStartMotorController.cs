using Domain.CarMachine;
using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Wallet;
using Domain.TB;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 機車取車
    /// </summary>
    public class BookingStartMotorController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string isDebug = ConfigurationManager.AppSettings["isDebug"].ToString();

        [HttpPost]
        public Dictionary<string, object> DoBookingStartMotor(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BookingStartMotorController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BookingStartMotor apiInput = null;
            OAPI_BookingStartMotor outputApi = new OAPI_BookingStartMotor();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            MotorInfo info = new MotorInfo();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            string CID = "";
            string deviceToken = "";
            int IsMotor = 0;
            int IsCens = 0;
            double mil = 0;
            List<CardList> lstCardList = new List<CardList>();
            bool CreditFlag = true;     // 信用卡綁卡
            bool WalletFlag = false;    // 綁定錢包
            int WalletAmout = 0;        // 錢包餘額
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BookingStartMotor>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.OrderNo))
                {
                    flag = false;
                    errCode = "ERR900";
                }
                else
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
            #region Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            #endregion
            #region 檢查信用卡是否綁卡、錢包是否開通
            if (flag)
            {
                #region 檢查信用卡是否綁卡
                DataSet ds = Common.getBindingList(IDNO, ref flag, ref errCode, ref errMsg);
                if (ds.Tables.Count == 0)
                {
                    CreditFlag = false;
                }
                else if (ds.Tables[0].Rows.Count == 0)
                {
                    CreditFlag = false;
                }
                ds.Dispose();
                #endregion

                #region 檢查錢包是否開通
                string SPName = "usp_CreditAndWalletQuery_Q01";
                SPInput_CreditAndWalletQuery spInput = new SPInput_CreditAndWalletQuery
                {
                    IDNO = IDNO,
                    Token = Access_Token,
                    LogID = LogID
                };
                SPOut_CreditAndWalletQuery spOut = new SPOut_CreditAndWalletQuery();
                SQLHelper<SPInput_CreditAndWalletQuery, SPOut_CreditAndWalletQuery> sqlHelp = new SQLHelper<SPInput_CreditAndWalletQuery, SPOut_CreditAndWalletQuery>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    WalletFlag = spOut.WalletStatus == "2" ? true : false;
                    WalletAmout = spOut.WalletAmout;
                }
                #endregion

                if (!CreditFlag && !WalletFlag) // 沒綁信用卡 也 沒開通錢包，就回錯誤訊息
                {
                    flag = false;
                    errCode = "ERR292";
                }
                #region 20220215 UPD BY AMBER REASON.因預約已預扣錢包50元，取消取車錢包餘額驗證
                //else if (!CreditFlag && WalletFlag) // 沒綁信用卡 但 有開通錢包
                //{
                //    if (WalletAmout < 50)   // 錢包餘額 < 50元 不給取車
                //    {
                //        flag = false;
                //        errCode = "ERR291";
                //    }
                //}
                #endregion
            }
            #endregion
            #region 檢查欠費 20220105路邊欠費查詢取消
            //if (flag)
            //{
            //    int TAMT = 0;
            //    WebAPI.Models.ComboFunc.ContactComm contract = new Models.ComboFunc.ContactComm();
            //    flag = contract.CheckNPR330(IDNO, LogID, ref TAMT);
            //    if (TAMT > 0)
            //    {
            //        flag = false;
            //        errCode = "ERR234";
            //    }
            //}
            #endregion
            #region 取車
            if (flag)
            {
                string CheckTokenName = "usp_BeforeBookingStart";
                SPInput_BeforeBookingStart spBeforeStart = new SPInput_BeforeBookingStart()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token,
                    PhoneLon = apiInput.PhoneLon,
                    PhoneLat = apiInput.PhoneLat
                };
                SPOutput_BeforeBookingStart spOut = new SPOutput_BeforeBookingStart();
                SQLHelper<SPInput_BeforeBookingStart, SPOutput_BeforeBookingStart> sqlHelp = new SQLHelper<SPInput_BeforeBookingStart, SPOutput_BeforeBookingStart>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spBeforeStart, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    CID = spOut.CID;
                    deviceToken = spOut.deviceToken;
                    IsCens = spOut.IsCens;
                    IsMotor = spOut.IsMotor;
                    List<ErrorInfo> lstCarError = new List<ErrorInfo>();
                    lstCardList = new CarCardCommonRepository(connetStr).GetCardListByCustom(CID.ToUpper(), ref lstCarError);
                }
                //開始對車機做動作
                if (flag)
                {
                    FETCatAPI FetAPI = new FETCatAPI();
                    string requestId = "";
                    string CommandType = "";
                    OtherService.Enum.MachineCommandType.CommandType CmdType;
                    string method = "";

                    #region ReportNow
                    if (isDebug == "0") // isDebug = 1，不送車機指令
                    {
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ReportNow);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.ReportNow;
                        WSInput_Base<Params> input = new WSInput_Base<Params>()
                        {
                            command = true,
                            method = CommandType,
                            requestId = string.Format("{0}_{1}", spOut.CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                            _params = new Params()
                        };
                        requestId = input.requestId;
                        method = CommandType;
                        //20210325 ADD BY ADAM REASON.車機指令優化取消REPORT NOW
                        //flag = FetAPI.DoSendCmd(spOut.deviceToken, spOut.CID, CmdType, input, LogID);
                        //if (flag)
                        //{
                        //    flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                        //}
                        if (flag)
                        {
                            info = new CarStatusCommon(connetStr).GetInfoByMotor(CID);
                            if (info != null)
                            {
                                mil = info.Millage;
                            }
                        }
                    }
                    #endregion
                    #region 執行sp合約
                    if (flag)
                    {
                        //20211012 ADD BY ADAM REASON.增加手機定位點
                        string BookingStartName = "usp_BookingStart";
                        Domain.SP.Input.Rent.SPInput_BookingStart SPBookingStartInput = new Domain.SP.Input.Rent.SPInput_BookingStart()
                        {
                            IDNO = IDNO,
                            LogID = LogID,
                            OrderNo = tmpOrder,
                            Token = Access_Token,
                            NowMileage = Convert.ToSingle(mil),
                            StopTime = "",
                            Insurance = 0
                            //20211012 ADD BY ADAM REASON.增加手機定位點
                            //PhoneLat = apiInput.PhoneLat,
                            //PhoneLon = apiInput.PhoneLon
                        };
                        SPOutput_Base SPBookingStartOutput = new SPOutput_Base();
                        SQLHelper<Domain.SP.Input.Rent.SPInput_BookingStart, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<Domain.SP.Input.Rent.SPInput_BookingStart, SPOutput_Base>(connetStr);
                        flag = SQLBookingStartHelp.ExecuteSPNonQuery(BookingStartName, SPBookingStartInput, ref SPBookingStartOutput, ref lstError);
                        baseVerify.checkSQLResult(ref flag, ref SPBookingStartOutput, ref lstError, ref errCode);
                    }
                    if (flag)
                    {
                        string BookingControlName = "usp_BookingControl";
                        SPInput_BookingControl SPBookingControlInput = new SPInput_BookingControl()
                        {
                            IDNO = IDNO,
                            OrderNo = tmpOrder,
                            Token = Access_Token,
                            LogID = LogID
                        };
                        SPOutput_Base SPBookingControlOutput = new SPOutput_Base();
                        SQLHelper<SPInput_BookingControl, SPOutput_Base> SQLBookingControlHelp = new SQLHelper<SPInput_BookingControl, SPOutput_Base>(connetStr);
                        flag = SQLBookingControlHelp.ExecuteSPNonQuery(BookingControlName, SPBookingControlInput, ref SPBookingControlOutput, ref lstError);
                        baseVerify.checkSQLResult(ref flag, ref SPBookingControlOutput, ref lstError, ref errCode);
                    }
                    #endregion
                    //20210325 ADD BY ADAM REASON.車機指令優化
                    if (isDebug == "0") // isDebug = 1，不送車機指令
                    {
                        #region 開啟電源
                        if (flag)
                        {
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOn);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOn;
                            WSInput_Base<Params> PowerOnInput = new WSInput_Base<Params>()
                            {
                                command = true,
                                method = CommandType,
                                requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                _params = new Params()
                            };
                            method = CommandType;
                            requestId = PowerOnInput.requestId;
                            flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, PowerOnInput, LogID);
                            if (flag)
                            {
                                flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                            }
                        }
                        #endregion
                        #region 設定租約
                        if (flag)
                        {
                            //租約再下租約應該沒關係
                            //if (info.extDeviceStatus1 == 0)
                            {
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetMotorcycleRent);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.SetMotorcycleRent;
                                WSInput_Base<BLECode> RentInput = new WSInput_Base<BLECode>()
                                {
                                    command = true,
                                    method = CommandType,
                                    requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                    _params = new BLECode()
                                };
                                RentInput._params.BLE_Code = IDNO.Substring(0, 9);
                                requestId = RentInput.requestId;
                                method = CommandType;
                                flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, RentInput, LogID);
                                if (flag)
                                {
                                    flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                                }

                                if (flag)
                                {
                                    Thread.Sleep(1000);
                                    //查ble
                                    BLEInfo ble = new CarCMDRepository(connetStr).GetBLEInfo(CID);
                                    if (ble != null)
                                    {
                                        outputApi.BLEDEVICEID = ble.BLE_Device;
                                        outputApi.BLEDEVICEPWD = ble.BLE_PWD;
                                    }

                                }
                            }
                        }
                        #endregion
                    }
                    #region 20210514 開啟電源後須紀錄電量 20210521 改為設定完租約再記錄電量
                    if (flag)
                    {
                        string SPInsMotorBattLogName = "usp_InsMotorBattLog";
                        SPInput_InsMotorBattLog SPInsMotorBattLogInput = new SPInput_InsMotorBattLog()
                        {
                            OrderNo = tmpOrder,
                            EventCD = "1",  //取車電量
                            LogID = LogID
                        };
                        SPOutput_Base SPInsMotorBattLogOutput = new SPOutput_Base();
                        SQLHelper<SPInput_InsMotorBattLog, SPOutput_Base> SQLInsMotorBattLogHelp = new SQLHelper<SPInput_InsMotorBattLog, SPOutput_Base>(connetStr);
                        flag = SQLInsMotorBattLogHelp.ExecuteSPNonQuery(SPInsMotorBattLogName, SPInsMotorBattLogInput, ref SPInsMotorBattLogOutput, ref lstError);
                        baseVerify.checkSQLResult(ref flag, ref SPInsMotorBattLogOutput, ref lstError, ref errCode);
                    }
                    #endregion
                }
            }
            #endregion
            #region 寫取車照片到azure
            if (flag)
            {
                OtherRepository otherRepository = new OtherRepository(connetStr);
                List<CarPIC> lstCarPIC = otherRepository.GetCarPIC(tmpOrder, 0);
                int PICLen = lstCarPIC.Count;
                for (int i = 0; i < PICLen; i++)
                {
                    try
                    {
                        string FileName = string.Format("{0}_{1}_{2}.png", apiInput.OrderNo, (lstCarPIC[i].ImageType == 5) ? "Sign" : "PIC" + lstCarPIC[i].ImageType.ToString(), DateTime.Now.ToString("yyyyMMddHHmmss"));

                        flag = new AzureStorageHandle().UploadFileToAzureStorage(lstCarPIC[i].Image, FileName, "carpic");
                        if (flag)
                        {
                            bool DelFlag = otherRepository.HandleTempCarPIC(tmpOrder, 0, lstCarPIC[i].ImageType, FileName); //更新為azure的檔名
                        }
                    }
                    catch (Exception ex)
                    {
                        flag = true; //先bypass，之後補傳再刪
                    }
                }
            }
            #endregion
            #endregion
            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
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