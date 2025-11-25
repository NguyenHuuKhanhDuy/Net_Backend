using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Net.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "payment");

            migrationBuilder.CreateTable(
                name: "Currency",
                schema: "payment",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MinorUnit = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    ProviderType = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethodCurrency",
                schema: "payment",
                columns: table => new
                {
                    PaymentMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    MinAmount = table.Column<decimal>(type: "numeric(20,8)", nullable: true),
                    MaxAmount = table.Column<decimal>(type: "numeric(20,8)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodCurrency", x => new { x.PaymentMethodId, x.CurrencyCode });
                    table.ForeignKey(
                        name: "FK_PaymentMethodCurrency_Currency_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "payment",
                        principalTable: "Currency",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentMethodCurrency_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "payment",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransaction",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    OrderId = table.Column<string>(type: "text", nullable: true),
                    PaymentMethodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CurrencyCode = table.Column<string>(type: "varchar(10)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(20,8)", nullable: false),
                    FeeAmount = table.Column<decimal>(type: "numeric(20,8)", nullable: true),
                    NetAmount = table.Column<decimal>(type: "numeric(20,8)", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    ProviderTxnId = table.Column<string>(type: "text", nullable: true),
                    ProviderRawReq = table.Column<string>(type: "jsonb", nullable: true),
                    ProviderRawRes = table.Column<string>(type: "jsonb", nullable: true),
                    ReturnUrl = table.Column<string>(type: "text", nullable: true),
                    CallbackUrl = table.Column<string>(type: "text", nullable: true),
                    CallbackData = table.Column<string>(type: "jsonb", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_Currency_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "payment",
                        principalTable: "Currency",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "payment",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentTransaction_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "payment",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantCredentials",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApiKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SecretEncrypted = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastRotatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantCredentials_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "payment",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantPaymentMethod",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ConfigJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantPaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantPaymentMethod_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "payment",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantPaymentMethod_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "payment",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantPaymentMethodCurrency",
                schema: "payment",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    MinAmount = table.Column<decimal>(type: "numeric(20,8)", nullable: true),
                    MaxAmount = table.Column<decimal>(type: "numeric(20,8)", nullable: true),
                    FeeType = table.Column<string>(type: "text", nullable: true),
                    FeeValue = table.Column<decimal>(type: "numeric(20,8)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantPaymentMethodCurrency", x => new { x.TenantId, x.PaymentMethodId, x.CurrencyCode });
                    table.ForeignKey(
                        name: "FK_TenantPaymentMethodCurrency_Currency_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "payment",
                        principalTable: "Currency",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantPaymentMethodCurrency_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "payment",
                        principalTable: "PaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantPaymentMethodCurrency_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "payment",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_global_method_currency",
                        columns: x => new { x.PaymentMethodId, x.CurrencyCode },
                        principalSchema: "payment",
                        principalTable: "PaymentMethodCurrency",
                        principalColumns: new[] { "PaymentMethodId", "CurrencyCode" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactionAudit",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldName = table.Column<string>(type: "text", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ChangedBy = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactionAudit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactionAudit_PaymentTransaction_TransactionId",
                        column: x => x.TransactionId,
                        principalSchema: "payment",
                        principalTable: "PaymentTransaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentTransactionAudit_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "payment",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactionStatusHistory",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromStatus = table.Column<int>(type: "integer", nullable: true),
                    ToStatus = table.Column<int>(type: "integer", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ChangedBy = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactionStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactionStatusHistory_PaymentTransaction_Transact~",
                        column: x => x.TransactionId,
                        principalSchema: "payment",
                        principalTable: "PaymentTransaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebhookEvent",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CallbackUrl = table.Column<string>(type: "text", nullable: false),
                    CallbackData = table.Column<string>(type: "jsonb", nullable: true),
                    Payload = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebhookEvent_PaymentTransaction_TransactionId",
                        column: x => x.TransactionId,
                        principalSchema: "payment",
                        principalTable: "PaymentTransaction",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WebhookEvent_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "payment",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestLog",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TenantCredentialId = table.Column<Guid>(type: "uuid", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Method = table.Column<string>(type: "text", nullable: false),
                    Headers = table.Column<string>(type: "jsonb", nullable: true),
                    QueryParams = table.Column<string>(type: "jsonb", nullable: true),
                    Body = table.Column<string>(type: "jsonb", nullable: true),
                    ResponseStatus = table.Column<int>(type: "integer", nullable: true),
                    ResponseBody = table.Column<string>(type: "jsonb", nullable: true),
                    DurationMs = table.Column<int>(type: "integer", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestLog_TenantCredentials_TenantCredentialId",
                        column: x => x.TenantCredentialId,
                        principalSchema: "payment",
                        principalTable: "TenantCredentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebhookDelivery",
                schema: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Attempt = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RequestBody = table.Column<string>(type: "jsonb", nullable: true),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    HttpStatus = table.Column<int>(type: "integer", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookDelivery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebhookDelivery_WebhookEvent_EventId",
                        column: x => x.EventId,
                        principalSchema: "payment",
                        principalTable: "WebhookEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_Code",
                schema: "payment",
                table: "PaymentMethod",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethodCurrency_CurrencyCode",
                schema: "payment",
                table: "PaymentMethodCurrency",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "idx_payment_tx_tenant_created_at",
                schema: "payment",
                table: "PaymentTransaction",
                columns: new[] { "TenantId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "idx_payment_tx_tenant_status",
                schema: "payment",
                table: "PaymentTransaction",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_CurrencyCode",
                schema: "payment",
                table: "PaymentTransaction",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_PaymentMethodId",
                schema: "payment",
                table: "PaymentTransaction",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "idx_payment_tx_audit_tenant",
                schema: "payment",
                table: "PaymentTransactionAudit",
                columns: new[] { "TenantId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "idx_payment_tx_audit_tx",
                schema: "payment",
                table: "PaymentTransactionAudit",
                columns: new[] { "TransactionId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "idx_payment_tx_status_hist_tx",
                schema: "payment",
                table: "PaymentTransactionStatusHistory",
                columns: new[] { "TransactionId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "idx_request_log_created_at",
                schema: "payment",
                table: "RequestLog",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_request_log_path",
                schema: "payment",
                table: "RequestLog",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "idx_request_log_tenant_credential",
                schema: "payment",
                table: "RequestLog",
                column: "TenantCredentialId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_Code",
                schema: "payment",
                table: "Tenant",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_tenant_credentials_apikey",
                schema: "payment",
                table: "TenantCredentials",
                column: "ApiKey");

            migrationBuilder.CreateIndex(
                name: "idx_tenant_credentials_tenant_status",
                schema: "payment",
                table: "TenantCredentials",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantPaymentMethod_PaymentMethodId",
                schema: "payment",
                table: "TenantPaymentMethod",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPaymentMethod_TenantId_PaymentMethodId",
                schema: "payment",
                table: "TenantPaymentMethod",
                columns: new[] { "TenantId", "PaymentMethodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantPaymentMethodCurrency_CurrencyCode",
                schema: "payment",
                table: "TenantPaymentMethodCurrency",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPaymentMethodCurrency_PaymentMethodId_CurrencyCode",
                schema: "payment",
                table: "TenantPaymentMethodCurrency",
                columns: new[] { "PaymentMethodId", "CurrencyCode" });

            migrationBuilder.CreateIndex(
                name: "idx_webhook_delivery_event",
                schema: "payment",
                table: "WebhookDelivery",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookEvent_TenantId",
                schema: "payment",
                table: "WebhookEvent",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookEvent_TransactionId",
                schema: "payment",
                table: "WebhookEvent",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTransactionAudit",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "PaymentTransactionStatusHistory",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "RequestLog",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "TenantPaymentMethod",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "TenantPaymentMethodCurrency",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "WebhookDelivery",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "TenantCredentials",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "PaymentMethodCurrency",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "WebhookEvent",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "PaymentTransaction",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "Currency",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "PaymentMethod",
                schema: "payment");

            migrationBuilder.DropTable(
                name: "Tenant",
                schema: "payment");
        }
    }
}
