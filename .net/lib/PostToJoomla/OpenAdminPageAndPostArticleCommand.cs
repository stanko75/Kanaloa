namespace PostToJoomla;

public class OpenAdminPageAndPostArticleCommand
{
    public string? LoginUrl { get; set; }
    public string? UserName { get; set; }
    public string? Pass { get; set; }
    public string? PostUrl { get; set; }
    public bool IsSaved { get; set; } = false;
    public string? Title { get; set; }
    public string? CategoryId { get; set; }
    public string ArticleText { get; set; }
}