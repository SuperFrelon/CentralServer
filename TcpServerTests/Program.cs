using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TcpServer;

namespace TcpServerTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //AsyncMain().Wait();
            ReconnectTest();
            Console.ReadLine();
        }

        static async Task AsyncMain()
        {

            ConcurrentTcpServer concurrentTcpServer = new ConcurrentTcpServer();
            concurrentTcpServer.LocalEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);

            concurrentTcpServer.Start();
            await Task.Delay(15000);

            //TcpClient tcpClient = new TcpClient(); //
            //await tcpClient.ConnectAsync("127.0.0.1", 8000); //

            await concurrentTcpServer.Move(ConcurrentTcpServer.Action.Backward);
            await concurrentTcpServer.Move(ConcurrentTcpServer.Action.Forward);
            await concurrentTcpServer.Move(ConcurrentTcpServer.Action.Left);
            await concurrentTcpServer.Move(ConcurrentTcpServer.Action.Right);

            //using (StreamReader sr = new StreamReader(tcpClient.GetStream())) //
            //{
            //    Console.WriteLine("Test : " + await sr.ReadLineAsync()); //
            //    Console.WriteLine("Test : " + await sr.ReadLineAsync()); //
            //    Console.WriteLine("Test : " + await sr.ReadLineAsync()); //
            //    Console.WriteLine("Test : " + await sr.ReadLineAsync()); //
            //}

            //await concurrentTcpServer.Stop(); //
            //tcpClient.Close(); //
        }

        static void ReconnectTest()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);

            Thread thread1 = new Thread(() =>
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    while (true)
                    {
                        try
                        {
                            Console.WriteLine("Tentative");
                            socket.Connect(endPoint);
                            Console.WriteLine("Connect !");
                            break;
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    Console.WriteLine("I'm connected");
                }
            });

            Thread thread2 = new Thread(() =>
            {
                Console.WriteLine("Thread Server launched");
                ConcurrentTcpServer concurrentTcpServer = new ConcurrentTcpServer
                {
                    LocalEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000)
                };
                Thread.Sleep(10000);
                concurrentTcpServer.Start();
            });

            thread1.Start();
            thread2.Start();
        }
    }
}
