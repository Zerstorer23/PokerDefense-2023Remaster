using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstantStrings;




public class UpgradeManager : MonoBehaviour
{
    GameSession session;
    [SerializeField] bool[] cheat = { false, false, false };

    private Dictionary<string, int> upgradeDictionary_vocal;
    private Dictionary<string, int> upgradeDictionary_visual;
    private Dictionary<string, int> upgradeDictionary_dance;
    private Dictionary<string, int> trendDictionary;
    private Dictionary<string, int> danceDictionary;
    private Dictionary<string, int> visualDictionary;
    private Dictionary<string, int>[] upgrade_list = new Dictionary<string, int>[3];
    const int MAX_UPGRADE_VOCAL = 200;
    const int MAX_UPGRADE_VISUAL = 5;
    const int MAX_UPGRADE_DANCE = 4;

    const float VOCAL_INCREMENT = 1.75f;
    const float VISUAL_INCREMENT = 1f;
    const float DANCE_INCREMENT = 1f;

    private int[] Max_Upgrades= { MAX_UPGRADE_VOCAL, MAX_UPGRADE_VISUAL, MAX_UPGRADE_DANCE };

                   // { YUKIHO, HARUKA, MIKI, CHIHAYA, AMI, MAMI, TAKANE, AZUSA, HIBIKI, IORI, YAYOI, MAKOTO };
    int[] COST_DANCE ={  1,      3,      3,      3,      2,  2,      3,     3,      3,      3,   2  ,   3   };
    int[] COST_VISUAL={  1 ,     1,      3,      2,      2,  2,      3,      4,      3,      3,   1,     3   };


    private static UpgradeManager upgradeManager = null;

    public static UpgradeManager instance
    {
        get
        {
            if (!upgradeManager)
            {
                upgradeManager = FindObjectOfType<UpgradeManager>();
                if (upgradeManager)
                { 
                    upgradeManager.Init();
                }
            }

            return upgradeManager;
        }
    }

    void Init()
    {
        instance.initDictionary();
        instance.session = GetComponent<GameSession>();

    
        foreach (string s in UID_List)
        {
            if(cheat[0])
            instance.upgradeDictionary_vocal[s] = MAX_UPGRADE_VOCAL;
            if (cheat[1])
                instance.upgradeDictionary_visual[s] = MAX_UPGRADE_VISUAL;
            if (cheat[2])
                instance.upgradeDictionary_dance[s] = MAX_UPGRADE_DANCE;
        }
       

    }

    public static void ResetUpgrades() {
        instance.Init();
    }
    //12명

 
    private void initDictionary() {
        upgradeDictionary_vocal = new Dictionary<string, int>();
        foreach (string s in UID_List) {
            upgradeDictionary_vocal.Add(s, 0);
        }
        upgradeDictionary_visual = new Dictionary<string, int>();
        foreach (string s in UID_List)
        {
            upgradeDictionary_visual.Add(s, 0);
        }
        upgradeDictionary_dance = new Dictionary<string, int>();
        trendDictionary = new Dictionary<string, int>();
        danceDictionary = new Dictionary<string, int>();
        visualDictionary = new Dictionary<string, int>();

  
        for (int i = 0; i < UID_List.Length; i++)
        {
            string s = UID_List[i];
            upgradeDictionary_dance.Add(s, 0);
            trendDictionary.Add(s, 0);
            danceDictionary.Add(s, COST_DANCE[i]);
            visualDictionary.Add(s, COST_VISUAL[i]);
        }
        upgrade_list[0] = upgradeDictionary_vocal;
        upgrade_list[1] = upgradeDictionary_visual;
        upgrade_list[2] = upgradeDictionary_dance;
    }

    internal static int GetUpgradeValue(UpgradeType uType, string uid)
    {
        switch (uType)
        {
            case UpgradeType.VISUAL:
                return instance.upgrade_list[(int)uType][uid] + instance.trendDictionary[uid];
            default:
                return instance.upgrade_list[(int)uType][uid];
        }
    }

    internal static int GetMaxUpgrade(UpgradeType uType, string uid) {

        if (uType == UpgradeType.VISUAL)
        {
            return instance.Max_Upgrades[(int)uType] + instance.trendDictionary[uid];
        }
        return instance.Max_Upgrades[(int)uType]; 
    }

    internal static int GetUpgradeCost(UpgradeType uType,string uid)
    {
        int level = instance.upgrade_list[(int)uType][uid];
        switch (uType)
        {
            case UpgradeType.VOCAL:
                return (int)(level * VOCAL_INCREMENT);
            case UpgradeType.VISUAL:
                return instance.visualDictionary[uid] +(int)( level*VISUAL_INCREMENT);
            case UpgradeType.DANCE:
                return instance.danceDictionary[uid] + (int)(level * DANCE_INCREMENT);
        }
        return -1;
    }

    internal static void SetTrends(string[] currentTrends, int trendValue)
    {
        foreach (string s in UID_List)
        {
            instance.trendDictionary[s] = 0;
        }
        for (int i = 0; i < currentTrends.Length; i++) {
            string uid = currentTrends[i];
            if (uid.Equals(AMI))
            {
                instance.trendDictionary[MAMI]= trendValue;
            }
            instance.trendDictionary[uid] = trendValue;
        }
    }


    internal static bool IsTrending(string uid) {
        return instance.trendDictionary[uid] != 0;
    }

    internal static bool DoUpgrade(UpgradeType uType,string uid)
    {
        int currUp = GetUpgradeValue(uType, uid);
        int max = GetMaxUpgrade(uType, uid);
        if (currUp >= max) return false;

        int cost = GetUpgradeCost(uType, uid);
        bool success = instance.session.mineralManager.SpendResource(uType,cost);

        if (!success) return false;

        if (uid.Equals(AMI))
        {
            instance.upgrade_list[(int)uType][MAMI]++;
        }
        instance.upgrade_list[(int)uType][uid]++;
        
        if (uType == UpgradeType.DANCE) {
            EventManager.TriggerEvent(MyEvents.EVENT_SKILL_UPGRADED, new EventObject(uid));
            if (instance.upgrade_list[(int)uType][uid] == max) {
                CheckMaxUpgradeAchievement(uid);
            }

        }
        return true;
    }


    public static void CheckMaxUpgradeAchievement(string uid)
    {
        string achieveID;
        switch (uid) {
            case YUKIHO:
                achieveID = IdolDefense.achievement_yukiho_unique;
                break;
            case HARUKA:
                achieveID = IdolDefense.achievement_haruka_unique;
                break;
            case AMI:
            case MAMI:
                achieveID = IdolDefense.achievement_futami_unique;
                break;
            case CHIHAYA:
                achieveID = IdolDefense.achievement_chihaya_unique;
                break;
            case YAYOI:
                achieveID = IdolDefense.achievement_yayoi_unique;
                break;
            case MAKOTO:
                achieveID = IdolDefense.achievement_makoto_unique;
                break;
            case TAKANE:
                achieveID = IdolDefense.achievement_takane_unique;
                break;
            case HIBIKI:
                achieveID = IdolDefense.achievement_hibiki_unique;
                break;
            case MIKI:
                achieveID = IdolDefense.achievement_miki_unique;
                break;
            case IORI:
                achieveID = IdolDefense.achievement_iori_beam;
                break;
            case AZUSA:
                achieveID = IdolDefense.achievement_azusa_storm;
                break;
            default:
                return;
        }
        GooglePlayManager.AddAchievement(achieveID);
    }
}


public enum UpgradeType { 
    VOCAL=0,VISUAL=1,DANCE=2
}
