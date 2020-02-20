namespace WarCardGame
{
    public class Card
    {
        public int Value { get; set; }
        public Suit Suit { get; set; }

        public override string ToString()
        {
            switch (Value)
            {
                case 11:
                    return $"Jack of {Suit.ToString()}";
                case 12:
                    return $"Queen of {Suit.ToString()}";
                case 13:
                    return $"King of {Suit.ToString()}";
                case 14:
                    return $"Ace of {Suit.ToString()}";
                default:
                    return $"{Value} of {Suit.ToString()}";
            }
        }
    }

    public enum Suit
    {
        Diamonds,
        Clubs,
        Hearts,
        Spades
    }
}