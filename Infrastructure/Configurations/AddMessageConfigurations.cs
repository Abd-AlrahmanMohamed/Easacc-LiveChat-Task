using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Infrastructure.Configurations
{
    public class AddMessageConfigurations : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder
        .Property(b => b.SentAt)
        .HasDefaultValueSql("GETUTCDATE()");

            // Sender
            builder
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            // Receiver
            builder
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceivedId)
                .OnDelete(DeleteBehavior.NoAction);


            builder
                .HasOne(b => b.Chat) 
                .WithMany(e => e.Messages) 
                .HasForeignKey(b => b.ChatId) 
                .OnDelete(DeleteBehavior.NoAction);  

        }
    }
}
