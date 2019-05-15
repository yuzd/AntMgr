using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infrastructure.Logging
{
    public class LogStream : StreamWriter
    {
        public LogStream(string path) : base(path, true, Encoding.UTF8)
        {

        }

        public override void WriteLine(string message)
        {
            var txt = $"[{DateTime.Now.ToString("MM-dd HH:mm:ss")}]{message}";
            Console.WriteLine(txt);
            base.WriteLine(txt);
            this.Flush();
        }
    }
}
