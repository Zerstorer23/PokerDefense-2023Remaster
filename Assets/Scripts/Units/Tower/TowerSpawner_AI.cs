using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PokerHand;

public class TowerSpawner_AI : MonoBehaviour
{
    //Cache
    TowerSpawner towerSpawner;
    MapInitialiser mapInfo;
    //Placemode
    // List<UnitConfig> towerSpawnPool = new List<UnitConfig>();
    List<UnitConfig> towerSpawnPool = new List<UnitConfig>();
    public PokerMachineAI pokerAI;
    Owner Username = Owner.KUROI;


    internal Dictionary<string, int> scoreboard = new Dictionary<string, int>();

    private void Awake()

    {
      //  Debug.Log("spawnerai srtart");
        towerSpawner = GetComponent<TowerSpawner>();
        mapInfo = GetComponent<MapInitialiser>();
        EventManager.StartListening(MyEvents.EVENT_POKERHAND_FINALISED_AI, ReadAIHand);
      //  Debug.Log("spawnerai end");
    }
    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_POKERHAND_FINALISED_AI, ReadAIHand);
    }

    private void ReadAIHand(EventObject lexington)
    {
        PokerHand hand = lexington.GetPokerHand();
        towerSpawnPool = towerSpawner.tradeRule.TradeToTower(hand);
        StartSpawning();
        
    }

    private void StartSpawning()
    {
        foreach (UnitConfig uConfig in towerSpawnPool) {

            Vector3 mapPos = mapInfo.GetValidPosition(Owner.KUROI);
            if (mapPos == Vector3.back) {
                mapPos = RemoveLowestTower(uConfig);
                if (mapPos == Vector3.back)
                {//새 유닛이 최약체거나 자리가 아예없음
                    continue;
                }
            }
            Vector3 worldPos = mapPos.x * mapInfo.map_stepX+ mapPos.y * mapInfo.map_stepY + mapInfo.map_home;
          //  Debug.Log("Spawned at " + worldPos + " from " + towerSpawner.map_home);
            SpawnDefenderAt(worldPos, mapPos, uConfig);
        }
    }
    public void Cheat_SetCards_Kuroi(int v)
    {
        GetComponent<TowerRelocator>().AbortPlaceMode(null);
        PokerHandType pType = (PokerHandType)v;
        PokerHand ph = new PokerHand(pType);
        towerSpawnPool = towerSpawner.tradeRule.TradeToTower(ph);
        StartSpawning();
    }

    private Vector3 RemoveLowestTower(UnitConfig newUnitConfig)
    {
        Tower removeTower = null;
        double lowestDPS = 0f;
        double newUnitDPS = pokerAI.GetDPSofTower(unitConfig:newUnitConfig);
       //   Debug.Log("Finding replace for " + newUnitConfig.GetCharacterID()+" with "+newUnitDPS );
        var towers = towerSpawner.GetMyTowers().Values;

        foreach (Tower tower in towers) {
            if (tower.owner == Username)
            {
                double myDPS = pokerAI.GetDPSofTower(uid:tower.GetUID());
                if (removeTower == null || myDPS < lowestDPS)
                {
                    removeTower = tower;
                    lowestDPS = myDPS;
                 //  Debug.Log("     Found low dps " + myDPS + " at " + tower.transform.position + " / " + tower.GetCharacterID());
                }            
            }
        }
        if (removeTower == null || newUnitDPS < lowestDPS)
        {
       //     Debug.Log("     New unit has lower dps " + newUnitDPS + " => "+ removeTower.GetCharacterID()+ " / " + lowestDPS);
            return Vector3.back;

        }
        else
        {
         //   Debug.Log("     Replace " + removeTower.GetCharacterID() +" by "+newUnitConfig.GetCharacterID()+ " / " + removeTower.mapPosition);
            Vector3 removedPos = removeTower.mapPosition;
            towerSpawner.RemoveTowerFromMapByGameID(removeTower.gameObject,true);
            return removedPos;
        }

    }

 
    private bool SpawnDefenderAt(Vector3 worldPos, Vector3 boardPos, UnitConfig spawnConfig)
    {
        GameObject Lexington = Instantiate(towerSpawner.towerBase, Vector3.zero, Quaternion.identity) as GameObject;
        Lexington.transform.parent = transform;
        Lexington.transform.localPosition = worldPos;
        UnitConfig Saratoga = Instantiate(spawnConfig) as UnitConfig;
        Tower t = Lexington.GetComponent<Tower>();

        string gid = towerSpawner.PollNextGameID();
        t.SetConfig(gid, Saratoga, Username);
        Lexington.GetComponentInChildren<Tower_OnClick>().SetPlateFocus(false);
        Lexington.GetComponentInChildren<BuffIndicator>().MarkAsKuroi();
        towerSpawner.myTowers.Add(gid,t);
        mapInfo.towerOccupiedMap[(int)boardPos.x, (int)boardPos.y] = t;
        t.mapPosition = boardPos;

        EventManager.TriggerEvent(MyEvents.EVENT_TOWER_PLACED, new EventObject(Lexington));
        return true;
    }

}
