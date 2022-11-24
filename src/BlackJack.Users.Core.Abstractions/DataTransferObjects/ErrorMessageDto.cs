namespace BlackJack.Users.Functions.DataTransferObjects;

public class ErrorMessageDto
{
    public string ErrorMessage { get; set; }
    public string ErrorCode { get; set; }
    public string TranslationKey { get; set; }
    public List<ErrorSubstituteDto> Substitutions { get; set; }
}