using NetMQ;
using NetMQ.Sockets;

namespace ZeroServer;

public class JobsService
{
    //collate jobs and ensure they're all complete
    //add a timeout to resend any incomplete jobs
    public async Task Start(int jobsNumber)
    {
        await Task.Run(() =>
        {
            using (var receiver = new PullSocket("@tcp://*:5558"))
            {
                for (int taskNumber = 0; taskNumber < 100; taskNumber++)
                {
                    var workerDoneTrigger = receiver.ReceiveFrameString();
                    if (taskNumber % 10 == 0)
                    {
                        Console.Write(":");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                //Calculate and report duration of batch
                Console.WriteLine("Jobs Complete.");
                Console.WriteLine();

            }
        });
    }
}
