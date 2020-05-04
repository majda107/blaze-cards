using BlazeCardsCore.Behaviors;
using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Factories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Components
{
    public class TextComponent : CardComponent
    {
        //public TextBehavior TextBehavior { get; private set; }

        public ElementReference TextRef { get; private set; }
        public TextCard TextDescriptor { get => this.Descriptor as TextCard; }

        public TextComponent()
        {
            //this.TextBehavior = new TextBehavior(this);
        }

        protected void HookDoubleClick(RenderTreeBuilder builder, ref int seq)
        {
            builder.AddAttribute(seq++, "ondblclick", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
            {
                //this.Canvas.State.Deselect();

                this.TextDescriptor.TextBehavior.Editing = true;
                this.InvokeChange();
            }));
        }

        protected void HookBlur(RenderTreeBuilder builder, ref int seq)
        {
            builder.AddAttribute(seq++, "onblur", EventCallback.Factory.Create(this, () =>
            {
                this.TextDescriptor.TextBehavior.Editing = false;
                this.InvokeChange();
                //this.Canvas.State.Selected = null;
            }));
        }


        public override void Deselect()
        {
            this.TextDescriptor.TextBehavior.Editing = false;
            this.InvokeChange();

            base.Deselect();
        }

        protected override void RenderInner(RenderTreeBuilder builder, ref int seq)
        {
            this.RenderTextAddition(builder, ref seq);

            builder.OpenElement(seq++, "text");

            builder.AddAttribute(seq++, "class", "blaze-text");
            builder.AddAttribute(seq++, "tabindex", "0");
            builder.AddAttribute(seq++, "x", $"{this.TextDescriptor.TextBehavior.Padding.X}px");
            builder.AddAttribute(seq++, "y", $"{this.TextDescriptor.TextBehavior.Padding.Y + 20}px");

            this.HookDoubleClick(builder, ref seq);
            //this.HookBlur(builder, ref seq);

            builder.AddAttribute(seq++, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, (e) =>
            {
                //Console.WriteLine("key down...");
                this.TextDescriptor.TextBehavior.KeyDown(e);
                this.InvokeChange();
            }));


            this.HookMouseDown(builder, ref seq);

            builder.AddContent(seq++, this.TextDescriptor.TextBehavior.Value);



            builder.AddElementReferenceCapture(seq++, (eref) =>
            {
                this.TextRef = eref;
            });
            builder.CloseElement();


            if (this.TextDescriptor.TextBehavior.Editing)
            {
                builder.OpenElement(seq++, "rect");
                builder.AddAttribute(seq++, "x", (this.TextDescriptor.TextBehavior.Caret + 4).ToString("0.0").Replace(',', '.'));
                builder.AddAttribute(seq++, "y", $"{this.TextDescriptor.TextBehavior.Padding.Y + 4}px");
                builder.AddAttribute(seq++, "height", "20px");
                builder.AddAttribute(seq++, "width", "2px");
                builder.AddAttribute(seq++, "class", "card-caret");
                builder.CloseElement();
            }
            else seq += 6;
        }

        protected virtual void RenderTextAddition(RenderTreeBuilder builder, ref int seq) { }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.TextDescriptor.BufferSizeAsync();

            if (this.TextDescriptor.TextBehavior.Editing)
            {
                this.TextDescriptor.TextBehavior.Focus();
                this.Canvas.State.Highlighter.SizeBehavior.Size = this.Descriptor.GetSize();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
