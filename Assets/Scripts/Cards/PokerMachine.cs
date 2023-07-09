using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static PokerHand;
using static PokerHandState;

public class PokerMachine : MonoBehaviour
{
    //UI
    [Header("UI Elements")]
    [SerializeField] Sprite[] spriteReferences;
    [SerializeField] GameCardObject[] cardsObjects;
    [SerializeField] Text handNameText;
    public GameObject visualCombinationBox;
    public GameObject KotoriPanel;
    [SerializeField] internal GameSession gameSession;
    [SerializeField] GameObject[] BoardUI;
   public bool[] cardsChanged = new bool[5];
    [SerializeField] TrendManager trendManager;
    [Header("Data elements")]
    //====
    [SerializeField] internal PokerMachineAI PokerAI;


    PokerHandState myHandState;
/*    GameCard[] myCards = new GameCard[5];
    int[] numberCounts;
    int[] colorCounts;*/

    //Need reset Everytime
    [SerializeField] internal int numColor;
    [SerializeField] internal int numClass;

/*    private void Start()
    {
        PokerTest3();
    }*/

    internal void DoTurn()
    {
        bool hasTrends = trendManager.CheckTrend(gameSession.waveManager.waveIndex);
        if (!hasTrends) {
            StartPokerGame();
        }
 
    }
    public void StartPokerGame() {
        DispenseCards();

        if (PokerAI.IsTurn(gameSession.waveManager.waveIndex))
        {
            PokerAI.SetPokerBoardAvailable(true);
            PokerAI.DoMove();
        }
        else
        {
            SetPokerBoardAvailable(true);
            PokerAI.SetPokerBoardAvailable(false);
        }
        UpdateCardUI();
        ShowMyHand();
    }

    public void DispenseCards()
    {
        myHandState = new PokerHandState(numClass, numColor);
        //Five cards
        for (int i = 0; i < myHandState.myCards.Length; i++) {
            myHandState.myCards[i] = GetRandomCard();
            cardsObjects[i].SetButtonVisibility(true);
            cardsChanged[i] = false;
        }

       // myHandState.myCards = GetFixedCard();
        PokerAI.CopyCards(myHandState,false);
    }

    public void SetPokerBoardAvailable(bool enable) {
        foreach (GameObject obj in BoardUI) {
            obj.SetActive(enable);
        }
        KotoriPanel.SetActive(enable);
    }

    private GameCard[] GetFixedCard() {

        return new GameCard[] {
            new GameCard(CardClass.Ace, CardColor.Club),
            new GameCard(CardClass.Seven, CardColor.Club),
            new GameCard(CardClass.Eight, CardColor.Diamond),
            new GameCard(CardClass.Nine, CardColor.Club),
            new GameCard(CardClass.Ten, CardColor.Club)
        };
    }

  

    public void ReRollCardAt(int i) {
        if (cardsChanged[i]) return;
        cardsChanged[i] = true;
        cardsObjects[i].SetButtonVisibility(false);
        Image img = cardsObjects[i].GetComponent<GameCardObject>().cardImage;
        img.DOFade(0, 0.25f)
            .OnComplete(
            () =>
            {
                GameCard prevCard = myHandState.myCards[i];
                GameCard newCard = GetRandomCard();
                while (newCard.IsSame(prevCard))
                {
                    newCard = GetRandomCard();
                }
                myHandState.myCards[i] = newCard;
                UpdateCardUI();
                img.DOFade(1, 0.25f);
                ShowMyHand();
            }
            );
    }

    public void ShowMyHand() {
        myHandState.CalculateHand();
        handNameText.text = myHandState.pokerHand.GetName();
        bool special = CheckSpecialCombination(false);
        visualCombinationBox.SetActive(special);
    }

    private bool CheckSpecialCombination(bool giveReward = true)
    {
        //RED FLUSH
        //Q or A top / pair / kinds / straight
        PokerHand myHand = myHandState.pokerHand;
        bool givePoint = false;
        switch (myHand.GetHandType())
        {
            case PokerHandType.STRAIGHT_FLUSH:
            case PokerHandType.FIVE_CARDS:
            case PokerHandType.ROYAL_STRAIGHT_FLUSH:
                givePoint = true;
                break;
            case PokerHandType.FLUSH:
                givePoint = (myHand.GetTopColor() == CardColor.Diamond);
                break;
            case PokerHandType.TOP:
                givePoint = (myHand.GetTopClass() == CardClass.Queen);
                break;
            case PokerHandType.ONE_PAIR:
                givePoint = (myHand.GetTopClass() == CardClass.Ace);
                break;
            case PokerHandType.TWO_PAIRS:
                givePoint = (myHand.GetTopClass() == CardClass.Ace);
                break;
            case PokerHandType.TRIPLE:
                givePoint = (myHand.GetTopClass() == CardClass.Queen);
                break;
            case PokerHandType.FULL_HOUSE:
                givePoint = (myHand.GetTopClass() == CardClass.Queen 
                            || myHand.GetTopClass() == CardClass.Ace);
                break;
            case PokerHandType.FOUR_CARDS:
                givePoint = (myHand.GetTopClass() == CardClass.Queen);
                break;
            case PokerHandType.STRAIGHT:
                givePoint = (myHand.GetTopClass() == CardClass.Ace);
                break;
        }
        if (givePoint && giveReward)
        {
            GiveVisualPoint(1);
        }
        return givePoint;
    }
    private void GiveVisualPoint(int a) {
        gameSession.mineralManager.AddResource(UpgradeType.VISUAL, a);
    }

    public void FinaliseHand() {
        if (myHandState == null) return;
        CheckSpecialCombination();
        EventManager.TriggerEvent(MyEvents.EVENT_POKERHAND_FINALISED, new EventObject(myHandState.pokerHand));
        EventManager.TriggerEvent(MyEvents.EVENT_SHOW_PANEL, new EventObject(ScreenType.MAP));
    }

    public GameCard GetRandomCard()
    {
        int cclass = Random.Range(0, numClass);
        int ctype = Random.Range(0, numColor);
        return new GameCard(cclass, ctype); ;
    }
    public void UpdateCardUI()
    {
        for (int i = 0; i <myHandState.myCards.Length; i++)
        {
            cardsObjects[i].SetCardImage(GetCardSprite(myHandState.myCards[i]));
        }
    }
    public Sprite GetCardSprite(GameCard card) {
        return spriteReferences[(int)card.GetColor() * numClass + (int)card.GetClass()];    
    }

    void PokerTest() {
        for (int x = 0; x < 100000; x++)
        {
           GameCard c = GetRandomCard();
            Debug.Log(c.GetClass());
            Debug.Log(c.GetColor());
        }
    }
    void PokerTest2()
    {
        for (int x = 0; x < 100000; x++)
        {
            DispenseCardsTest();
        }

    }
    void PokerTest3()
    {
        for (int x = 0; x < 10000; x++)
        {
            DispenseCardsTest();
        }

    }
    void DispenseCardsTest()
    {

        myHandState = new PokerHandState(numClass, numColor);
        //Five cards
        for (int i = 0; i < myHandState.myCards.Length; i++)
        {
            myHandState.myCards[i] = GetRandomCard();
        }
        PokerAI.TestMove(myHandState);
        // CheckSpecialCombination();
        Debug.Log(myHandState.pokerHand.type);
        
    }

    /// <summary>
    /// //
    /// </summary>
    /// <returns></returns>

}
