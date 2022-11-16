using BlackJack.Users.Abstractions.DataTransferObjects;

namespace BlackJack.Users.Abstractions.Repositories;

public interface IBlackJackUsersRepository
{
    Task<bool> CreateAsync(Guid userId);
    Task<UserDto?> GetAsync(Guid userId);
}