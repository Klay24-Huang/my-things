using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
   public  class BE_HoildayData
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int year { set; get; }
        /// <summary>
        /// 月份
        /// </summary>
        public int month { set; get; }
        /// <summary>
        /// 日期
        /// </summary>
        public int day { set; get; }
        /// <summary>
        /// 是否是假日
        /// </summary>
        public int IsHoilday { set; get; }
        /// <summary>
        /// 是否點選
        /// </summary>
        public int IsSelect { set; get; }
    }
}
