using Application.MediatorHandler.MediatorQuery;
using Domain.Models;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.MediatorHandler.MediatorQueryHandler
{

    public class GetAllMessagesForSenderHandler : IRequestHandler<GetAllMessagesForSender, List<Message>>
    {
        private readonly IUnityOfWork _unitOfWork;

        public GetAllMessagesForSenderHandler(IUnityOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Message>> Handle(GetAllMessagesForSender request, CancellationToken cancellationToken)
        {
            var allMessages = await _unitOfWork.Repository<Message>().GetAllDataAsync();

            var messages = allMessages
              .Where(m =>
                (m.SenderId == request.SenderId && m.ReceivedId == request.ReceiverId) ||
                (m.SenderId == request.ReceiverId && m.ReceivedId == request.SenderId))
                .OrderByDescending(m => m.SentAt)
                .ToList();

            return messages;
        }
    }
}


