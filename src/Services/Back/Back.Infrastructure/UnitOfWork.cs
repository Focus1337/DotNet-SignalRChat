using Back.Core.Interfaces;
using Back.Infrastructure.Data;
using Back.Infrastructure.Repositories;

namespace Back.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private IMessageRepository? _messageRepository;
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext) =>
        _dbContext = dbContext;

    public IMessageRepository Message => _messageRepository ??= new MessageRepository(_dbContext);

    public async Task SaveChangesAsync() =>
        await _dbContext.SaveChangesAsync();
}