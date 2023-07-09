using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_PurchaseOptions : MonoBehaviour
{
    public Button[] buttons;
    public Text[] texts;
    public Image[] boundary;
    public Image[] coins;
    public Sprite[] coinImages;
    [SerializeField] MainHudManager mainHud;


    public GameObject focusedObject = null;
    Tower tower = null;
    ConstructionArea construction = null;
    bool isTower = false;
    public BoxCollider2D clickBlock;

    UpgradeType[] costTypesTower = new UpgradeType[2] { UpgradeType.DANCE, UpgradeType.VOCAL };
    UpgradeType[] costTypesTile = new UpgradeType[2] { UpgradeType.DANCE, UpgradeType.VISUAL };
    int[] tower_cost =new int[2] {2,765 };
    int[] tile_cost = new int[2] { 4, 3 };
    public string[] hexColors = new string[3] { "#FF603E", "#FFE400", "#5DB9FF" };



    public delegate void eventFunction(EventObject eo);
    private void Awake()
    {
   //     Debug.Log("purchase init srtart");
        EventManager.StartListening(MyEvents.EVENT_CLICK_TOWER, OnShowOptions);
        EventManager.StartListening(MyEvents.EVENT_SHOW_PURCHASE_OPTION, OnShowOptions);

        EventManager.StartListening(MyEvents.EVENT_CLICK_ENEMY, OnHideOptions);
        EventManager.StartListening(MyEvents.EVENT_BACKGROUND_CLICKED, OnHideOptions);
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, OnHideOptions);
        EventManager.StartListening(MyEvents.EVENT_MOUSE_USER_CHANGED, OnMouseChanged);
        clickBlock = GetComponent<BoxCollider2D>();
     //   Debug.Log("purchase init end");
    }

    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_CLICK_TOWER, OnShowOptions);
        EventManager.StopListening(MyEvents.EVENT_SHOW_PURCHASE_OPTION, OnShowOptions);

        EventManager.StopListening(MyEvents.EVENT_CLICK_ENEMY, OnHideOptions);
        EventManager.StopListening(MyEvents.EVENT_BACKGROUND_CLICKED, OnHideOptions);
        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, OnHideOptions);
        EventManager.StopListening(MyEvents.EVENT_MOUSE_USER_CHANGED, OnMouseChanged);
    }
    private void OnShowOptions(EventObject eo)
    {
        if (ClickManager.GetCurrentUser() != MouseUser.NONE) return;
        DelegateOnShowOptions(eo);
      //  StartCoroutine(WaitAndShow(DelegateOnShowOptions,eo));
    }
    private void DelegateOnShowOptions(EventObject eo)
    {
        //  Debug.Log("Show");
        focusedObject = eo.GetGameObject();
        if (focusedObject == null) return;
        bool show = SetUpInformation();
        clickBlock.enabled = show;
        if (!show)
        {
            OnHideOptions(null);
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            Button bt = buttons[i];
            bt.gameObject.SetActive(true);
        }


        CameraManager.CameraZoomAndMove(focusedObject, true);
    }

    IEnumerator WaitAndShow(eventFunction func, EventObject eo) {
        yield return new WaitForFixedUpdate();
        func(eo);
    }

    private bool SetUpInformation()
    {
        HideTileFocus();
        tower = focusedObject.GetComponent<Tower>();
        isTower = (tower != null);
        UpgradeType typeOne  = (isTower) ? costTypesTower[0] : costTypesTile[0];
        UpgradeType typeTwo  = (isTower) ? costTypesTower[1] : costTypesTile[1];
        boundary[0].color = ConstantStrings.GetColorByHex(hexColors[(int)typeOne]);
        boundary[1].color = ConstantStrings.GetColorByHex(hexColors[(int)typeTwo]);
        coins[0].sprite = coinImages[(int)typeOne];
        coins[1].sprite = coinImages[(int)typeTwo];


        if (isTower)
        {
            if((tower.owner == Owner.KUROI))
            TutorialManager.CheckTutorial("LearnSellKuroi");
            texts[0].text = "x" + tower_cost[0] + "\n" + LocalizationManager.Convert("TXT_KEY_PURCHASE_TOWER_0");
            texts[1].text = "x" + tower_cost[1] + "\n" + LocalizationManager.Convert("TXT_KEY_PURCHASE_TOWER_1");
            return (tower.owner == Owner.KUROI);

        }
        else
        {
            TutorialManager.CheckTutorial("LearnPurchaseTile");
            construction = focusedObject.GetComponent<ConstructionArea>();
            texts[0].text = "x" + tile_cost[0] + "\n" + LocalizationManager.Convert("TXT_KEY_PURCHASE_TILE_0");
            texts[1].text = "x" + tile_cost[1] + "\n" + LocalizationManager.Convert("TXT_KEY_PURCHASE_TILE_1");
            return construction != null;
        }
    }

    private void OnHideOptions(EventObject eo)
    {
        HideTileFocus();
        focusedObject = null;
        isTower = false;
        tower = null;
        construction = null;

        foreach (Button bt in buttons)
        {
            bt.gameObject.SetActive(false);
        }
        CameraManager.CameraZoomAndMove(null, false);
    }
    private void HideTileFocus() {
        if (construction != null) {
            construction.focusSprite.enabled = false;
        }
    }
    //-----stat------//
    int numTileBought = 0;
    public void OnClickPurchase(int option) {
        int cost = (isTower) ? tower_cost[option] : tile_cost[option];
        UpgradeType costType = (isTower) ? costTypesTower[option] : costTypesTile[option];

        bool success= GameSession.GetGameSession().mineralManager.SpendResource(costType, cost);
        if (success)
        {
            if (isTower)
            {
                TutorialManager.CheckTutorial("KuroiReaction ");
                GooglePlayManager.IncrementAchievement(IdolDefense.achievement_shiika_no_tameni);
                EventManager.TriggerEvent(MyEvents.EVENT_TOWER_SELL_REQUESTED, new EventObject(focusedObject) { boolObj = false });
            }
            else {
                numTileBought++;
                if (numTileBought == 2) {
                    GooglePlayManager.AddAchievement(IdolDefense.achievement_yayoi_no_yume);
                }
                Destroy(focusedObject);
            }
            OnHideOptions( null);
        }
        else {
            EventManager.TriggerEvent(MyEvents.EVENT_MESSAGE_TRIGGERED, new EventObject(LocalizationManager.Convert("TXT_KEY_NOT_ENOUGH_RESOURCES")));
        }
    }

    private void OnMouseChanged(EventObject eo)
    {
        MouseUser newUser = (MouseUser)eo.GetInt();
        if (newUser == MouseUser.RESERVE_SPAWNER)
        {
            OnHideOptions(null);
        }
    }


}
