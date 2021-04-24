namespace hw1.Locks
{
    public interface IWriterLock : ILock
    {
        void AcquireWriterLock(int timeoutMilliseconds);
        void ReleaseWriterLock();
    }
}