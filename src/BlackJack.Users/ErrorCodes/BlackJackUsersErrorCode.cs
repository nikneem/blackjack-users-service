
using BlackJack.Core.ErrorCodes;

namespace BlackJack.Users.ErrorCodes;

public abstract class BlackJackUsersErrorCode : BlackJackErrorCode
{
    public static readonly BlackJackUsersErrorCode FailedToCreateUser = new BlackJackUserFailedToCreateErrorCode();
    public override string ErrorNamespace => "Errors.Users";
}
