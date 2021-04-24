namespace hw1.Locks
{
    public interface IReaderLock : ILock
    {
        void AcquireReaderLock(int timeOutMilliseconds);
        void ReleaseReaderLock();
    }
}