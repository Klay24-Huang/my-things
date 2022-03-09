using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.Member;
using Domain.SP.Input.Register;
using Domain.SP.Output;
using Domain.SP.Output.Member;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 註冊會員資料
    /// </summary>
    public class RegisterMemberDataController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string key = ConfigurationManager.AppSettings["apikey"].ToString();
        private string salt = ConfigurationManager.AppSettings["salt"].ToString();
        /// <summary>
        /// 註冊寫入會員資料
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doRegisterMemberData(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "RegisterMemberDataController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_RegisterMemberData apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            DateTime Birth = DateTime.Now;
            string Contentjson = "";
            string FileName = "";
            int MEMRFNBR = 0;
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_RegisterMemberData>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.IDNO, apiInput.MEMCNAME, apiInput.MEMBIRTH, apiInput.DeviceID, apiInput.APPVersion, apiInput.MEMADDR, apiInput.MEMEMAIL, apiInput.Signture };
                string[] errList = { "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (flag)
                {
                    //2.判斷格式
                    flag = baseVerify.checkIDNO(apiInput.IDNO);
                    if (false == flag)
                    {
                        errCode = "ERR103";
                    }
                    //2.1判斷生日格式
                    if (flag)
                    {
                        flag = DateTime.TryParse(apiInput.MEMBIRTH, out Birth);
                        if (false == flag)
                        {
                            errCode = "ERR107";
                        }
                    }
                    //2.2判斷行政區id
                    if (flag)
                    {
                        if (apiInput.AreaID == 0)
                        {
                            flag = false;
                            errCode = "ERR108";
                        }
                    }
                }
                if (flag)
                {
                    if (apiInput.APPVersion.Split('.').Count() < 3)
                    {
                        flag = false;
                        errCode = "ERR104";
                    }
                }
                if (flag)
                {
                    flag = (apiInput.APP.HasValue);
                    if (false == flag)
                    {
                        errCode = "ERR900";
                    }
                    else
                    {
                        APPKind = apiInput.APP.Value;
                        if (APPKind < 0 || APPKind > 1)
                        {
                            flag = false;
                            errCode = "ERR105";
                        }
                    }
                }
            }
            #endregion
            #region Azure Storage(上傳簽名檔)
            if (flag)
            {
                try
                {
                    FileName = string.Format("{0}_{1}_{2}.png", apiInput.IDNO, "Signture", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.Signture, FileName, "credential");
                }
                catch (Exception ex)
                {
                    flag = false;
                    errCode = "ERR226";
                }
            }
            #endregion
            #region TB
            #region 判斷短租流水號
            if (flag)
            {
                string spName = "usp_GetMemberData";
                SPInput_GetMemberData spInput = new SPInput_GetMemberData
                {
                    IDNO = apiInput.IDNO,
                    Token = "",
                    CheckToken = 0,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetMemberData, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetMemberData, SPOutput_Base>(connetStr);
                List<RegisterData> ListOut = new List<RegisterData>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref ListOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    if (ListOut.Count > 0)
                    {
                        flag = int.TryParse(ListOut.FirstOrDefault().MEMRFNBR.Replace("ir", ""), out MEMRFNBR);
                    }
                }
            }
            #endregion
            #region 取得MEMRFNBR
            //20201125 ADD BY ADAM REASON.寫入帳號前先去短租那邊取得MEMRFNBR
            if (flag && MEMRFNBR == 0)
            {
                WebAPIOutput_NPR013Reg wsOutput = new WebAPIOutput_NPR013Reg();
                HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                flag = wsAPI.NPR013Reg(apiInput.IDNO, apiInput.MEMCNAME, ref wsOutput);
                if (flag)
                {
                    if (wsOutput.Data.Length > 0)
                    {
                        MEMRFNBR = wsOutput.Data[0].MEMRFNBR == "" ? 0 : MEMRFNBR_FromStr(wsOutput.Data[0].MEMRFNBR);
                    }
                }
            }
            #endregion
            #region 基本資料存檔
            if (flag)
            {
                string spName = "usp_RegisterMemberData";
                SPInput_RegisterMemberData spInput = new SPInput_RegisterMemberData()
                {
                    IDNO = apiInput.IDNO,
                    DeviceID = apiInput.DeviceID,
                    MEMCNAME = apiInput.MEMCNAME,
                    MEMBIRTH = Birth,
                    MEMCITY = apiInput.AreaID,
                    MEMADDR = apiInput.MEMADDR,
                    MEMEMAIL = apiInput.MEMEMAIL,
                    FileName = FileName,
                    MEMRFNBR = MEMRFNBR,     //20201126 ADD BY ADAM REASON.增加短租流水號
                    LogID = LogID,
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_RegisterMemberData, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_RegisterMemberData, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            }
            #endregion
            #region 會員同意條款存檔
            // 20210812 UPD BY YEH REASON:增加會員同意條款存檔
            if (flag)
            {
                string spName = "usp_SetMemberCMK";
                SPInput_SetMemberCMK spInput = new SPInput_SetMemberCMK()
                {
                    IDNO = apiInput.IDNO,
                    VerType = "Hims",
                    Version = "",   // 從DB抓
                    SeqNo = 1,
                    Source = "I",
                    AgreeDate = DateTime.Now,
                    TEL = "Y",      // 預設Y
                    SMS = "Y",      // 預設Y
                    EMAIL = "Y",    // 預設Y
                    POST = "Y",     // 預設Y
                    APIName = funName,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_SetMemberCMK, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_SetMemberCMK, SPOutput_Base>(connetStr);
                List<MemberCMKList> ListOut = new List<MemberCMKList>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref ListOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    // 20210825 UPD BY YEH REASON:拋短租
                    // 20211105 UPD BY YEH REASON:改成迴圈拋多筆
                    // 20211214 UPD BY YEH REASON:註冊不拋短租
                    //foreach (var list in ListOut)
                    //{
                    //    if (flag)
                    //    {
                    //        WebAPIInput_TransIRentMemCMK wsInput = new WebAPIInput_TransIRentMemCMK
                    //        {
                    //            IDNO = list.MEMIDNO,
                    //            VERTYPE = list.VerType,
                    //            VER = list.Version,
                    //            VERSOURCE = list.Source,
                    //            TEL = list.TEL,
                    //            SMS = list.SMS,
                    //            EMAIL = list.EMAIL,
                    //            POST = list.POST,
                    //            MEMO = "",
                    //            COMPID = "EF",
                    //            COMPNM = "和雲",
                    //            PRGID = "iRent_6",
                    //            USERID = "iRentUser"
                    //        };
                    //        WebAPIOutput_TransIRentMemCMK wsOutput = new WebAPIOutput_TransIRentMemCMK();
                    //        HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();

                    //        flag = hiEasyRentAPI.TransIRentMemCMK(wsInput, ref wsOutput);
                    //        if (flag == false)
                    //        {
                    //            errCode = "ERR776";
                    //        }
                    //    }
                    //}
                }
            }
            #endregion
            #endregion
            #region 產生加密及發信
            if (flag)
            {
                SendMail send = new SendMail();
                string Source = new AESEncrypt().doEncrypt(key, salt, apiInput.IDNO + "⊙" + apiInput.MEMEMAIL);
                string Title = "iRent會員電子信箱認證通知信";

                string url = "https://irentcar.azurefd.net/api/VerifyEMail?VerifyCode=" + Source;

                string Body =
                    "<img src='https://irentv2-as-verify.azurewebsites.net/images/irent.png'>" +
                    "<p>親愛的會員您好：</p>" +
                    "<p>請點擊下方連結完成電子信箱認證</p>" +
                    "<p><a href='" + url + "'>請按這裡</a></p>" +
                    "<p>不是您本人嗎?請直接忽略或刪除此信件，</p>" +
                    "<p>如有問題請撥打客服專線0800-024-550，謝謝！</p>" +
                    "<img src='https://irentv2-as-verify.azurewebsites.net/images/hims_logo.png' width='300'>";

                // flag = Task.Run(() => send.DoSendMail(Title, Body, apiInput.MEMEMAIL)).Result;
                flag = send.DoSendMail(Title, Body, apiInput.MEMEMAIL);
            }
            #endregion
            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }

        private int MEMRFNBR_FromStr(string sour)
        {
            if (double.TryParse(sour, out double d_sour))
                return Convert.ToInt32(Math.Floor(d_sour));
            else
                throw new Exception("MEMRFNBR格式錯誤");
        }
    }
}