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
            CheckTokenOnlyToken,
            CheckTokenReturnID,
            /// <summary>
            /// 寫入刷卡資料
            /// </summary>
            InsTrade,
            /// <summary>
            /// 更新刷卡結果
            /// </summary>
            UpdTrade,
            InsTradeRefund,
            UpdTradeRefund,
            /// <summary>
            /// 新增解除綁定Log
            /// </summary>
            InsUnBindLog,
            /// <summary>
            /// 寫入呼叫興聯車機資料
            /// </summary>
            InsCensCMDLog,
            /// <summary>
            /// 遠傳車機韌體資訊
            /// </summary>
            /// <summary>
            /// 台新錢包直接儲值+開戶 LOG寫入
            /// </summary>
            InsStoreValueCreateAccountLog,
            /// <summary>
            /// 台新錢包扣款 LOG寫入
            /// </summary>
            InsPayTransactionLog,
            /// <summary>
            /// 台新錢包轉贈 LOG寫入
            /// </summary>
            InsTransferStoreValueLog
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
                    SPName = "usp_HandleCarStatusByCar_20210621";
                    break;
                case SPType.HandleCarStatusByMotor:
                    SPName = "usp_HandleCarStatusByMotor_20210621";
                    break;
                case SPType.InsTrade:
                    SPName = "usp_InsTrade";
                    break;
                case SPType.UpdTrade:
                    SPName = "usp_UpdTrade";
                    break;
                case SPType.InsTradeRefund:
                    SPName = "usp_InsTradeRefund";
                    break;
                case SPType.UpdTradeRefund:
                    SPName = "usp_UpdTradeRefund";
                    break;

                case SPType.InsUnBindLog:
                    SPName = "usp_InsUnBindLog";
                    break;
                case SPType.InsCensCMDLog:
                    SPName = "usp_InsCensCMDLog";
                    break;
                case SPType.UpdCarMachineVerInfo:
                    SPName = "usp_UpdCarMachineVerInfo";
                    break;
                case SPType.InsStoreValueCreateAccountLog:
                    SPName = "usp_InsStoreValueCreateAccountLog";
                    break;
                case SPType.InsPayTransactionLog:
                    SPName = "usp_InsPayTransactionLog";
                    break;
                case SPType.InsTransferStoreValueLog:
                    SPName = "usp_InsTransferStoreValueCreateAccountLog";
                    break;
            }
            return SPName;
        }

    }
}
