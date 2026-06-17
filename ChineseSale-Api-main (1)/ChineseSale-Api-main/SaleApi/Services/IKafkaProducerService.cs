using SaleApi.Dto;

namespace SaleApi.Services;

public interface IKafkaProducerService
{
    Task PublishOrderEventAsync(OrderEventMessage orderEvent, CancellationToken cancellationToken = default);
}
