using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LocalizationManager;

public class Lesson_InfoPanel : MonoBehaviour
{
    [Header("Vocal Line")]
    [SerializeField] GameObject vocal_Object;
    [SerializeField] Image vocal_slider;
    [SerializeField] TextMeshProUGUI vocal_costText;
    [SerializeField] Text vocal_levelText;
    [SerializeField] Text vocal_descText;
    [SerializeField] Button vocal_upgradeButton;

    [Header("Visual Line")]
    [SerializeField] GameObject visual_Object;
    [SerializeField] Image visual_slider;
    [SerializeField] TextMeshProUGUI visual_costText;
    [SerializeField] Button visual_upgradeButton;
    [SerializeField] Text visual_descText;
    [SerializeField] Text visual_levelText;

    [Header("Dance Line")]
    [SerializeField] Image dance_slider;
    [SerializeField] TextMeshProUGUI dance_costText;
    [SerializeField] Button dance_upgradeButton;
    [SerializeField] Text dance_descText;
    [SerializeField] GameObject[] da_level_indicators;


    [Header("Result Panel")]
    [SerializeField] Text vocalFinal;
    [SerializeField] Text visualFinal;
    [SerializeField] Text damageFinal;


    [Header("Skill Panel")]
    [SerializeField] LessonPanel_SkillDisplay[] skillDisplays;
    [SerializeField] Text previewDescText;


    UnitConfig currentConfig;


    string maxText;
    string upgradeText;
    public void SetInfoPanel(UnitConfig unitConfig)
    {
        currentConfig = unitConfig;
        maxText = Convert("TXT_KEY_MAX");
        upgradeText = Convert("TXT_KEY_UPGRADE");
        UpdateInfoPanel();
    }

    public void UpdateInfoPanel()
    {
        if (currentConfig.GetCharacterID() == TowerCharacter.Azusa)
        {
            //vocal_Object.SetActive(false);
            visual_Object.SetActive(false);
        }
        else
        {
            vocal_Object.SetActive(true);
            visual_Object.SetActive(true);
            SetVisual();
        }
        SetVocal();
        SetDance();
        SetResult();
        SetSkill();
    }

    private void SetResult()
    {
        float vocalDamage = currentConfig.GetFinalVocalDamage();
        float visualDamage = currentConfig.GetFinalVisualDamage();
        vocalFinal.text = vocalDamage.ToString();
        visualFinal.text = visualDamage.ToString();
        damageFinal.text = ((int)(vocalDamage * visualDamage)).ToString();
    }

    private void SetDance()
    {
        int currUp = UpgradeManager.GetUpgradeValue(UpgradeType.DANCE, currentConfig.GetUID());
        int max = UpgradeManager.GetMaxUpgrade(UpgradeType.DANCE, currentConfig.GetUID());
        if (currUp >= max)
        {
            dance_upgradeButton.interactable = false;
            dance_upgradeButton.GetComponentInChildren<Text>().text = maxText;
            dance_costText.text = "";
        }
        else
        {
            dance_upgradeButton.interactable = true;
            dance_costText.text = UpgradeManager.GetUpgradeCost(UpgradeType.DANCE, currentConfig.GetUID()).ToString();
            dance_upgradeButton.GetComponentInChildren<Text>().text = upgradeText;
        }
        for (int i = 0; i < da_level_indicators.Length; i++)
        {

            da_level_indicators[i].GetComponent<Image>().color =
                (i < currUp) ? new Color(0, 0, 255) : new Color(255, 255, 255);
        }
        dance_slider.fillAmount = (float)currUp / max;

    }

