using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Matching.Application.Features.Swipes.EventHandlers
{
    public class LeftSwipeEventHandler : IConsumer<LeftSwipeEvent>
    {

        private readonly ILogger<LeftSwipeEventHandler> _logger;

        public LeftSwipeEventHandler(ILogger<LeftSwipeEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<LeftSwipeEvent> context)
        {
            var @event = context.Message;
            var eventStr = JsonSerializer.Serialize(@event);
            _logger.LogInformation("Left Swipe Event {} consumed", eventStr);
        }
    }
}
