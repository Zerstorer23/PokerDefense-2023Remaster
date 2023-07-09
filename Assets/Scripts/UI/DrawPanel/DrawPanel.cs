using DG.Tweening;
using UnityEngine;
[RequireComponent(typeof(RectTransform))]
[System.Serializable]
public class DrawPanel : MonoBehaviour
{

    [SerializeField] PokerMachine pokerMachine;
    RectTransform rectTransform;
    bool visibility = false;
    internal ScreenType mType = ScreenType.DRAW;

    bool learntoplay_tutorial = false;

    public void SetCanvasVisibility(bool isVisible, float delay)
    {
        //   Debug.Log(mType + "  visibility to " + isVisible);
        if (visibility != isVisible)
        {
            visibility = isVisible;
            if (visibility)
            {
                Show(delay);
            }
            else
            {
                Hide(delay);
            }
        }

    }
    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Show(float delay = 0f)
    {
        gameObject.SetActive(true);
        pokerMachine.SetPokerBoardAvailable(false);
        pokerMachine.PokerAI.SetPokerBoardAvailable(false);
        Vector2 tar = new Vector2(0, 0);


        EventManager.TriggerEvent(MyEvents.EVENT_CANVAS_TRANSITION_FINISHED, null);
        bool doTween = true;
        if (doTween)
        {
            rectTransform.DOLocalMove(tar, delay).OnComplete(
                () =>
                {
                    pokerMachine.DoTurn();
                    EventManager.TriggerEvent(MyEvents.EVENT_CANVAS_TRANSITION_FINISHED, null);
                    if (!learntoplay_tutorial)
                    {
                        TutorialManager.CheckTutorial("LearnToPlay");
                        learntoplay_tutorial = true;

                    }
                }
            );
        }
        else
        {

            rectTransform.localPosition = tar;
            pokerMachine.DoTurn();
        }
    }


    private void Hide(float delay = 0f)
    {
        Vector2 tar = new Vector2(0, -1080);
        rectTransform.DOLocalMove(tar, delay).OnComplete(
            () =>
            {
                gameObject.SetActive(false);
                EventManager.TriggerEvent(MyEvents.EVENT_CANVAS_TRANSITION_FINISHED, null);
            }
        );
    }

    /*
         void Start()
    {
        mySequence = DOTween.Sequence()
        .OnStart(() => {
            transform.localScale = Vector3.zero;
            GetComponent<CanvasGroup>().alpha = 0;
        })
        .Append(transform.DOScale(1, 1).SetEase(Ease.OutBounce))
        .Join(GetComponent<CanvasGroup>().DOFade(1, 1))
        .SetDelay(0.5f);
    }
[출처] [Asset] Unity3D 'DOTween' 2 : Sequence와 팁|작성자 두야
     */
}
