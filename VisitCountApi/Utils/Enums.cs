using System.ComponentModel.DataAnnotations;

namespace AzmoonirCore.Utils
{
    public class Enums
    {
        public enum ApiResultStatusCode
        {
            Ok = 200,
            BadRequest = 400,
            Unauthorized = 401,
            [Display(Name = "Token Expired")]
            TokenExpired = 402,
            AccessDenied = 403,
            NotFound = 404,
            ServerError = 500,
            LogicError = 501,
        }
    }

}
