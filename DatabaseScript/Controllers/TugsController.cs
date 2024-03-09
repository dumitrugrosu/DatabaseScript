using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using DatabaseScript.Models;

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
                        // Assuming ID is in the first column
                        if (!int.TryParse(worksheet.Cells[row, 1].Value?.ToString().Trim(), out int primaryId))
                        {
                            _logger.LogWarning($"Invalid or missing ID at row {row}");
                            continue; // Skip this row if the ID is invalid
                        }
                        string primaryName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                        string fakesString = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        List<string> fakes = new List<string>(fakesString?.Split(',').Select(s => s.Trim()));
                        ProcessPrimaryTug(dbContext, primaryId, primaryName, fakes);
                    }
                    dbContext.SaveChanges();
                }
            }
        }
        private void ProcessPrimaryTug(ScriptDbContext dbContext, int primaryId, string primaryName, List<string> fakes)
        {
            var primaryTug = dbContext.AuxTugs.FirstOrDefault(t => t.IdTug == primaryId && t.NameTug == primaryName);
            if (primaryTug != null)
            {
                _logger.LogInformation($"Primary {primaryName} with ID {primaryId} found");

                foreach (var fakeName in fakes)
                {
                    var fakeTug = dbContext.AuxTugs.FirstOrDefault(t => t.NameTug == fakeName && t.IdTug != primaryId);
                    if (fakeTug != null)
                    {
                        UpdateAndDeleteFakeTugs(dbContext, primaryId, fakeTug.IdTug); // Pass fake ID instead of fake name
                    }
                    else
                    {
                        _logger.LogWarning($"No match found for fake {fakeName}");
                    }
                }
            }
            else
            {
                _logger.LogWarning($"No match found for primary with ID {primaryId} and name {primaryName}");
            }
        }
        private void UpdateAndDeleteFakeTugs(ScriptDbContext dbContext, int primaryId, int fakeId)
        {
            var fakeTug = dbContext.AuxTugs.FirstOrDefault(t => t.IdTug == fakeId);
            if (fakeTug != null)
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // Update movement tugs
                        var movementTugsToUpdate = dbContext.AuxMovementTugs.Where(mt => mt.IdTug == fakeId).ToList();
                        foreach (var mt in movementTugsToUpdate)
                        {
                            mt.IdTug = (uint)primaryId;
                        }

                        // Save changes for movement tugs
                        dbContext.SaveChanges();

                        // Remove fake tug
                        dbContext.AuxTugs.Remove(fakeTug);

                        // Save changes for removing fake tug
                        dbContext.SaveChanges();

                        transaction.Commit();
                        _logger.LogInformation($"Updated and deleted fake {fakeTug.NameTug} with ID {fakeId}");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, $"Error occurred while updating and deleting fake tug with ID {fakeId}");
                        throw;
                    }
                }
            }
            else
            {
                _logger.LogWarning($"No match found for fake with ID {fakeId}");
            }
        }








    }
}