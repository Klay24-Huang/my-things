using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    /// <summary>
    /// 出車
    /// </summary>
   public  class WebAPIInput_NPR125Save
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
    }
}
