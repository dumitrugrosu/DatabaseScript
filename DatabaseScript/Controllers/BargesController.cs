using Microsoft.AspNetCore.Mvc;
using DatabaseScript.Context;
using OfficeOpenXml;
using System.Diagnostics;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BargesController : ControllerBase
    {
        private readonly ILogger<BargesController> _logger;
        private readonly ScriptDbContext _dbContext;
        private readonly string _bargesFilePath;

        public BargesController(ILogger<BargesController> logger, ScriptDbContext dbContext, IConfiguration config)
        {
            _logger = logger;
            _dbContext = dbContext;
            _bargesFilePath = config["FilePath:BargesFile"];
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
                Debug.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while processing the file");
            }
        }
        private void ProcessExcelFile(string filePath)
        {
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
                    ProcessPrimaryBarge(primaryName, fakes);

                }
            }
        }
        private void ProcessPrimaryBarge(string primaryName, List<string>fakes)
        {
            var primaryBarge = _dbContext.Barges.FirstOrDefault(p => p.Name == primaryName);
            if (primaryBarge == null)
            {
                int primaryId = primaryBarge.Id;
                string primaryNameDb = primaryBarge.Name;
                _logger.LogInformation($"Primary {primaryNameDb} found with id_barge: {primaryId}");
                foreach (var fakeName in fakes)
                {
                    UpdateAndDeleteFakeBarges(primaryId, fakeName);
                }
            }
            else
            {
                _logger.LogInformation($"Primary {primaryName} not found");
            }
        }
        private void UpdateAndDeleteFakeBarges(int primaryId, string fakeName)
        {
            var fakeBarge = _dbContext.Barges.FirstOrDefault(p => p.Name == fakeName);
            if (fakeBarge != null)
            {
                int fakeId = fakeBarge.Id;
                var movementBargesToUpdate = _dbContext.Barges.Where(mt => mt.Id == fakeId);
                foreach (var mt in movementBargesToUpdate)
                {
                    mt.Id = primaryId;
                }
                _dbContext.Barges.Update(fakeBarge);
                _dbContext.SaveChanges();
                _logger.LogInformation($"Fake {fakeName} updated with primary_id: {primaryId}");
                _logger.LogInformation($"Fake {fakeName} deleted");
            }
            else
            {
                _logger.LogWarning($"No match found for fake {fakeName}");
            }
        }
    }
}
