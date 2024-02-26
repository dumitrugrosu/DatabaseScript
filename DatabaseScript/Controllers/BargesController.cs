using Microsoft.AspNetCore.Mvc;
using DatabaseScript.Context;
using Microsoft.EntityFrameworkCore;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BargesController : ControllerBase
    {
        private readonly ScriptDbContext _context;

        public BargesController(ScriptDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetBarges()
        {
            // Ensure that _context.Barges is of type IQueryable<Barge>
            return await _context.Barges.ToListAsync();
        }

        // Implement other CRUD actions as needed
    }
}
