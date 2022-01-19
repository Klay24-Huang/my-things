using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CTBCPOS
{
    /// <summary>
    /// 請款
    /// </summary>
    public class WebAPIInput_Cap : WebAPIInput_CTBCBase
    {

        /// <summary>
        /// 請款金額
        /// </summary>
        public int CapAmt { get; set; } = 0;

    }
}
