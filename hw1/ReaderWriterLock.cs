using System;
using System.Diagnostics;
using System.Threading;

namespace hw1
{
    public class ReaderWriterLock : IReaderLock, IWriterLock
    {
        /// <summary>
        /// Using a condition variable and a mutex
        /// write-preferring RW
        /// </summary>
        
        private object g;

        private bool cond;
        
        private int num_readers_active;

        private int num_writers_waiting;
        private bool writer_active;

        public ReaderWriterLock()
        {
            g = new object();
        }

        private void wait(int timeout)
        {
            Monitor.Exit(g);
            var stopwatch = Stopwatch.StartNew();
            while (!cond) { 
                if (stopwatch.ElapsedMilliseconds > timeout)
                    throw new ApplicationException("timeout");
            }

            cond = false;
        }

        private void notify()
        {
            Monitor.PulseAll(g);

            cond = true;
        }

        public void AcquireReaderLock(int timeOutMilliseconds)
        {
            Monitor.Enter(g);
            try
            {
                while (num_writers_waiting > 0 || writer_active)
                {
                    // wait(timeOutMilliseconds);
                    Monitor.Wait(g);
                }

                num_readers_active++;
            }
            finally
            {
                Monitor.Exit(g);
            }
        }

        public void ReleaseReaderLock()
        {
            Monitor.Enter(g);
            try
            {
                num_readers_active--;
                if (num_readers_active == 0)
                {
                    // notify();
                    Monitor.PulseAll(g);
                }

            }
            finally
            {
                Monitor.Exit(g);
            }
        }

        public void AcquireWriterLock(int timeoutMilliseconds)
        {
            Monitor.Enter(g);
            try
            {
                num_writers_waiting++;
                while (num_readers_active > 0 || writer_active)
                {
                    // wait(timeoutMilliseconds);
                    Monitor.Wait(g);
                }
                num_writers_waiting--;
                writer_active = true;
            }
            finally
            {
                Monitor.Exit(g);
            }
        }

        public void ReleaseWriterLock()
        {
            Monitor.Enter(g);
            try
            {
                writer_active = false;
                // notify();
                Monitor.PulseAll(g);
            }
            finally
            {
                Monitor.Exit(g);
            }
        }
    }
}