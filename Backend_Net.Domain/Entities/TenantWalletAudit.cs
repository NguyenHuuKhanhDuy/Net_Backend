using System;
using Backend_Net.Domain.Enums;

namespace Backend_Net.Domain.Entities;

public class TenantWalletAudit
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public Guid TransactionId { get; set; }
    public string? ReferenceId { get; set; }
    public TenantWalletAuditRelatedType RelatedType { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public TenantWalletAuditDirection Direction { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }

    // Navigation
    public TenantWallet Wallet { get; set; } = default!;
}