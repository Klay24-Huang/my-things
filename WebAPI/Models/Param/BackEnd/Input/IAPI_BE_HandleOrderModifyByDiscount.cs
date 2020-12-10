using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HandleOrderModifyByDiscount:IAPI_BE_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 使用的汽車點數
        /// </summary>
        public int CarPoint { set; get; }
        /// <summary>
        /// 使用的機車點數
        /// </summary>
        public int MotorPoint { set; get; }
        /// <summary>
        /// 專案類型
        /// </summary>
        public int ProjType { set; get; }
        /// <summary>
        /// 差額
        /// </summary>
        public int DiffPrice { set; get; }
        /// <summary>
        /// 修改後的金額
        /// </summary>
        public int FinalPrice { set; get; }
        /// <summary>
        /// 修改原因
        /// <para>0:停車場出入異常</para>
        /// <para>1:清潔耽誤出還車</para>
        /// <para>2:取車時沒電耽誤</para>
        /// <para>3:其他</para>
        /// </summary>
        public int UseStatus { set; get; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { set; get; }
        public int PAYAMT { set; get; } //20201210唐加
    }
}