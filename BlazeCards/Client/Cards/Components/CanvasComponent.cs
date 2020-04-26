using BlazeCards.Client.Cards.Descriptors;
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

        public IList<Card> Cards { get; set; }

        public int Sequence { get; set; }

        public ElementReference CanvasReference { get; private set; }

        public CanvasComponent()
        {
            this.State = new CardState();
            this.Cards = new List<Card>();
            this.Sequence = 0;




            this.Cards.Add(new RectCard());
            this.Cards.Add(new TextCard());


            var list = new VerticalListCard();
            list.AddChild(new RectCard());
            list.AddChild(new RectCard());
            list.PositionBehavior.Position = new Vector2f(100, 100);

            this.Cards.Add(list);
            //this.Cards.Add(new CardComponent());
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            this.Sequence = 0;
            builder.OpenElement(this.Sequence++, "svg");
            builder.AddAttribute(this.Sequence++, "class", "canvas");

            builder.AddAttribute(this.Sequence++, "onmousedown", EventCallback.Factory.Create(this, (e) =>
            {
                // broken event propag
                if (this.State.ComponentClicked)
                {
                    this.State.ComponentClicked = false;
                    return;
                }


                //this.State.Selector = new RectCard();
                //var pos = new Vector2f((float)e.ClientX, (float)e.ClientY);
                //pos.ToLocalFromClient(this.JSRuntime, this.CanvasReference).ContinueWith(t =>
                //{
                //    this.State.Selector.PositionBehavior.Position = pos;
                //});

                //this.State.Selector.SizeBehavior.Size = Vector2f.Zero;

                Console.WriteLine("canvas down...");
            }));

            builder.AddAttribute(this.Sequence++, "onmousemove", EventCallback.Factory.Create<MouseEventArgs>(this, async (e) =>
            {
                this.State.Mouse.OnMove(new Vector2f((int)e.ClientX, (int)e.ClientY));
            }));

            builder.AddAttribute(this.Sequence++, "onmouseup", EventCallback.Factory.Create<MouseEventArgs>(this, async (e) =>
            {
                this.State.Mouse.OnUp(new Vector2f((int)e.ClientX, (int)e.ClientY));
                this.State.Selected = null;
                this.State.Selector = null;
            }));



            builder.OpenElement(this.Sequence++, "g");
            builder.AddAttribute(this.Sequence++, "class", "canvas-zoom");

            builder.OpenElement(this.Sequence++, "g");
            builder.AddAttribute(this.Sequence++, "class", "canvas-graphics");

            foreach (var card in this.Cards)
            {
                //card.Render().Invoke(builder);
                builder.OpenComponent(this.Sequence++, card.GetComponentType());
                builder.AddAttribute(this.Sequence++, "Canvas", this);
                builder.AddAttribute(this.Sequence++, "Descriptor", card);
                builder.CloseComponent();
            }



            if (this.State.Selector != null)
            {
                builder.OpenComponent(this.Sequence++, this.State.Selector.GetComponentType());
                builder.AddAttribute(this.Sequence++, "Canvas", this);
                builder.AddAttribute(this.Sequence++, "Descriptor", this.State.Selector);
                builder.CloseComponent();
            }
            else this.Sequence += 3;


            builder.AddElementReferenceCapture(this.Sequence++, (eref) =>
            {
                this.CanvasReference = eref;
            });

            builder.CloseElement();
            builder.CloseElement();

            builder.CloseElement();
        }
    }
}
