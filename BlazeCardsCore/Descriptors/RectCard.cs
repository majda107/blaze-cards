using BlazeCardsCore.Behaviors;
using BlazeCardsCore.Components;
using BlazeCardsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Descriptors
{
    public class RectCard : Card
    {
        public SizeBehavior SizeBehavior { get; private set; }
        public RectCard() : base()
        {
            this.SizeBehavior = new SizeBehavior(30, 30);
        }

        public override Type GetComponentType() => typeof(RectComponent);

        public override void AssignComponent(CardComponent component)
        {
            base.AssignComponent(component);
            this.SizeBehavior.AssignComponent(component);
        }

        public override Vector2f GetSize()
        {
            return this.SizeBehavior.Size;
        }

        //public override void Update()
        //{
        //    this.SizeBehavior.set
        //    base.Update();
        //}
    }
}
