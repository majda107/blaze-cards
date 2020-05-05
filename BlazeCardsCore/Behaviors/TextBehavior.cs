using BlazeCardsCore.Components;
using BlazeCardsCore.Extension;
using BlazeCardsCore.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Behaviors
{
    public class TextBehavior
    {
        public TextComponent Card { get; private set; }
        public SizeBehavior BufferedSize { get; private set; }
        public Vector2f Padding { get; set; }

        public string Value { get; set; }
        public StringSelection Selection { get; set; }
        public float Caret { get => this.BufferedSize.Size.X; }

        public bool Editing { get; set; }

        public TextBehavior()
        {
            this.Value = "default text";
            this.BufferedSize = new SizeBehavior();
            this.Padding = new Vector2f(6.0f, 3.0f);
        }

        public void Highlight(int start, int end) => this.Selection = new StringSelection(start, end);

        public void AssignComponent(TextComponent card)
        {
            this.Card = card;
            this.BufferedSize.AssignComponent(card);
        }

        public void Focus()
        {
            this.Card.Canvas.JSRuntime.InvokeVoidAsync("setFocus", this.Card.GetTextID());
        }

        public void KeyDown(KeyboardEventArgs e)
        {
            if (!this.Editing) return;

            if (e.Key.ToLower() == "backspace")
            {
                if (this.Selection != StringSelection.Empty)
                    this.Value = this.Selection.RemoveFrom(this.Value);
                else
                    this.Value = this.Value.RemoveLast();

                return;
            }

            if (e.Key.ToLower() == "space")
            {
                this.Value += " ";
            }

            if (e.Key.ToString().Length > 1) return;

            this.Value += e.Key.ToString();
        }

        //public async Task GetCaretAsync()
        //{
        //    float res = await this.Card.Canvas.JSRuntime.InvokeAsync<float>("getTextWidth", this.Card.TextRef);
        //    if (res != this.Caret)
        //    {
        //        this.Caret = res;
        //        this.Card.InvokeChange();
        //    }
        //}

        public async Task<BoundingClientRect> CalculateTextRect(string text)
        {
            var box = await this.Card.Canvas.JSRuntime.InvokeAsync<BoundingClientRect>("calculateTextRect", text);
            return box;
        }

        public async Task BufferSizeAsync()
        {
            var size = new Vector2f();

            var box = await this.Card.Canvas.JSRuntime.InvokeAsync<BoundingClientRect>("getBoudingRect", this.Card.GetTextID());
            size.X = (float)box.Width;
            size.Y = (float)box.Height;

            // oh god fix
            size /= this.Card.Canvas.State.Mouse.Zoom;

            size += this.Padding * 2f;

            if (!this.BufferedSize.Size.Equals(size))
            {
                //Console.WriteLine("Buffering new text size");
                this.BufferedSize.Size = size;
                this.Card.InvokeChange();
            }
        }
    }
}
