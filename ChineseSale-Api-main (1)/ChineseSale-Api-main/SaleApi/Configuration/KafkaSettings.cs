namespace SaleApi.Configuration;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string TopicName { get; set; } = "order-events";
}
