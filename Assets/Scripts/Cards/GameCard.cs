using System;

public class GameCard 
{
    CardClass cardClass;
    CardColor cardColor;



    public GameCard(int cclass, int ccolor) {
        cardClass = (CardClass)cclass;
        cardColor = (CardColor)ccolor;
    }
    public GameCard(CardClass cl, CardColor cc)
    {
        cardClass = cl;
        cardColor =cc;
    }

    public CardClass GetClass() {
        return cardClass;
    }

    public CardColor GetColor()
    {
        return cardColor;
    }

    override public string ToString() {
        return cardColor + " " + cardClass; 
    }

    internal bool IsSame(GameCard prevCard)
    {
        return cardClass == prevCard.cardClass && cardColor == prevCard.cardColor;
    }
}
