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

        [HttpGet("/UpdateBarges")]
        
        public async Task<IActionResult> GetBargesById(int id)
        {
            try
            {
                var barges = await _context.Barges.FirstAsync(x => x.Primary == id);
                return Ok(barges);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Implement other CRUD actions as needed
    }
}
