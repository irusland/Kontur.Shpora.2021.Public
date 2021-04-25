using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
// using log4net;

namespace NMAP
{
    public class AsyncScanner
    {
        // protected virtual ILog log => LogManager.GetLogger(typeof(SequentialScanner));

        public  async Task<List<(IPAddress, int)>> Scan(IPAddress[] ipAddrs, int[] ports)
        {
            var l = new List<(IPAddress, int)>();
            
            foreach (var iEnumerable in ipAddrs.Select(ipAddr => ProcessPair(ipAddr, ports)))
            {
                await foreach (var valueTuple in iEnumerable)
                {
                    l.Add(valueTuple);
                }
            }

            return l;
        }

        private async IAsyncEnumerable<(IPAddress, int)> ProcessPair(IPAddress ip, int[] ports)
        {
            // var ping = await PingAddr(ip);
            // if (ping != IPStatus.Success)
            //     yield break;
            foreach (var port in ports)
            {
                var isAvailable = await CheckPort(ip, port);
                if (isAvailable)
                    yield return (ip, port);
            }
        }


        protected async Task<IPStatus> PingAddr(IPAddress ipAddr, int timeout = 3000)
        {
            Console.WriteLine($"Pinging {ipAddr}");
            using (var ping = new Ping())
            {
                var status = (await ping.SendPingAsync(ipAddr, timeout)).Status;
                Console.WriteLine($"Pinged {ipAddr}: {status}");
                return status;
            }
        }

        protected async Task<bool> CheckPort(IPAddress ipAddr, int port, int timeout = 20)
        {
            using (var tcpClient = new TcpClient())
            {
                // Console.WriteLine($"Checking {ipAddr}:{port}");

                var connectTask = await tcpClient.ConnectWithTimeoutAsync(ipAddr, port, timeout);
                // await connectTask;
                PortStatus portStatus;
                switch (connectTask.Status)
                {
                    case TaskStatus.RanToCompletion:
                        portStatus = PortStatus.OPEN;
                        return true;
                    case TaskStatus.Faulted:
                        portStatus = PortStatus.CLOSED;
                        break;
                    default:
                        portStatus = PortStatus.FILTERED;
                        break;
                }

                // Console.WriteLine($"Checked {ipAddr}:{port} - {portStatus}");
                return false;

            }
        }
    }
}