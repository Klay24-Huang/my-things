using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Arrears
{
    public class SPInput_ArrearsQuery
    {
        /// <summary>
        /// 使否儲存查詢紀錄:0(不儲存), 1(儲存)
        /// </summary>
        public int IsSave { get; set; }
        public Int64 LogID { get; set; }
    }
}
