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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace BlazeCardsCore.Components
{
    public class TextComponent : CardComponent
    {
        public TextCard TextDescriptor { get => this.Descriptor as TextCard; }


        private bool initialized;
        public TextComponent()
        {
            this.initialized = false;
        }

        //protected override void OnParametersSet()
        //{
        //    //Console.WriteLine($"~~~ {this.TextDescriptor.TextBehavior.Value} #{this.GetHashCode()}");
        //    base.OnParametersSet();
        //    //this.initialized = false;
        //    //this.TextDescriptor.AssignComponent(this);
        //}

        [JSInvokable]
        public async Task KeyDown(int key, bool shiftKey)
        {
            if (key == 16) return; // shift
            if (key == 27) // escape
            {
                this.Deselect();
                return;
            }

            if (this.TextDescriptor.TextBehavior.Editing && this.Canvas.State.Keyboard.IsDown("Control"))
            {
                await this.TextDescriptor.SelectionBehavior.SelectAll(this.Canvas);
            }
            else
            {
                //this.shouldResetCared = true;
                await this.TextDescriptor.TextBehavior.KeyDown(key, this.Canvas, shiftKey);
            }

            this.InvokeChange();

            //Console.WriteLine($"KEEEEEEEEY: {key}");
        }

        public async Task Init()
        {
            await this.TextDescriptor.SelectionBehavior.Init(this.Canvas);

            this.TextDescriptor.OnDown += async (s, e) =>
            {
                var textCard = s as TextCard;

                var pos = e - s.GetGlobalPosition() - this.Canvas.State.Mouse.Scroll - textCard.TextBehavior.Padding;
                await textCard.SelectionBehavior.OnDown(pos, this.Canvas);
            };

            this.TextDescriptor.OnMove += (s, dev) =>
            {
                var textCard = s as TextCard;

                if (!textCard.TextBehavior.Editing) return;

                if (!textCard.SelectionBehavior.Selecting)
                {
                    textCard.SelectionBehavior.Selecting = true;
                    //this.InvokeChange();
                }

                textCard.SelectionBehavior.SelectorDescriptor.SizeBehavior.Size += new Vector2f(dev.X, 0);
            };

            this.TextDescriptor.OnUp += async (s, dev) =>
            {
                var textCard = s as TextCard;

                textCard.SelectionBehavior.Selecting = false;
                await textCard.SelectionBehavior.OnUp(this.Canvas);
            };
        }


        protected void HookDoubleClick(RenderTreeBuilder builder, ref int seq)
        {
            builder.AddAttribute(seq++, "ondblclick", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
            {
                if (!this.TextDescriptor.Editable) return;
                this.TextDescriptor.Draggable = false;
                this.TextDescriptor.TextBehavior.Editing = true;

                this.InvokeChange();
            }));
        }

        public override void Deselect()
        {
            this.TextDescriptor.Draggable = true;
            this.TextDescriptor.TextBehavior.Editing = false;
            this.InvokeChange();

            base.Deselect();
        }

        public string GetTextID() => $"blaze-text-{this.GetUniquieID()}";
        protected override void RenderInner(RenderTreeBuilder builder, ref int seq)
        {
            this.RenderTextAddition(builder, ref seq);

            if (this.TextDescriptor.TextBehavior.Editing)
                this.TextDescriptor.SelectionBehavior.SelectorDescriptor.InvokeRender(builder, ref seq, this.Canvas);

            builder.OpenElement(seq++, "text");

            var classString = "blaze-text";
            if (this.TextDescriptor.TextBehavior.Editing) classString += " blaze-text-editing";
            builder.AddAttribute(seq++, "class", classString);
            builder.AddAttribute(seq++, "id", this.GetTextID());

            builder.AddAttribute(seq++, "tabindex", "0");
            builder.AddAttribute(seq++, "x", $"{this.TextDescriptor.TextBehavior.Padding.X}px");
            builder.AddAttribute(seq++, "y", $"{this.TextDescriptor.TextBehavior.Padding.Y + 20}px");

            if (this.TextDescriptor.Editable)
                this.HookDoubleClick(builder, ref seq);


            this.HookMouseDown(builder, ref seq);


            //builder.AddContent(seq++, this.TextDescriptor.TextBehavior.Value + $" {this.GetHashCode()}");
            builder.AddContent(seq++, this.TextDescriptor.TextBehavior.Value);


            builder.CloseElement();

            if (this.TextDescriptor.TextBehavior.Editing && !this.TextDescriptor.SelectionBehavior.Selecting)
                this.TextDescriptor.SelectionBehavior.CaretDescriptor.InvokeRender(builder, ref seq, this.Canvas);
        }

        protected virtual void RenderTextAddition(RenderTreeBuilder builder, ref int seq) { }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.TextDescriptor.BufferSizeAsync();

            if (!this.initialized)
            {
                this.initialized = true;
                Console.WriteLine($"~~~~~~ TEXT FIRST RENDER! {this.TextDescriptor.TextBehavior.Value}");
                await this.Init();
            }
                

            if (this.TextDescriptor.TextBehavior.Editing)
            {
                await this.JSRuntime.InvokeVoidAsync("hookEditing", DotNetObjectReference.Create(this));

                var size = this.Descriptor.GetSize();
                if (this.Canvas.State.Highlighter != null) // BEWARE THIS!!!!!!!!!
                    this.Canvas.State.Highlighter.SizeBehavior.Size = size;
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
