using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.Common;
using Domain.SP.Input.HiEasyRent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.HiEasyRent;
using Domain.TB;
using Domain.TB.BackEnd;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using Domain.WebAPI.output.Taishin;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.ComboFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.BackEnd.Output;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】強制取款
    /// </summary>
    public class BE_AutoPayMoneyController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoBE_AutoPayMoney([FromBody] Dictionary<string, object> value)
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
            string funName = "BE_AutoPayMoneyController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_AutoPayMoney apiInput = null;
            NullOutput outputApi = new NullOutput();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<Models.Param.Output.PartOfParam.CreditCardBindList> lstBindList = null;
            List<AutoPayMoneyData> TmpData = new List<AutoPayMoneyData>();
            int HasBind = 0;
            string UserName = "";
            string EMail = "";
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            RegisterData registerData = null;
            string htmlBody = "";
            string IDNO = "";

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_AutoPayMoney>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.CUSTID))
                {
                    flag = false;
                    errCode = "ERR900";
                }

                if (flag)
                {
                    //2.判斷格式
                    flag = baseVerify.checkIDNO(apiInput.CUSTID);
                    if (false == flag)
                    {
                        errCode = "ERR103";
                    }
                }
                if (flag)
                {
                    if (apiInput.Data == null)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else
                    {
                        if (apiInput.Data.Count == 0)
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                    }
                }

            }

            #endregion
            #region TB
            //先查詢是否有綁卡
            if (flag)
            {
                IDNO = apiInput.CUSTID;
                registerData = new MemberRepository(connetStr).GetMemberData(IDNO);
                if (registerData != null)
                {
                    UserName = registerData.MEMCNAME;
                    EMail = registerData.MEMEMAIL;
                }
                flag = new CreditAuthComm().DoQueryCardList(IDNO,ref HasBind, ref lstBindList, ref errCode, ref errMsg);
                if (HasBind==0) {
                    flag = false;
                    errCode = "ERR762";
                }

            }
            //順序為送台新取款、送短租、更新TB
            if (flag)
            {
               // WebAPIOutput_NPR270Query wsOutput = new WebAPIOutput_NPR270Query();

                HiEasyRentAPI WebAPI = new HiEasyRentAPI();

                int len = apiInput.Data.Count;
                if (len > 0)
                {
                    int totalAMT = 0;
             
                    for (int i = 0; i < len; i++)
                    {
                        SPInput_HandleNPR340Save spInput = new SPInput_HandleNPR340Save()
                        {
                            AMOUNT = apiInput.Data[i].TAMT.ToString(),
                            CARNO = apiInput.Data[i].CARNO,
                            CNTRNO = apiInput.Data[i].CNTRNO,
                            CUSTID = apiInput.CUSTID,
                            ORDNO = apiInput.Data[i].ORDNO,
                            POLNO = apiInput.Data[i].POLNO,
                            PAYMENTTYPE = apiInput.Data[i].PAYMENTTYPE.ToString(),
                            LogID = LogID
                        };
                        int hasPaid = DoCheck(spInput, ref flag, ref errCode, ref errMsg, ref lstError);
                        if (hasPaid == 0)
                        {
                            totalAMT += apiInput.Data[i].TAMT;
                            TmpData.Add(apiInput.Data[i]);
                        }
                       
                      
                    }
                    if (totalAMT > 0)
                    {
                        WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();
                        flag = new CreditAuthComm().DoAuth(IDNO, totalAMT, lstBindList[0].CardToken, 3, ref errCode, ref errMsg, ref WSAuthOutput);
                        if (flag)
                        {

                            HiEasyRentAPI webAPI = new HiEasyRentAPI();
                            WebAPIInput_NPR340Save wsInput = null;
                            WebAPIOutput_NPR340Save wsOutput = new WebAPIOutput_NPR340Save();
                            wsInput = new WebAPIInput_NPR340Save()
                            {

                                tbNPR340SaveServiceVar = new List<NPR340SaveServiceVar>(),
                                tbNPR340PaymentDetail = new List<NPR340PaymentDetail>()
                            };
                             len = TmpData.Count;
                            for (int i = 0; i < len; i++)
                            {
                                htmlBody += "<tr><td>" + apiInput.Data[i].IRENTORDNO + "</td><td>" + apiInput.Data[i].SPAYMENTTYPE + "</td><td>" + apiInput.Data[i].SPAYMENTTYPE + "</td><td>" + apiInput.Data[i].TAMT + "</td></tr>";
                                SPInput_HandleNPR340SaveU1 SPInput = new SPInput_HandleNPR340SaveU1()
                                {
                                    AMOUNT = TmpData[i].TAMT.ToString(),
                                    AUTH_CODE = WSAuthOutput.ResponseParams.ResultData.AuthIdResp,
                                    CARDNO = lstBindList[0].CardNumber,
                                    CARNO = TmpData[i].CARNO,
                                    CNTRNO = TmpData[i].CNTRNO,
                                    CUSTID = apiInput.CUSTID,
                                    ORDNO = TmpData[i].ORDNO,
                                    POLNO = TmpData[i].POLNO,
                                    PAYMENTTYPE = TmpData[i].PAYMENTTYPE.ToString(),
                                    PAYDATE = DateTime.Now.ToString("yyyyMMdd"),
                                    NORDNO = WSAuthOutput.ResponseParams.ResultData.ServiceTradeNo,
                                    CDTMAN = UserName,
                                    LogID = LogID,
                                    MerchantTradeNo = WSAuthOutput.OriRequestParams.RequestParams.MerchantTradeNo,
                                    ServerTradeNo = WSAuthOutput.ResponseParams.ResultData.ServiceTradeNo
                                };

                                wsInput.tbNPR340SaveServiceVar.Add(new NPR340SaveServiceVar()
                                {
                                    AMOUNT = TmpData[i].TAMT.ToString(),
                                    AUTH_CODE = WSAuthOutput.ResponseParams.ResultData.AuthIdResp,
                                    CARDNO = lstBindList[0].CardNumber,
                                    CARNO = TmpData[i].CARNO,
                                    CNTRNO = TmpData[i].CNTRNO,
                                    CUSTID = apiInput.CUSTID,
                                    ORDNO = TmpData[i].ORDNO,
                                    POLNO = TmpData[i].POLNO,
                                    PAYMENTTYPE = TmpData[i].PAYMENTTYPE,
                                    PAYDATE = DateTime.Now.ToString("yyyyMMdd"),
                                    NORDNO = WSAuthOutput.ResponseParams.ResultData.ServiceTradeNo,

                                    CDTMAN = UserName

                                });
                                wsInput.tbNPR340PaymentDetail.Add(new NPR340PaymentDetail()
                                {
                                    CNTRNO = TmpData[i].CNTRNO,
                                    PAYAMT = TmpData[i].TAMT.ToString(),
                                    PAYMENTTYPE = TmpData[i].PAYMENTTYPE.ToString(),
                                    PAYMEMO = TmpData[i].SPAYMENTTYPE,
                                    PORDNO = TmpData[i].IRENTORDNO,
                                    PAYTCD = "1"
                                });
                                DoSave(SPInput, ref flag, ref errCode, ref errMsg, ref lstError);
                            }
                            flag = webAPI.NPR340Save(wsInput, ref wsOutput);
                            if (flag)
                            {
                                SPInput_HandleNPR340SaveU2 spInput = new SPInput_HandleNPR340SaveU2()
                                {
                                    isRetry = 0,
                                    LogID = LogID,
                                    MerchantTradeNo = WSAuthOutput.OriRequestParams.RequestParams.MerchantTradeNo,
                                    ServerTradeNo = WSAuthOutput.ResponseParams.ResultData.ServiceTradeNo
                                };
                                DoUPD(spInput, ref flag, ref errCode, ref errMsg, ref lstError);
                            }
                            else
                            {
                                errCode = "ERR764";

                            }

                        }
                        #region 發mail
                        if ((flag && htmlBody != "") || (flag == false && errCode == "ERR764"))
                        {
                            string title = "iRent刷卡通知";
                            string body = "親愛的會員您好\n\n";
                            body += "感謝您對iRent的支持！和雲行動服務自您的信用卡帳號取款<u>" + totalAMT + "</u>元，詳細合約內容及收\n費明細如下表。若您仍有其他帳務問題，請洽客服專線0800-024-550。";
                            body += "<table><thead style=\"background-color:yellow\"><tr><td>iRent合約編號</td><td>類別</td><td>說明</td><td>金額</td></tr></thead>";
                            if (htmlBody != "")
                            {
                                body += "<body>" + htmlBody + "</body></html>";
                            }
                            if (registerData.MEMEMAIL != "")
                            {
                                bool flag2 = SendMail(title, body, registerData.MEMEMAIL, "");
                            }

                        }
                        #endregion
                    }
                    else
                    {
                        //直接沖銷
                        List<BE_NPR340Retry> lstNPR340 = new HiEasyRentRepository(connetStr).GetNPR340RetryByID(apiInput.CUSTID);
                        if (lstNPR340 != null)
                        {
                            if (lstNPR340.Count > 0)
                            {
                                int trueLen = apiInput.Data.Count;
                                HiEasyRentAPI webAPI = new HiEasyRentAPI();
                                WebAPIInput_NPR340Save wsInput = null;
                                WebAPIOutput_NPR340Save wsOutput = new WebAPIOutput_NPR340Save();
                                string MerchantTradeNo = "";
                                string ServiceTradeNo = "";
                                wsInput = new WebAPIInput_NPR340Save()
                                {
                                    tbNPR340SaveServiceVar = new List<NPR340SaveServiceVar>(),
                                    tbNPR340PaymentDetail = new List<NPR340PaymentDetail>()
                                };
                                for (int i = 0; i < len; i++)
                                {
                                    int Index = lstNPR340.FindIndex(delegate (BE_NPR340Retry t) {
                                        return t.AMOUNT == apiInput.Data[i].TAMT.ToString() && t.CNTRNO == apiInput.Data[i].CNTRNO && t.ORDNO == apiInput.Data[i].ORDNO && t.PAYMENTTYPE == apiInput.Data[i].PAYMENTTYPE.ToString() && t.POLNO == apiInput.Data[i].POLNO && t.CARNO == apiInput.Data[i].CARNO && t.CDTMAN == UserName;

                                    });
                                    if (Index > -1)
                                    {
                                        MerchantTradeNo = lstNPR340[Index].MerchantTradeNo;
                                        ServiceTradeNo = lstNPR340[Index].ServerTradeNo;
                                        wsInput.tbNPR340SaveServiceVar.Add(new NPR340SaveServiceVar()
                                        {
                                            AMOUNT = lstNPR340[Index].AMOUNT.ToString(),
                                            AUTH_CODE = lstNPR340[Index].AUTH_CODE,
                                            CARDNO = lstNPR340[Index].CARDNO,
                                            CARNO = lstNPR340[Index].CARNO,
                                            CNTRNO = lstNPR340[Index].CNTRNO,
                                            CUSTID = lstNPR340[Index].CUSTID,
                                            ORDNO = lstNPR340[Index].ORDNO,
                                            POLNO = lstNPR340[Index].POLNO,
                                            PAYMENTTYPE = Convert.ToInt64(lstNPR340[Index].PAYMENTTYPE),
                                            PAYDATE = lstNPR340[Index].PAYDATE,
                                            NORDNO = lstNPR340[Index].ServerTradeNo,

                                            CDTMAN = lstNPR340[Index].CDTMAN

                                        });
                                        wsInput.tbNPR340PaymentDetail.Add(new NPR340PaymentDetail()
                                        {
                                            CNTRNO = lstNPR340[Index].CNTRNO,
                                            PAYAMT = lstNPR340[Index].AMOUNT.ToString(),
                                            PAYMENTTYPE = lstNPR340[Index].PAYMENTTYPE.ToString(),
                                            PAYMEMO = apiInput.Data[i].SPAYMENTTYPE,
                                            PORDNO = apiInput.Data[i].IRENTORDNO,
                                            PAYTCD = "1"
                                        });
                                    }
                                }
                                flag = webAPI.NPR340Save(wsInput, ref wsOutput);
                                if (flag)
                                {
                                    SPInput_HandleNPR340SaveU2 spInput = new SPInput_HandleNPR340SaveU2()
                                    {
                                        isRetry = 0,
                                        LogID = LogID,
                                        MerchantTradeNo = MerchantTradeNo,
                                        ServerTradeNo = ServiceTradeNo
                                    };
                                    DoUPD(spInput, ref flag, ref errCode, ref errMsg, ref lstError);
                                }
                                else
                                {
                                    errCode = "ERR764";

                                }

                            }
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
        private int DoCheck(SPInput_HandleNPR340Save spInput, ref bool flag, ref string errCode, ref string errMsg, ref List<ErrorInfo> lstError)
        {
            int hasPay = 0;
            SPOutput_HandleNPR340Save spOut = new SPOutput_HandleNPR340Save();
            string spName = new ObjType().GetSPName(ObjType.SPType.HandleNPR340Save);
            SQLHelper<SPInput_HandleNPR340Save, SPOutput_HandleNPR340Save> sqlHelp = new SQLHelper<SPInput_HandleNPR340Save, SPOutput_HandleNPR340Save>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
            new CommonFunc().checkSQLResult(ref flag,  spOut.Error,spOut.ErrorCode, ref lstError, ref errCode);
            if (flag)
            {
                hasPay = spOut.hadPaid;
            }
            return hasPay;
        }
        private void DoSave(SPInput_HandleNPR340SaveU1 spInput, ref bool flag,ref string errCode,ref string errMsg,ref List<ErrorInfo> lstError)
        {
            SPOutput_Base spOut = new SPOutput_Base();
            string spName = new ObjType().GetSPName(ObjType.SPType.HandleNPR340SaveU1);
            SQLHelper<SPInput_HandleNPR340SaveU1, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_HandleNPR340SaveU1, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
            new CommonFunc().checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
        }
        private void DoUPD(SPInput_HandleNPR340SaveU2 spInput, ref bool flag, ref string errCode, ref string errMsg, ref List<ErrorInfo> lstError)
        {
            SPOutput_Base spOut = new SPOutput_Base();
            string spName = new ObjType().GetSPName(ObjType.SPType.HandleNPR340SaveU2);
            SQLHelper<SPInput_HandleNPR340SaveU2, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_HandleNPR340SaveU2, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
            new CommonFunc().checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
        }
        private bool SendMail(string Title, string Body, string receive, string attach)
        {
            bool flag = true;

            System.Net.Mail.SmtpClient MySmtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            // Console.WriteLine("{0}開始執行發信", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //發送Email
            try
            {
                string SendID = ConfigurationManager.AppSettings["SendID"].ToString();
                string SendPWD = ConfigurationManager.AppSettings["SendPWD"].ToString();
                string receiver = receive;

                //設定你的帳號密碼
                //MySmtp.Credentials = new System.Net.NetworkCredential(SendID, SendPWD);
                ////Gmial 的 smtp 使用 SSL
                //MySmtp.EnableSsl = true;
                //MySmtp.Send(SendID+"@goodarc.com", receiver, Title, Body);

                string[] toa = receiver.Trim().Split(";".ToCharArray());
                MailMessage newMail = new MailMessage();
                newMail.From = new MailAddress("iRent刷卡通知<" + SendID + "@gmail.com>");

                newMail.Priority = MailPriority.Low;
                newMail.IsBodyHtml = true;
                newMail.Body = Body;
                foreach (string to in toa)
                {
                    newMail.To.Add(new MailAddress(to));
                }
                newMail.Subject = Title;
                //Attachment attachment = new Attachment(attach);
                //attachment.Name = "訂單編號" + System.IO.Path.GetFileName(attach).Replace(".pdf", "") + "消費紀錄.pdf";
                //attachment.NameEncoding = Encoding.GetEncoding("utf-8");
                //newMail.Attachments.Add(attachment);
                SmtpClient sc = new SmtpClient("smtp.gmail.com", 587);
                sc.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                sc.UseDefaultCredentials = false;
                sc.EnableSsl = true;
                sc.Credentials = new System.Net.NetworkCredential(SendID, SendPWD);
                //Gmial 的 smtp 使用 SSL

                sc.Send(newMail);
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.Message);
                flag = false;
            }
            return flag;
        }
    }
}
