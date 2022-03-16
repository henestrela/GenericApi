using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Authorization
{
    public class AllowAnonymousHandlerDeveloper : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (IAuthorizationRequirement requirement in context.PendingRequirements.ToList())
            {
                context.Succeed(requirement); //Simply pass all requirements
            }

            return Task.CompletedTask;
        }

    }
}
