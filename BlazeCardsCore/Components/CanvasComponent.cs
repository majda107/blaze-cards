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
using BlazeCardsCore.Extension;
using System.Threading.Tasks;

namespace BlazeCardsCore.Components
{
    public class CanvasComponent : ComponentBase
    {
        public CardState State { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }



        //public ElementReference CanvasReference { get; private set; }
        //public ElementReference CanvasGraphicsReference { get; private set; }
        //public ElementReference CanvasZoomReference { get; private set; }



        public BoundingClientRect Box { get; private set; }

        public bool ShouldInvalidate { get; set; }

        public CanvasComponent()
        {
            this.State = new CardState(this);
            this.ShouldInvalidate = true;
        }

        public string GetCanvasID() => $"blaze-canvas-{this.GetHashCode()}";
        public string GetTranslateID() => $"blaze-translate-{this.GetHashCode()}";
        public string GetZoomID() => $"blaze-zoom-{this.GetHashCode()}";
        public void Translate()
        {
            //this.shouldTranslate = true;
            this.State.InteropQueue.QueueChange(new PositionChange(this.GetTranslateID(), this.State.Mouse.Scroll));
            this.State.InteropQueue.Flush(this.JSRuntime);
        }

        public void Zoom()
        {
            //this.JSRuntime.InvokeVoidAsync("scaleGraphics", this.CanvasZoomReference, this.State.Mouse.Zoom, this.Box.CenterX, this.Box.CenterY);
            MonoInteropState.InvokeScale(this.GetZoomID(), this.State.Mouse.Zoom, (float)this.Box.CenterX, (float)this.Box.CenterY);

            //this.InvokeChange();
        }

        [JSInvokable]
        public void CanvasSizeChanged(BoundingClientRect rect)
        {
            this.Box = rect;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await this.JSRuntime.InvokeVoidAsync("hookCanvasElement", this.GetCanvasID(), DotNetObjectReference.Create(this));

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

            //return true; // debug
        }

        public void InvokeChange()
        {
            this.ShouldInvalidate = true;
            this.StateHasChanged();
        }

        private void OnUpLeaveCallback(float clientX, float clientY)
        {
            this.State.Mouse.CheckDrop();
            this.State.Mouse.OnUp(this.Box.Center - new Vector2f(clientX, clientY)); // will snap
            this.State.Mouse.CheckSelector();


            this.InvokeChange();
            this.ShouldInvalidate = true;
        }

        private void OnDownCallback(float clientX, float clientY, bool createSelector)
        {
            var local = this.Box.Center - new Vector2f(clientX, clientY);
            // broken event propag

            if (this.State.ComponentClicked)
            {
                this.State.ComponentClicked = false;
                return;
            }

            if (this.State.Selected.Count > 0)
            {
                //this.State.Selected.Clear();
                this.State.Deselect();
                this.State.Highlighter = null;
                //return;
            }


            if (createSelector)
            {
                var pos = new Vector2f(local.X, local.Y);
                //var pos = new Vector2f(clientX, clientY);
                //pos.ToLocalFromClient(this.Box);

                //this.State.Selector = RectFactory.CreateSelector(pos);

                // reset selector
                this.State.Selector.PositionBehavior.Position = (this.Box.Center - (pos / this.State.Mouse.Zoom)) - this.State.Mouse.Scroll;
                //this.State.Selector.PositionBehavior.Position = pos - this.State.Mouse.Scroll;
                this.State.Selector.PositionBehavior.Correction = Vector2f.Zero;
                this.State.Selector.SizeBehavior.Size = Vector2f.Zero;
                this.State.Selector.Visible = true;
                this.State.Selector.Component.InvokeChange();
            }


            //Console.WriteLine("canvas down...");
            this.State.Mouse.OnDown(local);
            this.ShouldInvalidate = true;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int seq = 0;
            builder.OpenElement(seq++, "svg");
            builder.AddAttribute(seq++, "class", "canvas");
            builder.AddAttribute(seq++, "id", this.GetCanvasID());
            builder.AddAttribute(seq++, "tabindex", "0");

            builder.AddAttribute(seq++, "onwheel", EventCallback.Factory.Create(this, e =>
            {
                //this.State.Mouse.ScrollToZoom(e.DeltaY > 0);
                this.State.Mouse.Zoom -= (float)e.DeltaY * 0.001f;
                //Console.WriteLine(this.State.Mouse.Zoom2);
                //Console.WriteLine(e.DeltaY);
                //var zoom = (float)e.DeltaY * 0.001f;
                //if (this.State.Mouse.Zoom2 < 0) this.State.Mouse.Zoom2 = 0.001f;

                this.Translate();
                this.Zoom();
            }));

            builder.AddAttribute(seq++, "onkeydown", EventCallback.Factory.Create(this, e =>
            {
                this.State.Keyboard.KeyDown(e.Key);
            }));

            builder.AddAttribute(seq++, "onkeyup", EventCallback.Factory.Create(this, e =>
            {
                this.State.Keyboard.KeyUp(e.Key);
            }));



            builder.AddAttribute(seq++, "onmousedown", EventCallback.Factory.Create(this, (e) =>
            {
                this.OnDownCallback((float)e.ClientX, (float)e.ClientY, this.State.Keyboard.IsDown("Shift"));
            }));

            builder.AddAttribute(seq++, "ontouchstart", EventCallback.Factory.Create<TouchEventArgs>(this, (e) =>
            {
                this.OnDownCallback((float)e.Touches[0].ClientX, (float)e.Touches[0].ClientY, false);
            }));

            builder.AddEventStopPropagationAttribute(seq++, "ontouchstart", true);



            builder.AddAttribute(seq++, "onmousemove", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
            {
                this.State.Mouse.OnMove(this.Box.ToCenterCoords(new Vector2f((float)e.ClientX, (float)e.ClientY)));
            }));

