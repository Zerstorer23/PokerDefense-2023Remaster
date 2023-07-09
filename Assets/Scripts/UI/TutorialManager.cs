using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager prTutorial;
    [SerializeField] Text kotoriLine;
    [SerializeField] Image portraitImage;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] RectTransform tutorialCursor;
    [SerializeField] TutorialObject[] tutorialObjects;
    [SerializeField] BoxCollider2D boxCollider;

    [SerializeField]  private Queue<string> lineQueue;
    private Queue<Vector3> positionQueue;
    private Queue<Vector2> sizeQueue;
    private Queue<Sprite> spriteQueue;
    bool init = false;

   
    float xFactor = 1f;
    float yFactor = 1f;

    public static TutorialManager instance
    {
        get
        {
            if (!prTutorial)
            {
                prTutorial = FindObjectOfType<TutorialManager>();
                if (!prTutorial)
                {
                    Debug.LogWarning("There needs to be one active TutorialManager script on a GameObject in your scene.");
                }

            }

            return prTutorial;
        }
    }
    public static void Init()
    {
        instance.lineQueue = new Queue<string>();
        instance.positionQueue = new Queue<Vector3>();
        instance.sizeQueue = new Queue<Vector2>();
        instance.spriteQueue = new Queue<Sprite>();

        instance.xFactor =  Screen.width / 1920;
        instance.yFactor = Screen.height / 1080;
        //Debug.Log(Screen.currentResolution);
/*        Debug.Log(Screen.width);
        Debug.Log(Screen.height);*/
        instance.init = true;

    }

    private static void TriggerTutorial(string id)
    {
        if (!instance.init) Init();

        TutorialObject tObj = instance.GetObjectByID(id);
        if (tObj == null)
        {

            return;
        }

        bool needToOpenWindow = instance.lineQueue.Count == 0;

        instance.EnqueueTutorial(tObj);
        if (needToOpenWindow)
        {
            instance.SetUpTutorial();
        }
    }
  

    private void EnqueueTutorial(TutorialObject tObj)
    {
        int size = tObj.kotorilines.Length;
        for (int i = 0; i < size; i++) {
            lineQueue.Enqueue(LocalizationManager.Convert(tObj.kotorilines[i]));
            spriteQueue.Enqueue(tObj.portrait);
            if (i < tObj.focusPosition.Length)
            {
                positionQueue.Enqueue(tObj.focusPosition[i]);
            }
            else
            {
                positionQueue.Enqueue(Vector3.zero);
            }
            if (i < tObj.focusSize.Length)
            {
                sizeQueue.Enqueue(tObj.focusSize[i]);
            }
            else
            {
                sizeQueue.Enqueue(Vector3.zero);
            }
        }
        Debug.Assert(lineQueue.Count == sizeQueue.Count, " size mismatch");
        Debug.Assert(lineQueue.Count == positionQueue.Count, " size mismatch");
    }


    private void SetUpTutorial()
    {
            SetVisibility(true);
            DoNext();
    }

    private void SetVisibility(bool v)
    {
        if (tutorialPanel.activeSelf == v) return;
        tutorialPanel.SetActive(v);
        boxCollider.enabled = v;
        tutorialCursor.gameObject.SetActive(false);
 
    }


    private TutorialObject GetObjectByID(string id)
    {
        foreach (TutorialObject t in tutorialObjects)
        {
            if (t.TUTORIAL_ID.Equals(id))
            {
                return t;
            }
        }
        Debug.LogError("No such tutorial");
        return null;
    }
    private void OnMouseDown()
    {
        instance.DoNext();
    }
    public void OnClickSkip()
    {
        instance.SetVisibility(false);
        Init();
    }
    public void OnClickDoNext()
    {
        instance.DoNext();
    }
    public void DoNext()
    {
      //  Debug.Log("Do next cliced");
        if (lineQueue.Count == 0)
        {
            instance.SetVisibility(false);
            return;
        }
        Sprite portrait = spriteQueue.Dequeue();

        if (portrait != null) {
            portraitImage.sprite = portrait;
        }

        string line = lineQueue.Dequeue();
        DOTween.Kill(kotoriLine);
        kotoriLine.text = "";
        kotoriLine.DOText(line, 0.5f);
        Vector3 position = positionQueue.Dequeue();
        Vector3 cursorSize = sizeQueue.Dequeue();
        MoveCursor(position, cursorSize);
    }

    private void MoveCursor(Vector3 position, Vector3 cursorSize)
    {
        Vector3 modPosition = new Vector3(position.x * xFactor, position.y * yFactor);
        Vector3 modCursorSize = new Vector3(cursorSize.x * xFactor, cursorSize.y * yFactor);

        if (modPosition == Vector3.zero)
        {
            tutorialCursor.gameObject.SetActive(false);
            return;
        }
        else
        {
            tutorialCursor.gameObject.SetActive(true);
        }
        tutorialCursor.DOLocalMove(modPosition, 0.5f);
        tutorialCursor.DOSizeDelta(modCursorSize, 0.5f);

      //  tutorialCursor.localPosition = position;
      //  tutorialCursor.sizeDelta = cursorSize;

    }

    public static void CheckTutorial(string key) {
        int savedData = PlayerPrefs.GetInt(key, 0);
        if (savedData != 0) return;
        if (StatisticsManager.GetStat(key) == 0) {
            TriggerTutorial(key);
            PlayerPrefs.SetInt(key, 1);
            StatisticsManager.AddToStat(key, 1);
        }
    
    }
}