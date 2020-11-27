using Domain.Common;
using Domain.CarMachine;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Common;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Output.CENS;
using OtherService;
using Reposotory.Implement;
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
            int IsCens = 0;
            string CardNo = "";
            string IDNO = "";
            string CID = "";
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
            //Token判斷
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

            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.BE_GetCarMachineAndCheckOrder);
                SPInput_BE_GetCarMachineAndCheckOrder spInput = new SPInput_BE_GetCarMachineAndCheckOrder()
                {
                    IDNO = IDNO,
                    OrderNo = tmpOrder,
                    LogID = LogID
                };
                SPOutput_BE_GetCarMachineAndCheckOrder spOut = new SPOutput_BE_GetCarMachineAndCheckOrder();
                SQLHelper<SPInput_BE_GetCarMachineAndCheckOrder, SPOutput_BE_GetCarMachineAndCheckOrder> sqlHelp = new SQLHelper<SPInput_BE_GetCarMachineAndCheckOrder, SPOutput_BE_GetCarMachineAndCheckOrder>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    #region 車機
                    CarCommonFunc CarComm = new CarCommonFunc();
                    if (spOut.IsCens == 1)
                    {
                        SendCarNoData[] sendCarNoDatas = new SendCarNoData[1];
                        SendCarNoData obj = new SendCarNoData()
                        {
                            CardNo = spOut.CardNo,
                            CardType = 1
                        };
                        sendCarNoDatas[0] = obj;
                        flag = CarComm.DoSetCensCustomerCard(spOut.CID, sendCarNoDatas, 0, ref errCode);
                    }
                    else
                    {
                        flag = CarComm.DoSetFETCustomerCard(spOut.CID, spOut.deviceToken, new string[] { spOut.CardNo }, 0, LogID, ref errCode);
                    }
                    #endregion
                }
            }

            //開始做讀卡判斷
            if (flag)
            {
                SPInput_ReadCard spInput = new SPInput_ReadCard()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.ReadCard);
                SPOutput_ReadCard spOut = new SPOutput_ReadCard();
                SQLHelper<SPInput_ReadCard, SPOutput_ReadCard> sqlHelp = new SQLHelper<SPInput_ReadCard, SPOutput_ReadCard>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    // 興聯車機要開讀卡機電源
                    if (spOut.IsCens == 1)
                    {
                        CensWebAPI censWebAPI = new CensWebAPI();
                        WSOutput_Base WsOutput = new WSOutput_Base();
                        flag = censWebAPI.NFCPower(spOut.CID, 1, LogID, ref WsOutput);
                        if (flag == false)
                        {
                            errCode = WsOutput.ErrorCode;
                            errMsg = WsOutput.ErrMsg;
                        }

                        IsCens = 1;


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
                        ReadFlag = new CarCMDRepository(connetStr).CheckHasReadCard(spOut.CID, NowTime.ToString("yyyy-MM-dd HH:mm:ss"), ref CardNo);
                        if (ReadFlag)
                        {
                            outputApi.HasBind = 1;
                            break;
                        }
                        NowCount++;
                    }
                    CID = spOut.CID;
                    if (ReadFlag == false)
                    {
                        outputApi.HasBind = 0;
                    }

                    if (ReadFlag)
                    {
                        string SPName2 = new ObjType().GetSPName(ObjType.SPType.BindUUCard);
                        SPInput_BindUUCard SPBindInput = new SPInput_BindUUCard()
                        {
                            IDNO = IDNO,
                            OrderNo = tmpOrder,
                            Token = Access_Token,
                            CardNo = CardNo,
                            LogID = LogID
                        };
                        SPOutput_Base SPBindOutput = new SPOutput_Base();
                        SQLHelper<SPInput_BindUUCard, SPOutput_Base> sqlBindHelp = new SQLHelper<SPInput_BindUUCard, SPOutput_Base>(connetStr);
                        flag = sqlBindHelp.ExecuteSPNonQuery(SPName2, SPBindInput, ref SPBindOutput, ref lstError);
                        baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);


                        //寫入顧客卡
                        if (flag && IsCens == 1)
                        {
                            CensWebAPI webAPI = new CensWebAPI();
                            //取最新狀況
                            WSOutput_GetInfo wsOutInfo = new WSOutput_GetInfo();
                            flag = webAPI.GetInfo(CID, ref wsOutInfo);
                            if (false == flag)
                            {
                                errCode = wsOutInfo.ErrorCode;
                                //mil = 0;
                            }
                            else
                            {
                                if (wsOutInfo.data.CID == CID)
                                {
                                    if (wsOutInfo.data.Milage > 0)
                                    {
                                        //mil = wsOutInfo.data.Milage;
                                    }
                                    else
                                    {
                                        //判斷是否為0，若是0則抓取前一天內里程大於0的值
                                        //DbAssister da = new DbAssister();
                                        //mil = 0;
                                    }
                                }
                                else
                                {
                                    flag = false;
                                    errCode = "ERR468";
                                }
                            }
                            //要將卡號寫入車機
                            int count = 0;
                            //int CardLen = lstCardList.Count;
                            int CardLen = 1;
                            if (CardLen > 0)
                            {
                                SendCarNoData[] CardData = new SendCarNoData[CardLen];
                                //寫入顧客卡
                                WSInput_SendCardNo wsInput = new WSInput_SendCardNo()
                                {
                                    CID = CID,
                                    mode = 1
                                };
                                for (int i = 0; i < CardLen; i++)
                                {
                                    CardData[i] = new SendCarNoData();
                                    //CardData[i].CardNo = lstCardList[i].CardNO;
                                    //CardData[i].CardType = (lstCardList[i].CardType == "C") ? 1 : 0;
                                    CardData[i].CardNo = CardNo;
                                    CardData[i].CardType = 1;
                                    count++;
                                }
                                //  Array.Resize(ref CardData, count + 1);
                                wsInput.data = new SendCarNoData[CardLen];
                                wsInput.data = CardData;
                                WSOutput_Base wsOut = new WSOutput_Base();
                                Thread.Sleep(500);
                                flag = webAPI.SendCardNo(wsInput, ref wsOut);
                                if (false == flag)
                                {
                                    errCode = wsOut.ErrorCode;
                                }
                            }
                        }
                        else
                        {
                            /*
                            if (lstCardList != null)
                            {
                                int CardLen = lstCardList.Count;
                                if (CardLen > 0)
                                {
                                    string[] CardStr = new string[CardLen];
                                    for (int i = 0; i < CardLen; i++)
                                    {
                                        CardStr[i] = lstCardList[i].CardNO;
                                    }
                                    if (CardStr.Length > 0)
                                    {
                                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo);
                                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo;
                                        WSInput_Base<ClientCardNoObj> SetCardInput = new WSInput_Base<ClientCardNoObj>()
                                        {
                                            command = true,
                                            method = CommandType,
                                            requestId = string.Format("{0}_{1}", spOut.CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                            _params = new ClientCardNoObj()
                                            {
                                                ClientCardNo = CardStr
                                            }
                                        };
                                        requestId = SetCardInput.requestId;
                                        method = CommandType;
                                        flag = FetAPI.DoSendCmd(spOut.deviceToken, spOut.CID, CmdType, SetCardInput, LogID);
                                        if (flag)
                                        {
                                            flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                                        }
                                    }
                                }

                            }
                            */
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