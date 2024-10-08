namespace Contracts.Image
{
    public record ImageData
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = [];
    }
}
