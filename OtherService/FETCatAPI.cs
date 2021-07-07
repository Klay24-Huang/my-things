﻿using Domain.SP.Input.OtherService.FET;
using Domain.SP.Output;
using Domain.TB;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Newtonsoft.Json;
using NLog;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebCommon;

namespace OtherService
{
    /// <summary>
    /// 遠傳CAT平台
    /// </summary>
    public class FETCatAPI
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private string BasePath = ConfigurationManager.AppSettings.Get("CatBaseURL");
        private string SyncPath = ConfigurationManager.AppSettings.Get("CatSyncURL");
        private string[] SyncCommand = ConfigurationManager.AppSettings.Get("CatSyncCommand").Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string catCombineCmdAllSupport = ConfigurationManager.AppSettings["CatCombineCmdAllSupport"] ?? "";
        private string catCombineCmdSupportFwVer = ConfigurationManager.AppSettings["CatCombineCmdSupportFwVer"] ?? "0";
        public bool DoSendCmd(string deviceToken,string CID,OtherService.Enum.MachineCommandType.CommandType commandType,WSInput_Base<Params> Input, Int64 LogID)
        {
            bool flag = false;
            //寫入送出記錄
            //string URL = string.Format(BasePath, deviceToken);
            string URL = GetUrl(deviceToken, commandType.ToString());
            string inputStr = JsonConvert.SerializeObject(Input).Replace("_params", "params");
            SPInput_InsSendCMD spInput = new SPInput_InsSendCMD()
            {
                CID = CID,
                deviceToken = deviceToken,
                LogID = LogID,
                method = commandType.ToString(),
                requestId = Input.requestId,
                SendParams = inputStr
            };
            var result = doSendCMD(inputStr, URL, spInput).Result;
            flag = Convert.ToBoolean(result);
            return flag;
        }
        /// <summary>
        /// 設定顧客卡號
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="CID"></param>
        /// <param name="commandType"></param>
        /// <param name="Input"></param>
        /// <returns></returns>
        public bool DoSendCmd(string deviceToken, string CID, OtherService.Enum.MachineCommandType.CommandType commandType, WSInput_Base<ClientCardNoObj> Input, Int64 LogID)
        {
            bool flag = false;
            //寫入送出記錄
            //string URL = string.Format(BasePath, deviceToken);
            string URL = GetUrl(deviceToken, commandType.ToString());
            string inputStr = JsonConvert.SerializeObject(Input).Replace("_params", "params");
            SPInput_InsSendCMD spInput = new SPInput_InsSendCMD()
            {
                CID = CID,
                deviceToken = deviceToken,
                LogID = LogID,
                method = commandType.ToString(),
                requestId = Input.requestId,
                SendParams = inputStr
            };
            var result = doSendCMD(inputStr, URL,spInput).Result;
            flag = Convert.ToBoolean(result);
            return flag;
        }
        /// <summary>
        /// 設定萬用卡號
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="CID"></param>
        /// <param name="commandType"></param>
        /// <param name="Input"></param>
        /// <returns></returns>
        public bool DoSendCmd(string deviceToken, string CID, OtherService.Enum.MachineCommandType.CommandType commandType, WSInput_Base<UnivCardNoObj> Input,Int64 LogID)
        {
            bool flag = false;
            //寫入送出記錄
            //string URL = string.Format(BasePath, deviceToken);
            string URL = GetUrl(deviceToken, commandType.ToString());
            string inputStr = JsonConvert.SerializeObject(Input).Replace("_params", "params");
            SPInput_InsSendCMD spInput = new SPInput_InsSendCMD()
            {
                CID = CID,
                deviceToken = deviceToken,
                LogID = LogID,
                method = commandType.ToString(),
                requestId = Input.requestId,
                SendParams = inputStr
            };
            var result = doSendCMD(inputStr, URL,spInput).Result;
            flag = Convert.ToBoolean(result);
            return flag;
        }
        /// <summary>
        /// 機車設定租約
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="CID"></param>
        /// <param name="commandType"></param>
        /// <param name="Input"></param>
        /// <param name="LogID"></param>
        /// <returns></returns>
        public bool DoSendCmd(string deviceToken, string CID, OtherService.Enum.MachineCommandType.CommandType commandType, WSInput_Base<BLECode> Input, Int64 LogID)
        {
            bool flag = false;
            //寫入送出記錄
            //string URL = string.Format(BasePath, deviceToken);
            string URL = GetUrl(deviceToken, commandType.ToString());
            string inputStr = JsonConvert.SerializeObject(Input).Replace("_params", "params");
            SPInput_InsSendCMD spInput = new SPInput_InsSendCMD()
            {
                CID = CID,
                deviceToken = deviceToken,
                LogID = LogID,
                method = commandType.ToString(),
                requestId = Input.requestId,
                SendParams = inputStr
            };
            var result = doSendCMD(inputStr, URL, spInput).Result;
            flag = Convert.ToBoolean(result);
            return flag;
        }
        public bool DoWaitReceive(string requestId, string method,ref string errCode)
        {
            //bool flag = true;
            int nowCount = 0;
            bool waitFlag = false;
            //while (nowCount < 30)
            while (nowCount < 25)  //20201204 ADD BY ADAM 改為20秒
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
                    Thread.Sleep(1000);
                    break;
                }
                nowCount++;
            }

            if (waitFlag == false)
            {
                if (errCode != "ERR167")
                {
                    //      flag = false;
                    errCode = "ERR166";
                }

            }
            else
            {
                errCode = "000000";
            }
            
            return waitFlag;
        }
        private async Task<Boolean> doSendCMD(string input, string URL, SPInput_InsSendCMD spInput)
        {
            bool flag = false;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;
         
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string postBody = input;//將匿名物件序列化為json字串
                byte[] byteArray = Encoding.UTF8.GetBytes(postBody);//要發送的字串轉為byte[]

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }



                //發出Request
                string responseStr = "";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    if (response.StatusCode.ToString() == "OK")
                    {
                        flag = true;
                    }
                    spInput.HttpStatus = response.StatusCode.ToString();

                }
            }
            catch (Exception ex)
            {
                logger.Error(String.Format("{0} send fail: {1}", spInput.requestId, ex.Message));
                spInput.HttpStatus = "exception";
            }
            finally
            {
                SPOutput_Base spout = new SPOutput_Base();
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                string SPName = new OtherService.Enum.ObjType().GetSPName(Enum.ObjType.SPType.InsSendCMD);
             
             
                SQLHelper<SPInput_InsSendCMD, SPOutput_Base> SQLCancelHelper = new SQLHelper<SPInput_InsSendCMD, SPOutput_Base>(connetStr);
                flag = SQLCancelHelper.ExecuteSPNonQuery(SPName, spInput, ref spout, ref lstError);
            }
            return flag;
        }

        private string GetUrl(string deviceToken, string method)
        {
            string url = "";
            if (SyncCommand.Contains(method))
            {
                url = string.Format(SyncPath, deviceToken);
            }
            else
            {
                url = string.Format(BasePath, deviceToken);
            }
            return url;
        }
        /// <summary>
        /// 判斷能否使用巨集功能
        /// </summary>
        /// <param name="cid">CID</param>
        /// <returns></returns>
        public bool IsSupportCombineCmd(string cid)
        {
            bool support = false;
            bool flag = false;
            try
            {
                if (catCombineCmdAllSupport == "1")
                {
                    support = true;
                }
                else
                {
                    CarCMDRepository CarCMDRepository = new CarCMDRepository(connetStr);
                    CarCmdData carCmdData = CarCMDRepository.GetCarCMDDataByCID(cid, ref flag);
                    if (!string.IsNullOrEmpty(carCmdData.CensFWVer))
                    {
                        support = chkCatFwVer(carCmdData.CensFWVer);
                    }
                }
            }
            catch
            {
            }
            return support;
        }
        /// <summary>
        /// 檢查遠傳車機韌體版本是否支援新功能
        /// </summary>
        /// <param name="fwver"></param>
        /// <returns></returns>
        public bool chkCatFwVer(string fwver)
        {
            bool support = false;
            try
            {
                string version = "0";
                Regex regex = new Regex(@".+-(\d{8})");
                Match match = regex.Match(fwver);
                if (match.Success)
                {
                    version = match.Groups[1].Value;
                }
                if (Convert.ToInt32(version) >= Convert.ToInt32(catCombineCmdSupportFwVer))
                {
                    support = true;
                }
            }
            catch
            {
            }
            return support;
        }
    }
}
