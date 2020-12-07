using Domain.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Rent;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Domain.WebAPI.output.HiEasyRentAPI;
using Domain.WebAPI.Output.CENS;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 一次性開門申請（含重開發簡訊）
    /// </summary>
    public class OpenDoorController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoOpenDoor(Dictionary<string, object> value)
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
            string funName = "OpenDoorController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_OpenDoor apiInput = null;
            NullOutput outputApi = null;
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            string deviceToken = "";    //遠傳車機token
            string CID = "";            //車機編號
            int IsMotor = 0;            //是否為機車（0:否;1:是)
            int IsCens = 0;             //是否為興聯車機(0:否;1:是)
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_OpenDoor>(Contentjson);
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
            //Token判斷
            if (flag)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }

            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.CheckCanOpenDoor);
                SPInput_CheckCanOpenDoor spInput = new SPInput_CheckCanOpenDoor()
                {
                    LogID = LogID,
                    IDNO = IDNO,
                    OrderNo = tmpOrder,
                    Token = Access_Token
                };
                SPOutput_CheckCanOpenDoor spOut = new SPOutput_CheckCanOpenDoor();
                SQLHelper<SPInput_CheckCanOpenDoor, SPOutput_CheckCanOpenDoor> sqlHelp = new SQLHelper<SPInput_CheckCanOpenDoor, SPOutput_CheckCanOpenDoor>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag,  spOut.Error,spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    // 發送簡訊
                    //string VerifyCode = baseVerify.getRand(0, 999999);
                    //HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                    //WebAPIOutput_NPR260Send wsOutput = new WebAPIOutput_NPR260Send();
                    //string Message = string.Format("您的一次性開門驗證碼是：{0}", VerifyCode);
                    //flag = hiEasyRentAPI.NPR260Send(spOut.Mobile, Message, "", ref wsOutput);

                    //if (flag)
                    //{
                    //    spName = new ObjType().GetSPName(ObjType.SPType.InsOpenDoorCode);
                    //    SPInput_InsOpenDoorCode SPInput = new SPInput_InsOpenDoorCode()
                    //    {
                    //        LogID = LogID,
                    //        IDNO = IDNO,
                    //        OrderNo = tmpOrder,
                    //        Token = Access_Token
                    //    };
                    //    SPOutput_Base SPOut = new SPOutput_Base();
                    //    SQLHelper<SPInput_InsOpenDoorCode, SPOutput_Base> sqlInsHelp = new SQLHelper<SPInput_InsOpenDoorCode, SPOutput_Base>(connetStr);
                    //    flag = sqlInsHelp.ExecuteSPNonQuery(spName, SPInput, ref SPOut, ref lstError);
                    //    baseVerify.checkSQLResult(ref flag, SPOut.Error, SPOut.ErrorCode, ref lstError, ref errCode);
                    //    if (flag)
                    //    {
                    //        outputApi = new OAPI_OpenDoor()
                    //        {
                    //            DeadLine = spOut.DeadLine.ToString("yyyy-MM-dd HH:mm:ss"),
                    //            VerifyCode = VerifyCode
                    //        };
                    //    }
                    //}

                    // 車機
                    deviceToken = spOut.deviceToken;
                    CID = spOut.CID;
                    IsMotor = spOut.IsMotor;
                    IsCens = spOut.IsCens;

                    if (IsMotor == 0)   //是否為機車（0:否;1:是)
                    {
                        if (IsCens == 1)    //是否為興聯車機(0:否;1:是)
                        {
                            CensWebAPI censWebAPI = new CensWebAPI();
                            WSInput_SendLock wsInput = new WSInput_SendLock()
                            {
                                CID = CID,
                                CMD = 2
                            };
                            WSOutput_Base wsOutput = new WSOutput_Base();
                            flag = censWebAPI.SendLock(wsInput, ref wsOutput);
                            if (!flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                        }
                        else
                        {
                            FETCatAPI FetAPI = new FETCatAPI();
                            OtherService.Enum.MachineCommandType.CommandType CmdType = new OtherService.Enum.MachineCommandType.CommandType();
                            string CommandType = "";
                            string requestId = "";
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.Unlock);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.Unlock;
                            WSInput_Base<Params> input = new WSInput_Base<Params>()
                            {
                                command = true,
                                method = CommandType,
                                requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                _params = new Params()
                            };
                            string method = CommandType;
                            requestId = input.requestId;
                            flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                            if (flag)
                            {
                                flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                            }
                        }
                    }
                    else
                    {
                        // 機車
                        FETCatAPI FetAPI = new FETCatAPI();
                        OtherService.Enum.MachineCommandType.CommandType CmdType = new OtherService.Enum.MachineCommandType.CommandType();
                        string CommandType = "";
                        string requestId = "";
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.OpenSet);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.OpenSet;
                        WSInput_Base<Params> input = new WSInput_Base<Params>()
                        {
                            command = true,
                            method = CommandType,
                            requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                            _params = new Params()
                        };
                        string method = CommandType;
                        requestId = input.requestId;
                        flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                        if (flag)
                        {
                            flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                        }
                    }
                }
            }
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