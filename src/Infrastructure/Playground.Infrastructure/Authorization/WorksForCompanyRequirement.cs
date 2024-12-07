namespace Playground.Infrastructure.Authorization;

using Microsoft.AspNetCore.Authorization;

public class WorksForCompanyRequirement : IAuthorizationRequirement
{
    public WorksForCompanyRequirement(string domainName)
    {
        DomainName = domainName;
    }

    public string DomainName { get; }
}