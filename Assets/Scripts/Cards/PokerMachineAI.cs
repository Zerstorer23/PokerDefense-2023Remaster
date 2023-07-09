using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static ConstantStrings;

public class PokerMachineAI : MonoBehaviour
{
    [SerializeField]  PokerMachine pokerMachine;
    [SerializeField] CardTradeRule tradeRule;
    internal Dictionary<string, UnitConfig> unitDictionary;

    [SerializeField] GameCardObject[] cardsObjects;
    [SerializeField] GameObject BoardUI;
    [SerializeField] Text handNameText;
    [SerializeField] bool debugOut = false;
    int numColor;
    int numClass;

    PokerHandState aiHandState;
    bool[] cardsChanged = new bool[5];
    double[] dps_list = new double[5];
    [SerializeField] public int intrudeAtWave = 0;
  //  [SerializeField] public int removeTowerTerm = 10;


    /// <summary>
    /// Tween
    /// </summary>
    Vector3 originaltLocation = new Vector3(200, 200);
    Vector3 originalScale = new Vector3(1f, 1f, 1f);
    Vector3 targetLocation = new Vector3(0, -300);
    Vector3 targetScale = new Vector3(0.5f, 0.5f, 1f);
    float duration = 0.5f;

    private void Awake()
    {
    //    Debug.Log("poker init start");
        InitNumbers();
      //  Debug.Log("local init end");
    }
    private void Start()
    {
        unitDictionary = GameSession.GetGameSession().GetUnitDictionary();
        Debug.Assert(unitDictionary != null, "No unit dictionary");
    }

    internal bool IsTurn(int waveIndex)
    {
        return (waveIndex+1) >= intrudeAtWave;
    }
    void InitNumbers() {

        numColor = pokerMachine.numColor;
        numClass = pokerMachine.numClass;
    }
    public void CopyCards(PokerHandState playerState, bool doCopy) {

        aiHandState = new PokerHandState(numClass, numColor);
        if (doCopy)
        {
            aiHandState.CloneState(playerState);
        }
        else {
            DispenseCards(aiHandState);
        }
        cardsChanged = Enumerable.Repeat(false, cardsChanged.Length).ToArray();
        CalculateMove(aiHandState);
        UpdateCardUI();
    }

    internal void DispenseCards(PokerHandState pokerState)
    {
        for (int i = 0; i < pokerState.myCards.Length; i++)
        {
            pokerState.myCards[i] = pokerMachine.GetRandomCard();
            cardsObjects[i].SetButtonVisibility(true);
        }
    }

    public void DoMove() {
        CalculateMove(aiHandState);
        UpdateCardUI();
        StartCoroutine(DoSingleMove());
    }
    IEnumerator DoSingleMove()
    {
        int indexToChange = GetBestMove(aiHandState);
        while (indexToChange >= 0)
        {
            if (debugOut)
            {
                Debug.Log(aiHandState.HandToString());
                string str = "";
                for (int i = 0; i < 5; i++)
                {
                    str += dps_list[i] + " |";
                }
                Debug.Log(str);
                Debug.Log("Change " + indexToChange);
            }
            ReRollCardAt(indexToChange);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            indexToChange = GetBestMove(aiHandState);
        }
        ExitPhase();

    }

    public void TestMove(PokerHandState state)
    {
        cardsChanged = Enumerable.Repeat(false, cardsChanged.Length).ToArray();
        int indexToChange = GetBestMove(state);
        while (indexToChange >= 0)
        {
            Test_RerollAt(state,indexToChange);
            indexToChange = GetBestMove(state);
        }

    }

    private void ExitPhase()
    {
        RectTransform rect = BoardUI.GetComponent<RectTransform>();
      
        Sequence s = DOTween.Sequence()
            .Append(rect.DOLocalMove(targetLocation, duration))
            .Join(rect.DOScale(targetScale, duration))
            .OnComplete(
                 () =>
                 {
                     pokerMachine.SetPokerBoardAvailable(true);
                     EventManager.TriggerEvent(MyEvents.EVENT_POKERHAND_FINALISED_AI, new EventObject(aiHandState.pokerHand));
                 }
             );
        
    }

    internal void CalculateMove(PokerHandState pokerState)
    {
       // Debug.Log("calc"+aiHandState.numberCounts.Length + " , " + aiHandState.colorCounts.Length);
        pokerState.CalculateHand();
        for (int i = 0; i < 5; i++)
        {
            double expectedChange = -9999f;
            if (!cardsChanged[i]) {
                expectedChange = SearchPossibilityAt(pokerState, i);
            }
        //    Debug.Log(expectedChange);
            dps_list[i] = expectedChange;
            cardsObjects[i].SetExpectedDPSChange(expectedChange);
        }

    }

