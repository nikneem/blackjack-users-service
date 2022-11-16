namespace BlackJack.Users.ErrorCodes;

public class BlackJackUserFailedToCreateErrorCode : BlackJackUsersErrorCode
{
    public override string Code => GetType().Name;
}