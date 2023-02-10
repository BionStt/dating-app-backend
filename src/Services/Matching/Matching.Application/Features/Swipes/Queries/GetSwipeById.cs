using Application.Shared.Exceptions;
using Carter;
using Dapper;
using Matching.Application.Domain.Entities;
using Matching.Application.Infrastructure.Dapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Matching.Application.Features.Swipes.Queries
{
    public class GetSwipeById : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/swipes/{swipeId}", async (string swipeId, IMediator mediator) =>
            {
                return await mediator.Send(new GetSwipeByIdQuery(swipeId));
            })
                .WithName(nameof(GetSwipeById))
                .WithTags(nameof(Swipe));
        }
    }

    public class GetSwipeByIdHandler : IRequestHandler<GetSwipeByIdQuery, GetSwipeByIdResponse>
    {
        private readonly IDapperContext _context;

        public GetSwipeByIdHandler(IDapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetSwipeByIdResponse> Handle(GetSwipeByIdQuery request, CancellationToken cancellationToken)
        {
            var query = @"SELECT Swipes.Id AS Id, Swipes.FromUserId AS FromUserId, Swipes.ToUserId AS ToUserId, 
                          Swipes.Type AS Type, Swipes.CreatedAt AS CreatedAt FROM Swipes WHERE Id = @Id";

            var @params = new { Id = request.SwipeId };
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<GetSwipeByIdResponse>(query, @params);
                if (result == null)
                {
                    var notFoundError = $"Swipe with the id {request.SwipeId} was not found.";
                    throw new NotFoundException(notFoundError);
                }
                return result;
            }
        }
    }

    public record GetSwipeByIdQuery(string SwipeId) : IRequest<GetSwipeByIdResponse>;
    public class GetSwipeByIdResponse 
    {
        public string Id { get; set; } = default!;
        public string FromUserId { get; set; } = default!;
        public string ToUserId { get; set; } = default!;
        public string Type { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; } = default;
    }
}
