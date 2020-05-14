using System;
using System.Collections.Generic;
using System.Text;

namespace ZNC_Log_Importer
{
    class Message
    {
        private DateTime time { get; }
        private String mode { get; }
        private String name { get; }
        private String text { get; }

        public Message(DateTime time, String mode, String name, String text)
        {
            this.time = time;
            this.mode = mode;
            this.name = name;
            this.text = text;
        }
    }
}
