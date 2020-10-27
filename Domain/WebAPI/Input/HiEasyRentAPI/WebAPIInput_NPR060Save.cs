using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.HiEasyRentAPI
{
    /// <summary>
    /// 060
    /// </summary>
   public  class WebAPIInput_NPR060Save
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
        /// <para>A:新增</para>
        /// <para>F:作廢</para>
        /// </summary>
        public string PROCD { set; get; }
        /// <summary>
        /// 訂車客戶ID(必填)
        /// </summary>
        public string ODCUSTID { set; get; }
        /// <summary>
        /// 訂車客戶名稱(必填)
        /// </summary>
        public string ODCUSTNM { set; get; }
        /// <summary>
        /// 連絡電話(必填)
        /// </summary>
        public string TEL1 { set; get; }
        /// <summary>
        /// 行動電話(必填)
        /// </summary>
        public string TEL2 { set; get; }
        /// <summary>
        /// 公司電話
        /// </summary>
        public string TEL3 { set; get; }
        /// <summary>
        /// 受訂日期(必填)（對應tb的bookingdate
        ///  <para>格式：yyyyMMdd</para>
        /// </summary>
        public string ODDATE { set; get; }
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
        /// 車型代碼(必填)，ex：000278
        /// </summary>
        public string CARTYPE { set; get; }
        /// <summary>
        /// 車號(必填)
        /// </summary>
        public string CARNO { set; get; }
        /// <summary>
        /// 出車據點(必填)
        /// </summary>
        public string OUTBRNH { set; get; }
        /// <summary>
        /// 預計還車據點(必填)
        /// </summary>
        public string INBRNH { set; get; }
        /// <summary>
        /// 租金(必填)
        /// </summary>
        public string ORDAMT { set; get; }
        /// <summary>
        /// 備註(必填)
        /// </summary>
        public string REMARK { set; get; }
        /// <summary>
        /// 已付租金(必填)
        /// </summary>
        public string PAYAMT { set; get; }
        /// <summary>
        /// 每日租金(必填)
        /// </summary>
        public string RPRICE { set; get; }
        /// <summary>
        /// 保費(日)(必填)
        /// </summary>
        public string RINV { set; get; }
        /// <summary>
        /// 折扣率(必填)
        /// </summary>
        public string DISRATE { set; get; }
        /// <summary>
        /// 折扣後金額(必填)
        /// </summary>
        public string NETRPRICE { set; get; }
        /// <summary>
        /// 租金小計(必填)
        /// </summary>
        public string RNTAMT { set; get; }
        /// <summary>
        /// 保費小計(必填)
        /// </summary>
        public string INSUAMT { set; get; }
        /// <summary>
        /// 預定使用天數(必填)
        /// </summary>
        public string RENTDAY { set; get; }
        /// <summary>
        /// EBONUS(必填)，代入0
        /// </summary>
        public string EBONUS { set; get; }
        /// <summary>
        /// 專案代碼(必填)
        /// </summary>
        public string PROJTYPE { set; get; }
        /// <summary>
        /// 預約型態(必填)
        /// <para>1:短租 </para>
        /// <para>2:附駕</para>
        /// </summary>
        public string TYPE { set; get; }
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
        /// 發票抬頭
        /// </summary>
        public string INVTITLE { set; get; }
        /// <summary>
        /// 發票統編
        /// </summary>
        public string UNIMNO { set; get; }
        /// <summary>
        /// 車輛編號(必填)
        /// </summary>
        public string TSEQNO { set; get; }
        /// <summary>
        /// 預約編號
        /// </summary>
        public string ORDNO { set; get; }
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
