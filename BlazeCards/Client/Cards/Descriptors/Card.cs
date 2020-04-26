using BlazeCards.Client.Cards.Behaviors;
using BlazeCards.Client.Cards.Components;
using BlazeCards.Client.Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.Descriptors
{
    public abstract class Card
    {
        public CardComponent Component { get; set; }
        public PositionBehavior PositionBehavior { get; private set; }

        public IList<Card> Children { get; private set; }
        public Card Parent { get; set; }

        public Card(Card parent = null)
        {
            // move to asign component
            //this.PositionBehavior = new PositionBehavior(this.Component);
            this.PositionBehavior = new PositionBehavior();
            this.Children = new List<Card>();
            this.Parent = parent;
        }

        public virtual void AssignComponent(CardComponent component)
        {
            Console.WriteLine("Assigning card component");

            this.Component = component;
            this.PositionBehavior.AssignComponent(component);
        }

        public virtual Type GetComponentType() => typeof(CardComponent);


        public virtual Vector2f GetPosition() => this.PositionBehavior.Position;
        public abstract Vector2f GetSize();



        public virtual void Snap() { this.Parent?.Snap(); }
        public void AddChild(Card child)
        {
            child.Parent = this;
            this.Children.Add(child);
            this.Snap();
        }
    }
}
