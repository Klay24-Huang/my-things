using Domain.Common;
using Domain.TB;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Domain.WebAPI.Output.CENS;
using Newtonsoft.Json;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 對車機下cmd web
    /// </summary>
    public class SendCarCMDController : ApiController
    {
        private static string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string isDebug = ConfigurationManager.AppSettings["isDebug"].ToString();

        [HttpPost]
        public Dictionary<string, object> DoSendCmd(Dictionary<string, object> value)
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
            string funName = "SendCarCMDController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            string CID = "4BF5";
            string CENSCID = "90001";
            CommonFunc baseVerify = new CommonFunc();
            Token token = null;
            string deviceToken = "";
            IAPI_SendCarCMD apiInput = null;
            OAPI_Base outputApi = null;
            int CarMachineType = 0;

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string method = "", requestId = "";
            string CarHornFlg = "";
            CarCommonFunc carCommon = new CarCommonFunc();
            OtherService.Enum.MachineCommandType.CommandType CmdType = new OtherService.Enum.MachineCommandType.CommandType();
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_SendCarCMD>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.CID))
                {
                    flag = false;
                    errCode = "ERR900";
                }
                else
                {
                    CID = apiInput.CID;
                    if (apiInput.CID.Length == 5)
                    {
                        CarMachineType = 1;
                        CENSCID = CID;
                    }
                }
                if (flag)
                {
                    if (CarMachineType != apiInput.IsCens)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }
                if (flag)
                {
                    if (CarMachineType == 0)
                    {

                        if (string.IsNullOrWhiteSpace(apiInput.deviceToken))
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                        else
                        {
                            deviceToken = apiInput.deviceToken;
                        }
                    }
                }
            }
            #endregion
            #region 第二段防呆
            if (flag)
            {
                if ((CarMachineType == 0 && apiInput.CmdType > 14 && (apiInput.CmdType < 51 || apiInput.CmdType > 60) && apiInput.CmdType != 99) || (CarMachineType == 1 && (apiInput.CmdType < 15 || (apiInput.CmdType > 50 && apiInput.CmdType < 61))))
                {
                    flag = false;
                    errCode = "ERR900";
                }
            }
            #endregion
            #region 判斷車機類型
            if (flag)
            {
                if (isDebug == "0" || CID =="70001") // isDebug = 1，不送車機指令
                {
                    if (CarMachineType == 0)
                    {
                        FETCatAPI FetAPI = new FETCatAPI();
                        string CommandType = "";
                        /// <para>0:尋車</para>
                        /// <para>1:查詢萬用卡號</para>
                        /// <para>2:設定萬用卡號</para>
                        /// <para>3:中控解鎖，防盜關閉</para>
                        /// <para>4:中控上鎖，防盜啟動</para>
                        /// <para>5:汽車設為有租約狀態</para>
                        /// <para>6:設定無租約狀態</para>
                        /// <para>7:中控解鎖</para>
                        /// <para>8:中控上鎖</para>
                        /// <para>9:防盜關閉</para>
                        /// <para>10:防盜啟動</para>
                        /// <para>11:查詢顧客卡號</para>
                        /// <para>12:設定顧客卡號</para>
                        /// <para>13:清除全部顧客卡號</para>
                        /// <para>14:清除全部萬用卡號</para>
                        /// <para>51:啟動喇叭搜尋汽車</para>
                        /// <para>52:啟動閃燈搜尋汽車</para>
                        /// <para>53:汽車租車組合指令</para>
                        /// <para>54:汽車還車組合指令</para>
                        switch (apiInput.CmdType)
                        {
                            case 0:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SearchVehicle);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.SearchVehicle;
                                break;
                            case 1:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.QueryUnivCardNo);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.QueryUnivCardNo;
                                break;
                            case 2:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetUnivCardNo);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.SetUnivCardNo;
                                break;
                            case 3:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.Unlock_AlertOff);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.Unlock_AlertOff;
                                break;
                            case 4:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.Lock_AlertOn);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.Lock_AlertOn;
                                break;
                            case 5:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetVehicleRent);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.SetVehicleRent;
                                break;
                            case 6:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetNoRent);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.SetNoRent;
                                break;

                            case 7:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.Unlock);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.Unlock;
                                break;
                            case 8:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.Lock);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.Lock;
                                break;
                            case 9:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.AlertOff);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.AlertOff;
                                break;
                            case 10:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.AlertOn);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.AlertOn;
                                break;
                            case 11:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.QueryClientCardNo);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.QueryClientCardNo;
                                break;

                            case 12:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo;
                                break;
                            case 13:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ClearAllClientCard);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.ClearAllClientCard;
                                break;
                            case 14:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ClearAllUnivCard);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.ClearAllUnivCard;
                                break;
                            case 51:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SearchVehicleHornOn);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.SearchVehicleHornOn;
                                break;
                            case 52:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SearchVehicleLightFlash);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.SearchVehicleLightFlash;
                                break;
                            case 53:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.VehicleRentCombo);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.VehicleRentCombo;
                                break;
                            case 54:
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.VehicleNoRentCombo);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.VehicleNoRentCombo;
                                break;
                            case 99: //99:ReportNow
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ReportNow);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.ReportNow;
                                break;
                        }
                        //針對尋車響喇叭做特別處理
                        if (apiInput.CmdType == 0 || apiInput.CmdType == 51)
                        {
                            if (!carCommon.BE_CheckCarHornFlgByCID(apiInput.CID, LogID, apiInput.UserId, ref errCode))
                            {
                                //晚上不要吵人
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SearchVehicleLightFlash);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.SearchVehicleLightFlash;
                            }
                            
                        }

                        if (apiInput.CmdType == 2)
                        {
                            WSInput_Base<UnivCardNoObj> input = new WSInput_Base<UnivCardNoObj>()
                            {
                                command = true,
                                method = CommandType,
                                requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                _params = new UnivCardNoObj()

                            };
                            input._params.UnivCardNo = apiInput.UnivCard;
                            requestId = input.requestId;
                            flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                        }
                        else if (apiInput.CmdType == 12 || apiInput.CmdType == 53)
                        {
                            WSInput_Base<ClientCardNoObj> input = new WSInput_Base<ClientCardNoObj>()
                            {
                                command = true,
                                method = CommandType,
                                requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                _params = new ClientCardNoObj()

                            };
                            input._params.ClientCardNo = apiInput.ClientCardNo;
                            //組合指令顧客卡必輸入，若沒有則帶隨機值
                            if (apiInput.CmdType == 53 && input._params.ClientCardNo.Length == 0)
                            {
                                input._params.ClientCardNo = new string[] { (new Random()).Next(10000000, 99999999).ToString().PadLeft(10, 'X') };
                            }
                            requestId = input.requestId;
                            flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                        }
                        else
                        {
                            WSInput_Base<Params> input = new WSInput_Base<Params>()
                            {
                                command = true,
                                method = CommandType,
                                requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                _params = new Params()

                            };
                            requestId = input.requestId;
                            flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                        }
                        method = CommandType;
                    }
                    else
                    {
                        ///  <para>15:興聯-尋車</para>
                        ///  <para>16:興聯-汽車設為有租約狀態</para>
                        ///  <para>17:興聯-設定無租約狀態</para>
                        ///  <para>18:興聯-全車解鎖</para>
                        ///  <para>19:興聯-全車上鎖</para>
                        ///  <para>20:興聯-中控解鎖</para>
                        ///  <para>21:興聯-中控上鎖</para>
                        ///  <para>22:興聯-防盜解鎖</para>
                        ///  <para>23:興聯-防盜上鎖</para>
                        ///  <para>24:興聯-設定萬用卡號</para>
                        ///  <para>25:興聯-解除萬用卡號</para>
                        ///  <para>26:興聯-設定客戶卡號</para>
                        ///  <para>27:興聯-解除客戶卡號</para>
                        ///  <para>28:興聯-開啟NFC電源</para>
                        ///  <para>29:興聯-關閉NFC電源</para>
                        ///  <para>30:興聯-重啟車機</para>
                        ///  <para>31:興聯-指定喇叭尋車方式</para>
                        ///  <para>32:興聯-指定閃燈尋車方式</para>
                        ///  <para>33:興聯-取車巨集指令</para>
                        ///  <para>34:興聯-還車巨集指令</para>
                        CensWebAPI CensAPI = new CensWebAPI();
                        WSOutput_Base wsOutput = new WSOutput_Base();
                        WSInput_SetOrderStatus wsOrderInput = null;
                        WSInput_SendLock wsLockInput = null;
                        WSInput_SendCardNo SendCardInput = null;
                        WSOutput_GetInfo wsOutputGetInfo = new WSOutput_GetInfo();
                        WSInput_SearchCarForSituation SearchCarForSituation = null;
                        WSInput_CombineCmdGetCar CombineCmdGetCar = null;
                        switch (apiInput.CmdType)
                        {
                            case 15:
                                if (!carCommon.BE_CheckCarHornFlgByCID(apiInput.CID, LogID, apiInput.UserId, ref errCode))
                                {
                                    SearchCarForSituation = new WSInput_SearchCarForSituation()
                                    {
                                        CID = CENSCID,
                                        CMD = 2
                                    };
                                    flag = CensAPI.SearchCarForSituation(SearchCarForSituation, ref wsOutput);
                                }
                                else
                                {
                                    flag = CensAPI.SearchCar(CENSCID, ref wsOutput);
                                }
                                if (!flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 16: //設定租約
                                wsOrderInput = new WSInput_SetOrderStatus()
                                {
                                    CID = CENSCID,
                                    OrderStatus = 1
                                };

                                flag = CensAPI.SetOrderStatus(wsOrderInput, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 17: //解除租約
                                wsOrderInput = new WSInput_SetOrderStatus()
                                {
                                    CID = CENSCID,
                                    OrderStatus = 0
                                };

                                flag = CensAPI.SetOrderStatus(wsOrderInput, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 18: //全車解鎖
                                wsLockInput = new WSInput_SendLock()
                                {
                                    CID = CENSCID,
                                    CMD = 0
                                };

                                flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 19: //全車上鎖
                                wsLockInput = new WSInput_SendLock()
                                {
                                    CID = CENSCID,
                                    CMD = 1
                                };

                                flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 20: //中控解鎖
                                wsLockInput = new WSInput_SendLock()
                                {
                                    CID = CENSCID,
                                    CMD = 2
                                };

                                flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;

                            case 21: //中控上鎖
                                wsLockInput = new WSInput_SendLock()
                                {
                                    CID = CENSCID,
                                    CMD = 3
                                };

                                flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;

                            case 22: //防盜解鎖
                                wsLockInput = new WSInput_SendLock()
                                {
                                    CID = CENSCID,
                                    CMD = 4
                                };

                                flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;

                            case 23: //防盜上鎖
                                wsLockInput = new WSInput_SendLock()
                                {
                                    CID = CENSCID,
                                    CMD = 5
                                };

                                flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;

                            case 24: //設定萬用卡
                                     //寫入萬用卡
                                SendCardInput = new WSInput_SendCardNo()
                                {
                                    CID = CENSCID,
                                    mode = 1

                                };

                                int count = 0;
                                int CardLen = apiInput.UnivCard.Length;
                                if (CardLen > 0)
                                {
                                    SendCarNoData[] CardData = new SendCarNoData[CardLen];
                                    for (int i = 0; i < CardLen; i++)
                                    {
                                        CardData[i] = new SendCarNoData();
                                        CardData[i].CardNo = apiInput.UnivCard[i];
                                        CardData[i].CardType = 0;
                                        count++;
                                    }
                                    //  Array.Resize(ref CardData, count + 1);
                                    SendCardInput.data = CardData;

                                    flag = CensAPI.SendCardNo(SendCardInput, ref wsOutput);
                                }
                                if (false == flag)
                                {
                                    errCode = wsOutput.ErrorCode;
                                }
                                break;
                            case 25: //解除萬用卡
                                break;
                            case 26: //設定客戶卡
                                SendCardInput = new WSInput_SendCardNo()
                                {
                                    CID = CENSCID,
                                    mode = 1

                                };

                                int ClientCount = 0;
                                int ClientCardLen = apiInput.ClientCardNo.Length;
                                if (ClientCardLen > 0)
                                {
                                    SendCarNoData[] CardData = new SendCarNoData[ClientCardLen];
                                    for (int i = 0; i < ClientCardLen; i++)
                                    {
                                        CardData[i] = new SendCarNoData();
                                        CardData[i].CardNo = apiInput.ClientCardNo[i];
                                        CardData[i].CardType = 1;
                                        ClientCount++;
                                    }
                                    //  Array.Resize(ref CardData, count + 1);
                                    SendCardInput.data = CardData;

                                    flag = CensAPI.SendCardNo(SendCardInput, ref wsOutput);
                                }
                                if (false == flag)
                                {
                                    errCode = wsOutput.ErrorCode;
                                }
                                break;

                            case 27: //解除客戶卡
                                break;
                            case 28: //開啟NFC電源
                                flag = CensAPI.NFCPower(CENSCID, 1, LogID, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 29: //關閉NFC電源
                                flag = CensAPI.NFCPower(CENSCID, 0, LogID, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 30: //重啟車機
                                flag = CensAPI.SoftwareReset(CENSCID, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 31:  //指定喇叭尋車方式
                                SearchCarForSituation = new WSInput_SearchCarForSituation()
                                {
                                    CID = CENSCID,
                                    CMD = 1
                                };
                                if (!carCommon.BE_CheckCarHornFlgByCID(apiInput.CID, LogID, apiInput.UserId, ref errCode))
                                {
                                    SearchCarForSituation.CMD = 2;
                                }
                                flag = CensAPI.SearchCarForSituation(SearchCarForSituation, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 32:  //指定閃燈尋車方式
                                SearchCarForSituation = new WSInput_SearchCarForSituation()
                                {
                                    CID = CENSCID,
                                    CMD = 2
                                };
                                flag = CensAPI.SearchCarForSituation(SearchCarForSituation, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 33:  //取車巨集指令
                                CombineCmdGetCar = new WSInput_CombineCmdGetCar()
                                {
                                    CID = CENSCID,
                                    data = new WSInput_CombineCmdGetCar.SendCarNoData[] { }
                                };
                                //寫入顧客卡
                                int CombineClientCount = 0;
                                int CombineClientCardLen = apiInput.ClientCardNo.Length;
                                if (CombineClientCardLen > 0)
                                {
                                    WSInput_CombineCmdGetCar.SendCarNoData[] CardData = new WSInput_CombineCmdGetCar.SendCarNoData[CombineClientCardLen];
                                    for (int i = 0; i < CombineClientCardLen; i++)
                                    {
                                        CardData[i] = new WSInput_CombineCmdGetCar.SendCarNoData();
                                        CardData[i].CardNo = apiInput.ClientCardNo[i];
                                        CombineClientCount++;
                                    }
                                    CombineCmdGetCar.data = CardData;
                                }
                                flag = CensAPI.CombineCmdGetCar(CombineCmdGetCar, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 34:  //還車巨集指令
                                flag = CensAPI.CombineCmdReturnCar(CENSCID, ref wsOutput);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                            case 100: //GetInfo
                                flag = CensAPI.GetInfo(CENSCID, ref wsOutputGetInfo);
                                if (false == flag || wsOutput.Result == 1)
                                {
                                    errCode = wsOutput.ErrorCode;
                                    errMsg = wsOutput.ErrMsg;
                                }
                                break;
                        }
                    }
                }
            }
            #endregion
            #region 判斷是否成功
            if (flag)
            {
                if (CarMachineType == 0 && isDebug == "0")
                {
                    int nowCount = 0;
                    bool waitFlag = false;
                    while (nowCount < 30)
                    {
                        Thread.Sleep(1000);
                        CarCMDResponse obj = new CarCMDRepository(connetStr).GetCMDData(requestId, method);
                        if (obj != null)
                        {
                            waitFlag = true;
                            if (obj.CmdReply != "Okay")
                            {
                                waitFlag = false;
                                errCode = "ERR167";
                            }
                            break;
                        }
                        nowCount++;
                    }
                    if (waitFlag == false)
                    {
                        if (errCode != "ERR167")
                        {
                            flag = false;
                            errCode = "ERR166";
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

        public static Dictionary<string, object> SendCarCmd(string CarNo, int inCmdType, string[] ClientCardNo, string[] UnivCard, string IPAddress)
        {
            #region 初始宣告
            //string[] headers=httpContext.Request.Headers.AllKeys;
            CarCMDRepository CarCMDRepository = new CarCMDRepository(connetStr);
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "SendCarCMDController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            string CID = "4BF5";
            string CENSCID = "90001";
            CommonFunc baseVerify = new CommonFunc();
            string deviceToken = "";
            IAPI_SendCarCMD apiInput = new IAPI_SendCarCMD();
            OAPI_Base outputApi = null;
            int CarMachineType = 0;


            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string method = "", requestId = "";
            OtherService.Enum.MachineCommandType.CommandType CmdType = new OtherService.Enum.MachineCommandType.CommandType();
            #endregion
            #region 防呆

            CarCmdData carCmdData = CarCMDRepository.GetCarCMDData(CarNo, ref flag);
            apiInput.deviceToken = carCmdData.deviceToken;
            apiInput.CID = carCmdData.CID;
            apiInput.IsCens = carCmdData.IsCens;
            apiInput.CmdType = inCmdType;
            apiInput.ClientCardNo = ClientCardNo;
            apiInput.UnivCard = UnivCard;

            if (apiInput.IsCens == 1)
            {
                //興聯及RITI轉換
                switch (apiInput.CmdType)
                {
                    case 0:
                        apiInput.CmdType = 15;
                        break;
                    case 2:
                        apiInput.CmdType = 24;
                        break;
                    case 3:
                        apiInput.CmdType = 18;
                        break;
                    case 4:
                        apiInput.CmdType = 19;
                        break;
                    case 5:
                        apiInput.CmdType = 16;
                        break;
                    case 6:
                        apiInput.CmdType = 17;
                        break;
                    case 7:
                        apiInput.CmdType = 20;
                        break;
                    case 8:
                        apiInput.CmdType = 21;
                        break;
                    case 9:
                        apiInput.CmdType = 22;
                        break;
                    case 10:
                        apiInput.CmdType = 23;
                        break;
                    case 12:
                        apiInput.CmdType = 26;
                        break;
                    case 13:
                        apiInput.CmdType = 27;
                        break;
                    case 14:
                        apiInput.CmdType = 25;
                        break;
                }
            }

            if (flag)
            {
                //apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_SendCarCMD>(Contentjson);
                //寫入API Log
                flag = baseVerify.InsAPLog(JsonConvert.SerializeObject(apiInput), IPAddress, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.CID))
                {
                    flag = false;
                    errCode = "ERR900";
                }
                else
                {
                    CID = apiInput.CID;
                    if (apiInput.CID.Length == 5)
                    {
                        CarMachineType = 1;
                        CENSCID = CID;
                    }
                }
                if (flag)
                {
                    if (CarMachineType != apiInput.IsCens)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }
                if (flag)
                {
                    if (CarMachineType == 0)
                    {

                        if (string.IsNullOrWhiteSpace(apiInput.deviceToken))
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                        else
                        {
                            deviceToken = apiInput.deviceToken;
                        }
                    }
                }
            }

            #endregion
            #region 第二段防呆
            if (flag)
            {
                if ((CarMachineType == 0 && apiInput.CmdType > 14 && (apiInput.CmdType < 51 || apiInput.CmdType > 60) && apiInput.CmdType != 99) || (CarMachineType == 1 && (apiInput.CmdType < 15 || (apiInput.CmdType > 50 && apiInput.CmdType < 61))))
                {
                    flag = false;
                    errCode = "ERR900";
                }
            }
            #endregion
            #region 判斷車機類型
            if (flag)
            {
                if (CarMachineType == 0)
                {
                    FETCatAPI FetAPI = new FETCatAPI();
                    string CommandType = "";
                    /// <para>0:尋車</para>
                    /// <para>1:查詢萬用卡號</para>
                    /// <para>2:設定萬用卡號</para>
                    /// <para>3:中控解鎖，防盜關閉</para>
                    /// <para>4:中控上鎖，防盜啟動</para>
                    /// <para>5:汽車設為有租約狀態</para>
                    /// <para>6:設定無租約狀態</para>
                    /// <para>7:中控解鎖</para>
                    /// <para>8:中控上鎖</para>
                    /// <para>9:防盜關閉</para>
                    /// <para>10:防盜啟動</para>
                    /// <para>11:查詢顧客卡號</para>
                    /// <para>12:設定顧客卡號</para>
                    /// <para>13:清除全部顧客卡號</para>
                    /// <para>14:清除全部萬用卡號</para>
                    /// <para>51:啟動喇叭搜尋汽車</para>
                    /// <para>52:啟動閃燈搜尋汽車</para>
                    /// <para>53:汽車租車組合指令</para>
                    /// <para>54:汽車還車組合指令</para>
                    switch (apiInput.CmdType)
                    {
                        case 0:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SearchVehicle);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SearchVehicle;

                            break;
                        case 1:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.QueryUnivCardNo);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.QueryUnivCardNo;
                            break;
                        case 2:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetUnivCardNo);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SetUnivCardNo;
                            break;
                        case 3:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.Unlock_AlertOff);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.Unlock_AlertOff;
                            break;
                        case 4:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.Lock_AlertOn);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.Lock_AlertOn;
                            break;
                        case 5:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetVehicleRent);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SetVehicleRent;
                            break;
                        case 6:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetNoRent);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SetNoRent;
                            break;

                        case 7:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.Unlock);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.Unlock;
                            break;
                        case 8:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.Lock);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.Lock;
                            break;
                        case 9:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.AlertOff);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.AlertOff;
                            break;
                        case 10:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.AlertOn);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.AlertOn;
                            break;
                        case 11:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.QueryClientCardNo);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.QueryClientCardNo;
                            break;

                        case 12:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo;
                            break;
                        case 13:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ClearAllClientCard);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.ClearAllClientCard;
                            break;
                        case 14:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ClearAllUnivCard);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.ClearAllUnivCard;
                            break;
                        case 51:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SearchVehicleHornOn);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SearchVehicleHornOn;
                            break;
                        case 52:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SearchVehicleLightFlash);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SearchVehicleLightFlash;
                            break;
                        case 53:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.VehicleRentCombo);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.VehicleRentCombo;
                            break;
                        case 54:
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.VehicleNoRentCombo);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.VehicleNoRentCombo;
                            break;
                        case 99: //99:ReportNow
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ReportNow);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.ReportNow;
                            break;
                    }
                    if (apiInput.CmdType == 2)
                    {
                        WSInput_Base<UnivCardNoObj> input = new WSInput_Base<UnivCardNoObj>()
                        {
                            command = true,
                            method = CommandType,
                            requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                            _params = new UnivCardNoObj()

                        };
                        input._params.UnivCardNo = apiInput.UnivCard;
                        requestId = input.requestId;
                        flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                    }
                    else if (apiInput.CmdType == 12 || apiInput.CmdType == 53)
                    {
                        WSInput_Base<ClientCardNoObj> input = new WSInput_Base<ClientCardNoObj>()
                        {
                            command = true,
                            method = CommandType,
                            requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                            _params = new ClientCardNoObj()

                        };
                        input._params.ClientCardNo = apiInput.ClientCardNo;
                        //組合指令顧客卡必輸入，若沒有則帶隨機值
                        if (apiInput.CmdType == 53 && input._params.ClientCardNo.Length == 0)
                        {
                            input._params.ClientCardNo = new string[] { (new Random()).Next(10000000, 99999999).ToString().PadLeft(10, 'X') };
                        }
                        requestId = input.requestId;
                        flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                    }
                    else
                    {
                        WSInput_Base<Params> input = new WSInput_Base<Params>()
                        {
                            command = true,
                            method = CommandType,
                            requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                            _params = new Params()

                        };
                        requestId = input.requestId;
                        flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                    }
                    method = CommandType;

                    #region 判斷是否成功
                    if (flag)
                    {
                        if (CarMachineType == 0)
                        {
                            int nowCount = 0;
                            bool waitFlag = false;
                            while (nowCount < 30)
                            {
                                Thread.Sleep(1000);
                                CarCMDResponse obj = new CarCMDRepository(connetStr).GetCMDData(requestId, method);
                                if (obj != null)
                                {
                                    waitFlag = true;
                                    if (obj.CmdReply != "Okay")
                                    {
                                        waitFlag = false;
                                        errCode = "ERR167";
                                        //    break;
                                    }
                                    break;
                                }
                                nowCount++;
                            }
                            if (waitFlag == false)
                            {
                                if (errCode != "ERR167")
                                {
                                    flag = false;
                                    errCode = "ERR166";
                                }

                            }
                        }
                    }
                    #endregion

                }
                else
                {
                    ///  <para>15:興聯-尋車</para>
                    ///  <para>16:興聯-汽車設為有租約狀態</para>
                    ///  <para>17:興聯-設定無租約狀態</para>
                    ///  <para>18:興聯-全車解鎖</para>
                    ///  <para>19:興聯-全車上鎖</para>
                    ///  <para>20:興聯-中控解鎖</para>
                    ///  <para>21:興聯-中控上鎖</para>
                    ///  <para>22:興聯-防盜解鎖</para>
                    ///  <para>23:興聯-防盜上鎖</para>
                    ///  <para>24:興聯-設定萬用卡號</para>
                    ///  <para>25:興聯-解除萬用卡號</para>
                    ///  <para>26:興聯-設定客戶卡號</para>
                    ///  <para>27:興聯-解除客戶卡號</para>
                    ///  <para>28:興聯-開啟NFC電源</para>
                    ///  <para>29:興聯-關閉NFC電源</para>
                    ///  <para>30:興聯-重啟車機</para>
                    ///  <para>31:興聯-指定喇叭尋車方式</para>
                    ///  <para>32:興聯-指定閃燈尋車方式</para>
                    ///  <para>33:興聯-取車巨集指令</para>
                    ///  <para>34:興聯-還車巨集指令</para>
                    CensWebAPI CensAPI = new CensWebAPI();
                    WSOutput_Base wsOutput = new WSOutput_Base();
                    WSInput_SetOrderStatus wsOrderInput = null;
                    WSInput_SendLock wsLockInput = null;
                    WSInput_SendCardNo SendCardInput = null;
                    WSOutput_GetInfo wsOutputGetInfo = new WSOutput_GetInfo();
                    WSInput_SearchCarForSituation SearchCarForSituation = null;
                    WSInput_CombineCmdGetCar CombineCmdGetCar = null;
                    switch (apiInput.CmdType)
                    {
                        case 15:

                            flag = CensAPI.SearchCar(CENSCID, ref wsOutput);
                            if (!flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 16: //設定租約
                            wsOrderInput = new WSInput_SetOrderStatus()
                            {
                                CID = CENSCID,
                                OrderStatus = 1
                            };

                            flag = CensAPI.SetOrderStatus(wsOrderInput, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 17: //解除租約
                            wsOrderInput = new WSInput_SetOrderStatus()
                            {
                                CID = CENSCID,
                                OrderStatus = 0
                            };

                            flag = CensAPI.SetOrderStatus(wsOrderInput, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 18: //全車解鎖
                            wsLockInput = new WSInput_SendLock()
                            {
                                CID = CENSCID,
                                CMD = 0
                            };

                            flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 19: //全車上鎖
                            wsLockInput = new WSInput_SendLock()
                            {
                                CID = CENSCID,
                                CMD = 1
                            };

                            flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 20: //中控解鎖
                            wsLockInput = new WSInput_SendLock()
                            {
                                CID = CENSCID,
                                CMD = 2
                            };

                            flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;

                        case 21: //中控上鎖
                            wsLockInput = new WSInput_SendLock()
                            {
                                CID = CENSCID,
                                CMD = 3
                            };

                            flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;

                        case 22: //防盜解鎖
                            wsLockInput = new WSInput_SendLock()
                            {
                                CID = CENSCID,
                                CMD = 4
                            };

                            flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;

                        case 23: //防盜上鎖
                            wsLockInput = new WSInput_SendLock()
                            {
                                CID = CENSCID,
                                CMD = 5
                            };

                            flag = CensAPI.SendLock(wsLockInput, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;

                        case 24: //設定萬用卡
                                 //寫入萬用卡
                            SendCardInput = new WSInput_SendCardNo()
                            {
                                CID = CENSCID,
                                mode = 1

                            };

                            int count = 0;
                            int CardLen = apiInput.UnivCard.Length;
                            if (CardLen > 0)
                            {
                                SendCarNoData[] CardData = new SendCarNoData[CardLen];
                                for (int i = 0; i < CardLen; i++)
                                {
                                    CardData[i] = new SendCarNoData();
                                    CardData[i].CardNo = apiInput.UnivCard[i];
                                    CardData[i].CardType = 0;
                                    count++;
                                }
                                //  Array.Resize(ref CardData, count + 1);
                                SendCardInput.data = CardData;

                                flag = CensAPI.SendCardNo(SendCardInput, ref wsOutput);
                            }
                            if (false == flag)
                            {
                                errCode = wsOutput.ErrorCode;
                            }
                            break;
                        case 25: //解除萬用卡
                            break;
                        case 26: //設定客戶卡
                            SendCardInput = new WSInput_SendCardNo()
                            {
                                CID = CENSCID,
                                mode = 1

                            };

                            int ClientCount = 0;
                            int ClientCardLen = apiInput.ClientCardNo.Length;
                            if (ClientCardLen > 0)
                            {
                                SendCarNoData[] CardData = new SendCarNoData[ClientCardLen];
                                for (int i = 0; i < ClientCardLen; i++)
                                {
                                    CardData[i] = new SendCarNoData();
                                    CardData[i].CardNo = apiInput.ClientCardNo[i];
                                    CardData[i].CardType = 1;
                                    ClientCount++;
                                }
                                //  Array.Resize(ref CardData, count + 1);
                                SendCardInput.data = CardData;

                                flag = CensAPI.SendCardNo(SendCardInput, ref wsOutput);
                            }
                            if (false == flag)
                            {
                                errCode = wsOutput.ErrorCode;
                            }
                            break;

                        case 27: //解除客戶卡
                            break;
                        case 28: //開啟NFC電源
                            flag = CensAPI.NFCPower(CENSCID, 1, LogID, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 29: //關閉NFC電源
                            flag = CensAPI.NFCPower(CENSCID, 0, LogID, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 30: //重啟車機
                            flag = CensAPI.SoftwareReset(CENSCID, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 31:  //指定喇叭尋車方式
                            SearchCarForSituation = new WSInput_SearchCarForSituation()
                            {
                                CID = CENSCID,
                                CMD = 1
                            };
                            flag = CensAPI.SearchCarForSituation(SearchCarForSituation, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 32:  //指定閃燈尋車方式
                            SearchCarForSituation = new WSInput_SearchCarForSituation()
                            {
                                CID = CENSCID,
                                CMD = 2
                            };
                            flag = CensAPI.SearchCarForSituation(SearchCarForSituation, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 33:  //取車巨集指令
                            CombineCmdGetCar = new WSInput_CombineCmdGetCar()
                            {
                                CID = CENSCID,
                                data = new WSInput_CombineCmdGetCar.SendCarNoData[] { }
                            };
                            //寫入顧客卡
                            int CombineClientCount = 0;
                            int CombineClientCardLen = apiInput.ClientCardNo.Length;
                            if (CombineClientCardLen > 0)
                            {
                                WSInput_CombineCmdGetCar.SendCarNoData[] CardData = new WSInput_CombineCmdGetCar.SendCarNoData[CombineClientCardLen];
                                for (int i = 0; i < CombineClientCardLen; i++)
                                {
                                    CardData[i] = new WSInput_CombineCmdGetCar.SendCarNoData();
                                    CardData[i].CardNo = apiInput.ClientCardNo[i];
                                    CombineClientCount++;
                                }
                                CombineCmdGetCar.data = CardData;
                            }
                            flag = CensAPI.CombineCmdGetCar(CombineCmdGetCar, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 34:  //還車巨集指令
                            flag = CensAPI.CombineCmdReturnCar(CENSCID, ref wsOutput);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
                        case 100: //GetInfo
                            flag = CensAPI.GetInfo(CENSCID, ref wsOutputGetInfo);
                            if (false == flag || wsOutput.Result == 1)
                            {
                                errCode = wsOutput.ErrorCode;
                                errMsg = wsOutput.ErrMsg;
                            }
                            break;
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, "");
            return objOutput;
            #endregion
        }
    }
}