using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/values")]
public class ValuesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ValuesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("latest")]
    public async Task<ActionResult<List<Value>>> GetLatestValues([FromQuery] string? fileName)
    {
        if(string.IsNullOrWhiteSpace(fileName))
        {
            return BadRequest("Specify the filename in the fileName");
        }

        var values = await _context.Values
        .AsNoTracking()
        .Where(value => value.FileName == fileName)
        .OrderByDescending(value => value.Date)
        .ThenByDescending(value => value.Id)
        .Take(10)
        .ToListAsync();

        return Ok(values);
    }
}