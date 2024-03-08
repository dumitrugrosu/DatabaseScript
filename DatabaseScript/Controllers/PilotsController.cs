using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using DatabaseScript.Models;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PilotsController : ControllerBase
   
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PilotsController> _logger;
        private readonly string _pilotsFilePath;
        private readonly string _connectionString;

        public PilotsController(ILogger<PilotsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _pilotsFilePath = configuration["FilePaths:File"];
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }
        private ScriptDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScriptDbContext>();
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));

            return new ScriptDbContext(optionsBuilder.Options);
        }
        [HttpPost("UploadPilots")]
        public IActionResult ProcessUploadedFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File is empty");

                // Save the uploaded file to the server
                string filePath = Path.Combine(_pilotsFilePath, file.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                // Process the uploaded Excel file
                ProcessExcelFile(filePath);

                return Ok("File uploaded and processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing uploaded file");
                return StatusCode(500, "An error occurred while processing the file");
            }
        }
        private void ProcessExcelFile(string filePath)
        {
            using (var dbContext = CreateDbContext())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                        throw new Exception("Worksheet not found in Excel file");

                    // Read Excel rows and process data
                    for (int row = worksheet.Dimension.Start.Row + 1; row <= worksheet.Dimension.End.Row; row++)
                    {
                        string primaryName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                        string fakesString = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        List<string> fakes = new List<string>(fakesString?.Split(',').Select(s => s.Trim()));
                        ProcessPrimaryPilot(dbContext, primaryName, fakes);
                    }
                    dbContext.SaveChanges();
                }
            }
        }
        private void ProcessPrimaryPilot(ScriptDbContext dbContext, string primaryName, List<string> fakes)
        {
            var primaryPilot = dbContext.AuxPilots.Where(p => p.Pilot == primaryName).OrderBy(p => p.IdPilot).FirstOrDefault();
            if (primaryPilot != null)
            {
                int primaryId = primaryPilot.IdPilot;
                string primaryNameDb = primaryPilot.Pilot;
                _logger.LogInformation($"Primary {primaryNameDb} found with id_pilot: {primaryId}");
                foreach (var fakeName in fakes)
                {
                    UpdateAndDeleteFakePilots(dbContext, primaryId, fakeName);
                }
            }
            else
            {
                _logger.LogWarning($"No match found for primary {primaryName}");
            }   
        }
        private void UpdateAndDeleteFakePilots(ScriptDbContext dbContext, int primaryId, string fakeName)
        {
            var fakePilot = dbContext.AuxPilots.FirstOrDefault(p => p.Pilot == fakeName);
            if (fakePilot != null)
            {
                var movementPilotsToUpdate = dbContext.AuxMovements.Where(m => m.PilotField == fakePilot.IdPilot);
                foreach (var m in movementPilotsToUpdate)
                {
                    m.PilotField = (uint)primaryId; // Set the new primary key value
                    dbContext.SaveChanges();
                }
                dbContext.AuxPilots.Remove(fakePilot);
                _logger.LogInformation($"Updated aux_movement_pilots for fake {fakeName}");
                _logger.LogInformation($"Deleted fake {fakeName} from aux_pilot");
                dbContext.SaveChanges();

            }
            else
            {
                _logger.LogWarning($"No match found for fake {fakeName}");
            }
        }
    }
}
