using api.TransferModels;

namespace api.Helper;

public class ResponseHelper
{
    public ResponseDto Success(HttpContext http, int statusCode, string messageToClient, object? responseData = null)
    {
        http.Response.StatusCode = statusCode;
        return new ResponseDto(messageToClient)
        {
            ResponseData = responseData
        };
    }
    
}