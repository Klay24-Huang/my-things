using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HandleOrderModify:IAPI_BE_Base
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
        /// 開始時間
        /// </summary>
        public DateTime StartDate { set; get; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime EndDate { set; get; }
        /// <summary>
        /// 開始里程
        /// </summary>
        public float start_mile { set; get; }
        /// <summary>
        /// 結束里程
        /// </summary>
        public float end_mile { set; get; }
        /// <summary>
        /// 逾時費
        /// </summary>
        public int fine_price { set; get; }
        /// <summary>
        /// 安心服務
        /// </summary>
        public int Insurance_price { set; get; }

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
        /// <summary>
        /// 車輛調度費
        /// </summary>
        public int CarDispatch { set; get; }
        /// <summary>
        /// 車輛調度費備註
        /// </summary>
        public string DispatchRemark { set; get; }
        /// <summary>
        /// 清潔費
        /// </summary>
        public int CleanFee { set; get; }
        /// <summary>
        /// 清潔費說明
        /// </summary>
        public string CleanFeeRemark { set; get; }
        /// <summary>
        /// 物品損壞/遺失費
        /// </summary>
        public int DestroyFee { set; get; }
        /// <summary>
        ///  物品損壞/遺失費設明
        /// </summary>
        public string DestroyFeeRemark { set; get; }
        /// <summary>
        /// 停車費
        /// </summary>
        public int ParkingFee { set; get; }
        /// <summary>
        /// 停車費說明
        /// </summary>
        public string ParkingFeeRemark { set; get; }
        /// <summary>
        /// 拖吊費
        /// </summary>
        public int DraggingFee { set; get; }
        /// <summary>
        /// 拖吊費備註
        /// </summary>
        public string DraggingFeeRemark { set; get; }
        /// <summary>
        /// 其他
        /// </summary>
        public int OtherFee { set; get; }
        /// <summary>
        /// 其他費用備註
        /// </summary>
        public string OtherFeeRemark { set; get; }
        /// <summary>
        /// 代收停車費
        /// </summary>
        public int ParkingFeeByMachi { set; get; }
        /// <summary>
        /// 代收停車費說明
        /// </summary>
        public string ParkingFeeByMachiRemark { set; get; }
        /// <summary>
        /// 里程費用
        /// </summary>
        public int Mileage { set; get; }
    }
}