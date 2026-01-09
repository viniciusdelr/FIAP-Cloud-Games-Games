namespace FCG.API.Events
{
    public record GamePurchased
    {
        public Guid CorrelationId { get; init; } // ID para rastrear a venda
        public int UserId { get; init; }
        public decimal Price { get; init; }
    }
}
