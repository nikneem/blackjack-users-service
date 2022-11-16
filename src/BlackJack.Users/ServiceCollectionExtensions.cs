using BlackJack.Users.Abstractions.Repositories;
using BlackJack.Users.Abstractions.Services;
using BlackJack.Users.Repositories;
using BlackJack.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlackJack.Users;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlackJackUsers(this IServiceCollection services)
    {
        services.AddTransient<IBlackJackUsersService, BlackJackUsersService>();
        services.AddTransient<IBlackJackUsersRepository, BlackJackUsersRepository>();
        return services;
    }
}