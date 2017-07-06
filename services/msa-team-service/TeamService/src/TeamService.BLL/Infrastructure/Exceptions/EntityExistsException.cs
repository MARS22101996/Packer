namespace TeamService.BLL.Infrastructure.Exceptions
{
    public class EntityExistsException : ServiceException
    {
        public EntityExistsException(string message, string entity) : base(message, entity)
        {
        }
    }
}
