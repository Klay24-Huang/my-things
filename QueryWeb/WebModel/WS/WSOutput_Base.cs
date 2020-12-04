using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WebCommon;

namespace WebModel
{
    public class WSInput_MemberLogin
    {
        public string MEMIDNO
        {
            get;
            set;
        }

        public string MEMPWD
        {
            get;
            set;
        }

        public WSInput_MemberLogin()
        {
        }
    }
}