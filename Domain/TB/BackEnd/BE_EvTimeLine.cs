using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_EvTimeLine
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 出廠年份
        /// </summary>
        public string FactoryYear { set; get; }
        public string CCNum { set; get; }
        /// <summary>
        /// GPS狀態
        /// </summary>
        public Int16 GPSStatus { set; get; }
        public double Speed { set; get; }
        public Decimal Latitude { set; get; }
        public Decimal Longitude { set; get; }
        public DateTime GPSTime { set; get; }

        public Int64 CarStatus { set; get; }
    }
}
