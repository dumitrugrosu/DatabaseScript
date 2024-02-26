using Microsoft.AspNetCore.Mvc;
using DatabaseScript.Context;
using Microsoft.EntityFrameworkCore;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TugsController : ControllerBase
    {
        private readonly ScriptDbContext _context;

        public TugsController(ScriptDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetTugs()
        {
            return await _context.Tugs.ToListAsync();
        }

        // Implement other CRUD actions as needed
    }
}
