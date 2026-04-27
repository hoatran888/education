namespace SchoolSystem.Web.Services;

/// <summary>
/// Redirects unauthenticated requests to /account/login when Entra is not configured.
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

            if (!path.StartsWith("/account",    StringComparison.OrdinalIgnoreCase) &&
                !path.StartsWith("/_blazor",    StringComparison.OrdinalIgnoreCase) &&
                !path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase))
            {
                var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);
                context.Response.Redirect($"/account/login?returnUrl={returnUrl}");
                return;
            }
        }

        await _next(context);
    }
}
