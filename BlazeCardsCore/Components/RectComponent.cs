using BlazeCardsCore.Behaviors;
using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Components
{
    public class RectComponent : CardComponent
    {
        //public SizeBehavior Size { get; private set; }

        public RectCard RectDescriptor { get => this.Descriptor as RectCard; }
        public ElementReference RectElement { get; private set; }
        public RectComponent()
        {
            //this.Size = new SizeBehavior(this, 30, 30);
        }


        protected override void RenderInner(RenderTreeBuilder builder, ref int seq)
        {
            builder.OpenElement(seq++, "rect");

            var classList = "blaze-rect";
            foreach (var elementClass in this.Descriptor.Classes)
                classList += $" {elementClass}";


            builder.AddAttribute(seq++, "class", classList);

            //foreach (var elementClass in this.Descriptor.Classes)
            //    builder.AddAttribute(seq++, "class", elementClass);


            this.HookMouseDown(builder, ref seq);

            builder.AddElementReferenceCapture(seq++, eref =>
            {
                this.RectElement = eref;
            });

            builder.CloseElement();
        }

        public override void OnBlazeRender()
        {
            this.RectDescriptor.SizeBehavior.SetWidthHeightAttribute(this.RectElement);
            base.OnBlazeRender();
        }
    }
}
