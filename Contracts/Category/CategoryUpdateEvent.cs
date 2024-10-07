namespace Contracts.Category
{
    public record CategoryUpdateEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
