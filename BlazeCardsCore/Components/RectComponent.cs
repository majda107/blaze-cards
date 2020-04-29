using BlazeCardsCore.Behaviors;
using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Components
{
    public class RectComponent : CardComponent
    {
        //public SizeBehavior Size { get; private set; }

        public RectCard RectDescriptor { get => this.Descriptor as RectCard; }
        public RectComponent()
        {
            //this.Size = new SizeBehavior(this, 30, 30);
        }


        protected override void RenderInner(RenderTreeBuilder builder, ref int seq)
        {
            builder.OpenElement(seq++, "rect");
            builder.AddAttribute(seq++, "class", "blaze-rect");
            builder.AddAttribute(seq++, "width", $"{Math.Abs(this.RectDescriptor.SizeBehavior.Width).ToString("0.0").Replace(',', '.')}px");
            builder.AddAttribute(seq++, "height", $"{Math.Abs(this.RectDescriptor.SizeBehavior.Height).ToString("0.0").Replace(',', '.')}px");

            foreach (var elementClass in this.Descriptor.Classes)
                builder.AddAttribute(seq++, "class", elementClass);

            this.HookMouseDown(builder, ref seq);

            builder.CloseElement();
        }
    }


}
