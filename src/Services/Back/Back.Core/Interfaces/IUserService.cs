namespace Back.Core.Interfaces;

public interface IUserService<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetUsers();
    Task<TEntity?> GetCurrentUser();
}