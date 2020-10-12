using Domain.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Rent;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Domain.WebAPI.Output.CENS;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 一次性開門結束
    /// </summary>
    public class OpenDoorFinishController : ApiController
    {

        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoOpenDoorFinish(Dictionary<string, object> value)
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
            string funName = "OpenDoorFinishController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_OpenDoorFinish apiInput = null;
            NullOutput outputApi = new NullOutput();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();


            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";
            string CID = "";
            string deviceToken = "";
            int IsMotor = 0;
            int IsCens = 0;
            double mil = 0;


            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_OpenDoorFinish>(Contentjson);
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
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.GetCarStatusBeforeOpenDoorFinish);
                SPInput_GetCarStatusBeforeOpenDoorFinish spInput = new SPInput_GetCarStatusBeforeOpenDoorFinish()
                {
                    LogID = LogID,
                    IDNO = IDNO,
                    OrderNo = tmpOrder,
                    Token = Access_Token
                };
                SPOutput_GetCarStatusBeforeOpenDoorFinish spOut = new SPOutput_GetCarStatusBeforeOpenDoorFinish();
                SQLHelper<SPInput_GetCarStatusBeforeOpenDoorFinish, SPOutput_GetCarStatusBeforeOpenDoorFinish> sqlHelp = new SQLHelper<SPInput_GetCarStatusBeforeOpenDoorFinish, SPOutput_GetCarStatusBeforeOpenDoorFinish>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    deviceToken = spOut.deviceToken;
                    CID = spOut.CID;
                    IsMotor = spOut.IsMotor;
                    IsCens = spOut.IsCens;
                    #region 車機
                    if (IsMotor == 0)
                    {
                        if (IsCens == 1)
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
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.Lock);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.Lock;
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
                    #endregion
                }
                if (flag)
                {
                    spName = new ObjType().GetSPName(ObjType.SPType.FinishOpenDoor);
                    SPInput_OpenDoorFinish FinishInput = new SPInput_OpenDoorFinish()
                    {
                        LogID = LogID,
                        IDNO = IDNO,
                        OrderNo = tmpOrder,
                        Token = Access_Token
                    };
                    SPOutput_Base FinishOutput = new SPOutput_Base();
                 
                    SQLHelper<SPInput_OpenDoorFinish, SPOutput_Base> FinishHelp = new SQLHelper<SPInput_OpenDoorFinish, SPOutput_Base>(connetStr);
                    flag = FinishHelp.ExecuteSPNonQuery(spName, FinishInput, ref FinishOutput, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref FinishOutput, ref lstError, ref errCode);
                }                
            }
            
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
