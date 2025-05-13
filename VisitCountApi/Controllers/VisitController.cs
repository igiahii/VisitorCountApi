using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VisitCountApi.Data;
using VisitCountApi.Entities;

namespace VisitCountApi.Controllers
{
    [Route("Session")]
    [ApiController]
    public class VisitController : ControllerBase
    {
        private readonly ILogger<VisitController> _logger;
        private readonly VisitorCountContext _context;
        private readonly IConfiguration _configuration;
        public VisitController(ILogger<VisitController> logger, VisitorCountContext context , IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }


        // Client send something e.g.uploading Image to server and the method create or modify visitor properties and also add Session
        [HttpGet]
        public async Task<IActionResult> VisitCount(string t)
        {
            if (string.IsNullOrWhiteSpace(t)&& false)
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

            if (string.IsNullOrEmpty(visitIdStr) || !Guid.TryParse(visitIdStr , out Guid visitID))
            {
                visitID = Guid.NewGuid();
                SetCookie(cookieKey, visitID.ToString());
                success = await AddVisitor(visitID);
                if (success)
                {
                    return File(file, "image/png");
                }
            }
            else
            {
                success = await UpdateVisitor(visitID);
                if (success)
                    return File(file, "image/png");
            }
            return BadRequest("something went wrong!");
        }


        private async Task<bool> AddVisitor(Guid visitID)
        {
            try
            {
                if (await _context.Visitors.AnyAsync(v => v.VisitId == visitID))
                {
                    visitID = Guid.NewGuid();
                }
                var visitor = new Visitor
                {
                    VisitId = visitID,
                    InstertDateTime = DateTime.Now,
                    LastUpdateDateTime = DateTime.Now,
                    VisitCount = 1
                };
                _context.Visitors.Add(visitor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VisitController AddVisitor");
                return false;
            }
        }

        private async Task<bool> UpdateVisitor(Guid visitID)
        {
            try
            {
                var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.VisitId == visitID);
                if (visitor is null)
                {
                    _logger.LogWarning("Visitor not found for VisitID: {VisitID}", visitID);
                    return false;
                }
                visitor.LastUpdateDateTime = DateTime.Now;
                visitor.VisitCount++;
                await _context.SaveChangesAsync(); 
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VisitController UpdateVisitor");
                return false;
            }
        }

        private void SetCookie(string key, string value)
        {
            int expirationTime = _configuration.GetValue<int>("CookieSettings:ExpirationTime");
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(expirationTime) ,
                HttpOnly = true ,
                Secure = true ,
                SameSite = SameSiteMode.Lax
            };

            Response.Cookies.Append(key, value, cookieOptions);
        }

    }
}
