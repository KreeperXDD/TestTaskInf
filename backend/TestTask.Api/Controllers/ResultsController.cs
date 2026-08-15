using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/results")]
public class ResultsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public ResultsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Result>>> GetResult(
        [FromQuery] string? fileName,
        [FromQuery] DateTime? firstOperationDateFrom,
        [FromQuery] DateTime? firstOperationDateTo,
        [FromQuery] double? avarageMetricValueFrom,
        [FromQuery] double? avarageMetricValueTo,
        [FromQuery] double? avarageExecutionTimeFrom,
        [FromQuery] double? avarageExecutionTimeTo)
    {
        
        if(firstOperationDateFrom >firstOperationDateTo)
        {
            return BadRequest("Base date cant be later than end date");
        }

        if(avarageMetricValueFrom > avarageMetricValueTo)
        {
            return BadRequest("avarage min value cant be bigest than max value");
        }

        if(avarageExecutionTimeFrom > avarageExecutionTimeTo)
        {
            return BadRequest("Min avarage time cant be bigest than max time");
        }
        
        IQueryable<Result> query = _context.Results.AsNoTracking();

        if(!string.IsNullOrWhiteSpace(fileName))
        {
            query = query.Where(result => result.FileName.Contains(fileName));
        }

        if(firstOperationDateFrom.HasValue)
        {
            query = query.Where(result => result.FirstOperationDate >= firstOperationDateFrom.Value);
        }

        if(firstOperationDateTo.HasValue)
        {
            query = query.Where(result => result.FirstOperationDate <= firstOperationDateTo.Value);
        }        

        if(avarageMetricValueFrom.HasValue)
        {
            query = query.Where(result => result.AvarageMetricValue >= avarageMetricValueFrom.Value);
        }

        if(avarageMetricValueTo.HasValue)
        {
            query = query.Where(result => result.AvarageMetricValue <= avarageMetricValueTo.Value);
        }

        if(avarageExecutionTimeFrom.HasValue)
        {
            query = query.Where(result => result.AvarageExecutionTime >= avarageExecutionTimeFrom.Value);
        }
        
        if(avarageExecutionTimeTo.HasValue)
        {
            query = query.Where(result => result.AvarageExecutionTime <= avarageExecutionTimeTo.Value);
        }

        var results = await query
        .OrderByDescending(result => result.FirstOperationDate)
        .ToListAsync();

        return Ok(results);
    }

}