using BlazeCardsCore.Components;
using BlazeCardsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Descriptors
{
    public class VerticalListCard : Card
    {
        public bool Fixed { get; set; }

        public VerticalListCard(bool @fixed = false)
        {
            this.Fixed = @fixed;
        }

        public override Vector2f GetSize()
        {
            var size = Vector2f.Zero;

            foreach (var child in this.Children)
            {
                var childSize = child.GetSize();
                size.Y += childSize.Y;

                if (childSize.X > size.X)
                    size.X = childSize.X;
            }

            return size;
        }

        public override void Snap()
        {
            base.Snap();

            Console.WriteLine($"List snapping {this.Children.Count} children...");

            if (!this.Fixed) this.Children = this.Children.OrderBy(card => card.GetPosition().Y).ToList();

            var offset = 0f;
            foreach (var child in this.Children)
            {
                child.PositionBehavior.Position = new Vector2f(0, offset);
                offset += child.GetSize().Y;
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
