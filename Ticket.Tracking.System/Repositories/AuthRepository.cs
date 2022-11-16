using Ticket.Tracking.System.RepositoryInterfaces;

namespace Ticket.Tracking.System.Repositories;

public class AuthRepository : IAuthRepository
{

    //public UserAccountRepository(PostgresDbContext dbContext) : base(dbContext) { }

    //public Task<List<UserAccount>> GetAllAsync()
    //{
    //    return _dbContext.UserAccounts!.Where(x => x.IsAdmin == false).ToListAsync();
    //}

    //public Task<UserAccount?> GetByNameAsync(string username, CancellationToken ct = default)
    //{
    //    return _dbContext.UserAccounts!.FirstOrDefaultAsync(x => x.UserName!.Equals(username), ct);
    //}

    //public async Task<string> GetEmpIdAsync()
    //{
    //    var users = await GetAllAsync();
    //    return string.Join(", ", users.Select(i => i.EmpId));
    //}
}
