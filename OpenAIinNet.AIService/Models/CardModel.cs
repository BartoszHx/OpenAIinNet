using System.ComponentModel;

namespace OpenAIinNET.AI.Models
{
    public class CardModel
    {
        [Description("Card Price")]
        public double Price { get; set; }

        [Description("Price currency")]
        public string Currency { get; set; }

        [Description("Card Name")]
        public string Name { get; set; }
    }
}
