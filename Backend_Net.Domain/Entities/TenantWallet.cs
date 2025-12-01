namespace Backend_Net.Domain.Entities
{
    public class TenantWallet
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }
        public string CurrencyCode { get; set; } = default!;

        public decimal Balance { get; set; }
        public decimal PendingBalance { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = default!;
        public Currency Currency { get; set; } = default!;

        public ICollection<TenantWalletAudit> Audits { get; set; } = new List<TenantWalletAudit>();
    }
}