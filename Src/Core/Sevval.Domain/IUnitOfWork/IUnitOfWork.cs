namespace GridBox.Solar.Domain.IUnitOfWork
{
    public interface IUnitOfWork
    {
        void ClearTracking();
        int Commit();
        Task<int> CommitAsync(CancellationToken cancellationToken);
    }
}
