using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    /// <summary>
    /// 寫入強還是bypass的錯誤
    /// </summary>
   public  class SPInput_BE_InsCarReturnError:SPInput_Base
    {
        public Int64 OrderNo { set; get; }
        public string CarError { set; get; }
        public string UserID { set; get; }
    }
}
