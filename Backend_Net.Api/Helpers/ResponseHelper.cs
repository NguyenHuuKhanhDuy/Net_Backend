using System.Net;
using Backend_Net.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Net.Api.Helpers;

public static class ResponseHelper
{
    public static ObjectResult ToResponse(HttpStatusCode httpStatusCode, BaseResponse baseResponse, object data = null)
    {
        return new ObjectResult(new
            {
                success = baseResponse.Success,
                errorMessage = baseResponse.ErrorMessage,
                errorMessageCode = baseResponse.ErrorMessageCode,
                data = data
            })
            { StatusCode = (int)httpStatusCode };
    }
    
    public static ObjectResult ToPaginationResponse(int httpStatusCode, BaseResponse baseResponse, object data = null)
    {
        return new ObjectResult(new
            {
                success = baseResponse.Success,
                errorMessage = baseResponse.ErrorMessage,
                errorMessageCode = baseResponse.ErrorMessageCode,
                data = data,
                paging = baseResponse.Paging
            })
            { StatusCode = httpStatusCode };
    }
}