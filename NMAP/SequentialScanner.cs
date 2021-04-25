using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
// using log4net;

namespace NMAP
{
    public class SequentialScanner : IPScanner
    {
        // protected virtual ILog log => LogManager.GetLogger(typeof(SequentialScanner));

        public virtual Task Scan(IPAddress[] ipAddrs, int[] ports)
        {
            return Task.Run(() =>
            {
                Parallel.ForEach(ipAddrs, (ipAddr) =>
                {
                    if (PingAddr(ipAddr) != IPStatus.Success)
                        return;

                    Parallel.ForEach(ports , port =>
                    {
                        CheckPort(ipAddr, port);
                    });
                });
            });
        }

        protected IPStatus PingAddr(IPAddress ipAddr, int timeout = 3000)
        {
            Console.WriteLine($"Pinging {ipAddr}");
            using (var ping = new Ping())
            {
                var status = ping.Send(ipAddr, timeout).Status;
                Console.WriteLine($"Pinged {ipAddr}: {status}");
                return status;
            }
        }

        protected void CheckPort(IPAddress ipAddr, int port, int timeout = 3000)
        {
            using (var tcpClient = new TcpClient())
            {
                Console.WriteLine($"Checking {ipAddr}:{port}");

                var connectTask = tcpClient.ConnectWithTimeout(ipAddr, port, timeout);
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