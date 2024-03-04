using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using DatabaseScript.Context;

namespace DatabaseScript.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TugsController : ControllerBase
    {
        private readonly ILogger<TugsController> _logger;
        private readonly ScriptDbContext _dbContext;
        private readonly string _tugsFilePath;

        public TugsController(ILogger<TugsController> logger, ScriptDbContext dbContext, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _logger = logger;
            _dbContext = dbContext;
            _tugsFilePath = config["FilePaths:TugsFile"];
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

                    ProcessPrimaryTug(primaryName, fakes);
                }

                _dbContext.SaveChanges();
            }
        }

        private void ProcessPrimaryTug(string primaryName, List<string> fakes)
        {
            var primaryTug = _dbContext.Tugs.FirstOrDefault(t => t.Name == primaryName);

            if (primaryTug != null)
            {
                int primaryId = primaryTug.Id;
                string primaryNameDb = primaryTug.Name;

                _logger.LogInformation($"Primary {primaryNameDb} found with id_tug: {primaryId}");

                foreach (var fakeName in fakes)
                {
                    UpdateAndDeleteFakeTugs(primaryId, fakeName);
                }
            }
            else
            {
                _logger.LogWarning($"No match found for primary {primaryName}");
            }
        }

        private void UpdateAndDeleteFakeTugs(int primaryId, string fakeName)
        {
            var fakeTug = _dbContext.Tugs.FirstOrDefault(t => t.Name == fakeName);

            if (fakeTug != null)
            {
                int fakeId = fakeTug.Id;

                var movementTugsToUpdate = _dbContext.Tugs.Where(mt => mt.Id == fakeId);
                foreach (var mt in movementTugsToUpdate)
                {
                    mt.Id = primaryId;
                }

                _dbContext.Tugs.Remove(fakeTug);

                _logger.LogInformation($"Updated aux_movement_tugs for fake {fakeName}");
                _logger.LogInformation($"Deleted fake {fakeName} from aux_tugs");
            }
            else
            {
                _logger.LogWarning($"No match found for fake {fakeName}");
            }
        }
    }
}
