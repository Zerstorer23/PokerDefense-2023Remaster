using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LessonPanel : MonoBehaviour
{
    [SerializeField] Lesson_InfoPanel infoPanel;
    [SerializeField] UnitConfig defaultConfig;
    [SerializeField] CharacterBodyManager profileCharacter;

    RectTransform rectTransform;
    bool visibility = false;
    UnitConfig currentConfig = null;

    internal ScreenType mType = ScreenType.LESSON;

    public void SetCanvasVisibility(bool isVisible, float delay)
    {
       // Debug.Log(mType + "  visibility to " + isVisible);
        if (visibility != isVisible)
        {
            visibility = isVisible;
            if (visibility)
            {
                Show(delay);
                if (currentConfig == null) {
                    currentConfig = defaultConfig;
                }
                Debug.Log("Visible...");
                infoPanel.SetInfoPanel(currentConfig);
                CountPanelOpen();

            }
            else
            {

                Hide(delay);
            }
        }

    }

    private void CountPanelOpen()
    {
        int numOpen = StatisticsManager.AddToStat(ConstantStrings.STAT_LESSON_OPENED, 1);
        switch (numOpen)
        {
            case 1:
                TutorialManager.CheckTutorial("LearnVocal");
                break;
            case 2:

                TutorialManager.CheckTutorial("LearnVisual");
                break;
            case 3:
                TutorialManager.CheckTutorial("LearnDance");
                break;
        }
    }

    public void UpgradeUnit(int type_id)
    {
        profileCharacter.SetAnimationTrigger("DoJump");
        UpgradeManager.DoUpgrade((UpgradeType)type_id, currentConfig.GetUID());
        infoPanel.UpdateInfoPanel();
    }

    bool doAutoUpgrade = false;
    public float upgradeStartTime = 0;
    Coroutine upgradeRoutine;
    public void UpgradeUnit_Auto(int type_id)
    {
        profileCharacter.SetAnimationTrigger("DoJump");
        doAutoUpgrade = true;
        upgradeStartTime = Time.time;
        upgradeRoutine = StartCoroutine(DoSerialUpgrade((UpgradeType)type_id));
    }
    IEnumerator DoSerialUpgrade(UpgradeType upgradeType)
    {
        while (doAutoUpgrade) {
            bool success = UpgradeManager.DoUpgrade(upgradeType, currentConfig.GetUID());
            if (success)
            {
                infoPanel.UpdateInfoPanel();
            }
            else {
                doAutoUpgrade = false;  
            }

            float timePassed = Time.time - upgradeStartTime;
            if (timePassed < 0.75f)
            {
                yield return new WaitForSeconds(0.75f);
            }
            else if (timePassed < 2f)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return new WaitForSeconds(0.05f);

            }
        }

    }
    public void UpgradeUnit_Stop(int type_id)
    {
        StopCoroutine(upgradeRoutine);
        doAutoUpgrade = false;
        upgradeStartTime = Time.time;
      //  Debug.Log("Pointer up " + Time.time);
    }

    internal void SetInfoPanel(UnitConfig unitConfig)
    {

        currentConfig = unitConfig;
        infoPanel.SetInfoPanel(unitConfig);
        profileCharacter.SetEyeSkin(unitConfig.characterConfig.eyeID);
        profileCharacter.SetMouthSkin(unitConfig.GetUID());
        profileCharacter.SetHairSkins(unitConfig.GetUID());
        profileCharacter.SetAnimationBool("DoUpgradeIdle",true);
    }

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Show(float delay = 0f)
    {
        gameObject.SetActive(true);
        Vector2 tar = new Vector2(960, 0);
        rectTransform.DOLocalMove(tar, delay).OnComplete(
             () => {
                 EventManager.TriggerEvent(MyEvents.EVENT_CANVAS_TRANSITION_FINISHED, null);
             });
    }


    private void Hide(float delay = 0f)
    {
        Vector2 tar = new Vector2(-960, 0);
        rectTransform.DOLocalMove(tar, delay).OnComplete(
             () =>{
                 gameObject.SetActive(false);
                 EventManager.TriggerEvent(MyEvents.EVENT_CANVAS_TRANSITION_FINISHED, null);
                }
        );
    }

}
