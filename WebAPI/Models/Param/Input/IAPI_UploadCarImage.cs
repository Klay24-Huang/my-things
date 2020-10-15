using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_UploadCarImage
    {
        public string OrderNo { set; get; }
        /// <summary>
        /// 模式
        /// <para>0:取車</para>
        /// <para>1:還車</para>
        /// </summary>
        public int Mode { set; get; }
        /// <summary>
        /// 1~8
        /// </summary>
        public int CarType { set; get; }

        /// <summary>
        /// 圖片base64
        /// </summary>
        public string CarImage { set; get; }

    }

    public class IAPI_UploadCarImage2
    {
        public string OrderNo { set; get; }
        /// <summary>
        /// 模式
        /// <para>0:取車</para>
        /// <para>1:還車</para>
        /// </summary>
        public int Mode { set; get; }
        /// <summary>
        /// 1~8
        /// </summary>
        //public int CarType { set; get; }

        /// <summary>
        /// 圖片base64
        /// </summary>
        //public string CarImage { set; get; }

        public List<CarImages> CarImages { get; set; }
    }

    public class CarImages
    {
        /// <summary>
        /// 1~8
        /// </summary>
        public int CarType { set; get; }
        /// <summary>
        /// 圖片base64
        /// </summary>
        public string CarImage { set; get; }
    }
}