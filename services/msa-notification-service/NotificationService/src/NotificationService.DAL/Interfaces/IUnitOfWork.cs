namespace NotificationService.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IWatcherRepository Watchers { get; }
    }
}