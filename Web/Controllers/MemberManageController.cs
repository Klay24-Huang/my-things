﻿using Domain.TB.BackEnd;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult Audit(int AuditMode,int AuditType,string StartDate,string EndDate,int AuditReuslt,string UserName,string IDNO,string[] IDNOSuff)
        {
            ViewData["AuditMode"] = AuditMode;
            ViewData["AuditType"] = AuditType;
            ViewData["StartDate"] = StartDate;
            ViewData["EndDate"] = EndDate;
            ViewData["AuditReuslt"] = AuditReuslt;
            ViewData["UserName"] = UserName;
            ViewData["IDNO"] = IDNO;
            ViewData["IDNOSuff"] =(IDNOSuff==null)?"": string.Join(",", IDNOSuff);
            string IDNoSuffCombind = "";
            if (IDNOSuff != null)
            {
                if (IDNOSuff.Length > 0)
                {
                    IDNoSuffCombind += string.Format("'{0}'", IDNOSuff[0]);
                    int IDLEN = IDNOSuff.Length;
                    for (int i = 1; i < IDLEN; i++)
                    {
                        IDNoSuffCombind += string.Format(",'{0}'", IDNOSuff[i]);
                    }
                }
            }
        
            List<BE_GetAuditList> lstData= new MemberRepository(connetStr).GetAuditLists(AuditMode, AuditType, StartDate, EndDate, AuditReuslt, UserName, IDNO, IDNoSuffCombind);

            return View(lstData);
        }
        [HttpPost]
        public ActionResult AuditDetail(string AuditIDNO)
        {
            string ImgURL = StorageBaseURL + credentialContainer + "/";
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
            List<BE_SameMobileData> lstMobile = null;
            Data.History = new List<BE_AuditHistory>();
            Data.History = lstHistory;
            Data.SameMobile = new List<BE_SameMobileData>();
            if (obj != null)
            {
                lstMobile = new MemberRepository(connetStr).GetSameMobile(AuditIDNO, obj.MEMTEL);
                Data.SameMobile = lstMobile;
            }
            Data.detail = new BE_AuditDetail();
            Data.detail = obj;
            if (lstAudits != null)
            {

                Data.Images = new BE_AuditCrentials();
                for(int i = 1; i < 11; i++)
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
                                Data.Images.ID_1 = (lstAudits[index].CrentialsFile == "") ? "" : ImgURL + lstAudits[index].CrentialsFile;
                                Data.Images.ID_1_IsNew = 1;
                                break;
                            case 2:
                                Data.Images.ID_2 = (lstAudits[index].CrentialsFile == "") ? "" : ImgURL + lstAudits[index].CrentialsFile;
                                Data.Images.ID_2_IsNew = 1;
                                break;
                            case 3:
                                Data.Images.Car_1 = (lstAudits[index].CrentialsFile == "") ? "" : ImgURL + lstAudits[index].CrentialsFile;
                                Data.Images.Car_1_IsNew = 1;
                                break;
                            case 4:
                                Data.Images.Car_2 = (lstAudits[index].CrentialsFile == "") ? "" : ImgURL + lstAudits[index].CrentialsFile;
                                Data.Images.Car_2_IsNew = 1;
                                break;
                            case 5:
                                Data.Images.Motor_1 = (lstAudits[index].CrentialsFile == "") ? "" : ImgURL + lstAudits[index].CrentialsFile;
                                Data.Images.Motor_1_IsNew = 1;
                                break;
                            case 6:
                                Data.Images.Motor_2 = (lstAudits[index].CrentialsFile == "") ? "" : ImgURL + lstAudits[index].CrentialsFile;
                                Data.Images.Motor_2_IsNew = 1;
                                break;
                            case 7:
                                Data.Images.Self_1 = (lstAudits[index].CrentialsFile == "") ? "" : ImgURL + lstAudits[index].CrentialsFile;
                                Data.Images.Self_1_IsNew = 1;
                                break;
                            case 8:
                                Data.Images.F01 = (lstAudits[index].CrentialsFile == "") ? "" : ImgURL + lstAudits[index].CrentialsFile;
                                Data.Images.F01_IsNew = 1;
                                break;
                            case 9:
                                Data.Images.Other_1 = (lstAudits[index].CrentialsFile == "") ? "" : ImgURL + lstAudits[index].CrentialsFile;
                                Data.Images.Other_1_IsNew = 1;
                                break;
                            case 10:
                                Data.Images.Business_1 = (lstAudits[index].CrentialsFile == "") ? "" : ImgURL + lstAudits[index].CrentialsFile;
                                Data.Images.Business_1_IsNew = 1;
                                break;
                            case 11:
                                Data.Images.Signture_1 = (lstAudits[index].CrentialsFile == "") ? "" : ImgURL + lstAudits[index].CrentialsFile;
                                Data.Images.Signture_1_IsNew = 1;
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
                                    Data.Images.ID_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : ImgURL + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.ID_1_IsNew = 0;
                                    break;
                                case 2:
                                    Data.Images.ID_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : ImgURL + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.ID_2_IsNew = 0;
                                    break;
                                case 3:
                                    Data.Images.Car_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : ImgURL + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Car_1_IsNew = 0;
                                    break;
                                case 4:
                                    Data.Images.Car_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : ImgURL + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Car_2_IsNew = 0;
                                    break;
                                case 5:
                                    Data.Images.Motor_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : ImgURL + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Motor_1_IsNew = 0;
                                    break;
                                case 6:
                                    Data.Images.Motor_2 = (lstAudits[Oindex].AlreadyFile == "") ? "" : ImgURL + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Motor_2_IsNew = 0;
                                    break;
                                case 7:
                                    Data.Images.Self_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : ImgURL + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Self_1_IsNew = 0;
                                    break;
                                case 8:
                                    Data.Images.F01 = (lstAudits[Oindex].AlreadyFile == "") ? "" : ImgURL + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.F01_IsNew = 0;
                                    break;
                                case 9:
                                    Data.Images.Other_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : ImgURL + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Other_1_IsNew = 0;
                                    break;
                                case 10:
                                    Data.Images.Business_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : ImgURL + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Business_1_IsNew = 0;
                                    break;
                                case 11:
                                    Data.Images.Signture_1 = (lstAudits[Oindex].AlreadyFile == "") ? "" : ImgURL + lstAudits[Oindex].AlreadyFile;
                                    Data.Images.Signture_1_IsNew = 0;
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
    }
}