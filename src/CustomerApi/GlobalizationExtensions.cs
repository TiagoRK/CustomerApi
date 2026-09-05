using System.Globalization;

namespace CustomerApi.Extensions;

public static class GlobalizationExtensions
{
    public static IServiceCollection AddGlobalization(this IServiceCollection services, IConfiguration configuration)
    {
        var defaultCulture = configuration["Globalization:DefaultCulture"] ?? "en-US";
        var supportedCultures = configuration.GetSection("Globalization:SupportedCultures")
            .Get<string[]>() ?? ["en-US", "pt-BR"];

        var cultures = supportedCultures
            .Select(c => new CultureInfo(c))
            .ToList();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(defaultCulture);
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
            options.RequestCultureProviders =
            [
                new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider()
            ];
        });

        return services;
    }

    public static IApplicationBuilder UseGlobalization(this IApplicationBuilder app)
    {
        app.UseRequestLocalization();
        return app;
    }
}
