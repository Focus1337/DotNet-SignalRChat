namespace Back.Application.Interfaces;

public interface IUserService<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetUsers();
    Task<TEntity?> FindByEmail(string email);
    Task<TEntity?> FindById(Guid id);
    Task<TEntity?> GetCurrentUser();
    Task<bool> ValidateCredentials(string email, string? password = null, bool checkPassword = false);
    Task<TEntity> UpdateUser(TEntity entity);
}