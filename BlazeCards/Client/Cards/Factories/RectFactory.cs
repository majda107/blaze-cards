using BlazeCards.Client.Cards.Descriptors;
using BlazeCards.Client.Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.Factories
{
    public static class RectFactory
    {
        public static RectCard CreateHighlighter(Card card)
        {
            var highlighter = new RectCard();
            highlighter.PositionBehavior.Position = card.GetGlobalPosition();
            highlighter.SizeBehavior.Size = card.GetSize();
            highlighter.Clickable = false;
            highlighter.Classes.Add("card-highlighter");
            //highlighter.PositionBehavior.Update();

            Console.WriteLine($"Created highlighter with size: {highlighter.SizeBehavior.Width} {highlighter.SizeBehavior.Height}, position {highlighter.PositionBehavior.Position.X}, {highlighter.PositionBehavior.Position.Y}");
            return highlighter;
        }

        public static RectCard CreateSelector(Vector2f pos)
        {
            var selector = new RectCard();
            selector.PositionBehavior.Position = pos;
            selector.SizeBehavior.Size = Vector2f.Zero;
            selector.SizeBehavior.HookNegativeSize(selector.PositionBehavior);
            selector.Classes.Add("card-highlighter");

            return selector;
        }
    }
}
