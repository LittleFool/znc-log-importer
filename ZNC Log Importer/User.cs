using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ZNC_Log_Importer
{
    class User
    {
        private string name;
        private string mode;
        private List<Message> messages = new List<Message>();

        public User(string name, string mode)
        {
            this.name = name;
            this.mode = mode;
        }

        public void setMode(DateTime time, string mode)
        {
            Message m = new Message(time, this.mode, name, "changed mode to " + mode);
            messages.Add(m);
            this.mode = mode;
        }

        public void setName(DateTime time, string name)
        {
            Message m = new Message(time, mode, this.name, "changed name to " + name);
            messages.Add(m);
            this.name = name;
        }

        public void addMessage(Message m)
        {
            messages.Add(m);
        }
    }
}
