namespace IncidentAlert.Models.Dto
{
    public abstract class BaseDto<T>
    {
        public T Id { get; set; } = default!;

    }
}
