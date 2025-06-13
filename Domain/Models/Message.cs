using Microsoft.AspNetCore.Http;

namespace Domain.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string? SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public string? ReceivedId { get; set; }
        public ApplicationUser Receiver { get; set; }
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }
        public string? Content { get; set; } = string.Empty;
        public string? FileUrl { get; set; }= string.Empty;
        public MessageType Type { get; set; }
        public DateTime SentAt { get; set; }
        public bool Seen { get; set; }
    }

    public enum MessageType
    {
        Text, Image, Document, Audio
    }
}
