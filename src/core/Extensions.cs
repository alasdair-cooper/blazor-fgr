using BlazorFgr.Core.Primitives.Resource;
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

    extension<T1, T2>((ResourceValue<T1>, ResourceValue<T2>) value)
    {
        public ResourceValue<(T1, T2)> Transpose() =>
            value switch
            {
                (ResourceValue<T1>.Loaded(var a), ResourceValue<T2>.Loaded(var b)) => new ResourceValue<(T1, T2)>.Loaded((a, b)),
                (ResourceValue<T1>.Errored(var e1), ResourceValue<T2>.Errored(var e2)) => new ResourceValue<(T1, T2)>.Errored(new AggregateException(e1, e2)),
                (ResourceValue<T1>.Errored(var e), _) => new ResourceValue<(T1, T2)>.Errored(e),
                (_, ResourceValue<T2>.Errored(var e)) => new ResourceValue<(T1, T2)>.Errored(e),
                _ => new ResourceValue<(T1, T2)>.Loading()
            };
    }
}