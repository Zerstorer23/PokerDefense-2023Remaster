using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryIconBehaviour : MonoBehaviour
{

    [SerializeField] Text amountText;
    [SerializeField] Image portraitImage;
    [SerializeField] Image boundaryImage;


    string thisID;
    int thisAmount;

    internal void HideIcon()
    {
        gameObject.SetActive(false);
    }

    internal void SetIcon(Sprite towerSprite, KeyValuePair<string, int> entry)
    {
        gameObject.SetActive(true);
        thisID = entry.Key;
        thisAmount = entry.Value;

        portraitImage.sprite = towerSprite;
        amountText.text = "" + thisAmount;
        UnfocusIcon();
    }

    public void OnClickReserve() {
        if (!ClickManager.IsNone() &&
            ClickManager.GetCurrentUser()!=MouseUser.RESERVE_SPAWNER
            ) return;

      bool triggered= ReserveManager.SelectReservedTower(thisID);
      boundaryImage.enabled = triggered;
    }
    public void UnfocusIcon() {
        boundaryImage.enabled = false;
    }

}
