﻿namespace Domain.SP.Input.Member
{
    public class SPInput_MemberUnBindCheck : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
    }
}