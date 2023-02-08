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
    public class GetAllSwipesDeliveredByUser : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/swipes/{userId}/delivered", async (string userId, IMediator mediator) =>
            {
                return await mediator.Send(new GetAllSwipesDeliveredByUserQuery(userId));
            })
                .WithName(nameof(GetAllSwipesDeliveredByUser))
                .WithTags(nameof(Swipe));
        }
    }

    public class GetAllSwipesDeliveredByUserHandler : IRequestHandler<GetAllSwipesDeliveredByUserQuery, List<GetAllSwipesDeliveredByUserResponse>>
    {
        private readonly IDapperContext _context;

        public GetAllSwipesDeliveredByUserHandler(IDapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<GetAllSwipesDeliveredByUserResponse>> Handle(GetAllSwipesDeliveredByUserQuery request, CancellationToken cancellationToken)
        {
            var query = @"SELECT Swipes.Id AS Id, Swipes.FromUserId AS FromUserId, Swipes.ToUserId AS ToUserId, 
                             Swipes.Type AS Type, Swipes.CreatedAt AS CreatedAt FROM Swipes WHERE FromUserId = @FromUserId";

            var @params = new { FromUserId = request.UserId };
            using (var conection = _context.CreateConnection())
            {
                var swipes = await conection.QueryAsync<GetAllSwipesDeliveredByUserResponse>(query, @params);
                return swipes.ToList();
            }
        }
    }

    public record GetAllSwipesDeliveredByUserQuery(string UserId) : IRequest<List<GetAllSwipesDeliveredByUserResponse>>;

    public class GetAllSwipesDeliveredByUserResponse {
        public string Id { get; set; } = default!;
        public string FromUserId { get; set; } = default!;
        public string ToUserId { get; set; } = default!;
        public string Type { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
