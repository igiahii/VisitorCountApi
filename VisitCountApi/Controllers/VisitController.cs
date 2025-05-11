using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VisitCountApi.Data;
using VisitCountApi.Entities;

namespace VisitCountApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitController : ControllerBase
    {
        private readonly ILogger<VisitController> _logger;
        private readonly VisitorCountContext _context;
        public VisitController(ILogger<VisitController> logger, VisitorCountContext context)
        {
            _logger = logger;
            _context = context;
        }


        // Client send something e.g.uploading Image to server and the method create or modify visitor properties and also add Session
        [HttpPost]
        public async Task<IActionResult> VisitCount(string something)
        {
            if (string.IsNullOrWhiteSpace(something )&& false)
            {
                //Parameter logic
                return BadRequest("Something went wrong!");
            }
            const string cookieKey = "VisitID";
            var visitIdStr = Request.Cookies[cookieKey];
            bool success;
            if (string.IsNullOrEmpty(visitIdStr) || !Guid.TryParse(visitIdStr , out Guid visitID))
            {
                visitID = Guid.NewGuid();
                SetCookie(cookieKey, visitID.ToString());
                success = await AddVisitor(visitID);
                if (success) 
                    return Ok("Record Successfully Initialized");
            }
            else
            {
                success = await UpdateVisitor(visitID);
                if (success)
                    return Ok("Record Successfully Updated");
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
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(20) ,
                HttpOnly = true ,
                Secure = true ,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append(key, value, cookieOptions);
        }

    }
}
