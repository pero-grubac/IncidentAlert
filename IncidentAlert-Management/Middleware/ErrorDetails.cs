using System.Text.Json;

namespace IncidentAlert_Management.Middleware
{
    public class ErrorDetails
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
