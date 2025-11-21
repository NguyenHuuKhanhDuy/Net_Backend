namespace Backend_Net.Application.Common.Models;

public class PagingInfo
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public long TotalItem { get; set; }
    public int TotalPage { get; set; }
}

public class BaseResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorMessageCode { get; set; }
    public PagingInfo? Paging { get; set; }

    public static BaseResponse Ok() => new() { Success = true };

    public static BaseResponse Fail(string message, string? code = null)
        => new() { Success = false, ErrorMessage = message, ErrorMessageCode = code };
}

public class BaseResponse<T> : BaseResponse
{
    public T? Data { get; set; }

    public static BaseResponse<T> Ok(T data, PagingInfo? paging = null)
        => new()
        {
            Success = true,
            Data = data,
            Paging = paging
        };

    public static new BaseResponse<T> Fail(string message, string? code = null)
        => new()
        {
            Success = false,
            ErrorMessage = message,
            ErrorMessageCode = code
        };
}