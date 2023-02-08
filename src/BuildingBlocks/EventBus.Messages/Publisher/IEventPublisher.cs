using EventBus.Messages.Common;

namespace EventBus.Messages.Publisher
{
    public interface IEventPublisher
    {
        Task PublishAsync(BaseEvent @event, CancellationToken cancellationToken = default);
    }
}
