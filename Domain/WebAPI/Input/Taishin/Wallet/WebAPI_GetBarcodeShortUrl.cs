﻿using Domain.WebAPI.Input.Taishin.Wallet.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Wallet
{
    public class WebAPI_GetBarcodeShortUrl
    {
        public IHUBReqHeader header { get; set; }

        public ShortUrlReq body { get; set; }
    }

}
