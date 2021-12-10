using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin
{
    /// <summary>
    /// 中介程式傳輸參數
    /// </summary>
    public class WebAPIInput_RelayPost
    {
        public string BaseUrl { get; set; }
        public string ApiUrl { get; set; }
        public string RequestData { get; set; }
    }
}
