using BlazorFgr.Core.Primitives;
using Microsoft.AspNetCore.Components;

namespace BlazorFgr.Core.Rendering;

public class SignalView<T> : IComponent
{
    [Parameter]
    [EditorRequired]
    public Signal<T> Value { get; set; }

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

                    return ValueTask.CompletedTask;
                },
                async void (x) => await _renderHandle.Dispatcher.InvokeAsync(x));

        effect.Schedule();
        return Task.CompletedTask;
    }

    public static RenderFragment FromSignal(Signal<T> signal) =>
        builder =>
        {
            builder.OpenComponent<SignalView<T>>(0);
            builder.AddComponentParameter(1, nameof(SignalView<>.Value), signal);
            builder.CloseComponent();
        };
}