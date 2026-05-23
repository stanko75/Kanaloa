namespace PostToJoomla;

public class LoginToJoomlaCommand: CommonCommand
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? TokenName { get; set; }
    public string? Url { get; set; }
    public bool IsLoginSuccessful { get; set; }
}