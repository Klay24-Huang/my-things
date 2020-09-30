﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Output.CENS;
using WebCommon;
using System.Net;
using Domain.SP.Output;
using System.IO;
using Domain.SP.Input.OtherService.Common;
using Domain.SP.Input.OtherService.CENS;
using OtherService.Common;

namespace OtherService
{
    /// <summary>
    /// 興聯車機
    /// </summary>
    public class CensWebAPI
    {
        private string BasePath = ConfigurationManager.AppSettings.Get("CENSBaseURL");
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        /// <summary>
        /// 2.1取得即時狀態
        /// </summary>
        /// <param name="CID"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool GetInfo(string CID, ref WSOutput_GetInfo output)
        {
            bool flag = true;

            string Name = "GetInfo";
            string URL = BasePath + Name;
            Dictionary<string, object> input = new Dictionary<string, object>();
            WSInput_Base wsInput = new WSInput_Base()
            {
                CID = CID
            };
            input.Add("para", wsInput);
            output = doSendCmd(JsonConvert.SerializeObject(input), URL, Name).Result;
            if (output.Result == 1)
            {
                flag = false;
            }
            return flag;

        }
        /// <summary>
        /// 設定/解除租約
        /// </summary>
        /// <param name="CID"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool SetOrderStatus(WSInput_SetOrderStatus input, ref WSOutput_Base output)
        {
            bool flag = true;

            string Name = "SetOrderStatus";
            string URL = BasePath + Name;
            Dictionary<string, object> inputObj = new Dictionary<string, object>();
            inputObj.Add("para", input);
            output = doSendCMD(JsonConvert.SerializeObject(inputObj), URL, Name).Result;
            if (output.Result == 1)
            {
                flag = false;
            }
            return flag;

        }
        /// <summary>
        /// 發送卡號
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool SendCardNo(WSInput_SendCardNo input, ref WSOutput_Base output)
        {
            bool flag = true;
            string Name = "SendCardNo";
            string URL = BasePath + Name;

            Dictionary<string, object> inputObj = new Dictionary<string, object>();
            inputObj.Add("para", input);
            output = doSendCMD(JsonConvert.SerializeObject(inputObj), URL, Name).Result;
            if (output.Result == 1)
            {
                flag = false;
            }
            return flag;

        }
        /// <summary>
        /// 上解鎖
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool SendLock(WSInput_SendLock input, ref WSOutput_Base output)
        {
            bool flag = true;
            string Name = "SendLock";
            string URL = BasePath + Name;

            Dictionary<string, object> inputObj = new Dictionary<string, object>();
            inputObj.Add("para", input);
            output = doSendCMD(JsonConvert.SerializeObject(inputObj), URL, Name).Result;
            if (output.Result == 1)
            {
                flag = false;
            }
            return flag;

        }
        /// <summary>
        /// 尋車
        /// </summary>
        /// <param name="CID"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool SearchCar(string CID, ref WSOutput_Base output)
        {
            bool flag = true;
            string Name = "SearchCar";
            string URL = BasePath + Name;
            WSInput_Base wsInput = new WSInput_Base()
            {
                CID = CID
            };
            Dictionary<string, object> inputObj = new Dictionary<string, object>();
            inputObj.Add("para", wsInput);
            output = doSendCMD(JsonConvert.SerializeObject(inputObj), URL, Name).Result;
            if (output.Result == 1)
            {
                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// 設定車機重置
        /// </summary>
        /// <param name="CID"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool SoftwareReset(string CID, ref WSOutput_Base output)
        {
            bool flag = true;
            string Name = "SoftwareReset";
            string URL = BasePath + Name;
            WSInput_Base wsInput = new WSInput_Base()
            {
                CID = CID
            };
            Dictionary<string, object> inputObj = new Dictionary<string, object>();
            inputObj.Add("para", wsInput);
            output = doSendCMD(JsonConvert.SerializeObject(inputObj), URL, Name).Result;
            if (output.Result == 1)
            {
                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// NFC電源控制
        /// </summary>
        /// <param name="CID"></param>
        /// <param name="Mode">
        /// <para>0:關閉</para>
        /// <para>1:開啟</para>
        /// </param>
        /// <param name="LogID">
        /// 
        /// </param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool NFCPower(string CID, int Mode,Int64 LogID, ref WSOutput_Base output)
        {
            bool flag = true;
            string Name = "NFCPower";
            string URL = BasePath + Name;
            WSInput_NFCPower wsInput = new WSInput_NFCPower()
            {
                CID = CID,
                CMD = Mode
                 
            };
            Dictionary<string, object> inputObj = new Dictionary<string, object>();
            inputObj.Add("para", wsInput);
            output = doSendCMD(JsonConvert.SerializeObject(inputObj), URL, Name).Result;
            if (output.Result == 1)
            {
                flag = false;
            }
            else
            {
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                string SPName = new OtherService.Enum.ObjType().GetSPName(Enum.ObjType.SPType.UpdNFCStatus);
                SPInput_UPDNFCStatus spInputCancel = new SPInput_UPDNFCStatus()
                {
                    CID = wsInput.CID,
                    Mode = wsInput.CMD,
                    LogID =LogID
                    
                };
                SPOutput_Base spOutCancel = new SPOutput_Base();
                SQLHelper<SPInput_UPDNFCStatus, SPOutput_Base> SQLCancelHelper = new SQLHelper<SPInput_UPDNFCStatus, SPOutput_Base>(connetStr);
                flag = SQLCancelHelper.ExecuteSPNonQuery(SPName, spInputCancel, ref spOutCancel, ref lstError);
            }
            return flag;
        }
        /// <summary>
        /// 2.2~2.5共用
        /// </summary>
        /// <param name="input"></param>
        /// <param name="URL"></param>
        /// <returns></returns>
        private async Task<WSOutput_Base> doSendCMD(string input, string URL, string Name)
        {
            WSOutput_Base output = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            try
            {
                string postBody = input;//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WSOutput_Base>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WSOutput_Base()
                {
                    Result = 1,
                    ErrorCode = "ERR996",
                    ErrMsg = "連結車機失敗"
                };

            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = input,
                    WebAPIName = Name,
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = URL
                };
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }
            return output;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="URL"></param>
        /// <returns></returns>

        private async Task<WSOutput_GetInfo> doSendCmd(string input, string URL, string Name)
        {
            WSOutput_GetInfo output = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
                string postBody = input;//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }



                //發出Request
                string responseStr = "";
                using (WebResponse response = request.GetResponse())
                {

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = reader.ReadToEnd();
                        RTime = DateTime.Now;
                        output = JsonConvert.DeserializeObject<WSOutput_GetInfo>(responseStr);
                    }

                }
            }
            catch (Exception ex)
            {
                RTime = DateTime.Now;
                output = new WSOutput_GetInfo()
                {
                    Result = 1,
                    ErrorCode = "ERR996",
                    ErrMsg = "連結車機失敗"
                };

            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = input,
                    WebAPIName = Name,
                    WebAPIOutput = JsonConvert.SerializeObject(output),
                    WebAPIURL = URL
                };
                //  SPOutput_Base spOut = new SPOutput_Base();
                bool flag = true;
                string errCode = "";
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }
            return output;
        }
    }
}