using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class FtpCityParkData
    {
        /// <summary>
        /// 停車場ID
        /// </summary>
        public string Ftp_facility_id { get; set; }
        /// <summary>
        /// 車輛入車編號
        /// </summary>
        public string Ftp_entrance_uuid { get; set; }
        /// <summary>
        /// 車號
        /// </summary>
        public string Ftp_license_plate_number { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Ftp_entered_at { get; set; }
        /// <summary>
        /// 車輛入場車道
        /// </summary>
        public string Ftp_entrance_id { get; set; }
        /// <summary>
        /// 車輛離場時間
        /// </summary>
        public string Ftp_left_at { get; set; }
        /// <summary>
        /// 車輛離場車道
        /// </summary>
        public string Ftp_exit_id { get; set; }
        /// <summary>
        /// 停車費用
        /// </summary>
        public string Ftp_amount { get; set; }
        

    }
}
