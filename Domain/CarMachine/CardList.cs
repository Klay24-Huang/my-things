using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CarMachine
{
   public class CardList
    {
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNO { get; set; }
        /// <summary>
        /// 類型
        /// <para>C:顧客卡</para>
        /// <para>M:萬用卡</para>
        /// </summary>
        public string CardType { get; set; }
    }
}
