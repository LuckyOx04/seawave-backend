namespace SeawaveAPI.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (FormatException ex)
        {
            await WriteError(context, 400, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteError(context, 401, ex.Message);
        }
        catch (FileNotFoundException ex)
        {
            await WriteError(context, 404, ex.Message);
        }
        catch (Exception ex)
        {
            await WriteError(context, 500, ex.Message);
        }
    }

    private static Task WriteError(HttpContext context, int code, string msg)
    {
        context.Response.StatusCode = code;
        return context.Response.WriteAsJsonAsync(new { message = msg });
    }
}