using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ReadmeController : ControllerBase
{
    [HttpGet]
    public IActionResult GetReadmeFile()
    {
        var filePath = "DatabaseScript_Controllers_Readme.md"; // Path to your README file
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        return File(fileStream, "text/markdown", Path.GetFileName(filePath));
    }
}