using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Backend_Net.Application.Services.Signature;

public class SignatureService : ISignatureService
{
    private string ComputeSignature(string canonical, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var dataBytes = Encoding.UTF8.GetBytes(canonical);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);

        return ConvertToHex(hash); // hex lowercase
    }
    
    private static string ConvertToHex(byte[] bytes)
    {
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
            sb.Append(b.ToString("x2")); // lowercase
        return sb.ToString();
    }
    
    // Build canonical query: key1=val1&key2=val2 (sorted by key, URL-encoded)
    private static string BuildCanonical(IDictionary<string, string>? components)
    {
        if (components == null || components.Count == 0)
            return string.Empty;

        var sb = new StringBuilder();
        foreach (var kv in components.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            if (sb.Length > 0)
                sb.Append('&');

            var key = kv.Key;
            var value = kv.Value;

            // URL-encode value
            sb.Append(key);
            sb.Append('=');
            sb.Append(HttpUtility.UrlEncode(value));
        }

        return sb.ToString();
    }

    public string CreateSignature(Dictionary<string, string> parameters, string secretKey)
    {
        var canonical = BuildCanonical(parameters);
        var signature = ComputeSignature(canonical, secretKey);
        return signature;
    }
}