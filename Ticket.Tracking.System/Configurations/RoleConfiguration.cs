using Microsoft.AspNetCore.Identity;

namespace Unt.Pinking.DataPostgres.Configurations;

public class RoleConfiguration
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleConfiguration(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
        _ = CreateRole("Admin");
        _ = CreateRole("RD");
        _ = CreateRole("QA");
    }

    public async Task CreateRole(string name)
    {
        var roleExist = await _roleManager.RoleExistsAsync(name);
        if (!roleExist)
        {
            await _roleManager.CreateAsync(new IdentityRole(name));
        }
    }
}
