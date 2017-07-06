namespace TeamService.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        ITeamRepository Teams { get; }
    }
}