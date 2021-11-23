using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Member
{
    public class HotaiMemberRequestBody
    {
        /// <summary>
        /// WebRequest Method(POST/GET)
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// Route
        /// </summary>
        public string Route { get; set; }
        /// <summary>
        /// really Body(AES128 Encrypt)
        /// </summary>
        public string Body { get; set; }
    }
}
