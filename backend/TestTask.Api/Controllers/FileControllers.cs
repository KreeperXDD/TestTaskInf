using CsvHelper;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/files")]
public class FileControllers : ControllerBase
{
    private readonly CsvImportService _csvImportService;

    public FileControllers(CsvImportService csvImportService)
    {
        _csvImportService = csvImportService;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<CsvImportResult>> Upload(IFormFile file)
    {
        if(file is null || file.Length ==0)
        {
            return BadRequest("Choice not null file");
        }
        
        if(!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Can be load inly csv file");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            
            var result = await _csvImportService.ImportAsync(stream, Path.GetFileName(file.FileName));

            return Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (CsvHelperException)
        {
            return BadRequest("csv-file is not correct or collumns value is not correct");
        }
    }
}