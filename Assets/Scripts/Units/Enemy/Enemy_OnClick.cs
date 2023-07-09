using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_OnClick : MonoBehaviour
{
    [SerializeField] GameObject parentMain;
    [SerializeField] SpriteRenderer focusedSprite;
    bool isFocused = false;
    private void OnMouseDown()
    {
        EventManager.TriggerEvent(MyEvents.EVENT_CLICK_ENEMY, new EventObject(parentMain));
        ShowDisplay();
    }
    private void OnEnable()
    {
        HideDisplay();
    }
    void ShowDisplay()
    {

        if (!ClickManager.IsNone()) return;
        if (isFocused) return;
        focusedSprite.enabled = true;
        isFocused = true;

    }
    public void HideDisplay()
    {
        focusedSprite.enabled = false;
        isFocused = false;
    }

}
