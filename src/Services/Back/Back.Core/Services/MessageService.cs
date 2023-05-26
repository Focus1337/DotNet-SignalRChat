using Back.Core.Interfaces;
using Back.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Back.Core.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Message?> FindById(Guid id) =>
        await _unitOfWork.Message.GetAll().FirstOrDefaultAsync(m => m.Id == id);

    public async Task<IEnumerable<Message>> GetMessages() =>
        await _unitOfWork.Message.GetAll().ToListAsync();

    public async Task<Message?> CreateMessage(Message message)
    {
        await _unitOfWork.Message.CreateAsync(message);
        await _unitOfWork.SaveChangesAsync();
        return await FindById(message.Id);
    }

    public async Task<Message> UpdateMessage(Message message)
    {
        await _unitOfWork.Message.UpdateAsync(message);
        await _unitOfWork.SaveChangesAsync();
        return (await FindById(message.Id))!;
    }

    public async Task DeleteMessage(Message message)
    {
        await _unitOfWork.Message.DeleteAsync(message);
        await _unitOfWork.SaveChangesAsync();
    }
}