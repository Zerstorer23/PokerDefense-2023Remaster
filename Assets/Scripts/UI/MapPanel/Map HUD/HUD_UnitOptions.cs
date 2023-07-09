using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HUD_UnitOptions : MonoBehaviour
{
    public Button[] buttons;
    public string[] hexColors = new string[3] { "#FF603E", "#FFE400", "#5DB9FF" };
    public Sprite[] coinImages;
    public Text[] coinAmounts;
    public Image[] coins;
    [SerializeField] MainHudManager mainHud;
    public BoxCollider2D clickBlock;

    public GameObject focusedObject = null;
    SellType sellType = SellType.None;

    bool visibility = false;
    private void Awake()
    {
     //   Debug.Log("unitoption init srtart");
        EventManager.StartListening(MyEvents.EVENT_CLICK_ENEMY, OnHideOptions);
        EventManager.StartListening(MyEvents.EVENT_SHOW_TOWER_OPTIONS, OnClickSpawnPool);
        EventManager.StartListening(MyEvents.EVENT_CLICK_TOWER, OnClickTower);
        EventManager.StartListening(MyEvents.EVENT_BACKGROUND_CLICKED, OnHideOptions);
        EventManager.StartListening(MyEvents.EVENT_SHOW_PURCHASE_OPTION, OnHideOptions);
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, OnPanelChanged);
      //  EventManager.StartListening(MyEvents.EVENT_MOUSE_USER_CHANGED, OnMouseChanged);
        clickBlock = GetComponent<BoxCollider2D>();
     //   Debug.Log("unitoption init end");
    }

    private void OnPanelChanged(EventObject arg0)
    {
        ScreenType scr = arg0.screenType;
        if (scr != ScreenType.MAP) {
            OnHideOptions(null);
        }
    }

    private void OnDestroy()
    {
        //   EventManager.StopListening(MyEvents.EVENT_UNITOPTION_SHOW_REQUEST, OnShowOptions);
        // EventManager.StopListening(MyEvents.EVENT_UNITOPTION_HIDE_REQUEST, OnHideOptions);
        EventManager.StopListening(MyEvents.EVENT_SHOW_PURCHASE_OPTION, OnHideOptions);
        EventManager.StopListening(MyEvents.EVENT_CLICK_ENEMY, OnHideOptions);
        EventManager.StopListening(MyEvents.EVENT_SHOW_TOWER_OPTIONS, OnClickSpawnPool);
        EventManager.StopListening(MyEvents.EVENT_CLICK_TOWER, OnClickTower);
        EventManager.StopListening(MyEvents.EVENT_BACKGROUND_CLICKED, OnHideOptions);
        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, OnPanelChanged);
        //   EventManager.StopListening(MyEvents.EVENT_MOUSE_USER_CHANGED, OnHideOptions);
    }



    private void OnClickTower(EventObject eo)
    {
        GameObject obj = eo.GetGameObject();
        Tower t = obj.GetComponent<Tower>();
        if (t.owner == Owner.NAMCO)
        {
            clickBlock.enabled = true;
            sellType = SellType.Tower;
            OnShowOptions(obj);
            SetSellColor(t.sell_Vocal, t.sell_Visual, t.sell_Dance);
        }
        else
        {
            clickBlock.enabled = false;
            focusedObject = null;
            OnHideOptions(null);
        }
    }
    private void OnClickSpawnPool(EventObject eo)
    {
        bool show = eo.boolObj;
        if (show)
        {
            GameObject obj = eo.GetGameObject();
            sellType = SellType.Card;
            clickBlock.enabled = true;
            TowerSpawningButton tower = obj.GetComponent<TowerSpawningButton>();
            Debug.Assert(tower != null, "Click null1!");
            UnitConfig cfg = tower.GetConfig();
            OnShowOptions(obj, true, false, false);
            SetSellColor(cfg.sellValue_Vocal, cfg.sellValue_Visual, cfg.sellValue_Dance);
        }
        else
        {
            OnHideOptions(null);
        }

    }
    public void OnClickReservePool(GameObject towerObj)
    {
        Tower t = towerObj.GetComponent<Tower>();
        clickBlock.enabled = true;
        sellType = SellType.Reserve;
        OnShowOptions(towerObj, false, false, false);
        SetSellColor(t.sell_Vocal, t.sell_Visual, t.sell_Dance);
    }


    public void OnSellUnit()
    {
        if (focusedObject == null) return;
        switch (sellType)
        {
            case SellType.None:
                break;
            case SellType.Tower:
                EventManager.TriggerEvent(MyEvents.EVENT_TOWER_SELL_REQUESTED, new EventObject(focusedObject) { boolObj = true });
                OnHideOptions(null);
                break;
            case SellType.Card:
                GameSession.GetGameSession().towerSpawner.SellSelectedTowerInPool();
                break;
            case SellType.Reserve:
                ReserveManager.RemoveTowerFromReserve(true);
                GameSession.GetGameSession().towerSpawner.AbortReserveSpawn();
                OnHideOptions(null);
                break;
        }
    }
    public void OnReserveUnit()
    {
        if (focusedObject == null || !ReserveManager.HasEmptySpot()) return;
        switch (sellType)
        {
            case SellType.None:
                break;
            case SellType.Tower:
                ReserveManager.AddTowerReserve(focusedObject);
                HideTowerUI();
                OnHideOptions(null);
                break;
            case SellType.Card:
                GameSession.GetGameSession().towerSpawner.ReserveSelectedTowerInPool();
                break;
            case SellType.Reserve:
                break;
        }
    }

    public void OnRelocateUnit()
    {
        if (focusedObject == null) return;
        if (sellType == SellType.Tower)
        {
            EventManager.TriggerEvent(MyEvents.EVENT_TOWER_RELOCATION_REQUESTED, new EventObject(focusedObject));
            HideTowerUI();
        }
    }
    private void HideTowerUI()
    {

        focusedObject.GetComponentInChildren<Tower_OnClick>().HideDisplay();
        OnHideOptions(null);
    }

    public void OnHideOptions(EventObject eo)
    {
        if (visibility == false) return;
        visibility = false;
        focusedObject = null;
        sellType = SellType.None;
        foreach (Button bt in buttons)
        {

            bt.GetComponent<Image>().DOFade(0, 0.2f).OnComplete(
                    () =>
                    {
                        bt.gameObject.SetActive(false);
                    }
            );
        }
        CameraManager.CameraZoomAndMove(null, false);
    }

    private void OnShowOptions(GameObject eo, bool showReserve = true, bool showRelocation = true, bool doFocus = true)
    {
        visibility = true;
        focusedObject = eo;
        bool hasEmpty = ReserveManager.HasEmptySpot();



        buttons[0].gameObject.SetActive(true);
        buttons[1].gameObject.SetActive(hasEmpty && showReserve);
        buttons[2].gameObject.SetActive(showRelocation);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Image>().DOFade(1, 0.2f);
        }
        if (doFocus)
            CameraManager.CameraZoomAndMove(focusedObject, true);
    }
    void SetSellColor(int vocal, int visual, int dance)
    {
        int[] costs = new int[3] { vocal, visual, dance };
        int costIndex = 0;
        for (int i = 0; i < coins.Length; i++)
        {
            if (costIndex < costs.Length)
            {
                if (costs[costIndex] > 0)
                {
                    coins[i].enabled = true;
                    coins[i].sprite = coinImages[costIndex];
                    coinAmounts[i].text = costs[costIndex].ToString();
                    coinAmounts[i].color = ConstantStrings.GetColorByHex(hexColors[costIndex]);
                }
                else
                {
                    i--;
                }

            }
            else
            {
                coins[i].enabled = false;
                coinAmounts[i].text = "";
            }


            costIndex++;
        }


    }

}
public enum SellType { 
    None,Tower,Card,Reserve
}
