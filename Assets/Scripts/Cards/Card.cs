namespace Cards {
    public class Card {
        public Card(int number, CardEnum.CardSuit suit, CardEnum.SuitColor suitColor) {
            Number = number;
            Suit = suit;
            SuitColor = suitColor;
        }

        public int Number { get; }
        public CardEnum.CardSuit Suit { get; }
        public CardEnum.SuitColor SuitColor { get; }

        public bool IsVisible { get; set; }

        public bool HasBeenInVerticalColumn { get; set; } = false;
    }
}