using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BlazeCardsCore.Models
{
    public class SelectionModel
    {
        [JsonPropertyName("baseOffset")]
        public int BaseOffset { get; set; }

        [JsonPropertyName("extentOffset")]
        public int ExtentOffset { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        public override string ToString()
        {
            return $"Selection: Base = {this.BaseOffset}, Extent = {this.ExtentOffset}, Type = {this.Type}";
        }

        public SelectionModel()
        {

        }

        public SelectionModel(int baseOffset, int extentOffset)
        {
            this.BaseOffset = baseOffset;
            this.ExtentOffset = extentOffset;
        }

        public void MoveLeft()
        {
            if (this.BaseOffset > 0)
                this.BaseOffset -= 1;

            this.ExtentOffset = this.BaseOffset;
        }

        public void MoveRight(int maxLen)
        {
            if (this.BaseOffset < maxLen - 1)
                this.BaseOffset += 1;

            this.ExtentOffset = this.BaseOffset;
        }
    }
}
