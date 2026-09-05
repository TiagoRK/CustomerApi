using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace CustomerApi.Web.Configurations;

public static class LocalizationConfigs
{
    private static readonly string[] _supportedCultures = ["en", "en-US", "pt-BR"];

    public static IServiceCollection AddLocalizationConfigs(this IServiceCollection services)
    {
        services.AddLocalization();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var cultures = _supportedCultures
                .Select(c => new CultureInfo(c))
                .ToList();

            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
            options.FallBackToParentCultures = true;
            options.FallBackToParentUICultures = true;
        });

        return services;
    }

    public static IApplicationBuilder UseLocalizationConfigs(this IApplicationBuilder app)
    {
        app.UseRequestLocalization();
        return app;
    }
}
