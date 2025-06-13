using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public class AddUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder
                .HasMany(u => u.SentMessages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder
                .HasMany(u => u.ReceivedMessages)
                .WithOne(m => m.Receiver)
                .HasForeignKey(m => m.ReceivedId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
               .HasMany(u => u.SentChat)
               .WithOne(b => b.Sender)
               .HasForeignKey(b => b.SenderId)
               .OnDelete(DeleteBehavior.NoAction);
            builder
               .HasMany(u => u.ReceivedChat)
               .WithOne(b => b.Receiver)
               .HasForeignKey(b => b.ReceivedId)
               .OnDelete(DeleteBehavior.NoAction);

            //builder
            //  .HasMany(u => u.Chats)
            //  .WithOne(b => b.Receiver)
            //  .HasForeignKey(b => b.ReceivedId)
            //  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
