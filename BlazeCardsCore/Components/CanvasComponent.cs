using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Factories;
using BlazeCardsCore.Models;
using BlazeCardsCore.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazeCardsCore.Components
{
    public class CanvasComponent : ComponentBase
    {
        private bool shouldTranslate;
        public CardState State { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public IList<Card> Cards { get; set; }

        public int Sequence { get; set; }

        public ElementReference CanvasGraphicsReference { get; private set; }
        public ElementReference CanvasZoomReference { get; private set; }

        public BoundingClientRect Box { get; private set; }

        public bool ShouldInvalidate { get; set; }

        public CanvasComponent()
        {

            this.State = new CardState(this);
            this.Cards = new List<Card>();
            this.Sequence = 0;

            this.ShouldInvalidate = true;

            this.Cards.Add(new RectCard());
            //this.Cards.Add(new TextCard());

            for (int i = 0; i < 20; i++)
            {
                var card = new RectCard();
                card.PositionBehavior.Position = new Vector2f(i * 30, i % 10 * 30);
                this.Cards.Add(card);

                var textCard = new TextCard();
                textCard.PositionBehavior.Position = new Vector2f(i * 30 + 40, i % 10 * 30);
                this.Cards.Add(textCard);
            }


            //var list = new VerticalListCard(false);
            //list.AddChild(new RectCard());
            //list.AddChild(new RectCard());
            //list.AddChild(new TextCard());
            //list.AddChild(new RectCard());
            //list.AddChild(new RectCard());
            //list.PositionBehavior.Position = new Vector2f(100, 100);

            //var list2 = new HorizontalListCard(false, 10);
            //list2.AddChild(new RectCard());
            //list2.AddChild(new RectCard());
            //list2.AddChild(new TextCard());
            //list2.PositionBehavior.Position = new Vector2f(100, 400);

            //this.Cards.Add(list);
            //this.Cards.Add(list2);
            //this.Cards.Add(new CardComponent());
        }

        public void Translate()
        {
            //this.shouldTranslate = true;
            this.State.InteropQueue.QueueChange(new PositionChange(this.CanvasGraphicsReference, this.State.Mouse.Scroll));
            this.State.InteropQueue.Flush(this.JSRuntime);
        }

        public void Zoom()
        {
            this.JSRuntime.InvokeVoidAsync("scaleGraphics", this.CanvasZoomReference, this.State.Mouse.Zoom);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                this.Box = await JSRuntime.InvokeAsync<BoundingClientRect>("getBoudingRect", this.CanvasGraphicsReference);


            Console.WriteLine("Re-rendering canvas");

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override bool ShouldRender()
        {
            if (this.ShouldInvalidate)
            {
                this.ShouldInvalidate = false;
                return true;
            }

            return false;
            //return base.ShouldRender();
        }

        public void InvokeChange()
        {
            this.ShouldInvalidate = true;
            this.StateHasChanged();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            this.Sequence = 0;
            builder.OpenElement(this.Sequence++, "svg");
            builder.AddAttribute(this.Sequence++, "class", "canvas");
            builder.AddAttribute(this.Sequence++, "tabindex", "0");

            builder.AddAttribute(this.Sequence++, "onwheel", EventCallback.Factory.Create(this, e =>
            {
                Console.WriteLine("Zoomin !");

                this.State.Mouse.Zoom -= (float)e.DeltaY * 0.001f;
                this.Zoom();
            }));

            builder.AddAttribute(this.Sequence++, "onkeydown", EventCallback.Factory.Create(this, e =>
            {
                this.State.Keyboard.KeyDown(e.Key);
                //this.ShouldInvalidate = true;
            }));

            builder.AddAttribute(this.Sequence++, "onkeyup", EventCallback.Factory.Create(this, e =>
            {
                this.State.Keyboard.KeyUp(e.Key);
                //this.ShouldInvalidate = true;
            }));

            builder.AddAttribute(this.Sequence++, "onmousedown", EventCallback.Factory.Create(this, (e) =>
            {
                // broken event propag

                if (this.State.ComponentClicked)
                {
                    this.State.ComponentClicked = false;
                    return;
                }

                if (this.State.Selected.Count > 0)
                {
                    this.State.Selected.Clear();
                    this.State.Highlighter = null;
                    //return;
                }


                if (this.State.Keyboard.IsDown("Shift"))
                {
                    var pos = new Vector2f((float)e.ClientX, (float)e.ClientY);
                    pos.ToLocalFromClient(this.Box);

                    //this.State.Selector = RectFactory.CreateSelector(pos);

                    // reset selector
                    this.State.Selector.PositionBehavior.Position = pos - this.State.Mouse.Scroll;
                    this.State.Selector.PositionBehavior.Correction = Vector2f.Zero;
                    this.State.Selector.SizeBehavior.Size = Vector2f.Zero;
                    this.State.Selector.Visible = true;
                    this.State.Selector.Component.InvokeChange();
                }


                //Console.WriteLine("canvas down...");
                this.State.Mouse.OnDown(new Vector2f((float)e.ClientX, (float)e.ClientY));
                this.ShouldInvalidate = true;
            }));

            builder.AddAttribute(this.Sequence++, "onmousemove", EventCallback.Factory.Create<MouseEventArgs>(this, async (e) =>
            {
                this.State.Mouse.OnMove(new Vector2f((int)e.ClientX, (int)e.ClientY));
            }));

            builder.AddAttribute(this.Sequence++, "onmouseup", EventCallback.Factory.Create<MouseEventArgs>(this, async (e) =>
            {
                this.State.Mouse.OnUp(new Vector2f((int)e.ClientX, (int)e.ClientY));
                //this.State.Selected = null;
                //this.State.Highlighter = null;

                if (this.State.Selector != null && this.State.Selector.Visible)
                {
                    var selectorBox = BoundingRect.FromPositionSize(this.State.Selector.GetGlobalPosition(), this.State.Selector.GetSize());
                    var traversed = new List<Card>();
                    foreach (var card in this.Cards)
                    {
                        card.TraverseOverlap(selectorBox, traversed);
                    }

                    if (traversed.Count > 0)
                    {
                        //Console.WriteLine($"Selecting {traversed.Count} items...");

                        foreach (var traversedCard in traversed)
                            this.State.Selected.Add(traversedCard);
                        //this.State.Selected = traversedCard
                        //this.State.Highlighter = RectFactory.CreateHighlighter(traversed.First());
                        this.State.Highlighter = RectFactory.CreateHighlighter(traversed);
                        //break;
                    }

                    //Console.WriteLine("No overlap");

                    this.State.Selector.Visible = false;
                    this.State.Selector.Component.InvokeChange();
                }

                this.ShouldInvalidate = true;
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

            //Console.WriteLine("Re-rendering highlighter");
            if (this.State.Highlighter != null)
            {
                builder.OpenComponent(this.Sequence++, this.State.Highlighter.GetComponentType());
                builder.AddAttribute(this.Sequence++, "Canvas", this);
                builder.AddAttribute(this.Sequence++, "Descriptor", this.State.Highlighter);
                builder.CloseComponent();
            }
            else this.Sequence += 3;


            builder.AddElementReferenceCapture(this.Sequence++, (eref) =>
            {
                this.CanvasGraphicsReference = eref;
            });

            builder.CloseElement();


            builder.AddElementReferenceCapture(this.Sequence++, (eref) =>
            {
                this.CanvasZoomReference = eref;
            });


            builder.CloseElement();
            builder.CloseElement();
        }
    }
}
