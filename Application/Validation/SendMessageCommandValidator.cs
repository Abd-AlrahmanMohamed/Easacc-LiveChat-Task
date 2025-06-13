using Application.MediatorHandler.MediatorCommend;
using Domain.Models;
using FluentValidation;
namespace Application.Validation
{
    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageCommandValidator()
        {

            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.Content).MaximumLength(500);
            //RuleFor(x => x.SenderId).NotEmpty();
            //RuleFor(x => x.ReceiverId).NotEmpty();
            //RuleFor(x => x.Type).IsInEnum();

            //RuleFor(x => x.Content)
            //    .NotEmpty()
            //    .When(x => x.Type == MessageType.Text)
            //    .WithMessage("Text content is required for text messages.");

            //RuleFor(x => x.FileUrl)
            //    .NotEmpty()
            //    .When(x => x.Type != MessageType.Document)
            //    .WithMessage("File URL is required for non-text messages.");
        }
    }
}
