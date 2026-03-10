namespace Credio.Core.Domain.Common
{
    public abstract class AuditableBaseEntity : Entity
    {
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
