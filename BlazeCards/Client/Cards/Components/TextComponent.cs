using BlazeCards.Client.Cards.Behaviors;
using BlazeCards.Client.Cards.Descriptors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.Components
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

        protected override void RenderInner(RenderTreeBuilder builder, ref int seq)
        {

            builder.OpenElement(seq++, "text");

            builder.AddAttribute(seq++, "class", "blaze-text");
            builder.AddAttribute(seq++, "tabindex", "0");
            builder.AddAttribute(seq++, "x", "0");
            builder.AddAttribute(seq++, "y", "20");

            builder.AddAttribute(seq++, "ondblclick", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
            {
                //Console.WriteLine("CANVAS DOUBLE CLICCCCC...");
                this.TextDescriptor.TextBehavior.Editing = true;
                this.Canvas.State.Selected.Add(this.Descriptor);

                Console.WriteLine("Editing...");
            }));

            builder.AddAttribute(seq++, "onblur", EventCallback.Factory.Create(this, () =>
            {
                this.TextDescriptor.TextBehavior.Editing = false;
                //this.Canvas.State.Selected = null;
            }));

            builder.AddAttribute(seq++, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, (e) =>
            {
                Console.WriteLine("key down...");
                this.TextDescriptor.TextBehavior.KeyDown(e);
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
                builder.AddAttribute(seq++, "x", (this.TextDescriptor.TextBehavior.Caret + 2).ToString("0.0").Replace(',', '.'));
                builder.AddAttribute(seq++, "y", "4");
                builder.AddAttribute(seq++, "height", "20px");
                builder.AddAttribute(seq++, "width", "2px");
                builder.AddAttribute(seq++, "class", "card-caret");
                builder.CloseElement();
            }
            else seq += 6;
        }

        private void Init()
        {

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await this.TextDescriptor.TextBehavior.BufferSizeAsync();

            if (this.TextDescriptor.TextBehavior.Editing)
            {
                this.TextDescriptor.TextBehavior.Focus();
            }

            if (firstRender)
                this.Init();

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
