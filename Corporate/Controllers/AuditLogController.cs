using Microsoft.AspNetCore.Mvc;

namespace Corporate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAuditLogs([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var logs = _context.AuditLogs.AsQueryable();

            if (from.HasValue)
            {
                logs = logs.Where(log => log.Timestamp >= from.Value);
            }

            if (to.HasValue)
            {
                logs = logs.Where(log => log.Timestamp <= to.Value);
            }

            return Ok(logs.OrderByDescending(log => log.Timestamp).Take(10).ToList());
        }
    }
}
