
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ConstantStrings;
using static LocalizationManager;
using System;

public class IdolRankingHelper : MonoBehaviour
{
    public Text[] idolNames;
    public Text[] idolKills;
    public CharacterBodyManager idolCharacter;
    public GameObject mainPanel;

    Dictionary<string, int> currentBoard;
    CanvasGroup canvasGroup;
    Dictionary<string, UnitConfig>unitDictionary = null;
    BoxCollider2D clickBlocker;
    public Image[] colorChangers;

    private void Awake()
    {
     //   Debug.Log("idolrank init start");
        EventManager.StartListening(MyEvents.EVENT_SHOW_IDOL_RANK, OnPanelVisibility);
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, OnPanelOpen);
        canvasGroup = GetComponent<CanvasGroup>();
        clickBlocker = GetComponent<BoxCollider2D>();
        SetVisibility(false);
   //     Debug.Log("idolrank init end");
    }

    private void OnPanelOpen(EventObject arg0)
    {
        ScreenType screenType = arg0.screenType;
        if (screenType != ScreenType.MAP) {
            SetVisibility(false);
        }
    }

    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_SHOW_IDOL_RANK, OnPanelVisibility);
        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, OnPanelOpen);

    }

    private void OnPanelVisibility(EventObject eo)
    {
        SetVisibility(eo.boolObj);
    }
    void SetVisibility(bool enable) {
        canvasGroup.blocksRaycasts = enable;
        canvasGroup.interactable = enable;
        canvasGroup.alpha = (enable) ? 1f : 0f;

        mainPanel.SetActive(enable);
        clickBlocker.enabled = enable;


        if (enable)
        {
            UpdateLeaderboard();
            EventManager.TriggerEvent(MyEvents.EVENT_SHOW_PANEL, new EventObject(ScreenType.MAP));
        }

    }

    void BuildCurrentBoard() {
        currentBoard = new Dictionary<string, int>();
        if (unitDictionary == null)
        {
            unitDictionary = GameSession.GetGameSession().GetUnitDictionary();
        }

        string achievementID;
        TowerCharacter tc;
        foreach (string uid in UID_List) {
            int kill = StatisticsManager.GetStat("KILL_" + uid);
            currentBoard.Add(uid, kill);


            tc = unitDictionary[uid].characterID;
            achievementID = UidToLeaderboard(tc);
            GooglePlayManager.AddToLeaderboard(achievementID, kill);
        }

 
    }
    void UpdateLeaderboard()
    {
        BuildCurrentBoard();

        var items = from pair in currentBoard
                    orderby pair.Value descending
                    select pair;
        var listed = items.ToList();
        for (int i = 0; i < idolNames.Length; i++)
        {
            if (i < listed.Count)
            {
                if (listed[i].Value == 0) continue;
                UnitConfig u557 = unitDictionary[listed[i].Key];
                idolNames[i].text = (i + 1) + ". " + Convert(u557.txt_name);
                idolKills[i].text = listed[i].Value.ToString() ;
            }
        }
        UnitConfig ToP = unitDictionary[listed[0].Key];
        Debug.Assert(ToP != null, "Unit config is NULL!!");
        SetIdolInfo(ToP);
        ChangeColors(GetColorByHex(ToP.colorHex));
    }
    void ChangeColors(Color color) {
        foreach (Image img in colorChangers) {
            img.color = color;
        }
    }




    void SetIdolInfo(UnitConfig unitConfig)
    {
        idolCharacter.SetHairSkins(unitConfig.GetUID());
        idolCharacter.SetAnimationTrigger("DoIdolRanked");
    }


    public void OnClickExit()
    {
        SetVisibility(false);
        ///----RESET_KILL_INFO----//
        foreach (string s in UID_List)
        {
            StatisticsManager.SetStat("KILL_" + s, 0);
        }
    }
    public string UidToLeaderboard(TowerCharacter tc) {
        switch (tc)
        {
            case TowerCharacter.Yukiho:
                return IdolDefense.leaderboard_yukiho_kills;
            case TowerCharacter.Haruka:
                return IdolDefense.leaderboard_haruka_kills;
            case TowerCharacter.Ami:
                return IdolDefense.leaderboard_ami_kills;
            case TowerCharacter.Mami:
                return IdolDefense.leaderboard_mami_kills;

            case TowerCharacter.Chihaya:
                return IdolDefense.leaderboard_chihaya_kills;
            case TowerCharacter.Yayoi:
                return IdolDefense.leaderboard_yayoi_kills;
            case TowerCharacter.Takane:
                return IdolDefense.leaderboard_takane_kills;
            case TowerCharacter.Hibiki:
                return IdolDefense.leaderboard_hibiki_kills;

            case TowerCharacter.Makoto:
                return IdolDefense.leaderboard_makoto_kills;

            case TowerCharacter.Miki:
                return IdolDefense.leaderboard_miki_kills;
            case TowerCharacter.Iori:
                return IdolDefense.leaderboard_iori_kills;

            case TowerCharacter.Azusa:
                return IdolDefense.leaderboard_azusa_kills;
            default:
                Debug.LogError("No such id!!! " + tc);
                return null;
        }
    }

}
