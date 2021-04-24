using System.Threading;

namespace hw1.Locks
{
    public class WriterReaderLock : IReaderWriterLock
    {
        /// <summary>
        /// Using a condition variable and a mutex
        /// write-preferring
        /// </summary>
        
        private object g;
        
        private int activeReaders;

        private int waitingWriters;
        private bool isWriterActive;

        public WriterReaderLock()
        {
            g = new object();
        }

        public void AcquireReaderLock(int timeOutMilliseconds)
        {
            Monitor.Enter(g);
            try
            {
                while (waitingWriters > 0 || isWriterActive)
                {
                    Monitor.Wait(g, timeOutMilliseconds);
                }

                activeReaders++;
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
                activeReaders--;
                if (activeReaders == 0)
                {
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
                waitingWriters++;
                while (activeReaders > 0 || isWriterActive)
                {
                    Monitor.Wait(g, timeoutMilliseconds);
                }
                waitingWriters--;
                isWriterActive = true;
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
                isWriterActive = false;
                Monitor.PulseAll(g);
            }
            finally
            {
                Monitor.Exit(g);
            }
        }
    }
}