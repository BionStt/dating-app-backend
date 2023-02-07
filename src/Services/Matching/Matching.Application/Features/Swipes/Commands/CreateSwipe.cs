using Carter;
using FluentValidation;
using Matching.Application.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Matching.Application.Features.Swipes.Commands
{
    public class CreateSwipe : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/swipes", async (HttpRequest req, IMediator mediator, CreateSwipeCommand commnad) =>
            {
                return await mediator.Send(commnad);
            })
                .WithName(nameof(CreateSwipe))
                .WithTags(nameof(Swipe))
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status201Created);
        }

    }

    public class CreateSwipeCommand : IRequest<IResult>
    {
        public string FromUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }


    public class CreateSwipesHandler : IRequestHandler<CreateSwipeCommand, IResult>
    {
        private readonly IValidator<CreateSwipe> _validator;

        public Task<IResult> Handle(CreateSwipeCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }


    public class CreateSwipeCommandValidator : AbstractValidator<CreateSwipeCommand>
    {
        public CreateSwipeCommandValidator() 
        {
            RuleFor(s => s.FromUserId).NotEmpty();
            RuleFor(s => s.ToUserId).NotEmpty();
            RuleFor(s => s)
               .Must(s => s.FromUserId != s.ToUserId)
               .WithMessage("'FromUserId' and 'ToUserId' must be different.");

        }
    }

}
