CREATE TABLE IF NOT EXISTS payment."Currency"
(
    "Code"      varchar(10) NOT NULL,
    "Name"      text        NOT NULL,
    "MinorUnit" integer     NOT NULL DEFAULT 2,
    "IsActive"  boolean     NOT NULL DEFAULT TRUE,
    CONSTRAINT "PK_Currency" PRIMARY KEY ("Code")
);

CREATE TABLE IF NOT EXISTS payment."PaymentMethod"
(
    "Id"           uuid                     NOT NULL DEFAULT (gen_random_uuid()),
    "Code"         text                     NOT NULL,
    "DisplayName"  text                     NOT NULL,
    "ProviderType" text                     NOT NULL,
    "IsActive"     boolean                  NOT NULL DEFAULT TRUE,
    "CreatedAt"    timestamp with time zone NOT NULL DEFAULT (now()),
    "UpdatedAt"    timestamp with time zone NOT NULL DEFAULT (now()),
    CONSTRAINT "PK_PaymentMethod" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS payment."Tenant"
(
    "Id"        uuid                     NOT NULL DEFAULT (gen_random_uuid()),
    "Code"      text                     NOT NULL,
    "Name"      text                     NOT NULL,
    "IsActive"  boolean                  NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now()),
    "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now()),
    CONSTRAINT "PK_Tenant" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS payment."PaymentMethodCurrency"
(
    "PaymentMethodId" uuid        NOT NULL,
    "CurrencyCode"    varchar(10) NOT NULL,
    "IsActive"        boolean     NOT NULL DEFAULT TRUE,
    "MinAmount"       numeric(20, 8),
    "MaxAmount"       numeric(20, 8),
    CONSTRAINT "PK_PaymentMethodCurrency" PRIMARY KEY ("PaymentMethodId", "CurrencyCode"),
    CONSTRAINT "FK_PaymentMethodCurrency_Currency_CurrencyCode" FOREIGN KEY ("CurrencyCode") REFERENCES payment."Currency" ("Code") ON DELETE CASCADE,
    CONSTRAINT "FK_PaymentMethodCurrency_PaymentMethod_PaymentMethodId" FOREIGN KEY ("PaymentMethodId") REFERENCES payment."PaymentMethod" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS payment."PaymentTransaction"
(
    "Id"              uuid                     NOT NULL DEFAULT (gen_random_uuid()),
    "TenantId"        uuid                     NOT NULL,
    "UserId"          text,
    "OrderId"         text,
    "PaymentMethodId" uuid,
    "CurrencyCode"    varchar(10),
    "Amount"          numeric(20, 8)           NOT NULL,
    "FeeAmount"       numeric(20, 8),
    "NetAmount"       numeric(20, 8),
    "Status"          integer                  NOT NULL DEFAULT 2,
    "ProviderTxnId"   text,
    "ProviderRawReq"  jsonb,
    "ProviderRawRes"  jsonb,
    "ReturnUrl"       text,
    "CallbackUrl"     text,
    "CallbackData"    jsonb,
    "Description"     text,
    "CreatedAt"       timestamp with time zone NOT NULL DEFAULT (now()),
    "UpdatedAt"       timestamp with time zone NOT NULL DEFAULT (now()),
    CONSTRAINT "PK_PaymentTransaction" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PaymentTransaction_Currency_CurrencyCode" FOREIGN KEY ("CurrencyCode") REFERENCES payment."Currency" ("Code"),
    CONSTRAINT "FK_PaymentTransaction_PaymentMethod_PaymentMethodId" FOREIGN KEY ("PaymentMethodId") REFERENCES payment."PaymentMethod" ("Id"),
    CONSTRAINT "FK_PaymentTransaction_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES payment."Tenant" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS payment."TenantCredentials"
(
    "Id"              uuid                     NOT NULL DEFAULT (gen_random_uuid()),
    "TenantId"        uuid                     NOT NULL,
    "ApiKey"          character varying(100)   NOT NULL,
    "SecretEncrypted" text                     NOT NULL,
    "Status"          integer                  NOT NULL,
    "CreatedAt"       timestamp with time zone NOT NULL DEFAULT (NOW()),
    "ExpiredAt"       timestamp with time zone,
    "LastRotatedAt"   timestamp with time zone NOT NULL DEFAULT (NOW()),
    CONSTRAINT "PK_TenantCredentials" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TenantCredentials_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES payment."Tenant" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS payment."TenantPaymentMethod"
(
    "Id"              uuid                     NOT NULL DEFAULT (gen_random_uuid()),
    "TenantId"        uuid                     NOT NULL,
    "PaymentMethodId" uuid                     NOT NULL,
    "IsEnabled"       boolean                  NOT NULL DEFAULT TRUE,
    "ConfigJson"      jsonb,
    "CreatedAt"       timestamp with time zone NOT NULL DEFAULT (now()),
    "UpdatedAt"       timestamp with time zone NOT NULL DEFAULT (now()),
    CONSTRAINT "PK_TenantPaymentMethod" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TenantPaymentMethod_PaymentMethod_PaymentMethodId" FOREIGN KEY ("PaymentMethodId") REFERENCES payment."PaymentMethod" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TenantPaymentMethod_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES payment."Tenant" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS payment."TenantPaymentMethodCurrency"
(
    "TenantId"        uuid        NOT NULL,
    "PaymentMethodId" uuid        NOT NULL,
    "CurrencyCode"    varchar(10) NOT NULL,
    "IsEnabled"       boolean     NOT NULL DEFAULT TRUE,
    "MinAmount"       numeric(20, 8),
    "MaxAmount"       numeric(20, 8),
    "FeeType"         text,
    "FeeValue"        numeric(20, 8),
    CONSTRAINT "PK_TenantPaymentMethodCurrency" PRIMARY KEY ("TenantId", "PaymentMethodId", "CurrencyCode"),
    CONSTRAINT "FK_TenantPaymentMethodCurrency_Currency_CurrencyCode" FOREIGN KEY ("CurrencyCode") REFERENCES payment."Currency" ("Code") ON DELETE CASCADE,
    CONSTRAINT "FK_TenantPaymentMethodCurrency_PaymentMethod_PaymentMethodId" FOREIGN KEY ("PaymentMethodId") REFERENCES payment."PaymentMethod" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TenantPaymentMethodCurrency_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES payment."Tenant" ("Id") ON DELETE CASCADE,
    CONSTRAINT fk_global_method_currency FOREIGN KEY ("PaymentMethodId", "CurrencyCode") REFERENCES payment."PaymentMethodCurrency" ("PaymentMethodId", "CurrencyCode") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS payment."PaymentTransactionAudit"
(
    "Id"            uuid                     NOT NULL DEFAULT (gen_random_uuid()),
    "TransactionId" uuid                     NOT NULL,
    "TenantId"      uuid                     NOT NULL,
    "FieldName"     text                     NOT NULL,
    "OldValue"      text,
    "NewValue"      text,
    "ChangedAt"     timestamp with time zone NOT NULL DEFAULT (now()),
    "ChangedBy"     text,
    "Reason"        text,
    CONSTRAINT "PK_PaymentTransactionAudit" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PaymentTransactionAudit_PaymentTransaction_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES payment."PaymentTransaction" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PaymentTransactionAudit_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES payment."Tenant" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS payment."PaymentTransactionStatusHistory"
(
    "Id"            uuid                     NOT NULL DEFAULT (gen_random_uuid()),
    "TransactionId" uuid                     NOT NULL,
    "FromStatus"    integer,
    "ToStatus"      integer                  NOT NULL,
    "ChangedAt"     timestamp with time zone NOT NULL DEFAULT (now()),
    "ChangedBy"     text,
    "Reason"        text,
    CONSTRAINT "PK_PaymentTransactionStatusHistory" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PaymentTransactionStatusHistory_PaymentTransaction_Transact~" FOREIGN KEY ("TransactionId") REFERENCES payment."PaymentTransaction" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS payment."WebhookEvent"
(
    "Id"            uuid                     NOT NULL DEFAULT (gen_random_uuid()),
    "EventType"     text                     NOT NULL,
    "TenantId"      uuid                     NOT NULL,
    "TransactionId" uuid,
    "CallbackUrl"   text                     NOT NULL,
    "CallbackData"  jsonb,
    "Payload"       jsonb                    NOT NULL,
    "CreatedAt"     timestamp with time zone NOT NULL DEFAULT (now()),
    CONSTRAINT "PK_WebhookEvent" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_WebhookEvent_PaymentTransaction_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES payment."PaymentTransaction" ("Id"),
    CONSTRAINT "FK_WebhookEvent_Tenant_TenantId" FOREIGN KEY ("TenantId") REFERENCES payment."Tenant" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS payment."RequestLog"
(
    "Id"                 uuid                     NOT NULL DEFAULT (gen_random_uuid()),
    "TenantCredentialId" uuid                     NOT NULL,
    "Path"               text                     NOT NULL,
    "Method"             text                     NOT NULL,
    "Headers"            jsonb,
    "QueryParams"        jsonb,
    "Body"               jsonb,
    "ResponseStatus"     integer,
    "ResponseBody"       jsonb,
    "DurationMs"         integer,
    "IpAddress"          text,
    "UserAgent"          text,
    "ErrorMessage"       text,
    "CreatedAt"          timestamp with time zone NOT NULL DEFAULT (now()),
    CONSTRAINT "PK_RequestLog" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_RequestLog_TenantCredentials_TenantCredentialId" FOREIGN KEY ("TenantCredentialId") REFERENCES payment."TenantCredentials" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS payment."WebhookDelivery"
(
    "Id"           uuid                     NOT NULL DEFAULT (gen_random_uuid()),
    "EventId"      uuid                     NOT NULL,
    "Attempt"      integer                  NOT NULL DEFAULT 1,
    "Status"       text                     NOT NULL,
    "RequestBody"  jsonb,
    "ResponseBody" text,
    "HttpStatus"   integer,
    "ErrorMessage" text,
    "SentAt"       timestamp with time zone,
    "CreatedAt"    timestamp with time zone NOT NULL DEFAULT (now()),
    CONSTRAINT "PK_WebhookDelivery" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_WebhookDelivery_WebhookEvent_EventId" FOREIGN KEY ("EventId") REFERENCES payment."WebhookEvent" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_PaymentMethod_Code" ON payment."PaymentMethod" ("Code");

CREATE INDEX "IX_PaymentMethodCurrency_CurrencyCode" ON payment."PaymentMethodCurrency" ("CurrencyCode");

CREATE INDEX idx_payment_tx_tenant_created_at ON payment."PaymentTransaction" ("TenantId", "CreatedAt");

CREATE INDEX idx_payment_tx_tenant_status ON payment."PaymentTransaction" ("TenantId", "Status");

CREATE INDEX "IX_PaymentTransaction_CurrencyCode" ON payment."PaymentTransaction" ("CurrencyCode");

CREATE INDEX "IX_PaymentTransaction_PaymentMethodId" ON payment."PaymentTransaction" ("PaymentMethodId");

CREATE INDEX idx_payment_tx_audit_tenant ON payment."PaymentTransactionAudit" ("TenantId", "ChangedAt");

CREATE INDEX idx_payment_tx_audit_tx ON payment."PaymentTransactionAudit" ("TransactionId", "ChangedAt");

CREATE INDEX idx_payment_tx_status_hist_tx ON payment."PaymentTransactionStatusHistory" ("TransactionId", "ChangedAt");

CREATE INDEX idx_request_log_created_at ON payment."RequestLog" ("CreatedAt");

CREATE INDEX idx_request_log_path ON payment."RequestLog" ("Path");

CREATE INDEX idx_request_log_tenant_credential ON payment."RequestLog" ("TenantCredentialId");

CREATE UNIQUE INDEX "IX_Tenant_Code" ON payment."Tenant" ("Code");

CREATE INDEX idx_tenant_credentials_apikey ON payment."TenantCredentials" ("ApiKey");

CREATE INDEX idx_tenant_credentials_tenant_status ON payment."TenantCredentials" ("TenantId", "Status");

CREATE INDEX "IX_TenantPaymentMethod_PaymentMethodId" ON payment."TenantPaymentMethod" ("PaymentMethodId");

CREATE UNIQUE INDEX "IX_TenantPaymentMethod_TenantId_PaymentMethodId" ON payment."TenantPaymentMethod" ("TenantId", "PaymentMethodId");

CREATE INDEX "IX_TenantPaymentMethodCurrency_CurrencyCode" ON payment."TenantPaymentMethodCurrency" ("CurrencyCode");

CREATE INDEX "IX_TenantPaymentMethodCurrency_PaymentMethodId_CurrencyCode" ON payment."TenantPaymentMethodCurrency" ("PaymentMethodId", "CurrencyCode");

CREATE INDEX idx_webhook_delivery_event ON payment."WebhookDelivery" ("EventId");

CREATE INDEX "IX_WebhookEvent_TenantId" ON payment."WebhookEvent" ("TenantId");

CREATE INDEX "IX_WebhookEvent_TransactionId" ON payment."WebhookEvent" ("TransactionId");