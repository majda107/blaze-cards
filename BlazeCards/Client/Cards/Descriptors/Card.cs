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

        public bool Clickable { get; set; }
        public List<string> Classes { get; set; }

        public Card(Card parent = null)
        {
            // move to asign component
            //this.PositionBehavior = new PositionBehavior(this.Component);
            this.PositionBehavior = new PositionBehavior();
            this.Children = new List<Card>();
            this.Parent = parent;

            this.Clickable = true;
            this.Classes = new List<string>();
        }

        public virtual void AssignComponent(CardComponent component)
        {
            this.Component = component;
            this.PositionBehavior.AssignComponent(component);
        }

        public virtual Type GetComponentType() => typeof(CardComponent);


        public virtual Vector2f GetPosition() => this.PositionBehavior.Position;
        public abstract Vector2f GetSize();


        public Vector2f GetGlobalPosition()
        {
            if (this.Parent == null) return this.GetPosition();
            return this.Parent.GetPosition() + this.GetPosition();
        }

        public virtual void Snap() { this.Parent?.Snap(); }
        public void AddChild(Card child)
        {
            child.Parent = this;
            this.Children.Add(child);
            this.Snap();
        }

        // substitute this into hierarchy manager 
        public bool HasDescendant(Card card)
        {
            foreach (var child in this.Children)
            {
                if (child == card) return true;
                if (child.HasDescendant(card)) return true;
            }

            return false;
        }

        public void TraverseOverlap(BoundingRect box, IList<Card> cards)
        {
            var thisBox = BoundingRect.FromPositionSize(this.GetGlobalPosition(), this.GetSize());
            if (thisBox.Overlap(box))
            {
                cards.Add(this);
                return;
            }

            foreach (var child in this.Children)
            {
                child.TraverseOverlap(box, cards);
            }
        }
    }
}
