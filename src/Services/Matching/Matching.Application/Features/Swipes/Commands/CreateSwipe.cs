﻿using Application.Shared.Exceptions;
using Carter;
using EventBus.Messages.Events;
using FluentValidation;
using MassTransit;
using Matching.Application.Common.Interfaces;
using Matching.Application.Domain.Entities;
using Matching.Application.Domain.Factories;
using Matching.Application.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Utils.Time;

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
        public string Type { get; set; } = default!;
    }

    public class CreateSwipesHandler : IRequestHandler<CreateSwipeCommand, IResult>
    {
        private readonly MatchingDbContext _context;
        private readonly ISwipesCacheRepository _cacheRepository;
        private readonly ISwipeIdFactory _swipeIdfactory;
        private readonly IMatchIdFactory _matchIdFactory;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<CreateSwipesHandler> _logger;

        public CreateSwipesHandler(MatchingDbContext context, ISwipesCacheRepository cacheRepository, ISwipeIdFactory swipeIdfactory, IMatchIdFactory matchIdFactory, IPublishEndpoint publishEndpoint, IDateTimeProvider dateTimeProvider, ILogger<CreateSwipesHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cacheRepository = cacheRepository ?? throw new ArgumentNullException(nameof(cacheRepository));
            _swipeIdfactory = swipeIdfactory ?? throw new ArgumentNullException(nameof(swipeIdfactory));
            _matchIdFactory = matchIdFactory ?? throw new ArgumentNullException(nameof(matchIdFactory));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IResult> Handle(CreateSwipeCommand request, CancellationToken cancellationToken)
        {
            var type = (SwipeType)Enum.Parse(typeof(SwipeType), request.Type, true);

            var fromUserSwipeId = _swipeIdfactory.Create(request.FromUserId, request.ToUserId, type);
            var toUserSwipeId = _swipeIdfactory.Create(request.ToUserId, request.FromUserId, type);


            if (await _cacheRepository.BothExistsByFromUserIdAndToUserIdAndType(request.FromUserId, request.ToUserId, type, cancellationToken))
            {
                var domainError = type == SwipeType.Right ? $"fromUserId : {request.FromUserId} and toUserId : {request.ToUserId} already match." : 
                                                        $"fromUserId : {request.FromUserId} and toUserId : {request.ToUserId} already left swipe each other.";
                throw new DomainException(domainError);
            }

            if (await _cacheRepository.ExistsAsync(fromUserSwipeId, cancellationToken))
            {
                var domainError = $"fromUserId : {request.FromUserId} already {request.Type} swipe toUserId {request.ToUserId}.";
                throw new DomainException(domainError);
            }

            // Match
            if (await _cacheRepository.ExistsAsync(toUserSwipeId, cancellationToken) && type == SwipeType.Right)
            {
                var newMatchSwipe = new Swipe(fromUserSwipeId, request.FromUserId, request.ToUserId, type, _dateTimeProvider.NowUtcOffset());
                _context.Swipes.Add(newMatchSwipe);


                await _cacheRepository.CreateAsync(newMatchSwipe, cancellationToken);

                var matchId = _matchIdFactory.Create(request.ToUserId, request.FromUserId);
                var match = new Match(matchId, request.ToUserId, request.FromUserId, _dateTimeProvider.NowUtcOffset());
                _context.Matches.Add(match);

                await _context.SaveChangesAsync(cancellationToken);


                var matchEvent = new MatchEvent(
                    Guid.NewGuid(),
                    _dateTimeProvider.NowUtcOffset(),
                    matchId.Value,
                    match.PartnerOneId,
                    match.PartnerTwoId
                    );

                await _publishEndpoint.Publish(matchEvent, cancellationToken);

                return Results.Created($"api/swipes/{newMatchSwipe.Id.Value}", null);
            }


            var newSwipe = new Swipe(fromUserSwipeId, request.FromUserId, request.ToUserId, type, _dateTimeProvider.NowUtcOffset());
            _context.Swipes.Add(newSwipe);
            await _context.SaveChangesAsync(cancellationToken);
            await _cacheRepository.CreateAsync(newSwipe, cancellationToken);


            if (type == SwipeType.Right)
            {
                var @event = new RightSwipeEvent(
                    Guid.NewGuid(),
                    _dateTimeProvider.NowUtcOffset(),
                    newSwipe.Id.Value,
                    newSwipe.FromUserId,
                    newSwipe.ToUserId);
                await _publishEndpoint.Publish(@event, cancellationToken);
            }
            else if (type == SwipeType.Left)
            {
                var @event = new LeftSwipeEvent(
                    Guid.NewGuid(),
                    _dateTimeProvider.NowUtcOffset(),
                    newSwipe.Id.Value,
                    newSwipe.FromUserId,
                    newSwipe.ToUserId);

                await _publishEndpoint.Publish(@event, cancellationToken);

            }

            return Results.Created($"api/swipes/{newSwipe.Id.Value}", null);
  
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

            RuleFor(s => s.Type).NotEmpty();
            RuleFor(s => s.Type).Must(s => MustBeInEnum(s));
        }

        private bool MustBeInEnum(string type)
        {
            type = type.ToLower();
            List<string> types = Enum.GetNames(typeof(SwipeType)).ToList();
            var typesSet = (from t in types select t.ToLower()).ToHashSet();
            return typesSet.Contains(type);
        }


    }
}
