namespace PostToJoomla;

public class OpenAddArticleCommand: CommonCommand
{
    public string? AddArticleUrl { get; set; }
    public string? TokenName { get; set; }
    public bool Ok { get; set; }
}