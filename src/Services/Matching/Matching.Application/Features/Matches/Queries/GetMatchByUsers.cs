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
    public class GetMatchByUsers : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/matches/{partnerOneId}/{partnerTwoId}", async (string partnerOneId, string partnerTwoId, IMediator mediator) =>
            {
                return await mediator.Send(new GetMatchByUsersQuery(partnerOneId, partnerTwoId));
            })
                .WithName(nameof(GetMatchByUsers))
                .WithTags(nameof(Match));
        }
    }

    public class GetMatchByUsersHandler : IRequestHandler<GetMatchByUsersQuery, GetMatchByUsersResponse>
    {
        private readonly IDapperContext _context;

        public GetMatchByUsersHandler(IDapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetMatchByUsersResponse> Handle(GetMatchByUsersQuery request, CancellationToken cancellationToken)
        {
            var query = @"SELECT [Matches].Id AS Id, [Matches].PartnerOneId AS PartnerOneId, [Matches].PartnerTwoId AS PartnerTwoId,
                          [Matches].CreatedAt AS CreatedAt FROM [Matches] WHERE (PartnerOneId = @PartnerOneId AND PartnerTwoId = @PartnerTwoId) OR 
                          (PartnerOneId = @PartnerTwoId AND PartnerTwoId = @PartnerOneId)";
            
            var @params = new { PartnerOneId = request.PartnerOneId, PartnerTwoId = request.PartnerTwoId };

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<GetMatchByUsersResponse>(query, @params);
                if (result == null)
                {
                    var notFoundMessage = $"Match between partnerOneId : {request.PartnerOneId} and partnerTwoId : {request.PartnerTwoId} was not found.";
                    throw new NotFoundException(notFoundMessage);
                }
                return result;
            }
        }
    }

    public record GetMatchByUsersQuery(string PartnerOneId, string PartnerTwoId) : IRequest<GetMatchByUsersResponse>;

    public class GetMatchByUsersResponse
    {
        public string Id { get; set; } = default!;
        public string PartnerOneId { get; set; } = default!;
        public string PartnerTwoId { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; } = default!;

    }
}

