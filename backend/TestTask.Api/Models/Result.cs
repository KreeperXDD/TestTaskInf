public class Result
{
    public long Id {get; set; }

    public string FileName {get; set; } = string.Empty;

    public DateTime FirstOperationDate {get; set; }

    public double TimeDeltaSeconds {get; set; }
    
    public double AvarageExecutionTime {get; set; }

    public double AvarageMetricValue {get; set; }

    public double MedianMetricValue {get; set; }

    public double MaxMetricValue {get; set; }

    public double MinMetricValue {get; set; }
}