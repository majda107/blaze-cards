using BlazeCards.Client.Cards.Behaviors;
using BlazeCards.Client.Cards.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.Descriptors
{
    public class Card
    {
        public CardComponent Component { get; set; }
        public PositionBehavior PositionBehavior { get; private set; }
        public Card()
        {
            // move to asign component
            //this.PositionBehavior = new PositionBehavior(this.Component);
            this.PositionBehavior = new PositionBehavior();
        }

        public void AssignComponent(CardComponent component)
        {
            Console.WriteLine("Assigning card component");

            this.Component = component;
            this.PositionBehavior.AssignComponent(component);
        }

    }
}
