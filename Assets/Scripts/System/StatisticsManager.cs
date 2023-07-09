using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPSobject {
    float time;
    public double damage;
    public DPSobject(double _d) {
        time = Time.time;
        damage = _d;
    }

    internal bool IsValidTime(float _time)
    {
        float diff = Mathf.Abs(_time - time);
        return diff <= 60f;
    }
}

public class StatisticsManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static StatisticsManager prStatManager;
    public List<double> dpsDisplay = new List<double>();
    public  string[] nameDisplay = new string[] {"아미","아즈사","치하야","하루카","히비키","이오리","마코토","마미","미키","타카네","야요이","유키호" };
    private Dictionary<string, int> statLibrary;
    private Dictionary<string, int> towerNumbers;
    private Dictionary<string, List<DPSobject>> damageLibrary;
    private Dictionary<string, double> dpsList;
    UnitConfig[] unitList;
    private double maxDPS = 0f;
    /*
    /*
     //스탯매니져에 DPS매니져 추가
DPS오브젝트
  -time
  -damage

Dictionary<uid,List<dpsobj>> =>total data

build() -> 현시점 60초전 데미지 삭제 후 평균 계산
데미지 없거나 자료수 60 / 공속개 이하면 null

Dictionary<uid,float> => public data
     */

 
    public static StatisticsManager instance
    {
        get
        {
            if (!prStatManager)
            {
                prStatManager = FindObjectOfType<StatisticsManager>();
                if (!prStatManager)
                {
                    Debug.LogWarning("There needs to be one active EventManger script on a GameObject in your scene.");
                }

            }

            return prStatManager;
        }
    }
    public static void Init()
    {
        instance.statLibrary = new Dictionary<string, int>();
        instance.damageLibrary = new  Dictionary<string, List<DPSobject>>();
        instance.dpsList = new  Dictionary<string, double>();
        instance.towerNumbers = new  Dictionary<string, int>();

        instance.unitList = GameSession.GetGameSession().UnitConfigs;
        foreach (UnitConfig u in instance.unitList)
        {
            instance.damageLibrary.Add(u.GetUID(), new List<DPSobject>());
            instance.dpsList.Add(u.GetUID(), 0);
            instance.towerNumbers.Add(u.GetUID(), 0);
        }

    }
    public static int AddToStat(string tag, int amount) {


        if (!instance.statLibrary.ContainsKey(tag))
        {
            instance.statLibrary.Add(tag, amount);
        }
        else {
            instance.statLibrary[tag] += amount;
        }
        return instance.statLibrary[tag];
    }

    public static int GetStat(string tag) {
        if (instance.statLibrary.ContainsKey(tag))
        {
           return instance.statLibrary[tag];
        }
        else return 0;
    }


    //======DPS LIB=\===//
    void CountNumberOfTowers(TowerSpawner towerSpawner) {
        foreach (UnitConfig u in unitList)
        {
            towerNumbers[u.uid] = 0;
        }

        foreach (Tower t in towerSpawner.GetMyTowers().Values) {
           if(t.gameObject.activeSelf)towerNumbers[t.GetUID()]++;
        }
    }

    internal static void SetStat(string s, int v)
    {
        if (instance.statLibrary.ContainsKey(s))
        {
            instance.statLibrary[s] = v;
        }
    }

    public static void AddDamage(string uid, double damage) {
        DPSobject dps = new DPSobject(damage);
        instance.damageLibrary[uid].Add(dps);
    }
    public static void BuildData(TowerSpawner towerSpawner) {
        instance.CountNumberOfTowers(towerSpawner);
        float timeElapsed = GameSession.GetGameSession().waveManager.GetLastWaveElapsedTime();

        instance.dpsDisplay = new List<double>();
        foreach (KeyValuePair<string, List<DPSobject>> entry in instance.damageLibrary) {
            double totalDamage = 0;
            int index = 0;
            while (index < entry.Value.Count) {
                if (entry.Value[index].IsValidTime(Time.time))
                {
                    totalDamage += entry.Value[index].damage;
                    index++;
                }
                else
                {
                    entry.Value.RemoveAt(index);
                }
            }
            int numTowers = instance.towerNumbers[entry.Key];
            // Debug.Log(entry.Key+" :: damage " + totalDamage + " over " + timeElapsed + " by " + numTowers);
            float bot = timeElapsed * numTowers;
            
            instance.dpsList[entry.Key] =(bot > 0)? totalDamage / bot : 0;
            instance.dpsDisplay.Add(instance.dpsList[entry.Key]);
        }
        instance.SetHighestDPS();
    }
    public static double ReferenceDPS(string uid) { 
    
    return (instance.dpsList.ContainsKey(uid))
            ?    instance.dpsList[uid]
            :    -1;
    }

    void SetHighestDPS() {
        maxDPS = 0f;
        foreach (float dps in instance.dpsList.Values)
        {
            if (dps > maxDPS)
            {
                maxDPS = dps;
            }
        }
    }
    public static double GetHighestDPS() {
        return instance.maxDPS;
    }

}
