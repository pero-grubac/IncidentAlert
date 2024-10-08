namespace Contracts.Category
{
    public record CategoryUpdatedConsumer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
