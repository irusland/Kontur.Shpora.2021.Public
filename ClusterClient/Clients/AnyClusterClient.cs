using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace ClusterClient.Clients
{
    public class AnyClusterClient : ClusterClientBase
    {
        public AnyClusterClient(string[] replicaAddresses) : base(replicaAddresses)
        {
        }

        public override async Task<string> ProcessRequestAsync(string query, TimeSpan timeout)
        {
            var tasks = ReplicaAddresses.Select(async (uri) =>
            {
                var webRequest = CreateRequest(uri + "?query=" + query);
                Console.WriteLine($"Processing {webRequest.RequestUri}");
                return await ProcessRequestAsync(webRequest);
            });
            var timeoutTask = Task.Delay(timeout);

            tasks.Append(timeoutTask);
            
            var resultTask = await Task.WhenAny(tasks);
            if (!resultTask.IsCompleted)
                throw new TimeoutException();

            return resultTask.Result;
        }

        protected override ILog Log { get; }
    }
}