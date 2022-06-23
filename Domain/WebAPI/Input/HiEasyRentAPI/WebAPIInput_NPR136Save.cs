using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_NPR136Save
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public string user_id { set; get; }
        /// <summary>
        /// 認證簽章
        /// </summary>
        public string sig { set; get; }

        /// <summary>
        /// 處理區分
        /// </summary>
        public string PROCD { set; get; }
        /// <summary>
        /// 預約編號
        /// </summary>
        public string ORDNO { set; get; }
        /// <summary>
        /// iRent訂單編號
        /// </summary>
        public string IRENTORDNO { set; get; }
        /// <summary>
        /// 身份證號/客戶編號
        /// </summary>
        public string CUSTID { set; get; }
        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string CUSTNM { set; get; }
        /// <summary>
        /// 生日
        /// </summary>
        public string BIRTH { set; get; }
        /// <summary>
        /// 客戶類型
        /// </summary>
        public string CUSTTYPE { set; get; }
        /// <summary>
        /// 簽約客戶
        /// </summary>
        public string ODCUSTID { set; get; }
        /// <summary>
        /// 車型
        /// </summary>
        public string CARTYPE { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CARNO { set; get; }
        /// <summary>
        /// 車輛編號
        /// </summary>
        public string TSEQNO { set; get; }
        /// <summary>
        /// 出車日期
        /// </summary>
        public string GIVEDATE { set; get; }
        /// <summary>
        /// 出車時間
        /// </summary>
        public string GIVETIME { set; get; }
        /// <summary>
        /// 租用天數
        /// </summary>
        public string RENTDAYS { set; get; }
        /// <summary>
        /// 出車里程
        /// </summary>
        public string GIVEKM { set; get; }
        /// <summary>
        /// 出車據點
        /// </summary>
        public string OUTBRNHCD { set; get; }
        /// <summary>
        /// 還車日期
        /// </summary>
        public string RNTDATE { set; get; }
        /// <summary>
        /// 還車時間
        /// </summary>
        public string RNTTIME { set; get; }
        /// <summary>
        /// 還車里程
        /// </summary>
        public string RNTKM { set; get; }
        /// <summary>
        /// 還車據點
        /// </summary>
        public string INBRNHCD { set; get; }
        /// <summary>
        /// 日租金
        /// </summary>
        public string RPRICE { set; get; }
        /// <summary>
        /// 日保費
        /// </summary>
        public string RINSU { set; get; }
        /// <summary>
        /// 折扣
        /// </summary>
        public string DISRATE { set; get; }
        /// <summary>
        /// 逾時時數
        /// </summary>
        public string OVERHOURS { set; get; }
        /// <summary>
        /// 逾時費用
        /// </summary>
        public string OVERAMT2 { set; get; }
        /// <summary>
        /// 還車費用
        /// </summary>
        public string RNTAMT { set; get; }
        /// <summary>
        /// 租金小計
        /// </summary>
        public string RENTAMT { set; get; }
        /// <summary>
        /// 油料補貼
        /// </summary>
        public string LOSSAMT2 { set; get; }
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string PROJID { set; get; }
        /// <summary>
        /// 網刷訂單編號
        /// </summary>
        public string REMARK { set; get; }
        /// <summary>
        /// 發票聯式
        /// <para>1:捐贈</para>
        /// <para>2:EMAIL</para>
        /// <para>3:二聯式</para>
        /// <para>4:三聯式</para>
        /// </summary>
        public string INVKIND { set; get; }
        /// <summary>
        /// 發票統編
        /// </summary>
        public string UNIMNO { set; get; }
        /// <summary>
        /// 發票抬頭
        /// </summary>
        public string INVTITLE { set; get; }
        /// <summary>
        /// 發票地址
        /// </summary>
        public string INVADDR { set; get; }
        /// <summary>
        /// 抵用時數
        /// </summary>
        public string GIFT { set; get; }
        /// <summary>
        /// 抵用時數(機車)
        /// </summary>
        public string GIFT_MOTO { set; get; }
        /// <summary>
        /// 信用卡號
        /// </summary>
        public string CARDNO { set; get; }
        /// <summary>
        /// 授權碼
        /// </summary>
        public string AUTHCODE { set; get; }
        /// <summary>
        /// 已付金額
        /// </summary>
        public string PAYAMT { set; get; }
        /// <summary>
        /// 載具條碼
        /// </summary>
        public string CARRIERID { set; get; }
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { set; get; }
        /// <summary>
        /// 營損－車輛調度
        /// </summary>
        public string CTRLAMT { set; get; }
        /// <summary>
        /// 營損－車輛調度備註
        /// </summary>
        public string CTRLMEMO { set; get; }
        /// <summary>
        /// 營損－清潔費
        /// </summary>
        public string CLEANAMT { set; get; }
        /// <summary>
        /// 營損－清潔費備註
        /// </summary>
        public string CLEANMEMO { set; get; }
        /// <summary>
        /// 營損－物品損壞/遺失
        /// </summary>
        public string EQUIPAMT { set; get; }
        /// <summary>
        /// 營損－物品損壞/遺失備註
        /// </summary>
        public string EQUIPMEMO { set; get; }
        /// <summary>
        /// 營損－停車費
        /// </summary>
        public string PARKINGAMT { set; get; }
        /// <summary>
        /// 營損－停車費備註
        /// </summary>
        public string PARKINGMEMO { set; get; }
        /// <summary>
        /// 營損－拖吊費
        /// </summary>
        public string TOWINGAMT { set; get; }
        /// <summary>
        /// 營損－拖吊費備註
        /// </summary>
        public string TOWINGMEMO { set; get; }
        /// <summary>
        /// 營損－其他費用
        /// </summary>
        public string OTHERAMT { set; get; }
        /// <summary>
        /// 營損－其他費用備註
        /// </summary>
        public string OTHERMEMO { set; get; }
        /// <summary>
        /// 代收停車費
        /// </summary>
        public string PARKINGAMT2 { set; get; }
        /// <summary>
        /// 代收停車費備註
        /// </summary>
        public string PARKINGMEMO2 { set; get; }
        /// <summary>
        /// 安心服務
        /// </summary>
        public string NOCAMT { set; get; }
        public List<PaymentDetail> tbPaymentDetail { set; get; }
    }

    public class WebAPIInput_NPR136V2Save
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public string user_id { set; get; }
        /// <summary>
        /// 認證簽章
        /// </summary>
        public string sig { set; get; }
        /// <summary>
        /// 使用者
        /// </summary>
        public string USERID { get; set; }
        /// <summary>
        /// 合約編號
        /// </summary>
        public string CNTRNO { get; set; }
        /// <summary>
        /// IRENT訂單號
        /// </summary>
        public string IRENTORDNO { get; set; }
        /// <summary>
        /// 調度費
        /// </summary>
        public int CarDispatch { get; set; }
        /// <summary>
        /// 調度費備註
        /// </summary>
        public string DispatchRemark { get; set; }
        /// <summary>
        /// 停車費
        /// </summary>
        public int ParkingFee { get; set; }
        /// <summary>
        /// 停車費備註
        /// </summary>
        public string ParkingFeeRemark { get; set; }
    }

}
