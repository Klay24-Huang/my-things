using Domain.SP.Input.OtherService.FET;
using Domain.SP.Output;
using Domain.TB;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Newtonsoft.Json;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
        private string BasePath = ConfigurationManager.AppSettings.Get("CatBaseURL");
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public bool DoSendCmd(string deviceToken,string CID,OtherService.Enum.MachineCommandType.CommandType commandType,WSInput_Base<Params> Input, Int64 LogID)
        {
            bool flag = false;
            //寫入送出記錄
            string URL = string.Format(BasePath, deviceToken);
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
            string URL = string.Format(BasePath, deviceToken);
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
            string URL = string.Format(BasePath, deviceToken);
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
            string URL = string.Format(BasePath, deviceToken);
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
            while (nowCount < 10)  //測試方便先等10秒  20201028 ADD BY ADAM 
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
            //20201028車機無回應時設定為成功，以狀態檔為主
            waitFlag = true;
            if (waitFlag == false)
            {
                if (errCode != "ERR167")
                {
              //      flag = false;
                    errCode = "ERR166";
                }

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
    }
}
