using Domain.TB.BackEnd;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Params;
using Web.Utilities;

namespace Web.Controllers
{
    /// <summary>
    /// 會員管理
    /// </summary>
    public class MemberManageController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        string StorageBaseURL = (System.Configuration.ConfigurationManager.AppSettings["StorageBaseURL"] == null) ? "" : System.Configuration.ConfigurationManager.AppSettings["StorageBaseURL"].ToString();
        string credentialContainer = (System.Configuration.ConfigurationManager.AppSettings["credentialContainer"] == null) ? "" : System.Configuration.ConfigurationManager.AppSettings["credentialContainer"].ToString();
        /// <summary>
        /// 會員審核
        /// </summary>
        /// <returns></returns>
        public ActionResult Audit()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Audit(int AuditMode,int AuditType,string StartDate,string EndDate,int AuditReuslt,string UserName,string IDNO,string[] IDNOSuff, string AuditError)
        {
            ViewData["AuditMode"] = AuditMode;
            ViewData["AuditType"] = AuditType;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["AuditReuslt"] = AuditReuslt;
            ViewData["UserName"] = UserName;
            ViewData["IDNO"] = IDNO;
            ViewData["IDNOSuff"] =(IDNOSuff==null)?"": string.Join(",", IDNOSuff);
            ViewData["AuditError"] = AuditError;
            string IDNoSuffCombind = "";
            if (IDNOSuff != null)
            {
                if (IDNOSuff.Length > 0)
                {
                    IDNoSuffCombind += string.Format("{0},", IDNOSuff[0]);
                    int IDLEN = IDNOSuff.Length;
                    for (int i = 1; i < IDLEN; i++)
                    {
                        //IDNoSuffCombind += string.Format(",'{0}'", IDNOSuff[i]);
                        IDNoSuffCombind += string.Format("{0},", IDNOSuff[i]);
                    }
                }
            }
        
            List<BE_GetAuditList> lstData= new MemberRepository(connetStr).GetAuditLists(AuditMode, AuditType, StartDate, EndDate, AuditReuslt, UserName, IDNO, IDNoSuffCombind, AuditError);

            return View(lstData);
        }
        [HttpPost]
        public ActionResult AuditDetail(string AuditIDNO, string UserName)
        {
            if (UserName != null && Session["Account"]!=null)
            {
                List<BE_AuditImage> lstAuditsxx = new MemberRepository(connetStr).UpdateMemberName(AuditIDNO, UserName, Session["Account"].ToString());
            }
            string ImgURL = StorageBaseURL + credentialContainer + "/";
            string lsURL = ConfigurationManager.AppSettings["LS_FTP_URL_RESOLVE"];
            BE_AuditDetailCombind Data = new BE_AuditDetailCombind();
            WebAPIOutput_NPR172Query wsoutput = new WebAPIOutput_NPR172Query();
            HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
            bool flag = hiEasyRentAPI.NPR172Query(AuditIDNO, ref wsoutput);
            if (flag)
            {
                int index = wsoutput.Data.ToList().FindIndex(delegate (WebAPIOutput_NPR172QueryData data)
                  {
                      return data.MEMIDNO == AuditIDNO;
  
                  });
                if (index > -1)
                {
                    Data.block = new WebAPIOutput_NPR172QueryData();
                    Data.block = wsoutput.Data[index];
                }
            }

            BE_AuditDetail obj = new MemberRepository(connetStr).GetAuditDetail(AuditIDNO);
            List<BE_AuditImage> lstAudits=new MemberRepository(connetStr).GetAuditImage(AuditIDNO);
            List<BE_AuditHistory> lstHistory = new MemberRepository(connetStr).GetAuditHistory(AuditIDNO);
            List<BE_InsuranceData> lstInsuranceData = new MemberRepository(connetStr).GetGetInsuranceData(AuditIDNO);
            List<BE_SameMobileData> lstMobile = null;
            Data.RecommendHistory = new List<BE_AuditRecommendHistory>();
            Data.History = new List<BE_AuditHistory>();
            Data.History = lstHistory;

            Data.InsuranceData = lstInsuranceData;

            BaseParams param = new BaseParams();
            string returnMessage = "";
            var parms = new object[]
            {AuditIDNO};
            DataSet ds = WebApiClient.SPRetB(ServerInfo.GetServerInfo(param), "LS..SP_LSQ270_P01", parms, ref returnMessage);

            if (ds.Tables.Count > 0)
            {
                for(int i=0;i< ds.Tables[0].Rows.Count; i++)
                {
                    Data.RecommendHistory.Add(new BE_AuditRecommendHistory
                    {
                        gift_name = ds.Tables[0].Rows[i]["獎勵名稱"]==null?"":ds.Tables[0].Rows[i]["獎勵名稱"].ToString(),
                        gift_mins = ds.Tables[0].Rows[i]["獎勵時數"] == null ? "" : ds.Tables[0].Rows[i]["獎勵時數"].ToString(),
                        U_SYSDT = ds.Tables[0].Rows[i]["獎勵時間"] == null ? "" : ds.Tables[0].Rows[i]["獎勵時間"].ToString(),
                        recommended = ds.Tables[0].Rows[i]["被推薦人"] == null ? "" : ds.Tables[0].Rows[i]["被推薦人"].ToString(),
                        recommend = ds.Tables[0].Rows[i]["推薦人"] == null ? "" : ds.Tables[0].Rows[i]["推薦人"].ToString(),
                        memrfbnr = ds.Tables[0].Rows[i]["推薦碼"] == null ? "" : ds.Tables[0].Rows[i]["推薦碼"].ToString()
                    });
                }
            }


            Data.SameMobile = new List<BE_SameMobileData>();
            if (obj != null)
            {
                lstMobile = new MemberRepository(connetStr).GetSameMobile(AuditIDNO, obj.MEMTEL);
                Data.SameMobile = lstMobile;
            }
            Data.detail = new BE_AuditDetail();
            Data.detail = obj;
            Data.detail.SPED = obj.SPED != "" ? obj.SPED.Substring(0, 4) + "-" + obj.SPED.Substring(4, 2) + "-" + obj.SPED.Substring(6, 2) : obj.SPED;
            Data.detail.SPSD = obj.SPSD != "" ? obj.SPSD.Substring(0, 4) + "-" + obj.SPSD.Substring(4, 2) + "-" + obj.SPSD.Substring(6, 2) : obj.SPSD;
            if (lstAudits != null)
            {

                Data.Images = new BE_AuditCrentials();
                for(int i = 1; i <= 11; i++)
                {

                    int index = lstAudits.FindIndex(delegate (BE_AuditImage image)
                    {
                        return image.CrentialsType == i;
                    });
                    if (index > -1)
                    {
                        switch (i)
                        {
                            case 1:
                                Data.Images.ID_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP")>-1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.ID_1_IsNew = 1;
                                Data.Images.ID_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //若UPDTime是空，則AuditResult=sp傳回的AuditResult。若UPDTime不是空，則AuditResult=(若sp傳回的AuditResult不是1且有註記失敗原因則=-1，不然就0)
                                Data.Images.ID_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.ID_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 2:
                                Data.Images.ID_2 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.ID_2_IsNew = 1;
                                Data.Images.ID_2_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                Data.Images.ID_2_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.ID_2_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 3:
                                Data.Images.Car_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Car_1_IsNew = 1;
                                Data.Images.Car_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                Data.Images.Car_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Car_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 4:
                                Data.Images.Car_2 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Car_2_IsNew = 1;
                                Data.Images.Car_2_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                Data.Images.Car_2_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Car_2_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 5:
                                Data.Images.Motor_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Motor_1_IsNew = 1;
                                Data.Images.Motor_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                Data.Images.Motor_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Motor_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 6:
                                Data.Images.Motor_2 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Motor_2_IsNew = 1;
                                Data.Images.Motor_2_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                Data.Images.Motor_2_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Motor_2_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 7:
                                Data.Images.Self_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Self_1_IsNew = 1;
                                Data.Images.Self_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                Data.Images.Self_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Self_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 8:
                                Data.Images.F01 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.F01_IsNew = 1;
                                Data.Images.F01_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                Data.Images.F01_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.F01_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 9:
                                Data.Images.Other_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Other_1_IsNew = 1;
                                Data.Images.Other_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                Data.Images.Other_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Other_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 10:
                                Data.Images.Business_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Business_1_IsNew = 1;
                                Data.Images.Business_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                Data.Images.Business_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Business_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 11:
                                Data.Images.Signture_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Signture_1_IsNew = 1;
                                Data.Images.Signture_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //20210120唐改，簽名檔沒有UPDTIME會產生BUG，所以拿掉UPDTIME判定
                                //Data.Images.Signture_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Signture_1_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0;
                                Data.Images.Signture_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                        }
                    }
                    else
                    {
                        int Oindex= lstAudits.FindIndex(delegate (BE_AuditImage image)
                        {
                            return image.AlreadyType == i;
                        });
                        if (Oindex > -1)
                        {
                            switch (i)
                            {
                                case 1:
                                    Data.Images.ID_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.ID_1_IsNew = 0;
                                    Data.Images.ID_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    Data.Images.ID_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.ID_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 2:
                                    Data.Images.ID_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.ID_2_IsNew = 0;
                                    Data.Images.ID_2_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    Data.Images.ID_2_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.ID_2_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 3:
                                    Data.Images.Car_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Car_1_IsNew = 0;
                                    Data.Images.Car_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    Data.Images.Car_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Car_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 4:
                                    Data.Images.Car_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Car_2_IsNew = 0;
                                    Data.Images.Car_2_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    Data.Images.Car_2_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Car_2_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 5:
                                    Data.Images.Motor_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Motor_1_IsNew = 0;
                                    Data.Images.Motor_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    Data.Images.Motor_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Motor_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 6:
                                    Data.Images.Motor_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Motor_2_IsNew = 0;
                                    Data.Images.Motor_2_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    Data.Images.Motor_2_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Motor_2_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 7:
                                    Data.Images.Self_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Self_1_IsNew = 0;
                                    Data.Images.Self_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    Data.Images.Self_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Self_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 8:
                                    Data.Images.F01 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.F01_IsNew = 0;
                                    Data.Images.F01_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    Data.Images.F01_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.F01_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 9:
                                    Data.Images.Other_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Other_1_IsNew = 0;
                                    Data.Images.Other_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    Data.Images.Other_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Other_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 10:
                                    Data.Images.Business_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Business_1_IsNew = 0;
                                    Data.Images.Business_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    Data.Images.Business_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Business_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 11:
                                    Data.Images.Signture_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Signture_1_IsNew = 0;
                                    Data.Images.Signture_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    Data.Images.Signture_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Signture_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                            }
                        }
                       
                      
                    }
                }
            }
       

            return View(Data);
        }
        public ActionResult AuditHistory(string IDNO)
        {
            return View();
        }
        public ActionResult CredentialsView(string IDNO)
        {
            return View();
        }
        /// <summary>
        /// 手機重複清單
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewSameMobile()
        {
            MemberRepository repository = new MemberRepository(connetStr);
            List<BE_SameMobileData> lstData = repository.GetSameMobile();
            return View(lstData);
        }

        /// <summary>
        /// 會員審核
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword( string IDNO, string Password)
        {
            List<BE_GetAuditList> lstData = new List<BE_GetAuditList>();
            try
            {
                ViewData["IDNO"] = IDNO;
                ViewData["Password"] = Password;

                lstData = new MemberRepository(connetStr).ChangePassword(IDNO, Password);
                ViewData["ErrorMessage"] = "修改成功";
            }catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
            }

            return View(lstData);
        }
    }
}