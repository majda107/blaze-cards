using BlazeCardsCore.Behaviors;
using BlazeCardsCore.Components;
using BlazeCardsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Descriptors
{
    public class TextCard : Card
    {
        public TextBehavior TextBehavior { get; private set; }
        public TextCard() : base()
        {
            this.TextBehavior = new TextBehavior();
        }
        public override Type GetComponentType() => typeof(TextComponent);

        public override void AssignComponent(CardComponent component)
        {
            base.AssignComponent(component);
            this.TextBehavior.AssignComponent(component as TextComponent);
        }

        public override Vector2f GetSize()
        {
            return this.TextBehavior.BufferedSize;
        }
    }
}
