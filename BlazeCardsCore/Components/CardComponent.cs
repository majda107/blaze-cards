using BlazeCardsCore.Behaviors;
using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Factories;
using BlazeCardsCore.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Components
{
    public abstract class CardComponent : ComponentBase
    {
        [Parameter]
        public CanvasComponent Canvas { get; set; }
        public IJSRuntime JSRuntime { get => this.Canvas.JSRuntime; }


        public ElementReference Graphics { get; private set; }


        // Behaviors
        //public PositionBehavior Position { get; private set; }
        [Parameter]
        public Card Descriptor { get; set; }

        public bool ShouldInvalidate { get; set; }

        public CardComponent()
        {
            this.ShouldInvalidate = true;
        }

        protected override bool ShouldRender()
        {
            if (this.ShouldInvalidate)
            {
                this.ShouldInvalidate = false;
                return true;
            }

            return false;
        }

        public virtual void OnBlazeRender()
        {
            this.Descriptor?.PositionBehavior?.Update();
        }

        protected override void OnInitialized()
        {
            Console.WriteLine("Element initialized!");
            this.Descriptor.AssignComponent(this);
            base.OnInitialized();
        }

        //private void Init()
        //{
        //    Console.WriteLine("Initing card...");
        //    this.Position = new PositionBehavior(this);
        //}

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int seq = 0;


            builder.OpenElement(seq++, "g");
            //if (this.Descriptor.Visible)
            //    builder.AddAttribute(seq++, "class", "visible");
            //else seq++;


            this.RenderInner(builder, ref seq);


            builder.AddElementReferenceCapture(seq++, (eref) =>
            {
                this.Graphics = eref;
            });

            builder.CloseElement();
        }

        protected void HookMouseDown(RenderTreeBuilder builder, ref int seq)
        {
            builder.AddAttribute(seq++, "onmousedown", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
            {
                this.Canvas.State.ComponentClicked = true; // broken event propag

                if (!this.Descriptor.Clickable) return;

                var selected = this.Canvas.State.Selected;
                if (selected.Count <= 0)
                {
                    this.Canvas.State.Mouse.OnDown(new Vector2f((int)e.ClientX, (int)e.ClientY));
                    this.Canvas.State.Selected.Add(this.Descriptor);
                    this.Canvas.State.Highlighter = RectFactory.CreateHighlighter(this.Canvas.State.Selected);
                }
                else
                {
                    foreach (var card in this.Canvas.State.Selected)
                        if (card.HasDescendant(this.Descriptor) || card == this.Descriptor)
                        {
                            this.Canvas.State.Mouse.OnDown(new Vector2f((int)e.ClientX, (int)e.ClientY));
                            return;
                        }
                }



                //if (this.Canvas.State.Selected == this.Descriptor) // because of non-clickable lists selector selection ? does this even make sense, i am disgusted lol
                //    this.Canvas.State.Mouse.OnDown(new Vector2f((int)e.ClientX, (int)e.ClientY));
                //else if (this.Canvas.State.Selected != null && this.Canvas.State.Selected.HasDescendant(this.Descriptor))
                //    this.Canvas.State.Mouse.OnDown(new Vector2f((int)e.ClientX, (int)e.ClientY));

                //if (this.Canvas.State.Selected != null) return;

                //if (!this.Descriptor.Clickable) return;

                ////if (this.Canvas.State.Selected == this.Descriptor) return;

                //this.Canvas.State.Mouse.OnDown(new Vector2f((int)e.ClientX, (int)e.ClientY));
                //this.Canvas.State.Selected = this.Descriptor;
                ////this.Canvas.State.Highlighter = null;
                //this.Canvas.State.Highlighter = RectFactory.CreateHighlighter(this.Descriptor);

                ////this.Canvas.InvokeChange();
            }));
        }

        protected virtual void RenderInner(RenderTreeBuilder builder, ref int seq)
        {

        }

        public void InvokeChange()
        {
            this.ShouldInvalidate = true;
            this.Descriptor.Update();
            this.StateHasChanged();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //if (firstRender)
            //    this.Init();

            //this.Descriptor.AssignComponent(this);

            Console.WriteLine($"Rendering component {this.GetType().ToString()}");

            this.Descriptor?.AssignComponent(this);
            this.OnBlazeRender();

            base.OnAfterRender(firstRender);
        }
    }
}
