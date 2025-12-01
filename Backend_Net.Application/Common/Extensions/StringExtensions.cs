using System.Web;

namespace Backend_Net.Application.Common.Extensions;

public static class StringExtensions
{
    public static string ToUrlDecode(this string value)
    {
        return HttpUtility.UrlDecode(value);
    }

    public static string ToUrlEncode(this string value)
    {
        return HttpUtility.UrlEncode(value);
    }
    
    public static string ToBase64Encode(this string plainText) 
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
    
    public static string ToBase64Decode(this string base64EncodedData) 
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }
}