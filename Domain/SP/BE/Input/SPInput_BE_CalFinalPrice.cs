﻿using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_CalFinalPrice : SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        public string UserID { set; get; }
        /// <summary>
        /// 總價
        /// </summary>
        public int final_price { set; get; } = 0;
        /// <summary>
        /// 車輛租金
        /// </summary>
        public int pure_price { set; get; } = 0;
        /// <summary>
        /// 里程費
        /// </summary>
        public int mileage_price { set; get; } = 0;
        /// <summary>
        /// 安心服務費
        /// </summary>
        public int Insurance_price { set; get; } = 0;
        /// <summary>
        ///  罰金
        /// </summary>
        public int fine_price { set; get; } = 0;
        /// <summary>
        /// 使用時數(汽車)
        /// </summary>
        public int gift_point { set; get; } = 0;

        /// <summary>
        /// ETAG費用
        /// </summary>
        public int Etag { set; get; } = 0;
        /// <summary>
        /// 特約停車場停車費
        /// </summary>
        public int parkingFee { set; get; } = 0;
        /// <summary>
        /// 轉乘優惠
        /// </summary>
        public int TransDiscount { set; get; } = 0;

    }
}
