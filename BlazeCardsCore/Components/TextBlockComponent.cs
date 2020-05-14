using BlazeCardsCore.Behaviors;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.Components
{
    public class TextBlockComponent : TextComponent
    {
        public ElementReference TextBackgroundReference { get; private set; }

        protected override void RenderTextAddition(RenderTreeBuilder builder, ref int seq)
        {
            builder.OpenElement(seq++, "rect");
            builder.AddAttribute(seq++, "x", "0");
            builder.AddAttribute(seq++, "y", "0");
            //builder.AddAttribute(seq++, "class", "text-background blaze-rect");
            this.BuildClassAttribute(builder, ref seq);

            this.HookMouseDown(builder, ref seq);

            if (this.TextDescriptor.Editable)
                this.HookDoubleClick(builder, ref seq);

            builder.AddElementReferenceCapture(seq++, eref =>
            {
                this.TextBackgroundReference = eref;
            });

            builder.CloseElement();
        }
    }
}
