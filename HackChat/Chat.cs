using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NMAP;

namespace HackChat
{
	public class Chat
	{
		public const int DefaultPort = 31337;

		private readonly byte[] PingMsg = new byte[1];
		private readonly ConcurrentDictionary<IPEndPoint, (TcpClient Client, NetworkStream Stream)> Connections = new ConcurrentDictionary<IPEndPoint, (TcpClient Client, NetworkStream Stream)>();

		private readonly int port;
		private readonly TcpListener tcpListener;

		public Chat(int port) => tcpListener = new TcpListener(IPAddress.Any, this.port = port);
		public void Start()
		{
			Task.Run(DiscoverLoop);
			Task.Run(() =>
			{
				string line;
				while((line = Console.ReadLine()) != null)
					Task.Run(() => BroadcastAsync(line));
			});
			Task.Run(() =>
			{
				tcpListener.Start(100500);
				while(true)
				{
					var tcpClient = tcpListener.AcceptTcpClient();
					Task.Run(() => ProcessClientAsync(tcpClient));
				}
			});
		}

		private async Task BroadcastAsync(string message)
		{
			foreach (var (ip, port) in discovered)
			{
				using (var tcpClient = new TcpClient())
				{
					tcpClient.Connect(ip, port);
					var buffer = Encoding.UTF8.GetBytes(message);
					using (var s = tcpClient.GetStream())
					{
						s.Write(buffer, 0, buffer.Length);
						Console.WriteLine($"{message} {discovered.Count} {ip} {port}");
					}
				}
			}
		}

		private async void DiscoverLoop()
		{
			while(true)
			{
				try { await Discover(); } catch { /* ignored */ }
				await Task.Delay(3000);
			}
		}

		private List<(IPAddress, int)> discovered;
		private async Task Discover()
		{
			var ipAddrs = NMAP.Program.GenIpAddrs();
			var ports = new[] {31337};
			var port = 31337;
			// var scanner = new SequentialScanner();
			var scanner = new AsyncScanner();
			discovered = await scanner.Scan(ipAddrs, ports);
		}

		private static async Task ProcessClientAsync(TcpClient tcpClient)
		{
			EndPoint endpoint = null;
			try { endpoint = tcpClient.Client.RemoteEndPoint; } catch { /* ignored */ }
			await Console.Out.WriteLineAsync($"[{endpoint}] connected");
			try
			{
				using(tcpClient)
				{
					var stream = tcpClient.GetStream();
					await ReadLinesToConsoleAsync(stream);
				}
			}
			catch { /* ignored */ }
			await Console.Out.WriteLineAsync($"[{endpoint}] disconnected");
		}

		private static async Task ReadLinesToConsoleAsync(Stream stream)
		{
			string line;
			using var sr = new StreamReader(stream);
			while((line = await sr.ReadLineAsync()) != null)
				await Console.Out.WriteLineAsync($"[{((NetworkStream)stream).Socket.RemoteEndPoint}] {line}");
		}
	}
}