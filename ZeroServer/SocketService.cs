
using Microsoft.AspNetCore.Hosting.Server;
using NetMQ;
using NetMQ.Sockets;

namespace ZeroServer
{
    public class SocketService : BackgroundService
    {
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Server starting...");

            await Task.Run(() =>
            {
                using (var server = new ResponseSocket())
                {
                    server.Bind("tcp://*:5555");
                    while (!stoppingToken.IsCancellationRequested)
                    //while (true)
                    {
                        //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                        var message = server.ReceiveFrameString();
                        Console.WriteLine("Received {0}", message);
                        // processing the request
                        Console.WriteLine("Sending World");
                        server.SendFrame("World");
                    }
                }
            });
            Console.WriteLine("Server started...");
        }

    }
}
