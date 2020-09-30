using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin
{
   public  class DeleteCreditCardAuthRequestParamasData
    {
        /// <summary>
        /// 替代性信用卡卡號或替代表銀行卡號
        /// </summary>
        public string CardToken { get; set; }
        /// <summary>
        /// 會員編號
        /// </summary>
        public string MemberId { set; get; }
    }
}
