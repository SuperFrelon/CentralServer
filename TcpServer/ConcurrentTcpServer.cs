using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    public class ConcurrentTcpServer
    {
        TcpListener _tcpListener;
        bool _isOpen;
        bool _robotIsConnected;
        TcpClient _tcpClient;
        private readonly AsyncLock _mutex = new AsyncLock();

        public IPEndPoint LocalEndPoint { get; set; }

        public bool RobotIsConnected => _robotIsConnected;

        public enum Action
        {
            None,
            Left,
            Right,
            Forward,
            Backward,
            BackLeft,
            BackRight,
            RotateLeft,
            RotateRight,
            Stop
        }

        public void Start()
        {
            using (_mutex.Lock())
            {
                if (_isOpen) throw new InvalidOperationException("Server is already started");
                if (LocalEndPoint == null) throw new InvalidOperationException("LocalEndPoint must not be null");

                Console.WriteLine("Is Started");
                _tcpListener = new TcpListener(LocalEndPoint);
                _tcpListener.Start();
                _isOpen = true;

                Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();
                            if (_tcpClient != null) _tcpClient.Dispose();
                            _tcpClient = tcpClient;
                            _robotIsConnected = true;
                            Console.Beep(1000, 500);
                        }
                        catch(ObjectDisposedException)
                        {
                            if (_isOpen) throw;
                            break;
                        }
                    }
                });
            }
        }

        public async Task Move(Action action)
        {
            using (await _mutex.LockAsync())
            {
                await Ping();

                if (_robotIsConnected)
                {
                    using (StreamWriter streamWriter = new StreamWriter(_tcpClient.GetStream(), Encoding.UTF8, 2048, true))
                    {
                        await streamWriter.WriteLineAsync(action.ToString());
                        await streamWriter.FlushAsync();
                        Console.WriteLine("Instruction send : {0}", action);
                    }
                }
            }
        }

        public async Task Stop()
        {
            using (await _mutex.LockAsync())
            {
                if (!_isOpen) throw new InvalidOperationException("The server must be open.");
                Console.WriteLine("Is Stopped");

                Debug.Assert(_tcpListener != null);
                // Prevenir client d'une deco
                // Puis les déco
                _tcpListener.Stop();
                _isOpen = false;
            }
        }

        static bool IsSocketConnected(Socket s)
        {
            return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
        }

        public async Task Ping()
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(_tcpClient.GetStream(), Encoding.UTF8, 2048, true))
                {
                    await streamWriter.WriteLineAsync("Ping");
                    await streamWriter.FlushAsync();
                }
                using (StreamReader streamReader = new StreamReader(_tcpClient.GetStream(), Encoding.UTF8, false, 2048, true))
                {
                    string responce = await streamReader.ReadLineAsync();
                    if (responce == "Pong") _robotIsConnected = true;
                }
            }
            catch(IOException)
            {
                _robotIsConnected = false;
            }
        }
    }
}