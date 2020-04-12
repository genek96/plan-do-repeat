using System;
using System.Collections.Generic;
using System.Text;

namespace Commons.StringHelpers
{
    public static class StringExtension
    {
        public static byte[] ToByteArray(this string source)
        {
            return Encoding.UTF8.GetBytes(source);
        }
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
