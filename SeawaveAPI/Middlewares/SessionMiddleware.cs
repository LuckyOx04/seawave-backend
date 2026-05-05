using Services;

namespace SeawaveAPI.Middlewares;

public class SessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, AuthService authService)
    {
        string? token = context.Request.Headers.Authorization;

        if (!string.IsNullOrEmpty(token))
        {
            var userId = await authService.ValidateSessionAsync(token);

            if (userId.HasValue)
            {
                context.Items["UserId"] = userId.Value;
            }

            await next(context);
        }
    }
}