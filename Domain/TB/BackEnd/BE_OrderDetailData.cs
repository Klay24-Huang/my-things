using System;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 訂單明細資料
    /// </summary>
    public class BE_OrderDetailData
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 預約日期
        /// </summary>
        public DateTime BookingDate { set; get; }
        /// <summary>
        /// 取還車狀態
        /// <para>0 = 尚未取車</para><para>1 = 已經上傳出車照片</para>
        /// <para>2 = 已經簽名出車單</para>
        /// <para>3 = 已經信用卡認證</para>
        /// <para>4 = 已經取車(記錄起始時間)</para>
        /// <para>11 = 已經紀錄還車時間</para>
        /// <para>12 = 已經上傳還車角度照片</para>
        /// <para>13 = 已經上傳還車車損照片</para>
        /// <para>14 = 已經簽名還車單</para>
        /// <para>15 = 已經信用卡付款</para>
        /// <para>16 = 已經檢查車輛完成並已經解除卡號</para>
        /// </summary>
        public Int16 CMS { set; get; }
        /// <summary>
        /// 預約單狀態
        /// <para>0 = 會員預約</para>
        /// <para>1 = 管理員清潔預約</para>
        /// <para>2 = 管理員保修預約</para>
        /// <para>3 = 延長用車狀態</para>
        /// <para>4 = 強迫延長用車狀態</para>
        /// <para>5 = 合約完成(已完成解除卡號動作)</para>
        /// </summary>
        public Int16 BS { set; get; }
        /// <summary>
        /// 訂單修改狀態：
        /// <para>0 = 無(訂單未刪除，正常預約狀態)</para>
        /// <para>1 = 修改指派車輛(此訂單因其他預約單強迫延長而更改過訂單 or 後台重新配車過 or 取車時無車，重新配車)</para>
        /// <para>2 = 此訂單被人工介入過(後台協助取還車 or 後台修改訂單資料)</para>
        /// <para>3 = 訂單已取消(會員主動取消 or 逾時15分鐘未取車)</para>
        /// <para>4 = 訂單已取消(因車輛仍在使用中又無法預約到其他車輛而取消)</para>
        /// </summary>
        public Int16 CS { set; get; }
        /// <summary>
        /// 是否有修改合約
        /// <para>0:否</para>
        /// <para>1:有</para>
        /// </summary>
        public Int16 MS { set; get; }
        /// <summary>
        /// 預計取車
        /// </summary>
        public DateTime SD { set; get; }
        /// <summary>
        /// 預計還車
        /// </summary>
        public DateTime ED { set; get; }
        /// <summary>
        /// 延長用車
        /// </summary>
        public DateTime OStopTime { set; get; }
        /// <summary>
        /// 取車據點
        /// </summary>
        public string LStation { set; get; }
        /// <summary>
        /// 還車據點
        /// </summary>
        public string RStation { set; get; }
        /// <summary>
        /// 車型
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string PRONAME { set; get; }
        /// <summary>
        /// 發票寄送方式：
        /// <para>1:捐贈</para>
        /// <para>2:email</para>
        /// <para>3:二聯</para>
        /// <para>4:三聯</para>
        /// <para>5:手機條碼</para>
        /// <para>6:自然人憑證</para>
        /// </summary>
        public Int16 InvoicKind { set; get; }
        /// <summary>
        /// 統編
        /// </summary>
        public string BCode { set; get; }
        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string TEL { set; get; }
        /// <summary>
        /// 預估租金
        /// </summary>
        public int PurePrice { set; get; }
        /// <summary>
        /// 手機/自然人載具
        /// </summary>
        public string CARRIERID { set; get; }
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { set; get; }
        /// <summary>
        /// 公司抬頭
        /// </summary>
        public string title { set; get; }
        /// <summary>
        /// 實際取車
        /// </summary>
        public DateTime FS { set; get; }
        /// <summary>
        /// 實際還車
        /// </summary>
        public DateTime FE { set; get; }
        /// <summary>
        /// 開始里程
        /// </summary>
        public Single StartMile { set; get; }
        /// <summary>
        /// 結束里程
        /// </summary>
        public Single StopMile { set; get; }
        /// <summary>
        /// 純租金
        /// </summary>
        public int CarRent { set; get; }
        /// <summary>
        /// 租金總計
        /// </summary>
        public int FinalPrice { set; get; }
        /// <summary>
        /// 逾時罰金
        /// </summary>
        public int FinePrice { set; get; }
        /// <summary>
        /// 里程費
        /// </summary>
        public int Mileage { set; get; }
        /// <summary>
        /// eTag費用
        /// </summary>
        public int eTag { set; get; }
        /// <summary>
        /// 轉乘優惠
        /// </summary>
        public int TransDiscount { set; get; }
        /// <summary>
        /// 使用的汽車點數
        /// </summary>
        public int CarPoint { set; get; }
        /// <summary>
        /// 使用的機車點數
        /// </summary>
        public int MotorPoint { set; get; }
        /// <summary>
        /// 逾時時間
        /// </summary>
        public DateTime FineTime { set; get; }
        /// <summary>
        /// 安心服務
        /// </summary>
        public int Insurance { set; get; }
        /// <summary>
        /// 安心服務費
        /// </summary>
        public int Insurance_price { set; get; } //2021唐改，原為InsurancePurePrice，抓預估安心服務價格，現改抓實際的
        /// <summary>
        /// 取車回饋
        /// </summary>
        public string LFeedBack { set; get; }
        /// <summary>
        /// 還車回饋
        /// </summary>
        public string RFeedBack { set; get; }
        /// <summary>
        /// 機車專用-取車時左電池電量
        /// </summary>
        public decimal P_LBA { set; get; }
        /// <summary>
        /// 機車專用-取車時右電池電量
        /// </summary>
        public decimal P_RBA { set; get; }
        /// <summary>
        /// 機車專用-取車時平均電量(3TBA)
        /// </summary>
        public decimal P_TBA { set; get; }
        /// <summary>
        /// 機車專用-取車時核心電池電量
        /// </summary>
        public decimal P_MBA { set; get; }
        /// <summary>
        /// 機車專用-取車時經度
        /// </summary>
        public decimal P_lon { set; get; }
        /// <summary>
        /// 機車專用-取車時緯度
        /// </summary>
        public decimal P_lat { set; get; }
        /// <summary>
        /// 機車專用-還車時左電池電量
        /// </summary>
        public decimal R_LBA { set; get; }
        /// <summary>
        /// 機車專用-還車時右電池電量
        /// </summary>
        public decimal R_RBA { set; get; }
        /// <summary>
        /// 機車專用-還車時平均電量(3TBA)
        /// </summary>
        public decimal R_TBA { set; get; }
        /// <summary>
        /// 機車專用-還車時核心電池電量
        /// </summary>
        public decimal R_MBA { set; get; }
        /// <summary>
        ///  機車專用-還車時經度
        /// </summary>
        public decimal R_lon { set; get; }
        /// <summary>
        ///  機車專用-還車時緯度
        /// </summary>
        public decimal R_lat { set; get; }
        /// <summary>
        /// 換電獎勵
        /// </summary>
        public int Reward { set; get; }
        /// <summary>
        /// 營損-車輛調度費
        /// </summary>
        public int CarDispatch { set; get; }
        /// <summary>
        /// 營損-車輛調度費備註
        /// </summary>
        public string DispatchRemark { set; get; }
        /// <summary>
        /// 營損-清潔費
        /// </summary>
        public int CleanFee { set; get; }
        /// <summary>
        /// 營損-清潔費備註
        /// </summary>
        public string CleanFeeRemark { set; get; }
        /// <summary>
        /// 營損-物品損壞/遣失費
        /// </summary>
        public int DestroyFee { set; get; }
        /// <summary>
        /// 營損-物品損壞/遣失費備註
        /// </summary>
        public string DestroyFeeRemark { set; get; }
        /// <summary>
        /// 營損-非配合停車場停車費
        /// </summary>
        public int ParkingFee { set; get; }
        /// <summary>
        /// 營損-非配合停車場停車費備註
        /// </summary>
        public string ParkingFeeRemark { set; get; }
        /// <summary>
        /// 營損-拖吊費
        /// </summary>
        public int DraggingFee { set; get; }
        /// <summary>
        /// 營損-拖吊費備註
        /// </summary>
        public string DraggingFeeRemark { set; get; }
        /// <summary>
        /// 營損-其他費用
        /// </summary>
        public int OtherFee { set; get; }
        /// <summary>
        /// 營損-其他費用備註
        /// </summary>
        public string OtherFeeRemark { set; get; }
        /// <summary>
        /// 車麻吉費用
        /// </summary>
        public int MachiFee { set; get; }
        /// <summary>
        /// 引擎(車身)號碼
        /// </summary>
        public string EngineNO { set; get; }
        /// <summary>
        /// 顏色
        /// </summary>
        public string CarColor { set; get; }
        /// <summary>
        /// 廠牌
        /// </summary>
        public string CarBrend { set; get; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime MEMBIRTH { set; get; }
        /// <summary>
        /// 城市
        /// </summary>
        public string CityName { set; get; }
        /// <summary>
        /// 區域
        /// </summary>
        public string AreaName { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string MEMADDR { set; get; }
        /// <summary>
        /// 停車格
        /// </summary>
        public string parkingSpace { set; get; }
        /// <summary>
        /// 合約狀態
        /// </summary>
        public int car_mgt_status { set; get; }
        /// <summary>
        /// 付款時間
        /// </summary>
        public string PayTime { set; get; }
        /// <summary>
        /// StoreTradeNo
        /// </summary>
        public string RetCode { set; get; }
        /// <summary>
        /// StoreTradeNo
        /// </summary>
        public string RetMsg { set; get; }
        /// <summary>
        /// StoreTradeNo
        /// </summary>
        public string MerchantTradeNo { set; get; }
        /// <summary>
        /// 台新交易序號
        /// </summary>
        public string TaishinTradeNo { set; get; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string CreditType { set; get; }
        /// <summary>
        /// 付款金額
        /// </summary>
        public int PayAmount { set; get; }
        /// <summary>
        /// 安心服務費率
        /// </summary>
        public int InsurancePerHours { set; get; }
        /// <summary>
        /// AES編碼 20210316 ADD BY ADAM 
        /// </summary>
        public string AesEncode { set; get; }
    }

    /// <summary>
    /// 訂單明細資料
    /// </summary>
    public class BE_OrderPaymentData
    {
        /// <summary>
        /// 付款時間
        /// </summary>
        public string PayTime { set; get; }
        /// <summary>
        /// StoreTradeNo
        /// </summary>
        public string RetCode { set; get; }
        /// <summary>
        /// StoreTradeNo
        /// </summary>
        public string RetMsg { set; get; }
        /// <summary>
        /// StoreTradeNo
        /// </summary>
        public string MerchantTradeNo { set; get; }
        /// <summary>
        /// 台新交易序號
        /// </summary>
        public string TaishinTradeNo { set; get; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public string CreditType { set; get; }
        /// <summary>
        /// 付款金額
        /// </summary>
        public int PayAmount { set; get; }
    }
}