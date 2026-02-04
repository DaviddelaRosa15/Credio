namespace Credio.Infrastructure.Identity.Entities;

public class SystemSettings
{
    public Guid Key { get; set; }

    public string Value { get; set; } = string.Empty;

    public string DataType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string GroupName { get; set; } = string.Empty;

    public bool IsEditable { get; set; }

    public string ModifiedBy { get; set; } = string.Empty;

    public DateTime Modified { get; set; }
}