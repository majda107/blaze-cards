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
        public SelectionBehavior SelectionBehavior { get; private set; }

        public bool Editable { get; set; }
        public TextCard() : base()
        {
            this.TextBehavior = new TextBehavior(this);
            this.SelectionBehavior = new SelectionBehavior(this);

            this.Editable = true;

            this.OnUp += (s, e) =>
            {
                if (this.TextBehavior.Value == "")
                {
                    Console.WriteLine("Deleting empty text card!");
                    s.UnhookFromParent();
                }

                s.InvokeComponentChange();
            };
        }

            
        public override Type GetComponentType() => typeof(TextComponent);

        public override void AssignComponent(CardComponent component)
        {
            base.AssignComponent(component);
            this.TextBehavior.AssignComponent(component as TextComponent);
        }

        public override Vector2f GetSize()
        {
            return this.TextBehavior.BufferedSize.Size;
        }

        public virtual async Task BufferSizeAsync()
        {
            await this.TextBehavior.BufferSizeAsync();
        }
    }
}
