using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using static ConstantStrings;

public class SkillButtonBehaviour : MonoBehaviour
{
    float offset = 260f;
    [SerializeField] float delay = 0.5f;
    [SerializeField] int myId = 0;
    [SerializeField] Image backgroundImage;
    
   
    [SerializeField] Text autoText;
    [SerializeField] Text skillNameText;
    [SerializeField] Button skillButton;
    [SerializeField] Text currentStack;
    [SerializeField] Text healAmount;
    [SerializeField] Image loadedImage;

    [SerializeField] bool autoEnabled = false;
    [SerializeField] bool canBeAuto = false;


    private void Awake()
    {
      //  Debug.Log("skillbutton init srtart");
        EventManager.StartListening(MyEvents.EVENT_SHOW_ACTIVESKILL,Show);
        EventManager.StartListening(MyEvents.EVENT_HIDE_ACTIVESKILL, Hide);
        EventManager.StartListening(MyEvents.EVENT_ACTIVESKILL_TOGGLE_POSSIBLE, SetTogglePossible);
      //  Debug.Log("skillbutton init end");

    }


    private void OnDestroy()
    {
        //EventManager.enemyDiedEvent.AddListener(RemoveEnemy);
        EventManager.StopListening(MyEvents.EVENT_SHOW_ACTIVESKILL, Show);
        EventManager.StopListening(MyEvents.EVENT_HIDE_ACTIVESKILL, Hide);
        EventManager.StopListening(MyEvents.EVENT_ACTIVESKILL_TOGGLE_POSSIBLE, SetTogglePossible);
    }
    


    // Start is called before the first frame update
    public void Show(EventObject eo)
    {
        int id = eo.intObj;
        if (id != myId) return;
        if (autoEnabled)
        {
            ShowAuto();
        }
        else
        {
            ShowUnAuto();
        }
    }
    public void Hide(EventObject eo)
    {
        int id = eo.intObj;
        if (id != myId) return;

        GetComponent<RectTransform>().DOLocalMoveX(offset, delay);

    }
    public  void RequestSkillActivation() {
        EventManager.TriggerEvent(MyEvents.EVENT_ACTIVE_SKILL_ACTIVATE_REQUESTED, new EventObject(myId));
    }


    public void OnToggleAuto() {
        if (!canBeAuto)
        {
            RequestSkillActivation();
            return;
        }
        autoEnabled = !autoEnabled;
        if (autoEnabled)
        {
            ShowAuto();

        }
        else
        {
            ShowUnAuto();
        }
        EventManager.TriggerEvent(MyEvents.EVENT_ACTIVE_SKILL_ACTIVATE_TOGGLED, new EventObject(myId) { boolObj = autoEnabled});
        // EventManager.TriggerEvent(MyEvents.EVENT_ACTIVESKILL_AUTO_ON_OFF, new EventObject(enabled) { intObj = myId });

    }
    public void SetTogglePossible(EventObject eo) {
        int index = eo.intObj;
        if (index != myId) return;
   
        autoText.gameObject.SetActive(true);
        canBeAuto = true;
    }
     private void ShowAuto()
    {
        backgroundImage.color = GetColorByHex("#77C877");

        GetComponent<RectTransform>().DOLocalMoveX(180f, delay);
    }
    private void ShowUnAuto() {
        backgroundImage.color = GetColorByHex("#FFFFFF");
        GetComponent<RectTransform>().DOLocalMoveX(0f, delay);
    }

    public void SetFocused(bool focused) {
        if (focused)
        {
            backgroundImage.color = GetColorByHex("#F87777");
        }
        else if(!autoEnabled){

            backgroundImage.color = GetColorByHex("#FFFFFF");
        }
    }

    public bool GetAutoStatus() => autoEnabled;

    internal void SetInformation(string nameKey, int curr, int max, float heal)
    {
        skillNameText.text = LocalizationManager.Convert(nameKey);
        healAmount.text = "<color=#00ff00>+" + heal+"/"+LocalizationManager.Convert("TXT_KEY_TURN") + "</color>";
        currentStack.text= "<color=#ff0000>" + curr  +"</color>";
        loadedImage.fillAmount = curr / (float) max;
    }
}
