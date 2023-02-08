using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Matching.Application.Features.Swipes.EventHandlers
{
    public class RightSwipeEventHandler : IConsumer<RightSwipeEvent>
    {
        private readonly ILogger<RightSwipeEventHandler> _logger;

        public RightSwipeEventHandler(ILogger<RightSwipeEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<RightSwipeEvent> context)
        {
            var @event = context.Message;
            var eventStr = JsonSerializer.Serialize(@event);
            _logger.LogInformation("Right Swipe Event {} consumed", eventStr);
        }
    }
}
