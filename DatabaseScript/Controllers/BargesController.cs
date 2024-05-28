using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using DatabaseScript.Models;


namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BargesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BargesController> _logger;
        private readonly string _bargesFilePath;
        private readonly string _connectionString;
        public BargesController(ILogger<BargesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _bargesFilePath = configuration["FilePaths:File"];
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        private ScriptDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScriptDbContext>();
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));

            return new ScriptDbContext(optionsBuilder.Options);
        }
        [HttpPost("UploadBarges")]
        public IActionResult ProcessUploadedFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File is empty");
                // Save the uploaded file to the server
                string filePath = Path.Combine(_bargesFilePath, file.FileName);
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
                        if (!int.TryParse(worksheet.Cells[row, 1].Value?.ToString().Trim(), out int primaryId))
                        {
                            _logger.LogWarning($"Invalid or missing ID at row {row}");
                            continue; // Skip this row if the ID is invalid
                        }
                        string primaryName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                        string fakesString = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        List<string> fakes = new List<string>(fakesString?.Split(',').Select(s => s.Trim()));
                        ProcessPrimaryBarge(dbContext, primaryId, primaryName, fakes);
                    }
                    dbContext.SaveChanges();
                }
            }
        }
        private void ProcessPrimaryBarge(ScriptDbContext dbContext, int primaryId, string primaryName, List<string> fakes)
        {
            var primaryBarge = 
                dbContext.AuxBarges.FirstOrDefault(b => b.IdBarge == primaryId && b.Barge == primaryName);
            if (primaryBarge != null)
            {
                _logger.LogInformation($"Primary {primaryName}  with ID {primaryId} found");
                foreach (var fakeName in fakes)
                {
                    var fakeBarge = dbContext.AuxBarges.FirstOrDefault(b => b.Barge == fakeName && b.IdBarge != primaryId);
                    if (fakeBarge != null)
                    {
                        UpdateAndDeleteFakeBarges(dbContext, primaryId, fakeBarge.IdBarge);
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
        private void UpdateAndDeleteFakeBarges(ScriptDbContext dbContext, int primaryId, int fakeId)
        {
            var fakeBarge = dbContext.AuxBarges.FirstOrDefault(b => b.IdBarge == fakeId);
            if (fakeBarge != null)
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var movementBargesToUpdate = dbContext.AuxMovements.Where(m => m.BargeField == fakeId).ToList();
                        foreach (var m in movementBargesToUpdate)
                        {
                            m.BargeField = (uint)primaryId;
                        }
                        dbContext.SaveChanges();
                        dbContext.AuxBarges.Remove(fakeBarge);
                        dbContext.SaveChanges();
                        transaction.Commit();
                        _logger.LogInformation($"Updated and deleted fake {fakeBarge.Barge} with ID {fakeId}");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, $"Error updating and deleting fake {fakeBarge.Barge} with ID {fakeId}");
                        throw;
                    }
                }
            }
            else
            {
                _logger.LogWarning($"No match found for fake {fakeId}");
            }
        }
    }
}
