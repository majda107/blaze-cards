using BlazeCardsCore.Behaviors;
using BlazeCardsCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazeCardsCore.Descriptors
{
    class TextBlockCard : TextCard
    {
        private SizeBehavior sizeBehavior;
        public TextBlockCard()
        {
            this.sizeBehavior = new SizeBehavior();
        }
        public override Type GetComponentType()
        {
            return typeof(TextBlockComponent);
        }

        public override async Task BufferSizeAsync()
        {
            await base.BufferSizeAsync();

            Console.WriteLine("Buffering size from textblockcard...");
            this.sizeBehavior.Size = this.TextBehavior.BufferedSize;
            this.sizeBehavior.SetWidthHeightAttribute((this.Component as TextBlockComponent).TextBackgroundReference);
        }

        public override void AssignComponent(CardComponent component)
        {
            base.AssignComponent(component);
            this.sizeBehavior.AssignComponent(component);
        }
    }
}
