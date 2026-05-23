namespace PostToJoomla;

public class OpenJoomlaAdminPageCommand: CommonCommand
{
    public string? AdministratorUrl { get; set; }
    public string? TokenName { get; set; }
    public bool Ok { get; set; }
}