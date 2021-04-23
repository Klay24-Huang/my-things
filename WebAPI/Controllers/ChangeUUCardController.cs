﻿using Domain.CarMachine;
using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Common;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Domain.WebAPI.Output.CENS;
using OtherService;
using Reposotory.Implement;
using StackExchange.Redis;
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
    public class ChangeUUCardController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoChangeUUCard(Dictionary<string, object> value)
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
            string funName = "ChangeUUCardController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_ReadCard apiInput = null;
            OAPI_ReadCard outputApi = new OAPI_ReadCard();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = false;
            int IsCens = 0;             //是否為興聯車機(0:否;1:是)
            string CardNo = "";         //新悠遊卡卡號
            string IDNO = "";
            string CID = "";            //車機編號
            string DeviceToken = "";    //遠傳車機token
            string OldCardNo = "";      //舊悠遊卡卡號
            List<CardList> lstCardList = new List<CardList>();
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_ReadCard>(Contentjson);
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
            // Token判斷
            if (flag)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    Token = Access_Token,
                    LogID = LogID
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

            // 開始做讀卡判斷
            if (flag)
            {
                SPInput_ReadCard spInput = new SPInput_ReadCard()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.ReadUUCard);
                SPOutput_ReadCard spOut = new SPOutput_ReadCard();
                SQLHelper<SPInput_ReadCard, SPOutput_ReadCard> sqlHelp = new SQLHelper<SPInput_ReadCard, SPOutput_ReadCard>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    CID = spOut.CID;
                    IsCens = spOut.IsCens;
                    DeviceToken = spOut.deviceToken;

                    List<ErrorInfo> lstCarError = new List<ErrorInfo>();
                    // 取得會員現在綁定的卡號
                    lstCardList = new CarCardCommonRepository(connetStr).GetCardListByCustom(IDNO, ref lstCarError);
                    if (lstCardList.Count > 0)
                    {
                        OldCardNo = lstCardList[0].CardNO;
                    }

                    // 興聯車機要開讀卡機電源
                    if (IsCens == 1)
                    {
                        CensWebAPI censWebAPI = new CensWebAPI();
                        WSOutput_Base WsOutput = new WSOutput_Base();
                        flag = censWebAPI.NFCPower(CID, 1, LogID, ref WsOutput);
                        if (flag == false)
                        {
                            errCode = WsOutput.ErrorCode;
                            errMsg = WsOutput.ErrMsg;
                        }
                    }
                }

                if (flag)
                {
                    int NowCount = 0;
                    DateTime NowTime = DateTime.Now.AddSeconds(-15);
                    bool ReadFlag = false;

                    while (NowCount < 60)
                    {
                        Thread.Sleep(1000);
                        ReadFlag = new CarCMDRepository(connetStr).CheckHasReadCard(CID, NowTime.ToString("yyyy-MM-dd HH:mm:ss"), ref CardNo);
                        if (ReadFlag)
                        {
                            break;
                        }
                        NowCount++;
                    }

                    // 20210414 ADD REASON.更換悠遊卡的流程修正
                    // 原流程：讀卡>解卡>綁卡>更新MemberData卡號
                    // 因須等車機回應會花很多時間，因此將這支API拆掉，只做到讀卡，有讀到卡就回應，解卡以後的事情用排程去處理
                    if (ReadFlag)
                    {
                        if (OldCardNo != CardNo)
                        {
                            string SPName2 = new ObjType().GetSPName(ObjType.SPType.ChangeUUCard);
                            SPInput_ChangeUUCard SPInput = new SPInput_ChangeUUCard
                            {
                                OrderNo = tmpOrder,
                                IDNO = IDNO,
                                CID = CID,
                                DeviceToken = DeviceToken,
                                IsCens = IsCens,
                                OldCardNo = OldCardNo,
                                NewCardNo = CardNo,
                                LogID = LogID
                            };
                            SPOutput_Base SPOutput = new SPOutput_Base();
                            SQLHelper<SPInput_ChangeUUCard, SPOutput_Base> sqlBindHelp = new SQLHelper<SPInput_ChangeUUCard, SPOutput_Base>(connetStr);
                            flag = sqlBindHelp.ExecuteSPNonQuery(SPName2, SPInput, ref SPOutput, ref lstError);
                            baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                        }

                        if (flag)
                            outputApi.HasBind = 1;
                        else
                            outputApi.HasBind = 0;
                    }
                    else
                    {
                        outputApi.HasBind = 0;
                    }

                    #region Mark
                    //if (ReadFlag == false)
                    //{
                    //    outputApi.HasBind = 0;
                    //}

                    //// 讀到卡才解綁卡
                    //if (ReadFlag)
                    //{
                    //    // 有舊卡才解卡
                    //    if (!string.IsNullOrEmpty(OldCardNo))
                    //    {
                    //        CarCommonFunc CarComm = new CarCommonFunc();
                    //        if (spOut.IsCens == 1)
                    //        {
                    //            SendCarNoData[] sendCarNoDatas = new SendCarNoData[1];
                    //            SendCarNoData obj = new SendCarNoData()
                    //            {
                    //                CardNo = OldCardNo,
                    //                CardType = 1
                    //            };
                    //            sendCarNoDatas[0] = obj;
                    //            flag = CarComm.DoSetCensCustomerCard(CID, sendCarNoDatas, 0, ref errCode);
                    //        }
                    //        else
                    //        {
                    //            flag = CarComm.DoSetFETCustomerCard(CID, spOut.deviceToken, new string[] { OldCardNo }, 0, LogID, ref errCode);
                    //        }

                    //        if (!flag)
                    //        {
                    //            outputApi.HasBind = 0;
                    //        }
                    //    }

                    //    // 有新卡才綁卡
                    //    if (flag && OldCardNo != CardNo)
                    //    {
                    //        // 開始對車機做動作
                    //        if (IsCens == 1)
                    //        {
                    //            // 興聯車機
                    //            CensWebAPI webAPI = new CensWebAPI();
                    //            // 取最新狀況
                    //            WSOutput_GetInfo wsOutInfo = new WSOutput_GetInfo();
                    //            flag = webAPI.GetInfo(CID, ref wsOutInfo);
                    //            if (flag == false)
                    //            {
                    //                errCode = wsOutInfo.ErrorCode;
                    //            }
                    //            else
                    //            {
                    //                if (wsOutInfo.data.CID != CID)
                    //                {
                    //                    flag = false;
                    //                    errCode = "ERR468";
                    //                }
                    //            }
                    //            // 寫入顧客卡
                    //            if (flag)
                    //            {
                    //                //要將卡號寫入車機
                    //                SendCarNoData[] CardData = new SendCarNoData[1];
                    //                //寫入顧客卡
                    //                WSInput_SendCardNo wsInput = new WSInput_SendCardNo()
                    //                {
                    //                    CID = CID,
                    //                    mode = 1
                    //                };

                    //                CardData[0] = new SendCarNoData();
                    //                CardData[0].CardNo = CardNo;
                    //                CardData[0].CardType = 1;

                    //                wsInput.data = new SendCarNoData[0];
                    //                wsInput.data = CardData;
                    //                WSOutput_Base wsOut = new WSOutput_Base();
                    //                Thread.Sleep(500);
                    //                flag = webAPI.SendCardNo(wsInput, ref wsOut);
                    //                if (flag == false)
                    //                {
                    //                    errCode = wsOut.ErrorCode;
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            // 遠傳
                    //            //取最新狀況, 先送getlast之後從tb捉最近一筆
                    //            FETCatAPI FetAPI = new FETCatAPI();
                    //            string requestId = "";
                    //            string CommandType = "";
                    //            OtherService.Enum.MachineCommandType.CommandType CmdType;
                    //            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ReportNow);
                    //            CmdType = OtherService.Enum.MachineCommandType.CommandType.ReportNow;
                    //            WSInput_Base<Params> input = new WSInput_Base<Params>()
                    //            {
                    //                command = true,
                    //                method = CommandType,
                    //                requestId = string.Format("{0}_{1}", spOut.CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                    //                _params = new Params()
                    //            };
                    //            requestId = input.requestId;
                    //            string method = CommandType;
                    //            flag = FetAPI.DoSendCmd(spOut.deviceToken, spOut.CID, CmdType, input, LogID);
                    //            if (flag)
                    //            {
                    //                flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                    //            }

                    //            if (flag)
                    //            {
                    //                string[] CardStr = new string[1];
                    //                CardStr[0] = CardNo;

                    //                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo);
                    //                CmdType = OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo;
                    //                WSInput_Base<ClientCardNoObj> SetCardInput = new WSInput_Base<ClientCardNoObj>()
                    //                {
                    //                    command = true,
                    //                    method = CommandType,
                    //                    requestId = string.Format("{0}_{1}", spOut.CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                    //                    _params = new ClientCardNoObj()
                    //                    {
                    //                        ClientCardNo = CardStr
                    //                    }
                    //                };
                    //                requestId = SetCardInput.requestId;
                    //                method = CommandType;
                    //                flag = FetAPI.DoSendCmd(spOut.deviceToken, spOut.CID, CmdType, SetCardInput, LogID);
                    //                if (flag)
                    //                {
                    //                    flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                    //                }
                    //            }
                    //        }

                    //        if (flag)
                    //        {
                    //            string SPName2 = new ObjType().GetSPName(ObjType.SPType.BindUUCard);
                    //            SPInput_BindUUCard SPBindInput = new SPInput_BindUUCard()
                    //            {
                    //                IDNO = IDNO,
                    //                OrderNo = tmpOrder,
                    //                Token = Access_Token,
                    //                CardNo = CardNo,
                    //                LogID = LogID
                    //            };
                    //            SPOutput_Base SPBindOutput = new SPOutput_Base();
                    //            SQLHelper<SPInput_BindUUCard, SPOutput_Base> sqlBindHelp = new SQLHelper<SPInput_BindUUCard, SPOutput_Base>(connetStr);
                    //            flag = sqlBindHelp.ExecuteSPNonQuery(SPName2, SPBindInput, ref SPBindOutput, ref lstError);
                    //            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                    //        }

                    //        if (flag)
                    //        {
                    //            outputApi.HasBind = 1;
                    //        }
                    //    }
                    //}
                    #endregion
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