using static ConstantStrings;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    
    [SerializeField] internal MapPanel mapPanel;
    [SerializeField] DrawPanel drawPanel;
    [SerializeField] LessonPanel lessonPanel;
    [SerializeField] float transitionDelay = 1f;

    bool isInTransition = false;

    ScreenType currentPanel = ScreenType.MAP;


    private void Awake()
    {
      //  Debug.Log("hud init srtart");
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, EventOpenPanel);
        EventManager.StartListening(MyEvents.EVENT_CANVAS_TRANSITION_FINISHED, UnlockTransitionMutex);
     //   Debug.Log("hud init end");
    }
    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, EventOpenPanel);
        EventManager.StopListening(MyEvents.EVENT_CANVAS_TRANSITION_FINISHED, UnlockTransitionMutex);

    }


    void UnlockTransitionMutex(EventObject lexington) {
        isInTransition = false;    
    }

    public void OnClickButton(string typename) {
   
        switch (typename) {
            case STRING_PANEL_MAP:
                EventManager.TriggerEvent(MyEvents.EVENT_SHOW_PANEL, new EventObject(ScreenType.MAP));
                break;
            case STRING_PANEL_LESSON:
                EventManager.TriggerEvent(MyEvents.EVENT_SHOW_PANEL, new EventObject(ScreenType.LESSON));
                break;
            case STRING_PANEL_DRAW:
                EventManager.TriggerEvent(MyEvents.EVENT_SHOW_PANEL, new EventObject(ScreenType.DRAW));
                break;
        }

    }

    private void EventOpenPanel(EventObject v)
    {
        ScreenType screenType = v.screenType;
        Debug.Log("Changing to " + screenType);
        if (isInTransition) return;
        isInTransition = true;
        mapPanel.SetCanvasVisibility(screenType==mapPanel.mType);
        drawPanel.SetCanvasVisibility(screenType == drawPanel.mType, transitionDelay);
        lessonPanel.SetCanvasVisibility(screenType == lessonPanel.mType, transitionDelay);
        currentPanel = screenType;
    }


}

[System.Serializable]
public enum ScreenType { 
    MAP,DRAW,LESSON
}
