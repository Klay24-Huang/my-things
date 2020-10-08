using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OrderList
{
    public class SPInput_GetFinishOrder : SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public int ShowYear { set; get; }
        /// <summary>
        /// 每頁幾筆
        /// </summary>
        public int pageSize { set; get; }
        /// <summary>
        /// 第幾頁
        /// </summary>
        public int pageNo { set; get; }
    }
}
