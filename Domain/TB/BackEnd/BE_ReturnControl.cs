using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_ReturnControl
    {
        /// <summary>
        /// 
        /// </summary>
        public string PROCD { get; set; }

        /// <summary>
        /// 短租預約編號
        /// </summary>
        public string ORDNO { get; set; }

        /// <summary>
        /// irent訂單編號
        /// </summary>
        public string IRENTORDNO { get; set; }

        /// <summary>
        /// 客戶編號
        /// </summary>
        public string CUSTID { get; set; }

        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string CUSTNM { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string BIRTH { get; set; }

        /// <summary>
        /// 客戶類型
        /// </summary>
        public string CUSTTYPE { get; set; }

        /// <summary>
        /// 簽約客戶
        /// </summary>
        public string ODCUSTID { get; set; }

        /// <summary>
        /// 車型
        /// </summary>
        public string CARTYPE { get; set; }

        /// <summary>
        /// 車號
        /// </summary>
        public string CARNO { get; set; }

        /// <summary>
        /// 車輛流水號
        /// </summary>
        public string TSEQNO { get; set; }

        /// <summary>
        /// 出車日期
        /// </summary>
        public string GIVEDATE { get; set; }

        /// <summary>
        /// 出車時間
        /// </summary>
        public string GIVETIME { get; set; }

        /// <summary>
        /// 租用天數
        /// </summary>
        public string RENTDAYS { get; set; }

        /// <summary>
        /// 出車里程
        /// </summary>
        public string GIVEKM { get; set; }

        /// <summary>
        /// 出車據點
        /// </summary>
        public string OUTBRNHCD { get; set; }

        /// <summary>
        /// 還車日期
        /// </summary>
        public string RNTDATE { get; set; }

        /// <summary>
        /// 還車時間
        /// </summary>
        public string RNTTIME { get; set; }

        /// <summary>
        /// 還車里程
        /// </summary>
        public string RNTKM { get; set; }

        /// <summary>
        /// 日租金
        /// </summary>
        public string RPRICE { get; set; }

        /// <summary>
        /// 日保費
        /// </summary>
        public string RINSU { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public string DISRATE { get; set; }

        /// <summary>
        /// 逾時時數
        /// </summary>
        public string OVERHOURS { get; set; }

        /// <summary>
        /// 逾時費用
        /// </summary>
        public string OVERAMT2 { get; set; }

        /// <summary>
        /// 還車費用
        /// </summary>
        public string RNTAMT { get; set; }

        /// <summary>
        /// 租金小計
        /// </summary>
        public string RENTAMT { get; set; }

        /// <summary>
        /// 油料補貼
        /// </summary>
        public string LOSSAMT2 { get; set; }

        /// <summary>
        /// 專案代碼
        /// </summary>
        public string PROJID { get; set; }

        /// <summary>
        /// 網刷訂單編號
        /// </summary>
        public string REMARK { get; set; }

        /// <summary>
        /// 發票寄送方式1:捐贈 2:EMAIL 3:郵寄二聯 4:郵寄三聯 5:手機載具(會員設定)
        /// </summary>
        public string INVKIND { get; set; }

        /// <summary>
        /// 發票統編
        /// </summary>
        public string UNIMNO { get; set; }

        /// <summary>
        /// 發票抬頭
        /// </summary>
        public string INVTITLE { get; set; }

        /// <summary>
        /// 發票地址
        /// </summary>
        public string INVADDR { get; set; }

        /// <summary>
        /// 折抵時數-汽車
        /// </summary>
        public string GIFT { get; set; }

        /// <summary>
        /// 折抵時數-機車
        /// </summary>
        public string GIFT_MOTO { get; set; }

        /// <summary>
        /// 信用卡號
        /// </summary>
        public string CARDNO { get; set; }

        /// <summary>
        /// 授權碼
        /// </summary>
        public string AUTHCODE { get; set; }

        /// <summary>
        /// 實際付款金額
        /// </summary>
        public int PAYAMT { get; set; }

        /// <summary>
        /// 載具條碼
        /// </summary>
        public string CARRIERID { get; set; }

        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { get; set; }

        /// <summary>
        /// 代收停車費
        /// </summary>
        public int PARKINGAMT2 { get; set; }

        /// <summary>
        /// 安心服務費用
        /// </summary>
        public string NOCAMT { get; set; }

        /// <summary>
        /// etag費用
        /// </summary>
        public int eTag { get; set; }

        /// <summary>
        /// 關帳金額
        /// </summary>
        public int CloseAmout { get; set; }
        /// <summary>
        /// 商代代碼
        /// </summary>
        public string MerchantID { get; set; }
        /// <summary>
        /// 刷卡的訂單編號(專給中信使用)
        /// </summary>
        public string MerchantTradeNo { get; set; }
        /// <summary>
        /// 錢包支付金額 20220307 ADD BY ADAM 
        /// </summary>
        public int WalletAmount { get; set; }
        /// <summary>
        /// 支付金額
        /// </summary>
        public int PaymentAmount { get; set; }
        /// <summary>
        /// 支付訂單編號
        /// </summary>
        public string PaymentNORDNO { get; set; }
        /// <summary>
        /// 支付類型 1:信用卡 2:錢包
        /// </summary>
        public string PaymentType { get; set; }
    }
}