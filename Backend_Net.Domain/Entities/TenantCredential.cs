using Backend_Net.Domain.Enums;

namespace Backend_Net.Domain.Entities
{
    public class TenantCredential
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string ApiKey { get; set; }
        public string SecretEncrypted { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime LastRotatedAt { get; set; }
        
        public Tenant Tenant { get; set; }
        public ICollection<RequestLog> RequestLogs { get; set; } = new List<RequestLog>();
    }
}