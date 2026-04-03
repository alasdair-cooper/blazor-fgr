using BlazorFgr.Core.Primitives;
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

    extension<T>(ResourceValue<T> value)
    {
        public ResourceValue<(T, TOther)> And<TOther>(ResourceValue<TOther> other) =>
            (value, other) switch
            {
                (ResourceValue<T>.Loaded(var a), ResourceValue<TOther>.Loaded(var b)) => new ResourceValue<(T, TOther)>.Loaded((a, b)),
                (ResourceValue<T>.Errored(var e1), ResourceValue<TOther>.Errored(var e2)) => new ResourceValue<(T, TOther)>.Errored(
                    new AggregateException(e1, e2)),
                (ResourceValue<T>.Errored(var e), _) => new ResourceValue<(T, TOther)>.Errored(e),
                (_, ResourceValue<TOther>.Errored(var e)) => new ResourceValue<(T, TOther)>.Errored(e),
                _ => new ResourceValue<(T, TOther)>.Loading()
            };

        public ResourceValue<TResult> Map<TResult>(Func<T, TResult> func) =>
            value switch
            {
                ResourceValue<T>.Loaded(var a) => new ResourceValue<TResult>.Loaded(func(a)),
                ResourceValue<T>.Errored(var e) => new ResourceValue<TResult>.Errored(e),
                _ => new ResourceValue<TResult>.Loading()
            };
    }

    extension<T1, T2>((IGettable<ResourceValue<T1>>, IGettable<ResourceValue<T2>>) value)
    {
        public ResourceValue<(T1, T2)> Transpose() => value.Item1.Value.And(value.Item2.Value);
    }

    extension<T1, T2, T3>((IGettable<ResourceValue<T1>>, IGettable<ResourceValue<T2>>, IGettable<ResourceValue<T3>>) value)
    {
        public ResourceValue<(T1, T2, T3)> Transpose() =>
            value.Item1.Value.And(value.Item2.Value).And(value.Item3.Value).Map(x => (x.Item1.Item1, x.Item1.Item2, x.Item2));
    }

    extension<T1, T2, T3, T4>(
        (IGettable<ResourceValue<T1>>, IGettable<ResourceValue<T2>>, IGettable<ResourceValue<T3>>, IGettable<ResourceValue<T4>>) value)
    {
        public ResourceValue<(T1, T2, T3, T4)> Transpose() =>
            value.Item1
                .Value
                .And(value.Item2.Value)
                .And(value.Item3.Value)
                .And(value.Item4.Value)
                .Map(x => (x.Item1.Item1.Item1, x.Item1.Item1.Item2, x.Item1.Item2, x.Item2));
    }

    extension<T1, T2, T3, T4, T5>(
        (IGettable<ResourceValue<T1>>, IGettable<ResourceValue<T2>>, IGettable<ResourceValue<T3>>, IGettable<ResourceValue<T4>>,
            IGettable<ResourceValue<T5>>) value)
    {
        public ResourceValue<(T1, T2, T3, T4, T5)> Transpose() =>
            value.Item1
                .Value
                .And(value.Item2.Value)
                .And(value.Item3.Value)
                .And(value.Item4.Value)
                .And(value.Item5.Value)
                .Map(x => (x.Item1.Item1.Item1.Item1, x.Item1.Item1.Item1.Item2, x.Item1.Item1.Item2, x.Item1.Item2, x.Item2));
    }
}