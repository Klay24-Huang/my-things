using Domain.CarMachine;
using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Car;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class MotorCmdController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string isDebug = ConfigurationManager.AppSettings["isDebug"].ToString();

        [HttpPost]
        public Dictionary<string, object> DoMotorCmd(Dictionary<string, object> value)
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
            string funName = "MotorCmdController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_MotorCmd apiInput = null;
            NullOutput outputApi = new NullOutput();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            MotorInfo info = new MotorInfo();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";
            string CID = "";
            string deviceToken = "";
            int IsMotor = 0;
            int IsCens = 0;
            DateTime StopTime;
            List<CardList> lstCardList = new List<CardList>();

            //20201203 ADD BY ADAM REASON.增加DEVICEID判斷登入
            string DeviceID = (httpContext.Request.Headers["DeviceID"] == null) ? "" : httpContext.Request.Headers["DeviceID"]; //Bearer 
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MotorCmd>(Contentjson);
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
                    if (flag)
                    {
                        if (apiInput.CmdType < 1 || apiInput.CmdType > 4)
                        {
                            flag = false;
                            errCode = "ERR900";
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
                /*
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {

                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                */
                //20201203 ADD BY ADAM REASON.改為載入DEVICEID判斷
                string CheckTokenName = "usp_CheckTokenDeviceReturnID";
                SPInput_CheckTokenDevice spCheckTokenDevice = new SPInput_CheckTokenDevice()
                {
                    Token = Access_Token,
                    DeviceID = DeviceID,
                    LogID = LogID
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenDevice, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenDevice, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenDevice, ref spOut, ref lstError);

                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }
            #endregion
            //取車機
            if (flag)
            {
                SPInput_CarMachineCommon spInput = new SPInput_CarMachineCommon()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                string SPName = "usp_GetCarMachineInfoCommon_Q01";
                SPOutput_CarMachineCommon spOut = new SPOutput_CarMachineCommon();
                SQLHelper<SPInput_CarMachineCommon, SPOutput_CarMachineCommon> sqlHelp = new SQLHelper<SPInput_CarMachineCommon, SPOutput_CarMachineCommon>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    if (spOut.car_mgt_status >= 4 && spOut.car_mgt_status < 16 && spOut.cancel_status == 0)  //未完成訂單（包含未取消）
                    {
                        FETCatAPI FetAPI = new FETCatAPI();
                        OtherService.Enum.MachineCommandType.CommandType CmdType = new OtherService.Enum.MachineCommandType.CommandType();
                        string CommandType = "";
                        string requestId = "";
                        WSInput_Base<Params> input;
                        string method = CommandType;
                        #region 先捉最近一次資料
                        //20201127 小BENSON建議先把指令前的REPORT NOW取消掉測試看看
                        /*
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
                        string method = CommandType;
                        flag = FetAPI.DoSendCmd(spOut.deviceToken, spOut.CID, CmdType, input, LogID);

                        // 20201030 ADD BY ADAM REASON.先取消ReportNow等待，直接寫入記錄到TB_CarStatus 
                        if (flag)
                        {
                            flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                        }
                        */
                        if (flag)
                        {
                            info = new CarStatusCommon(connetStr).GetInfoByMotor(spOut.CID);
                            if (info == null)
                            {
                                flag = false;
                                errCode = "ERR216";
                            }
                            else
                            {
                                CID = spOut.CID;
                                deviceToken = spOut.deviceToken;
                            }
                        }
                        #endregion
                        if (flag)
                        {
                            switch (apiInput.CmdType)
                            {
                                case 1: //2:開啟電源
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOn);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOn;
                                    break;
                                case 2: //3:關閉電源
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOff);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOff;
                                    break;
                                case 3: //6:開啟坐墊
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.OpenSet);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.OpenSet;
                                    break;
                                case 4: //7:開啟/關閉電池蓋
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetBatteryCap);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.SetBatteryCap;
                                    break;
                            }

                            #region 20210514 開啟電源後須紀錄電量
                            //20210605 ADD BY ADAM REASON.改為先記錄再跑車機指令，讓換電的紀錄可以完整的紀錄起來
                            if (flag && (apiInput.CmdType == 2 || apiInput.CmdType == 4))
                            {
                                string EventCD = "";
                                switch (apiInput.CmdType)
                                {
                                    case 2:
                                        EventCD = "2";
                                        break;
                                    case 4:
                                        EventCD = "3";  //電池蓋先押3，前後判斷在SP裡面處理
                                        break;
                                }
                                string SPInsMotorBattLogName = "usp_InsMotorBattLog";
                                SPInput_InsMotorBattLog SPInsMotorBattLogInput = new SPInput_InsMotorBattLog()
                                {
                                    OrderNo = tmpOrder,
                                    EventCD = EventCD,  //取車電量
                                    LogID = LogID
                                };
                                SPOutput_Base SPInsMotorBattLogOutput = new SPOutput_Base();
                                SQLHelper<SPInput_InsMotorBattLog, SPOutput_Base> SQLInsMotorBattLogHelp = new SQLHelper<SPInput_InsMotorBattLog, SPOutput_Base>(connetStr);
                                flag = SQLInsMotorBattLogHelp.ExecuteSPNonQuery(SPInsMotorBattLogName, SPInsMotorBattLogInput, ref SPInsMotorBattLogOutput, ref lstError);
                                baseVerify.checkSQLResult(ref flag, ref SPInsMotorBattLogOutput, ref lstError, ref errCode);
                            }
                            #endregion

                            if (flag)
                            {
                                if (isDebug == "0") // isDebug = 1，不送車機指令
                                {
                                    input = new WSInput_Base<Params>()
                                    {
                                        command = true,
                                        method = CommandType,
                                        requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                        _params = new Params()
                                    };
                                    method = CommandType;
                                    requestId = input.requestId;
                                    flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                                    //20201020 MARK BY JERRY 無連續動作，可以先不執行等待
                                    //20201203 ADD BY ADAM REASON.今天開會決議APP要等待指令結果
                                    if (flag)
                                    {
                                        flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                                    }

                                    // 20201029 ADD BY ADAM REASON. 儲存指令結果，後續還是看ReportNow
                                    // 20201127 小BENSON建議先把指令前的REPORT NOW取消掉測試看看
                                    // 這邊就先不跑強更狀態
                                    if (flag && false)
                                    {
                                        SPInput_SetMotorStatus spInput2 = new SPInput_SetMotorStatus()
                                        {
                                            CID = CID,
                                            CmdType = CommandType,
                                            LogID = LogID
                                        };
                                        string SPName2 = "usp_SetMotorStatus";
                                        SPOutput_Base spOutBase = new SPOutput_Base();
                                        SQLHelper<SPInput_SetMotorStatus, SPOutput_Base> sqlHelp2 = new SQLHelper<SPInput_SetMotorStatus, SPOutput_Base>(connetStr);
                                        flag = sqlHelp2.ExecuteSPNonQuery(SPName2, spInput2, ref spOutBase, ref lstError);
                                        baseVerify.checkSQLResult(ref flag, spOutBase.Error, spOutBase.ErrorCode, ref lstError, ref errCode);
                                    }
                                }
                            }
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