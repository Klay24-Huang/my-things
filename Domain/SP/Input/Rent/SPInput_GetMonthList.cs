﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    public class SPInput_GetMonthList
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        /// <summary>
        /// 月租分類
        /// <para>0無</para>
        /// <para>1月租</para>
        /// <para>2訂閱制</para>
        /// <para>3短租</para>
        /// </summary>
        public int MonType { get; set; }
    }
}
