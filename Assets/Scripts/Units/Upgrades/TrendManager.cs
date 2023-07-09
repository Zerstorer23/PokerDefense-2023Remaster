using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static ConstantStrings;
using static LocalizationManager;
using Random = UnityEngine.Random;

public class TrendManager : MonoBehaviour
{
    //  GameSession session;
    [SerializeField] public PokerMachine pokerMachine;
    [SerializeField] public int TREND_THRESHOLD = 7;
    [SerializeField] public int INTRODUCE_TREND_AT = 7;
    const int TREND_VISUAL_INCREMENT_BASE = 4;
    const int TREND_VISUAL_INCREMENT_PER_TURN = 10; // 10턴당 1
    public int currentTrendVisualIncrement = 0;

    public string[] currentTrends = new string[2];
    [SerializeField]  private string[] nameList = { YUKIHO, HARUKA, MIKI, CHIHAYA, AMI, TAKANE, HIBIKI, IORI, YAYOI, MAKOTO }; //아즈사 비행
    [SerializeField] private bool[] trendChecklist;

    [SerializeField] Text kotoriLine;
    [SerializeField] Text trendNotifier;
    Dictionary<string, UnitConfig> idolDictionary;

    public int currentWave = 0;
    public int trendClock = 0;



    private void Awake()
    {
      //  Debug.Log("trend init start");
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, ClearText);
        trendChecklist = new bool[nameList.Length];
        ShuffleList();
     //  Debug.Log("trend init end");
    }

    private void OnDestroy()
    {

        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, ClearText);
    }
    void ShuffleList()
    {
        for (int i = 0; i < nameList.Length; i++)
        {
            int index1 = Random.Range(0, nameList.Length - 1);
            int index2 = Random.Range(0, nameList.Length - 1);
            string temp = nameList[index1];
            nameList[index1] = nameList[index2];
            nameList[index2] = temp;
        }
    }
    internal bool CheckTrend(int waveIndex)
    {
        if (idolDictionary == null)
        {
            idolDictionary = GameSession.GetGameSession().GetUnitDictionary();
        }

        currentWave = waveIndex + 1;
        if (currentWave < INTRODUCE_TREND_AT)
        {
            return false;
        }
        bool hasTrends = (--trendClock <= 0);
        currentTrendVisualIncrement = TREND_VISUAL_INCREMENT_BASE + currentWave/  TREND_VISUAL_INCREMENT_PER_TURN  ;

        if (hasTrends)
        {
            SetTrends(currentTrendVisualIncrement);
            SetKotoriText();
            trendClock = TREND_THRESHOLD;
        }
        else
        {
            SetHeaderText();
        }
        return hasTrends;
    }

    private void SetHeaderText()
    {
        trendNotifier.text =
            Convert("TXT_KEY_TRENDING_IDOL_HEADER",
            GetIdolName(currentTrends[0]),
            GetIdolName(currentTrends[1]),
            trendClock.ToString(),
            currentTrendVisualIncrement.ToString()
            );
    }

    private void SetKotoriText()
    {

        string text =
            Convert("TXT_KEY_TRENDING_IDOL_KOTORI_LINE",
            GetIdolName(currentTrends[0]),
            GetIdolName(currentTrends[1])
            );
        kotoriLine.DOText(text, 3f).OnComplete(
            () =>
            {
                SetHeaderText();
                kotoriLine.text = "";
                pokerMachine.StartPokerGame();
            }
        );

    }

    public void ClearText(EventObject eo)
    {

        kotoriLine.text = "";
        trendNotifier.text = "";
    }
    private String GetIdolName(string uid)
    {
        UnitConfig unit = idolDictionary[uid];
        Debug.Assert(unit != null, " no such unit");

        string text = "<color=" + unit.colorHex + ">" + Convert(unit.txt_name) + "</color>";
        return text;
    }

    private void SetTrends(int trendValue)
    {
        currentTrends[0] = GetRandomTrend();
        currentTrends[1] = GetCeilingTrend();
        UpgradeManager.SetTrends(currentTrends, trendValue);
    }

    private string GetCeilingTrend()
    {
        for (int i = 0; i < trendChecklist.Length; i++)
        {
            if (!trendChecklist[i])
            {
                trendChecklist[i] = true;
                return nameList[i];
            }
        }
        trendChecklist = new bool[nameList.Length];
        trendChecklist[0] = true;
        return nameList[0];
    }

    private string GetRandomTrend()
    {
        int index = Random.Range(0, nameList.Length - 1);
        trendChecklist[index] = true;
        return nameList[index];
    }
}
