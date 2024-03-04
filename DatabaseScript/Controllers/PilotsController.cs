using Microsoft.AspNetCore.Mvc;
using DatabaseScript.Context;
using OfficeOpenXml;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PilotsController : ControllerBase
   
    {
        private readonly ILogger<PilotsController> _logger;
        private readonly ScriptDbContext _dbContext;
        private readonly string _pilotsFilePath;

        public PilotsController(ILogger<PilotsController> logger, ScriptDbContext dbContext, IConfiguration config)
        {
            _logger = logger;
            _dbContext = dbContext;
            _pilotsFilePath = config["FilePath:PilotsFile"];
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
                    ProcessPrimaryPilot(primaryName, fakes);
                }
            }
        }
        private void ProcessPrimaryPilot(string primaryName, List<string> fakes)
        {
            var primaryPilot = _dbContext.Pilots.FirstOrDefault(p => p.Name == primaryName);
            if (primaryPilot != null)
            {
                int primaryId = primaryPilot.Id;
                string primaryNameDb = primaryPilot.Name;
                _logger.LogInformation($"Primary {primaryNameDb} found with id_pilot: {primaryId}");
                foreach (var fakeName in fakes)
                {
                    UpdateAndDeleteFakePilots(primaryId, fakeName);
                }
            }
            else
            {
                _logger.LogWarning($"No match found for primary {primaryName}");
            }   
        }
        private void UpdateAndDeleteFakePilots(int primaryId, string fakeName)
        {
            var fakePilot = _dbContext.Pilots.FirstOrDefault(p => p.Name == fakeName);
            if (fakePilot != null)
            {
                int fakeId = fakePilot.Id;
                var movementPilotsToUpdate = _dbContext.Pilots.Where(mt => mt.Id == fakeId);
                foreach (var mt in movementPilotsToUpdate)
                {
                    mt.Id = primaryId;
                }
                _dbContext.Pilots.Remove(fakePilot);
                _dbContext.SaveChanges();
                _logger.LogInformation($"Updated aux_movement_pilots for fake {fakeName}");
                _logger.LogInformation($"Deleted fake {fakeName} from aux_pilots");
            }
            else
            {
                _logger.LogWarning($"No match found for fake {fakeName}");
            }
        }
    }
}
