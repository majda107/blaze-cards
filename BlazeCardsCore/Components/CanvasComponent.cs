﻿using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Factories;
using BlazeCardsCore.Models;
using BlazeCardsCore.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Components
{
    public class CanvasComponent : ComponentBase
    {
        public CardState State { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public IList<Card> Cards { get; set; }

        public int Sequence { get; set; }

        public ElementReference CanvasReference { get; private set; }
        public BoundingClientRect Box { get; private set; }

        public CanvasComponent()
        {
            this.State = new CardState();
            this.Cards = new List<Card>();
            this.Sequence = 0;


            this.Cards.Add(new RectCard());
            this.Cards.Add(new TextCard());


            var list = new VerticalListCard(false);
            list.AddChild(new RectCard());
            list.AddChild(new RectCard());
            list.AddChild(new TextCard());
            list.AddChild(new RectCard());
            list.AddChild(new RectCard());
            list.PositionBehavior.Position = new Vector2f(100, 100);

            var list2 = new HorizontalListCard(false, 10);
            list2.AddChild(new RectCard());
            list2.AddChild(new RectCard());
            list2.AddChild(new TextCard());
            list2.PositionBehavior.Position = new Vector2f(100, 400);

            this.Cards.Add(list);
            this.Cards.Add(list2);
            //this.Cards.Add(new CardComponent());
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                this.Box = await JSRuntime.InvokeAsync<BoundingClientRect>("getBoudingRect", this.CanvasReference);

            await base.OnAfterRenderAsync(firstRender);
        }

        public void InvokeChange() => this.StateHasChanged();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            this.Sequence = 0;
            builder.OpenElement(this.Sequence++, "svg");
            builder.AddAttribute(this.Sequence++, "class", "canvas");

            builder.AddAttribute(this.Sequence++, "onmousedown", EventCallback.Factory.Create(this, (e) =>
            {
                // broken event propag
                if (this.State.ComponentClicked)
                {
                    this.State.ComponentClicked = false;
                    return;
                }

                if (this.State.Selected.Count > 0)
                {
                    this.State.Selected.Clear();
                    this.State.Highlighter = null;
                    //return;
                }


                var pos = new Vector2f((float)e.ClientX, (float)e.ClientY);
                pos.ToLocalFromClient(this.Box);
                this.State.Selector = RectFactory.CreateSelector(pos);


                Console.WriteLine("canvas down...");
                this.State.Mouse.OnDown(new Vector2f((float)e.ClientX, (float)e.ClientY));
            }));

            builder.AddAttribute(this.Sequence++, "onmousemove", EventCallback.Factory.Create<MouseEventArgs>(this, async (e) =>
            {
                this.State.Mouse.OnMove(new Vector2f((int)e.ClientX, (int)e.ClientY));
            }));

            builder.AddAttribute(this.Sequence++, "onmouseup", EventCallback.Factory.Create<MouseEventArgs>(this, async (e) =>
            {
                this.State.Mouse.OnUp(new Vector2f((int)e.ClientX, (int)e.ClientY));
                //this.State.Selected = null;
                //this.State.Highlighter = null;

                if (this.State.Selector != null)
                {
                    var selectorBox = BoundingRect.FromPositionSize(this.State.Selector.GetGlobalPosition(), this.State.Selector.GetSize());
                    var traversed = new List<Card>();
                    foreach (var card in this.Cards)
                    {
                        card.TraverseOverlap(selectorBox, traversed);
                    }

                    if (traversed.Count > 0)
                    {
                        Console.WriteLine($"Selecting {traversed.Count} items...");

                        foreach (var traversedCard in traversed)
                            this.State.Selected.Add(traversedCard);
                        //this.State.Selected = traversedCard
                        //this.State.Highlighter = RectFactory.CreateHighlighter(traversed.First());
                        this.State.Highlighter = RectFactory.CreateHighlighter(traversed);
                        //break;
                    }

                    //Console.WriteLine("No overlap");

                    this.State.Selector = null;
                }
            }));



            builder.OpenElement(this.Sequence++, "g");
            builder.AddAttribute(this.Sequence++, "class", "canvas-zoom");

            builder.OpenElement(this.Sequence++, "g");
            builder.AddAttribute(this.Sequence++, "class", "canvas-graphics");

            foreach (var card in this.Cards)
            {
                //card.Render().Invoke(builder);
                builder.OpenComponent(this.Sequence++, card.GetComponentType());
                builder.AddAttribute(this.Sequence++, "Canvas", this);
                builder.AddAttribute(this.Sequence++, "Descriptor", card);
                builder.CloseComponent();
            }



            if (this.State.Selector != null)
            {
                builder.OpenComponent(this.Sequence++, this.State.Selector.GetComponentType());
                builder.AddAttribute(this.Sequence++, "Canvas", this);
                builder.AddAttribute(this.Sequence++, "Descriptor", this.State.Selector);
                builder.CloseComponent();
            }
            else this.Sequence += 3;

            //Console.WriteLine("Re-rendering highlighter");
            if (this.State.Highlighter != null)
            {
                builder.OpenComponent(this.Sequence++, this.State.Highlighter.GetComponentType());
                builder.AddAttribute(this.Sequence++, "Canvas", this);
                builder.AddAttribute(this.Sequence++, "Descriptor", this.State.Highlighter);
                builder.CloseComponent();
            }
            else this.Sequence += 3;


            builder.AddElementReferenceCapture(this.Sequence++, (eref) =>
            {
                this.CanvasReference = eref;
            });

            builder.CloseElement();
            builder.CloseElement();

            builder.CloseElement();
        }
    }
}