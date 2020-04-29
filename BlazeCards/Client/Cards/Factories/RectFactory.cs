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

        public static RectCard CreateHighlighter(IList<Card> cards)
        {
            if (cards.Count <= 0) return null;

            var pos = cards[0].GetGlobalPosition();
            var size = cards[0].GetSize() + pos;

            var notedPos = cards[0].GetGlobalPosition();
            foreach (var card in cards)
            {
                var cardPos = card.GetGlobalPosition();
                var cardRightBottom = card.GetSize() + cardPos;
                if (cardPos.X < pos.X) pos.X = cardPos.X;
                if (cardPos.Y < pos.Y) pos.Y = cardPos.Y;

                if (cardRightBottom.X > size.X)
                    size.X = cardRightBottom.X;

                if (cardRightBottom.Y > size.Y)
                    size.Y = cardRightBottom.Y;
            }

            size -= pos;

            var highlighter = new RectCard();
            highlighter.PositionBehavior.Position = pos;
            highlighter.SizeBehavior.Size = size;
            highlighter.Clickable = false;
            highlighter.Classes.Add("card-highlighter");

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
