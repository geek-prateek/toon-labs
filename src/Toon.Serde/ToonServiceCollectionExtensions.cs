using Microsoft.Extensions.DependencyInjection;

namespace Toon.Serde
{
    public static class ToonServiceCollectionExtensions
    {
        public static IServiceCollection AddToonHttpClient(this IServiceCollection services, string name = "toon", ToonOptions? options = null)
        {
            services.AddHttpClient(name)
                    .ConfigurePrimaryHttpMessageHandler(() => new ToonHttpClientHandler(options));
            return services;
        }
    }
}
