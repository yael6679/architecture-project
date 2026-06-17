using Confluent.Kafka;
using Microsoft.Extensions.Options;
using SaleApi.Configuration;
using SaleApi.Dto;
using System.Text.Json;

namespace SaleApi.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaConsumerService> _logger;

    public KafkaConsumerService(IOptions<KafkaSettings> settings, ILogger<KafkaConsumerService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = "chinesesale-order-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_settings.TopicName);

        _logger.LogInformation(
            "Kafka consumer started. Topic: {Topic}, BootstrapServers: {BootstrapServers}",
            _settings.TopicName,
            _settings.BootstrapServers);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                if (consumeResult?.Message?.Value == null)
                {
                    continue;
                }

                var orderEvent = JsonSerializer.Deserialize<OrderEventMessage>(consumeResult.Message.Value);
                if (orderEvent == null)
                {
                    _logger.LogWarning("Received invalid order event payload");
                    continue;
                }

                _logger.LogInformation(
                    "Order event consumed: {EventType}, UserId={UserId}, OrderId={OrderId}, OrderGroupId={OrderGroupId}, GiftId={GiftId}, TotalPrice={TotalPrice}, ItemCount={ItemCount}",
                    orderEvent.EventType,
                    orderEvent.UserId,
                    orderEvent.OrderId,
                    orderEvent.OrderGroupId,
                    orderEvent.GiftId,
                    orderEvent.TotalPrice,
                    orderEvent.Items.Count);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error");
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Kafka consumer");
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }

        consumer.Close();
        _logger.LogInformation("Kafka consumer stopped");
    }
}
