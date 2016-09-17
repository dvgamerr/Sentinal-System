using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travox.Sentinel.Engine
{
    public class ConsoleWriter : TextWriter
    {
        private StreamWriter Stream;

        public override Encoding Encoding { get { return Encoding.UTF8; } }

        public ConsoleWriter(String path, FileMode mode)
        {
            Stream = new StreamWriter(new FileStream(path, mode));

            Stream.AutoFlush = true;
        }

        public override void Write(string value)
        {
            if (WriteEvent != null) WriteEvent(this, new ConsoleWriterEventArgs(value));
            Stream.Write(value);
            base.Write(value);
        }

        public override void WriteLine(string value)
        {
            if (WriteLineEvent != null) WriteLineEvent(this, new ConsoleWriterEventArgs(value));
            Stream.WriteLine(value);
            base.WriteLine(value);
        }

        public event EventHandler<ConsoleWriterEventArgs> WriteEvent;
        public event EventHandler<ConsoleWriterEventArgs> WriteLineEvent;
    }

    public class ConsoleWriterEventArgs : EventArgs
    {
        public string Value { get; private set; }
        public ConsoleWriterEventArgs(string value)
        {
            Value = value;
        }
    }

}

