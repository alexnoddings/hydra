using hydra.Config;
using hydra.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace hydra.Endpoints;

public static class GetEndpoint
{
    public static async Task<Results<RedirectHttpResult, NotFound, BadRequest>> ExecuteAsync(
        [FromServices] IHydraConfigProvider configProvider,
        [FromServices] IHydraHttpContext httpContext
    )
    {
        var component = httpContext.Host;
        if (component is null)
            return TypedResults.BadRequest();

        var config = await configProvider.GetConfigAsync();

        var componentConfig = config[component];
        if (componentConfig is null)
            return TypedResults.NotFound();

        var slug = httpContext.Path;
        var redirectTo = componentConfig[slug];
        if (redirectTo is null)
            return TypedResults.NotFound();
        
        return TypedResults.Redirect(redirectTo);
    }

    public static IEndpointRouteBuilder MapGetEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/{**slug}", ExecuteAsync);
        return builder;
    }
}
