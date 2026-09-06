using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace CustomerApi.Controllers;

public static class RequestLocalizationConfiguration
{
    public static IServiceCollection AddRequestLocalization(this IServiceCollection services)
    {
        var supportedCultures = new[] { "en-US", "pt-BR" };
        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
            options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
        });
        return services;
    }
}
