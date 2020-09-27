using Domain.CarMachine;
using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Common;
using Domain.SP.Output;
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
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
    /// 汽車上解鎖
    /// </summary>
    public class SendLockController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string isDebug = (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["isDebug"])) ? "0" : ConfigurationManager.AppSettings["isDebug"].ToString();
        private string CENSCID = (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["MockCID"])) ? "90001" : ConfigurationManager.AppSettings["MockCID"].ToString();
        [HttpPost]
        public Dictionary<string, object> DoSendLock(Dictionary<string, object> value)
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
            string funName = "SendLockController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_SendLock apiInput = null;
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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_SendLock>(Contentjson);
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
                        apiInput.Lock = (apiInput.Lock < 0) ? 0: apiInput.Lock;
                        apiInput.Lock = (apiInput.Lock >1) ? 1: apiInput.Lock;
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

                    if (spOut.car_mgt_status>4 && spOut.car_mgt_status<16 && spOut.cancel_status==0)  //未完成訂單（包含未取消）
                    {
                        if (spOut.IsCens == 1)
                        {
                            if (isDebug == "1")
                            {
                                CID = CENSCID;
                            }
                            #region 興聯車機
                            CensWebAPI censWebAPI = new CensWebAPI();
                            WSOutput_GetInfo output = new WSOutput_GetInfo();
                            flag = censWebAPI.GetInfo(CID, ref output);
                            if (flag)
                            {
                                if (output.data.doorStatus != "1111")
                                {
                                    flag = false;
                                    errCode = "ERR429"; //車門未關
                                }
                                else
                                {
                                    if (output.data.CentralLock == 1 && apiInput.Lock==1)
                                    {
                                        flag = false;
                                        errCode = "ERR427"; //已經是解鎖狀態
                                 
                                    }
                                    else if (output.data.CentralLock == 0 && apiInput.Lock == 0)
                                    {
                                        flag = false;
                                        errCode = "ERR428"; //已經是上鎖狀態
                                  
                                    }
                                }
                                if (flag)//執行上解鎖
                                {
                                    int lockAction = (apiInput.Lock == 0) ? 3 : 2;
                                    WSInput_SendLock wsInput = new WSInput_SendLock()
                                    {
                                        CID = CID,
                                        CMD = lockAction
                                    };
                                    WSOutput_Base wsOutput = new WSOutput_Base();
                                    flag = censWebAPI.SendLock(wsInput, ref wsOutput);
                                    if (!flag || wsOutput.Result == 1)
                                    {
                                        errCode = wsOutput.ErrorCode;
                                        errMsg = wsOutput.ErrMsg;
                                    }
                                 
                                }
                            }
                            else
                            {
                                errCode = output.ErrorCode;
                                errMsg = output.ErrMsg;
                            }
                            #endregion
                        }
                        else
                        {
                            FETCatAPI FetAPI = new FETCatAPI();
                            string requestId = "";
                            string CommandType = "";
                            OtherService.Enum.MachineCommandType.CommandType CmdType;
                            if (spOut.IsMotor == 0)
                            {
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SearchVehicle);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.SearchVehicle;
                            }
                            else
                            {
                                if (DateTime.Now.Hour >= 7 && DateTime.Now.Hour < 22)//白天
                                {
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetHornOn);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.SetHornOn;
                                }
                                else
                                {
                                    //晚上不要吵人
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetLightFlash);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.SetLightFlash;
                                }
                            }
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
