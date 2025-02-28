namespace Application.Exceptions;

public class CustomValidationException : Exception
{
    public List<string> ErrorMessages { get; set; }
    public string FriendlyErrorMessage { get; set; }
    public CustomValidationException(List<string> errorMessage, string friendlyMessage) :
        base(friendlyMessage)
    {
        ErrorMessages = errorMessage;
        FriendlyErrorMessage = friendlyMessage;
    }
}