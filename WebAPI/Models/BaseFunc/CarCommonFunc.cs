﻿using Domain.CarMachine;
using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Common;
using Domain.SP.Output.Rent;
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

namespace WebAPI.Models.BaseFunc
{
    /// <summary>
    /// 車機基本共用
    /// </summary>
    public class CarCommonFunc
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string isDebug = (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["isDebug"])) ? "0" : ConfigurationManager.AppSettings["isDebug"].ToString();
        private string CENSCID = (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["MockCID"])) ? "90001" : ConfigurationManager.AppSettings["MockCID"].ToString();
        private string ClosePolygonOpen = (ConfigurationManager.AppSettings["ClosePolygonOpen"] == null) ? "1" : ConfigurationManager.AppSettings["ClosePolygonOpen"].ToString();
        /// <summary>
        /// 還車最後檢查
        /// </summary>
        /// <param name="tmpOrder">訂單編號</param>
        /// <param name="IDNO">身份證</param>
        /// <param name="LogID">api呼叫記錄</param>
        /// <param name="Access_Token">token</param>
        /// <para>TRUE:是</para>
        /// <para>FALSE:否</para>
        /// </param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool CheckReturnCar(Int64 tmpOrder,string IDNO,Int64 LogID,string Access_Token,ref string errCode)
        {
            int IsCens = 0;
            int IsMotor = 0;
            string deviceToken = "";
            string StationID = "";
            string CID = "";
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            CommonFunc baseVerify = new CommonFunc();
            bool flag = false;
            SPInput_CheckCarByReturn spInput = new SPInput_CheckCarByReturn()
            {
                OrderNo = tmpOrder,
                IDNO = IDNO,
                LogID = LogID,
                Token = Access_Token
            };
            string SPName = new ObjType().GetSPName(ObjType.SPType.CheckCarStatusByReturn);
            SPOutput_CheckCarStatusByReturn spOut = new SPOutput_CheckCarStatusByReturn();
            SQLHelper<SPInput_CheckCarByReturn, SPOutput_CheckCarStatusByReturn> sqlHelp = new SQLHelper<SPInput_CheckCarByReturn, SPOutput_CheckCarStatusByReturn>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            if (flag)
            {
                CID = spOut.CID;
                StationID = spOut.StationID;
                IsCens = spOut.IsCens;
                IsMotor = spOut.IsMotor;
                deviceToken = spOut.deviceToken;
            }
            #region 車機
            if (flag)
            {
                #region 汽車
                if (IsMotor == 0)
                {

                    if (IsCens == 1)
                    {
                        #region 興聯車機
                        CensWebAPI webAPI = new CensWebAPI();
                        //取最新狀況
                        WSOutput_GetInfo wsOutInfo = new WSOutput_GetInfo();
                        flag = webAPI.GetInfo(CID, ref wsOutInfo);
                        if (false == flag)
                        {
                            errCode = wsOutInfo.ErrorCode;

                        }
                        else
                        {
                            if (wsOutInfo.data.CID != CID)

                            {
                                flag = false;
                                errCode = "ERR400";
                            }
                        }
                        #region 判斷是否熄火
                        if (flag)
                        {
                           
                            if (wsOutInfo.data.PowOn == 1)
                            {
                                flag = false;
                                errCode = "ERR186";
                            }
                        }
                        #endregion
                        #region 判斷是否關閉電源
                        if (flag)
                        {
                            if (wsOutInfo.data.AccOn == 1)
                            {
                                flag = false;
                                errCode = "ERR187";
                            }
                        }
                        #endregion
                        #region 判斷是否在據點內
                        if (flag)
                        {
                            Domain.Common.Polygon Nowlatlng = new Domain.Common.Polygon()
                            {
                                Latitude = Convert.ToDouble(wsOutInfo.data.Lat),
                                Longitude = Convert.ToDouble(wsOutInfo.data.Lng)
                            };
                            flag = CheckInPolygon(Nowlatlng, StationID);
                            #region 快樂模式
                            if (ClosePolygonOpen == "0")
                            {
                                flag = true;
                            }
                            #endregion
                            if (false == flag)
                            {
                                errCode = "ERR188";
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        #region 遠傳車機
                        //取最新狀況, 先送getlast之後從tb捉最近一筆
                        FETCatAPI FetAPI = new FETCatAPI();
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
                        flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                        if (flag)
                        {
                            flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                        }
                        if (flag)
                        {
                            CarInfo info = new CarStatusCommon(connetStr).GetInfoByCar(CID);
                            if (info != null)
                            {
                                #region 判斷是否熄火
                                if (flag)
                                {
                              
                                    if (info.PowerONStatus == 1)
                                    {
                                        flag = false;
                                        errCode = "ERR186";
                                    }
                                }
                                #endregion
                                #region 判斷是否關閉電源
                                if (flag)
                                {
                                    if (info.ACCStatus == 1)
                                    {
                                        flag = false;
                                        errCode = "ERR187";
                                    }
                                }
                                #endregion
                                #region 判斷是否在據點內
                                if (flag)
                                {
                                    Domain.Common.Polygon Nowlatlng = new Domain.Common.Polygon()
                                    {
                                        Latitude = info.Latitude,
                                        Longitude = info.Longitude
                                    };
                                    flag = CheckInPolygon(Nowlatlng, StationID);
                                    #region 快樂模式
                                    if (ClosePolygonOpen == "0")
                                    {
                                        flag = true;
                                    }
                                    #endregion
                                    if (false == flag)
                                    {
                                        errCode = "ERR188";
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion
                    }
                }
                #endregion
                else
                {
                    #region 機車
                    FETCatAPI FetAPI = new FETCatAPI();
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
                    flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                    if (flag)
                    {
                        flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                    }

                    if (flag)
                    {
                        MotorInfo info = new CarStatusCommon(connetStr).GetInfoByMotor(CID);
                        if (info != null)
                        {
                            #region 判斷是否熄火
                            if (flag)
                            {
                               
                                if (info.ACCStatus == 1)
                                {
                                    flag = false;
                                    errCode = "ERR186";
                                }
                            }
                            #endregion
                            #region 判斷是否關閉電池架
                            if (flag)
                            {
                                if (info.deviceBat_Cover == 1)
                                {
                                    flag = false;
                                    errCode = "ERR189";
                                }
                            }
                            #endregion
                            #region 判斷是否在據點內
                            if (flag)
                            {
                                Domain.Common.Polygon Nowlatlng = new Domain.Common.Polygon()
                                {
                                    Latitude = info.Latitude,
                                    Longitude = info.Longitude
                                };
                                flag = CheckInPolygon(Nowlatlng, StationID);
                                #region 快樂模式
                                if (ClosePolygonOpen == "0")
                                {
                                    flag = true;
                                }
                                #endregion
                                if (false == flag)
                                {
                                    errCode = "ERR188";
                                }

                            }
                            #endregion
                        }
                    }
                    #endregion
                }

            }
            #endregion
            return flag;
        }
        public bool DoCloseRent(Int64 tmpOrder, string IDNO, Int64 LogID, string Access_Token, ref string errCode)
        {
            int IsCens = 0;
            int IsMotor = 0;
            string deviceToken = "";
            string StationID = "";
            string CID = "";
            List<CardList> lstCardList = new List<CardList>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            CommonFunc baseVerify = new CommonFunc();
            bool flag = false;
            SPInput_CheckCarByReturn spInput = new SPInput_CheckCarByReturn()
            {
                OrderNo = tmpOrder,
                IDNO = IDNO,
                LogID = LogID,
                Token = Access_Token
            };
            string SPName = new ObjType().GetSPName(ObjType.SPType.CheckCarStatusByReturn);
            SPOutput_CheckCarStatusByReturn spOut = new SPOutput_CheckCarStatusByReturn();
            SQLHelper<SPInput_CheckCarByReturn, SPOutput_CheckCarStatusByReturn> sqlHelp = new SQLHelper<SPInput_CheckCarByReturn, SPOutput_CheckCarStatusByReturn>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            if (flag)
            {
                CID = spOut.CID;
                StationID = spOut.StationID;
                IsCens = spOut.IsCens;
                IsMotor = spOut.IsMotor;
                deviceToken = spOut.deviceToken;
                List<ErrorInfo> lstCarError = new List<ErrorInfo>();
                lstCardList = new CarCardCommonRepository(connetStr).GetCardListByCustom(CID.ToUpper(), ref lstCarError);
            }
            #region 車機
            if (flag)
            {
                #region 汽車
                if (IsMotor == 0)
                {

                    if (IsCens == 1)
                    {
                        #region 興聯車機
                        CensWebAPI webAPI = new CensWebAPI();
                        //取最新狀況
                        WSOutput_GetInfo wsOutInfo = new WSOutput_GetInfo();
                        flag = webAPI.GetInfo(CID, ref wsOutInfo);
                        if (false == flag)
                        {
                            errCode = wsOutInfo.ErrorCode;

                        }
                        else
                        {
                            if (wsOutInfo.data.CID != CID)

                            {
                                flag = false;
                                errCode = "ERR400";
                            }
                        }
                        #region 車門
                        if (flag)
                        {
                            //車門
                            if (wsOutInfo.data.doorStatus == "N")
                            {
                                flag = false;
                                errCode = "ERR434";
                            }
                            else
                            {
                                int DoorOpen = wsOutInfo.data.doorStatus.IndexOf('0');
                                if (DoorOpen > -1)
                                {
                                    flag = false;
                                    errCode = "ERR43" + DoorOpen.ToString();
                                }
                            }
                        }
                        #endregion
                        #region 室內燈
                        //室內燈
                        if (flag && wsOutInfo.data.IndoorLight != 0)
                        {
                            if (wsOutInfo.data.IndoorLight == 1)
                            {
                                flag = false;
                                errCode = "ERR439";
                            }
                            else
                            {
                                flag = false;
                                errCode = "ERR440";
                            }
                        }
                        #endregion
                        #region 判斷是否熄火
                        if (flag)
                        {

                            if (wsOutInfo.data.PowOn == 1)
                            {
                                flag = false;
                                errCode = "ERR186";
                            }
                        }
                        #endregion
                        #region 判斷是否關閉電源
                        if (flag)
                        {
                            if (wsOutInfo.data.AccOn == 1)
                            {
                                flag = false;
                                errCode = "ERR187";
                            }
                        }
                        #endregion
                        #region 判斷是否在據點內
                        if (flag)
                        {
                            Domain.Common.Polygon Nowlatlng = new Domain.Common.Polygon()
                            {
                                Latitude = Convert.ToDouble(wsOutInfo.data.Lat),
                                Longitude = Convert.ToDouble(wsOutInfo.data.Lng)
                            };
                            flag = CheckInPolygon(Nowlatlng, StationID);
                            #region 快樂模式
                            if (ClosePolygonOpen == "0")
                            {
                                flag = true;
                            }
                            #endregion
                            if (false == flag)
                            {
                                errCode = "ERR188";
                            }
                        }
                        #endregion
                        #region 清空顧客卡及寫入萬用卡
                        WSOutput_Base wsOut = new WSOutput_Base();
                        if (lstError.Count == 0)
                        {
                            //清空顧客卡及寫入萬用卡
                            WSInput_SendCardNo wsInput = new WSInput_SendCardNo()
                            {
                                CID = CID,
                                mode = 0

                            };

                            int count = 0;
                            int CardLen = lstCardList.Count;
                            SendCarNoData[] CardData = new SendCarNoData[CardLen];
                            for (int i = 0; i < CardLen; i++)
                            {
                                CardData[i] = new SendCarNoData();
                                CardData[i].CardNo = lstCardList[i].CardNO;
                                CardData[i].CardType = (lstCardList[i].CardType == "C") ? 1 : 0;
                                count++;
                            }
                            //  Array.Resize(ref CardData, count + 1);
                            wsInput.data = CardData;

                            flag = webAPI.SendCardNo(wsInput, ref wsOut);
                        }
                        if (false == flag)
                        {
                            errCode = wsOut.ErrorCode;
                        }
                        //解除白名單
                        if (flag)
                        {
                            WSInput_SetOrderStatus wsOrderInput = new WSInput_SetOrderStatus()
                            {
                                CID = CID,
                                OrderStatus = 0
                            };
                            flag = webAPI.SetOrderStatus(wsOrderInput, ref wsOut);
                            if (false == flag || wsOut.Result == 1)
                            {
                                errCode = wsOut.ErrorCode;
                            }
                        }
                        //上防盜
                        if (flag)
                        {
                            WSInput_SendLock wsLockInput = new WSInput_SendLock()
                            {
                                CID = CID,
                                CMD = 1
                            };
                            flag = webAPI.SendLock(wsLockInput, ref wsOut);
                            if (false == flag || wsOut.Result == 1)
                            {
                                errCode = wsOut.ErrorCode;
                            }
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        #region 遠傳車機
                        //取最新狀況, 先送getlast之後從tb捉最近一筆
                        FETCatAPI FetAPI = new FETCatAPI();
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
                        flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                        if (flag)
                        {
                            flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                        }
                        if (flag)
                        {
                            CarInfo info = new CarStatusCommon(connetStr).GetInfoByCar(CID);
                            if (info != null)
                            {
                                #region 判斷是否熄火
                                if (flag)
                                {

                                    if (info.PowerONStatus == 1)
                                    {
                                        flag = false;
                                        errCode = "ERR186";
                                    }
                                }
                                #endregion
                                #region 判斷是否關閉電源
                                if (flag)
                                {
                                    if (info.ACCStatus == 1)
                                    {
                                        flag = false;
                                        errCode = "ERR187";
                                    }
                                }
                                #endregion
                                #region 判斷是否在據點內
                                if (flag)
                                {
                                    Domain.Common.Polygon Nowlatlng = new Domain.Common.Polygon()
                                    {
                                        Latitude = info.Latitude,
                                        Longitude = info.Longitude
                                    };
                                    flag = CheckInPolygon(Nowlatlng, StationID);
                                    #region 快樂模式
                                    if (ClosePolygonOpen == "0")
                                    {
                                        flag = true;
                                    }
                                    #endregion
                                    if (false == flag)
                                    {
                                        errCode = "ERR188";
                                    }
                                }
                                #endregion
                            }
                            
                        }
                        if (flag)
                        {
                            if (lstCardList != null)
                            {

                                int CardLen = lstCardList.Count;
                                //清空萬用卡
                                if (flag)
                                {
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ClearAllClientCard);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.ClearAllClientCard;
                                    WSInput_Base<Params> ClearInput = new WSInput_Base<Params>()
                                    {
                                        command = true,
                                        method = CommandType,
                                        requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                        _params = new Params()

                                    };
                                    requestId = input.requestId;
                                    flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, ClearInput, LogID);
                                    if (flag)
                                    {
                                        flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                                    }
                                }
                                //寫入萬用卡
                                if (CardLen > 0)
                                {
                                    
                                    

                                    string[] CardStr = new string[CardLen];
                                    int NowCount = -1;
                                    for (int i = 0; i < CardLen; i++)
                                    {
                                        if (lstCardList[i].CardType == "M")
                                        {
                                            NowCount++;
                                            CardStr[NowCount] = lstCardList[i].CardNO;
                                           
                                        }
                                      
                                    }

                                    if (NowCount >= 0)
                                    {
                                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetUnivCardNo);
                                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SetUnivCardNo;
                                        WSInput_Base<UnivCardNoObj> SetCardInput = new WSInput_Base<UnivCardNoObj>()
                                        {
                                            command = true,
                                            method = CommandType,
                                            requestId = string.Format("{0}_{1}", spOut.CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                            _params = new UnivCardNoObj()
                                            {
                                                 UnivCardNo = CardStr
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
                                //清除租約
                                if (flag)
                                {
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetNoRent);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.SetNoRent;
                                    WSInput_Base<Params> SetNoRentInput = new WSInput_Base<Params>()
                                    {
                                        command = true,
                                        method = CommandType,
                                        requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                        _params = new Params()

                                    };
                                    requestId = input.requestId;
                                    flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, SetNoRentInput, LogID);
                                    if (flag)
                                    {
                                        flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                                    }
                                }
                                //全車上鎖
                                if (flag)
                                {
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.Lock_AlertOn);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.Lock_AlertOn;
                                    WSInput_Base<Params> SetNoRentInput = new WSInput_Base<Params>()
                                    {
                                        command = true,
                                        method = CommandType,
                                        requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                        _params = new Params()

                                    };
                                    requestId = input.requestId;
                                    flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, SetNoRentInput, LogID);
                                    if (flag)
                                    {
                                        flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                                    }
                                }

                            }
                        }
                        #endregion
                    }
                }
                #endregion
                else
                {
                    #region 機車
                    FETCatAPI FetAPI = new FETCatAPI();
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
                    flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, input, LogID);
                    if (flag)
                    {
                        flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                    }

                    if (flag)
                    {
                        MotorInfo info = new CarStatusCommon(connetStr).GetInfoByMotor(CID);
                        if (info != null)
                        {
                            #region 判斷是否熄火
                            if (flag)
                            {

                                if (info.ACCStatus == 1)
                                {
                                    flag = false;
                                    errCode = "ERR186";
                                }
                            }
                            #endregion
                            #region 判斷是否關閉電池架
                            if (flag)
                            {
                                if (info.deviceBat_Cover == 1)
                                {
                                    flag = false;
                                    errCode = "ERR189";
                                }
                            }
                            #endregion
                            #region 判斷是否在據點內
                            if (flag)
                            {
                                Domain.Common.Polygon Nowlatlng = new Domain.Common.Polygon()
                                {
                                    Latitude = info.Latitude,
                                    Longitude = info.Longitude
                                };
                                flag = CheckInPolygon(Nowlatlng, StationID);
                                #region 快樂模式
                                if (ClosePolygonOpen == "0")
                                {
                                    flag = true;
                                }
                                #endregion
                                if (false == flag)
                                {
                                    errCode = "ERR188";
                                }

                            }
                            #endregion
                        }
                        if (flag)
                        {
                            //解除租約
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetNoRent);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SetNoRent;
                            WSInput_Base<Params> SetNoRentInput = new WSInput_Base<Params>()
                            {
                                command = true,
                                method = CommandType,
                                requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                _params = new Params()

                            };
                            requestId = input.requestId;
                            flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, SetNoRentInput, LogID);
                            if (flag)
                            {
                                flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                            }
                        }
                        if (flag)
                        {
                            //熄火
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOff);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOff;
                            WSInput_Base<Params> SetNoRentInput = new WSInput_Base<Params>()
                            {
                                command = true,
                                method = CommandType,
                                requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                _params = new Params()

                            };
                            requestId = input.requestId;
                            flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, SetNoRentInput, LogID);
                            if (flag)
                            {
                                flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                            }
                        }
                    }
                    #endregion
                }

            }
            #endregion
            return flag;
        }
        private bool CheckInPolygon(Domain.Common.Polygon latlng, string StationID)
        {
            bool flag = false;
            StationAndCarRepository _repository = new StationAndCarRepository(connetStr);

            List<GetPolygonRawData> lstData = new List<GetPolygonRawData>();
            lstData = _repository.GetPolygonRaws(StationID);
            bool polygonFlag = false;
            int DataLen = lstData.Count;
            PolygonModel pm = new PolygonModel();
            for (int i = 0; i < DataLen; i++)
            {

                string[] tmpLonGroup = lstData[i].Longitude.Split('⊙');
                string[] tmpLatGroup = lstData[i].Longitude.Split('⊙');
                int tmpLonGroupLen = tmpLonGroup.Length;


                for (int j = 0; j < tmpLonGroupLen; j++)
                {
                    string tmpData = "";
                    string[] tmpLon = tmpLonGroup[j].Split(',');
                    string[] tmpLat = tmpLatGroup[j].Split(',');
                    int LonLen = tmpLon.Length;
                    List<Domain.Common.Polygon> polygonGroups = new List<Domain.Common.Polygon>();
                    for (int k = 0; k < LonLen; k++)
                    {
                        polygonGroups.Add(new Domain.Common.Polygon()
                        {
                            Latitude = Convert.ToDouble(tmpLat[k]),
                            Longitude = Convert.ToDouble(tmpLon[k])
                        });
                    }

                    polygonFlag = pm.isInPolygonNew(ref polygonGroups, latlng);
                    if (polygonFlag)
                    {
                        if (lstData[i].PolygonMode == 0)
                        {
                            break;
                        }
                        else
                        {
                            polygonFlag = false;
                            break;
                        }
                    }



                }


            }
            flag = polygonFlag;
            return flag;
        }
    }
}