using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainHudManager : MonoBehaviour
{

    /*
     IMPORTANT NOTE

    need collider at z = -2.
    z= -1 = spawn area.
     */
    [SerializeField] HUD_UnitOptions unitOptions;


    [Header("Portrait Panel")]
    [SerializeField] Image portraitImage;
    [SerializeField] Text unitName;
    [Header("Skill Panel")]
    [SerializeField] SkillIconBehaviour[] skillButtons;
    [SerializeField] HUD_BuffDescriptionHandler buffHandler;

    [SerializeField] HUD_ProjectileHandler projectileHandler;

    [Header("Info Panel")]
    [SerializeField] GameObject statBox;
    [SerializeField] Text killTextBox;
    [SerializeField] GameObject hpSlider;

    [Header("Managers")]
    [SerializeField] StatBoxManager statManager;
    [SerializeField] CustomColorManager colorManager;


    [Header("Leaderboard")]
    [SerializeField] TowerSpawner spawner;
    [SerializeField] Text[] leaderboards;
    public GameObject focusedUnit;
    bool isTower = true;

    //public static bool activeskillfired = false;

    private void Awake()
    {
      //  Debug.Log("display init start");
        //EventManager.enemyDiedEvent.AddListener(RemoveEnemy);
        EventManager.StartListening(MyEvents.EVENT_CLICK_TOWER, OnTowerClicked);
        EventManager.StartListening(MyEvents.EVENT_CLICK_ENEMY, OnEnemyClicked);
        EventManager.StartListening(MyEvents.EVENT_ENEMY_DEAD, OnSomeoneDied);
        EventManager.StartListening(MyEvents.EVENT_BACKGROUND_CLICKED, OnBackgroundClicked);
        EventManager.StartListening(MyEvents.EVENT_SHOW_PURCHASE_OPTION, OnBackgroundClicked);
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, OnPanelChanged);
        ClearDisplay();
     //   Debug.Log("display init end");
    }
    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_CLICK_TOWER, OnTowerClicked);
        EventManager.StopListening(MyEvents.EVENT_CLICK_ENEMY, OnEnemyClicked);
        EventManager.StopListening(MyEvents.EVENT_ENEMY_DEAD, OnSomeoneDied);
        EventManager.StopListening(MyEvents.EVENT_BACKGROUND_CLICKED, OnBackgroundClicked);
        EventManager.StopListening(MyEvents.EVENT_SHOW_PURCHASE_OPTION, OnBackgroundClicked);
        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, OnPanelChanged);

    }


    private void Update()
    {
        if (!focusedUnit) return;
        if (isTower)
        {
            UpdateTowerInfo(focusedUnit.GetComponent<Tower>());

        }
        else
        {
            UpdateEnemyInfo(focusedUnit.GetComponent<Enemy_Main>());
        }
    }



    private void UpdateTowerInfo(Tower tower)
    {
        statManager.SetTowerStatDisplay(tower); //Add buff activate trigger
    }

    void OnSomeoneDied(EventObject eo)
    {
        if (focusedUnit == null)
        {
            ClearDisplay();
        }
        else {
            if (isTower)
            {
                UpdateInfoPanelTower();
            }
    /*        else
            {
                UpdateInfoPanelTower();
            }*/
        }
    }


    void OnBackgroundClicked(EventObject lex) {
        RemoveFocuses();
    }

    private void OnPanelChanged(EventObject eo)
    {
        if (eo.screenType != ScreenType.MAP)
        {
            RemoveFocuses();
        }
    }

    void RemoveFocuses()
    {
        if (focusedUnit == null) return;
        if (isTower)
        {
            focusedUnit.GetComponentInChildren<Tower_OnClick>().HideDisplay();
        }
        else
        {
            focusedUnit.GetComponent<Enemy_Main>().RemoveFocusUI();
            
        }
        ClearDisplay();
        focusedUnit = null;
        isTower = false;
    }
    public void ClearDisplay()
    {
        SetInfoPanel(" "," ", null);
        SetPortraitImage(null);
        foreach (SkillIconBehaviour icon in skillButtons) {
            icon.ShowDescription(false);
        }
        projectileHandler.SetProfectileInfo("",null);
        buffHandler.SetBuffInformations(null);
        statManager.HideNameFields();
    }
    void OnTowerClicked(EventObject eo)
    {

        if (ClickManager.GetCurrentUser() == MouseUser.ACTIVE_SKILL) return;
        RemoveFocuses();
        GameObject unitObject = eo.gameObject;
        focusedUnit = unitObject;
        SetTowerPanel(unitObject);
    }

    public void SetTowerPanel(GameObject unitObject)
    {
        Tower tower = unitObject.GetComponent<Tower>();
        isTower = true;
        List<Skill> skills = tower.GetSkills();
        SetPortraitImage(tower.GetPortraitSprite());
        projectileHandler.SetProfectileInfo(tower.GetUID(),tower.firepowerManager.myProjConfig);

        string towerName = LocalizationManager.Convert(tower.myConfig.txt_name);
        SetInfoPanel(towerName, 
            LocalizationManager.Convert("TXT_KEY_STAT_KILLS") + " "  + tower.GetKills()
            , skills);

        statManager.SetTowerStatDisplay(tower);
        colorManager.SetBoundaryColor(tower.myConfig.colorHex);
        buffHandler.SetBuffInformations(tower.buffManager);
        UpdateHP(null);
    }


    void OnEnemyClicked(EventObject eo)
    {
        if (ClickManager.GetCurrentUser() == MouseUser.ACTIVE_SKILL) return;


        RemoveFocuses();
        GameObject unitObject = eo.gameObject;
        focusedUnit = unitObject;
        Enemy_Main enemy = unitObject.GetComponent<Enemy_Main>();
        HealthPoint healthManager = unitObject.GetComponent<HealthPoint>();
        isTower = false;
        int hp = (int)healthManager.GetHP();
        if (hp < 0) hp = 0;
        SetInfoPanel(enemy.GetName(), hp + "/"+ healthManager.GetFullHP(),null);
        projectileHandler.SetProfectileInfo("",null);
        statManager.SetEnemyStatDisplay(enemy);
        buffHandler.SetBuffInformations(enemy.buffManager);
        SetPortraitImage(enemy.GetPortaitImage());
    }

    void SetInfoPanel(String name, string kills,List<Skill> skills) {

        unitName.text = name;
      
        killTextBox.text = kills;
        ParseSkills(skills);
    }
    private void SetPortraitImage(Sprite sprite)
    {
        if (sprite == null)
        {
            portraitImage.enabled = false;
            return;
        }
        portraitImage.enabled = true;
        portraitImage.sprite = sprite;
    }


    void UpdateInfoPanelTower() {
        if (focusedUnit == null) return;
        int kills = focusedUnit.GetComponent<Tower>().GetKills();
        killTextBox.text = LocalizationManager.Convert("TXT_KEY_STAT_KILLS")+" "+ kills;
        UpdateHP(null);
    }
    private void UpdateEnemyInfo(Enemy_Main enemy)
    {
        if (focusedUnit == null) return;
        UpdateHP(enemy.GetComponent<HealthPoint>());
    }

    private void UpdateHP(HealthPoint healthManager) {
        if (healthManager == null) {
            hpSlider.SetActive(false);
            return;
        }
        hpSlider.SetActive(true);
        int hp = (int)healthManager.GetHP();
        if (hp < 0) hp = 0;
        killTextBox.text = hp + "/" + healthManager.GetFullHP();
        Slider sl = hpSlider.GetComponent<Slider>();
        sl.value = (float)(healthManager.GetHP() / healthManager.GetFullHP());
    }



    private void ParseSkills(List<Skill> skills)
    {
        int buttonIndex = 0;
        if (skills == null) {
            for(;buttonIndex<skillButtons.Length;buttonIndex++)
            {
                skillButtons[buttonIndex].ClearSkill();
            }
            return;
        }
        foreach (Skill skill in skills) {
            skillButtons[buttonIndex++].SetSkill(skill);
            if (buttonIndex >= skillButtons.Length) {
                break;
            }
        }

    }
}
