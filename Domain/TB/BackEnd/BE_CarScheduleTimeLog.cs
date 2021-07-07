using System;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 後台車輛行程管理
    /// </summary>
    public class BE_CarScheduleTimeLog
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNum { set; get; }

        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }

        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string UName { set; get; }

        /// <summary>
        /// 手機
        /// </summary>
        public string Mobile { set; get; }

        /// <summary>
        /// 取還車狀態：0 = 尚未取車、1 = 已經上傳出車照片、2 = 已經簽名出車單、3 = 已經信用卡認證、4 = 已經取車(記錄起始時間)、11 = 已經紀錄還車時間、12 = 已經上傳還車角度照片、13 = 已經上傳還車車損照片、14 = 已經簽名還車單、15 = 已經信用卡付款、16 = 已經檢查車輛完成並已經解除卡號
        /// </summary>
        public int car_mgt_status { set; get; }

        /// <summary>
        /// 預約單狀態0 = 會員預約、1 = 管理員清潔預約、2 = 管理員保修預約、3 = 延長用車狀態、4 = 強迫延長用車狀態、5 = 合約完成(已完成解除卡號動作)
        /// </summary>
        public int booking_status { set; get; }

        /// <summary>
        /// 訂單修改狀態：0 = 無(訂單未刪除，正常預約狀態)、1 = 修改指派車輛(此訂單因其他預約單強迫延長而更改過訂單 or 後台重新配車過 or 取車時無車，重新配車)、2 = 此訂單被人工介入過(後台協助取還車 or 後台修改訂單資料)、3 = 訂單已取消(會員主動取消 or 逾時15分鐘未取車)、4 = 訂單已取消(因車輛仍在使用中又無法預約到其他車輛而取消)、5 = 整備使用(逾時未取或還車)
        /// </summary>
        public int cancel_status { set; get; }

        /// <summary>
        /// 預約取車時間
        /// </summary>
        public DateTime SD { set; get; }

        /// <summary>
        /// 預約還車時間
        /// </summary>
        public DateTime ED { set; get; }

        /// <summary>
        /// 實際出車時間
        /// </summary>
        public DateTime FS { set; get; }

        /// <summary>
        /// 實際還車時間
        /// </summary>
        public DateTime FE { set; get; }
    }
}