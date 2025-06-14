using Application.MediatorHandler.MediatorCommend;
using Domain.Models;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.MediatorHandler.MediatorCommandHandler
{
    public class SendMessageHandler : IRequestHandler<SendMessageCommand>
    {
        private readonly IUnityOfWork _unitOfWork;

        public SendMessageHandler(IUnityOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            string fileUrl = null;

            if (request.Type == MessageType.Image ||
                request.Type == MessageType.Document ||
                request.Type == MessageType.Audio)
            {
                if (request.FileUrl != null && request.FileUrl.Length > 0)
                {
                    var typeFolder = request.Type.ToString().ToLower();
                    var uploadsFolder = Path.Combine("wwwroot", "uploads", typeFolder);

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.FileUrl.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.FileUrl.CopyToAsync(stream);
                    }

                    fileUrl = $"/uploads/{typeFolder}/{uniqueFileName}";
                }
                else
                {
                    fileUrl = null;
                }
            }
            


            //var chat = await _unitOfWork.Repository<Chat>().FindAsync(c =>
            //    c.Messages.Any(m => m.Content == request.Content && m.Type == request.Type));
            //string user1 = request.SenderId.CompareTo(request.ReceiverId) < 0 ? request.SenderId : request.ReceiverId;
            //string user2 = request.SenderId.CompareTo(request.ReceiverId) < 0 ? request.ReceiverId : request.SenderId;
            //string user1 = request.SenderId.CompareTo(request.ReceiverId) < 0 ? request.SenderId : request.ReceiverId;
            //string user2 = request.SenderId.CompareTo(request.ReceiverId) < 0 ? request.ReceiverId : request.SenderId;

            //var chat = await _unitOfWork.Repository<Chat>().FindAsync(c =>
            //    (c.SenderId == request.SenderId && c.ReceivedId == request.ReceiverId));

            var senderExists = await _unitOfWork.Repository<Chat>()
                 .FindAsync(u => u.SenderId == request.SenderId);

            var receiverExists = await _unitOfWork.Repository<Chat>()
                .FindAsync(u => u.ReceivedId == request.ReceiverId);

            //if (senderExists && receiverExists) { 
            //}

                var chat = await _unitOfWork.Repository<Chat>()
                .FindWithIncludeAsync(c =>
                c.Messages.Any(m =>
                (m.SenderId == request.SenderId && m.ReceivedId == request.ReceiverId) ||
                (m.SenderId == request.ReceiverId && m.ReceivedId == request.SenderId)));


            if (chat.SenderId != request.SenderId && chat.ReceivedId != request.ReceiverId)
            {
                chat = new Chat
                {
                    Id = Guid.NewGuid(),
                    SenderId = request.SenderId,
                    ReceivedId = request.ReceiverId,
                    Messages = new List<Message>(),
                    LastActive = DateTime.UtcNow
                };

                await _unitOfWork.Repository<Chat>().AddAsync(chat);
            }


            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = chat.Id,
                SenderId = request.SenderId,
                ReceivedId = request.ReceiverId,
                Content = request.Content,
                FileUrl = fileUrl,
                Type = request.Type,
                SentAt = DateTime.UtcNow,
                Seen = false
            };
            await _unitOfWork.Repository<Message>().AddAsync(message);


            //if (chat.Messages == null)
            //    {
            //    chat.Messages = new List<Message>();
            //    chat.Messages.Add(message);

            //    }
                chat.LastActive = chat.Messages
                .OrderByDescending(m => m.SentAt)
                .Select(m => m.SentAt)
                .FirstOrDefault();
            await _unitOfWork.Complete();


            return Unit.Value;
        }

        Task IRequestHandler<SendMessageCommand>.Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }
    }

}
