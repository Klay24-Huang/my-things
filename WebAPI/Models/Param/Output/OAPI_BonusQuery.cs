using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.TB;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_BonusQuery
    {
        /// <summary>
        /// 20201009 ADD BY ADAM REASON.增加總點數跟總剩餘
        /// 折抵時數資料
        /// </summary>
        public List<BonusData> BonusObj { set; get; }
        /// <summary>
        /// 汽車折抵點數加總
        /// </summary>
        public int TotalCarGIFTPOINT { set; get; }
        /// <summary>
        /// 機車折抵點數加總
        /// </summary>
        public int TotalMotorGIFTPOINT { set; get; }
        /// <summary>
        /// 折抵點數加總
        /// </summary>
        public int TotalGIFTPOINT { set; get; }
        /// <summary>
        /// 汽車剩餘點數加總
        /// </summary>
        public int TotalCarLASTPOINT { set; get; }
        /// <summary>
        /// 機車剩餘點數加總
        /// </summary>
        public int TotalMotorLASTPOINT { set; get; }
        /// <summary>
        /// 剩餘點數加總
        /// </summary>
        public int TotalLASTPOINT { set; get; }
    }
}