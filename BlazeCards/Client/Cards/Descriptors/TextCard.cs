using BlazeCards.Client.Cards.Behaviors;
using BlazeCards.Client.Cards.Components;
using BlazeCards.Client.Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.Descriptors
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
