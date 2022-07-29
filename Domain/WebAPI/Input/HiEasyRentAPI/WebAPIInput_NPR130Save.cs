using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    public class WebAPIInput_NPR130Save
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
        /// 處理區分(必填)
        /// <para>A:新增</para>
        /// <para>F:作廢</para>
        /// </summary>
        public string PROCD { set; get; }
        /// <summary>
        /// 短租預約編號(必填)
        /// </summary>
        public string ORDNO { set; get; }
        /// <summary>
        /// iRent訂單編號(必填)
        /// </summary>
        public string IRENTORDNO { set; get; }
        /// <summary>
        /// 客戶編號(必填)
        /// </summary>
        public string CUSTID { set; get; }
        /// <summary>
        /// 客戶名稱(必填)
        /// </summary>
        public string CUSTNM { set; get; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string BIRTH { set; get; }
        /// <summary>
        /// 客戶類別(必填)
        /// </summary>
        public string CUSTTYPE { set; get; }
        /// <summary>
        /// 簽約客戶(必填)
        /// </summary>
        public string ODCUSTID { set; get; }
        /// <summary>
        /// 車型代碼(必填)，ex：000278
        /// </summary>
        public string CARTYPE { set; get; }
        /// <summary>
        /// 車號(必填)
        /// </summary>
        public string CARNO { set; get; }
        /// <summary>
        /// 車輛編號(必填)
        /// </summary>
        public string TSEQNO { set; get; }
        /// <summary>
        /// 預計取車日期(必填)
        /// <para>格式：yyyyMMdd</para>
        /// </summary>
        public string GIVEDATE { set; get; }
        /// <summary>
        /// 預計取車時間(必填)
        ///  <para>格式：HHmm</para>
        /// </summary>
        public string GIVETIME { set; get; }
        /// <summary>
        /// 預定使用天數(必填)
        /// </summary>
        public string RENTDAYS { set; get; }
        /// <summary>
        /// 取車里程
        /// </summary>
        public string GIVEKM { set; get; }
        /// <summary>
        /// 出車據點(必填)
        /// </summary>
        public string OUTBRNHCD { set; get; }
        /// <summary>
        /// 預計還車日期(必填)
        /// <para>格式：yyyyMMdd</para>
        /// </summary>
        public string RNTDATE { set; get; }
        /// <summary>
        /// 預計還車時間(必填)
        ///  <para>格式：HHmm</para>
        /// </summary>
        public string RNTTIME { set; get; }
        /// <summary>
        /// 還車里程
        /// </summary>
        public string RNTKM { set; get; }
        /// <summary>
        /// 還車據點(必填)
        /// </summary>
        public string INBRNHCD { set; get; }
        /// <summary>
        /// 每日租金(必填)
        /// </summary>
        public string RPRICE { set; get; }
        /// <summary>
        /// 保費(日)(必填)
        /// </summary>
        public string RINSU { set; get; }
        /// <summary>
        /// 折扣率(必填)
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
        /// 還車費用(租金小計+逾時費用)
        /// </summary>
        public string RNTAMT { set; get; }
        /// <summary>
        /// 租金小計
        /// </summary>
        public string RENTAMT { set; get; }
        /// <summary>
        /// 油料補貼(里程費)
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
        /// 發票方式
        /// <para>1:捐贈                     </para>
        /// <para>2:EMAIL                    </para>
        /// <para>3:郵寄二聯                 </para>
        /// <para>4:郵寄三聯                 </para>
        /// <para>5:手機載具(會員設定)       </para>
        /// <para>6:自然人憑證條碼(會員設定) </para>
        /// <para>7:其他社福團體(會員設定)   </para>
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
        /// 使用汽車點數
        /// </summary>
        public string GIFT { set; get; }
        /// <summary>
        /// 使用機車點數
        /// </summary>
        public string GIFT_MOTO { set; get; }
        /// <summary>
        /// 信用卡卡號
        /// </summary>
        public string CARDNO { set;get;}
        /// <summary>
        /// 授權碼
        /// </summary>
        public string AUTHCODE{ set; get; }

        /// <summary>
        ///實際付款金額
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
        /// 安心服務小計
        /// </summary>
        public string NOCAMT { set; get; }
        /// <summary>
        /// 停車費 20210818補上
        /// </summary>
        public string PARKINGAMT2 { set; get; }
        /// <summary>
        /// 企客帳號
        /// </summary>
        public string TAXID { set; get; }
        /// <summary>
        /// 是否ETAG自付額
        /// </summary>
        public string EC_ETAG { set; get; }
        /// <summary>
        /// 是否安心服務自負額
        /// </summary>
        public string EC_INSURANCE { set; get; }
        /// <summary>
        /// 是否停車費自付額
        /// </summary>
        public string EC_PARKING { set; get; }
        /// <summary>
        /// ETAG
        /// </summary>
        public string ETAG { set; get; }
        /// <summary>
        /// 非一般時數折抵(優惠券,久停)
        /// </summary>
        public string DISCOUNTMINS { set; get; }
        /// <summary>
        /// 訂閱制折抵
        /// </summary>
        public string MONTHRENTMINS { set; get; }

        public PaymentDetail[] tbPaymentDetail { set; get; }
        /// <summary>
        /// 副承租人清單
        /// </summary>
        public SavePassenger[] tbSavePassenger { set; get; }
    }
    public class PaymentDetail
    {
        /// <summary>
        /// 支付方式
        /// <para>1:信用卡            </para>
        /// <para>2:電子錢包-信用卡   </para>
        /// <para>3:電子錢包-虛擬帳戶 </para>
        /// <para>4:電子錢包-超商     </para>
        /// </summary>
        public string PAYMENTTYPE{set;get;}
        /// <summary>
        /// 支付類型
        /// <para>1:租金</para>
        /// <para>2:ETAG</para>
        /// </summary>
        public string PAYTYPE    {set;get;}   
            /// <summary>
            /// 支付金額
            /// </summary>
        public string PAYAMT     {set;get;}  
        /// <summary>
        /// 支付訂單編號
        /// </summary>
         public string PORDNO     {set;get;}  
        /// <summary>
        /// 支付說明(ex:租金)
        /// </summary>
        public string PAYMEMO { set; get; } 
        /// <summary>
        /// 0: 台新 1:中信
        /// </summary>
        public int OPERATOR { set; get; }
    }
    //20211221 ADD BY ADAM REASON.增加副承租人清單
    public class SavePassenger
    {
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string MEMIDNO { get; set; }
        /// <summary>
        /// 副承租人姓名
        /// </summary>
        public string MEMCNAME { get; set; }
        /// <summary>
        /// 副承租人安心服務每小時金額
        /// </summary>
        public int InsurancePerHours { get; set; }
        /// <summary>
        /// 承租人安心服務費用
        /// </summary>
        public int InsuranceAMT { get; set; }

    }
}
