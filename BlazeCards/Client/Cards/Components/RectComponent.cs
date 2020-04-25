using BlazeCards.Client.Cards.Behaviors;
using BlazeCards.Client.Cards.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.Components
{
    public class RectComponent : CardComponent
    {
        public SizeBehavior Size { get; private set; }
        public RectComponent()
        {
            this.Size = new SizeBehavior(this, 30, 30);
        }

        protected override RenderFragment RenderInner()
        {
            return new RenderFragment(builder =>
            {
                builder.OpenElement(this.Canvas.Sequence++, "rect");
                builder.AddAttribute(this.Canvas.Sequence++, "width", $"{this.Size.Width}px");
                builder.AddAttribute(this.Canvas.Sequence++, "height", $"{this.Size.Height}px");

                this.HookMouseDown().Invoke(builder);

                //builder.AddAttribute(this.Canvas.Sequence++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
                //{
                //    this.Canvas.State.Selected = this;
                //    this.Canvas.State.Mouse.OnDown(new Vector2f((int)e.ClientX, (int)e.ClientY));
                //}));

                builder.CloseElement();
            });
        }
    }

    
}