    private void SetVisual()
    {
        int currUp = UpgradeManager.GetUpgradeValue(UpgradeType.VISUAL, currentConfig.GetUID());
        int max = UpgradeManager.GetMaxUpgrade(UpgradeType.VISUAL, currentConfig.GetUID());



        visual_costText.text = UpgradeManager.GetUpgradeCost(UpgradeType.VISUAL, currentConfig.GetUID()).ToString();
        if (currUp >= max)
        {
            visual_upgradeButton.interactable = false;
            visual_upgradeButton.GetComponentInChildren<Text>().text = maxText;
            visual_costText.text = "";
        }
        else
        {
            visual_upgradeButton.interactable = true;
            visual_costText.text = UpgradeManager.GetUpgradeCost(UpgradeType.VISUAL, currentConfig.GetUID()).ToString();
            visual_upgradeButton.GetComponentInChildren<Text>().text = upgradeText;
        }
        visual_levelText.text = currUp.ToString();


        visual_slider.fillAmount = (float)currUp / max;
        SetVisualDesc(currUp);
    }

    private void SetVocal()
    {
        int currUp = UpgradeManager.GetUpgradeValue(UpgradeType.VOCAL, currentConfig.GetUID());
        int max = UpgradeManager.GetMaxUpgrade(UpgradeType.VOCAL, currentConfig.GetUID());
        vocal_levelText.text = currUp.ToString();
        vocal_costText.text = UpgradeManager.GetUpgradeCost(UpgradeType.VOCAL, currentConfig.GetUID()).ToString();
        if (currUp >= max)
        {
            vocal_upgradeButton.interactable = false;
            vocal_upgradeButton.GetComponentInChildren<Text>().text = maxText;
            vocal_costText.text = "";
        }
        else
        {
            vocal_upgradeButton.interactable = true;
            vocal_costText.text = UpgradeManager.GetUpgradeCost(UpgradeType.VOCAL, currentConfig.GetUID()).ToString();
            vocal_upgradeButton.GetComponentInChildren<Text>().text = upgradeText;
        }
        vocal_slider.fillAmount = (float)currUp / max;

        SetVocalDesc(currUp);
    }

    private void SetVocalDesc(int currUp)
    {
        vocal_descText.text = Convert("TXT_KEY_LESSON_VOCAL_DESC",
                                    "<color=#C80000>" + currUp + "</color>",
                                    currentConfig.attackUpgradeIncrement.ToString(),
                                   // currentConfig.attackDamage.ToString(),
                                     (currentConfig.attackDamage + currentConfig.attackUpgradeIncrement * currUp).ToString()
                                    );


    }
    private void SetVisualDesc(int currUp)
    {

        visual_descText.text = Convert("TXT_KEY_LESSON_VISUAL_DESC",
                            "<color=#C80000>" + currUp + "</color>",
                            currentConfig.VisualUpgradeIncrement.ToString(),
                             (currentConfig.VisualUpgradeIncrement * currUp).ToString()
                            );
    }
    private void SetSkill()
    {
        SetDefaultSkillDesc();
    }

    public void SetDefaultSkillDesc()
    {

        previewDescText.gameObject.SetActive(false);
        int skillLevel = UpgradeManager.GetUpgradeValue(UpgradeType.DANCE, currentConfig.GetUID());
        for (int i = 0; i < skillDisplays.Length; i++)
        {
            if (i < currentConfig.skillConfigs.Count)
            {
                skillDisplays[i].SetDisplayInfo(currentConfig.skillConfigs[i], skillLevel);
            }
            else
            {
                skillDisplays[i].SetDisplayInfo(null, skillLevel);
            }
        }
    }

    public void SetDanceUpgradeDesc(int level)
    {
        for (int i = 0; i < skillDisplays.Length; i++)
        {
            skillDisplays[i].SetDisplayInfo(null, 0);
        }
        previewDescText.gameObject.SetActive(true);
        switch (level)
        {
            case 1:
                previewDescText.text = Convert(currentConfig.dance_upgrade_text_1);
                break;
            case 2:
                previewDescText.text = Convert(currentConfig.dance_upgrade_text_2);
                break;
            case 3:
                previewDescText.text = Convert(currentConfig.dance_upgrade_text_3);
                break;
            case 4:
                previewDescText.text = Convert(currentConfig.dance_upgrade_text_4);
                break;


        }


    }

}
