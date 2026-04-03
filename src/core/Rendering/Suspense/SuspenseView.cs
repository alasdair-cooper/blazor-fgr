using BlazorFgr.Core.Primitives;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorFgr.Core.Rendering.Suspense;

public class SuspenseView : FgrScope
{
    protected override RenderFragment RenderFragment =>
        builder =>
        {
            var ctx = new SuspenseContext();

            using (FgrContext.CreateHostScope(ctx))
            {
                base.RenderFragment(builder);
            }

            if (!ctx.Waitables.All(x => x.IsReady()))
            {
                builder.Clear();
                builder.AddMarkupContent(0, "<div>Loading...</div>");
            }
        };

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => ChildContent?.Invoke(builder);

    private class SuspenseContext : IHost
    {
        private readonly List<IWaitable> _waitables = [];

        public IEnumerable<IWaitable> Waitables => _waitables;

        public void Register(IWaitable waitable) => _waitables.Add(waitable);
    }
}