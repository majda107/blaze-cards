using BlazeCardsCore.Behaviors;
using BlazeCardsCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazeCardsCore.Descriptors
{
    public class TextBlockCard : TextCard
    {
        public TextBlockCard()
        {
            this.Classes.Add("text-background");
            this.Classes.Add("blaze-rect");
        }
        public override Type GetComponentType()
        {
            return typeof(TextBlockComponent);
        }

        public override async Task BufferSizeAsync()
        {
            await base.BufferSizeAsync();

            this.TextBehavior.BufferedSize.SetWidthHeightAttribute((this.Component as TextBlockComponent).TextBackgroundReference);
        }
    }
}
