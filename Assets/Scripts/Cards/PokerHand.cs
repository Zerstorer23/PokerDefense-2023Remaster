using static LocalizationManager;
public class PokerHand 
{
    public PokerHandType type;
    CardClass topClass;
    CardColor topColor;
    public PokerHand( PokerHandType htype)
    {
        this.type = htype;
        if (htype == PokerHandType.ROYAL_STRAIGHT_FLUSH) topClass = CardClass.Ace;
    }
    public PokerHand(CardClass cclass, PokerHandType htype) {
        this.topClass = cclass;
        this.type = htype;
    }
    public PokerHand(CardColor ccolor, PokerHandType htype)
    {
        this.topColor = ccolor;
        this.type = htype;
    }
    public PokerHand(CardClass cclass, CardColor ccolor, PokerHandType htype)
    {
        this.topClass = cclass;
        this.topColor = ccolor;
        this.type = htype;
    }
    public CardClass GetTopClass() => topClass;
    public CardColor GetTopColor() =>topColor;


    public string GetName() {

        switch (type) {
            case PokerHandType.ROYAL_STRAIGHT_FLUSH:
                return Convert("TXT_KEY_ROYAL_STRAIGHT_FLUSH");
            case PokerHandType.STRAIGHT_FLUSH:
                return GetClassString(topClass)+" " +Convert("TXT_KEY_STRAIGHT_FLUSH");
            case PokerHandType.FIVE_CARDS:
                return GetClassString(topClass)+" " + Convert("TXT_KEY_FIVE_CARDS");
            case PokerHandType.FOUR_CARDS:
               return GetClassString(topClass)+" " + Convert("TXT_KEY_FOUR_CARDS");
            case PokerHandType.FULL_HOUSE:
                return GetClassString(topClass)+" " + Convert("TXT_KEY_FULL_HOUSE");
            case PokerHandType.FLUSH:
                return GetColorString(topColor)+" " + Convert("TXT_KEY_FLUSH");
            case PokerHandType.STRAIGHT:
                return GetClassString(topClass)+" "+ Convert("TXT_KEY_STRAIGHT");
            case PokerHandType.TRIPLE:
                return GetClassString(topClass)+" " + Convert("TXT_KEY_TRIPLE");
            case PokerHandType.TWO_PAIRS:
                return GetClassString(topClass)+" " + Convert("TXT_KEY_TWO_PAIRS");
            case PokerHandType.ONE_PAIR:
                return GetClassString(topClass)+" " + Convert("TXT_KEY_ONE_PAIR");
            case PokerHandType.TOP:
                return GetClassString(topClass)+" " + Convert("TXT_KEY_TOP");
            default:
                return "UNKNOWN";
        }

    
    }
    public static string GetClassOfHand(PokerHandType hand) {
        switch (hand)
        {
            case PokerHandType.ROYAL_STRAIGHT_FLUSH:
                return Convert("TXT_KEY_ROYAL_STRAIGHT_FLUSH");
            case PokerHandType.STRAIGHT_FLUSH:
                return Convert("TXT_KEY_STRAIGHT_FLUSH");
            case PokerHandType.FIVE_CARDS:
                return Convert("TXT_KEY_FIVE_CARDS");
            case PokerHandType.FOUR_CARDS:
                return Convert("TXT_KEY_FOUR_CARDS");
            case PokerHandType.FULL_HOUSE:
                return Convert("TXT_KEY_FULL_HOUSE");
            case PokerHandType.FLUSH:
                return Convert("TXT_KEY_FLUSH");
            case PokerHandType.STRAIGHT:
                return Convert("TXT_KEY_STRAIGHT");
            case PokerHandType.TRIPLE:
                return Convert("TXT_KEY_TRIPLE");
            case PokerHandType.TWO_PAIRS:
                return Convert("TXT_KEY_TWO_PAIRS");
            case PokerHandType.ONE_PAIR:
                return Convert("TXT_KEY_ONE_PAIR");
            case PokerHandType.TOP:
                return Convert("TXT_KEY_TOP");
            default:
                return "UNKNOWN";
        }
    }

    public PokerHandType GetHandType() => type;
    public string GetClassString(CardClass cardClass) {
        switch (cardClass)
        {
            case CardClass.Seven:
                return Convert("TXT_KEY_SEVEN");
             
            case CardClass.Eight:
                return Convert("TXT_KEY_EIGHT");
             
            case CardClass.Nine:
                return Convert("TXT_KEY_NINE");
             
            case CardClass.Ten:
                return Convert("TXT_KEY_TEN");
             
            case CardClass.Jack:
                return Convert("TXT_KEY_JACK");
             
            case CardClass.Queen:
                return Convert("TXT_KEY_QUEEN");
             
            case CardClass.King:
                return Convert("TXT_KEY_KING");
             
            case CardClass.Ace:
                return Convert("TXT_KEY_ACE");
             
        }
        return null;

    }
    public string GetColorString(CardColor cardColor)
    {
        switch (cardColor)
        {
            case CardColor.Club:
                return Convert("TXT_KEY_BLUE");
            case CardColor.Spade:
                return Convert("TXT_KEY_YELLOW");
            case CardColor.Diamond:
                return Convert("TXT_KEY_RED");
        }
        return null;
    }

    public enum PokerHandType{ 
        TOP      ,
        ONE_PAIR ,
        TWO_PAIRS,
        TRIPLE   ,
        STRAIGHT ,
        FLUSH    ,
        FULL_HOUSE,
        FOUR_CARDS,
        FIVE_CARDS,
        STRAIGHT_FLUSH,
        ROYAL_STRAIGHT_FLUSH
    }
}