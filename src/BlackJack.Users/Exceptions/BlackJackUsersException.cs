using BlackJack.Core.Exceptions;
using BlackJack.Users.ErrorCodes;

namespace BlackJack.Users.Exceptions;

public class BlackJackUsersException : BlackJackException
{
    public BlackJackUsersException(BlackJackUsersErrorCode errorCode, string message, Exception? ex = null) : base(errorCode, message, ex)
    {
    }
}