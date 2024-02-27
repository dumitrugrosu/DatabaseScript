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

        [HttpGet("/UpdatePilots")]
        public async Task<IActionResult> GetTugsById(int id)
        {
            try
            {
                var tugs = await _context.Tugs.FirstAsync(x => x.Primary == id);
                return Ok(tugs);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Implement other CRUD actions as needed
    }
}
