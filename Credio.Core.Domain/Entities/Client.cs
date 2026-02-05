using Credio.Core.Domain.Common;

namespace Credio.Core.Domain.Entities
{
    public class Client : AuditableBaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int age { get; set; }
        public string DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal HomeLatitude { get; set; }
        public decimal HomeLongitude { get; set; }
        public Employee Officer { get; set; }
        public string OfficerId { get; set; }
        public string RouteId { get; set; }
        public string UserId { get; set; }
        public string AddressId { get; set; }
    }
}
