namespace ProjectIssueTracker.Repositories.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IProjectRepository Projects { get; }

        Task SaveChangesAsync();
    }
}
