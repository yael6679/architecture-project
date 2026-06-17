using Confluent.Kafka;
using Microsoft.Extensions.Options;
using SaleApi.Configuration;
using SaleApi.Dto;
using System.Text.Json;

namespace SaleApi.Services;

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topicName;
    private readonly ILogger<KafkaProducerService> _logger;

    public KafkaProducerService(IOptions<KafkaSettings> settings, ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
        var kafkaSettings = settings.Value;
        _topicName = kafkaSettings.TopicName;

        var config = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishOrderEventAsync(OrderEventMessage orderEvent, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(orderEvent);
        var key = orderEvent.OrderGroupId?.ToString()
            ?? orderEvent.OrderId?.ToString()
            ?? orderEvent.UserId.ToString();

        try
        {
            var result = await _producer.ProduceAsync(
                _topicName,
                new Message<string, string> { Key = key, Value = payload },
                cancellationToken);

            _logger.LogInformation(
                "Kafka message published to {Topic} partition {Partition} offset {Offset}",
                result.Topic,
                result.Partition.Value,
                result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to publish order event to Kafka topic {Topic}", _topicName);
            throw;
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
