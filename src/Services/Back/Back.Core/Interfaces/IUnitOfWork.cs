namespace Back.Core.Interfaces;

public interface IUnitOfWork
{
    IMessageRepository Message { get; }
    Task SaveChangesAsync();
}