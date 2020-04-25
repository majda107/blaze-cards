using BlazeCards.Client.Cards.Behaviors;
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
        public TextBehavior TextBehavior { get; private set; }

        public ElementReference TextRef { get; private set; }

        public TextComponent()
        {
            this.TextBehavior = new TextBehavior(this);
        }

        protected override RenderFragment RenderInner()
        {
            return new RenderFragment(builder =>
            {
                builder.OpenElement(this.Canvas.Sequence++, "text");

                builder.AddAttribute(this.Canvas.Sequence++, "tabindex", "1");
                builder.AddAttribute(this.Canvas.Sequence++, "x", "0");
                builder.AddAttribute(this.Canvas.Sequence++, "y", "20");


                //builder.AddAttribute(this.Canvas.Sequence++, "onmousedown", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
                //{
                //    Console.WriteLine("lel");
                //}));

                builder.AddAttribute(this.Canvas.Sequence++, "ondblclick", EventCallback.Factory.Create<MouseEventArgs>(this, (e) =>
                {
                    this.TextBehavior.Editing = true;
                    this.Canvas.State.Selected = this;

                    Console.WriteLine("Editing...");
                }));



                //builder.AddAttribute(this.Canvas.Sequence++, "onfocusout", EventCallback.Factory.Create(this, () =>
                //{
                //    //this.TextBehavior.Editing = false;
                //    //this.Canvas.State.Selected = null;
                //}));

                builder.AddAttribute(this.Canvas.Sequence++, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, (e) =>
                {
                    Console.WriteLine("key down...");
                    this.TextBehavior.KeyDown(e);
                }));

                //this.HookMouseDown().Invoke(builder);


                builder.AddContent(this.Canvas.Sequence++, this.TextBehavior.Value);

                builder.AddElementReferenceCapture(this.Canvas.Sequence++, (eref) =>
                {
                    this.TextRef = eref;
                });

                builder.CloseElement();


                // add caret
                if (this.TextBehavior.Editing)
                {
                    builder.OpenElement(this.Canvas.Sequence++, "rect");
                    builder.AddAttribute(this.Canvas.Sequence++, "x", (this.TextBehavior.Caret + 4).ToString("0.0").Replace(',', '.'));
                    builder.AddAttribute(this.Canvas.Sequence++, "y", "2");
                    builder.AddAttribute(this.Canvas.Sequence++, "height", "20px");
                    builder.AddAttribute(this.Canvas.Sequence++, "width", "2px");
                    builder.AddAttribute(this.Canvas.Sequence++, "class", "card-caret");
                    builder.CloseElement();
                }
            });
        }

        private void Init()
        {

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (this.TextBehavior.Editing)
            {
                await this.TextBehavior.GetCaretAsync();
                this.TextBehavior.Focus();
            }

            if (firstRender)
                this.Init();

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
