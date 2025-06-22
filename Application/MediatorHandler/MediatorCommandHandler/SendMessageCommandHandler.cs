using Application.Extensions;
using Application.MediatorHandler.MediatorCommend;
using Domain.Models;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Hosting;

using Microsoft.EntityFrameworkCore;

namespace Application.MediatorHandler.MediatorCommandHandler
{
    public class SendMessageHandler : IRequestHandler<SendMessageCommand>
    {
        private readonly IUnityOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public SendMessageHandler(IUnityOfWork unitOfWork, IWebHostEnvironment environment)
    {
      _unitOfWork = unitOfWork;
      _environment = environment;
    }


    public async Task<Unit> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            string fileUrl = null;

            if (request.Type == MessageType.Image ||
                request.Type == MessageType.Document ||
                request.Type == MessageType.Audio)
            {
               if (request.FileUrl != null)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.FileUrl.FileName);
            string fullPath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await request.FileUrl.CopyToAsync(fileStream, cancellationToken);
            }

            fileUrl = $"/uploads/{uniqueFileName}"; // relative path to be served
        }
                else
                {
                    fileUrl = null;
                }
            }
            


           

            var senderExists = await _unitOfWork.Repository<Chat>()
                 .FindAsync(u => u.SenderId == request.SenderId);

            var receiverExists = await _unitOfWork.Repository<Chat>()
                .FindAsync(u => u.ReceivedId == request.ReceiverId);

            

                var chat = await _unitOfWork.Repository<Chat>()
                .FindWithIncludeAsync(c =>
                c.Messages.Any(m =>
                (m.SenderId == request.SenderId && m.ReceivedId == request.ReceiverId) ||
                (m.SenderId == request.ReceiverId && m.ReceivedId == request.SenderId)));


            if (chat == null || chat.SenderId != request.SenderId && chat.ReceivedId != request.ReceiverId)
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
      if (chat.Messages != null && chat.Messages.Any())
      {
        chat.LastActive = chat.Messages
            .OrderByDescending(m => m.SentAt)
            .Select(m => m.SentAt)
            .FirstOrDefault();
      }
      else
      {
        chat.LastActive = DateTime.UtcNow;
      }

      await _unitOfWork.Complete();


            return Unit.Value;
        }

        Task IRequestHandler<SendMessageCommand>.Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }
    }

}
