using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_OnClick : MonoBehaviour
{
    [SerializeField] Tower parentTower;
    bool isFocused = false;
    [SerializeField] SpriteRenderer spriteRenderer;

    private void OnMouseDown()
    {

        if (parentTower.owner == Owner.KUROI && ClickManager.GetCurrentUser() == MouseUser.ACTIVE_SKILL) {
            EventManager.TriggerEvent(MyEvents.EVENT_CLICK_TOWER, new EventObject(parentTower.gameObject));
            return;
        }
        if (!ClickManager.IsNone()) return;

        EventManager.TriggerEvent(MyEvents.EVENT_CLICK_TOWER, new EventObject(parentTower.gameObject));
        if(parentTower.owner == Owner.NAMCO)  ShowDisplay();
        TutorialManager.CheckTutorial("LearnOptions");
    }


    void ShowDisplay() {
        //사거리, 옵션 활성화
        if (isFocused) return;
     //   MakeCanvasVisible();
        parentTower.targetFinder.SetRangeVisibility(true);
        SetPlateFocus(true);
        isFocused = true;
    }
    public void HideDisplay()
    {
        parentTower.targetFinder.SetRangeVisibility(false);
        SetPlateFocus(false);
        isFocused = false;
    }
    public void SetPlateFocus(bool enabled)
    {
        spriteRenderer.enabled = enabled;
    }
/*    internal void MakeCanvasInvisible()
    {
        EventManager.TriggerEvent(MyEvents.EVENT_UNITOPTION_HIDE_REQUEST, new EventObject(parentTower.gameObject));
    }
    private void MakeCanvasVisible()
    {
        EventManager.TriggerEvent(MyEvents.EVENT_UNITOPTION_SHOW_REQUEST, new EventObject(parentTower.gameObject));
    }*/

}