using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Wallet.ResultParam
{
    /// <summary>
    /// 條碼回覆物件
    /// </summary>
    public class BarcodeRes
    {

        /// <summary>
        /// BarCode 圖檔資料1.BASE64 ENCODE 字串 2.尺寸：320 X 480，由前端轉為PNG 格式等比顯示
        /// </summary>
        public string barcode64 { get; set; }

        /// <summary>
        /// 第一段條碼文字
        /// </summary>
        public string code1 { get; set; }

        /// <summary>
        /// 第二段銷帳編號
        /// </summary>
        public string code2 { get; set; }

        /// <summary>
        /// 第三段條碼文字
        /// </summary>
        public string code3 { get; set; }

        /// <summary>
        /// 回應狀態代碼
        /// </summary>
        public string statusCode { get; set; }

        /// <summary>
        /// 狀態說明
        /// </summary>
        public string statusDesc { get; set; }

    }

}
