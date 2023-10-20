
using Microsoft.AspNetCore.Hosting.Server;
using NetMQ;
using NetMQ.Sockets;
using System.Reflection;
using static System.Reflection.Metadata.BlobBuilder;

namespace ZeroServer
{
    public class SocketService : BackgroundService
    {
        private JobsService _jobs;

        public SocketService(JobsService jobs)
        {
            _jobs = jobs;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Server starting...");

            await Task.Run(() =>
            {
               
                using (var queue = new PushSocket("@tcp://*:5557"))
                using (var server = new ResponseSocket())
                {
                    server.Bind("tcp://*:5555");
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                        var jobsMsg = server.ReceiveFrameString();
                        int jobsCount = Convert.ToInt32(jobsMsg);
                        Console.WriteLine("Received {0}", jobsCount);
                        _jobs.Start(jobsCount);
                        // processing the request
                        Console.WriteLine("Sending tasks to workers");
                        //initialise random number generator
                        Random rand = new Random(0);
                        //expected costs in Ms
                        int totalMs = 0;
                        //send 100 tasks (workload for tasks, is just some random sleep time that
                        //the workers can perform, in real life each work would do more than sleep
                        for (int taskNumber = 0; taskNumber < jobsCount; taskNumber++)
                        {
                            //Random workload from 1 to 100 msec
                            int workload = rand.Next(0, 100);
                            totalMs += workload;
                            Console.WriteLine("Workload : {0}", workload);
                            queue.SendFrame(workload.ToString());
                        }
                        Console.WriteLine("Total expected cost : {0} msec", totalMs);

                        Console.WriteLine("Sending World");
                        server.SendFrame(totalMs.ToString());
                    }
                }
            });
            Console.WriteLine("Server started...");
        }

    }
}
