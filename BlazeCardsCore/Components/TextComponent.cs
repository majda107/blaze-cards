using BlazeCardsCore.Behaviors;
using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Factories;
using BlazeCardsCore.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BlazeCardsCore.Components
{
    public class TextComponent : CardComponent
    {
        //public TextBehavior TextBehavior { get; private set; }

        //public ElementReference TextRef { get; private set; }
        public TextCard TextDescriptor { get => this.Descriptor as TextCard; }

        private RectCard TextSelectionDescriptor;

        public TextComponent()
        {
            this.TextSelectionDescriptor = new RectCard();
            this.TextSelectionDescriptor.Classes.Add("blaze-text-select");

            //this.TextBehavior = new TextBehavior(this);
        }

        protected void HookDoubleClick(RenderTreeBuilder builder, ref int seq)
        {
            builder.AddAttribute(seq++, "ondblclick", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
            {
                if (!this.TextDescriptor.Editable) return;
                //this.Canvas.State.Deselect();

                if (this.TextDescriptor.TextBehavior.Editing)
                    this.TextDescriptor.TextBehavior.Highlight(0, this.TextDescriptor.TextBehavior.Value.Length);
                else
                    this.TextDescriptor.TextBehavior.Editing = true;

                this.InvokeChange();
            }));
        }

        public override void Deselect()
        {
            this.TextDescriptor.TextBehavior.Editing = false;
            this.TextDescriptor.TextBehavior.Selection = StringSelection.Empty;
            this.InvokeChange();

            base.Deselect();
        }

        public string GetTextID() => $"blaze-text-{this.GetUniquieID()}";
        protected override void RenderInner(RenderTreeBuilder builder, ref int seq)
        {
            this.RenderTextAddition(builder, ref seq);

            if (this.TextDescriptor.TextBehavior.Selection != StringSelection.Empty)
            {
                // SUBSTITUTE TO CARD.INVOKE_RENDER !!!!!!!!!!!!!
                builder.OpenComponent(seq++, this.TextSelectionDescriptor.GetComponentType());
                builder.AddAttribute(seq++, "Canvas", this.Canvas);
                builder.AddAttribute(seq++, "Descriptor", this.TextSelectionDescriptor);
                builder.CloseComponent();
            }

            builder.OpenElement(seq++, "text");

            var classString = "blaze-text";
            if (this.TextDescriptor.TextBehavior.Editing) classString += " blaze-text-editing";
            builder.AddAttribute(seq++, "class", classString);
            builder.AddAttribute(seq++, "id", this.GetTextID());

            builder.AddAttribute(seq++, "tabindex", "0");
            builder.AddAttribute(seq++, "x", $"{this.TextDescriptor.TextBehavior.Padding.X}px");
            builder.AddAttribute(seq++, "y", $"{this.TextDescriptor.TextBehavior.Padding.Y + 20}px");


            this.HookDoubleClick(builder, ref seq);
            builder.AddAttribute(seq++, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, (e) =>
            {
                if (this.Canvas.State.Keyboard.IsDown("Control"))
                    this.TextDescriptor.TextBehavior.Highlight(0, this.TextDescriptor.TextBehavior.Value.Length);
                else
                {
                    this.TextDescriptor.TextBehavior.KeyDown(e);
                    this.TextDescriptor.TextBehavior.Selection = StringSelection.Empty;
                }

                this.InvokeChange();
            }));


            this.HookMouseDown(builder, ref seq);


            builder.AddContent(seq++, this.TextDescriptor.TextBehavior.Value);


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
        }

        protected virtual void RenderTextAddition(RenderTreeBuilder builder, ref int seq) { }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.TextDescriptor.BufferSizeAsync();

            if (this.TextDescriptor.TextBehavior.Editing)
            {
                this.TextDescriptor.TextBehavior.Focus();

                if (this.Canvas.State.Highlighter != null) // BEWARE THIS!!!!!!!!!
                    this.Canvas.State.Highlighter.SizeBehavior.Size = this.Descriptor.GetSize();
            }

            if (firstRender)
            {
                this.TextSelectionDescriptor.PositionBehavior.Position = this.TextDescriptor.TextBehavior.Padding;
                this.TextSelectionDescriptor.SizeBehavior.Size = new Vector2f(0, 0);
            }

            if (this.TextDescriptor.TextBehavior.Selection != StringSelection.Empty)
            {
                var str = this.TextDescriptor.TextBehavior.Selection.Cut(this.TextDescriptor.TextBehavior.Value);
                var selectionBox = await this.TextDescriptor.TextBehavior.CalculateTextRect(str);
                this.TextSelectionDescriptor.SizeBehavior.Size = selectionBox.Size;
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
