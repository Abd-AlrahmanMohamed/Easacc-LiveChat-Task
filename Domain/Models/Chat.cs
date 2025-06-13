namespace Domain.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public string? SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public string? ReceivedId { get; set; }
        public ApplicationUser Receiver { get; set; }
        public ICollection<Message> Messages { get; set; }
        public DateTime LastActive { get; set; }
    }
}
