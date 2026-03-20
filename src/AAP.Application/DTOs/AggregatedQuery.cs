namespace AAP.Application.DTOs
{
    public class AggregatedQuery
    {
        public string? SortBy { get; set; }    
        public string? SortDirection { get; set; } 
        public string? Author { get; set; }          
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
