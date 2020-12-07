using Domain.SP.BE.Input;
using Domain.SP.Output;
using Domain.TB.BackEnd;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Enum;
using WebAPI.Models.BaseFunc;
using WebCommon;

namespace Web.Controllers
{
    /// <summary>
    /// 卡片管理
    /// </summary>
    public class CardDataInfoController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        /// <summary>
        /// 萬用卡管理
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public ActionResult MasterCardSetting(string CardNo, string UserID, string Mode, HttpPostedFileBase fileImport)
        {
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            CarCardCommonRepository CardRepository = new CarCardCommonRepository(connetStr);
            ViewData["CardNo"] = CardNo;
            ViewData["UserID"] = UserID;
            ViewData["Mode"] = Mode;

            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            if (Mode == "Query")
            {
                List<BE_MasterCardData> lstData = new List<BE_MasterCardData>();
                List<BE_MasterCarDataOfPart> tmpData = CardRepository.GetAllCardListByMaster(CardNo,UserID);
                if (tmpData != null)
                {
                    int tmpLen = tmpData.Count();
                    if (tmpLen > 0)
                    {
                        lstData.Add(new BE_MasterCardData()
                        {
                            CarData = new List<BE_MasterCardBindCar>()
                             {
                                  new BE_MasterCardBindCar()
                                  {
                                       CarNo=tmpData[0].CarNo.Replace(" ","")
                                  }
                             },
                            CardNo = tmpData[0].CardNo.Replace(" ", "").PadLeft(10, '0'),
                            ManagerId = tmpData[0].ManagerId
                        }); 
                    }
                    for(int i =1; i < tmpLen; i++)
                    {
                        int index = lstData.FindIndex(delegate (BE_MasterCardData t)
                          {
                              return t.ManagerId == tmpData[i].ManagerId && t.CardNo == tmpData[i].CardNo;
  
                          });
                        if (index > -1)
                        {
                            lstData[index].CarData.Add(new BE_MasterCardBindCar()
                            {
                                CarNo = tmpData[i].CarNo.Replace(" ", "")
                            });
                        }
                        else
                        {
                            lstData.Add(new BE_MasterCardData()
                            {
                                CarData = new List<BE_MasterCardBindCar>()
                             {
                                  new BE_MasterCardBindCar()
                                  {
                                       CarNo=tmpData[i].CarNo.Replace(" ","")
                                  }
                             },
                                CardNo = tmpData[i].CardNo.Replace(" ", "").PadLeft(10, '0'),
                                ManagerId = tmpData[i].ManagerId
                            });
                        }
                    }
                }
                return View(lstData);
            }
            else
            {
                if (fileImport != null)
                {
                    if (fileImport.ContentLength > 0)
                    {
                        CommonFunc baseVerify = new CommonFunc();
                        List<ErrorInfo> lstError = new List<ErrorInfo>();
                        string errCode = "";
                        string fileName = string.Concat(new string[]{
                    "ImportMasterCardData_",
                    ((Session["Account"]==null)?"":Session["Account"].ToString()),
                    "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx"
                    });
                        DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/ImportMasterCardData"));
                        if (!di.Exists)
                        {
                            di.Create();
                        }
                        string path = Path.Combine(Server.MapPath("~/Content/upload/ImportMasterCardData"), fileName);
                        fileImport.SaveAs(path);
                        IWorkbook workBook = new XSSFWorkbook(path);
                        ISheet sheet = workBook.GetSheetAt(0);
                        int sheetLen = sheet.LastRowNum;
                        string[] field = { "員工編號","卡號", "車號" };
                        int fieldLen = field.Length;
                        //第一關，判斷位置是否相等
                        for (int i = 0; i < fieldLen; i++)
                        {
                            ICell headCell = sheet.GetRow(0).GetCell(i);
                            if (headCell.ToString().Replace(" ", "").ToUpper() != field[i])
                            {
                                errorLine = "標題列不相符";
                                flag = false;
                                break;
                            }
                        }
                        //通過第一關 
                        if (flag)
                        {
                            string UserId = ((Session["Account"] == null) ? "" : Session["Account"].ToString());
                            string SPName = new ObjType().GetSPName(ObjType.SPType.ImportMasterCardData);
                            for (int i = 1; i <= sheetLen; i++)
                            {

                                SPInput_BE_ImportMasterCardData data = new SPInput_BE_ImportMasterCardData()
                                {
                                    ManagerId = sheet.GetRow(i).GetCell(0).ToString().Replace(" ", ""),
                                    CardNo = sheet.GetRow(i).GetCell(1).ToString().Replace(" ", ""),
                                    CarNo = sheet.GetRow(i).GetCell(2).ToString().Replace(" ", "").PadLeft(6, '0'),
                                    UserID = UserId,
                                    LogID = 0
                                };


                                if (data.CardNo.Trim()!="" && data.CarNo.Trim() != "")
                                {
                                    if (flag)
                                    {
                                        SPOutput_Base SPOutput = new SPOutput_Base();
                                        flag = new SQLHelper<SPInput_BE_ImportMasterCardData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                        baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                        if (flag == false)
                                        {
                                            errorLine = i.ToString();
                                            errorMsg = string.Format("寫入第{0}筆資料時，發生錯誤：{1}", i.ToString(), baseVerify.GetErrorMsg(errCode));
                                        }
                                        else
                                        {
                                            string[] ClientCardNo = new string[0];
                                            string[] UnivCard = new string[1];
                                            UnivCard[0] = data.CardNo;
                                            WebAPI.Controllers.SendCarCMDController.SendCarCmd(data.CarNo, 2, ClientCardNo, UnivCard, "0.0.0.0");
                                        }
                                    }

                                }

                            }

                        }
                        else
                        {
                            ViewData["errorMsg"] = "未上傳檔案";
                        }
                    }
                    else
                    {
                        flag = false;
                        errorMsg = "未上傳檔案";

                    }
                }
                else
                {
                    flag = false;
                    errorMsg = "未上傳檔案";
                }

                if (flag)
                {
                    ViewData["errorLine"] = "ok";
                }
                else
                {
                    ViewData["errorMsg"] = errorMsg;
                    ViewData["errorLine"] = errorLine.ToString();
                }
                return View();
            }
          
        }
        /// <summary>
        /// 發送卡號設定
        /// </summary>
        /// <returns></returns>
        public ActionResult SentCardSetting()
        {
            return View();
        }
        /// <summary>
        /// 會員卡號解除
        /// </summary>
        /// <returns></returns>
        public ActionResult UnBindCard()
        {
            return View();
        }
    }
}