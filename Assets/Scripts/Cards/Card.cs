namespace Cards {
    public class Card {
        public int Number { get; }
        public CardEnum.CardSuit Suit { get; }
        public CardEnum.SuitColor SuitColor { get; }

        public bool IsVisible { get; set; }

        public Card(int number, CardEnum.CardSuit suit, CardEnum.SuitColor suitColor) {
            Number = number;
            Suit = suit;
            SuitColor = suitColor;
        }
    }
}