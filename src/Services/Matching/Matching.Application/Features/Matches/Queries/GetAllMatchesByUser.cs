using Carter;
using Dapper;
using Matching.Application.Domain.Entities;
using Matching.Application.Infrastructure.Dapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Matching.Application.Features.Matches.Queries
{
    public class GetAllMatchesByUser : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/matches/{userId}/all", async (string userId, IMediator mediator) =>
            {
                return await mediator.Send(new GetAllMatchesByUserQuery(userId));
            })
            .WithName(nameof(GetAllMatchesByUser))
            .WithTags(nameof(Match));
        }
    }


    public class GetAllMatchesByUserHandler : IRequestHandler<GetAllMatchesByUserQuery, List<GetAllMatchesByUserResponse>>
    {
        private readonly IDapperContext _context;
        public GetAllMatchesByUserHandler(IDapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<GetAllMatchesByUserResponse>> Handle(GetAllMatchesByUserQuery request, CancellationToken cancellationToken)
        {
            var query = @"SELECT [Matches].Id AS Id, [Matches].PartnerOneId AS PartnerOneId, [Matches].PartnerTwoId AS PartnerTwoId,
                            [Matches].CreatedAt AS CreatedAt FROM [Matches] WHERE [Matches].PartnerOneId = @PartnerOneId OR
                            [Matches].PartnerTwoId = @PartnerTwoId ORDER BY [Matches].CreatedAt DESC"; 

            var @params = new { PartnerOneId = request.UserId, PartnerTwoId = request.UserId };
            using (var connection = _context.CreateConnection())
            {
                var matches = await connection.QueryAsync<GetAllMatchesByUserResponse>(query, @params);
                return matches.ToList();
            }
        }
    }


    public record GetAllMatchesByUserQuery(string UserId) : IRequest<List<GetAllMatchesByUserResponse>>;

    public class GetAllMatchesByUserResponse
    {
        public string Id { get; set; } = default!;
        public string PartnerOneId { get; set; } = default!;
        public string PartnerTwoId { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; } = default!;

    }

}