    private int GetBestMove(PokerHandState pokerState)
    {
        CalculateMove(pokerState);
        int maxIndex = 0;
        bool noAvailableChange = true;
        for (int i = 0; i < dps_list.Length; i++)
        {
          
            if (!cardsChanged[i] && dps_list[i] >=0)
            {
                noAvailableChange = false;
                if (dps_list[i] > dps_list[maxIndex]) {
                    maxIndex = i;
                } else if (dps_list[i] == dps_list[maxIndex]) {
                    //Compare card class, choose Lower
                    if (pokerState.myCards[i].GetClass() <
                       pokerState.myCards[maxIndex].GetClass()) {
                        maxIndex = i;
                    }
                }
            }
        }
        return (noAvailableChange) ? -1 : maxIndex;
    }
    private void Test_RerollAt(PokerHandState state, int i)
    {
        cardsChanged[i] = true;
      
        GameCard prevCard = state.myCards[i];
        GameCard newCard = pokerMachine.GetRandomCard();
        while (newCard.IsSame(prevCard))
        {
            newCard = pokerMachine.GetRandomCard();
        }
        state.myCards[i] = newCard;
        //   Debug.Log(i + ".set " + cardsChanged[i]);

    }
    public void ReRollCardAt(int i)
    {
        cardsObjects[i].SetButtonVisibility(false);
        cardsChanged[i] = true;
        Image img = cardsObjects[i].GetComponent<GameCardObject>().cardImage;
        img.DOFade(0, 0.25f)
            .OnComplete(
            () =>
                {
                    GameCard prevCard = aiHandState.myCards[i];
                    GameCard newCard = pokerMachine.GetRandomCard();
                    while (newCard.IsSame(prevCard))
                    {
                        newCard = pokerMachine.GetRandomCard();
                    }
                    aiHandState.myCards[i] = newCard;
                    UpdateCardUI();
                    img.DOFade(1, 0.25f);
                }
            );

  
     //   Debug.Log(i + ".set " + cardsChanged[i]);
        
    }

    internal double SearchPossibilityAt(PokerHandState parentState, int _position) {
        double parentDPS = GetDPSOfHand(parentState.pokerHand);
        List<double> childrenDPS = new List<double>();
        GameCard parentCard = parentState.myCards[_position];
        //GenerateChildren
        for (int colorIndex = 0; colorIndex < numColor; colorIndex++) {
            for (int classIndex = 0; classIndex < numClass; classIndex++)
            {
                if (colorIndex == (int)parentCard.GetColor() && classIndex == (int)parentCard.GetClass()) continue;
                PokerHandState childState = new PokerHandState(numClass, numColor);
                childState.CloneState(parentState);
                childState.myCards[_position] = new GameCard(classIndex, colorIndex);
                           //Calculate Values
                childState.CalculateHand();
                double childHandDPS = GetDPSOfHand(childState.pokerHand);
             //   Debug.Log(childState.myCards[_position].ToString() + " = " + childState.pokerHand.GetName() + " ->DSP:  " + childHandDPS+ " change : "+(childHandDPS-parentDPS));
                childrenDPS.Add(childHandDPS);
            }
        }

        double expectedChangeInDPS = 0f;
        for (int i = 0; i < childrenDPS.Count; i++) {
            expectedChangeInDPS += ( childrenDPS[i] - parentDPS);
        }

        return expectedChangeInDPS;
    }

    private double GetDPSOfHand(PokerHand hand)
    {
        double cumDPS = 0f;

        List<UnitConfig> tradedTowers = tradeRule.TradeToTower(hand);
        foreach (UnitConfig uCon in tradedTowers) {
            cumDPS += GetDPSofTower(unitConfig: uCon);
        }
        return cumDPS;
    }
    public double GetDPSofTower(string uid = null, UnitConfig unitConfig = null) {
        if (unitConfig == null) {
            Debug.Assert(uid != null, " No parameter given");
            unitConfig = unitDictionary[uid];
        }
        return GetDPSofTower_Helper(unitConfig);
    }
    private double GetDPSofTower_Helper(UnitConfig uConfig)
    {
        TowerCharacter charID = uConfig.GetCharacterID();
        string uid = uConfig.GetUID();
        double thisDPS;
        if (charID == TowerCharacter.Azusa)
        {
            //아즈사는 가장 강한 적 하나를 죽임
            thisDPS = StatisticsManager.GetHighestDPS() * 2;
        }
        else if (charID == TowerCharacter.Yukiho)
        {
            //유키호는 한명을 장애로 만듬
            thisDPS = StatisticsManager.GetHighestDPS() * 0.75;
        }
        else
        {
            double recordedDPS = StatisticsManager.ReferenceDPS(uid);
            double heuristicDPS = uConfig.GetFinalRawDPS();
            thisDPS = Math.Max(recordedDPS, heuristicDPS);
        }
        //   Debug.Log(uCon.name + " dps : " + uCon.GetFinalRawDPS());
        return thisDPS;
    }

    private void UpdateCardUI()
    {
        aiHandState.CalculateHand();
        handNameText.text = aiHandState.pokerHand.GetName();
        for (int i = 0; i < aiHandState.myCards.Length; i++)
        {
            cardsObjects[i].SetCardImage(pokerMachine.GetCardSprite(aiHandState.myCards[i]));
        }
    }
    public void SetPokerBoardAvailable(bool enable)
    {
         BoardUI.SetActive(enable);
        if (enable) {
            RectTransform rect = BoardUI.GetComponent<RectTransform>();
            rect.localPosition = originaltLocation;
            rect.localScale = originalScale;
        }
    }
}
