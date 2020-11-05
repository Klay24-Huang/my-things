﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Member
{
    public class SPInput_MemberStatus:SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// JWT Token
        /// </summary>
        public string Token { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public Int64 LogID { set; get; }
    }
}