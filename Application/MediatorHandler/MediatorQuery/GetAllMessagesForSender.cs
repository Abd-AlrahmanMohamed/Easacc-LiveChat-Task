using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MediatorHandler.MediatorQuery
{
    public record GetAllMessagesForSender(string SenderId,string ReceiverId): IRequest<List<Message>>;
}
