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
using Domain.SP.BE.Input;
using Domain.SP.Output;
using WebCommon;
using WebAPI.Models.BaseFunc;
using Web.Models.Enum;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

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

        #region 會員審核及明細
        /// <summary>
        /// 會員審核
        /// </summary>
        /// <returns></returns>
        public ActionResult Audit()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Audit(int AuditMode, int AuditType, string StartDate, string EndDate, int AuditReuslt, string UserName, string IDNO, string[] IDNOSuff, string AuditError)
        {
            ViewData["AuditMode"] = AuditMode;
            ViewData["AuditType"] = AuditType;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["AuditReuslt"] = AuditReuslt;
            ViewData["UserName"] = UserName;
            ViewData["IDNO"] = IDNO;
            ViewData["IDNOSuff"] = (IDNOSuff == null) ? "" : string.Join(",", IDNOSuff);
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

            List<BE_GetAuditList> lstData = new MemberRepository(connetStr).GetAuditLists(AuditMode, AuditType, StartDate, EndDate, AuditReuslt, UserName, IDNO, IDNoSuffCombind, AuditError);

            return View(lstData);
        }

        [HttpPost]
        public ActionResult AuditDetail(string AuditIDNO, string UserName)
        {
            if (UserName != null && Session["Account"] != null)
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
            List<BE_AuditImage> lstAudits = new MemberRepository(connetStr).GetAuditImage(AuditIDNO);//抓出每個人的12張圖片資訊
            List<BE_AuditHistory> lstHistory = new MemberRepository(connetStr).GetAuditHistory(AuditIDNO);
            List<BE_InsuranceData> lstInsuranceData = new MemberRepository(connetStr).GetGetInsuranceData(AuditIDNO);
            List<BE_SameMobileData> lstMobile = null;
            List<BE_MileStone> lstMileStone = new MemberRepository(connetStr).GetMileStone(AuditIDNO);
            List<BE_MileStoneDetail> lstMileStoneDetail = new MemberRepository(connetStr).GetMileStoneDetail(AuditIDNO);

            //Newtonsoft.Json序列化
            string jsonData = JsonConvert.SerializeObject(lstMileStoneDetail);
            Data.JsonMileStoneDetail = jsonData;

            string mobileBlock = ""; //20210310唐加
            Data.RecommendHistory = new List<BE_AuditRecommendHistory>();
            Data.History = new List<BE_AuditHistory>();
            Data.History = lstHistory;
            Data.MileStone = new List<BE_MileStone>();
            Data.MileStone = lstMileStone;
            //Data.MileStoneDetail = new List<BE_MileStoneDetail>();
            //Data.MileStoneDetail = lstMileStoneDetail;

            Data.InsuranceData = lstInsuranceData;

            BaseParams param = new BaseParams();
            string returnMessage = "";
            var parms = new object[]
            {AuditIDNO};
            DataSet ds = WebApiClient.SPRetB(ServerInfo.GetServerInfo(param), "LS..SP_LSQ270_P01", parms, ref returnMessage);

            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Data.RecommendHistory.Add(new BE_AuditRecommendHistory
                    {
                        gift_name = ds.Tables[0].Rows[i]["獎勵名稱"] == null ? "" : ds.Tables[0].Rows[i]["獎勵名稱"].ToString(),
                        gift_mins = ds.Tables[0].Rows[i]["獎勵時數"] == null ? "" : ds.Tables[0].Rows[i]["獎勵時數"].ToString(),
                        U_SYSDT = ds.Tables[0].Rows[i]["獎勵時間"] == null ? "" : ds.Tables[0].Rows[i]["獎勵時間"].ToString(),
                        recommended = ds.Tables[0].Rows[i]["被推薦人"] == null ? "" : ds.Tables[0].Rows[i]["被推薦人"].ToString(),
                        recommend = ds.Tables[0].Rows[i]["推薦人"] == null ? "" : ds.Tables[0].Rows[i]["推薦人"].ToString(),
                        memrfbnr = ds.Tables[0].Rows[i]["推薦碼"] == null ? "" : ds.Tables[0].Rows[i]["推薦碼"].ToString()
                    });
                }
            }


            Data.SameMobile = new List<BE_SameMobileData>();
            Data.mobileBlock = "";//20210310唐加
            if (obj != null)
            {
                lstMobile = new MemberRepository(connetStr).GetSameMobile(AuditIDNO, obj.MEMTEL);
                Data.SameMobile = lstMobile;
                //20210310唐加
                mobileBlock = new MemberRepository(connetStr).GetMobileBlock(obj.MEMTEL);
                Data.mobileBlock = mobileBlock;
            }
            Data.detail = new BE_AuditDetail();
            Data.detail = obj;
            Data.detail.SPED = obj.SPED != "" ? obj.SPED.Substring(0, 4) + "-" + obj.SPED.Substring(4, 2) + "-" + obj.SPED.Substring(6, 2) : obj.SPED;
            Data.detail.SPSD = obj.SPSD != "" ? obj.SPSD.Substring(0, 4) + "-" + obj.SPSD.Substring(4, 2) + "-" + obj.SPSD.Substring(6, 2) : obj.SPSD;

            if (lstAudits != null)//若有抓出圖片資訊就跑下面這段
            {
                Data.Images = new BE_AuditCrentials(); //宣告每張照片的屬性
                for (int i = 1; i <= 11; i++)
                {
                    //判斷CrentialsType有值的就設定
                    int index = lstAudits.FindIndex(delegate (BE_AuditImage image)
                    {
                        return image.CrentialsType == i;
                    });
                    if (index > -1)
                    {
                        switch (i)
                        {
                            //用UPDTime去交叉判斷，到底為何要這麼麻煩?
                            //若UPDTime是空(表示tmp沒資料)，則AuditResult=sp傳回的AuditResult。若UPDTime不是空，則AuditResult=(若sp傳回的AuditResult不是1且有註記失敗原因則=-1，不然就0)
                            case 1:
                                Data.Images.ID_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.ID_1_IsNew = 1;
                                Data.Images.ID_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.ID_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.ID_1_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                Data.Images.ID_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 2:
                                Data.Images.ID_2 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.ID_2_IsNew = 1;
                                Data.Images.ID_2_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.ID_2_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.ID_2_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                Data.Images.ID_2_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 3:
                                Data.Images.Car_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Car_1_IsNew = 1;
                                Data.Images.Car_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Car_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Car_1_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                Data.Images.Car_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 4:
                                Data.Images.Car_2 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Car_2_IsNew = 1;
                                Data.Images.Car_2_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Car_2_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Car_2_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                Data.Images.Car_2_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 5:
                                Data.Images.Motor_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Motor_1_IsNew = 1;
                                Data.Images.Motor_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Motor_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Motor_1_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                Data.Images.Motor_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 6:
                                Data.Images.Motor_2 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Motor_2_IsNew = 1;
                                Data.Images.Motor_2_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Motor_2_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Motor_2_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                Data.Images.Motor_2_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 7:
                                Data.Images.Self_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Self_1_IsNew = 1;
                                Data.Images.Self_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Self_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Self_1_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                Data.Images.Self_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 8:
                                Data.Images.F01 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.F01_IsNew = 1;
                                Data.Images.F01_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.F01_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.F01_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                Data.Images.F01_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 9:
                                Data.Images.Other_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Other_1_IsNew = 1;
                                Data.Images.Other_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Other_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Other_1_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                Data.Images.Other_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 10:
                                Data.Images.Business_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Business_1_IsNew = 1;
                                Data.Images.Business_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Business_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Business_1_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                Data.Images.Business_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 11:
                                Data.Images.Signture_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Signture_1_IsNew = 1;
                                Data.Images.Signture_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //20210120唐改，簽名檔沒有UPDTIME會產生BUG，所以拿掉UPDTIME判定
                                //Data.Images.Signture_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Signture_1_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                //Data.Images.Signture_1_AuditResult = lstAudits[index].AuditResult;
                                Data.Images.Signture_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                        }
                    }
                    else
                    {
                        //判斷AlreadyType有值的就設定
                        int Oindex = lstAudits.FindIndex(delegate (BE_AuditImage image)
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
                                    //Data.Images.ID_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.ID_1_AuditResult = lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : lstAudits[Oindex].AuditResult == 2 ? 1 : 0;
                                    Data.Images.ID_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 2:
                                    Data.Images.ID_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.ID_2_IsNew = 0;
                                    Data.Images.ID_2_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.ID_2_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.ID_2_AuditResult = lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : lstAudits[Oindex].AuditResult == 2 ? 1 : 0;
                                    Data.Images.ID_2_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 3:
                                    Data.Images.Car_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Car_1_IsNew = 0;
                                    Data.Images.Car_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Car_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Car_1_AuditResult = lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : lstAudits[Oindex].AuditResult == 2 ? 1 : 0;
                                    Data.Images.Car_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 4:
                                    Data.Images.Car_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Car_2_IsNew = 0;
                                    Data.Images.Car_2_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Car_2_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Car_2_AuditResult = lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : lstAudits[Oindex].AuditResult == 2 ? 1 : 0;
                                    Data.Images.Car_2_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 5:
                                    Data.Images.Motor_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Motor_1_IsNew = 0;
                                    Data.Images.Motor_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Motor_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Motor_1_AuditResult = lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : lstAudits[Oindex].AuditResult == 2 ? 1 : 0;
                                    Data.Images.Motor_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 6:
                                    Data.Images.Motor_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Motor_2_IsNew = 0;
                                    Data.Images.Motor_2_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Motor_2_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Motor_2_AuditResult = lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : lstAudits[Oindex].AuditResult == 2 ? 1 : 0;
                                    Data.Images.Motor_2_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 7:
                                    Data.Images.Self_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Self_1_IsNew = 0;
                                    Data.Images.Self_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Self_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Self_1_AuditResult = lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : lstAudits[Oindex].AuditResult == 2 ? 1 : 0;
                                    Data.Images.Self_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 8:
                                    Data.Images.F01 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.F01_IsNew = 0;
                                    Data.Images.F01_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.F01_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.F01_AuditResult = lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : lstAudits[Oindex].AuditResult == 2 ? 1 : 0;
                                    Data.Images.F01_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 9:
                                    Data.Images.Other_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Other_1_IsNew = 0;
                                    Data.Images.Other_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Other_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Other_1_AuditResult = lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : lstAudits[Oindex].AuditResult == 2 ? 1 : 0;
                                    Data.Images.Other_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 10:
                                    Data.Images.Business_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Business_1_IsNew = 0;
                                    Data.Images.Business_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Business_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Business_1_AuditResult = lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : lstAudits[Oindex].AuditResult == 2 ? 1 : 0;
                                    Data.Images.Business_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 11:
                                    Data.Images.Signture_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Signture_1_IsNew = 0;
                                    Data.Images.Signture_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Signture_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Signture_1_AuditResult = lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : lstAudits[Oindex].AuditResult == 2 ? 1 : 0;
                                    Data.Images.Signture_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                            }
                        }
                    }
                }
            }

            return View(Data);
        }
        #endregion

        # region 會員修改
        /// <summary>
        /// 會員修改
        /// </summary>
        /// <returns></returns>
        public ActionResult ModifyMember()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ModifyMember(int AuditMode, int AuditType, string StartDate, string EndDate, int AuditReuslt, string UserName, string IDNO, string[] IDNOSuff, string AuditError)
        {
            ViewData["AuditMode"] = AuditMode;
            ViewData["AuditType"] = AuditType;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["AuditReuslt"] = AuditReuslt;
            ViewData["UserName"] = UserName;
            ViewData["IDNO"] = IDNO;
            ViewData["IDNOSuff"] = (IDNOSuff == null) ? "" : string.Join(",", IDNOSuff);
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
            List<BE_GetAuditList> lstData = new List<BE_GetAuditList>();
            if (UserName != "" || IDNO != "")
            {
                lstData = new MemberRepository(connetStr).GetAuditLists(AuditMode, AuditType, StartDate, EndDate, AuditReuslt, UserName, IDNO, IDNoSuffCombind, AuditError);
            }

            return View(lstData);
        }
        [HttpPost]
        public ActionResult ModifyMemberDetail(string AuditIDNO, string UserName, string Mobile, string Power, string MEMEMAIL, string HasVaildEMail, string MEMMSG)
        {
            if (UserName != null && Session["Account"] != null)
            {
                List<BE_AuditImage> lstAuditsxx = new MemberRepository(connetStr).UpdateMemberData(AuditIDNO, UserName, Mobile, Power, MEMEMAIL, HasVaildEMail, MEMMSG, Session["Account"].ToString());
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
            List<BE_AuditImage> lstAudits = new MemberRepository(connetStr).GetAuditImage(AuditIDNO);
            List<BE_AuditHistory> lstHistory = new MemberRepository(connetStr).GetAuditHistory(AuditIDNO);
            List<BE_InsuranceData> lstInsuranceData = new MemberRepository(connetStr).GetGetInsuranceData(AuditIDNO);
            List<BE_SameMobileData> lstMobile = null;
            List<BE_MileStone> lstMileStone = new MemberRepository(connetStr).GetMileStone(AuditIDNO);
            List<BE_MileStoneDetail> lstMileStoneDetail = new MemberRepository(connetStr).GetMileStoneDetail(AuditIDNO);
            List<BE_MemberScore> lstMemberScore = new MemberRepository(connetStr).GetMemberScore(AuditIDNO);
            List<BE_ScoreBlock> lstScoreBlock = new MemberRepository(connetStr).GetScoreBlock(AuditIDNO);

            //Newtonsoft.Json序列化
            string jsonData = JsonConvert.SerializeObject(lstMileStoneDetail);
            Data.JsonMileStoneDetail = jsonData;

            string mobileBlock = ""; //20210310唐加
            Data.RecommendHistory = new List<BE_AuditRecommendHistory>();
            Data.History = new List<BE_AuditHistory>();
            Data.History = lstHistory;
            Data.MileStone = new List<BE_MileStone>();
            Data.MileStone = lstMileStone;
            //Data.MileStoneDetail = new List<BE_MileStoneDetail>();
            //Data.MileStoneDetail = lstMileStoneDetail;
            Data.MemberScore = new List<BE_MemberScore>();
            Data.MemberScore = lstMemberScore;
            Data.InsuranceData = lstInsuranceData;
            Data.ScoreBlock = lstScoreBlock;

            BaseParams param = new BaseParams();
            string returnMessage = "";
            var parms = new object[]
            {AuditIDNO};
            DataSet ds = WebApiClient.SPRetB(ServerInfo.GetServerInfo(param), "LS..SP_LSQ270_P01", parms, ref returnMessage);

            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Data.RecommendHistory.Add(new BE_AuditRecommendHistory
                    {
                        gift_name = ds.Tables[0].Rows[i]["獎勵名稱"] == null ? "" : ds.Tables[0].Rows[i]["獎勵名稱"].ToString(),
                        gift_mins = ds.Tables[0].Rows[i]["獎勵時數"] == null ? "" : ds.Tables[0].Rows[i]["獎勵時數"].ToString(),
                        U_SYSDT = ds.Tables[0].Rows[i]["獎勵時間"] == null ? "" : ds.Tables[0].Rows[i]["獎勵時間"].ToString(),
                        recommended = ds.Tables[0].Rows[i]["被推薦人"] == null ? "" : ds.Tables[0].Rows[i]["被推薦人"].ToString(),
                        recommend = ds.Tables[0].Rows[i]["推薦人"] == null ? "" : ds.Tables[0].Rows[i]["推薦人"].ToString(),
                        memrfbnr = ds.Tables[0].Rows[i]["推薦碼"] == null ? "" : ds.Tables[0].Rows[i]["推薦碼"].ToString()
                    });
                }
            }


            Data.SameMobile = new List<BE_SameMobileData>();
            Data.mobileBlock = "";//20210310唐加
            if (obj != null)
            {
                lstMobile = new MemberRepository(connetStr).GetSameMobile(AuditIDNO, obj.MEMTEL);
                Data.SameMobile = lstMobile;
                //20210310唐加
                mobileBlock = new MemberRepository(connetStr).GetMobileBlock(obj.MEMTEL);
                Data.mobileBlock = mobileBlock;
            }
            Data.detail = new BE_AuditDetail();
            Data.detail = obj;
            Data.detail.SPED = obj.SPED != "" ? obj.SPED.Substring(0, 4) + "-" + obj.SPED.Substring(4, 2) + "-" + obj.SPED.Substring(6, 2) : obj.SPED;
            Data.detail.SPSD = obj.SPSD != "" ? obj.SPSD.Substring(0, 4) + "-" + obj.SPSD.Substring(4, 2) + "-" + obj.SPSD.Substring(6, 2) : obj.SPSD;

            if (lstAudits != null)//若有抓出圖片資訊就跑下面這段
            {
                Data.Images = new BE_AuditCrentials(); //宣告每張照片的屬性
                for (int i = 1; i <= 11; i++)
                {
                    //判斷CrentialsType有值的就設定
                    int index = lstAudits.FindIndex(delegate (BE_AuditImage image)
                    {
                        return image.CrentialsType == i;
                    });
                    if (index > -1)
                    {
                        switch (i)
                        {
                            //用UPDTime去交叉判斷，到底為何要這麼麻煩? 20210401唐改AuditResult直接抓sp回傳值
                            //若UPDTime是空(表示tmp沒資料)，則AuditResult=sp傳回的AuditResult。若UPDTime不是空，則AuditResult=(若sp傳回的AuditResult不是1且有註記失敗原因則=-1，不然就0)
                            case 1:
                                Data.Images.ID_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.ID_1_IsNew = 1;
                                Data.Images.ID_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.ID_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.ID_1_AuditResult = lstAudits[index].AuditResult;
                                Data.Images.ID_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 2:
                                Data.Images.ID_2 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.ID_2_IsNew = 1;
                                Data.Images.ID_2_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.ID_2_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.ID_2_AuditResult = lstAudits[index].AuditResult;
                                Data.Images.ID_2_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 3:
                                Data.Images.Car_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Car_1_IsNew = 1;
                                Data.Images.Car_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Car_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Car_1_AuditResult = lstAudits[index].AuditResult;
                                Data.Images.Car_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 4:
                                Data.Images.Car_2 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Car_2_IsNew = 1;
                                Data.Images.Car_2_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Car_2_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Car_2_AuditResult = lstAudits[index].AuditResult;
                                Data.Images.Car_2_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 5:
                                Data.Images.Motor_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Motor_1_IsNew = 1;
                                Data.Images.Motor_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Motor_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Motor_1_AuditResult = lstAudits[index].AuditResult;
                                Data.Images.Motor_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 6:
                                Data.Images.Motor_2 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Motor_2_IsNew = 1;
                                Data.Images.Motor_2_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Motor_2_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Motor_2_AuditResult = lstAudits[index].AuditResult;
                                Data.Images.Motor_2_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 7:
                                Data.Images.Self_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Self_1_IsNew = 1;
                                Data.Images.Self_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Self_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Self_1_AuditResult = lstAudits[index].AuditResult;
                                Data.Images.Self_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 8:
                                Data.Images.F01 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.F01_IsNew = 1;
                                Data.Images.F01_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.F01_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.F01_AuditResult = lstAudits[index].AuditResult;
                                Data.Images.F01_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 9:
                                Data.Images.Other_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Other_1_IsNew = 1;
                                Data.Images.Other_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Other_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Other_1_AuditResult = lstAudits[index].AuditResult;
                                Data.Images.Other_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 10:
                                Data.Images.Business_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Business_1_IsNew = 1;
                                Data.Images.Business_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //Data.Images.Business_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                Data.Images.Business_1_AuditResult = 0;
                                Data.Images.Business_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                            case 11:
                                Data.Images.Signture_1 = (lstAudits[index].CrentialsFile == "") ? "" : (lstAudits[index].CrentialsFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[index].CrentialsFile;
                                Data.Images.Signture_1_IsNew = 1;
                                Data.Images.Signture_1_UPDTime = lstAudits[index].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[index].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                //20210120唐改，簽名檔沒有UPDTIME會產生BUG，所以拿掉UPDTIME判定
                                //Data.Images.Signture_1_AuditResult = lstAudits[index].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : 0) : lstAudits[index].AuditResult;
                                //Data.Images.Signture_1_AuditResult = lstAudits[index].AuditResult != 1 && lstAudits[index].RejectReason != "" ? -1 : lstAudits[index].AuditResult == 2 ? 1 : 0;
                                Data.Images.Signture_1_AuditResult = lstAudits[index].AuditResult;
                                Data.Images.Signture_1_RejectReason = lstAudits[index].AuditResult == 1 ? "" : lstAudits[index].RejectReason;
                                break;
                        }
                    }
                    else
                    {
                        //判斷AlreadyType有值的就設定
                        int Oindex = lstAudits.FindIndex(delegate (BE_AuditImage image)
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
                                    //Data.Images.ID_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.ID_1_AuditResult = lstAudits[Oindex].AuditResult;
                                    Data.Images.ID_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 2:
                                    Data.Images.ID_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.ID_2_IsNew = 0;
                                    Data.Images.ID_2_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.ID_2_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.ID_2_AuditResult = lstAudits[Oindex].AuditResult;
                                    Data.Images.ID_2_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 3:
                                    Data.Images.Car_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Car_1_IsNew = 0;
                                    Data.Images.Car_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Car_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Car_1_AuditResult = lstAudits[Oindex].AuditResult;
                                    Data.Images.Car_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 4:
                                    Data.Images.Car_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Car_2_IsNew = 0;
                                    Data.Images.Car_2_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Car_2_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Car_2_AuditResult = lstAudits[Oindex].AuditResult;
                                    Data.Images.Car_2_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 5:
                                    Data.Images.Motor_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Motor_1_IsNew = 0;
                                    Data.Images.Motor_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Motor_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Motor_1_AuditResult = lstAudits[Oindex].AuditResult;
                                    Data.Images.Motor_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 6:
                                    Data.Images.Motor_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Motor_2_IsNew = 0;
                                    Data.Images.Motor_2_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Motor_2_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Motor_2_AuditResult = lstAudits[Oindex].AuditResult;
                                    Data.Images.Motor_2_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 7:
                                    Data.Images.Self_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Self_1_IsNew = 0;
                                    Data.Images.Self_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Self_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Self_1_AuditResult = lstAudits[Oindex].AuditResult;
                                    Data.Images.Self_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 8:
                                    Data.Images.F01 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.F01_IsNew = 0;
                                    Data.Images.F01_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.F01_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.F01_AuditResult = lstAudits[Oindex].AuditResult;
                                    Data.Images.F01_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 9:
                                    Data.Images.Other_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Other_1_IsNew = 0;
                                    Data.Images.Other_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Other_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Other_1_AuditResult = lstAudits[Oindex].AuditResult;
                                    Data.Images.Other_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 10:
                                    Data.Images.Business_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Business_1_IsNew = 0;
                                    Data.Images.Business_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Business_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Business_1_AuditResult = lstAudits[Oindex].AuditResult;
                                    Data.Images.Business_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                                case 11:
                                    Data.Images.Signture_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : (lstAudits[Oindex].AlreadyFile.ToUpper().IndexOf("FTP") > -1 ? lsURL : ImgURL) + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Signture_1_IsNew = 0;
                                    Data.Images.Signture_1_UPDTime = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") == "00010101" ? "" : "上傳時間：" + lstAudits[Oindex].UPDTime.ToString("yyyy/MM/dd HH:mm:ss");
                                    //Data.Images.Signture_1_AuditResult = lstAudits[Oindex].UPDTime.ToString("yyyyMMdd") != "00010101" ? (lstAudits[Oindex].AuditResult != 1 && lstAudits[Oindex].RejectReason != "" ? -1 : 0) : lstAudits[Oindex].AuditResult;
                                    Data.Images.Signture_1_AuditResult = lstAudits[Oindex].AuditResult;
                                    Data.Images.Signture_1_RejectReason = lstAudits[Oindex].AuditResult == 1 ? "" : lstAudits[Oindex].RejectReason;
                                    break;
                            }
                        }


                    }
                }
            }


            return View(Data);
        }
        #endregion

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

        # region 改密碼
        /// <summary>
        /// 改密碼
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string IDNO, string Password)
        {
            List<BE_GetAuditList> lstData = new List<BE_GetAuditList>();
            try
            {
                ViewData["IDNO"] = IDNO;
                ViewData["Password"] = Password;

                lstData = new MemberRepository(connetStr).ChangePassword(IDNO, Password);
                ViewData["ErrorMessage"] = "修改成功";
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
            }

            return View(lstData);
        }
        #endregion

        # region 刪除會員
        /// <summary>
        /// 刪除會員
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteMember()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteMember(string IDNO, string IRent_Only, string Account)
        {
            MemberRepository repository = new MemberRepository(connetStr);
            if (repository.IsMemberExist(IDNO))
            {
                repository.DeleteMember(IDNO, IRent_Only, Account);
                ViewData["result"] = true;
            }
            else
            {
                ViewData["result"] = false;
            }

            return View();
        }
        #endregion

        # region 修改身份證字號
        /// <summary>
        /// 修改身份證字號
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeID()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangeID(string TARGET_ID, string AFTER_ID, string Account)
        {
            MemberRepository repository = new MemberRepository(connetStr);
            if (repository.IsMemberExist(TARGET_ID))
            {
                repository.ChangeID(TARGET_ID, AFTER_ID, Account);
                ViewData["result"] = true;
            }
            else
            {
                ViewData["result"] = false;
            }
            return View();
        }
        #endregion

        # region 會員勳章
        public ActionResult MedalMileStone()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MedalMileStone(string AuditMode, string IDNO, string ChoiceSelect, HttpPostedFileBase fileImport,string MEMO_CONTENT)
        {
            ViewData["IDNO"] = IDNO;
            ViewData["AuditMode"] = AuditMode;

            //string errorLine = "";
            //string errorMsg = "";
            //ViewData["errorLine"] = ""; 
            //ViewData["errorMsg"] = ""; 

            if (AuditMode == "0")
            {
                //List<BE_MileStone> lstData = new MemberRepository(connetStr).GetMileStone(IDNO);
                //return View(lstData);

                BE_AuditDetailCombind Data = new BE_AuditDetailCombind();
                List<BE_MileStone> lstMileStone = new MemberRepository(connetStr).GetMileStone(IDNO);
                List<BE_MileStoneDetail> lstMileStoneDetail = new MemberRepository(connetStr).GetMileStoneDetail(IDNO);

                //Newtonsoft.Json序列化
                string jsonData = JsonConvert.SerializeObject(lstMileStoneDetail);

                Data.MileStone = new List<BE_MileStone>();
                Data.MileStone = lstMileStone;
                //Data.MileStoneDetail = new List<BE_MileStoneDetail>();
                //Data.MileStoneDetail = lstMileStoneDetail;
                Data.JsonMileStoneDetail = jsonData;
                return View(Data);
            }
            else if (AuditMode == "1")
            {
                ////MemberRepository obj = new MemberRepository(connetStr).InsertMileStone(IDNO,ChoiceSelect); //會錯
                //MemberRepository obj = new MemberRepository(connetStr);
                //obj.InsertMileStone(IDNO, ChoiceSelect);
                //return View();

                bool flag = true;
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                string errCode = "";
                CommonFunc baseVerify = new CommonFunc();
                SPInput_BE_IneInsMileStone data = new SPInput_BE_IneInsMileStone()
                {
                    IDNO = IDNO,
                    ChoiceSelect = ChoiceSelect,
                    USERID = Session["Account"].ToString(),
                    MEMO = MEMO_CONTENT
                };
                SPOutput_Base SPOutput = new SPOutput_Base();
                flag = new SQLHelper<SPInput_BE_IneInsMileStone, SPOutput_Base>(connetStr).ExecuteSPNonQuery("usp_BE_InsMileStone", data, ref SPOutput, ref lstError);
                baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    ViewData["errorLine"] = "ok";
                    //return Content("<script>alert('ok');</script>");
                }
                else
                {
                    ViewData["errorLine"] = "新增失敗";
                }

                return View();
            }
            else
            {
                string errorMsg = "";
                bool flag = true;

                List<SPInput_BE_IneInsMileStone> lstData = new List<SPInput_BE_IneInsMileStone>();
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                string errCode = "";
                CommonFunc baseVerify = new CommonFunc();
                if (fileImport != null)
                {
                    if (fileImport.ContentLength > 0)
                    {
                        string fileName = string.Concat(new string[]{
                            "MedalMileStoneImport",
                            ((Session["Account"]==null)?"":Session["Account"].ToString()),
                            "_",
                            DateTime.Now.ToString("yyyyMMddHHmmss"),
                            ".xlsx"
                        });
                        DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/MedalMileStoneImport"));
                        if (!di.Exists)
                        {
                            di.Create();
                        }
                        string path = Path.Combine(Server.MapPath("~/Content/upload/MedalMileStoneImport"), fileName);
                        fileImport.SaveAs(path);
                        IWorkbook workBook = new XSSFWorkbook(path);
                        ISheet sheet = workBook.GetSheetAt(0);
                        int sheetLen = sheet.LastRowNum;
                        string[] field = { "ID", "匯入項目", "備註" };
                        int fieldLen = field.Length;
                        //第一關，判斷位置是否相等
                        for (int i = 0; i < fieldLen; i++)
                        {
                            ICell headCell = sheet.GetRow(0).GetCell(i);
                            if (headCell.ToString().Replace(" ", "").ToUpper() != field[i])
                            {
                                errorMsg = "標題列不相符";
                                flag = false;
                                break;
                            }
                        }
                        //判斷action是不是BackStageInsert=1的那幾個
                        if (flag)
                        {
                            for (int i=1; i<=sheetLen; i++)
                            {
                                if ((sheet.GetRow(i).GetCell(1).ToString().Replace(" ", "")).ToUpper() != "REPORT" &&
                                    (sheet.GetRow(i).GetCell(1).ToString().Replace(" ", "")).ToUpper() != "QUESTION" &&
                                    (sheet.GetRow(i).GetCell(1).ToString().Replace(" ", "")).ToUpper() != "DEBUG" &&
                                    (sheet.GetRow(i).GetCell(1).ToString().Replace(" ", "")).ToUpper() != "ADVICE")
                                {
                                    errorMsg = "Action名稱錯誤";
                                    flag = false;
                                    break;
                                }
                            }
                        }

                        //通過第一關 
                        if (flag)
                        {
                            string UserId = ((Session["Account"] == null) ? "" : Session["Account"].ToString());
                            string memo2 = "";
                            //string SPName = new ObjType().GetSPName(ObjType.SPType.InsTransParking);
                            for (int i = 1; i <= sheetLen; i++)
                            {
                                if (sheet.GetRow(i).GetCell(2)==null)
                                {
                                    memo2 = "";
                                }
                                else
                                {
                                    memo2 = sheet.GetRow(i).GetCell(2).ToString().Replace(" ", "");
                                }
                                SPInput_BE_IneInsMileStone data = new SPInput_BE_IneInsMileStone()
                                {
                                    IDNO = sheet.GetRow(i).GetCell(0).ToString().Replace(" ", ""),
                                    ChoiceSelect = sheet.GetRow(i).GetCell(1).ToString().Replace(" ", ""),
                                    USERID = UserId,
                                    MEMO = memo2
                                };

                                SPOutput_Base SPOutput = new SPOutput_Base();
                                flag = new SQLHelper<SPInput_BE_IneInsMileStone, SPOutput_Base>(connetStr).ExecuteSPNonQuery("usp_BE_InsMileStone", data, ref SPOutput, ref lstError);
                                baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                if (flag == false)
                                {
                                    //errorLine = i.ToString();
                                    errorMsg = string.Format("寫入第{0}筆資料時，發生錯誤：{1}", i.ToString(), baseVerify.GetErrorMsg(errCode));
                                }
                            }
                        }
                    }
                    else
                    {
                        flag = false;
                        errorMsg = "請上傳要匯入的資料";
                    }
                }
                else
                {
                    flag = false;
                    errorMsg = "請上傳要匯入的資料";
                }
                if (flag)
                {
                    ViewData["errorLine"] = "ok";
                }
                else
                {
                    ViewData["errorLine"] = errorMsg;
                }
                return View();
            };
        }
        #endregion

        # region 會員積分
        public ActionResult MemberScore()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MemberScore(string AuditMode, string IDNO, string MEMNAME, string ORDERNO, string ORDERNO_I, string StartDate, string EndDate, string ChoiceSelect_2, string MEMSCORE,string sonmemo, FormCollection collection, HttpPostedFileBase fileImport)
        {
            ViewData["IDNO"] = IDNO;
            ViewData["AuditMode"] = AuditMode;
            ViewData["MEMNAME"] = MEMNAME;
            ViewData["ORDERNO"] = ORDERNO;
            ViewData["ORDERNO_I"] = ORDERNO_I;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["SCITEM"] = (collection["ddlOperatorGG"] == null) ? "0" : collection["ddlOperatorGG"].ToString() == "" ? "0" : collection["ddlOperatorGG"].ToString(); //讓view知道我所選的評分行為(主項)的值
            ViewData["SCMITEM"] = (collection["ddlUserGroup"] == null) ? "0" : collection["ddlUserGroup"].ToString() == "" ? "0" : collection["ddlUserGroup"].ToString();
            //ViewData["score"] = (collection["scoreman"] == null) ? "0" : collection["scoreman"].ToString() == "" ? "0" : collection["scoreman"].ToString();
            int justSearch = Convert.ToInt32(collection["justSearch"] == null || collection["justSearch"].ToString() == "" ? "0" : collection["justSearch"]);

            bool flag_H = true;
            if (ORDERNO != "")
            {
                if (ORDERNO.IndexOf("H") < 0)
                {
                    flag_H = false;
                }
                else
                {
                    ORDERNO = ORDERNO.ToUpper();
                    ORDERNO = ORDERNO.Replace("H", "");
                }
            }
            if (ORDERNO_I != "")
            {
                if (ORDERNO_I.IndexOf("H") < 0)
                {
                    flag_H = false;
                }
                else
                {
                    ORDERNO_I = ORDERNO_I.ToUpper();
                    ORDERNO_I = ORDERNO_I.Replace("H", "");
                }
            }

            if (flag_H)
            {
                if (AuditMode == "1")
                {
                    List<BE_GetMemberScoreFull> lstData = new MemberRepository(connetStr).GetMemberScoreFull(IDNO, MEMNAME, ORDERNO, StartDate, EndDate);
                    return View(lstData);
                }
                else if (AuditMode == "0" && justSearch == 0)
                {
                    if (ORDERNO_I == "")
                    {
                        ORDERNO_I = "0";
                    }
                    bool flag = true;
                    if (collection["ddlUserGroup"] == "0")
                    {
                        flag = false;
                        ViewData["errorLine"] = "評分行為未選擇";
                    }
                    if (flag)
                    {
                        List<ErrorInfo> lstError = new List<ErrorInfo>();
                        string errCode = "";
                        CommonFunc baseVerify = new CommonFunc();
                        SP_BE_InsMemberScore data = new SP_BE_InsMemberScore()
                        {
                            //NAME = MEMNAME,
                            ID = IDNO,
                            ORDERNO = Convert.ToInt32(ORDERNO_I),
                            DAD = collection["ddlOperatorGG"].ToString(),
                            SON = collection["ddlUserGroup"].ToString(),
                            SCORE = int.Parse(MEMSCORE),
                            //APP = ChoiceSelect_2,
                            USERID = Session["Account"].ToString(),
                            MEMO = sonmemo
                        };
                        SPOutput_Base SPOutput = new SPOutput_Base();
                        flag = new SQLHelper<SP_BE_InsMemberScore, SPOutput_Base>(connetStr).ExecuteSPNonQuery("usp_BE_InsMemberScore", data, ref SPOutput, ref lstError);
                        baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);

                        if (flag)
                        {
                            ViewData["errorLine"] = "ok";
                            //return Content("<script>alert('ok');</script>");
                        }
                        else
                        {
                            ViewData["errorLine"] = SPOutput.ErrorMsg;
                        }
                    }

                    return View();
                }
                else if (AuditMode == "0" && justSearch == 1)
                {
                    return View();
                }
                else if (AuditMode == "2")
                {
                    string errorMsg = "";
                    bool flag = true;

                    List<SP_Input_BE_InsMemberScore> lstData = new List<SP_Input_BE_InsMemberScore>();
                    List<ErrorInfo> lstError = new List<ErrorInfo>();
                    string errCode = "";
                    CommonFunc baseVerify = new CommonFunc();
                    if (fileImport != null)
                    {
                        if (fileImport.ContentLength > 0)
                        {
                            string fileName = string.Concat(new string[]{
                            "MemberScoreImport",
                            ((Session["Account"]==null)?"":Session["Account"].ToString()),
                            "_",
                            DateTime.Now.ToString("yyyyMMddHHmmss"),
                            ".xlsx"
                        });
                            DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/MemberScoreImport"));
                            if (!di.Exists)
                            {
                                di.Create();
                            }
                            string path = Path.Combine(Server.MapPath("~/Content/upload/MemberScoreImport"), fileName);
                            fileImport.SaveAs(path);
                            IWorkbook workBook = new XSSFWorkbook(path);
                            ISheet sheet = workBook.GetSheetAt(0);
                            int sheetLen = sheet.LastRowNum;
                            string[] field = { "ID", "評分行為(主項)", "評分行為(子項)", "加/扣分", "合約編號" };
                            int fieldLen = field.Length;
                            //第一關，判斷位置是否相等
                            for (int i = 0; i < fieldLen; i++)
                            {
                                ICell headCell = sheet.GetRow(0).GetCell(i);
                                if (headCell.ToString().Replace(" ", "").ToUpper() != field[i])
                                {
                                    errorMsg = "標題列不相符";
                                    flag = false;
                                    break;
                                }
                            }
                            //判斷合約編號是否有H
                            for (int i = 1; i <= sheetLen; i++)
                            {
                                if (sheet.GetRow(i).GetCell(4).ToString().Replace(" ", "").IndexOf("H") < 0)
                                {
                                    errorMsg = "第"+(i+1)+"筆合約編號格式錯誤";
                                    flag = false;
                                    break;
                                }
                            }
                            //通過第一關 
                            if (flag)
                            {
                                string UserId = ((Session["Account"] == null) ? "" : Session["Account"].ToString());
                                //string SPName = new ObjType().GetSPName(ObjType.SPType.InsTransParking);
                                for (int i = 1; i <= sheetLen; i++)
                                {
                                    SP_Input_BE_InsMemberScore data = new SP_Input_BE_InsMemberScore()
                                    {
                                        ID = sheet.GetRow(i).GetCell(0).ToString().Replace(" ", ""),
                                        ORDERNO = int.Parse(sheet.GetRow(i).GetCell(4).ToString().Replace(" ", "").ToUpper().Replace("H", "")),
                                        DAD = sheet.GetRow(i).GetCell(1).ToString().Replace(" ", ""),
                                        SON = sheet.GetRow(i).GetCell(2).ToString().Replace(" ", ""),
                                        SCORE = int.Parse(sheet.GetRow(i).GetCell(3).ToString().Replace(" ", "")),
                                        APP = "",
                                        USERID = Session["Account"].ToString(),
                                        MEMO = (sheet.GetRow(i).GetCell(1).ToString().Replace(" ", "") == "其他") ? sheet.GetRow(i).GetCell(2).ToString().Replace(" ", "") : ""
                                    };

                                    SPOutput_Base SPOutput = new SPOutput_Base();
                                    flag = new SQLHelper<SP_Input_BE_InsMemberScore, SPOutput_Base>(connetStr).ExecuteSPNonQuery("usp_BE_InsMemberScore", data, ref SPOutput, ref lstError);
                                    baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                    if (flag == false)
                                    {
                                        //errorLine = i.ToString();
                                        errorMsg = string.Format("寫入第{0}筆資料時，發生錯誤：{1}", i.ToString(), baseVerify.GetErrorMsg(errCode));
                                    }
                                }
                            }
                        }
                        else
                        {
                            flag = false;
                            errorMsg = "請上傳要匯入的資料";
                        }
                    }
                    else
                    {
                        flag = false;
                        errorMsg = "請上傳要匯入的資料";
                    }

                    if (flag)
                    {
                        ViewData["errorLine"] = "ok";
                    }
                    else
                    {
                        ViewData["errorLine"] = errorMsg;
                    }

                    return View();
                }
                else
                {
                    List<BE_GetMemberData> lstData = new MemberRepository(connetStr).GetMemberData_ForScore(ORDERNO_I);
                    try
                    {
                        ViewData["IDNO"] = lstData[0].IDNO;
                        ViewData["MEMNAME"] = lstData[0].NAME;
                        //ViewData["AuditMode"] = "1";
                        return View();
                    }
                    catch
                    {
                        ViewData["errorLine"] = "無此合約";
                        return View();
                    }
                };
            }
            else
            {
                ViewData["errorLine"] = "合約編號格式錯誤";
                return View();
            }
        }
        public ActionResult ExplodeMemberScore(string AuditMode, string ExplodeSDate, string ExplodeEDate, string ExplodeIDNO, string ExplodeNAME, string ExplodeORDER)
        {
            ViewData["IDNO"] = ExplodeIDNO;
            ViewData["AuditMode"] = AuditMode;
            ViewData["MEMNAME"] = ExplodeNAME;
            ViewData["ORDERNO"] = ExplodeORDER;
            ViewData["StartDate"] = ExplodeSDate;
            ViewData["EndDate"] = ExplodeEDate;

            //List<BE_GetMemberScoreFull> lstData = new MemberRepository(connetStr).GetMemberScoreFull(IDNO, MEMNAME, ORDERNO, StartDate, EndDate);
            List<BE_GetMemberScoreFull> lstData = null;
            MemberRepository repository = new MemberRepository(connetStr);

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");

            string[] headerField = { "姓名", "加/扣分時間", "評分行為(主項)", "評分行為(子項)", "加/扣分", "出車時間", "合約編號", "APP刪除", "刪除時間", "操作人員" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                //sheet.AutoSizeColumn(j);
            }
            lstData = repository.GetMemberScoreFull(ExplodeIDNO, ExplodeNAME, ExplodeORDER, ExplodeSDate, ExplodeEDate);
            int len = lstData.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstData[k].MEMCNAME);
                content.CreateCell(1).SetCellValue(lstData[k].A_SYSDT);
                content.CreateCell(2).SetCellValue(lstData[k].DAD);
                content.CreateCell(3).SetCellValue(lstData[k].SON);
                content.CreateCell(4).SetCellValue(lstData[k].SCORE);
                content.CreateCell(5).SetCellValue(lstData[k].TIME);
                content.CreateCell(6).SetCellValue(lstData[k].ORDERNO);
                content.CreateCell(7).SetCellValue(lstData[k].APP);
                content.CreateCell(8).SetCellValue(lstData[k].DEL);
                content.CreateCell(9).SetCellValue(lstData[k].GM);

            }
            //for (int l = 0; l < headerFieldLen; l++)
            //{
            //    sheet.AutoSizeColumn(l);
            //}
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            // workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "會員積分清單" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
        #endregion
    }
}