            builder.AddAttribute(seq++, "ontouchmove", EventCallback.Factory.Create<TouchEventArgs>(this, (e) =>
            {
                var pos = new Vector2f((int)e.Touches[0].ClientX, (int)e.Touches[0].ClientY);

                if (e.Touches.Length == 1)
                {
                    if (!this.State.Selector.Visible && this.State.Selected.Count == 0)
                        this.OnDownCallback((float)e.Touches[0].ClientX, (float)e.Touches[0].ClientY, true);

                    this.State.Mouse.OnMove(this.Box.ToCenterCoords(pos));
                }
                else if (e.Touches.Length > 1)
                {

                    var currentHypot = MathExtension.Hypot(e.Touches[0].PageX - e.Touches[1].PageX, e.Touches[0].PageY - e.Touches[1].PageY);

                    var val = currentHypot - this.State.Touch.LastPinchHypot;

                    this.State.Touch.LastPinchHypot = currentHypot;

                    if (currentHypot > 130.0f)
                    {
                        this.State.Mouse.Zoom += val < 0 ? -0.05f : 0.05f;
                        this.Zoom();
                        this.State.Mouse.OnFakeMove(this.Box.ToCenterCoords(pos));
                    }
                    else
                        this.State.Mouse.OnMove(this.Box.ToCenterCoords(pos));
                }

            }));

            //builder.AddEventPreventDefaultAttribute(seq++, "ontouchmove", true);
            //builder.AddEventStopPropagationAttribute(seq++, "ontouchmove", true);



            builder.AddAttribute(seq++, "onmouseup", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
            {
                this.OnUpLeaveCallback((float)e.ClientX, (float)e.ClientY);
            }));


            builder.AddAttribute(seq++, "ontouchend", EventCallback.Factory.Create(this, (_) =>
            {
                this.OnUpLeaveCallback(0, 0);
            }));



            builder.AddAttribute(seq++, "onmouseleave", EventCallback.Factory.Create(this, (_) =>
            {
                this.OnUpLeaveCallback(0, 0);
            }));

            builder.AddAttribute(seq++, "ontouchcancel", EventCallback.Factory.Create<TouchEventArgs>(this, (e) =>
            {
                this.OnUpLeaveCallback(0, 0);
            }));



            builder.OpenElement(seq++, "g");
            builder.AddAttribute(seq++, "class", "canvas-zoom");
            builder.AddAttribute(seq++, "id", this.GetZoomID());

            builder.OpenElement(seq++, "g");
            builder.AddAttribute(seq++, "class", "canvas-graphics");
            builder.AddAttribute(seq++, "id", this.GetTranslateID());



            int seqHotFix = seq;
            seq += 100000; // another ugly hotfix
            foreach (var card in this.State.Storage.Cards)
            {
                //card.Render().Invoke(builder);
                builder.OpenComponent(seqHotFix++, card.GetComponentType());
                builder.AddAttribute(seqHotFix++, "Canvas", this);
                builder.AddAttribute(seqHotFix++, "Descriptor", card);
                builder.CloseComponent();
            }


            if (this.State.Selector != null)
            {
                builder.OpenComponent(seq++, this.State.Selector.GetComponentType());
                builder.AddAttribute(seq++, "Canvas", this);
                builder.AddAttribute(seq++, "Descriptor", this.State.Selector);
                builder.CloseComponent();
            }
            else seq += 3; // fix capture ref error


            //Console.WriteLine("Re-rendering highlighter");
            if (this.State.Highlighter != null)
            {
                builder.OpenComponent(seq++, this.State.Highlighter.GetComponentType());
                builder.AddAttribute(seq++, "Canvas", this);
                builder.AddAttribute(seq++, "Descriptor", this.State.Highlighter);
                builder.CloseComponent();
            }
            else seq += 3; // fix capture ref error



            builder.CloseElement();
            builder.CloseElement();
            builder.CloseElement();
        }
    }
}
