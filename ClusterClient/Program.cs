using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ClusterClient.Clients;
using Fclp;
using log4net;
using log4net.Config;

namespace ClusterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            

            if (!TryGetReplicaAddresses(args, out var replicaAddresses))
                return;

            try
            {
                var clients = new ClusterClientBase[]
                {
                    // new RandomClusterClient(replicaAddresses),
                    // new AnyClusterClient(replicaAddresses),
                    // new RoundRobin(replicaAddresses), 
                    new SmartClusterClient(replicaAddresses), 
                };

                var queries = new[]
                {
                    "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                    "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                    "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"
                };

                foreach (var client in clients)
                {
                    Console.WriteLine("Testing {0} started", client.GetType());
                    Task.WaitAll(queries.Select(
                        async query =>
                        {
                            var timer = Stopwatch.StartNew();
                            try
                            {
                                // await client.ProcessRequestAsync(query, TimeSpan.FromSeconds(1));
                                await client.ProcessRequestAsync(query, TimeSpan.FromMilliseconds(10));

                                Console.WriteLine("Processed query \"{0}\" in {1} ms", query,
                                    timer.ElapsedMilliseconds);
                            }
                            catch (TimeoutException)
                            {
                                Console.WriteLine("Query \"{0}\" timeout ({1} ms)", query, timer.ElapsedMilliseconds);
                            }
                        }).ToArray());
                    Console.WriteLine("Testing {0} finished", client.GetType());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"E :{e}");
            }
        }

        private static bool TryGetReplicaAddresses(string[] args, out string[] replicaAddresses)
        {
            var argumentsParser = new FluentCommandLineParser();
            string[] result = { };

            argumentsParser.Setup<string>(CaseType.CaseInsensitive, "f", "file")
                .WithDescription("Path to the file with replica addresses")
                .Callback(fileName => result = File.ReadAllLines(fileName))
                .Required();

            argumentsParser.SetupHelp("?", "h", "help")
                .Callback(text => Console.WriteLine(text));

            var parsingResult = argumentsParser.Parse(args);

            if (parsingResult.HasErrors)
            {
                argumentsParser.HelpOption.ShowHelp(argumentsParser.Options);
                replicaAddresses = null;
                return false;
            }

            replicaAddresses = result;
            return !parsingResult.HasErrors;
        }

    }
}