using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebCommon
{
    /// <summary>
    /// AES加密
    /// </summary>
    public class AESEncrypt
    {
        private enum EncryptMode
        {
            Encrypt,
            Decrypt
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="key">金鑰</param>
        /// <param name="salt">鹽巴</param>
        /// <param name="sourceCode">原始碼</param>
        /// <returns></returns>
        public string doEncrypt(string key, string salt, string sourceCode)
        {
            return EncryptHandle(key, salt, sourceCode, EncryptMode.Encrypt);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="key">金鑰</param>
        /// <param name="salt">鹽巴</param>
        /// <param name="sourceCode">要解密的文字</param>
        /// <returns>原文</returns>
        public string doDecrypt(string key, string salt, string sourceCode)
        {
            return EncryptHandle(key, salt, sourceCode, EncryptMode.Decrypt);
        }
        /// <summary>
        /// Refactor 將加/解密會用到的重覆指令抽取
        /// </summary>
        /// <param name="key">金鑰</param>
        /// <param name="salt">鹽巴</param>
        /// <param name="sourceCode">要加/解密的文字</param>
        /// <param name="mode">
        ///         <para>EncrypteMode.Encrypt：加密</para>
        ///         <para>EncrypteMode.Decrypt：解密</para>
        /// </param>
        /// <returns>String</returns>
        private string EncryptHandle(string key, string salt, string sourceCode, EncryptMode mode)
        {
            string returnStr = "";
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (System.Security.Cryptography.AesCryptoServiceProvider AES = new System.Security.Cryptography.AesCryptoServiceProvider())
                {
                    try
                    {
                        MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
                        AES.Key = MD5.ComputeHash(Encoding.UTF8.GetBytes(key));
                        AES.IV = MD5.ComputeHash(Encoding.UTF8.GetBytes(salt));

                        ICryptoTransform transform;

                        byte[] sourceBytes;
                        if (EncryptMode.Decrypt == mode)
                        {
                            transform = AES.CreateDecryptor();
                            sourceBytes =  HexStrToByte(sourceCode);
                        }
                        else
                        {
                            transform = AES.CreateEncryptor();
                            sourceBytes = Encoding.UTF8.GetBytes(sourceCode);
                            //sourceCode = key + salt + sourceCode;
                        }

                        byte[] outputData = transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);
                        if (mode == EncryptMode.Decrypt)
                        {
                            returnStr = Encoding.UTF8.GetString(outputData);
                        }
                        else
                        {
                            returnStr =  changeToHexStr(outputData);
                        }

                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                    return returnStr;
                }
            }
        }
        /// <summary>
        /// 轉為HEX STRING
        /// </summary>
        /// <param name="source">
        ///     <para>DataType: byte[]</para>
        ///     <para>加密後的文字以byte格式轉為HEX STR</para>
        /// </param>
        /// <returns></returns>
        public string changeToHexStr(byte[] source)
        {
            return BitConverter.ToString(source).Replace("-", string.Empty);
        }
        /// <summary>
        /// HEX轉str
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public byte[] HexStrToByte(string source)
        {
            int NumberChars = source.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(source.Substring(i, 2), 16);
            return bytes;
        }



        /// <summary>
        /// 使用AES 128 加密  20210316 ADD BY ADAM
        /// </summary>
        /// <param name="source">本文</param>
        /// <param name="key">因為是256 所以你密碼必須為32英文字=32*8=256</param>
        /// <param name="iv">IV為128 所以為 16 * 8= 128</param>
        /// <returns></returns>
        public static string EncryptAES128(string source, string key, string iv)
        {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            var aes = new RijndaelManaged();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform transform = aes.CreateEncryptor();

            return Convert.ToBase64String(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length));

        }

        /// <summary>
        /// 使用AES 128 解密 20210316 ADD BY ADAM 
        /// </summary>
        /// <param name="encryptData">Base64的加密後的字串</param>
        /// <param name="key">因為是256 所以你密碼必須為32英文字=32*8=256</param>
        /// <param name="iv">IV為128 所以為 16 * 8= 128</param>
        /// <returns></returns>
        public static string DecryptAES128(string encryptData, string key, string iv)
        {
            var encryptBytes = Convert.FromBase64String(encryptData);
            var aes = new RijndaelManaged();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform transform = aes.CreateDecryptor();

            return Encoding.UTF8.GetString(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length));

        }
    }
}
