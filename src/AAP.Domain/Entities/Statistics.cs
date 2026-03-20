namespace AAP.Domain.Entities
{
    public class Statistics
    {
        public string ApiName { get; set; } = string.Empty;

        public int TotalRequests { get; set; }

        public double AverageResponseTime { get; set; }

        public int FastRequests { get; set; }

        public int MediumRequests { get; set; }

        public int SlowRequests { get; set; }
    }
}