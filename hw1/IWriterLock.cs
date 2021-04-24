namespace hw1
{
    public interface IWriterLock : ILock
    {
        void AcquireWriterLock(int timeoutMilliseconds);
        void ReleaseWriterLock();
    }
}