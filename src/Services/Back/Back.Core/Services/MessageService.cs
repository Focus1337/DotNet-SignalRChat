using Back.Core.Interfaces;
using Back.Core.Models;
using FluentResults;
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
        await _unitOfWork.Message.GetAll().Include(m => m.User).FirstOrDefaultAsync(m => m.Id == id);

    public async Task<IEnumerable<Message>> GetMessages() =>
        await _unitOfWork.Message.GetAll().Include(m => m.User).ToListAsync();

    public async Task<Result<Message>> CreateMessage(Message message)
    {
        await _unitOfWork.Message.CreateAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return await FindById(message.Id) is not { } res
            ? Result.Fail<Message>(new Error("Failed to create message"))
            : Result.Ok(res);
    }

    public async Task<Result<Message>> UpdateMessage(Message message)
    {
        await _unitOfWork.Message.UpdateAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return await FindById(message.Id) is not { } res
            ? Result.Fail<Message>(new Error("Failed to update message"))
            : Result.Ok(res);
    }

    public async Task DeleteMessage(Message message)
    {
        await _unitOfWork.Message.DeleteAsync(message);
        await _unitOfWork.SaveChangesAsync();
    }
}