namespace AAP.Application.DTOs
{
    public class RedditResponse
    {
        public RedditRootData Data { get; set; } = new();
    }

    public class RedditRootData
    {
        public List<RedditChild> Children { get; set; } = new();
    }

    public class RedditChild
    {
        public RedditPostData Data { get; set; } = new();
    }

    public class RedditPostData
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        public double Created_Utc { get; set; }
    }
}