using Ticket.Tracking.System.Repositories;
using Ticket.Tracking.System.RepositoryInterfaces;

namespace Ticket.Tracking.System.Configurations;

public static class ServicesConfigurations
{
    public static IServiceCollection AddRepositoriesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<ITicketTrackingRepository, TicketTrackingRepository>();

        return services;
    }

    //public static IServiceCollection AddServicesConfiguration(this IServiceCollection services)
    //{
    //    services.AddScoped<IUserAccountService, UserAccountService>();
    //    services.AddScoped<ITokenFCMCleintService, TokenFCMClientService>();
    //    //services.AddScoped<IWarehouseService, WarehouseService>();

    //    return services;
    //}
}
