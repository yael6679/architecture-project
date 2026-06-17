namespace SaleApi.Dto;

public class OrderEventMessage
{
    public string EventType { get; set; } = "OrderCreated";
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public int? GiftId { get; set; }
    public int? OrderGroupId { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderEventItem> Items { get; set; } = new();
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}

public class OrderEventItem
{
    public int GiftId { get; set; }
    public string GiftName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int Price { get; set; }
}
