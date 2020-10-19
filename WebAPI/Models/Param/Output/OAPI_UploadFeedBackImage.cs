using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_UploadFeedBackImage
    {

        public List<FeedBackImageData> FeedBackImageObj { set; get; }

    }
    public class FeedBackImageData
    {
        public int SEQNO { set; get; }
        public int HasUpload { set; get; }
    }
}