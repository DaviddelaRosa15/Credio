using System.ComponentModel.DataAnnotations;

namespace Credio.Core.Domain.Entities;

public class SystemSettings
{
    [Key]
    public virtual string Key { get; set; }

    public string Value { get; set; } = string.Empty;

    public string DataType { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public string? GroupName { get; set; } = string.Empty;

    public bool? IsEditable { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? Modified { get; set; }

    public SystemSettings()
    {
        Key = Guid.NewGuid().ToString().Substring(0, 12);
    }
}