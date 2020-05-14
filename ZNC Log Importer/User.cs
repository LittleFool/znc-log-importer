﻿using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ZNC_Log_Importer
{
    class User
    {
        public string name { private set; get; }
        private string mode = "";
        private List<Message> messages = new List<Message>();

        public User(LocalDateTime dt, string name)
        {
            this.name = name;

            Message m = new Message(dt, mode, name, "joins");
            messages.Add(m);
        }

        public void setMode(LocalDateTime dt, string mode)
        {
            Message m = new Message(dt, this.mode, name, "set mode " + mode);
            messages.Add(m);
            this.mode = mode;
        }

        public void setName(LocalDateTime dt, string name)
        {
            Message m = new Message(dt, mode, this.name, "changed name to " + name);
            messages.Add(m);
            this.name = name;
        }

        public void addMessage(Message m)
        {
            messages.Add(m);
        }

        public void quit(LocalDateTime dt)
        {
            Message m = new Message(dt, mode, name, "quits");
            messages.Add(m);
        }
    }
}
