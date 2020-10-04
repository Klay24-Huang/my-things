using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_UploadCarImage
    {
        //public int CarImage_1 { set; get; }
        //public int CarImage_2 { set; get; }
        //public int CarImage_3 { set; get; }
        //public int CarImage_4 { set; get; }
        //public int CarImage_5 { set; get; }
        //public int CarImage_6 { set; get; }
        //public int CarImage_7 { set; get; }
        //public int CarImage_8 { set; get; }
        public List<CarImageData> CarImageObj { set; get; }

    }
    public class CarImageData
    {
        public int CarImageType { set; get; }
        public int HasUpload { set; get; }
    }
}