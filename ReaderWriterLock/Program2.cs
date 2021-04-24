using System;
using System.Threading;
using hw1.Locks;

namespace hw1
{
    public class Program
    {
        static IReaderWriterLock rwl;

        static int resource = 0;

        const int numThreads = 26;
        static bool running = true;

        static int readerTimeouts = 0;
        static int writerTimeouts = 0;
        static int reads = 0;
        static int writes = 0;

        public static void Main2()
        {
            rwl = new Locks.WriterReaderLock();
            // rwl = new Locks.ReaderWriterLock();
            
            var t = new Thread[numThreads];
            for (var i = 0; i < numThreads; i++)
            {
                t[i] = new Thread(new ThreadStart(ThreadProc))
                {
                    Name = new string(Convert.ToChar(i + 65), 1)
                };
                t[i].Start();
                if (i > 10)
                    Thread.Sleep(300);
            }

            running = false;
            for (var i = 0; i < numThreads; i++)
                t[i].Join();

            Console.WriteLine("\n{0} reads, {1} writes, {2} reader time-outs, {3} writer time-outs.",
                reads, writes, readerTimeouts, writerTimeouts);
            Console.Write("Press ENTER to exit... ");
            Console.ReadLine();
        }

        static void ThreadProc()
        {
            var rnd = new Random();

            while (running)
            {
                var action = rnd.NextDouble();
                if (action < .1)
                    ReadFromResource(10);
                else
                    WriteToResource(rnd, 100);
            }
        }

        static void ReadFromResource(int timeOut)
        {
            try
            {
                rwl.AcquireReaderLock(timeOut);
                try
                {
                    Display("reads resource value " + resource);
                    Interlocked.Increment(ref reads);
                }
                finally
                {
                    rwl.ReleaseReaderLock();
                }
            }
            catch (ThreadInterruptedException)
            {
                Interlocked.Increment(ref readerTimeouts);
            }
        }

        static void WriteToResource(Random rnd, int timeOut)
        {
            try
            {
                rwl.AcquireWriterLock(timeOut);
                try
                {
                    resource = rnd.Next(500);
                    Display("writes resource value " + resource);
                    Interlocked.Increment(ref writes);
                }
                finally
                {
                    rwl.ReleaseWriterLock();
                }
            }
            catch (ApplicationException)
            {
                Interlocked.Increment(ref writerTimeouts);
            }
        }

        static void Display(string msg)
        {
            Console.Write("Thread {0} {1}.       \r", Thread.CurrentThread.Name, msg);
        }
    }
}