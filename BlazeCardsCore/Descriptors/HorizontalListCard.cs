using BlazeCardsCore.Components;
using BlazeCardsCore.Models;
using BlazeCardsCore.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeCardsCore.Descriptors
{
    class HorizontalListCard : Card
    {
        public bool Fixed { get; set; }
        public int Spacing { get; set; }

        public HorizontalListCard(bool @fixed = false, int spacing = 0)
        {
            this.Fixed = @fixed;
            this.Spacing = spacing;
        }

        public override Vector2f GetSize()
        {
            var size = Vector2f.Zero;

            foreach (var child in this.Children)
            {
                var childSize = child.GetSize();
                size.X += childSize.X + this.Spacing;

                if (childSize.Y > size.Y)
                    size.Y = childSize.Y;
            }

            if (this.Children.Count > 0)
                size.X -= this.Spacing;

            return size;
        }

        public override void Snap()
        {
            base.Snap();

            //Console.WriteLine($"List snapping {this.Children.Count} children...");

            if (!this.Fixed) this.Children = this.Children.OrderBy(card => card.GetPosition().X).ToList();

            var offset = 0f;
            foreach (var child in this.Children)
            {
                child.PositionBehavior.Position = new Vector2f(offset, 0);
                offset += child.GetSize().X + this.Spacing;
            }
        }

        public override void Update()
        {
            base.Update();
            this.Snap();
        }

        public override Type GetComponentType() => typeof(ListComponent);
    }
}
