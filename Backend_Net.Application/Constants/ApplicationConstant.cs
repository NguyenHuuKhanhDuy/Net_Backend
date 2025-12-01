namespace Backend_Net.Application.Constants;

public static class ApplicationConstant
{
    public const string ApplicationName = "Backend Application";
    public const string PaymentUrl = "{0}/payments/process?token={1}";
    public const string ResultUrl = "{0}/payments/result?token={1}";
    public readonly struct Headers
    {
        public const string ApiKey = "X-API-KEY";
        public const string Signature = "X-SIGNATURE";
        public const string RequestId = "X-Request-ID";
        public const string CorrelationId = "X-Correlation-ID";
    }
    
    public readonly struct Currencies
    {
        public const string THB = "THB";
        public const string MYR = "MYR";
        public const string VND = "VND";
        public const string USD = "USD";
        public const string BDT = "BDT";
        public const string BRL = "BRL";
        public const string PHP = "PHP";
        public const string INR = "INR";
        public const string EGP = "EGP";
    }
}