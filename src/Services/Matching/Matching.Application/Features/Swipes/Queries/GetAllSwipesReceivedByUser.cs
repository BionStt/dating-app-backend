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
    public class GetAllSwipesReceivedByUser : ICarterModule
    {

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/swipes/{userId}/all/received", async (string userId, IMediator mediator) =>
            {
                return await mediator.Send(new GetAllSwipesReceivedByUserQuery(userId));
            })
                .WithName(nameof(GetAllSwipesReceivedByUser))
                .WithTags(nameof(Swipe));
        }
    }

    public class GetAllSwipesReceivedByUserHandler : IRequestHandler<GetAllSwipesReceivedByUserQuery, List<GetAllSwipesReceivedByUserResponse>>
    {
        private readonly IDapperContext _context;

        public GetAllSwipesReceivedByUserHandler(IDapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<GetAllSwipesReceivedByUserResponse>> Handle(GetAllSwipesReceivedByUserQuery request, CancellationToken cancellationToken)
        {
            var query = @"SELECT Swipes.Id AS Id, Swipes.FromUserId AS FromUserId, Swipes.ToUserId AS ToUserId, 
                             Swipes.Type AS Type, Swipes.CreatedAt AS CreatedAt FROM Swipes WHERE ToUserId = @ToUserId ORDER BY CreatedAt DESC";
            var @params = new { ToUserId = request.UserId };
            using (var connection = _context.CreateConnection())
            {
                var swipes = await connection.QueryAsync<GetAllSwipesReceivedByUserResponse>(query, @params);
                return swipes.ToList();
            }
        }
    }

    public record GetAllSwipesReceivedByUserQuery(string UserId) : IRequest<List<GetAllSwipesReceivedByUserResponse>>;

    public class GetAllSwipesReceivedByUserResponse
    {
        public string Id { get; set; } = default!;
        public string FromUserId { get; set; } = default!;
        public string ToUserId { get; set; } = default!;
        public string Type { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
