using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Hotai.Payment
{
    /// <summary>
    /// Hotai pay Request Body
    /// </summary>
    public class HotaiPCRequestBody
    {
        /// <summary>
        /// WebRequest Method(POST/GET)
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// Hotai Pay Service Route
        /// </summary>
        public string API { get; set; }
        /// <summary>
        /// really Body(AES128 Encrypt)
        /// </summary>
        public string Body { get; set; }
    }
}
