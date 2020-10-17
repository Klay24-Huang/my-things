using Domain.CarMachine;
using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Common;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Domain.WebAPI.Output.CENS;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class MotorCmdController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
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
                        if(apiInput.CmdType<1 || apiInput.CmdType > 4)
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
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetCarMachineInfoCommon);
                SPOutput_CarMachineCommon spOut = new SPOutput_CarMachineCommon();
                SQLHelper<SPInput_CarMachineCommon, SPOutput_CarMachineCommon> sqlHelp = new SQLHelper<SPInput_CarMachineCommon, SPOutput_CarMachineCommon>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {

                    if (spOut.car_mgt_status > 4 && spOut.car_mgt_status < 16 && spOut.cancel_status == 0)  //未完成訂單（包含未取消）
                    {

                        FETCatAPI FetAPI = new FETCatAPI();
                        OtherService.Enum.MachineCommandType.CommandType CmdType = new OtherService.Enum.MachineCommandType.CommandType();
                        string CommandType = "";
                        string requestId = "";
                        #region 先捉最近一次資料
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
                        if (flag)
                        {
                            flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                        }
                        if (flag)
                        {
                            info = new CarStatusCommon(connetStr).GetInfoByMotor(CID);
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
                                    if (info.ACCStatus == 1)
                                    {
                                        flag = false;
                                        errCode = "ERR217";

                                    }
                                    else
                                    {
                                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOn);
                                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOn;
                                    }
                                  
                                    break;
                                case 2: //3:關閉電源
                                    if (info.ACCStatus == 0)
                                    {
                                        flag = false;
                                        errCode = "ERR218";
                                    }
                                    else
                                    {
                                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOff);
                                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOff;
                                    }
                                    
                                    break;

                                case 3: //6:開啟坐墊
                                    if (info.devicePut_Down == 1)
                                    {
                                        flag = false;
                                        errCode = "ERR219";
                                    }
                                    else
                                    {
                                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.OpenSet);
                                        CmdType = OtherService.Enum.MachineCommandType.CommandType.OpenSet;
                                    }
                                   
                                    break;
                                case 4: //7:開啟/關閉電池蓋
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetBatteryCap);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.SetBatteryCap;
                                    break;
                            }
                            if (flag)
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
                                if (flag)
                                {
                                    flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
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
