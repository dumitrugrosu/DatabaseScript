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
                        string primaryName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                        string fakesString = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        List<string> fakes = fakesString.Split(',').Select(f => f.Trim()).ToList();
                        ProcessPrimaryBarge(dbContext, primaryName, fakes);

                    }
                    dbContext.SaveChanges();
                }
            }
        }
        private void ProcessPrimaryBarge(ScriptDbContext dbContext,  string primaryName, List<string>fakes)
        {
            var primaryBarge = dbContext.AuxBarges.FirstOrDefault(p => p.Barge == primaryName);
            if (primaryBarge != null)
            {
                int primaryId = primaryBarge.IdBarge;
                string primaryNameDb = primaryBarge.Barge;
                _logger.LogInformation($"Primary {primaryNameDb} found with id_barge: {primaryId}");
                foreach (var fakeName in fakes)
                {
                    UpdateAndDeleteFakeBarges(dbContext, primaryId, fakeName);
                }
            }
            else
            {
                _logger.LogInformation($"Primary {primaryName} not found");
            }
        }
        private void UpdateAndDeleteFakeBarges(ScriptDbContext dbContext, int primaryId, string fakeName)
        {
            var fakeBarge = dbContext.AuxBarges.FirstOrDefault(p => p.Barge == fakeName);

            if (fakeBarge != null)
            {
                var movementBargesToUpdate = dbContext.AuxMovements.Where(m => m.BargeField == fakeBarge.IdBarge);
                foreach (var m in movementBargesToUpdate)
                {
                    m.BargeField = (uint?)primaryId;
                }
                dbContext.AuxBarges.Remove(fakeBarge);
                _logger.LogInformation($"Fake {fakeName} updated with primary_id: {primaryId}");
                _logger.LogInformation($"Fake {fakeName} deleted");
                dbContext.SaveChanges();

            }
            else
            {
                _logger.LogWarning($"No match found for fake {fakeName}");
            }
        }
    }
}
