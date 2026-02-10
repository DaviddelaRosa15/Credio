namespace Credio.Core.Domain.Common;

public abstract class BaseEntity
{
    public virtual string Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public BaseEntity()
    {
        Id = Guid.NewGuid().ToString().Substring(0, 12);
    }
}