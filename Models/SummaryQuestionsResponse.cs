namespace AISummarizerMVC.Models
{
    public class SummaryQuestionsResponse
    {
        public string Summary { get; set; } = string.Empty;
        public List<string> Questions { get; set; } = new();
    }
}
