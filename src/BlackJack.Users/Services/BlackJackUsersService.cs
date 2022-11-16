using BlackJack.Users.Abstractions.DataTransferObjects;
using BlackJack.Users.Abstractions.Repositories;
using BlackJack.Users.Abstractions.Services;
using BlackJack.Users.ErrorCodes;
using BlackJack.Users.Exceptions;
using Microsoft.Extensions.Logging;

namespace BlackJack.Users.Services;

public class BlackJackUsersService : IBlackJackUsersService
{
    private readonly IBlackJackUsersRepository _repository;
    private readonly ILogger<BlackJackUsersService> _logger;

    public async Task<UserDto> Create()
    {
        var userId = Guid.NewGuid();
        _logger.LogTrace("New user GUID {userId} generated", userId);
        try
        {
            if (await _repository.CreateAsync(userId))
            {
                _logger.LogInformation("Created a new user entry with ID {userId}", userId);
                return new UserDto
                {
                    UserId = userId
                };
            }
        }
        catch (Exception ex)
        {
            throw new BlackJackUsersException(BlackJackUsersErrorCode.FailedToCreateUser, "An exception occured while trying to create a new user", ex);
        }

        _logger.LogInformation("Failed to create User ID, user will receive an exception");
        throw new BlackJackUsersException(BlackJackUsersErrorCode.FailedToCreateUser, "An unknown error occured while trying to create a new user" );
    }

    public async Task<UserDto> Restore(Guid userId)
    {
        var user = await _repository.GetAsync(userId) ?? await Create();
        return user;
    }


    public BlackJackUsersService(
        IBlackJackUsersRepository repository,
        ILogger<BlackJackUsersService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

}