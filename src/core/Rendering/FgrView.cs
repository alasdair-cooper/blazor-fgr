using BlazorFgr.Core.Primitives;
using BlazorFgr.Core.Primitives.Effect;
using BlazorFgr.Core.Primitives.Signal;
using Microsoft.AspNetCore.Components;

namespace BlazorFgr.Core.Rendering;

public class FgrView<T> : IComponent where T : IEquatable<T>
{
    [Parameter]
    [EditorRequired]
    public IGettable<T> Value { get; set; }

    [Parameter]
    public RenderFragment<T>? ChildContent { get; set; }

    private RenderHandle _renderHandle;

    public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        var effect =
            new Effect(
                () =>
                {
                    var value = Value.Get();

                    _renderHandle.Dispatcher.InvokeAsync(
                        () =>
                        {
                            _renderHandle.Render(
                                x =>
                                {
                                    switch (value)
                                    {
                                        case var _ when ChildContent is { } childContent:
                                            x.AddContent(0, childContent(value));
                                            break;
                                        case RenderFragment fragment:
                                            x.AddContent(0, fragment);
                                            break;
                                        case MarkupString markup:
                                            x.AddMarkupContent(0, markup.Value);
                                            break;
                                        case var _:
                                            x.AddContent(0, value);
                                            break;
                                    }
                                });

                            return Task.CompletedTask;
                        });
                });

        effect.Invalidate();

        return Task.CompletedTask;
    }

    public static RenderFragment FromSignal(Signal<T> signal) =>
        builder =>
        {
            builder.OpenComponent<FgrView<T>>(0);
            builder.AddComponentParameter(1, nameof(FgrView<>.Value), signal);
            builder.CloseComponent();
        };
}