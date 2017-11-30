using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TcpServer;
using ComputingServer.Models;

namespace ComputingServer.Controllers
{
    public class ManualController : Controller
    {
        ConcurrentTcpServer _concurrentTcpServer;

        public ManualController(ConcurrentTcpServer concurrentTcpServer)
        {
            _concurrentTcpServer = concurrentTcpServer;
        }

        [HttpGet]
        public async Task<string> Move(MovingDirection movingDirection)
        {
            if (movingDirection.Direction == ConcurrentTcpServer.Action.Right) await _concurrentTcpServer.Move(ConcurrentTcpServer.Action.Right);
            else if (movingDirection.Direction == ConcurrentTcpServer.Action.Left) await _concurrentTcpServer.Move(ConcurrentTcpServer.Action.Left);
            else if (movingDirection.Direction == ConcurrentTcpServer.Action.Forward) await _concurrentTcpServer.Move(ConcurrentTcpServer.Action.Forward);
            else if (movingDirection.Direction == ConcurrentTcpServer.Action.Backward) await _concurrentTcpServer.Move(ConcurrentTcpServer.Action.Backward);
            else if (movingDirection.Direction == ConcurrentTcpServer.Action.Stop) await _concurrentTcpServer.Move(ConcurrentTcpServer.Action.Stop);

            return movingDirection.Direction.ToString();
        }
    }
}