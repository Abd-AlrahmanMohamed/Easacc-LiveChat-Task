using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AddChatConfigurations : IEntityTypeConfiguration<Chat>

    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder
                .HasMany(u => u.Messages)
                .WithOne(b => b.Chat)
                .HasForeignKey(b => b.ChatId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(b => b.Sender)
                .WithMany(u => u.SentChat)
                .HasForeignKey(b => b.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(b => b.Receiver)
                .WithMany(u => u.ReceivedChat)
                .HasForeignKey(b => b.ReceivedId)
                .OnDelete(DeleteBehavior.NoAction);
            builder
            .HasIndex(c => new { c.SenderId, c.ReceivedId })
            .IsUnique();
        }
    }
}
