using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class OrderDetailData
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }

        /// <summary>
        /// 營運商
        /// </summary>
        public string Operator { set; get; }
        /// <summary>
        /// 車輛圖片
        /// </summary>
        public string CarTypePic { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 座椅數
        /// </summary>
        public int Seat { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string CarBrend { set; get; }
        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 據點名稱
        /// </summary>
        public string StationName { set; get; }
        /// <summary>
        /// 評分
        /// </summary>
        public float OperatorScore { set; get; }
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjName { set; get; }
        /// <summary>
        /// 車輛租金
        /// </summary>
        public int pure_price { set; get; }

        /// <summary>
        /// 月租抵扣
        /// </summary>
        public float MonthlyHours { set; get; }
        /// <summary>
        /// 時數抵扣
        /// </summary>
        public float GiftPoint { set; get; }

        /// <summary>
        /// 里程費
        /// </summary>
        public int mileage_price { set; get; }
        /// <summary>
        /// 安心服務費
        /// </summary>
        public int Insurance_price { set; get; }
        /// <summary>
        /// etag費用
        /// </summary>
        public int Etag { set; get; }
        /// <summary>
        /// 逾時費
        /// </summary>
        public int fine_price { set; get; }
        /// <summary>
        /// 代收停車費
        /// </summary>
        public int parkingFee { set; get; }
        /// <summary>
        /// 轉乘優惠折抵
        /// </summary>
        public int TransDiscount { set; get; }
        /// <summary>
        /// 總金額
        /// </summary>
        public int final_price { set; get; }
        /// <summary>
        /// 發票類型
        /// </summary>
        public int InvoiceType { set; get; }
        /// <summary>
        /// 捐贈碼
        /// </summary>
        public string NPOBAN { set; get; }
        /// <summary>
        /// 捐贈協會名稱
        /// </summary>
        public string NPOBAN_Name { set; get; }
        /// <summary>
        /// 發票號碼
        /// </summary>
        public string invoiceCode { set; get; }
        /// <summary>
        /// 發票日期
        /// </summary>
        public string invoice_date { set; get; }
        /// <summary>
        /// 發票金額
        /// </summary>
        public int invoice_price { set; get; }



        /// <summary>
        /// 開始時間
        /// </summary>
        public string StartTime { set; get; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public string EndTime { set; get; }
    }
}
