﻿using BlazeCardsCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.Descriptors
{
    public class ButtonCard : TextBlockCard
    {
        public CardEventHandler ButtonAction { get; private set; }

        public ButtonCard(string name, CardEventHandler buttonAction)
        {
            this.ButtonAction = buttonAction;
            this.OnClick += this.ButtonAction;

            this.TextBehavior.Value = name;
            this.Highlightable = false;
            this.Editable = false;

            this.Classes.Add("blaze-button");

            this.OnDown += (s, e) =>
            {
                s.Classes.Add("blaze-button-down");
                s.InvokeComponentChange();
            };

            this.OnUp += (s, e) =>
            {
                s.Classes.Remove("blaze-button-down");
                s.InvokeComponentChange();
            };
        }
    }
}
