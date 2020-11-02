using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherService.Enum
{
   public class ObjType
    {
        public enum SPType
        {
            /// <summary>
            /// 寫入呼叫webapi的log資料
            /// </summary>
             InsWebAPILog,
             /// <summary>
             /// 寫入呼叫FET Cat資料
             /// </summary>
             InsSendCMD,
             /// <summary>
             /// 寫入呼叫FET Cat回傳結果
             /// </summary>
             InsReceiveCMD,
             /// <summary>
             /// 更新興聯讀卡機狀態
             /// </summary>
             UpdNFCStatus,
             /// <summary>
             /// 寫入讀卡資料
             /// </summary>
             InsReadCard,
             /// <summary>
             /// 寫入遠傳定時回報及GetInfo
             /// </summary>
             HandleCarStatus,
             /// <summary>
             /// 寫入遠傳定時回報及GetInfo(機車)
             /// </summary>
             HandleCarStatusByMotor,
            BE_HandleCarMachineData,
            /// <summary>
            /// 取得DeviceName
            /// </summary>
            GetIDUCmdDeviceName
        }
        /// <summary>
        /// 取出SPName
        /// </summary>
        /// <param name="type">對應ObjType.SPType</param>
        /// <returns></returns>
        public string GetSPName(ObjType.SPType type)
        {
            string SPName = "";
            switch (type)
            {
                case SPType.InsWebAPILog:  //寫入API呼叫LOG進資料庫
                    SPName = "usp_InsWebAPILog";
                    break;
                case SPType.InsSendCMD:
                    SPName = "usp_InsSendCMD";
                    break;
                case SPType.InsReceiveCMD:
                    SPName = "usp_InsReceiveCMD";
                    break;
                case SPType.UpdNFCStatus:
                    SPName = "usp_UPDNFCStatus";
                    break;
                case SPType.InsReadCard:
                    SPName = "usp_InsReadCardData";
                    break;
                case SPType.HandleCarStatus:
                    SPName = "usp_HandleCarStatusByCar";
                    break;
                case SPType.HandleCarStatusByMotor:
                    SPName = "usp_HandleCarStatusByMotor";
                    break;
                case SPType.GetIDUCmdDeviceName:
                    SPName = "usp_GetIDUCmdDeviceName";
                    break;



            }
            return SPName;
        }

    }
}
