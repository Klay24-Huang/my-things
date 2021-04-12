using Domain.TB.BackEnd;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using WebAPI.Models.BaseFunc;//20210218唐加
//using WebCommon;//20210218唐加
//using Domain.SP.BE.Input;//20210218唐加
//using Domain.SP.BE.Output;//20210218唐加
//using Web.Models.Enum;//20210218唐加
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace Web.Controllers
{
    /// <summary>
    /// 地圖監控
    /// </summary>
    public class MonitorController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 地圖監控
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? Mode, string OrderNum, DateTime? start, DateTime? end, string objCar)
        {
            Int16 sMode = 2;
            List<BE_EvTimeLine> lstEv = null;
            if (Mode != null)
            {
                sMode = Convert.ToInt16(Mode);
                ViewData["Mode"] = sMode;
            }
            if (sMode < 2)
            {
                Int64 tmpOrder = (string.IsNullOrEmpty(OrderNum)) ? 0 : Convert.ToInt64(OrderNum.Replace("H", ""));
                string SD = (start == null) ? "" : Convert.ToDateTime(start).ToString("yyyy-MM-dd HH:mm:ss");
                string ED = (end == null) ? "" : Convert.ToDateTime(end).ToString("yyyy-MM-dd HH:mm:ss");
                string CarNo = (string.IsNullOrEmpty(objCar)) ? "" : objCar;
                EventHandleRepository _respository = new EventHandleRepository(connetStr);

                switch (sMode)
                {
                    case 0:
                        if (tmpOrder > 0)
                        {
                            ViewData["OrderNum"] = tmpOrder;

                            lstEv = _respository.GetMapDataByTimeLine(tmpOrder);
                        }
                        break;
                    case 1:
                        ViewData["CarNo"] = CarNo;
                        ViewData["SD"] = SD;
                        ViewData["ED"] = ED;
                        lstEv = _respository.GetMapDataByTimeLine(objCar, SD, ED);
                        break;
                }
            }
            return View(lstEv);
        }





        public ActionResult ExplodeMapQuery(string ExplodeOrderNum)
        {
            List<BE_MapList> lstRawDataOfMachi = new List<BE_MapList>();
            EventHandleRepository _repository = new EventHandleRepository(connetStr);

            Int64 tmpOrder = (string.IsNullOrEmpty(ExplodeOrderNum)) ? 0 : Convert.ToInt64(ExplodeOrderNum.Replace("H", ""));

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");

            string[] headerField = { "車號", "GPS時間", "經度", "緯度" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                sheet.AutoSizeColumn(j);
            }
            lstRawDataOfMachi = _repository.GetMapList(tmpOrder);
            int len = lstRawDataOfMachi.Count;
            for (int k = 0; k < len; k++)
            {
                IRow content = sheet.CreateRow(k + 1);
                content.CreateCell(0).SetCellValue(lstRawDataOfMachi[k].CarNo);
                content.CreateCell(1).SetCellValue(lstRawDataOfMachi[k].GPSTime);
                content.CreateCell(2).SetCellValue(lstRawDataOfMachi[k].Latitude);
                content.CreateCell(3).SetCellValue(lstRawDataOfMachi[k].Longitude);
            }
            for (int l = 0; l < headerFieldLen; l++)
            {
                sheet.AutoSizeColumn(l);
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            // workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "地圖軌跡資料" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");


        }
    }
}