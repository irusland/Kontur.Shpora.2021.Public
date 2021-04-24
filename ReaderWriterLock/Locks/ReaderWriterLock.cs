using System.Threading;

namespace hw1.Locks
{
    public class ReaderWriterLock : IReaderWriterLock
    {
        /// <summary>
        /// Using two mutexes
        /// read-preferring
        /// </summary>
        private object r;
        private object g;

        private int b;

        public ReaderWriterLock()
        {
            r = new object();
            g = new object();
        }

        public void AcquireReaderLock(int timeOutMilliseconds)
        {
            lock (r)
            {
                b++;
                if (b == 1)
                {
                    Monitor.Enter(g);
                }
            }
        }

        public void ReleaseReaderLock()
        {
            lock (r)
            {
                b--;
                if (b == 0)
                {
                    Monitor.PulseAll(g);
                }
            }
        }

        public void AcquireWriterLock(int timeoutMilliseconds)
        {
            Monitor.Wait(g, timeoutMilliseconds);
        }

        public void ReleaseWriterLock()
        {
            Monitor.Exit(g);
        }
    }
}