using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VisitCountApi.Data;
using VisitCountApi.Entities;
using VisitCountApi.Services.Interfaces;

namespace VisitCountApi.Controllers
{
    [Route("Session")]
    [ApiController]
    public class VisitController : ControllerBase
    {
        private readonly IVisitService _visitService;
        private readonly IConfiguration _configuration;
        public VisitController(IVisitService visitService, IConfiguration configuration)
        {
            _visitService = visitService;
            _configuration = configuration;
        }


        // Client send something e.g.uploading Image to server and the method create or modify visitor properties and also add Session
        [HttpGet]
        public async Task<IActionResult> VisitCount(string t)
        {
            if (string.IsNullOrWhiteSpace(t) && false)
            {
                //Parameter logic
                return BadRequest("Something went wrong!");
            }
            const string cookieKey = "VisitID";
            var visitIdStr = Request.Cookies[cookieKey];
            bool success;
            //get the byte[] file from path (sample)
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Img", "Screenshot.png");
            var file = System.IO.File.ReadAllBytes(path);

            if (string.IsNullOrEmpty(visitIdStr) || !Guid.TryParse(visitIdStr, out Guid visitID))
            {
                visitID = Guid.NewGuid();
                SetCookie(cookieKey, visitID.ToString());
                success = await _visitService.AddVisitorAsync(visitID);
                //if (success)
                //{
                //    return File(file, "image/png");
                //}
            }
            else
            {
                success = await _visitService.UpdateVisitorAsync(visitID);
            }
            if (success)
            {
                if(await _visitService.UpdateDailyVisitAsync())
                    return File(file, "image/png");
            }
            return BadRequest("something went wrong!");
        }



        private void SetCookie(string key, string value)
        {
            int expirationTime = _configuration.GetValue<int>("CookieSettings:ExpirationTime");
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(expirationTime),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };

            Response.Cookies.Append(key, value, cookieOptions);
        }

    }
}
