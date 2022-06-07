using JoreNoe.CommonInterFaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.JoreVerify
{
    public class VerifyApi
    {
        public const string Empty = "字段：{0},为空";
        public const string Null = "{0}为NULL";

        /// <summary>
        /// 检查是否为空
        /// </summary>
        /// <param name="Arg">参数</param>
        /// <returns></returns>
        public static bool CheckIsEmpty(string Arg)
        {
            if (string.IsNullOrEmpty(Arg))
                return false;
            else
                return true;
        }

    }
}
