using System.IO;
using System.Text;

namespace UnityModLoader.Library.Core.Logging
{
    public static class LoggerExtensions
    {
        public static void AddLine(this FileStream src, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + '\n');
            src.Write(data, 0, data.Length);

            src.Flush();
        }

        public static void SameLine(this FileStream src, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            src.Write(data, 0, data.Length);

            src.Flush();
        }
    }
}
