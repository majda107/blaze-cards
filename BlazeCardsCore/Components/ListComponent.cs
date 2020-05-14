using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Components
{
    public class ListComponent : CardComponent
    {
        protected override void RenderInner(RenderTreeBuilder builder, ref int seq)
        {
            int hotFix = seq;
            seq += 100000; // yup, this is pretty ugly but don't want to mess up with it rn

            foreach (var child in this.Descriptor.Children)
            {
                builder.OpenComponent(hotFix++, child.GetComponentType());
                builder.SetKey(child.GetHashCode());
                builder.AddAttribute(hotFix++, "Canvas", this.Canvas);
                builder.AddAttribute(hotFix++, "Descriptor", child);
                builder.CloseComponent();
            }
        }
    }
}
