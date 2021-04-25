using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
// using log4net;

namespace NMAP
{
    public class AsyncScanner : IPScanner
    {
        // protected virtual ILog log => LogManager.GetLogger(typeof(SequentialScanner));

        public virtual Task Scan(IPAddress[] ipAddrs, int[] ports)
        {
            return Task.WhenAll(ipAddrs.Select(ipAddr =>  ProcessPair(ipAddr, ports)));
        }

        private async Task ProcessPair(IPAddress ip, int[] ports)
        {
            var ping = await PingAddr(ip);
            if (ping != IPStatus.Success)
                return;
            await Task.WhenAll(ports.Select(port => CheckPort(ip, port)));
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

        protected async Task CheckPort(IPAddress ipAddr, int port, int timeout = 3000)
        {
            using (var tcpClient = new TcpClient())
            {
                Console.WriteLine($"Checking {ipAddr}:{port}");

                var connectTask = await tcpClient.ConnectWithTimeoutAsync(ipAddr, port, timeout);
                // await connectTask;
                PortStatus portStatus;
                switch (connectTask.Status)
                {
                    case TaskStatus.RanToCompletion:
                        portStatus = PortStatus.OPEN;
                        break;
                    case TaskStatus.Faulted:
                        portStatus = PortStatus.CLOSED;
                        break;
                    default:
                        portStatus = PortStatus.FILTERED;
                        break;
                }

                Console.WriteLine($"Checked {ipAddr}:{port} - {portStatus}");
            }
        }
    }
}