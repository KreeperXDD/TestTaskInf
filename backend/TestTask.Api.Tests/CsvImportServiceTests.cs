using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CsvImportServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly CsvImportService _service;

    public CsvImportServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _service = new CsvImportService(_context);
    }

    [Fact]
    public async Task ImportAsync_ValidCsv_CalculatesAndSavesStatistics()
    {
        const string csv = """
            Date,ExecutionTime,MetricValue
            2025-08-10T10:00:00,2,10
            2025-08-10T10:05:00,4,20
            2025-08-10T10:12:00,6,30
            """;

        using var stream = CreateStream(csv);

        var importResult = await _service.ImportAsync(stream, "test-data.csv");

        Assert.Equal("test-data.csv", importResult.FileName);
        Assert.Equal(3, importResult.RowsCount);
        Assert.Equal(new DateTime(2025, 8, 10, 10, 0, 0),
            importResult.Result.FirstOperationDate);

        Assert.Equal(720, importResult.Result.TimeDeltaSeconds);
        Assert.Equal(4, importResult.Result.AvarageExecutionTime);
        Assert.Equal(20, importResult.Result.AvarageMetricValue);
        Assert.Equal(20, importResult.Result.MedianMetricValue);
        Assert.Equal(10, importResult.Result.MinMetricValue);
        Assert.Equal(30, importResult.Result.MaxMetricValue);

        Assert.Equal(3, await _context.Values.CountAsync());
        Assert.Single(await _context.Results.ToListAsync());
    }

    [Fact]
    public async Task ImportAsync_EvenCountOfRows_CalculatesMedian()
    {
        const string csv = """
            Date,ExecutionTime,MetricValue
            2025-08-10T10:00:00,1,10
            2025-08-10T10:01:00,1,20
            2025-08-10T10:02:00,1,30
            2025-08-10T10:03:00,1,40
            """;

        using var stream = CreateStream(csv);

        var importResult = await _service.ImportAsync(stream, "median.csv");

        Assert.Equal(25, importResult.Result.MedianMetricValue);
    }

    [Fact]
    public async Task ImportAsync_SameFileName_ReplacesOldData()
    {
        const string firstCsv = """
            Date,ExecutionTime,MetricValue
            2025-08-10T10:00:00,2,10
            2025-08-10T10:05:00,4,20
            """;

        const string secondCsv = """
            Date,ExecutionTime,MetricValue
            2025-08-11T10:00:00,5,50
            2025-08-11T10:02:00,7,70
            """;

        using var firstStream = CreateStream(firstCsv);
        await _service.ImportAsync(firstStream, "same-file.csv");

        using var secondStream = CreateStream(secondCsv);
        var importResult = await _service.ImportAsync(secondStream, "same-file.csv");

        Assert.Equal(2, importResult.RowsCount);
        Assert.Equal(2, await _context.Values.CountAsync());
        Assert.Single(await _context.Results.ToListAsync());
        Assert.Equal(60, importResult.Result.AvarageMetricValue);
    }

    [Fact]
    public async Task ImportAsync_EmptyCsv_ThrowsArgumentException()
    {
        const string csv = "Date,ExecutionTime,MetricValue";

        using var stream = CreateStream(csv);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ImportAsync(stream, "empty.csv")
        );
    }

    [Fact]
    public async Task ImportAsync_NegativeMetricValue_ThrowsArgumentException()
    {
        const string csv = """
            Date,ExecutionTime,MetricValue
            2025-08-10T10:00:00,2,-10
            """;

        using var stream = CreateStream(csv);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ImportAsync(stream, "invalid.csv")
        );
    }

    private static MemoryStream
    CreateStream(string csv)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(csv));
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}