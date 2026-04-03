using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFgr.Core;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddBlazorFgr(Action<FgrOptions>? configure = null)
        {
            services.AddOptions<FgrOptions>();

            if (configure is not null)
            {
                services.Configure(configure);
            }
            
            return services;
        }
    }

    extension(ResourceAssetCollection assets)
    {
        public string BlazorFgrLibCss => assets["_content/BlazorFgr.Core/lib.css"];
    }
}