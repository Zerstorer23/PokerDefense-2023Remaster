using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCardObject : MonoBehaviour
{
    [SerializeField] internal Image cardImage;
    [SerializeField] PokerMachine pokerMachine;
    [SerializeField] GameObject button;
    [SerializeField] Text dpsText;

    public void SetCardImage(Sprite spriteImg) {
        cardImage.sprite = spriteImg;
    }

    public void ChangeThisCard(int i) {
        //   EventManager.TriggerEvent(MyEvents.EVENT_CARD_CHANGE_REQUESTED,new EventObject(i));
        pokerMachine.ReRollCardAt(i);
    }

    public void SetButtonVisibility(bool _visibility) {
        button.SetActive(_visibility);
    }

    internal void SetExpectedDPSChange(double expectedChange)
    {
        dpsText.text = expectedChange.ToString();
    }
}
