﻿using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Output;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using Prometheus; //20210707唐加prometheus
using StackExchange.Redis;//20210913唐加redis
using Domain.TB;
using Reposotory.Implement;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 會員審核
    /// </summary>
    public class BE_AuditController : ApiController
    {
        //唐加prometheus
        //private static readonly Counter ProcessedJobCount1 = Metrics.CreateCounter("BENSON_BE_AuditDetail", "the number of CALL BE_Audit",
        //    new CounterConfiguration
        //    {
        //        // Here you specify only the names of the labels.
        //        LabelNames = new[] { "method","server" }
        //    });
        private static readonly Gauge ProcessedJobCount1 = Metrics.CreateGauge("BENSON_BE_AuditDetail", "the number of CALL BE_Audit");

        //20210913唐加redis，解決azure多執行個體造成prometheus的數值亂跳問題
        private string RedisConnet = ConfigurationManager.ConnectionStrings["RedisConnectionString"].ConnectionString;
        private static Lazy<ConnectionMultiplexer> lazyConnection;
        public BE_AuditController()
        {
            if (lazyConnection == null)
            {
                lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(RedisConnet));
            }
        }

        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        string StorageBaseURL = (System.Configuration.ConfigurationManager.AppSettings["StorageBaseURL"] == null) ? "" : System.Configuration.ConfigurationManager.AppSettings["StorageBaseURL"].ToString();
        string credentialContainer = (System.Configuration.ConfigurationManager.AppSettings["credentialContainer"] == null) ? "" : System.Configuration.ConfigurationManager.AppSettings["credentialContainer"].ToString();
        /// <summary>
        /// 【後台】會員審核
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_Audit(Dictionary<string, object> value)
        {
            //ProcessedJobCount1.Inc();//唐加prometheus
            SetAuditDetailCount();

            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            //ProcessedJobCount1.WithLabels(httpContext.Request.HttpMethod, "NO2").Inc();
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BE_AuditController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_Audit apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";

            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_Audit>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID };
                string[] errList = { "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
            }
            #endregion
            #region 判斷有無修正圖片類型
            if (flag)
            {
                SPInput_BE_AuditImage spInput = new SPInput_BE_AuditImage()
                {
                    IDNO = apiInput.IDNO,
                    UserID = apiInput.UserID,
                    ID_1 = apiInput.ImageData.ID_1_new,
                    ID_1_Audit = (apiInput.ImageData.ID_1_Audit == 1) ? 2 : apiInput.ImageData.ID_1_Audit,
                    ID_1_Reason = apiInput.ImageData.ID_1_Reason,
                    ID_1_Image = (apiInput.ImageData.ID_1 == apiInput.ImageData.ID_1_new) ? RemoveSuff(apiInput.ImageData.ID_1_Image) : CheckNeedChangeName(apiInput.ImageData.ID_1, apiInput.ImageData.ID_1_new, RemoveSuff(apiInput.ImageData.ID_1_Image)),

                    ID_2 = apiInput.ImageData.ID_2_new,
                    ID_2_Audit = (apiInput.ImageData.ID_2_Audit == 1) ? 2 : apiInput.ImageData.ID_2_Audit,
                    ID_2_Reason = apiInput.ImageData.ID_2_Reason,
                    ID_2_Image = (apiInput.ImageData.ID_2 == apiInput.ImageData.ID_2_new) ? RemoveSuff(apiInput.ImageData.ID_2_Image) : CheckNeedChangeName(apiInput.ImageData.ID_2, apiInput.ImageData.ID_2_new, RemoveSuff(apiInput.ImageData.ID_2_Image)),

                    Car_1 = apiInput.ImageData.Car_1_new,
                    Car_1_Audit = (apiInput.ImageData.Car_1_Audit == 1) ? 2 : apiInput.ImageData.Car_1_Audit,
                    Car_1_Reason = apiInput.ImageData.Car_1_Reason,
                    Car_1_Image = (apiInput.ImageData.Car_1 == apiInput.ImageData.Car_1_new) ? RemoveSuff(apiInput.ImageData.Car_1_Image) : CheckNeedChangeName(apiInput.ImageData.Car_1, apiInput.ImageData.Car_1_new, RemoveSuff(apiInput.ImageData.Car_1_Image)),
                    Car_2 = apiInput.ImageData.Car_2_new,
                    Car_2_Audit = (apiInput.ImageData.Car_2_Audit == 1) ? 2 : apiInput.ImageData.Car_2_Audit,
                    Car_2_Reason = apiInput.ImageData.Car_2_Reason,
                    Car_2_Image = (apiInput.ImageData.Car_2 == apiInput.ImageData.Car_2_new) ? RemoveSuff(apiInput.ImageData.Car_2_Image) : CheckNeedChangeName(apiInput.ImageData.Car_2, apiInput.ImageData.Car_2_new, RemoveSuff(apiInput.ImageData.Car_2_Image)),

                    Motor_1 = apiInput.ImageData.Motor_1_new,
                    Motor_1_Audit = (apiInput.ImageData.Motor_1_Audit == 1) ? 2 : apiInput.ImageData.Motor_1_Audit,
                    Motor_1_Reason = apiInput.ImageData.Motor_1_Reason,
                    Motor_1_Image = (apiInput.ImageData.Motor_1 == apiInput.ImageData.Motor_1_new) ? RemoveSuff(apiInput.ImageData.Motor_1_Image) : CheckNeedChangeName(apiInput.ImageData.Motor_1, apiInput.ImageData.Motor_1_new, RemoveSuff(apiInput.ImageData.Motor_1_Image)),

                    Motor_2 = apiInput.ImageData.Motor_2_new,
                    Motor_2_Audit = (apiInput.ImageData.Motor_2_Audit == 1) ? 2 : apiInput.ImageData.Motor_2_Audit,
                    Motor_2_Reason = apiInput.ImageData.Motor_2_Reason,
                    Motor_2_Image = (apiInput.ImageData.Motor_2 == apiInput.ImageData.Motor_2_new) ? RemoveSuff(apiInput.ImageData.Motor_2_Image) : CheckNeedChangeName(apiInput.ImageData.Motor_2, apiInput.ImageData.Motor_2_new, RemoveSuff(apiInput.ImageData.Motor_2_Image)),

                    F01 = apiInput.ImageData.F01_new,
                    F01_Audit = (apiInput.ImageData.F01_Audit == 1) ? 2 : apiInput.ImageData.F01_Audit,
                    F01_Reason = apiInput.ImageData.F01_Reason,
                    F01_Image = (apiInput.ImageData.F01 == apiInput.ImageData.F01_new) ? RemoveSuff(apiInput.ImageData.F01_Image) : CheckNeedChangeName(apiInput.ImageData.F01, apiInput.ImageData.F01_new, RemoveSuff(apiInput.ImageData.F01_Image)),

                    Other_1 = apiInput.ImageData.Other_1_new,
                    Other_1_Audit = (apiInput.ImageData.Other_1_Audit == 1) ? 2 : apiInput.ImageData.Other_1_Audit,
                    Other_1_Reason = apiInput.ImageData.Other_1_Reason,
                    Other_1_Image = (apiInput.ImageData.Other_1 == apiInput.ImageData.Other_1_new) ? RemoveSuff(apiInput.ImageData.Other_1_Image) : CheckNeedChangeName(apiInput.ImageData.Other_1, apiInput.ImageData.Other_1_new, RemoveSuff(apiInput.ImageData.Other_1_Image)),

                    Self_1 = apiInput.ImageData.Self_1_new,
                    Self_1_Audit = (apiInput.ImageData.Self_1_Audit == 1) ? 2 : apiInput.ImageData.Self_1_Audit,
                    Self_1_Reason = apiInput.ImageData.Self_1_Reason,
                    Self_1_Image = (apiInput.ImageData.Self_1 == apiInput.ImageData.Self_1_new) ? RemoveSuff(apiInput.ImageData.Self_1_Image) : CheckNeedChangeName(apiInput.ImageData.Self_1, apiInput.ImageData.Self_1_new, RemoveSuff(apiInput.ImageData.Self_1_Image)),

                    Business_1 = apiInput.ImageData.Business_1_new,
                    Business_1_Audit = (apiInput.ImageData.Business_1_Audit == 1) ? 2 : apiInput.ImageData.Business_1_Audit,
                    Business_1_Reason = apiInput.ImageData.Business_1_Reason,
                    Business_1_Image = (apiInput.ImageData.Business_1 == apiInput.ImageData.Business_1_new) ? RemoveSuff(apiInput.ImageData.Business_1_Image) : CheckNeedChangeName(apiInput.ImageData.Business_1, apiInput.ImageData.Business_1_new, RemoveSuff(apiInput.ImageData.Business_1_Image)),

                    Signture_1 = apiInput.ImageData.Signture_1_new,
                    Signture_1_Audit = (apiInput.ImageData.Signture_1_Audit == 1) ? 2 : apiInput.ImageData.Signture_1_Audit,
                    Signture_1_Reason = apiInput.ImageData.Signture_1_Reason,
                    Signture_1_Image = (apiInput.ImageData.Signture_1 == apiInput.ImageData.Signture_1_new) ? RemoveSuff(apiInput.ImageData.Signture_1_Image) : CheckNeedChangeName(apiInput.ImageData.Signture_1, apiInput.ImageData.Signture_1_new, RemoveSuff(apiInput.ImageData.Signture_1_Image)),
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                string spName = new ObjType().GetSPName(ObjType.SPType.BE_HandleAuditImage);
                SQLHelper<SPInput_BE_AuditImage, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_AuditImage, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            }
            #endregion
            #region TB

            //這邊資料存到iRent db
            if (flag)
            {
                string CarRentType = "0";
                string MotorRentType = "0";
                for (int i = 0; i < apiInput.Driver.Count; i++)
                {
                    if (apiInput.Driver[i] == "CarDriver")
                    {
                        CarRentType = "1";
                    }
                    else
                    {
                        if (apiInput.Driver[i] == "MotoDriver2")
                        {
                            MotorRentType = "2";
                        }
                        else
                        {
                            MotorRentType = "1";
                        }
                    }
                }
                string spName = new ObjType().GetSPName(ObjType.SPType.BE_HandleAudit);
                SPInput_BE_Audit spInput = new SPInput_BE_Audit()
                {
                    LogID = LogID,
                    UserID = apiInput.UserID,
                    Addr = apiInput.Addr,
                    Area = apiInput.Area,
                    AuditStatus = apiInput.AuditStatus,
                    Birth = apiInput.Birth,
                    IDNO = apiInput.IDNO,
                    InvoiceType = apiInput.InvoiceType,
                    IsNew = apiInput.IsNew,
                    Mobile = apiInput.Mobile,
                    NotAuditReason = apiInput.NotAuditReason,
                    RejectReason = apiInput.RejectReason,
                    SPECSTATUS = apiInput.SPECSTATUS,
                    SPED = apiInput.SPED,
                    SPSD = apiInput.SPSD,
                    UniCode = apiInput.UniCode,
                    Driver = string.Format("{0}{1}", CarRentType, MotorRentType),
                    MEMHTEL = apiInput.MEMHTEL,
                    MEMCOMTEL = apiInput.MEMCOMTEL,
                    MEMCONTRACT = apiInput.MEMCONTRACT,
                    MEMCONTEL = apiInput.MEMCONTEL,
                    MEMEMAIL = apiInput.MEMEMAIL,
                    HasVaildEMail = apiInput.HasVaildEMail,
                    MEMMSG = apiInput.MEMMSG,
                    MEMONEW = apiInput.MEMONEW //20210115唐加
                };

                //20220521 ADD BY ADAM REASON.增加首次審核通過回傳
                //SPOutput_Base spOut = new SPOutput_Base();
                //SQLHelper<SPInput_BE_Audit, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_Audit, SPOutput_Base>(connetStr);
                SPOutput_BE_HandleAudit spOut = new SPOutput_BE_HandleAudit();
                SQLHelper<SPInput_BE_Audit, SPOutput_BE_HandleAudit> sqlHelp = new SQLHelper<SPInput_BE_Audit, SPOutput_BE_HandleAudit>(connetStr);

                logger.Trace("AuditSave:" + JsonConvert.SerializeObject(spInput));
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                logger.Trace("AuditSaveResult:" + JsonConvert.SerializeObject(spOut) + ",Error:" + JsonConvert.SerializeObject(lstError));
                //baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                //20220521 ADD BY ADAM REASON.增加首次審核通過回傳
                if (spOut.FirstAudit == "Y" && apiInput.MEMEMAIL.ToString() != "" && DateTime.Now < DateTime.Parse("2022-06-30 23:59"))
                {
                    //發送EDM通知
                    WebCommon.SendMail edm = new SendMail();
                    //string EDM_Body = "<a href='https://www.irentcar.com.tw/event/111event/3043/index.html' ><img src='https://www.irentcar.com.tw/event/111event/3043/img/main.jpg' /><br/>點選進入</a>";
                    edm.DoSendMail("iRent會員通知", EDM_Body, apiInput.MEMEMAIL);
                }
            }
            //這邊資料用api拋給sqyhi06vm
            if (flag && apiInput.AuditStatus == 1)
            {
                HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();

                if (flag)
                {
                    //20211222 UPD BY FRANK REASON.將拋短租轉移至會員審核通過後進行
                    GetMemberCMK memberCMKData = null;
                    memberCMKData = new HiEasyRentRepository(connetStr).GetMemberCMK(apiInput.IDNO);
                    WebAPIInput_TransIRentMemCMK wsInput = new WebAPIInput_TransIRentMemCMK
                    {
                        IDNO = memberCMKData.MEMIDNO,
                        VERTYPE = memberCMKData.VerType,
                        VER = memberCMKData.Version,
                        VERSOURCE = memberCMKData.Source,
                        TEL = memberCMKData.TEL,
                        SMS = memberCMKData.SMS,
                        EMAIL = memberCMKData.EMAIL,
                        POST = memberCMKData.POST,
                        MEMO = "",
                        COMPID = "EF",
                        COMPNM = "和雲",
                        PRGID = "iRent_6",
                        USERID = "iRentUser"
                    };
                    WebAPIOutput_TransIRentMemCMK CMKOutput = new WebAPIOutput_TransIRentMemCMK();
                    

                    flag = hiEasyRentAPI.TransIRentMemCMK(wsInput, ref CMKOutput);
                    if (flag == false)
                    {
                        errCode = "ERR776";
                    }
                }

                WebAPIOutput_NPR013Reg wsOutput = new WebAPIOutput_NPR013Reg();

                WebAPIInput_NPR010Save spInput = new WebAPIInput_NPR010Save() //宣告好多欄位傳去NPR010，但下面只有部分設定，部分還直接寫死
                {
                    user_id = "HLC",
                    MEMIDNO = apiInput.IDNO,
                    //MEMCNAME = apiInput.MEMCNAME,
                    //InvoiceType = apiInput.InvoiceType,
                    //IsNew = apiInput.IsNew,
                    MEMCEIL = apiInput.Mobile,
                    SPCSTATUS = apiInput.SPECSTATUS,
                    UNIMNO = apiInput.UniCode,
                    MEMBIRTH = apiInput.Birth,
                    MEMCITY = apiInput.Area.ToString(),
                    MEMADDR = apiInput.Addr,
                    MEMTEL = apiInput.MEMHTEL,
                    MEMCOMTEL = apiInput.MEMCOMTEL,
                    MEMCONTRACT = apiInput.MEMCONTRACT,
                    MEMCONTEL = apiInput.MEMCONTEL,
                    MEMMSG = apiInput.MEMMSG,
                    tbExtSigninList = new List<ExtSigninList>(),
                    //20210218唐:這邊還要加 IRENTFLG(N未審T審失敗Y審通過)
                    //PROCD = (apiInput.IsNew==1) ? "A" : "U",
                    IRENTFLG = (apiInput.AuditStatus == 0) ? "N" : ((apiInput.AuditStatus == -1) ? "T" : "Y")//AuditStatus=1通過、-1不通過、0都沒勾選
                };
                flag = hiEasyRentAPI.NPR010Save(spInput, ref wsOutput);
            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            #region 會員審核簡訊傳送
            //會員審核簡訊傳送
            if (apiInput.SendMessage == 1)
            {
                HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                WebAPIOutput_NPR260Send wsOutput = new WebAPIOutput_NPR260Send();
                string Message = "";
                if (apiInput.AuditStatus == 1)
                {
                    if (apiInput.IsNew == 1)
                    {
                        Message = "iRent會員審核通過：" +
                            "快前往體驗最方便的共享汽機車服務！" +
                            "新手上路前，完成小學堂測驗拿免費時數：https://bit.ly/3pAeK9R";
                    }
                    else
                    {
                        Message = string.Format("您已成功變更「iRent會員」身分。" +
                            "租車前完成小學堂教學及測驗即可拿免費時數：https://bit.ly/3pAeK9R", DateTime.Today.ToString("yyyy/MM/dd"));
                    }
                }
                else
                {
                    if (apiInput.IsNew == 1)
                    {
                        Message = string.Format("iRent審核未通過：" +
                            "原因為({0}，請登入App重新操作，" +
                            "如有疑問請洽客服0800-024-550", apiInput.RejectReason == "" ? apiInput.NotAuditReason : apiInput.RejectReason);
                    }
                    else
                    {
                        Message = string.Format("iRent會員變更未通過：" +
                            "原因為({0})，請登入App重新操作，" +
                            "如有疑問請洽客服0800-024-550", apiInput.RejectReason == "" ? apiInput.NotAuditReason : apiInput.RejectReason);
                    }
                }
                flag = hiEasyRentAPI.NPR260Send(apiInput.Mobile, Message, "", ref wsOutput);
            }
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
        private string RemoveSuff(string fileName)
        {
            string ImgURL = StorageBaseURL + credentialContainer + "/";
            return fileName.Replace(ImgURL, "");
        }
        private string CheckNeedChangeName(int OldType, int NewType, string OldFileName)
        {
            //string[] suff = { "", "ID_1", "ID_2", "Driver_1", "Driver_2", "Moto_1", "Moto_2", "Self_1", "F1", "Other_1", "Business_1", "Signture_1" };
            string[] suff = { "", "ID_1", "ID_2", "Car_1", "Car_2", "Moto_1", "Moto_2", "Self_1", "F1", "Other_1", "Business_1", "Signture_1" };
            string fileName = OldFileName.Replace(suff[OldType], suff[NewType]);
            bool flag = new AzureStorageHandle().RenameFromAzureStorage(fileName, OldFileName, credentialContainer);
            return fileName;
        }

        private void SetAuditDetailCount()
        {
            var value = 1;
            try
            {
                ConnectionMultiplexer connection = lazyConnection.Value;
                IDatabase cache = connection.GetDatabase();

                var key = "the number of CALL BE_Audit";
                var cacheString = cache.StringGet(key);
                if (cacheString.HasValue)
                {
                    int.TryParse(cacheString.ToString(), out value);
                    value++;

                    string _am = "00:00";
                    string _pm = "08:00";
                    TimeSpan dspAM = DateTime.Parse(_am).TimeOfDay;
                    TimeSpan dspPM = DateTime.Parse(_pm).TimeOfDay;
                    DateTime t1 = Convert.ToDateTime(DateTime.Now.ToString("HH:mm"));
                    TimeSpan dspNOW = t1.TimeOfDay;
                    if (dspNOW > dspAM && dspNOW < dspPM){ value=0; }
                }
                cache.StringSet(key, value);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            ProcessedJobCount1.Set(value); //宣告Guage才能用set
        }
        private int MEMRFNBR_FromStr(string sour)
        {
            if (double.TryParse(sour, out double d_sour))
                return Convert.ToInt32(Math.Floor(d_sour));
            else
                throw new Exception("MEMRFNBR格式錯誤");
        }


        private static string EDM_Body = @"
            <html>
            <head>
            <link rel='stylesheet' href='https://www.irentcar.com.tw/event/111event/3043/all.css'>
            <title>歡迎iRent新會員審核通過</title>
            <meta name = 'description' content='iRent新會員' />
            <meta name = 'viewport' content='width=device-width, initial-scale=1.0'>
            <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>

            <script async src='https://www.googletagmanager.com/gtag/js?id=UA-194927061-1' type='2ba0d8ddb35d90ceaf33e6e6-text/javascript'></script>
            <script type = '2ba0d8ddb35d90ceaf33e6e6-text/javascript' >

              window.dataLayer = window.dataLayer || [];

                    function gtag() { dataLayer.push(arguments); }

                    gtag('js', new Date());

 

              gtag('config', 'UA-194927061-1');

            </script>
            </head>
            <body>
            <div class='container'>
            <div class='container'>
            <div class='box pp'>
            <img id = 'banner' src='https://www.irentcar.com.tw/event/111event/3043/img/main.jpg'>
            <p class='text'><b>Hi iRent會員您好<br>恭喜您審核通過，成為iRent的一員！</br>是否已滿心期待，準備好讓iRent陪您來趟美好旅程了呢？<br>啟程前，您不可錯過的好康活動，推薦給您。<br>內含時數贈送活動，趕快點開一探究竟～<b></p>
            </div>
            <div class='card'>
            </br></br></br>
            <a href = 'https://www.irentcar.com.tw/event/111event/2098/index.html' target='_blank'>
            <img src = 'https://www.irentcar.com.tw/event/111event/3043/img/pic1.jpg' ></ a >
            <p><b>【限時活動】即日起至6/30止<br> 使用中信卡綁定和泰Pay，就可獲得免費汽車時數30分鐘(每ID限獲得乙次)<b></p>
            </div>
            <div class='container'>
            <div class='card'>
            </br></br></br>
            <a href = 'https://www.irentcar.com.tw/event/111event/3027/index.html' target='_blank'>
            <img src = 'https://www.irentcar.com.tw/event/111event/3043/img/pic2.jpg' ></ a >
            <p><b>【限時活動】即日起至6/30止<br> 天天登入享免費機車時數6分鐘，完成指定任務最高還可獲得汽車時數60分鐘<b></p>
            </div>
            <div class='card'>
            </br></br></br>
            <a href = 'https://www.irentcar.com.tw/iRentSchool/' target='_blank'>
            <img src = 'https://www.irentcar.com.tw/event/111event/3043/img/pic3.jpg' ></ a >
            <p><b> 立即完成小學堂測驗，最高可獲得300分鐘iRent時數<b></p>
            </div>
            <script src = '/cdn-cgi/scripts/7d0fa10a/cloudflare-static/rocket-loader.min.js' data-cf-settings='2ba0d8ddb35d90ceaf33e6e6-|49' defer=''></script><script defer src='https://static.cloudflareinsights.com/beacon.min.js/v652eace1692a40cfa3763df669d7439c1639079717194' integrity='sha512-Gi7xpJR8tSkrpF7aordPZQlW2DLtzUlZcumS8dMQjwDHEnw9I7ZLyiOj/6tZStRBGtGgN6ceN6cMH8z7etPGlw==' data-cf-beacon='{'rayId':'71058ce68eab6b6f','token':'032a1d9439474897bfe67e733c1a68a4','version':'2021.12.0','si':100}' crossorigin='anonymous'></script>
            </body>
            </html>
            ";

    }
}