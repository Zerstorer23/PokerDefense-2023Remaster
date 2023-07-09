using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ConstantStrings;
using static PokerHand;

public class TowerSpawner : MonoBehaviour
{




    //Cache
    [SerializeField] internal CardTradeRule tradeRule;
    [SerializeField] public GameObject towerBase;
    [SerializeField] TowerSpawningQueueHUD queueHUD;
    [SerializeField] HUD_UnitOptions unitOptions;
    [SerializeField] internal PlacerGroup cursor;
    internal Dictionary<string,Tower> myTowers = new Dictionary<string, Tower>();


    internal MapInitialiser mapInfo;
    internal  GameSession gameSession;


    int spawnIndex = 0;
    GameObject selectedObject = null;
    public List<GameObject> towerSpawnPool = new List<GameObject>();
    
    bool spawnByInstantiate = true;
    GameObject singleTowerObjectToSpawn = null;

    //---Statistics---//
    //ID
    int currWave=0;
    int currSpawned=0;
    
    private void Awake()
    {
      //  Debug.Log("spawner srtart");
        EventManager.StartListening(MyEvents.EVENT_CLICK_TOWER, OnTowerClicked);
        EventManager.StartListening(MyEvents.EVENT_POKERHAND_FINALISED, ReadPokerHand);
        EventManager.StartListening(MyEvents.EVENT_TOWER_TO_SPAWN_SELECTED, SetCursorIndexByUID);
      //  EventManager.StartListening(MyEvents.EVENT_TOWER_REMOVE_ALL_OF, RemoveAllTowerOf);
        EventManager.StartListening(MyEvents.EVENT_PLACE_AREA_CLICKED, OnPlaceAreaClicked);
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, AbortPlaceMode);
        EventManager.StartListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, OnWaveStarted);
        mapInfo = GetComponent<MapInitialiser>();
     //   Debug.Log("spawner end");

    }

    private void OnWaveStarted(EventObject arg0)
    {
        currWave = gameSession.waveManager.GetCurrentWaveNumber();
        currSpawned = 0;
    }
    internal string PollNextGameID() {
        string id = "T/" + currWave + "/" + currSpawned++;
        return id;
    }

    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_CLICK_TOWER, OnTowerClicked);
        EventManager.StopListening(MyEvents.EVENT_POKERHAND_FINALISED, ReadPokerHand);
        EventManager.StopListening(MyEvents.EVENT_TOWER_TO_SPAWN_SELECTED, SetCursorIndexByUID);
       // EventManager.StopListening(MyEvents.EVENT_TOWER_REMOVE_ALL_OF, RemoveAllTowerOf);
        EventManager.StopListening(MyEvents.EVENT_PLACE_AREA_CLICKED, OnPlaceAreaClicked);
        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, AbortPlaceMode);
        EventManager.StopListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, OnWaveStarted);

    }


    public Dictionary<string, Tower> GetMyTowers()
    {
        return myTowers;
    }

    public void ResetTowers() {
        //All towers

        foreach (KeyValuePair<string, Tower> pair in myTowers) {
                Destroy(pair.Value.gameObject);
        }
        myTowers = new Dictionary<string, Tower>();
        mapInfo.ResetMap();


    }

    internal void SpawnFromReserve(GameObject targetTower)
    {
        Debug.Assert(targetTower != null, "targetTower 매개 변수가 null입니다.");
        singleTowerObjectToSpawn = targetTower;
        spawnByInstantiate = false;
        unitOptions.OnClickReservePool(targetTower);
        TogglePlaceMode(true,MouseUser.RESERVE_SPAWNER);
    }
    internal void AbortReserveSpawn()
    {
        Debug.Assert(singleTowerObjectToSpawn != null, "targetTower 매개 변수가 null입니다.");
        singleTowerObjectToSpawn = null;
        spawnByInstantiate = true;
        unitOptions.OnHideOptions(null);
        TogglePlaceMode(false, MouseUser.RESERVE_SPAWNER);
    }

    public bool RemoveTowerFromMapByGameID(GameObject tower, bool doDestroy)
    {
        Tower t = tower.GetComponent<Tower>();
        string gid = t.GetGameID();
        mapInfo.towerOccupiedMap[(int)t.mapPosition.x, (int)t.mapPosition.y] = null;
        if (myTowers.ContainsKey(gid)) {
            myTowers.Remove(gid);
            if (doDestroy)
            {
                Destroy(tower);
            }
            return true;
        }
        Debug.LogWarning("Did not find " + gid+ " at "+gameObject.transform.position);
        return false;
    }

    private void OnTowerClicked(EventObject eo)
    {
        if (!(ClickManager.GetCurrentUser() == MouseUser.TOWER_SPAWNER
            || ClickManager.GetCurrentUser() == MouseUser.RESERVE_SPAWNER)
            ) return;
        TogglePlaceMode(false, ClickManager.GetCurrentUser());

    }

    private void OnPlaceAreaClicked(EventObject eo)
    {
        if (! (ClickManager.GetCurrentUser() == MouseUser.TOWER_SPAWNER
            || ClickManager.GetCurrentUser() == MouseUser.RESERVE_SPAWNER)
            ) return;

        if (cursor.CanPlace())
        {
            if (spawnByInstantiate)
            {
                SpawnByInstantiate();
            }
            else {
                SpawnByActivate();
            }
            return;
        }
        TogglePlaceMode(false, ClickManager.GetCurrentUser());
        return;
    }
    private void CheckWaveStart() {
        if (towerSpawnPool.Count == 0)
        {
            if (!ClickManager.IsNone())
            {
                TogglePlaceMode(false, MouseUser.TOWER_SPAWNER);
            }
            EventManager.TriggerEvent(MyEvents.EVENT_WAVE_START_REQUESTED, null);
        }
    }
    private void SpawnByInstantiate() {
        UnitConfig unitConfig = towerSpawnPool[spawnIndex].GetComponent<TowerSpawningButton>().GetConfig();
        bool success = SpawnDefenderAt(cursor.GetMousePosition(), unitConfig);
        if (success) { 
            RemoveTowerFromPool(spawnIndex);
        }
        CheckWaveStart();
    }
    private void SpawnByActivate()
    {
        singleTowerObjectToSpawn.transform.position = cursor.GetMousePosition();
        singleTowerObjectToSpawn.SetActive(true);
        Tower t = singleTowerObjectToSpawn.GetComponent<Tower>();
        t.skillManager.ReinitialiseActiveSkills();
        //t.focusedSprite.gameObject.SetActive(true);
        myTowers.Add(t.GetGameID(),t);
        mapInfo.AddTowerOnMap(t);
        EventManager.TriggerEvent(MyEvents.EVENT_TOWER_PLACED, new EventObject(singleTowerObjectToSpawn));
        AbortReserveSpawn();
        EventManager.TriggerEvent(MyEvents.EVENT_RESERVE_PLACE_RESULT, new EventObject(true));
    }
    private void ReadPokerHand(EventObject lexington)
    {
        spawnByInstantiate = true;
        PokerHand hand = lexington.GetPokerHand();
        List<UnitConfig> tradedTowers = tradeRule.TradeToTower(hand);
        AddCardsToQueue(tradedTowers);
        bool hasEmpty = mapInfo.HasEmptySpace();
        if (hasEmpty)
        {
            TogglePlaceMode(true, MouseUser.TOWER_SPAWNER);
        }
        else {
            OnNotEnoughSpace();
        }
    }
    private void OnNotEnoughSpace()
    {
        Debug.Log("Not enough space");
        TogglePlaceMode(false, ClickManager.GetCurrentUser());
        EventManager.TriggerEvent(MyEvents.EVENT_MESSAGE_TRIGGERED, new EventObject("공간이 부족합니다. 유닛을 정리해주세요"));
       // queueHUD.SetExtraOptionsVisibility(true);
    }

    internal GameObject GetMostValuable(Owner exceptThis, TowerCharacter mutexTower)
    {
        double maxDPS = 0;
        GameObject mvp = null;
        foreach (Tower tower in myTowers.Values)
        {
            if (tower.owner != exceptThis && !tower.buffManager.isTargetOfSkill)
            {
                if (mutexTower == tower.GetCharacterID())
                {
                    continue;
                }
                double thisDPS = StatisticsManager.ReferenceDPS(tower.GetUID());
                if (thisDPS <= 0)
                {
                    thisDPS = tower.firepowerManager.GetDPS();
                }
                if (thisDPS > maxDPS || mvp == null)
                {
                    maxDPS = thisDPS;
                    mvp = tower.gameObject;
                }
            }
        }
        return mvp;
    }

    private bool SpawnDefenderAt(Vector2 pos, UnitConfig spawnConfig)
    {
        GameObject Lexington = Instantiate(towerBase, pos, Quaternion.identity) as GameObject;
        Lexington.transform.parent = transform;
        UnitConfig Saratoga = Instantiate(spawnConfig) as UnitConfig;
        Tower t = Lexington.GetComponent<Tower>();
        string gid = PollNextGameID();

        t.SetConfig(gid, Saratoga, Owner.NAMCO);

        myTowers.Add(gid,t);
        mapInfo.AddTowerOnMap(t);
        //  Destroy(Saratoga);
        EventManager.TriggerEvent(MyEvents.EVENT_TOWER_PLACED, new EventObject(Lexington));
        return true;
    }

    public void HideTowerFocuses()
    {
        foreach (Tower myTower in myTowers.Values)
        {
            myTower.GetComponent<Tower_OnClick>().SetPlateFocus(false);
        }

    }
    public void TogglePlaceMode(bool nowPlace, MouseUser userType)
    {
        ClickManager.ToggleUser(userType, nowPlace);

        ActivateCursor(nowPlace);
        GetComponent<PolygonCollider2D>().enabled = nowPlace;
        //Turn on Cursor UI
        if (nowPlace) {
            if (singleTowerObjectToSpawn == null)
            {
                UnitConfig unitConfig = towerSpawnPool[spawnIndex].GetComponent<TowerSpawningButton>().GetConfig();
                float radius = unitConfig.attackDistance;
                cursor.SetRadius(radius);
            }
            else {
                float radius = singleTowerObjectToSpawn.GetComponent<Tower>().GetFinalAttackRange();
                cursor.SetRadius(radius);
            }
        }
        //Set Defender Info
    }
    private void OnMouseUp()
    {
        OnPlaceAreaClicked(null);
    }
    internal void ActivateCursor(bool enable)
    {
        foreach (Tower myTower in myTowers.Values)
        {
           // if (myTower == null) continue;
            myTower.GetComponentInChildren<Tower_OnClick>().SetPlateFocus(enable);
        }
        cursor.Activate(enable);
    }


    public void AbortPlaceMode(EventObject eo)
    {
        if (spawnByInstantiate) return;
        ScreenType type = eo.screenType;
        if (type != ScreenType.MAP)
        {
            singleTowerObjectToSpawn = null;
            spawnByInstantiate = true;
            TogglePlaceMode(false, MouseUser.RESERVE_SPAWNER);
            EventManager.TriggerEvent(MyEvents.EVENT_RESERVE_PLACE_RESULT, new EventObject(false));
        }
    }

    public void Cheat_SetCards(int v)
    {
        GetComponent<TowerRelocator>().AbortPlaceMode(null);

        ResetSpawnPool();

        PokerHandType pType = (PokerHandType)v;
        PokerHand ph = new PokerHand(pType);

        List<UnitConfig> tradedTowers = tradeRule.TradeToTower(ph);
        AddCardsToQueue(tradedTowers);
        TogglePlaceMode(true, MouseUser.TOWER_SPAWNER);
    }


    private void AddCardsToQueue(List<UnitConfig> sets)
    {
        ResetSpawnPool();
        foreach (UnitConfig ayanami in sets)
        {
            GameObject midway = queueHUD.AddButton(ayanami);
            towerSpawnPool.Add(midway);

           StatisticsManager.AddToStat("TOTAL_SPAWN_"+ayanami.uid, 1);
           StatisticsManager.AddToStat(STAT_TOTAL_TOWER_SPAWNED, 1);
           TutorialManager.CheckTutorial("LearnToPlace");
           TutorialManager.CheckTutorial(ayanami.txt_tutorial_key);
            CheckSpawningAchievement(ayanami);
        }
        Debug.Log("Open queue and select");
        SelectThisTowerButton(towerSpawnPool[0]);
        EventManager.TriggerEvent(MyEvents.EVENT_OPEN_TOWER_SPAWN_SELECTOR_HUD, null);
    }
 
    private void ResetSpawnPool()
    {
        foreach (GameObject lex in towerSpawnPool)
        {
            Destroy(lex);
        }
        towerSpawnPool = new List<GameObject>();
    }


    private void RemoveTowerFromPool(int index)
    {
        Destroy(selectedObject);
        towerSpawnPool.RemoveAt(index);
        spawnIndex = 0;
        if (towerSpawnPool.Count > 0)
        {
            SelectThisTowerButton(towerSpawnPool[spawnIndex]);
        }
        else
        {
            queueHUD.Hide();
            EventManager.TriggerEvent(MyEvents.EVENT_SHOW_TOWER_OPTIONS, new EventObject(false));
        }
    }

    private void SetCursorIndexByUID(EventObject cleveland)
    {
        GameObject obj = cleveland.GetGameObject();
        SetSelectedObject(obj);
        string uid = obj.GetComponent<TowerSpawningButton>().GetUID();

        for (int i = 0; i < towerSpawnPool.Count; i++)
        {
            if (towerSpawnPool[i].GetComponent<TowerSpawningButton>().GetUID().Equals(uid))
            {
                spawnIndex = i;
                break;
            }
        }
        if (mapInfo.HasEmptySpace() && ClickManager.IsNone()) {
            TogglePlaceMode(true, MouseUser.TOWER_SPAWNER);
        }
    }
    void SelectThisTowerButton(GameObject towerButton)
    {
        SetSelectedObject(towerButton);
        StartCoroutine(WaitAndSelect());
    }
    void SetSelectedObject(GameObject obj) {
        selectedObject = obj;
        EventManager.TriggerEvent(MyEvents.EVENT_SHOW_TOWER_OPTIONS, new EventObject(obj) { boolObj = true }); ;
    }

    public IEnumerator WaitAndSelect()
    {

        yield return new WaitForFixedUpdate();
        selectedObject.GetComponent<Toggle>().Select();

    }

    public void SellSelectedTowerInPool() {
        UnitConfig unitConfig = towerSpawnPool[spawnIndex].GetComponent<TowerSpawningButton>().GetConfig();

        gameSession.mineralManager.AddResource(UpgradeType.DANCE, unitConfig.sellValue_Dance);
        gameSession.mineralManager.AddResource(UpgradeType.VOCAL, unitConfig.sellValue_Vocal);
        gameSession.mineralManager.AddResource(UpgradeType.VISUAL, unitConfig.sellValue_Visual);
        CheckSellingEvent(unitConfig.characterID);

        RemoveTowerFromPool(spawnIndex);
        CheckWaveStart();
    }
    public void ReserveSelectedTowerInPool() {


        UnitConfig unitConfig = towerSpawnPool[spawnIndex].GetComponent<TowerSpawningButton>().GetConfig();

        GameObject Lexington = Instantiate(towerBase, Vector3.zero, Quaternion.identity) as GameObject;
        Lexington.transform.parent = transform;
        UnitConfig Saratoga = Instantiate(unitConfig) as UnitConfig;
        Tower t = Lexington.GetComponent<Tower>();
        t.SetConfig(PollNextGameID(), Saratoga,Owner.NAMCO);
        Lexington.SetActive(false);

        ReserveManager.AddTowerReserve(Lexington);
        RemoveTowerFromPool(spawnIndex);
        CheckWaveStart();
    }


    public void CheckSpawningAchievement(UnitConfig unit) {
        string achievementID;
        string eventID;

        switch (unit.characterID)
        {
            case TowerCharacter.Yukiho:
                achievementID = IdolDefense.achievement_yukiho_mster;
                eventID = IdolDefense.event_yukiho_summoned;
                break;
            case TowerCharacter.Haruka:
                achievementID = IdolDefense.achievement_hruk_mster;
                eventID = IdolDefense.event_haruka_summoned;
                break;
            case TowerCharacter.Ami:
                achievementID = IdolDefense.achievement_ami_mster;
                eventID = IdolDefense.event_futami_summoned;
                break;
            case TowerCharacter.Mami:
                achievementID = IdolDefense.achievement_mami_mster;
                eventID = IdolDefense.event_mami_summoned;
                break;
            case TowerCharacter.Chihaya:
                achievementID = IdolDefense.achievement_chihaya_mster;
                eventID = IdolDefense.event_chihaya_summoned;
                break;
            case TowerCharacter.Yayoi:
                achievementID = IdolDefense.achievement_yayoi_mster;
                eventID = IdolDefense.event_yayoi_summoned;
                break;
            case TowerCharacter.Takane:
                achievementID = IdolDefense.achievement_takane_mster;
                eventID = IdolDefense.event_takane_summoned;
                break;
            case TowerCharacter.Hibiki:
                achievementID = IdolDefense.achievement_hibiki_mster;
                eventID = IdolDefense.event_hibiki_summoned;
                break;
            case TowerCharacter.Makoto:
                achievementID = IdolDefense.achievement_makoto_mster;
                eventID = IdolDefense.event_makoto_summoned;
                break;
            case TowerCharacter.Miki:
                achievementID = IdolDefense.achievement_miki_mster;
                eventID = IdolDefense.event_miki_summoned;
                break;
            case TowerCharacter.Iori:
                achievementID = IdolDefense.achievement_iori_mster;
                eventID = IdolDefense.event_iori_summoned;
                break;
            case TowerCharacter.Azusa:
                achievementID = IdolDefense.achievement_azusa_mster;
                eventID = IdolDefense.event_azusa_sold;
                break;
            default:
                return;
        }
        GooglePlayManager.IncrementAchievement(achievementID);
        GooglePlayManager.IncrementEvent(eventID);
    }

    public static void CheckSellingEvent(TowerCharacter tc) {
        string eventID;

        switch (tc)
        {
            default:
                return;
            case TowerCharacter.Yukiho:
                eventID = IdolDefense.event_yukiho_sold;
                break;
            case TowerCharacter.Haruka:
                eventID = IdolDefense.event_haruka_sold;
                break;
            case TowerCharacter.Ami:
                eventID = IdolDefense.event_ami_sold;
                break;
            case TowerCharacter.Mami:
                eventID = IdolDefense.event_mami_sold;
                break;
            case TowerCharacter.Chihaya:
                eventID = IdolDefense.event_chihaya_sold;
                break;
            case TowerCharacter.Yayoi:
                eventID = IdolDefense.event_yayoi_sold;
                break;
            case TowerCharacter.Takane:
                eventID = IdolDefense.event_takane_sold;
                break;
            case TowerCharacter.Hibiki:
                eventID = IdolDefense.event_hibiki_sold;
                break;
            case TowerCharacter.Makoto:
                eventID = IdolDefense.event_makoto_sold;
                break;
            case TowerCharacter.Miki:
                eventID = IdolDefense.event_miki_sold;
                break;
            case TowerCharacter.Iori:
                eventID = IdolDefense.event_iori_sold;
                break;
            case TowerCharacter.Azusa:
                eventID = IdolDefense.event_azusa_sold;
                break;
        }
        GooglePlayManager.IncrementEvent(eventID, 1);
    }
}
