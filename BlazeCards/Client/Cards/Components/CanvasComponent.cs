using BlazeCards.Client.Cards.Models;
using BlazeCards.Client.Cards.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.Components
{
    public class CanvasComponent : ComponentBase
    {
        public CardState State { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public IList<CardComponent> Cards { get; set; }

        public int Sequence { get; set; }

        public CanvasComponent()
        {
            this.State = new CardState();
            this.Cards = new List<CardComponent>();
            this.Sequence = 0;

            //this.Cards.Add(new RectComponent());
            this.Cards.Add(new TextComponent());
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            this.Sequence = 0;
            builder.OpenElement(this.Sequence++, "svg");
            builder.AddAttribute(this.Sequence++, "class", "canvas");





            builder.AddAttribute(this.Sequence++, "onmousemove", EventCallback.Factory.Create<MouseEventArgs>(this, async (e) =>
            {
                this.State.Mouse.OnMove(new Vector2f((int)e.ClientX, (int)e.ClientY));
            }));

            builder.AddAttribute(this.Sequence++, "onmouseup", EventCallback.Factory.Create<MouseEventArgs>(this, async (e) =>
            {
                this.State.Mouse.OnUp(new Vector2f((int)e.ClientX, (int)e.ClientY));
                this.State.Selected = null;
            }));




            builder.OpenElement(this.Sequence++, "g");
            builder.AddAttribute(this.Sequence++, "class", "canvas-zoom");

            builder.OpenElement(this.Sequence++, "g");
            builder.AddAttribute(this.Sequence++, "class", "canvas-graphics");

            foreach (var card in this.Cards)
            {
                //card.Render().Invoke(builder);
                builder.OpenComponent(this.Sequence++, card.GetType());
                builder.AddAttribute(this.Sequence++, "Canvas", this);
                builder.CloseComponent();
            }

            builder.CloseElement();
            builder.CloseElement();

            builder.CloseElement();
        }
    }
}
