using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ConstantStrings;
using System.Linq;
using DG.Tweening;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] Image[] mostDrawnSprites;
    [SerializeField] Image[] mostSoldSprites;
    [SerializeField] Image[] mostKilledSprites;
    [SerializeField] Text[] mostDrawnValues;
    [SerializeField] Text[] mostSoldValues;
    [SerializeField] Text[] mostKilledValues;
    [SerializeField] BoxCollider2D blocker;
    [SerializeField] RectTransform quitButton;
    [SerializeField] Text title;
    [SerializeField] Text stageReached;
    Dictionary<string, UnitConfig> unitDictionary = null;



    public void ShowGameOver(bool victory) {
        GameSession session = GameSession.GetGameSession();
        if (victory)
        {
            title.text = LocalizationManager.Convert("TXT_KEY_PERFECT_WIN");
        }
        else {
            title.text = LocalizationManager.Convert("TXT_KEY_LOSE"); 
        }
        stageReached.text = LocalizationManager.Convert("TXT_KEY_STAGE_REACHED") + session.waveManager.GetCurrentWaveNumber();

        unitDictionary =session.GetUnitDictionary();
        SortInfo(mostKilledSprites,mostKilledValues, "TOTAL_KILL_");
        SortInfo(mostDrawnSprites, mostDrawnValues, "TOTAL_SPAWN_");
        SortInfo(mostSoldSprites, mostSoldValues, "TOTAL_SELL_");
        SetVisibility(true);
        moveRight();




    }
    void moveRight()
    {
        Vector3 right = new Vector3(900, -400, 0);
        quitButton.DOLocalMove(right, 7f).OnComplete(
              () =>
              {
                  moveLeft();
              }
          );

    }

    private void moveLeft()
    {
        Vector3 left = new Vector3(200, -400, 0);
        quitButton.DOLocalMove(left, 7f).OnComplete(
              () =>
              {
                  moveRight();
              }
          );
    }

    private void SortInfo(Image[] images, Text[] texts, string tag)
    {
        Dictionary<string, int> statBoard = new Dictionary<string, int>();
        foreach (string uid in UID_List)
        {
            int kill = StatisticsManager.GetStat(tag + uid);
            statBoard.Add(uid, kill);
        }

        var items = from pair in statBoard
                    orderby pair.Value descending
                    select pair;
        var listed = items.ToList();
        for (int i = 0; i < mostKilledSprites.Length; i++)
        {
            if (i < listed.Count)
            {
                UnitConfig u557 = unitDictionary[listed[i].Key];
                images[i].sprite = u557.myPortraitSprite;
                texts[i].text = listed[i].Value.ToString();
            }
        }
    }

    void SetVisibility(bool enable) {
        gameObject.SetActive(enable);
        blocker.enabled = enable;
    }
    public void OnClick_Retry() {
        SetVisibility(false);

        GameSession.GetGameSession().ResetGame();
    }
    public void OnClick_Quit()
    {
        Application.Quit();
    }


}
