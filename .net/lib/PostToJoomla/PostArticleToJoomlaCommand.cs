namespace PostToJoomla;

public class PostArticleToJoomlaCommand: CommonCommand
{
    public string? Title { get; set; }
    public string? CatId { get; set; }
    public string? ArticleText { get; set; }
    public string? TokenName { get; set; }
    public string? Url { get; set; }
    public bool IsSaved { get; set; }
}