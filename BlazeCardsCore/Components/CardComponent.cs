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


        //public ElementReference Graphics { get; private set; }


        // Behaviors
        //public PositionBehavior Position { get; private set; }

        private Card _descriptor;


        [Parameter]
        public Card Descriptor
        {
            get => this._descriptor;
            set
            {
                if (this._descriptor != null && value.GetHashCode() != this._descriptor.GetHashCode())
                    this.ShouldInvalidate = true;

                this._descriptor = value;
            }
        }

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
            this.Descriptor.PositionBehavior.Update(); // lol idk
        }

        public virtual void Deselect()
        {

        }

        protected override void OnInitialized()
        {
            this.Descriptor.AssignComponent(this);
            base.OnInitialized();
        }

        //protected override void OnParametersSet()
        //{
        //    this.Descriptor.AssignComponent(this);
        //    base.OnParametersSet();
        //}

        //private void Init()
        //{
        //    Console.WriteLine("Initing card...");
        //    this.Position = new PositionBehavior(this);
        //}

        public string GetUniquieID()
        {
            return $"blaze-g-{this.GetHashCode()}";
        }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int seq = 0;


            builder.OpenElement(seq++, "g");
            builder.AddAttribute(seq++, "id", this.GetUniquieID());

            this.RenderInner(builder, ref seq);


            //builder.AddElementReferenceCapture(seq++, (eref) =>
            //{
            //    this.Graphics = eref;
            //});

            builder.CloseElement();
        }

        protected void BuildClassAttribute(RenderTreeBuilder builder, ref int seq)
        {
            var classString = String.Empty;
            for (int i = 0; i < this.Descriptor.Classes.Count; i++)
            {
                classString += this.Descriptor.Classes[i];
                if (i == this.Descriptor.Classes.Count - 1) continue;
                classString += " ";
            }

            builder.AddAttribute(seq++, "class", classString);
        }

        private void MouseDownCallback(float clientX, float clientY)
        {
            if (!this.Descriptor.Clickable) return;

            foreach (var card in this.Canvas.State.Selected)
                if (card.HasDescendant(c => c == this.Descriptor) || card == this.Descriptor)
                {
                    this.Descriptor.FireDown(new Vector2f(clientX, clientY));
                    this.Canvas.State.Mouse.OnDown(this.Canvas.Box.Center - new Vector2f(clientX, clientY));
                    return;
                }

            this.Descriptor.FireDown(new Vector2f(clientX, clientY));
            this.Canvas.State.Mouse.OnDown(this.Canvas.Box.Center - new Vector2f(clientX, clientY));
            this.Canvas.State.Selected.Clear();
            this.Canvas.State.Selected.Add(this.Descriptor);
            this.Canvas.State.Highlighter = RectFactory.CreateHighlighter(this.Canvas.State.Selected);

            this.Canvas.InvokeChange();
        }

        protected void HookMouseDown(RenderTreeBuilder builder, ref int seq)
        {
            builder.AddAttribute(seq++, "onmousedown", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
            {
                this.MouseDownCallback((float)e.ClientX, (float)e.ClientY);
            }));

            builder.AddEventStopPropagationAttribute(seq++, "onmousedown", true);

            builder.AddAttribute(seq++, "ontouchstart", EventCallback.Factory.Create<TouchEventArgs>(this, (e) =>
            {
                if (e.Touches.Length > 1) return;
                this.MouseDownCallback((float)e.Touches[0].ClientX, (float)e.Touches[0].ClientY);
            }));

            builder.AddEventStopPropagationAttribute(seq++, "ontouchstart", true);
        }

        protected virtual void RenderInner(RenderTreeBuilder builder, ref int seq)
        {

        }

        public void InvokeChange()
        {
            this.ShouldInvalidate = true;

            this.Descriptor.Update();
            this.Descriptor.PositionBehavior.Update(); // lol idk

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
