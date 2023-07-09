

using System;
using static PokerHand;

public class PokerHandState 
{

    public GameCard[] myCards = new GameCard[5];
    public int[] numberCounts;
    public int[] colorCounts;
    public PokerHand pokerHand;

    public PokerHandState(int numClass, int numColor)
    {
        numberCounts = new int[numClass];
        colorCounts = new int[numColor];
    }
    public string HandToString() {
        string str = "";
        foreach (GameCard card in myCards) {
            str += card.ToString() + " / ";
        }
        return str;
    }
   private void ResetCountBin()
    {

        for (int i = 0; i < numberCounts.Length; i++)
        {
            numberCounts[i] = 0;
        }
        for (int i = 0; i < colorCounts.Length; i++)
        {
            colorCounts[i] = 0;
        }
        for (int i = 0; i < myCards.Length; i++)
        {
            GameCard card = myCards[i];
            colorCounts[(int)card.GetColor()]++;
            numberCounts[(int)card.GetClass()]++;
        }

    }
    public void CloneState(PokerHandState pState) {
        for (int i = 0; i < myCards.Length; i++) {
            myCards[i] = new GameCard((int)pState.myCards[i].GetClass(), (int)pState.myCards[i].GetColor());
        }
    }

    public void CalculateHand( )
    {
        ResetCountBin();
        bool royal = isRoyal();
        int straightNum = isStraight();
        int flushCol = isFlush();

        if (royal && flushCol != -1)
        {
            pokerHand = new PokerHand(PokerHandType.ROYAL_STRAIGHT_FLUSH);
            return;
        }
        if (straightNum != -1 && flushCol != -1)
        {
            pokerHand = new PokerHand((CardClass)straightNum, PokerHandType.STRAIGHT_FLUSH);
            return;
        }
        int[] nkind = isN_kind();
        if (nkind[0] == 5)
        {
            pokerHand = new PokerHand((CardClass)nkind[1], PokerHandType.FIVE_CARDS);
            return;
        }
        else if (nkind[0] == 4)
        {
            pokerHand = new PokerHand((CardClass)nkind[1], PokerHandType.FOUR_CARDS);
            return;
        }
        int fullHouseNum = isFullHouse();
        if (fullHouseNum != -1)
        {
            pokerHand = new PokerHand((CardClass)fullHouseNum, PokerHandType.FULL_HOUSE);
            return;
        }
        if (flushCol != -1)
        {
            pokerHand = new PokerHand((CardColor)flushCol, PokerHandType.FLUSH);
            return;
        }
        if (straightNum != -1)
        {
            pokerHand = new PokerHand((CardClass)straightNum, PokerHandType.STRAIGHT);
            return;
        }
        if (nkind[0] == 3)
        {
            pokerHand = new PokerHand((CardClass)nkind[1], PokerHandType.TRIPLE);
            return;
        }
        int[] nPair = isN_pair();
        if (nPair[0] == 2)
        {
            pokerHand = new PokerHand((CardClass)nPair[1], PokerHandType.TWO_PAIRS);
            return;
        }
        if (nPair[0] == 1)
        {
            pokerHand = new PokerHand((CardClass)nPair[1], PokerHandType.ONE_PAIR);
            return;
        }

        int tophand = isN_top();
        pokerHand = new PokerHand((CardClass)tophand, PokerHandType.TOP);
        return;
    }




    #region Poker Identifiers

    public bool isRoyal()
    {
        for (int i = 3; i < numberCounts.Length; i++)
        {
            if (numberCounts[i] == 0)
            {
                return false;
            };
        }
        return true;
    }
    public int isFlush()
    {
        //Return flush color
        for (int i = 0; i < colorCounts.Length; i++)
        {
            if (colorCounts[i] == 5)
            {
                return i;
            };
        }
        return -1;
    }
    public int isStraight()
    {
        //Return highest number
        //a k q j 10
        bool streak = false;
        int series = 0;
        for (int i = 0; i < numberCounts.Length; i++)
        {
            if (numberCounts[i] == 1)
            {
                if (!streak)
                {
                    //Start streak
                    streak = true;
                }
                series++;
                if (series == 5)
                {

                    return i;
                }
            }
            else
            {
                if (streak)
                {//Was in streak, no streak = not sttraight
                    if (CheckBackStraight())
                    {
                        return (int)CardClass.Ace;
                    }
                    return -1;
                }
            };
        }
        if (CheckBackStraight())
        {
            return (int)CardClass.Ace;
        }
        return -1;
    }
    public bool CheckBackStraight()
    {
        return (numberCounts[(int)CardClass.Seven] == 1 &&
            numberCounts[1] == 1 &&
            numberCounts[2] == 1 &&
            numberCounts[3] == 1 &&
            numberCounts[(int)CardClass.Ace] == 1);
    }

    public int[] isN_kind()
    { // nkind, highest card
        int nkind = 0;
        int highestCard = -1;
        for (int i = 0; i < numberCounts.Length; i++)
        {
            if (numberCounts[i] > nkind)
            {
                nkind = numberCounts[i];
                highestCard = i;
            }

        }
        nkind = (nkind < 3) ? -1 : nkind;
        return new int[2] { nkind, highestCard };
    }
    public int[] isN_pair()
    {
        //[numpair , highest pair]
        int numPair = 0;
        int highestPair = -1;
        for (int i = 0; i < numberCounts.Length; i++)
        {
            if (numberCounts[i] == 2)
            {
                numPair++;
                if (i > highestPair) highestPair = i;
            }
        }
        return new int[2] { numPair, highestPair };
    }
    public int isN_top()
    {
        int highest = -1;
        for (int i = 0; i < numberCounts.Length; i++)
        {
            if (numberCounts[i] > 0 && i > highest) highest = i;
        }
        return highest;
    }
    public int isFullHouse()
    {
        bool foundTwo = false;
        bool foundThree = false;
        int cardOfThree = -1;
        for (int i = 0; i < numberCounts.Length; i++)
        {
            if (numberCounts[i] == 2)
            {
                foundTwo = true;
            }
            if (numberCounts[i] == 3)
            {
                foundThree = true;
                cardOfThree = i;
            }
            if (foundThree && foundTwo)
            {
                return cardOfThree;

            }
        }
        return -1;
    }
    #endregion Poker Identifiers
}
