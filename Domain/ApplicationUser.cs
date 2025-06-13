using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
        
        public ICollection<Chat> SentChat { get; set; }
        public ICollection<Chat> ReceivedChat { get; set; }

        //[NotMapped]
        //public IEnumerable<Chat> Chats => SentChat.Concat(ReceivedChat);
        


    }
}
