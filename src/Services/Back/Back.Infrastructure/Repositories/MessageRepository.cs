using Back.Core.Interfaces;
using Back.Core.Models;
using Back.Infrastructure.Data;

namespace Back.Infrastructure.Repositories;

internal class MessageRepository : EfRepository<Message, AppDbContext>, IMessageRepository
{
    public MessageRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}