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
        //public TextBehavior TextBehavior { get; private set; }

        //public ElementReference TextRef { get; private set; }
        public TextCard TextDescriptor { get => this.Descriptor as TextCard; }

        private bool shouldResetCared;
        public RectCard CaretDescriptor { get; set; }


        public TextComponent()
        {
            this.shouldResetCared = true;
        }

        [JSInvokable]
        public async Task SelectionChanged(SelectionModel selection)
        {
            if (selection.BaseOffset > selection.ExtentOffset)
            {
                var swap = selection.BaseOffset;
                selection.BaseOffset = selection.ExtentOffset;
                selection.ExtentOffset = swap;
            }

            this.TextDescriptor.TextBehavior.Selection = selection;

            var str = this.TextDescriptor.TextBehavior.Value.Substring(0, selection.ExtentOffset);
            var box = await this.JSRuntime.InvokeAsync<BoundingClientRect>("calculateTextRect", str);

            this.CaretDescriptor.PositionBehavior.Position = new Vector2f((float)box.Width + this.TextDescriptor.TextBehavior.Padding.X, this.CaretDescriptor.PositionBehavior.Position.Y);
            this.Canvas.State.InteropQueue.Flush(this.Canvas.JSRuntime);

            Console.WriteLine($"Movin caret to {selection.ToString()}");
        }

        public void Init()
        {
            this.CaretDescriptor = new RectCard();
            this.CaretDescriptor.Highlightable = this.CaretDescriptor.Draggable = false;
            this.CaretDescriptor.Classes.Add("blaze-text-caret");
            this.CaretDescriptor.SizeBehavior.Size = new Vector2f(2, 24);

            var len = this.TextDescriptor.TextBehavior.Value.Length;
            this.TextDescriptor.TextBehavior.Selection = new SelectionModel() { BaseOffset = len, ExtentOffset = len };

            this.TextDescriptor.OnDown += async (s, e) =>
            {
                await this.JSRuntime.InvokeVoidAsync("hookEditingTextElement", DotNetObjectReference.Create(this));
            };
        }


        protected void HookDoubleClick(RenderTreeBuilder builder, ref int seq)
        {
            builder.AddAttribute(seq++, "ondblclick", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
            {
                if (!this.TextDescriptor.Editable) return;
                //this.Canvas.State.Deselect();

                this.TextDescriptor.Draggable = false;
                //this.shouldResetCared = true;
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
                if (e.Key.ToLower() == "shift") return;
                if (e.Key.ToLower() == "escape")
                {
                    this.Deselect();
                    return;
                }

                if (this.TextDescriptor.TextBehavior.Editing && this.Canvas.State.Keyboard.IsDown("Control"))
                    Console.WriteLine("SHOULD SELECT ALL!");
                else
                {
                    this.shouldResetCared = true;
                    this.TextDescriptor.TextBehavior.KeyDown(e);
                }

                this.InvokeChange();
            }));


            this.HookMouseDown(builder, ref seq);


            builder.AddContent(seq++, this.TextDescriptor.TextBehavior.Value);


            builder.CloseElement();

            if (this.TextDescriptor.TextBehavior.Editing)
            {
                //builder.OpenElement(seq++, "rect");
                //builder.AddAttribute(seq++, "x", (this.TextDescriptor.TextBehavior.Caret).ToString("0.0").Replace(',', '.'));
                //builder.AddAttribute(seq++, "y", $"{this.TextDescriptor.TextBehavior.Padding.Y + 4}px");
                //builder.AddAttribute(seq++, "height", "20px");
                //builder.AddAttribute(seq++, "width", "2px");
                //builder.AddAttribute(seq++, "class", "card-caret");
                //builder.CloseElement();

                this.CaretDescriptor.InvokeRender(builder, ref seq, this.Canvas);
            }
        }

        protected virtual void RenderTextAddition(RenderTreeBuilder builder, ref int seq) { }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.TextDescriptor.BufferSizeAsync();

            if (firstRender)
                this.Init();

            if (this.TextDescriptor.TextBehavior.Editing)
            {
                this.TextDescriptor.TextBehavior.Focus();

                var size = this.Descriptor.GetSize();
                if (this.Canvas.State.Highlighter != null) // BEWARE THIS!!!!!!!!!
                    this.Canvas.State.Highlighter.SizeBehavior.Size = size;


                if (!this.shouldResetCared) return;

                //var caretPos = this.Descriptor.GetSize();
                var str = this.TextDescriptor.TextBehavior.Value.Substring(0, this.TextDescriptor.TextBehavior.Selection.ExtentOffset);
                
                var caretPos = (await this.JSRuntime.InvokeAsync<BoundingClientRect>("calculateTextRect", str)).Size;
                Console.WriteLine($"SERGEEEEEEEEEEEJ: {str} {caretPos.ToString()}");

                caretPos.Y = this.TextDescriptor.TextBehavior.Padding.Y + 1;
                caretPos.X += this.TextDescriptor.TextBehavior.Padding.X;

                this.CaretDescriptor.PositionBehavior.Position = caretPos;
                this.Canvas.State.InteropQueue.Flush(this.Canvas.JSRuntime);

                this.shouldResetCared = false;
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
