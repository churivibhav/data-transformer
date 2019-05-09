using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Vhc.Optimus.Services
{
    public class StreamWriterEventArgs<T> : EventArgs
    {
        public T Value
        {
            get;
            private set;
        }
        public StreamWriterEventArgs(T value)
        {
            this.Value = value;
        }
    }


    public class EventRaisingStreamWriter : StreamWriter
    {
        public event EventHandler<StreamWriterEventArgs<string>> StringWritten;

        public EventRaisingStreamWriter(Stream s) : base(s)
        { }

        private void LaunchEvent(string txtWritten)
        {
            StringWritten?.Invoke(this, new StreamWriterEventArgs<string>(txtWritten));
        }

        public override void Write(string value)
        {
            base.Write(value);
            LaunchEvent(value);
        }
        public override void Write(bool value)
        {
            base.Write(value);
            LaunchEvent(value.ToString());
        }

    }

}
