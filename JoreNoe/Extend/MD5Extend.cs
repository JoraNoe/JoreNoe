using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.Extend
{
    public class MD5Extend
    {
        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ComputeMD5Hash(string input)
        {
            // 将字符串转换为字节数组
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                // 计算哈希值
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将字节数组转换为16进制字符串
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
