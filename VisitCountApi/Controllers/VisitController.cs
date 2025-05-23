﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VisitCountApi.Data;
using VisitCountApi.Entities;

namespace VisitCountApi.Controllers
{
    [Route("session")]
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
        [HttpGet]
        public async Task<IActionResult> VisitCount(string t)
        {
            if (string.IsNullOrWhiteSpace(t)&& false)
            {
                //Parameter logic
                return BadRequest("Something went wrong!");
            }
            const string sessionKey = "VisitID";
            var visitIdStr = HttpContext.Session.GetString(sessionKey);
            bool success;
            if (string.IsNullOrEmpty(visitIdStr) || !Guid.TryParse(visitIdStr , out Guid visitID))
            {
                visitID = Guid.NewGuid();
                HttpContext.Session.SetString(sessionKey, visitID.ToString());
                success = await AddVisitor(visitID);
                if (success) 
                    return File("~/collect.gif", "image/gif");
            }
            else
            {
                success = await UpdateVisitor(visitID);
                if (success)
                    //return Ok("Record Successfully Updated");
                    return File("~/collect.gif", "image/gif");
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

    }
}
