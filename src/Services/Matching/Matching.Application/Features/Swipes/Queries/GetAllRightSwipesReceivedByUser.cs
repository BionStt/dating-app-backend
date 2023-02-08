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
    public class GetAllRightSwipesReceivedByUser : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/swipes/{userId}/right/received", async (string userId, IMediator mediator) =>
            {
                return await mediator.Send(new GetAllRightSwipesReceivedByUserQuery(userId));
            })
                .WithName(nameof(GetAllRightSwipesReceivedByUser))
                .WithTags(nameof(Swipe));
        }
    }


    public class GetAllRightSwipesReceivedByUserHandler : IRequestHandler<GetAllRightSwipesReceivedByUserQuery, List<GetAllRightSwipesReceivedByUserResponse>>
    {
        private readonly IDapperContext _context;

        public GetAllRightSwipesReceivedByUserHandler(IDapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<GetAllRightSwipesReceivedByUserResponse>> Handle(GetAllRightSwipesReceivedByUserQuery request, CancellationToken cancellationToken)
        {
            var query = @"SELECT Swipes.Id AS Id, Swipes.FromUserId AS FromUserId, Swipes.ToUserId AS ToUserId, 
                             Swipes.Type AS Type, Swipes.CreatedAt AS CreatedAt FROM Swipes WHERE Type = @Type AND ToUserId = @ToUserId ORDER BY CreatedAt DESC";

            var @params = new { Type = "Right", ToUserId = request.UserId };
            using (var connection = _context.CreateConnection())
            {
                var swipes = await connection.QueryAsync<GetAllRightSwipesReceivedByUserResponse>(query, @params);
                return swipes.ToList();
            }
        }
    }


    public record GetAllRightSwipesReceivedByUserQuery(string UserId) : IRequest<List<GetAllRightSwipesReceivedByUserResponse>>;

    public class GetAllRightSwipesReceivedByUserResponse
    {
        public string Id { get; set; } = default!;
        public string FromUserId { get; set; } = default!;
        public string ToUserId { get; set; } = default!;
        public string Type { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; } = default!;
    }
}
