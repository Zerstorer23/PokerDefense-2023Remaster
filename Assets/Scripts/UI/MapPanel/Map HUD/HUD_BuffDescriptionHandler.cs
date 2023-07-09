using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_BuffDescriptionHandler : MonoBehaviour
{
    public HUD_BuffIconHandler[] buffIcons;
    public HUD_ProjectileHandler projHandler;
    BuffManager focusedBuffset = null;
    public GameObject mainPanel = null;
    public BoxCollider2D clickBlock;
    //--------display
    public Text buffName;
    public Text buffBy;
    public Text buffEffect;
    public Image buffIcon;
    public Image buffByPortrait;

    //========

    private void Awake()
    {
        //Debug.Log("bufdesc init start");
        foreach (HUD_BuffIconHandler icon in buffIcons)
        {
            icon.gameObject.SetActive(false);
            icon.descriptionBox = this;
        }
     //   Debug.Log("bufdesc init end");
    }

    public void SetVisibility(bool enable)
    {
        projHandler.gameObject.SetActive(!enable);
        mainPanel.SetActive(enable);
    }

    internal void SetDisplay(Buff thisBuff)
    {
        if (thisBuff.triggerTower == null) return;
        buffName.text = LocalizationManager.Convert(thisBuff.txt_buff_name);
        buffBy.text = LocalizationManager.Convert(thisBuff.triggerTower.myConfig.txt_name);

        string bufftypename = thisBuff.GetBuffDescription();

        buffEffect.text = bufftypename;
        buffIcon.sprite = Buff.GetBuffImage(thisBuff.GetBuffType());
        buffByPortrait.sprite = thisBuff.triggerTower.GetPortraitSprite();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBuffInformation();
    }

    internal void SetBuffInformations(BuffManager buffManager)
    {
        if (buffManager == null) {
            HideAllIcons();
            clickBlock.enabled = false;
            return;
        }
        focusedBuffset = buffManager;
        clickBlock.enabled = true;
        UpdateBuffInformation();
    }

    void UpdateBuffInformation()
    {
        if (focusedBuffset == null) return;
        List<Buff> bufflist = focusedBuffset.GetActiveBuffs();
        for (int i = 0; i < buffIcons.Length; i++)
        {
            if (i < bufflist.Count)
            {
                buffIcons[i].gameObject.SetActive(true);
                buffIcons[i].SetInformation(bufflist[i]);
            }
            else
            {
                buffIcons[i].gameObject.SetActive(false);
            }
        }
    }

    void HideAllIcons()
    {
        focusedBuffset = null;
        foreach (HUD_BuffIconHandler icon in buffIcons) {
            icon.gameObject.SetActive(false);
        }
        SetVisibility(false);
    }

}
