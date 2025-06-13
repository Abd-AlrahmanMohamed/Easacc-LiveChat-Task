using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
namespace Application.MediatorHandler.MediatorCommend
{
    public record SendMessageCommand(string SenderId, string ReceiverId, string? ChatId, string Content, MessageType Type, IFormFile? FileUrl) : IRequest;
    
}
