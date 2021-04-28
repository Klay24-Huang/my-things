using Domain.SP.BE.Input;
using Domain.SP.Output;
using Domain.TB.BackEnd;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.output.FET;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Enum;
using Web.Models.Params.Search.Input;
using WebAPI.Models.BaseFunc;
using WebCommon;

namespace Web.Controllers
{
    /// <summary>
    /// 車輛管理
    /// </summary>
    public class CarDataInfoController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 車輛中控台
        /// </summary>
        /// <returns></returns>
        public ActionResult CarDashBoard()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CarDashBoard(FormCollection collection)
        {
            Input_CarDashBoard QueryData = null;
            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);
            List<BE_CarDashBoardData> lstData = new List<BE_CarDashBoardData>();
            if (collection["queryData"] != null)
            {
                ViewData["QueryData"] = collection["queryData"];
                QueryData = Newtonsoft.Json.JsonConvert.DeserializeObject<Input_CarDashBoard>(collection["queryData"].ToString());
                if (QueryData != null)
                {
                    lstData = carStatusCommon.GetDashBoard(QueryData.CarNo, QueryData.StationID, QueryData.ShowType, QueryData.Terms);
                }

            }
            return View(lstData);
        }
        /// <summary>
        /// 保有車輛設定
        /// </summary>
        /// <returns></returns>
        public ActionResult CarSetting()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CarSetting(string CarNo,string StationID,int ShowType=3)
        {
            //BE_CarSettingData
            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);
            ViewData["ShowType"] = ShowType;
            ViewData["CarNo"] = CarNo;
            ViewData["StationID"] = StationID;
            List<BE_CarSettingData> lstData = new List<BE_CarSettingData>();
            lstData = carStatusCommon.GetCarSettingData(CarNo, StationID, ShowType);
           
            return View(lstData);
            
        }
        /// <summary>
        /// 車輛車機綁定
        /// </summary>
        /// <returns></returns>
        public ActionResult CarBind()
        {
            //GetCarBindData
            return View();
        }
        [HttpPost]
        [Obsolete]
        public ActionResult CarBind(string CarNo,string CID,string BindStatus, string Mode, HttpPostedFileBase fileImport)
        {
            //GetCarBindData
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);
            ViewData["BindStatus"] = BindStatus;
            ViewData["CarNo"] = CarNo;
            ViewData["CID"] = CID;
            ViewData["Mode"] = Mode;
            
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            if (Mode == "Query" || Mode=="Explode")
            {
                List<BE_GetCarBindData> lstData = new List<BE_GetCarBindData>();
                lstData = carStatusCommon.GetCarBindData(CarNo, CID, Convert.ToInt32(BindStatus));
                if (Mode == "Explode")
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("搜尋結果");
                    string[] headerField = { "車機號碼", "使用狀態", "車號", "遠傳Cat平台token", "iButton", "門號(SIM卡)" };
                    int headerFieldLen = headerField.Length;
                    int DataLen = lstData.Count();
                    IRow header = sheet.CreateRow(0);
                    for (int j = 0; j < headerFieldLen; j++)
                    {
                       header.CreateCell(j).SetCellValue(headerField[j]);
                       sheet.AutoSizeColumn(j);
                    }
                    for (int i = 0; i < DataLen; i++){
                        //20201207唐加CID
                       string[] DataArr = { lstData[i].CID, ((lstData[i].BindStatus == 1) ? "已綁定" : "未綁定"), lstData[i].CarNo, lstData[i].deviceToken, lstData[i].iButtonKey, lstData[i].MobileNum };
                       IRow content = sheet.CreateRow(i + 1);  
                       for(int k=0;k< headerFieldLen; k++)
                       {
                           content.CreateCell(k).SetCellValue(DataArr[k]);
                       }
                    }
                    MemoryStream ms = new MemoryStream();
                    workbook.Write(ms);
                    workbook.Clear();
                    //   return View();
                    return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "車機車輛綁定資料匯出_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
                }
                else
                {
                    return View(lstData);
                }
                
            }
            else
            {
                //20210315 改用ImportCarBindData進行維護
                //if (fileImport != null)
                //{
                //    if (fileImport.ContentLength > 0)
                //    {
                //        CommonFunc baseVerify = new CommonFunc();
                //        List<ErrorInfo> lstError = new List<ErrorInfo>();
                //        string errCode = "";
                //        string fileName = string.Concat(new string[]{
                //    "ImportCarBindData_",
                //    ((Session["Account"]==null)?"":Session["Account"].ToString()),
                //    "_",
                //    DateTime.Now.ToString("yyyyMMddHHmmss"),
                //    ".xlsx"
                //    });
                //        DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/ImportCarBindData"));
                //        if (!di.Exists)
                //        {
                //            di.Create();
                //        }
                //        string path = Path.Combine(Server.MapPath("~/Content/upload/ImportCarBindData"), fileName);
                //        fileImport.SaveAs(path);
                //        IWorkbook workBook = new XSSFWorkbook(path);
                //        ISheet sheet = workBook.GetSheetAt(0);
                //        int sheetLen = sheet.LastRowNum;
                //        string[] field = { "車機編號", "車號", "IBUTTON" };//20201207唐改ibutton -> IBUTTON
                //        int fieldLen = field.Length;
                //        //第一關，判斷位置是否相等
                //        for (int i = 0; i < fieldLen; i++)
                //        {
                //            ICell headCell = sheet.GetRow(0).GetCell(i);
                //            if (headCell.ToString().Replace(" ", "").ToUpper() != field[i])
                //            {
                //                errorLine = "標題列不相符";
                //                flag = false;
                //                break;
                //            }
                //        }
                //        //通過第一關 
                //        if (flag)
                //        {
                //            string UserId = ((Session["Account"] == null) ? "" : Session["Account"].ToString());
                //            string SPName = new ObjType().GetSPName(ObjType.SPType.ImportCarBindData);
                //            for (int i = 1; i <= sheetLen; i++)
                //            {


                //                SPInput_BE_ImportCarBindData data = new SPInput_BE_ImportCarBindData()
                //                {
                //                    CID= sheet.GetRow(i).GetCell(0).ToString().Replace(" ", ""),
                //                    CarNo = sheet.GetRow(i).GetCell(1).ToString().Replace(" ", ""),
                //                    iButtonKey = sheet.GetRow(i).GetCell(2).ToString().Replace(" ", "").PadLeft(6, '0'),
                //                    UserID = UserId,
                //                    LogID = 0
                //                };


                //                if (flag)
                //                {
                //                    SPOutput_Base SPOutput = new SPOutput_Base();
                //                    flag = new SQLHelper<SPInput_BE_ImportCarBindData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                //                    baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                //                    if (flag == false)
                //                    {
                //                        errorLine = i.ToString();
                //                        errorMsg = string.Format("寫入第{0}筆資料時，發生錯誤：{1}", i.ToString(), baseVerify.GetErrorMsg(errCode));
                //                    }
                //                }

                //            }

                //        }
                //        else
                //        {
                //            ViewData["errorMsg"] = "未上傳檔案";
                //        }
                //    }
                //    else
                //    {
                //        flag = false;
                //        errorMsg = "未上傳檔案";

                //    }
                //}
                //else
                //{
                //    flag = false;
                //    errorMsg = "未上傳檔案";
                //}

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
        /// 車輛資料管理
        /// </summary>
        /// <returns></returns>
        public ActionResult CarDataSetting()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CarDataSetting(string isExport,string CarNo, string StationID, int ShowType = 3)
        {
            //BE_CarSettingData
            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);
            ViewData["CarNo"] = CarNo;
            ViewData["StationID"] = StationID;
            ViewData["ShowType"] = ShowType;
            List<BE_GetPartOfCarDataSettingData> lstData = new List<BE_GetPartOfCarDataSettingData>();
            lstData = carStatusCommon.GetCarDataSettingData(CarNo, StationID, ShowType);

            if(isExport == "true")
            {

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Sheet");

                int col = 1;
                sheet.Cells[1, col++].Value = "ID";
                sheet.Cells[1, col++].Value = "車號";
                sheet.Cells[1, col++].Value = "車型";
                sheet.Cells[1, col++].Value = "營運狀態";
                sheet.Cells[1, col++].Value = "CID";
                sheet.Cells[1, col++].Value = "目前庫位";
                sheet.Cells[1, col++].Value = "所屬庫位";
                sheet.Cells[1, col++].Value = "備註";

                int row = 2;
                
                foreach (var i in lstData)
                {
                    col = 1;
                    string NowStr = "";
                    sheet.Cells[row, col++].Value = row - 1;
                    sheet.Cells[row, col++].Value = i.CarNo;
                    sheet.Cells[row, col++].Value = i.CarTypeName;

                    if (i.NowStatus == 1)
                    {
                        NowStr = "可出租";
                    }
                    else if (i.NowStatus == 2)
                    {
                        NowStr = "待上線";
                    }
                    else if (i.NowStatus == 0)
                    {
                        NowStr = "出租中";
                    }

                    sheet.Cells[row, col++].Value = NowStr;
                    sheet.Cells[row, col++].Value = i.CID;
                    sheet.Cells[row, col++].Value = i.NowStationName;
                    sheet.Cells[row, col++].Value = i.StationName;
                    sheet.Cells[row, col++].Value = i.Memo;
                    row++;
                }

                MemoryStream fileStream = new MemoryStream();
                ep.SaveAs(fileStream);
                ep.Dispose();
                fileStream.Position = 0;
                return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "車輛資料管理.xlsx");
            }
            else
            {
                return View(lstData);
            }
        }
        [HttpPost]
        public ActionResult ViewCarDetail(string ShowCarNo)
        {
            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);

            BE_GetCarDetail obj  = null;
            obj = carStatusCommon.GetCarDataSettingDetail(ShowCarNo);
            return View(obj);
           
        }

        /// <summary>
        /// 匯出中控車輛記錄
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportCarSettingData()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ExportCarSettingData(string isExport, string StationID, string Time_Start, string Time_End)
        {
            CarStatusCommon carStatusCommon = new CarStatusCommon(connetStr);
            List<BE_CarSettingRecord> lstData = new List<BE_CarSettingRecord>();
            lstData = carStatusCommon.GetCarSettingRecord(StationID, Time_Start, Time_End);


            if (isExport == "true")
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Sheet");

                int col = 1;
                int row = 2;
                MemoryStream fileStream = new MemoryStream();

                if(StationID == "X0SR" || StationID == "X0R4" || StationID == "X0U4" || StationID == "X1V4")
                {
                    sheet.Cells[1, col++].Value = "時間";
                    sheet.Cells[1, col++].Value = "據點";
                    sheet.Cells[1, col++].Value = "總數量";
                    sheet.Cells[1, col++].Value = "出租中";
                    sheet.Cells[1, col++].Value = "可出租";
                    sheet.Cells[1, col++].Value = "待上線";
                    sheet.Cells[1, col++].Value = "低電量";
                    sheet.Cells[1, col++].Value = "一小時無回應";
                    sheet.Cells[1, col++].Value = "無回應";


                    foreach (var i in lstData)
                    {
                        col = 1;
                        sheet.Cells[row, col++].Value = i.Time.ToString("yyyy-MM-dd HH:mm:ss");
                        sheet.Cells[row, col++].Value = i.Station;
                        sheet.Cells[row, col++].Value = i.Total;
                        sheet.Cells[row, col++].Value = i.Renting;
                        sheet.Cells[row, col++].Value = i.OnBoard;
                        sheet.Cells[row, col++].Value = i.OffBoard;
                        sheet.Cells[row, col++].Value = i.Volt;
                        sheet.Cells[row, col++].Value = i.Nonresponse_OneHour;
                        sheet.Cells[row, col++].Value = i.Nonresponse;
                        row++;
                    }

                    ep.SaveAs(fileStream);
                    ep.Dispose();
                    fileStream.Position = 0;
                    return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{Time_Start}_to_{Time_End}_車輛中控記錄.xlsx");
                }

                sheet.Cells[1, col++].Value = "時間";
                sheet.Cells[1, col++].Value = "據點";
                sheet.Cells[1, col++].Value = "總數量";
                sheet.Cells[1, col++].Value = "出租中";
                sheet.Cells[1, col++].Value = "可出租";
                sheet.Cells[1, col++].Value = "待上線";
                sheet.Cells[1, col++].Value = "低電量(3TBA)";
                sheet.Cells[1, col++].Value = "低電量(2TBA)";
                sheet.Cells[1, col++].Value = "一小時無回應";
                sheet.Cells[1, col++].Value = "無回應";


                foreach (var i in lstData)
                {
                    col = 1;
                    sheet.Cells[row, col++].Value = i.Time.ToString("yyyy-MM-dd HH:mm:ss");
                    sheet.Cells[row, col++].Value = i.Station;
                    sheet.Cells[row, col++].Value = i.Total;
                    sheet.Cells[row, col++].Value = i.Renting;
                    sheet.Cells[row, col++].Value = i.OnBoard;
                    sheet.Cells[row, col++].Value = i.OffBoard;
                    sheet.Cells[row, col++].Value = i.LowBattery_3TBA;
                    sheet.Cells[row, col++].Value = i.LowBattery_2TBA;
                    sheet.Cells[row, col++].Value = i.Nonresponse_OneHour;
                    sheet.Cells[row, col++].Value = i.Nonresponse;
                    row++;
                }

                ep.SaveAs(fileStream);
                ep.Dispose();
                fileStream.Position = 0;
                return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{Time_Start}_to_{Time_End}_車輛中控記錄.xlsx");
            }
            else
            {
                return View(lstData);
            }
        }

        /// <summary>
        /// 匯入機車車輛檔
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportMotorData()
        {
            return View();
        }
        [Obsolete]
        [HttpPost]
        public ActionResult ImportMotorData(HttpPostedFileBase fileImport)
        {
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            List<SPInput_BE_InsTransParking> lstData = new List<SPInput_BE_InsTransParking>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string errCode = "";
            CommonFunc baseVerify = new CommonFunc();
            if (fileImport != null)
            {
                if (fileImport.ContentLength > 0)
                {
                    string fileName = string.Concat(new string[]{
                    "ImportMotoData_",
                    ((Session["Account"]==null)?"":Session["Account"].ToString()),
                    "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx"
                    });
                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/ImportMotoData"));
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    string path = Path.Combine(Server.MapPath("~/Content/upload/ImportMotoData"), fileName);
                    fileImport.SaveAs(path);
                    IWorkbook workBook = new XSSFWorkbook(path);
                    ISheet sheet = workBook.GetSheetAt(0);
                    int sheetLen = sheet.LastRowNum;
                    //20201207唐改全部大寫
                    string[] field = {"CARNO","TSEQNO","CARTYPE","SEAT","FACTORYYEAR","CARCOLOR","ENGINENO","BODYNO","CCNUM"};
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
                        string SPName = new ObjType().GetSPName(ObjType.SPType.ImportMotoData);
                        for (int i = 1; i <= sheetLen; i++)
                        {

                            //20210105_Eric_增加防呆，若全部空白則跳開
                            bool CheckNotAllNull = false;
                            for (int j = 0; j < 8; j++)
                            {
                                if (false == string.IsNullOrWhiteSpace(sheet.GetRow(i).GetCell(j).ToString()) || false == string.IsNullOrEmpty(sheet.GetRow(i).GetCell(j).ToString()))
                                {
                                    CheckNotAllNull = true;
                                    break;
                                }
                            }
                            if (CheckNotAllNull)
                            {
                                 SPInput_BE_ImportMotoData data = new SPInput_BE_ImportMotoData()
                                {

                                    CarNo = sheet.GetRow(i).GetCell(0).ToString().Replace(" ", ""),
                                    TSEQNO = Convert.ToInt32(sheet.GetRow(i).GetCell(1).ToString().Replace(" ", "")),
                                    CarType = sheet.GetRow(i).GetCell(2).ToString().Replace(" ", "").PadLeft(6,'0'),
                                    Seat = Convert.ToInt16(sheet.GetRow(i).GetCell(3).ToString().Replace(" ", "")),
                                    FactoryYear = sheet.GetRow(i).GetCell(4).ToString().Replace(" ", ""),
                                    CarColor = sheet.GetRow(i).GetCell(5).ToString().Replace(" ", ""),
                                    EngineNO = sheet.GetRow(i).GetCell(6).ToString().Replace(" ", ""),
                                    BodyNO = sheet.GetRow(i).GetCell(7).ToString().Replace(" ", ""),
                                    CCNum = Convert.ToInt32(sheet.GetRow(i).GetCell(8).ToString().Replace(" ", "")),
                                    IsMotor=1,
                                    UserID = UserId,
                                    LogID = 0
                                };


                                if (flag)
                                {
                                    SPOutput_Base SPOutput = new SPOutput_Base();
                                    flag = new SQLHelper<SPInput_BE_ImportMotoData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                    baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                    if (flag == false)
                                    {
                                        errorLine = i.ToString();
                                        errorMsg = string.Format("寫入第{0}筆資料時，發生錯誤：{1}", i.ToString(), baseVerify.GetErrorMsg(errCode));
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
        /// <summary>
        /// 匯入汽車車輛檔
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportCarData()
        {
            return View();
        }
        [Obsolete]
        [HttpPost]
        public ActionResult ImportCarData(HttpPostedFileBase fileImport)
        {
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            List<SPInput_BE_InsTransParking> lstData = new List<SPInput_BE_InsTransParking>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string errCode = "";
            CommonFunc baseVerify = new CommonFunc();
            if (fileImport != null)
            {
                if (fileImport.ContentLength > 0)
                {
                    string fileName = string.Concat(new string[]{
                    "ImportCarData_",
                    ((Session["Account"]==null)?"":Session["Account"].ToString()),
                    "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx"
                    });
                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/ImportCarData"));
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    string path = Path.Combine(Server.MapPath("~/Content/upload/ImportCarData"), fileName);
                    fileImport.SaveAs(path);
                    IWorkbook workBook = new XSSFWorkbook(path);
                    ISheet sheet = workBook.GetSheetAt(0);
                    int sheetLen = sheet.LastRowNum;
                    //20201207唐改大寫
                    string[] field = { "CARNO", "TSEQNO", "CARTYPE", "SEAT", "FACTORYYEAR", "CARCOLOR", "ENGINENO", "BODYNO", "CCNUM" };
                    int fieldLen = field.Length;
                    //第一關，判斷位置是否相等
                    for (int i = 0; i < fieldLen; i++)
                    {
                        ICell headCell = sheet.GetRow(0).GetCell(i);
                        if (headCell.ToString().Replace(" ", "").ToUpper() != field[i].ToUpper())
                        {
                            //errorLine = "標題列不相符";
                            errorMsg = "欄位內容與標題列不相符";
                            flag = false;
                            break;
                        }
                    }
                    //通過第一關 
                    if (flag)
                    {
                        string UserId = ((Session["Account"] == null) ? "" : Session["Account"].ToString());
                        string SPName = new ObjType().GetSPName(ObjType.SPType.ImportCarData);
                        for (int i = 1; i <= sheetLen; i++)
                        {
                            //20210105_Eric_增加防呆，若全部空白則跳開
                            bool CheckNotAllNull = false;
                            for (int j = 0; j < 8; j++)
                            {
                                if (false == string.IsNullOrWhiteSpace(sheet.GetRow(i).GetCell(j).ToString()) || false == string.IsNullOrEmpty(sheet.GetRow(i).GetCell(j).ToString()))
                                {
                                    CheckNotAllNull = true;
                                    break;
                                }
                            }
                            if (CheckNotAllNull)
                            {
                                SPInput_BE_ImportCarData data = new SPInput_BE_ImportCarData()
                                {

                                    CarNo = sheet.GetRow(i).GetCell(0).ToString().Replace(" ", ""),
                                    TSEQNO = Convert.ToInt32(sheet.GetRow(i).GetCell(1).ToString().Replace(" ", "")),
                                    CarType = sheet.GetRow(i).GetCell(2).ToString().Replace(" ", "").PadLeft(6, '0'),
                                    Seat = Convert.ToInt16(sheet.GetRow(i).GetCell(3).ToString().Replace(" ", "")),
                                    FactoryYear = sheet.GetRow(i).GetCell(4).ToString().Replace(" ", ""),
                                    CarColor = sheet.GetRow(i).GetCell(5).ToString().Replace(" ", ""),
                                    EngineNO = sheet.GetRow(i).GetCell(6).ToString().Replace(" ", ""),
                                    BodyNO = sheet.GetRow(i).GetCell(7).ToString().Replace(" ", ""),
                                    CCNum = Convert.ToInt32(sheet.GetRow(i).GetCell(8).ToString().Replace(" ", "")),
                                    IsMotor = 0,
                                    UserID = UserId,
                                    LogID = 0
                                };


                                if (flag)
                                {
                                    SPOutput_Base SPOutput = new SPOutput_Base();
                                    flag = new SQLHelper<SPInput_BE_ImportCarData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                    baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                    if (flag == false)
                                    {
                                        errorLine = i.ToString();
                                        errorMsg = string.Format("寫入第{0}筆資料時，發生錯誤：{1}", i.ToString(), baseVerify.GetErrorMsg(errCode));
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
        public ActionResult ImportCarMachineData()
        {
            return View();
        }
        /// <summary>
        /// 匯入車機資料（含SIM）
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        [HttpPost]
        public ActionResult ImportCarMachineData(HttpPostedFileBase fileImport)
        {
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;
            List<SPInput_BE_InsTransParking> lstData = new List<SPInput_BE_InsTransParking>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string errCode = "";
            CommonFunc baseVerify = new CommonFunc();
            if (fileImport != null)
            {
                if (fileImport.ContentLength > 0)
                {
                    string fileName = string.Concat(new string[]{
                    "ImportCarMachineData_",
                    ((Session["Account"]==null)?"":Session["Account"].ToString()),
                    "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx"
                    });
                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/ImportCarMachineData"));
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    string path = Path.Combine(Server.MapPath("~/Content/upload/ImportCarMachineData"), fileName);
                    fileImport.SaveAs(path);
                    IWorkbook workBook = new XSSFWorkbook(path);
                    ISheet sheet = workBook.GetSheetAt(0);
                    int sheetLen = sheet.LastRowNum;
                    //20201207唐改大寫
                    //20210105Eric加存放地點
                    string[] field = { "車機編號", "門號(遠傳)", "卡號(遠傳)","DEVICETOKEN","存放地點" };
                    int fieldLen = field.Length;
                    //第一關，判斷位置是否相等
                    for (int i = 0; i < fieldLen; i++)
                    {
                        ICell headCell = sheet.GetRow(0).GetCell(i);
                        if (headCell.ToString().Replace(" ", "").ToUpper() != field[i].ToUpper())
                        {
                            //errorLine = "標題列不相符";
                            errorMsg = "欄位內容與標題列不相符";
                            flag = false;
                            break;
                        }
                    }
                    //通過第一關 
                    if (flag)
                    {
                        string UserId = ((Session["Account"] == null) ? "" : Session["Account"].ToString());
                        string SPName = new ObjType().GetSPName(ObjType.SPType.ImportCarMachineData);
                        //20210105_Eric_增加防呆，若全部空白則跳開
                        for (int i = 1; i <= sheetLen; i++)
                        {
                            bool CheckNotAllNull = false;
                            for(int j = 0; j < 5; j++)
                            {
                                if(false==string.IsNullOrWhiteSpace(sheet.GetRow(i).GetCell(j).ToString()) || false == string.IsNullOrEmpty(sheet.GetRow(i).GetCell(j).ToString()))
                                {
                                    CheckNotAllNull = true;
                                    break;
                                }
                            }
                            if (CheckNotAllNull)
                            {
                                //20210105Eric加存放地點
                                SPInput_BE_ImportCarMachineData data = new SPInput_BE_ImportCarMachineData()
                                {

                                    CID = sheet.GetRow(i).GetCell(0).ToString().Replace(" ", ""),
                                    MobileNum = sheet.GetRow(i).GetCell(1).ToString().Replace(" ", ""),
                                    SIMCardNo = sheet.GetRow(i).GetCell(2).ToString().Replace(" ", ""),
                                    deviceToken = sheet.GetRow(i).GetCell(3).ToString().Replace(" ", ""),
                                    depositary = sheet.GetRow(i).GetCell(4).ToString().Replace(" ", ""),
                                    UserID = UserId,
                                    LogID = 0
                                };


                                if (flag)
                                {
                                    SPOutput_Base SPOutput = new SPOutput_Base();
                                    flag = new SQLHelper<SPInput_BE_ImportCarMachineData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                    baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                    if (flag == false)
                                    {
                                        errorLine = i.ToString();
                                        errorMsg = string.Format("寫入第{0}筆資料時，發生錯誤：{1}", i.ToString(), baseVerify.GetErrorMsg(errCode));
                                    }
                                }

                            }

                        }

                    }
                    else
                    {
                        ViewData["errorMsg"] = "未上傳檔案";
                    }
                }else{
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
        /// <summary>
        /// 匯入車機綁定資料
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportCarBindData()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ImportCarBindData(HttpPostedFileBase fileImport)
        {
            string errorLine = "";
            string errorMsg = "";
            bool flag = true;
            CarCardCommonRepository carCardCommonRepository = new CarCardCommonRepository(connetStr);
            FETDeviceMaintainAPI deviceMaintain = new FETDeviceMaintainAPI();
            List<BE_CarBindImportData> lstCarBindImportData = new List<BE_CarBindImportData>();
            List<BE_CarBindImportDataResult> lstCarBindImportDataResult = new List<BE_CarBindImportDataResult>();

            ViewData["errorLine"] = null;
            ViewData["IsShowMessage"] = null;

            if (fileImport != null)
            {
                if (fileImport.ContentLength > 0)
                {
                    CommonFunc baseVerify = new CommonFunc();
                    List<ErrorInfo> lstError = new List<ErrorInfo>();
                    string errCode = "";
                    string fileName = string.Concat(new string[]{
                    "ImportCarBindData_",
                    ((Session["Account"]==null)?"":Session["Account"].ToString()),
                    "_",
                    DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ".xlsx"
                    });
                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Content/upload/ImportCarBindData"));
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    string path = Path.Combine(Server.MapPath("~/Content/upload/ImportCarBindData"), fileName);
                    fileImport.SaveAs(path);
                    IWorkbook workBook = new XSSFWorkbook(path);
                    ISheet sheet = workBook.GetSheetAt(0);
                    int sheetLen = sheet.LastRowNum;
                    string[] field = { "車號", "車機編號", "IBUTTON", "門號(遠傳)", "卡號(遠傳)" };
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
                        //檢查Excel內容
                        for (int i = 1; i <= sheetLen; i++)
                        {
                            //增加防呆，若全部空白則跳開
                            bool CheckNotAllNull = false;
                            for (int j = 0; j < 6; j++)
                            {
                                if (sheet.GetRow(i).GetCell(j) != null && (false == string.IsNullOrWhiteSpace(sheet.GetRow(i).GetCell(j).ToString()) || false == string.IsNullOrEmpty(sheet.GetRow(i).GetCell(j).ToString())))
                                {
                                    CheckNotAllNull = true;
                                    break;
                                }
                            }

                            if (CheckNotAllNull)
                            {
                                lstCarBindImportData.Add(new BE_CarBindImportData
                                {
                                    CarNo = (sheet.GetRow(i).GetCell(0) != null) ? sheet.GetRow(i).GetCell(0).ToString().Trim() : "",
                                    CID = (sheet.GetRow(i).GetCell(1) != null) ? sheet.GetRow(i).GetCell(1).ToString().Trim() : "",
                                    IBUTTON = (sheet.GetRow(i).GetCell(2) != null) ? sheet.GetRow(i).GetCell(2).ToString().Trim() : "",
                                    MobileNum = (sheet.GetRow(i).GetCell(3) != null) ? sheet.GetRow(i).GetCell(3).ToString().Trim() : "",
                                    SIMCardNo = (sheet.GetRow(i).GetCell(4) != null) ? sheet.GetRow(i).GetCell(4).ToString().Trim() : ""
                                });
                            }
                        }

                        if (flag)
                        {
                            if (lstCarBindImportData.Count > 0)
                            {
                                //取得CAT平台Token
                                var loginCAT = deviceMaintain.DoLogin();
                                if (!loginCAT.Result)
                                {
                                    flag = false;
                                    errorMsg = "取得CAT API Token失敗。" + loginCAT.Message;
                                }
                                else
                                {
                                    //查詢CarInfo資料
                                    var lstCarInfoForMachineData = carCardCommonRepository.GetCarInfoForMachineData("");
                                    foreach (var importData in lstCarBindImportData)
                                    {
                                        bool importFlag = true;
                                        string importMessage = "";
                                        if (string.IsNullOrEmpty(importData.CarNo) ||
                                            string.IsNullOrEmpty(importData.CID))
                                        {
                                            importFlag = false;
                                            importMessage = "[車號]、[車機編號]不可為空白";
                                        }
                                        else
                                        {
                                            var lstCarInfoData = (from data in lstCarInfoForMachineData
                                                                  where data.CarNo == importData.CarNo
                                                                  select data).ToList();

                                            if (lstCarInfoData.Count() == 0)
                                            {
                                                importFlag = false;
                                                importMessage = "[車號]查無資料";
                                            }
                                            else
                                            {
                                                BE_CarInfoForMachineData carInfoData = lstCarInfoData[0];
                                                if (carInfoData.HasIButton == 1 && importData.IBUTTON == "")
                                                {
                                                    importFlag = false;
                                                    importMessage = "[iButton]不可為空白";
                                                }
                                                if (importData.CID.Length != 4 && importData.CID.Length != 5)
                                                {
                                                    importFlag = false;
                                                    importMessage = (carInfoData.IsCens == 1) ? "興聯車機[CID]應為5碼" : "遠傳車機[CID]應為4碼";
                                                }
                                                if (carInfoData.IsCens == 1)
                                                {
                                                    //興聯車機直接更新資料
                                                    if (importFlag)
                                                    {
                                                        string SPName = new ObjType().GetSPName(ObjType.SPType.ImportCarBindData);
                                                        //更新資料
                                                        SPInput_BE_ImportCarBindData data = new SPInput_BE_ImportCarBindData()
                                                        {
                                                            CID = importData.CID,
                                                            CarNo = carInfoData.CarNo,
                                                            iButtonKey = importData.IBUTTON,
                                                            MobileNum = importData.MobileNum,
                                                            SIMCardNo = importData.SIMCardNo,
                                                            UserID = UserId,
                                                            LogID = 0
                                                        };

                                                        SPOutput_Base SPOutput = new SPOutput_Base();
                                                        flag = new SQLHelper<SPInput_BE_ImportCarBindData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                                        baseVerify.checkSQLResult(ref importFlag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                                        if (!importFlag)
                                                        {
                                                            importMessage = string.Format("更新車機綁定資訊:{0}", baseVerify.GetErrorMsg(errCode));
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //遠傳車機
                                                    //更新CAT資料，產生deviceId與deviceToken
                                                    if (importFlag)
                                                    {
                                                        if (carInfoData.deviceId == "" || carInfoData.deviceToken == "")
                                                        {
                                                            WebAPIOutput_ResultDTO<WebAPIOutput_AddDeviceToken> addDeviceToken = deviceMaintain.AddDeviceToken(importData.CarNo,
                                                                carInfoData.IsMotor == 0 ? FETDeviceMaintainAPI.CATCarType.Car : FETDeviceMaintainAPI.CATCarType.Motor);
                                                            if (addDeviceToken.Result)
                                                            {
                                                                carInfoData.deviceId = addDeviceToken.Data.deviceId;
                                                                carInfoData.deviceToken = addDeviceToken.Data.deviceToken;

                                                                //更新deviceId與deviceToken
                                                                string SPName = new ObjType().GetSPName(ObjType.SPType.UpdCATDeviceToken);
                                                                //更新資料
                                                                SPInput_BE_UpdCATDeviceToken data = new SPInput_BE_UpdCATDeviceToken()
                                                                {
                                                                    CarNo = carInfoData.CarNo,
                                                                    deviceId = addDeviceToken.Data.deviceId,
                                                                    deviceToken = addDeviceToken.Data.deviceToken,
                                                                    UserID = UserId,
                                                                    LogID = 0
                                                                };

                                                                SPOutput_Base SPOutput = new SPOutput_Base();
                                                                flag = new SQLHelper<SPInput_BE_UpdCATDeviceToken, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                                                baseVerify.checkSQLResult(ref importFlag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                                                if (!importFlag)
                                                                {
                                                                    importMessage = string.Format("更新deviceToken:{0}", baseVerify.GetErrorMsg(errCode));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                importFlag = false;
                                                                importMessage = string.Format("CAT:{0}", addDeviceToken.Message);
                                                            }
                                                        }
                                                    }
                                                    //更新GCP資料
                                                    if (importFlag)
                                                    {
                                                        List<WebAPIInput_GCPUpMapping> lstGCPUpMapping = new List<WebAPIInput_GCPUpMapping>();
                                                        lstGCPUpMapping.Add(new WebAPIInput_GCPUpMapping
                                                        {
                                                            deviceCID = importData.CID,
                                                            deviceName = carInfoData.CarNo,
                                                            deviceToken = carInfoData.deviceToken,
                                                            deviceType = carInfoData.IsMotor == 0 ? FETDeviceMaintainAPI.GCPCarType.Vehicle.ToString() : FETDeviceMaintainAPI.GCPCarType.Motorcycle.ToString()
                                                        });
                                                        WebAPIOutput_ResultDTO<List<WebAPIOutput_GCPUpMapping>> GCPUpMapping = deviceMaintain.GCPUpMapping(lstGCPUpMapping);
                                                        if (!GCPUpMapping.Result || GCPUpMapping.Data.Count == 0 || GCPUpMapping.Data[0].upResult == "NotOkay")
                                                        {
                                                            importFlag = false;
                                                            importMessage = "GCP資料錯誤";
                                                        }
                                                        else
                                                        {
                                                            string SPName = new ObjType().GetSPName(ObjType.SPType.ImportCarBindData);
                                                            //更新資料
                                                            SPInput_BE_ImportCarBindData data = new SPInput_BE_ImportCarBindData()
                                                            {
                                                                CID = importData.CID,
                                                                CarNo = carInfoData.CarNo,
                                                                iButtonKey = importData.IBUTTON,
                                                                MobileNum = importData.MobileNum,
                                                                SIMCardNo = importData.SIMCardNo,
                                                                UserID = UserId,
                                                                LogID = 0
                                                            };

                                                            SPOutput_Base SPOutput = new SPOutput_Base();
                                                            flag = new SQLHelper<SPInput_BE_ImportCarBindData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                                            baseVerify.checkSQLResult(ref importFlag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                                            if (!importFlag)
                                                            {
                                                                importMessage = string.Format("更新車機綁定資訊:{0}", baseVerify.GetErrorMsg(errCode));
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        lstCarBindImportDataResult.Add(new BE_CarBindImportDataResult
                                        {
                                            CID = importData.CID,
                                            CarNo = importData.CarNo,
                                            Result = importFlag,
                                            Message = importMessage
                                        });
                                        flag = true;
                                    }
                                }
                            }
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
            return View(lstCarBindImportDataResult);
        }
    }
}