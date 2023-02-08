using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Matching.Application.Features.Matches.EventHandlers
{
    public class MatchEventHandler : IConsumer<MatchEvent>
    {
        private readonly ILogger<MatchEventHandler> _logger;

        public MatchEventHandler(ILogger<MatchEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<MatchEvent> context)
        {
            var @event = context.Message;
            var eventStr = JsonSerializer.Serialize(@event);
            _logger.LogInformation("Match Event {} consumed", eventStr);
        }
    }
}
