namespace OrdersMicroservice.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger
        )
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                logger.LogError("{ExceptionType} {ExceptionMessage}",
                    ex.InnerException.GetType().ToString(),
                    ex.InnerException.Message);
            }
            else
            {
                logger.LogError("{ExceptionType} {ExceptionMessage}",
                    ex.GetType().ToString(),
                    ex.Message);
            }

            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsJsonAsync(new { Message = ex.Message, Type = ex.GetType().ToString() });
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}