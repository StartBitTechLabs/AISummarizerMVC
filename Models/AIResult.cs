namespace AISummarizerMVC.Models
{
    public class AIResult
    {
        public string Topic { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public List<string> Questions { get; set; } = new();
    }
}
