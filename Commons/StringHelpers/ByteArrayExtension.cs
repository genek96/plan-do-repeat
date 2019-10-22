using System.Text;

namespace Commons.StringHelpers
{
    public static class ByteArrayExtension
    {
        public static string ToUtf8String(this byte[] source)
        {
            return Encoding.UTF8.GetString(source);
        }
    }
}
