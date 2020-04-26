using BlazeCards.Client.Cards.Behaviors;
using BlazeCards.Client.Cards.Descriptors;
using BlazeCards.Client.Cards.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.Components
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
            builder.AddAttribute(seq++, "width", $"{this.RectDescriptor.SizeBehavior.Width}px");
            builder.AddAttribute(seq++, "height", $"{this.RectDescriptor.SizeBehavior.Height}px");

            this.HookMouseDown(builder, ref seq);

            builder.CloseElement();
        }
    }


}
