﻿using BlazeCardsCore.Descriptors;
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



        public ElementReference CanvasGraphicsReference { get; private set; }
        public ElementReference CanvasZoomReference { get; private set; }



        public BoundingClientRect Box { get; private set; }

        public bool ShouldInvalidate { get; set; }

        public CanvasComponent()
        {
            this.State = new CardState(this);
            this.ShouldInvalidate = true;
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

            //return true; // debug

            //return base.ShouldRender();
        }

        public void InvokeChange()
        {
            this.ShouldInvalidate = true;
            this.StateHasChanged();
        }

        private void OnUpLeaveCallback(float clientX, float clientY)
        {
            var pos = new Vector2f(clientX, clientY);


            this.State.Mouse.CheckDrop();
            this.State.Mouse.OnUp(pos); // will snap
            this.State.Mouse.CheckSelector();


            this.InvokeChange();
            this.ShouldInvalidate = true;
        }

        private void OnDownCallback(float clientX, float clientY)
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
                var pos = new Vector2f(clientX, clientY);
                pos.ToLocalFromClient(this.Box);

                //this.State.Selector = RectFactory.CreateSelector(pos);

                // reset selector
                this.State.Selector.PositionBehavior.Position = (pos / this.State.Mouse.Zoom) - this.State.Mouse.Scroll;
                this.State.Selector.PositionBehavior.Correction = Vector2f.Zero;
                this.State.Selector.SizeBehavior.Size = Vector2f.Zero;
                this.State.Selector.Visible = true;
                this.State.Selector.Component.InvokeChange();
            }


            //Console.WriteLine("canvas down...");
            this.State.Mouse.OnDown(new Vector2f(clientX, clientY));
            this.ShouldInvalidate = true;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int seq = 0;
            builder.OpenElement(seq++, "svg");
            builder.AddAttribute(seq++, "class", "canvas");
            builder.AddAttribute(seq++, "tabindex", "0");

            builder.AddAttribute(seq++, "onwheel", EventCallback.Factory.Create(this, e =>
            {
                //Console.WriteLine("Zoomin !");

                this.State.Mouse.Zoom -= (float)e.DeltaY * 0.001f;
                this.Zoom();
            }));

            builder.AddAttribute(seq++, "onkeydown", EventCallback.Factory.Create(this, e =>
            {
                this.State.Keyboard.KeyDown(e.Key);
                //this.ShouldInvalidate = true;
            }));

            builder.AddAttribute(seq++, "onkeyup", EventCallback.Factory.Create(this, e =>
            {
                this.State.Keyboard.KeyUp(e.Key);
                //this.ShouldInvalidate = true;
            }));



            builder.AddAttribute(seq++, "onmousedown", EventCallback.Factory.Create(this, (e) =>
            {
                this.OnDownCallback((float)e.ClientX, (float)e.ClientY);
            }));

            builder.AddAttribute(seq++, "ontouchstart", EventCallback.Factory.Create<TouchEventArgs>(this, (e) =>
            {
                this.OnDownCallback((float)e.Touches[0].ClientX, (float)e.Touches[0].ClientY);
            }));

            //builder.AddEventPreventDefaultAttribute(seq++, "ontouchstart", true);
            //builder.AddEventStopPropagationAttribute(seq++, "ontouchstart", true);



            builder.AddAttribute(seq++, "onmousemove", EventCallback.Factory.Create<MouseEventArgs>(this, async (e) =>
            {
                //if (!EventPrevention.EventsEnabled) return;

                this.State.Mouse.OnMove(new Vector2f((int)e.ClientX, (int)e.ClientY));
            }));

            builder.AddAttribute(seq++, "ontouchmove", EventCallback.Factory.Create<TouchEventArgs>(this, (e) =>
            {
                //if (!EventPrevention.EventsEnabled)
                //{
                //    Console.WriteLine("CANT FIRE BECAUSE OF MUTEX...");
                //    return;
                //}

                var pos = new Vector2f((int)e.Touches[e.Touches.Length - 1].ClientX, (int)e.Touches[e.Touches.Length - 1].ClientY);
                //Console.WriteLine($"Touch moved {pos.X} {pos.Y}");

                this.State.Mouse.OnMove(pos);
            }));

            //builder.AddEventPreventDefaultAttribute(seq++, "ontouchmove", true);
            builder.AddEventStopPropagationAttribute(seq++, "ontouchmove", true);



            builder.AddAttribute(seq++, "onmouseup", EventCallback.Factory.Create<MouseEventArgs>(this, async (e) =>
            {
                this.OnUpLeaveCallback((float)e.ClientX, (float)e.ClientY);
            }));


            builder.AddAttribute(seq++, "ontouchend", EventCallback.Factory.Create(this, (_) =>
            {
                this.OnUpLeaveCallback(0, 0);
            }));



            builder.AddAttribute(seq++, "onmouseleave", EventCallback.Factory.Create(this, async (_) =>
            {
                this.OnUpLeaveCallback(0, 0);
            }));

            builder.AddAttribute(seq++, "ontouchcancel", EventCallback.Factory.Create<TouchEventArgs>(this, (e) =>
            {
                this.OnUpLeaveCallback(0, 0);
            }));



            builder.OpenElement(seq++, "g");
            builder.AddAttribute(seq++, "class", "canvas-zoom");

            builder.OpenElement(seq++, "g");
            builder.AddAttribute(seq++, "class", "canvas-graphics");




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


            builder.AddElementReferenceCapture(seq++, (eref) =>
            {
                this.CanvasGraphicsReference = eref;
            });

            builder.CloseElement();


            builder.AddElementReferenceCapture(seq++, (eref) =>
            {
                this.CanvasZoomReference = eref;
            });


            builder.CloseElement();
            builder.CloseElement();
        }
    }
}
