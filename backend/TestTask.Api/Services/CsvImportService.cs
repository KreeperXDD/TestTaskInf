using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

public class CsvImportService
{
    private readonly AppDbContext _context;
    
    public CsvImportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CsvImportResult> ImportAsync(Stream fileStream, string fileName)
    {
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            PrepareHeaderForMatch = args => args.Header.Trim()
        };

        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, configuration);
        
        var csvRows = csv.GetRecords<CsvRow>().ToList();

        ValidateRows(csvRows);

        var values = csvRows
        .Select(row => new Value
        {
            FileName = fileName,
            Date = row.Date,
            ExecutionTime = row.ExecutionTime,
            MetricValue = row.MetricValue
        }).ToList();

        var orderMetricValues = values
        .Select(value => value.MetricValue)
        .OrderBy(value => value)
        .ToList();

        var result = new Result
        {
            FileName = fileName,
            FirstOperationDate = values.Min(value => value.Date),
            TimeDeltaSeconds = (
                values.Max(value => value.Date) - values.Min(value => value.Date)
            ).TotalSeconds,

            AvarageExecutionTime = values.Average(value => value.ExecutionTime),
            AvarageMetricValue = values.Average(value => value.MetricValue),
            MedianMetricValue = CalculateMedian(orderMetricValues),
            MaxMetricValue = values.Max(value => value.MetricValue),
            MinMetricValue = values.Min(value => value.MetricValue)
        };

        await using var transition = await _context.Database.BeginTransactionAsync();

        var oldValues = _context.Values.Where(value => value.FileName == fileName);

        _context.Values.RemoveRange(oldValues);

        var oldResults = await _context.Results.FirstOrDefaultAsync(result => result.FileName == fileName);

        if(oldResults is not null)
        {
            _context.Results.Remove(oldResults);
        }

        await _context.Values.AddRangeAsync(values);
        await _context.Results.AddAsync(result);

        await _context.SaveChangesAsync();;
        await transition.CommitAsync();

        return new CsvImportResult
        {
            FileName = fileName,
            RowsCount = values.Count,
            Result = result
        };
    }

    private static void ValidateRows(List<CsvRow> rows)
    {
        if (rows.Count ==0)
        {
            throw new ArgumentException("Csv file is not validate");
        }

        if (rows.Count > 10_000)
        {
            throw new ArgumentException("Csv file is biggest then 10000 rows");
        }

        foreach (var row in rows)
        {
            if (row.Date < new DateTime(2000, 1, 1))
            {
                throw new ArgumentException("Date is not can be earlier 01.01.2002");
            }

            if (row.Date > DateTime.Now)
            {
                throw new ArgumentException("Date is cant be after now date");
            }

            if (row.ExecutionTime < 0)
            {
                throw new ArgumentException("ExecutionTime cant be negative");

            }

            if (row.MetricValue < 0)
            {
                throw new ArgumentException("MetricValue cant be negative");
            }
        }
    }

    private static double CalculateMedian(List<double> sortedValues)
    {
        var middleIndex = sortedValues.Count/2;

        if (sortedValues.Count %2 == 1)
        {
            return sortedValues[middleIndex];
        }

        return(sortedValues[middleIndex-1] + sortedValues[middleIndex])/2;
    }
}

public class CsvRow
{
    public DateTime Date {get; set;}

    public double ExecutionTime {get; set; }

    public double MetricValue {get; set; }
}

public class CsvImportResult
{
    public string FileName {get;  set; } = string.Empty;

    public int RowsCount {get; set; }

    public Result Result {get; set; } = null!;
}