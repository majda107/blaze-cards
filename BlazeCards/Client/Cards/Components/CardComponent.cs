using BlazeCards.Client.Cards.Behaviors;
using BlazeCards.Client.Cards.Descriptors;
using BlazeCards.Client.Cards.Models;
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

        public CardComponent()
        {
            
        }

        protected override void OnInitialized()
        {
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
                if (this.Canvas.State.Selected == this) return;

                this.Canvas.State.ComponentClicked = true; // broken event propag
                this.Canvas.State.Selected = this;
                this.Canvas.State.Mouse.OnDown(new Vector2f((int)e.ClientX, (int)e.ClientY));
            }));

            //builder.AddAttribute(seq++, "onmousedown:stopPropagation", true);
        }

        protected virtual void RenderInner(RenderTreeBuilder builder, ref int seq)
        {

        }

        public void InvokeChange()
        {
            this.StateHasChanged();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //if (firstRender)
            //    this.Init();

            //this.Descriptor.AssignComponent(this);

            this.Descriptor?.PositionBehavior?.Update();

            base.OnAfterRender(firstRender);
        }
    }
}
