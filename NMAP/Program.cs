﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
// using log4net;
// using log4net.Config;

namespace NMAP
{
    public class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()), new FileInfo("log4net.config"));

            var ipAddrs = GenIpAddrs();
            var ports = new[] {21, 25, 80, 443, 3389};

            // var scanner = new SequentialScanner();
            var scanner = new AsyncScanner();
            scanner.Scan(ipAddrs, ports).Wait();
        }

        public static IPAddress[] GenIpAddrs()
        {
            var konturAddrs = new List<IPAddress>();
            uint focusIpInt = 0x0071a8c0;
            for(int b = 0; b <= byte.MaxValue; b++)
                konturAddrs.Add(new IPAddress((focusIpInt & 0x00FFFFFF) | (uint)b << 24));
            return konturAddrs.ToArray();
        }
    }
}