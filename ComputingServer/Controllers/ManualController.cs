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
            if (_concurrentTcpServer.RobotIsConnected)
            {
                await _concurrentTcpServer.Move(movingDirection.Direction);
                return movingDirection.Direction.ToString();
            }
            return "Robot is not connected";
        }
    }
}