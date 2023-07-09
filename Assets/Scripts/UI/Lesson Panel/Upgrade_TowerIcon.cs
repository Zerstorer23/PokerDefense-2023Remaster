using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade_TowerIcon : MonoBehaviour
{
    string unit_ID;
    [SerializeField] LessonPanel lessonPanel;
    [SerializeField] UnitConfig unitConfig;
    [SerializeField] Image profileImage;
    [SerializeField] Image backgroundImage;
    public Text handText;
    private void Awake()
    {
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, OnPanelOpen);
    }
    private void OnDestroy()
    {

        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, OnPanelOpen);
    }

    private void OnPanelOpen(EventObject arg0)
    {
        if (arg0.screenType == ScreenType.LESSON) {
            bool enable = StatisticsManager.GetStat("TOTAL_SPAWN_" + unitConfig.uid) > 0;
            SetColor(enable);
        
        }
    }

    private void Start()
    {
        profileImage.sprite = unitConfig.GetPortraitSprite();
        backgroundImage.color = ConstantStrings.GetColorByHex(unitConfig.colorHex);
        handText.text = PokerHand.GetClassOfHand(unitConfig.baseHand);
    }
    void SetColor(bool enable) {
        backgroundImage.color = (enable) ? ConstantStrings.GetColorByHex(unitConfig.colorHex)
            : ConstantStrings.GetColorByHex("#464646");

    }

    public void OnButtonClick()
    {
        unit_ID = unitConfig.GetUID();
        lessonPanel.SetInfoPanel(unitConfig);
    }
}
