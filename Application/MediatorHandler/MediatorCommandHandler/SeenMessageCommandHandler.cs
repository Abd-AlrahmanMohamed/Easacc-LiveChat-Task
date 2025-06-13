using Application.MediatorHandler.MediatorCommend;
using Domain.Models;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.MediatorHandler.MediatorCommandHandler
{
    public class SeenMessageHandler : IRequestHandler<SeenMessageCommand>
    {
        private readonly IUnityOfWork _unitOfWork;

        public SeenMessageHandler(IUnityOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(SeenMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _unitOfWork.Repository<Message>().GetByIdAsync(request.MessageId);
            if (message != null)
            {
                message.Seen = true;
                await _unitOfWork.Complete();
            }
            return Unit.Value;
        }

        Task IRequestHandler<SeenMessageCommand>.Handle(SeenMessageCommand request, CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }

        //Task IRequestHandler<SeenMessageCommand>.Handle(SeenMessageCommand request, CancellationToken cancellationToken)
        //{
        //    return Handle(request, cancellationToken);
        //}
    }
}
