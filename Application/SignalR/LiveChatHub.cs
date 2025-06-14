using Application.MediatorHandler.MediatorCommend;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Domain.Models;
using Infrastructure.Interfaces;
using Application.MediatorHandler.MediatorCommandHandler;
using Application.MediatorHandler.MediatorQuery;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.SignalR
{
    public class LiveChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();
        private readonly IMediator _mediator;
        private readonly IUnityOfWork _unitOfWork;

        public LiveChatHub(IMediator mediator, IUnityOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        //public async Task SendMessage(SendMessageHandler command)
        //{
        //    try
        //    {
        //        await _mediator.Send(command);
        //        await Clients.All
        //                     .SendAsync("ReceiveMessage", command);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in SendMessage: {ex.Message}");
        //    }
        //}

        public async Task SendMessage(SendMessageCommand command)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(command.Content) && command.FileUrl != null)
                {
                    var fileUrl = await SaveFileAsync(command.FileUrl); // Save and get URL
                    command = command with { Content = fileUrl };       // Immutable update
                }
                await _mediator.Send(command);

                await Clients.User(command.ReceiverId.ToString())
                             .SendAsync("ReceiveMessage", new
                             {
                                 senderId = command.SenderId,
                                 content = !string.IsNullOrWhiteSpace(command.Content) ? command.Content : await SaveFileAsync(command.FileUrl),
                                 type = command.Type,
                                 fileUrl = command.FileUrl, // now it's just a string (URL)
                                 sentAt = DateTime.UtcNow
                             });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
            }
        }

        public async Task GetMessages(string senderId, string receiverId)
        {
            try
            {
                var messages = await _mediator.Send(new GetAllMessagesForSender(senderId, receiverId));

                await Clients.Caller.SendAsync("ReceiveMessagesHistory", messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMessages: {ex.Message}");
            }
        }


        /*
         var chatId = GenerateChatId(senderId, receiverId);

    var messages = _context.Messages
        .Where(m => m.ChatId == chatId)
        .OrderBy(m => m.Timestamp)
        .ToList();

    await Clients.Caller.SendAsync("ReceiveMessagesHistory", messages);
         *
         */

        public async Task Typing(string chatId, string userId)
        {
            await Clients.OthersInGroup(chatId).SendAsync("UserTyping", userId);
        }

        public async Task StopTyping(string chatId, string userId)
        {
            await Clients.OthersInGroup(chatId).SendAsync("UserStoppedTyping", userId);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                // Track user connection
                _userConnections.AddOrUpdate(userId,
                    _ => new HashSet<string> { Context.ConnectionId },
                    (_, existing) =>
                    {
                        existing.Add(Context.ConnectionId);
                        return existing;
                    });

                // Fetch chat IDs where the user is sender or receiver
                var messages = await _unitOfWork.Repository<Message>()
                    .GetAllAsync(m => m.SenderId == userId || m.ReceivedId == userId);

                var chatIds = messages.Select(m => m.ChatId).Distinct();

                foreach (var chatId in chatIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

                    // Notify others in the group the user is online
                    await Clients.OthersInGroup(chatId.ToString()).SendAsync("UserOnline", userId);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId) &&
                _userConnections.TryGetValue(userId, out var connections))
            {
                connections.Remove(Context.ConnectionId);

                if (connections.Count == 0)
                {
                    _userConnections.TryRemove(userId, out _);

                    // Fetch chat IDs where the user was active
                    var messages = await _unitOfWork.Repository<Message>()
                        .GetAllAsync(m => m.SenderId == userId || m.ReceivedId == userId);

                    var chatIds = messages.Select(m => m.ChatId).Distinct();

                    foreach (var chatId in chatIds)
                    {
                        await Clients.OthersInGroup(chatId.ToString()).SendAsync("UserOffline", userId);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SeenMessage(SendMessageCommand command)
        {
            await _mediator.Send(command);
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative or absolute URL as needed
            return $"/uploads/{uniqueFileName}";
        }

    }
}


