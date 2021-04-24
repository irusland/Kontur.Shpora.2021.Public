namespace hw1
{
    public interface IReaderLock : ILock
    {
        void AcquireReaderLock(int timeOutMilliseconds);
        void ReleaseReaderLock();
    }
}