using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SeawaveAPI.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class SessionAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Items["UserId"] == null)
        {
            context.Result = new UnauthorizedObjectResult(
                new { message = "Session expired or is invalid. Please log in again." });
            return;
        }
        
        await next();
    }
}