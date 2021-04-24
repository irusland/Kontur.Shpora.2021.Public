using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace ThreadPool
{
	[TestFixture]
	public class Program
	{
		[Test]
		public void TestOneAction()
		{
			using var threadPool = CreateThreadPool(Concurrency);
			var countdown = new CountdownEvent(1);
			threadPool.EnqueueAction(() => Assert.IsTrue(countdown.Signal()));
			countdown.Wait();
		}

		[TestCase(Concurrency * 10)]
		public void TestSequentially(int iterations)
		{
			using var threadPool = CreateThreadPool(Concurrency);
			Enumerable.Range(0, iterations).ForEach(_ => TestOneAction());
		}

		[TestCase(1000)]
		public void TestDispose(int iterations)
		{
			Enumerable.Range(0, iterations).AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount).ForAll(_ => TestOneAction());
		}

		[TestCase(100000)]
		public void TestConcurrency(int multiplier)
		{
			var count = 0;
			using var threadPool = CreateThreadPool(Concurrency);
			var countdowns = Enumerable.Range(0, multiplier).Select(_ => new CountdownEvent(Concurrency)).ToArray();
			Enumerable.Range(0, Concurrency * multiplier).AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount * 4).ForAll(i => threadPool.EnqueueAction(() =>
			{
				if(Interlocked.Increment(ref count) > Concurrency)
					throw new Exception("Concurrency level exceeded");
				countdowns[i % countdowns.Length].Signal();
				Interlocked.Decrement(ref count);
			}));
			countdowns.ForEach(c => c.Wait());
		}

		[TestCase(1000), Explicit("Next level")]
		public void TestNoSleepInDispatchLoop(int iterations)
		{
			using var threadPool = CreateThreadPool(4);
			var stopwatch = Stopwatch.StartNew();
			Enumerable.Range(0, iterations).ForEach(_ => TestOneAction());
			stopwatch.Stop();
			Console.WriteLine(stopwatch.Elapsed);
			Assert.Less(stopwatch.ElapsedMilliseconds, iterations);
		}

		[Test, Explicit("Next level")]
		public void TestNoSpinInDispatchLoop()
		{
			using var threadPool = CreateThreadPool(Environment.ProcessorCount * 100);
			var process = Process.GetCurrentProcess();

			var sw = Stopwatch.StartNew();
			var start = process.TotalProcessorTime;
			Thread.Sleep(3000);
			var end = process.TotalProcessorTime;
			sw.Stop();

			var cpuUsage = (end - start).TotalMilliseconds / (Environment.ProcessorCount * sw.ElapsedMilliseconds);
			Console.WriteLine("CPU Usage with idle ThreadPool: " + cpuUsage.ToString("P"));

			Assert.Less(cpuUsage, 0.5);
		}

		private static IThreadPool CreateThreadPool(int concurrency)
			=> new SimpleLockThreadPool(concurrency);

		private const int Concurrency = 4;
	}

	internal class SimpleLockThreadPool : IThreadPool
	{
		EventWaitHandle _wh = new AutoResetEvent (false);
		Thread[] _worker;
		readonly object _locker = new object();
		Queue<Action> _tasks = new Queue<Action>();
		private bool isFinished = false;

		public SimpleLockThreadPool(in int concurrency)
		{
			_worker = new Thread[concurrency];
			for (int i = 0; i < concurrency; i++)
			{
				_worker[i] = new Thread(Work);
				_worker[i].Start();

			}
		}
 
		public void EnqueueAction(Action action)
		{
			lock (_locker) 
				_tasks.Enqueue(action);
			_wh.Set();
		}
 
		public void Dispose()
		{
			EnqueueAction(null);
			for (int i = 0; i < _worker.Length; i++)
			{
				_worker[i].Join();
			}
			_wh.Close();
		}
		
		void Work()
		{
			while (true)
			{
				Action task = null;
				lock (_locker)
					if (_tasks.Count > 0)
					{
						task = _tasks.Dequeue();
						if (isFinished || task == null)
						{
							isFinished = true;
							EnqueueAction(null);
							return;
						}
					}
				if (task != null)
				{
					Console.WriteLine ("Performing task: " + task);
					task.Invoke();
				}
				else
					_wh.WaitOne();         // No more tasks - wait for a signal
			}
		}
	}

	public static class Extension
	{
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach(var item in enumerable)
				action(item);
		}
	}
}
