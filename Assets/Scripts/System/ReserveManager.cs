using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class ReserveManager : MonoBehaviour
{
    [SerializeField] TowerSpawner towerSpawner;
    [SerializeField] InventoryIconBehaviour[] reserveIcons;
    [SerializeField] Text amountText;
    [SerializeField] GameObject title;


    [SerializeField] int MAX_INVENTORY_SIZE = 8;
    [SerializeField] List<GameObject> towerInventory = new List<GameObject>();
    int selectedIndex = -1;

    Dictionary<string, int> inventoryDictionary = new Dictionary<string, int>();
    Dictionary<string, Sprite> uidToTower = new Dictionary<string, Sprite>();


    private static ReserveManager reserveManager;
    private void Awake()
    {
    //    Debug.Log("Reserve init srtart");
        EventManager.StartListening(MyEvents.EVENT_RESERVE_PLACE_RESULT, OnReserveResult);
        EventManager.StartListening(MyEvents.EVENT_MOUSE_USER_CHANGED, OnMouseUserChanged);
    //    Debug.Log("Reserve init end");
    }

    private void OnMouseUserChanged(EventObject arg0)
    {
        MouseUser newUser = (MouseUser)arg0.GetInt();
        if (newUser == MouseUser.TOWER_SPAWNER && towerInventory.Count > 0)
        {
           // Show();
            Hide();
        }
        else if (newUser == MouseUser.NONE && towerInventory.Count > 0)
        {
            Show();
        }
    }

    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_RESERVE_PLACE_RESULT, OnReserveResult);
        EventManager.StopListening(MyEvents.EVENT_MOUSE_USER_CHANGED, OnMouseUserChanged);
    }

    private void OnReserveResult(EventObject eo)
    {
        bool success = eo.boolObj;
        reserveIcons[selectedIndex].UnfocusIcon();
        if (success)
        {
            RemoveTowerFromReserve(false);
        }
        else {
            selectedIndex = -1;
        }
        
    }

    public static ReserveManager instance
    {
        get
        {
            if (!reserveManager)
            {
                reserveManager = FindObjectOfType<ReserveManager>();
                if (!reserveManager)
                {
                    Debug.LogWarning("There needs to be one active EventManger script on a GameObject in your scene.");
                }

            }

            return reserveManager;
        }
    }

    public static bool HasEmptySpot() {
        return instance.towerInventory.Count < instance.MAX_INVENTORY_SIZE;    
    }
    public static void AddTowerReserve(GameObject tower) {
        tower.SetActive(false);
        instance.towerSpawner.RemoveTowerFromMapByGameID(tower, false);
        instance.towerInventory.Add(tower); 
        Tower towerComp = tower.GetComponent<Tower>();
        towerComp.gameObject.transform.position = Vector3.zero;
        instance.AddToDictionary(towerComp);
        instance.SortInventoryIcons();
    }

    public static bool SelectReservedTower(string requestedUID)
    {
        int newIndex = instance.FindIndexByUID(requestedUID);
        instance.RemoveAllFocus();
        if (instance.towerSpawner.mapInfo.HasEmptySpace())
            {
            if (instance.CheckSameTowerSelected(newIndex)) {
                return false;
            }  
            GameObject targetTower = instance.towerInventory[instance.selectedIndex];
            instance.towerSpawner.SpawnFromReserve(targetTower);

            return true;
        }
        else {

            EventManager.TriggerEvent(MyEvents.EVENT_MESSAGE_TRIGGERED, new EventObject("빈 타일이 없습니다."));
            return false;
        }
    }

    private void RemoveAllFocus()
    {
        for (int i = 0; i < reserveIcons.Length; i++) {
            reserveIcons[i].UnfocusIcon();
        }
    }

    private bool CheckSameTowerSelected(int newIndex) {
        if (ClickManager.GetCurrentUser() == MouseUser.RESERVE_SPAWNER)
        {
            if (newIndex == selectedIndex)
            {
                towerSpawner.AbortReserveSpawn();
                return true;
            }
        }
        selectedIndex = newIndex;
        return false;
    }
    private int FindIndexByUID(string requestedUID)
    {
        for (int i = 0; i < towerInventory.Count; i++) {
            if (towerInventory[i] == null) continue;
            if (towerInventory[i].GetComponent<Tower>().GetUID().Equals(requestedUID)) {
                return i;
                //return towerInventory[i];
            }
        }
        return -1;
    }

    public static void RemoveTowerFromReserve(bool giveReward)
    {
        Debug.Assert(instance.selectedIndex != -1, "선택되지않은 예비타워가 삭제됨");
        Tower t = instance.towerInventory[instance.selectedIndex].GetComponent<Tower>();
        TowerSpawner.CheckSellingEvent(t.GetCharacterID());
        string requestedUID = t.GetUID();

        instance.inventoryDictionary[requestedUID]--;
        if (instance.inventoryDictionary[requestedUID] <= 0) {
            instance.inventoryDictionary.Remove(requestedUID);
        }
        if (giveReward)
        {
            GameSession session = GameSession.GetGameSession();
            session.mineralManager.AddResource(UpgradeType.VOCAL, t.sell_Vocal);
            session.mineralManager.AddResource(UpgradeType.DANCE, t.sell_Dance);
            session.mineralManager.AddResource(UpgradeType.VISUAL, t.sell_Visual);
            t.KillItself();
        }
        instance.towerInventory.RemoveAt(instance.selectedIndex); //지우는 위치는 작위인데 추가 삭제는 항상 currSize에 넣엇 발생하는 에러
        instance.selectedIndex = -1;

        instance.SortInventoryIcons();
    }
    public static void ResetReserves() {
        for (int i = 0; i < instance.towerInventory.Count; i++)
        {
            Tower t = instance.towerInventory[i].GetComponent<Tower>();
            t.KillItself();
        }
        instance.towerInventory = new List<GameObject>();
        instance.selectedIndex = -1;
        instance.inventoryDictionary = new Dictionary<string, int>();
        instance.Hide();
    }

    private void UpdateInventoryAmountUI() {
        amountText.text = towerInventory.Count + "/" + MAX_INVENTORY_SIZE;
    }


    private void AddToDictionary(Tower towerComp)
    {
        string myUID = towerComp.GetUID();
        if (inventoryDictionary.ContainsKey(myUID))
        {
            inventoryDictionary[myUID]++;
        }
        else
        {
            inventoryDictionary.Add(myUID, 1);
            if (!uidToTower.ContainsKey(myUID)) {
                uidToTower.Add(myUID, towerComp.GetPortraitSprite());
            }
        }
    }

    private void SortInventoryIcons()
    {
        if (towerInventory.Count > 0)
        {
            SetVisibility(true);
        }
        else if (towerInventory.Count == 0) {
            SetVisibility(false);
        }
        var sortedDict = from entry in inventoryDictionary orderby entry.Value descending select entry;
        
        int index = 0;
        foreach (KeyValuePair<string, int> entry in sortedDict) {
            Sprite towerSprite = uidToTower[entry.Key];
            reserveIcons[index++].SetIcon(towerSprite, entry);
        }
        for (; index < MAX_INVENTORY_SIZE; index++)
        {
            reserveIcons[index].HideIcon();
        }
        UpdateInventoryAmountUI();
    }


    private void SetVisibility(bool enable) {
        if (enable)
        {
            Show();
        }
        else {
            Hide();
        }
        title.SetActive(enable);
    }
    private void Show()
    {
        GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        GetComponent<RectTransform>().DOLocalMoveY(-340f, 0.3f);
    }


    public void Hide()
    {
        GetComponent<RectTransform>().DOLocalMoveY(-540f, 0.3f).OnComplete(
            () =>
            {
                GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            }

        );
    }
}
