using Application.Shared.Exceptions;
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
    public class GetMatchById : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/matches/{matchId}", async (string matchId, IMediator mediator) =>
            {
                return await mediator.Send(new GetMatchByIdQuery(matchId));
            })
            .WithName(nameof(GetMatchById))
            .WithTags(nameof(Match));
        }
    }

    public class GetMatchByIdHandler : IRequestHandler<GetMatchByIdQuery, GetMatchByIdResponse>
    {
        private readonly IDapperContext _context;

        public GetMatchByIdHandler(IDapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetMatchByIdResponse> Handle(GetMatchByIdQuery request, CancellationToken cancellationToken)
        {
            var query = @"SELECT [Matches].Id AS Id, [Matches].PartnerOneId AS PartnerOneId, [Matches].PartnerTwoId AS PartnerTwoId,
                            [Matches].CreatedAt AS CreatedAt FROM [Matches] WHERE [Matches].Id = @Id";
            var @params = new { @Id = request.MatchId };

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<GetMatchByIdResponse>(query, @params);
                if (result == null)
                {
                    var notFoundError = $"Match with id : {request.MatchId}  was not found.";
                    throw new NotFoundException(notFoundError);
                }
                return result;
            }
        }
    }

    public record GetMatchByIdQuery(string MatchId) : IRequest<GetMatchByIdResponse>;

    public class GetMatchByIdResponse
    {
        public string Id { get; set; } = default!;
        public string PartnerOneId { get; set; } = default!;
        public string PartnerTwoId { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; } = default!;

    }

}
