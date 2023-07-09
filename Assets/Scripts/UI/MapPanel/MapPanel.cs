using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MapPanel : MonoBehaviour
{
  
    CanvasGroup canvasGroup;
    bool visibility = true;
    internal ScreenType mType = ScreenType.MAP;
    [SerializeField] GameObject drawOpenButton;
    [SerializeField]  MainHudManager mHUDmanager;

    private void Awake()
    {
      //  Debug.Log("mappanel init start");
        canvasGroup = GetComponent<CanvasGroup>();
        EventManager.StartListening(MyEvents.EVENT_GAMESESSION_WAVE_FINISHED, OnWaveFinish);
        SetDrawButtonVisibility(false);
    //    Debug.Log("mappanel init end");
    }
    public void OnClickShowDraw() {
        EventManager.TriggerEvent(MyEvents.EVENT_SHOW_PANEL, new EventObject(ScreenType.DRAW));
        SetDrawButtonVisibility(false);
    }

    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_GAMESESSION_WAVE_FINISHED, OnWaveFinish);

    }
    private void OnWaveFinish(EventObject eo)
    {
        SetDrawButtonVisibility(true);

    }
    private void SetDrawButtonVisibility(bool enable) {
        drawOpenButton.SetActive(enable);
    }

    public void SetCanvasVisibility(bool isVisible)
    {
       // Debug.Log(mType+"  visibility to " + isVisible);
        if (isVisible != visibility) {
            visibility = isVisible;
            if (visibility)
            {
                MakeCanvasVisible();
            }
            else
            {
                MakeCanvasInvisible();
            }
        }
        EventManager.TriggerEvent(MyEvents.EVENT_CANVAS_TRANSITION_FINISHED, null);
    }
    private void MakeCanvasInvisible() {
        canvasGroup.alpha = 0f; //this makes everything transparent
        canvasGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        canvasGroup.interactable = false;
    }
    private void MakeCanvasVisible()
    {
        canvasGroup.alpha = 1f; //this makes everything transparent
        canvasGroup.blocksRaycasts = true; //this prevents the UI element to receive input events
        canvasGroup.interactable = true;

    }
    public bool GetVisibility() => visibility;

    public static Vector2 RectToIso(Vector2 rectangularPos) {
        float newX = rectangularPos.x;
        float newY = rectangularPos.y * 1.125f;
        return new Vector2(newX,newY);
    }

    public static Vector2 IsoToRect(Vector2 IsometricPos)
    {
        float newX = IsometricPos.x;
        float newY = IsometricPos.y * 1.125f;
        return new Vector2(newX, newY);
    }
/*    Vector2 BoardToMap(float x, float y)
    {
        return new Vector2((x * stepSize) + startXoffset, (y * stepSize) + startYoffset);
    }

    int[] MapToBoard(float x, float y)
    {
        int xPos = (int)((x - startXoffset) / stepSize);
        int yPos = (int)((y - startYoffset) / stepSize);
        return new int[2] { xPos, yPos };
    }*/
}
