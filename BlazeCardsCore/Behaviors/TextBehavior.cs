using BlazeCardsCore.Components;
using BlazeCardsCore.Descriptors;
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
        public TextComponent Component { get; private set; }
        public TextCard Card { get; private set; }

        public SizeBehavior BufferedSize { get; private set; }
        public Vector2f Padding { get; set; }

        public string Value { get; set; }

        

        public bool Editing { get; set; }

        public TextBehavior(TextCard card)
        {
            this.Value = "default text";
            this.BufferedSize = new SizeBehavior();
            this.Padding = new Vector2f(6.0f, 3.0f);

            this.Card = card;
        }



        public void AssignComponent(TextComponent component)
        {
            this.Component = component;
            this.BufferedSize.AssignComponent(component);
        }

        public void Focus(bool prompt = false)
        {
            this.Component.Canvas.JSRuntime.InvokeVoidAsync("setFocus", this.Component.GetTextID(), prompt);
        }

        public async Task KeyDown(KeyboardEventArgs e, CanvasComponent canvas)
        {
            if (!this.Editing) return;

            if (e.Key.ToLower() == "arrowleft")
            {
                this.Card.SelectionBehavior.MoveLeft();
                await this.Card.SelectionBehavior.ExtentCaret(canvas);
                return;
            }

            if (e.Key.ToLower() == "arrowright")
            {
                this.Card.SelectionBehavior.MoveRight();
                await this.Card.SelectionBehavior.ExtentCaret(canvas);
                return;
            }

            if (e.Key.ToLower() == "backspace" || e.Key.ToLower() == "delete")
            {
                if (this.Card.SelectionBehavior.CorrectedBase == this.Card.SelectionBehavior.CorrectedExtent)
                {
                    if (e.Key.ToLower() == "delete")
                    {
                        this.Value = this.Value.Remove(this.Card.SelectionBehavior.CorrectedExtent, 1);
                    }
                    else if(this.Card.SelectionBehavior.BaseOffset > 0)
                    {
                        this.Value = this.Value.Remove(this.Card.SelectionBehavior.CorrectedExtent - 1, 1);
                        this.Card.SelectionBehavior.MoveLeft();
                        await this.Card.SelectionBehavior.ExtentCaret(canvas);
                    }
                }
                else
                {
                    this.Value = this.Value.Remove(this.Card.SelectionBehavior.CorrectedBase, this.Card.SelectionBehavior.CorrectedExtent - this.Card.SelectionBehavior.CorrectedBase);

                    this.Card.SelectionBehavior.ExtentOffset = this.Card.SelectionBehavior.CorrectedBase;
                    await this.Card.SelectionBehavior.ExtentCaret(canvas);
                }

                return;
            }

            if (e.Key.ToLower() == "space")
                e.Key = " "; // cheat lol

            if (e.Key.Length > 1) return;

            //if (this.Selection != StringSelection.Empty)
            //    this.Value = this.Selection.RemoveFrom(this.Value);

            this.Value = this.Value.Insert(this.Card.SelectionBehavior.ExtentOffset, e.Key.ToString());

            this.Card.SelectionBehavior.ExtentOffset += 1;
            this.Card.SelectionBehavior.BaseOffset = this.Card.SelectionBehavior.ExtentOffset;
            await this.Card.SelectionBehavior.ExtentCaret(canvas);
        }



        public async Task<BoundingClientRect> CalculateTextRect(string text)
        {
            var box = await this.Component.Canvas.JSRuntime.InvokeAsync<BoundingClientRect>("calculateTextRect", text);
            return box;
        }

        public async Task BufferSizeAsync()
        {
            var size = new Vector2f();

            var box = await this.Component.Canvas.JSRuntime.InvokeAsync<BoundingClientRect>("getBoudingRect", this.Component.GetTextID());
            size.X = (float)box.Width;
            size.Y = (float)box.Height;

            // oh god fix
            size /= this.Component.Canvas.State.Mouse.Zoom;

            size += this.Padding * 2f;

            if (!this.BufferedSize.Size.Equals(size))
            {
                //Console.WriteLine("Buffering new text size");
                this.BufferedSize.Size = size;
                this.Component.InvokeChange();
            }
        }
    }
}
