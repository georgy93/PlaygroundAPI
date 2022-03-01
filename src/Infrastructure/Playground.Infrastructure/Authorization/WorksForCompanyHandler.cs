namespace Playground.Infrastructure.Authorization
{
    using Microsoft.AspNetCore.Authorization;
    using System.Security.Claims;

    public class WorksForCompanyHandler : AuthorizationHandler<WorksForCompanyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WorksForCompanyRequirement requirement)
        {
            var userEmailAddres = context.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            if (userEmailAddres.EndsWith(requirement.DomainName))
                context.Succeed(requirement);
            else
                context.Fail(); // no other handler will succeed             

            return Task.CompletedTask;
        }
    }
}