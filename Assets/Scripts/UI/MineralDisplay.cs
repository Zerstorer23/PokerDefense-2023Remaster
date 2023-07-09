using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MineralDisplay : MonoBehaviour
{
    GameSession session;
    [SerializeField] public UpgradeType coinType;
    
    Text display;

    private void Awake()
    {
      //  Debug.Log("mineraldisplay init srtart");
        EventManager.StartListening(MyEvents.EVENT_SHOW_PANEL, UpdateDisplay);
        EventManager.StartListening(MyEvents.EVENT_MINERAL_CHANGED, UpdateDisplay);
        display = GetComponent<Text>();
        if (session == null)
        {
            session = GameSession.GetGameSession();
        }
/*        switch (coinType)
        {
            case UpgradeType.VOCAL:
                display.color = ConstantStrings.GetColorByHex("FF0C0C");
                break;
            case UpgradeType.VISUAL:
                display.color = ConstantStrings.GetColorByHex("FF8800");
                break;
            case UpgradeType.DANCE:
                display.color = ConstantStrings.GetColorByHex("0088FF");
                break;
        }
*/
        //  Debug.Log("mineraldisplay init end");
    }
    private void Start()
    {
        display.text = session.mineralManager.GetResource(coinType).ToString();

    }
    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_SHOW_PANEL, UpdateDisplay);
        EventManager.StopListening(MyEvents.EVENT_MINERAL_CHANGED, UpdateDisplay);
    }


    private void UpdateDisplay(EventObject eo)
    {
        if (display == null) return;
        display.text =session.mineralManager.GetResource(coinType).ToString();
    }
}
