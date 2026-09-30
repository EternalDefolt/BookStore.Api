using Asp.Versioning.ApiExplorer;

namespace BookStore.Api.Middleware;

public class DeprecationMiddleware
{
    private static readonly DateTimeOffset SunsetDate =
        new(2026, 12, 31, 23, 59, 59, TimeSpan.Zero);

    private readonly RequestDelegate _next;
    private readonly IApiVersionDescriptionProvider _versionDescriptions;

    public DeprecationMiddleware(RequestDelegate next, IApiVersionDescriptionProvider versionDescriptions)
    {
        _next = next;
        _versionDescriptions = versionDescriptions;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(AddDeprecationHeaders, context);
        await _next(context);
    }

    private Task AddDeprecationHeaders(object state)
    {
        var context = (HttpContext)state;
        var version = context.GetRequestedApiVersion();

        if (version is not null)
        {
            var description = _versionDescriptions.ApiVersionDescriptions
                .FirstOrDefault(d => d.ApiVersion == version);

            if (description is { IsDeprecated: true })
            {
                context.Response.Headers["Deprecation"] = "true";
                context.Response.Headers["Sunset"] = SunsetDate.ToString("R");
            }
        }

        return Task.CompletedTask;
    }
}
