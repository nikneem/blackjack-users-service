using BlackJack.Users.Abstractions.DataTransferObjects;

namespace BlackJack.Users.Abstractions.Services;

public interface IBlackJackUsersService
{
    Task<UserDto> Create();
    Task<UserDto> Restore(Guid userId);
}