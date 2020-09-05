using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OtherService.Common
{
    public class WebServiceBase : IDisposable
    {
        protected string userid;
        protected string apikey;
        bool disposed = false;
        public WebServiceBase()
        {
            apikey = ConfigurationManager.AppSettings.Get("HLCkey");
            userid = ConfigurationManager.AppSettings.Get("userid");
        }
        /// <summary>
        /// 產生憑證碼
        /// </summary>
        /// <returns></returns>
        protected string GenerateSig()
        {
            string EncryptStr = "";
            string sourceStr = apikey + userid + System.DateTime.Now.ToString("yyyyMMdd");
            ASCIIEncoding enc = new ASCIIEncoding();
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] shaHash = sha.ComputeHash(enc.GetBytes(sourceStr));
            EncryptStr = System.BitConverter.ToString(shaHash).Replace("-", string.Empty);
            return EncryptStr;
        }
        /// <summary>
        /// implementation of Dispose pattern callable by consumers
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Dispose();
            }
            disposed = true;
        }

        ~WebServiceBase()
        {
            Dispose(false);
        }
    }
}
