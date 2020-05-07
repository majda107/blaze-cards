using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace BlazeCardsCore.State
{
    public class CharacterState
    {
        public CardState State { get; private set; }
        public Dictionary<char, BoundingClientRect> CharacterMap { get; private set; }

        public CharacterState(CardState state)
        {
            this.State = state;
            this.CharacterMap = new Dictionary<char, BoundingClientRect>();
        }

        public async Task<BoundingClientRect> Get(char c)
        {
            if (!this.CharacterMap.ContainsKey(c))
            {
                var box = await this.State.Canvas.JSRuntime.InvokeAsync<BoundingClientRect>("calculateTextRect", c.ToString());
                this.CharacterMap.Add(c, box);
            }

            return this.CharacterMap[c];
        }
    }
}
