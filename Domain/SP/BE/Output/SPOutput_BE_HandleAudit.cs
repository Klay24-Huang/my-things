using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.SP.Output;

namespace Domain.SP.BE.Output
{
    public class SPOutput_BE_HandleAudit : SPOutput_Base
    {
        /// <summary>
        /// 首次審核 20220521 ADD BY ADAM 
        /// </summary>
        public string FirstAudit { get; set; }
    }
}
