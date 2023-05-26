using Back.Core.Models;

namespace Back.Core.Interfaces;

public interface IMessageService
{
    Task<Message?> FindById(Guid id);
    Task<IEnumerable<Message>> GetMessages();
    Task<Message?> CreateMessage(Message message);
    Task<Message> UpdateMessage(Message message);
    Task DeleteMessage(Message message);
}