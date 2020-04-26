using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.Components
{
    public class ListComponent : CardComponent
    {
        protected override void RenderInner(RenderTreeBuilder builder, ref int seq)
        {
            foreach (var child in this.Descriptor.Children)
            {
                builder.OpenComponent(seq++, child.GetComponentType());
                builder.AddAttribute(seq++, "Canvas", this.Canvas);
                builder.AddAttribute(seq++, "Descriptor", child);
                builder.CloseComponent();
            }
        }
    }
}
