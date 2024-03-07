using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using DatabaseScript.Context;
using Microsoft.EntityFrameworkCore;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TugsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TugsController> _logger;
        private readonly string _tugsFilePath;
        private readonly string _connectionString;

        public TugsController(ILogger<TugsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _tugsFilePath = configuration["FilePaths:File"];
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private ScriptDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScriptDbContext>();
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));

            return new ScriptDbContext(optionsBuilder.Options);
        }

        [HttpPost("UploadTugs")]
        public IActionResult ProcessUploadedFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File is empty");

                // Save the uploaded file to the server
                string filePath = Path.Combine(_tugsFilePath, file.FileName);
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

                        ProcessPrimaryTug(dbContext, primaryName, fakes);
                    }

                    dbContext.SaveChanges();
                }
            }
        }

        private void ProcessPrimaryTug(ScriptDbContext dbContext, string primaryName, List<string> fakes)
        {
            var primaryTug = dbContext.Tugs.FirstOrDefault(t => t.Name == primaryName);

            if (primaryTug != null)
            {
                int primaryId = primaryTug.Id;
                string primaryNameDb = primaryTug.Name;

                _logger.LogInformation($"Primary {primaryNameDb} found with id_tug: {primaryId}");

                foreach (var fakeName in fakes)
                {
                    UpdateAndDeleteFakeTugs(dbContext, primaryId, fakeName); // Pass dbContext to the method
                }
            }
            else
            {
                _logger.LogWarning($"No match found for primary {primaryName}");
            }
        }

        private void UpdateAndDeleteFakeTugs(ScriptDbContext dbContext, int primaryId, string fakeName)
        {
            var fakeTug = dbContext.Tugs.FirstOrDefault(t => t.Name == fakeName);

            if (fakeTug != null)
            {
                // Update related entries in aux_movement_tugs
                var movementTugsToUpdate = dbContext.MovementTugs.Where(mt => mt.IdTug == fakeTug.Id);
                foreach (var mt in movementTugsToUpdate)
                {
                    mt.IdTug = primaryId;
                }
                // Remove fakeTug
                dbContext.Tugs.Remove(fakeTug);

                _logger.LogInformation($"Updated aux_movement_tugs for fake {fakeName} and {fakeTug.Id}");
                _logger.LogInformation($"Deleted fake {fakeName} and {fakeTug.Id} from aux_tugs");

                // Save changes to the database
                dbContext.SaveChanges();
            }
            else
            {
                _logger.LogWarning($"No match found for fake {fakeName}");
            }
        }



    }
}