using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PokerHand;

[CreateAssetMenu(menuName = "Card Trade Rule")]
public class CardTradeRule : ScriptableObject
{

    [SerializeField] public List<UnitConfig> RoyalStraightFlush;
    [SerializeField] public List<UnitConfig> StraightFlush;
    [SerializeField] public List<UnitConfig> FiveCards;
    [SerializeField] public List<UnitConfig> FourCards;
    [SerializeField] public List<UnitConfig> FullHouse;
    [SerializeField] public List<UnitConfig> Flush;
    [SerializeField] public List<UnitConfig> Straight;
    [SerializeField] public List<UnitConfig> Triple;
    [SerializeField] public List<UnitConfig> TwoPairs;
    [SerializeField] public List<UnitConfig> OnePair;
    [SerializeField] public List<UnitConfig> Top;

    public List<UnitConfig> TradeToTower(PokerHand hand){
      //  Debug.Log("Trading " + hand.GetHandType());

        switch (hand.GetHandType())
        {
            case PokerHandType.ROYAL_STRAIGHT_FLUSH:
                return RoyalStraightFlush;
            case PokerHandType.STRAIGHT_FLUSH:
                return StraightFlush;
            case PokerHandType.FIVE_CARDS:
                return FiveCards;
            case PokerHandType.FOUR_CARDS:
                return FourCards;
            case PokerHandType.FULL_HOUSE:
                return FullHouse;
            case PokerHandType.FLUSH:
                return Flush;
            case PokerHandType.STRAIGHT:
                return Straight;
            case PokerHandType.TRIPLE:
                return Triple;
            case PokerHandType.TWO_PAIRS:
                return TwoPairs;
            case PokerHandType.ONE_PAIR:
                return OnePair;
            case PokerHandType.TOP:
                return Top;
            default:
                return null;
        }

    }

}
