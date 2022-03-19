namespace Base.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = default!;
}