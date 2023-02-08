
using EventBus.Messages.Common;
using MassTransit;

namespace EventBus.Messages.Publisher
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _endpoint;

        public EventPublisher(IPublishEndpoint endpoint)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        public async Task PublishAsync(BaseEvent @event, CancellationToken cancellationToken = default)
        {
            await _endpoint.Publish(@event, cancellationToken);
        }
    }
}
