using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRelocator : MonoBehaviour
{
    TowerSpawner towerSpawner;
    bool relocationMode = false;
    GameObject selectedObject = null;

    private void Awake()
    {
     //   Debug.Log("towerrelocator srtart");
        EventManager.StartListening(MyEvents.EVENT_TOWER_RELOCATION_REQUESTED, OnTowerRelocateRequested);
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, OnPanelChanged);
        EventManager.StartListening(MyEvents.EVENT_BACKGROUND_CLICKED, AbortPlaceMode);
        EventManager.StartListening(MyEvents.EVENT_CLICK_TOWER, AbortPlaceMode);
        EventManager.StartListening(MyEvents.EVENT_CLICK_ENEMY, AbortPlaceMode);
        EventManager.StartListening(MyEvents.EVENT_TOWER_SELL_REQUESTED, OnTowerSellRequested);
        towerSpawner = GetComponent<TowerSpawner>();
      //  Debug.Log("towerrelocator end");
    }
    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_TOWER_RELOCATION_REQUESTED, OnTowerRelocateRequested);
        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, OnPanelChanged);
        EventManager.StopListening(MyEvents.EVENT_BACKGROUND_CLICKED, AbortPlaceMode);
        EventManager.StopListening(MyEvents.EVENT_CLICK_TOWER, AbortPlaceMode);
        EventManager.StopListening(MyEvents.EVENT_CLICK_ENEMY, AbortPlaceMode);
        EventManager.StopListening(MyEvents.EVENT_TOWER_SELL_REQUESTED, OnTowerSellRequested);
    }

    private void OnTowerRelocateRequested(EventObject lexington)
    {
        selectedObject = lexington.GetGameObject();
        ToggleRelocateMode(true);
    }
    private void ToggleRelocateMode(bool nowRelocate)
    {
        if (relocationMode == nowRelocate) return;
        ClickManager.ToggleUser(MouseUser.RELOCATOR, nowRelocate);
        relocationMode = nowRelocate;
        towerSpawner.ActivateCursor(relocationMode);
        if (relocationMode) {
            towerSpawner.cursor.SetRadius(selectedObject.GetComponent<Tower>().attackDistance);
        }
        GetComponent<PolygonCollider2D>().enabled = relocationMode;
 
    }
    private void OnMouseDown()
    {
        if (ClickManager.GetCurrentUser() != MouseUser.RELOCATOR) return;
        if (selectedObject == null) return;
        if (towerSpawner.cursor.CanPlace())
        {
            ChangeTowerLocation();

            ToggleRelocateMode(false);
        }
        return;
    }
    private void ChangeTowerLocation() {
        Tower t = selectedObject.GetComponent<Tower>();
        towerSpawner.mapInfo.towerOccupiedMap[(int)t.mapPosition.x, (int)t.mapPosition.y] = null;

        selectedObject.transform.position = towerSpawner.cursor.GetMousePosition();
        towerSpawner.mapInfo.AddTowerOnMap(t);

        EventManager.TriggerEvent(MyEvents.EVENT_TOWER_PLACED, new EventObject(selectedObject));
        selectedObject = null;
    }

    private void OnTowerSellRequested(EventObject lexington)
    {
        AbortPlaceMode(null);
        GameObject unitObject = lexington.GetGameObject();
        bool giveReward = lexington.boolObj;
        Tower t = unitObject.GetComponent<Tower>(); 
        TowerSpawner.CheckSellingEvent(t.GetCharacterID());
        string uid = t.GetUID();
        towerSpawner.RemoveTowerFromMapByGameID(unitObject,true);
        if (giveReward) {
            towerSpawner.gameSession.mineralManager.AddResource(UpgradeType.VOCAL, t.sell_Vocal);
            towerSpawner.gameSession.mineralManager.AddResource(UpgradeType.DANCE, t.sell_Dance);
            towerSpawner.gameSession.mineralManager.AddResource(UpgradeType.VISUAL, t.sell_Visual);
            StatisticsManager.AddToStat("TOTAL_SELL_" + uid,1);
        }
        
    }

    void OnPanelChanged(EventObject eo) {
        ScreenType type = eo.screenType;
        if (type != ScreenType.MAP)
        { 
            AbortPlaceMode(null);
        }
    }


    public void AbortPlaceMode(EventObject eo)
    {
        if (selectedObject != null)
        {
           // selectedObject.GetComponent<Tower>().focusedSprite.gameObject.SetActive(true);
        }
        selectedObject = null;
       // Debug.Log("Abort relocation...from "+relocationMode);
        ToggleRelocateMode(false);
    }
}
