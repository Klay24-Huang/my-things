﻿using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.OrderList;
using Domain.SP.Output.Rent;
using Domain.TB;
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
using WebAPI.Models;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 還車起手式
    /// </summary>
    public class ReturnCarController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoBookingCancel(Dictionary<string, object> value)
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
            string funName = "ReturnCarController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_ReturnCar apiInput = null;
            NullOutput outputApi = new NullOutput();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            int IsCens = 0;
            int IsMotor = 0;
            string deviceToken = "";
            string StationID = "";
            string CID = "";
            
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_ReturnCar>(Contentjson);
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
            //Other rule
            //這邊也可以先做取車機，做完判斷再做訂單的判斷
            //開始做還車前檢查
            if (flag)
            {
                SPInput_ReturnCar spInput = new SPInput_ReturnCar()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.ReturnCar);
                SPOutput_ReturnCar spOut = new SPOutput_ReturnCar();
                SQLHelper<SPInput_ReturnCar, SPOutput_ReturnCar> sqlHelp = new SQLHelper<SPInput_ReturnCar, SPOutput_ReturnCar>(connetStr);
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
            }

            #endregion
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
                                    flag = CheckInPolygon(Nowlatlng,StationID);
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
        private bool CheckInPolygon(Domain.Common.Polygon latlng,string StationID)
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
                    for(int k = 0; k < LonLen; k++)
                    {
                        polygonGroups.Add(new Domain.Common.Polygon()
                        {
                            Latitude =  Convert.ToDouble(tmpLat[k]),
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