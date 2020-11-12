using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Car
{
    public class SPInput_CarStatus: SPInput_Base
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
        /// 車號
        /// </summary>
        public string CarNo { get; set; }
    }
}
