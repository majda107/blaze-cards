using BlazeCardsCore.Behaviors;
using BlazeCardsCore.Components;
using BlazeCardsCore.Models;
using BlazeCardsCore.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Descriptors
{
    public abstract class Card
    {
        public delegate void CardEventHandler(Card sender, Vector2f pos);

        public event CardEventHandler OnDown;
        public event CardEventHandler OnUp;
        public event CardEventHandler OnMove;
        public event CardEventHandler OnClick;

        public CardComponent Component { get; set; }
        public PositionBehavior PositionBehavior { get; private set; }

        public IList<Card> Children { get; protected set; }
        public Card Parent { get; set; }


        public bool Clickable { get; set; }
        public bool Draggable { get; set; }
        public bool Highlightable { get; set; }


        private bool _visible;
        public bool Visible
        {
            get => this._visible;
            set
            {
                if (value && !this.Classes.Contains("visible"))
                    this.Classes.Add("visible");
                else if (!value && this.Classes.Contains("visible"))
                    this.Classes.Remove("visible");

                this._visible = value;
            }
        }


        public List<string> Classes { get; set; }

        public Card(Card parent = null)
        {
            this.PositionBehavior = new PositionBehavior();
            this.Children = new List<Card>();
            this.Parent = parent;

            this.Clickable = true;
            this.Draggable = true;
            this.Highlightable = true;
            this.Classes = new List<string>();

            this.Visible = true;
        }

        public void FireDown(Vector2f position) => this.OnDown?.Invoke(this, position);
        public void FireUp() => this.OnUp?.Invoke(this, Vector2f.Zero);
        public void FireClick() => this.OnClick?.Invoke(this, Vector2f.Zero);
        public void FireMove() => this.OnMove?.Invoke(this, Vector2f.Zero);



        public virtual void AssignComponent(CardComponent component)
        {
            this.Component = component;
            this.PositionBehavior.AssignComponent(component);
        }

        public void InvokeComponentChange() => this.Component?.InvokeChange();

        public virtual Type GetComponentType() => typeof(CardComponent);


        public virtual Vector2f GetPosition() => this.PositionBehavior.Position;
        public abstract Vector2f GetSize();


        public Vector2f GetGlobalPosition()
        {
            if (this.Parent == null) return this.GetPosition();
            return this.Parent.GetGlobalPosition() + this.GetPosition();
        }

        public virtual void Snap() { this.Parent?.Snap(); }
        public virtual void Update() { this.Parent?.Update(); }


        public void AddChild(Card child)
        {
            if (child.Parent != null) child.UnhookFromParent();

            child.Parent = this;
            this.Children.Add(child);

            this.InvokeComponentChange();
            this.Snap();
        }

        public void UnhookChild(Card child)
        {
            if (!this.Children.Contains(child)) return;

            this.Children.Remove(child);
            this.InvokeComponentChange();

            child.Parent = null;
        }

        public void UnhookFromParent() => this.Parent.UnhookChild(this);


        // substitute this into hierarchy manager 
        public bool HasDescendant(Func<Card, bool> comparer)
        {
            foreach (var child in this.Children)
            {
                if (comparer(child)) return true;
                if (child.HasDescendant(comparer)) return true;
            }

            return false;
        }

        public void Deselect() => this.Component?.Deselect();
        public void TraverseCard(Func<Card, bool> comparer, IList<Card> cards)
        {
            if (comparer(this)) cards.Add(this);

            foreach (var child in this.Children)
                child.TraverseCard(comparer, cards);
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
                child.TraverseOverlap(box, cards);
        }

        public void TraverseTouches(BoundingRect box, IList<Card> cards)
        {
            var thisBox = BoundingRect.FromPositionSize(this.GetGlobalPosition(), this.GetSize());

            if (box.Right > thisBox.Left && box.Left < thisBox.Right && box.Bottom > thisBox.Top && box.Top < thisBox.Bottom)
            {
                cards.Add(this);
                return;
            }

            foreach (var child in this.Children)
                child.TraverseTouches(box, cards);
        }
    }
}
