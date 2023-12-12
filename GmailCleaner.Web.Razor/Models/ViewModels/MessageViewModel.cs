namespace GmailCleaner.Models.ViewModels;

/// <summary>
///  Having issues passing the "from" field from html to the post method, it keeps getting presented in the html wrong, encoding the < and > characters which breaks the from email address.
///  So instead using a view model.
/// </summary>

public record MessageViewModel
{
    public string From { get; set; } = string.Empty;
    //Message Id  I am using to tie back to the From email address. Arbitrary but I can not display the from email address in the html without it getting encoded.
    public string LinkingMessageId { get; set; } = string.Empty;
    public List<string> MessageGmailIds { get; set; } = new();
    public bool Delete { get; set; } = false;

}
