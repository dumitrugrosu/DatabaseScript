using DatabaseScript.Services;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly IDataMigrationService _dataMigrationService;

        public MigrationController(IDataMigrationService dataMigrationService)
        {
            _dataMigrationService = dataMigrationService;
        }

        [HttpPost]
        public IActionResult MigrateData()
        {
            try
            {
                // Call the MigrateData method of the DataMigrationService
                _dataMigrationService.MigrateData();
                return Ok("Migration completed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Migration failed: {ex.Message}");
            }
        }
    }
}