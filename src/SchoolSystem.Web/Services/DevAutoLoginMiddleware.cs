namespace SchoolSystem.Web.Services;

/// <summary>
/// Redirects unauthenticated requests to /dev-login when B2C is not configured.
/// </summary>
public class DevAutoLoginMiddleware
{
    private readonly RequestDelegate _next;

    public DevAutoLoginMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!(context.User.Identity?.IsAuthenticated ?? false))
        {
            var path = context.Request.Path.Value ?? "";

            // Let the dev-login controller handle its own routes
            if (!path.StartsWith("/dev-login", StringComparison.OrdinalIgnoreCase) &&
                !path.StartsWith("/_blazor",  StringComparison.OrdinalIgnoreCase) &&
                !path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase))
            {
                var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);
                context.Response.Redirect($"/dev-login?returnUrl={returnUrl}");
                return;
            }
        }

        await _next(context);
    }
}
