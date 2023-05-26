using Back.Core.Models;
using FluentResults;

namespace Back.Core.Interfaces;

public interface IMessageService
{
    Task<Message?> FindById(Guid id);
    Task<IEnumerable<Message>> GetMessages();
    Task<Result<Message>> CreateMessage(Message message);
    Task<Result<Message>> UpdateMessage(Message message);
    Task DeleteMessage(Message message);
}