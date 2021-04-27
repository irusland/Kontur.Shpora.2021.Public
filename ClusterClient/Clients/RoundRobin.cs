using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace ClusterClient.Clients
{
    public class RoundRobin : ClusterClientBase
    {
        private readonly Random random = new Random();

        public RoundRobin(string[] replicaAddresses) : base(replicaAddresses)
        {
        }

        public override async Task<string> ProcessRequestAsync(string query, TimeSpan timeout)
        {
            foreach (var uri in ReplicaAddresses.OrderBy(x => random.Next()))
            {
                var webRequest = CreateRequest(uri + "?query=" + query);
                var resultTask = ProcessRequestAsync(webRequest);
                Console.WriteLine($"Processing {webRequest.RequestUri}");


                await Task.WhenAny(resultTask, Task.Delay(timeout));
                if (resultTask.IsCompleted)
                    return resultTask.Result;
            }
            
            throw new TimeoutException();

        }

        private TimeSpan GetSoftTimeout(TimeSpan timeSpan)
        {
            return timeSpan.Divide(ReplicaAddresses.Length);
        }

        protected override ILog Log { get; }
    }
}