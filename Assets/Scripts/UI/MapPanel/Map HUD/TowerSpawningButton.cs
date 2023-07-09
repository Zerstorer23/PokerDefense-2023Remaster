using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerSpawningButton : MonoBehaviour
{
    Toggle toggle;
    UnitConfig config;
    string uid;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(
                (bool isOn)=> {
                    TriggerSelected();
                }
            );
    }

    internal void SetInfo(UnitConfig config)
    {
        GetComponent<Image>().sprite = config.GetPortraitSprite();
        uid = config.GetUID();
        this.config = config;

    }
    internal string GetUID() => uid;

    internal void TriggerSelected() {
        if (ClickManager.GetCurrentUser() == MouseUser.RESERVE_SPAWNER) {
            toggle.isOn = false;
        }
        EventManager.TriggerEvent(MyEvents.EVENT_TOWER_TO_SPAWN_SELECTED, new EventObject(gameObject));
    }

    internal UnitConfig GetConfig() => config;

 
}
