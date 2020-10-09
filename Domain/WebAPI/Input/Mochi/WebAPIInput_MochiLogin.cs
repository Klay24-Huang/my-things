using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Mochi
{
    /// <summary>
    /// 車麻吉登入
    /// </summary>
    public class WebAPIInput_MochiLogin
    {
        public string username { set; get; }
        public string password { set; get; }
    }
}
