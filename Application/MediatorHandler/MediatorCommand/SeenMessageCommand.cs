using Domain.Models;
using MediatR;

namespace Application.MediatorHandler.MediatorCommend
{
    public record SeenMessageCommand(Guid MessageId) : IRequest;
}
