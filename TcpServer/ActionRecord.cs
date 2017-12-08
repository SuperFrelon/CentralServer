using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace TcpServer
{
    public class ActionRecord
    {
        public ConcurrentTcpServer.Action Action { get; set; }

        public Timer Timer { get; set; }
    }
}
