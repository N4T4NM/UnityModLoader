using System;
using System.IO;

namespace UnityModLoader.Library.Core.Logging
{
    public class Logger
    {
        public static readonly Logger Instance = new Logger();

        FileStream _fStream;
        private Logger()
        {
            FileStream fStream = new FileStream("UnityModLoader.Log.txt", FileMode.Create);
            _fStream = fStream;
        }

        public void Append(string message)
            => _fStream.SameLine(message);

        public void Log(string message, bool nextLine = true, MessageType messageType = MessageType.Info)
        {
            string str = $"{DateTime.Now.ToString("T")} - [{messageType.ToString().ToUpper()}] {message}";
            if (nextLine)
                _fStream.AddLine(str);
            else _fStream.SameLine(str);
        }

        public enum MessageType
        {
            Info,
            Warning,
            Success,
            Error
        }
    }
}
