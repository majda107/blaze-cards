﻿using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BlazeCardsCore.Factories
{
    public static class RectFactory
    {
        //public static RectCard CreateHighlighter(Card card)
        //{
        //    //Console.WriteLine($"Creatin highlighter!! {card.GetType()} {card.Highlightable}");
        //    if (!card.Highlightable) return null;

        //    var highlighter = new RectCard();
        //    highlighter.PositionBehavior.Position = card.GetGlobalPosition();
        //    highlighter.SizeBehavior.Size = card.GetSize();
        //    highlighter.Clickable = false;
        //    highlighter.Classes.Add("card-highlighter");
        //    //highlighter.PositionBehavior.Update();

        //    //Console.WriteLine($"Created highlighter with size: {highlighter.SizeBehavior.Width} {highlighter.SizeBehavior.Height}, position {highlighter.PositionBehavior.Position.X}, {highlighter.PositionBehavior.Position.Y}");
        //    return highlighter;
        //}

        public static RectCard CreateHighlighter(IList<Card> cards)
        {
            if (cards.Count == 1 && !cards[0].Highlightable) return null;
            if (cards.Count <= 0) return null;

            var pos = cards[0].GetGlobalPosition();
            var size = cards[0].GetSize() + pos;

            //Console.WriteLine($"Creating highlighter with BASE size x:{cards[0].GetSize().X} y:{cards[0].GetSize().Y}");

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
            selector.Classes.Add("card-selector");

            return selector;
        }
    }
}
