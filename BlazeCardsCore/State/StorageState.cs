using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.State
{
    public class StorageState
    {
        public CardState State { get; private set; }
        public IList<Card> Cards { get; private set; }

        public StorageState(CardState state)
        {
            this.State = state;
            this.Cards = new List<Card>();

            this.Init();
        }

        private void Init()
        {
            for (int i = 0; i < 20; i++)
            {
                var card = new RectCard();
                card.PositionBehavior.Position = new Vector2f(i * 30 + 300, i % 10 * 30 + 300);
                this.Cards.Add(card);

                if (i <= 10)
                {
                    var textCard = new TextCard();
                    textCard.PositionBehavior.Position = new Vector2f(i * 30 + 340, i % 10 * 30 + 300);
                    this.Cards.Add(textCard);
                }
                else
                {
                    var textBlockCard = new TextBlockCard();
                    textBlockCard.PositionBehavior.Position = new Vector2f(i * 30 + 340, i % 10 * 30 + 300);
                    this.Cards.Add(textBlockCard);
                }
            }


            var list = new VerticalListCard(false);
            list.AddChild(new RectCard());
            list.AddChild(new TextCard());

            var drop = new DropAreaCard();
            drop.OnDrop += (o, e) =>
            {
                //this.State.Canvas.JSRuntime.InvokeVoidAsync("blazeAlert", $"{e.Cards.Count} has been dropped...");

                foreach (var card in e.Cards) // add on drop
                {
                    if (this.Cards.Contains(card)) this.Cards.Remove(card);
                    list.AddChild(card);
                }

            };

            list.AddChild(drop);

            list.PositionBehavior.Position = new Vector2f(300, 740);

            var innerList = new HorizontalListCard(false, 10);
            innerList.AddChild(new RectCard());
            innerList.AddChild(new TextCard());
            innerList.AddChild(new RectCard());

            list.AddChild(innerList);

            this.Cards.Add(list);
        }
    }
}
