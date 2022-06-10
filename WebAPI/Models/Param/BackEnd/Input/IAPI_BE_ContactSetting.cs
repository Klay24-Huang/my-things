using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_ContactSetting
    {
        public string OrderNo{ set; get; }
        /// <summary>
        /// 動作類型
        /// <para>0:強制取車</para>
        /// <para>1:強制還車</para>
        /// <para>2:強制取消</para>
        /// </summary>
        public Int16 type { set; get; }
        /// <summary>
        /// 動作用途
        /// <para>0:會員</para>
        /// <para>1:清潔取還車</para>
        /// <para>2:保修取還車</para>
        /// <para>3:系統操作異常</para>
        /// <para>4:逾時未還</para>
        /// <para>5:營運範圍外無法還車</para>
        /// <para>6:車輛沒電</para>
        /// <para>7:其他</para>
        /// </summary>
        public Int16 Mode { set; get; }
        /// <summary>
        /// 還車時間
        /// </summary>
        public string returnDate { set; get; }
        /// <summary>
        /// 發票寄送方式
        /// </summary>
        public string bill_option { set; get; }
        /// <summary>
        /// 手機條碼載具,自然人憑證載具
        /// </summary>
        public string CARRIERID { set; get; }
        /// <summary>
        /// 愛心碼
        /// </summary>
        public string NPOBAN { set; get; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string unified_business_no { set; get; }
        /// <summary>
        /// 停車格
        /// </summary>
        public string parkingSpace { set; get; }
        /// <summary>
        /// 車機出錯是否bypass
        /// <para>1:是</para>
        /// <para>0:否</para>
        /// </summary>
        public int ByPass { set; get; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { set; get; }
        /// <summary>
        /// 減免費用
        /// </summary>
        public int costRelife_cost { get; set; }
        /// <summary>
        /// 減免分鐘數
        /// </summary>
        public int costRelife_minute { get; set; }
        /// <summary>
        /// 未用車取消
        /// </summary>
        public bool notUseCar { get; set; }
        /// <summary>
        /// 取消逾時
        /// </summary>
        public bool cancelOvertime { get; set; }
        /// <summary>
        /// 機車折抵時數
        /// </summary>
        public int timeDiscount_motor { get; set; }
        /// <summary>
        /// 汽車折抵時數
        /// </summary>
        public int timeDiscount_car { get; set; }
    }
}