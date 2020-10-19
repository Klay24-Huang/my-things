using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 上傳多筆取車回饋照片
    /// </summary>
    public class IAPI_UploadFeedBackImage
    {
        public string OrderNo { set; get; }
        public List<FeedBackImage> FeedBack { set; get; }
    }
    public class FeedBackImage
    {
        /// <summary>
        /// 1~4
        /// </summary>
        public int SEQNO { set; get; }
        /// <summary>
        /// 圖片base64
        /// </summary>
        public string FeedBackFile { set; get; }
    }
}