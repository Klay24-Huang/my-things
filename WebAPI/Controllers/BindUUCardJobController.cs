using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Output;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Domain.WebAPI.Output.CENS;
using Newtonsoft.Json;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    public class BindUUCardJobController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoBindUUCardJob(Dictionary<string, object> value)
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
            string funName = "BindUUCardJobController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BindUUCardJob apiInput = null;
            OAPI_Base outputApi = new OAPI_Base();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = false;
            int OrderNumber = 0;        //訂單編號
            string IDNO = "";           //身分證字號
            string CID = "";            //車機編號
            string DeviceToken = "";    //遠傳車機token
            int IsCens = 0;             //是否為興聯車機(0:否;1:是)
            string OldCardNo = "";      //舊悠遊卡卡號
            string NewCardNo = "";      //新悠遊卡卡號
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_BindUUCardJob>(Contentjson);
                string ClientIP = baseVerify.GetClientIp(Request);
                //寫入API Log
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                OrderNumber = apiInput.OrderNumber;
                IDNO = apiInput.IDNO;
                CID = apiInput.CID;
                DeviceToken = apiInput.DeviceToken;
                IsCens = apiInput.IsCens;
                OldCardNo = apiInput.OldCardNo;
                NewCardNo = apiInput.NewCardNo;
            }
            #endregion

            #region TB
            if (flag)
            {
                // 有舊卡才解卡
                if (!string.IsNullOrEmpty(OldCardNo))
                {
                    CarCommonFunc CarComm = new CarCommonFunc();
                    if (IsCens == 1)
                    {
                        SendCarNoData[] sendCarNoDatas = new SendCarNoData[1];
                        SendCarNoData obj = new SendCarNoData()
                        {
                            CardNo = OldCardNo,
                            CardType = 1
                        };
                        sendCarNoDatas[0] = obj;
                        flag = CarComm.DoSetCensCustomerCard(CID, sendCarNoDatas, 0, ref errCode);
                    }
                    else
                    {
                        flag = CarComm.DoSetFETCustomerCard(CID, DeviceToken, new string[] { OldCardNo }, 0, LogID, ref errCode);
                    }
                }

                // 有新卡才綁卡
                if (flag && !string.IsNullOrEmpty(NewCardNo) && OldCardNo != NewCardNo)
                {
                    // 開始對車機做動作
                    if (IsCens == 1)
                    {
                        // 興聯車機
                        CensWebAPI webAPI = new CensWebAPI();
                        // 取最新狀況
                        WSOutput_GetInfo wsOutInfo = new WSOutput_GetInfo();
                        flag = webAPI.GetInfo(CID, ref wsOutInfo);
                        if (flag == false)
                        {
                            errCode = wsOutInfo.ErrorCode;
                        }
                        else
                        {
                            if (wsOutInfo.data.CID != CID)
                            {
                                flag = false;
                                errCode = "ERR468";
                            }
                        }
                        // 寫入顧客卡
                        if (flag)
                        {
                            //要將卡號寫入車機
                            SendCarNoData[] CardData = new SendCarNoData[1];
                            //寫入顧客卡
                            WSInput_SendCardNo wsInput = new WSInput_SendCardNo()
                            {
                                CID = CID,
                                mode = 1
                            };

                            CardData[0] = new SendCarNoData();
                            CardData[0].CardNo = NewCardNo;
                            CardData[0].CardType = 1;

                            wsInput.data = new SendCarNoData[0];
                            wsInput.data = CardData;
                            WSOutput_Base wsOut = new WSOutput_Base();
                            Thread.Sleep(500);
                            flag = webAPI.SendCardNo(wsInput, ref wsOut);
                            if (flag == false)
                            {
                                errCode = wsOut.ErrorCode;
                            }
                        }
                    }
                    else
                    {
                        // 遠傳
                        FETCatAPI FetAPI = new FETCatAPI();
                        // 取最新狀況
                        string requestId = "";
                        string CommandType = "";
                        OtherService.Enum.MachineCommandType.CommandType CmdType;
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ReportNow);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.ReportNow;
                        WSInput_Base<Params> input = new WSInput_Base<Params>()
                        {
                            command = true,
                            method = CommandType,
                            requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                            _params = new Params()
                        };
                        requestId = input.requestId;
                        string method = CommandType;
                        flag = FetAPI.DoSendCmd(DeviceToken, CID, CmdType, input, LogID);
                        if (flag)
                        {
                            flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                        }

                        if (flag)
                        {
                            string[] CardStr = new string[1];
                            CardStr[0] = NewCardNo;

                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo;
                            WSInput_Base<ClientCardNoObj> SetCardInput = new WSInput_Base<ClientCardNoObj>()
                            {
                                command = true,
                                method = CommandType,
                                requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                _params = new ClientCardNoObj()
                                {
                                    ClientCardNo = CardStr
                                }
                            };
                            requestId = SetCardInput.requestId;
                            method = CommandType;
                            flag = FetAPI.DoSendCmd(DeviceToken, CID, CmdType, SetCardInput, LogID);
                            if (flag)
                            {
                                flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                            }
                        }
                    }

                    // 不管車機結果如何，都要更新資料
                    string SPName = new ObjType().GetSPName(ObjType.SPType.BindUUCardJob);
                    SPInput_BindUUCardJob SPInput = new SPInput_BindUUCardJob()
                    {
                        IDNO = IDNO,
                        OrderNo = OrderNumber,
                        Result = flag ? 1 : 2,
                        CardNo = NewCardNo,
                        LogID = LogID
                    };
                    SPOutput_Base SPOutput = new SPOutput_Base();
                    SQLHelper<SPInput_BindUUCardJob, SPOutput_Base> sqlBindHelp = new SQLHelper<SPInput_BindUUCardJob, SPOutput_Base>(connetStr);
                    flag = sqlBindHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                    baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
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
