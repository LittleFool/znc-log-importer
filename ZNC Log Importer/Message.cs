using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZNC_Log_Importer
{
    class Message
    {
        private LocalDateTime dt { get; }
        private String mode { get; }
        private String name { get; }
        private String text { get; }

        public Message(LocalDateTime dt, String mode, String name, String text)
        {
            this.dt = dt;
            this.mode = mode;
            this.name = name;
            this.text = text;
        }
    }
}
