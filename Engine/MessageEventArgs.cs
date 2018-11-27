using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class MessageEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public bool ExtraNewLine { get; private set; }

        public MessageEventArgs(string message, bool extraNewLine)
        {
            Message = message;
            ExtraNewLine = extraNewLine;
        }
    }
}
