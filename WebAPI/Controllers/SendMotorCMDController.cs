using Domain.Common;
using Domain.TB;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
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
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 對機車下命令
    /// </summary>
    public class SendMotorCMDController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoSendMotorCmd(Dictionary<string, object> value)
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
            string funName = "SendMotorCMDController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
         
            CommonFunc baseVerify = new CommonFunc();
            Token token = null;
            string CID = "";
            string deviceToken = "";
            IAPI_SendMotorCMD apiInput = null;
            OAPI_Base outputApi = null;
            int CarMachineType = 0;

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string method = "", requestId = "";
            OtherService.Enum.MachineCommandType.CommandType CmdType = new OtherService.Enum.MachineCommandType.CommandType();
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_SendMotorCMD>(Contentjson);
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
                }
                if (flag)
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
               
                if (flag)
                {
                    if(apiInput.CmdType==0 && string.IsNullOrWhiteSpace(apiInput.BLE_Code))
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }
                if (flag)
                {
                    if (string.IsNullOrWhiteSpace(apiInput.UserId))
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }

            }

            #endregion
            #region 對車機下指令
            if (flag)
            {
                FETCatAPI FetAPI = new FETCatAPI();
                string CommandType = "";
                /// <para>0:設定租約狀態</para>
                ///  <para>1:解除租約狀態</para>
                /// <para>2:開啟電源</para>
                /// <para>3:關閉電源</para>
                /// <para>4:啟動喇叭尋車功能</para>
                /// <para>5:啟動閃燈尋車功能</para>
                /// <para>6:開啟坐墊</para>
                /// <para>7:開啟/關閉電池蓋</para>

                switch (apiInput.CmdType)
                {
                    case 0: //0:設定租約狀態
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetMotorcycleRent);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SetMotorcycleRent;
                        break;
                    case 1: //1:解除租約狀態
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetNoRent);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SetNoRent;
                        break;
                    case 2: //2:開啟電源
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOn);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOn;
                        break;
                    case 3: //3:關閉電源
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOff);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SwitchPowerOff;
                        break;
                    case 4: //4:啟動喇叭尋車功能
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetHornOn);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SetHornOn;
                        break;
                    case 5: //5:啟動閃燈尋車功能
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetLightFlash);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SetLightFlash;
                        break;
                    case 6: //6:開啟坐墊
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.OpenSet);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.OpenSet;
                        break;
                    case 7: //7:開啟/關閉電池蓋
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetBatteryCap);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SetBatteryCap;
                        break;
                    case 99: //99:ReportNow
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ReportNow);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.ReportNow;
                        break;
                }
                if (apiInput.CmdType == 0)
                {
                    WSInput_Base<BLECode> input = new WSInput_Base<BLECode>()
                    {
                        command = true,
                        method = CommandType,
                        requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                        _params = new BLECode()

                    };
                    input._params.BLE_Code = apiInput.BLE_Code;
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
            #endregion
            #region 判斷是否成功
            //20201020 MARK BY JERRY 無連續動作，可以先不執行等待
            //if (flag)
            //{
            //    if (CarMachineType == 0)
            //    {
            //        int nowCount = 0;
            //        bool waitFlag = false;
            //        while (nowCount < 30)
            //        {
            //            Thread.Sleep(1000);
            //            CarCMDResponse obj = new CarCMDRepository(connetStr).GetCMDData(requestId, method);
            //            if (obj != null)
            //            {
            //                waitFlag = true;
            //                if (obj.CmdReply != "Okay")
            //                {
            //                    waitFlag = false;
            //                    errCode = "ERR167";
            //                    //    break;
            //                }
            //                break;
            //            }
            //            nowCount++;
            //        }
            //        if (waitFlag == false)
            //        {
            //            if (errCode != "ERR167")
            //            {
            //                flag = false;
            //                errCode = "ERR166";
            //            }

            //        }
            //    }
            //}

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
