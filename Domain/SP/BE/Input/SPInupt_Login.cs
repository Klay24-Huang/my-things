using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInupt_Login
    {
        public string Account { set; get; }
        public string UserPwd { set; get; }

        public string ClientIP { set; get; }
    }
}
