using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace ClusterClient.Clients
{
    public class SmartClusterClient : ClusterClientBase
    {
        private readonly Random random = new Random();
        private readonly ConcurrentBag<Task<string>> Tasks;
        public SmartClusterClient(string[] replicaAddresses) : base(replicaAddresses)
        {
            Tasks = new ConcurrentBag<Task<string>>();
        }

        public override async Task<string> ProcessRequestAsync(string query, TimeSpan timeout)
        {
            foreach (var uri in ReplicaAddresses.OrderBy(x => random.Next()))
            {
                var webRequest = CreateRequest(uri + "?query=" + query);
                var resultTask = ProcessRequestAsync(webRequest);
                Console.WriteLine($"Processing {webRequest.RequestUri}");

                Tasks.Add(resultTask);
                if (TryCheckIfPreviousCompleted(out var completed))
                {
                    Console.WriteLine($"While processing {webRequest.RequestUri} old task {completed.Id} was completed");
                    return completed.Result;
                }
                
                await Task.WhenAny(resultTask, Task.Delay(GetSoftTimeout(timeout)));
                if (resultTask.IsCompleted)
                    return resultTask.Result;
            }

            // wait extra 
            Console.WriteLine($"Waiting extra than timeout {timeout.ToString()}"); 
            var answered = await Task.WhenAny(Tasks);
            if (answered.IsCompleted)
                return answered.Result;
            // or do not wait excess 
            throw new TimeoutException();
        }

        private bool TryCheckIfPreviousCompleted(out Task<string> completedTask)
        {
            foreach (var task in Tasks)
            {
                if (task.IsCompleted)
                {
                    completedTask = task;
                    return true;
                }
            }

            completedTask = null;
            return false;
        }

        private TimeSpan GetSoftTimeout(TimeSpan timeSpan)
        {
            return timeSpan.Divide(ReplicaAddresses.Length);
        }

        protected override ILog Log { get; }       
    }
}