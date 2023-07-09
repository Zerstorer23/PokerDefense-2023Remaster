using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour
{
    float stepSize = 0.5f;
    [SerializeField] Text timeText;
    [SerializeField] float totalTimeInSec;
    bool isInWave = false;
    float startedTime=0f;
    public bool cheat = false;
    IEnumerator updateNumerator;

    private void Awake()
    {

    //    Debug.Log("time init srtart");
        EventManager.StartListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, StartTime);
        EventManager.StartListening(MyEvents.EVENT_WAVE_ALL_DEAD, EndTime);
     //   Debug.Log("time init end");
    }
    private void OnDestroy()
    {
        EventManager.StopListening(MyEvents.EVENT_GAMESESSION_WAVE_STARTED, StartTime);
        EventManager.StopListening(MyEvents.EVENT_WAVE_ALL_DEAD, EndTime);

    }

    void StartTime(EventObject lex) {
        isInWave = true;
        startedTime = lex.GetFloat();
        if (cheat) return;
        if (updateNumerator != null) { 
             StopCoroutine(updateNumerator);
        }
        updateNumerator = WaitAndUpdate();
        StartCoroutine(updateNumerator);
    }

   

    IEnumerator WaitAndUpdate( ) {
        while (isInWave) {
            int remain = UpdateDisplay();
            if (remain <= 0) { 
                ProcessTimeOut();
                break;
            }
            yield return new WaitForSeconds(stepSize);
        }
    }
    private void ProcessTimeOut() {
        EventManager.TriggerEvent(MyEvents.EVENT_WAVE_TIMEOUT, null);
        isInWave = false;
        Debug.Log("Time out");
    }
    void EndTime(EventObject lex)
    {
        isInWave = false;
        ClearDisplay();
    }

    int UpdateDisplay() {
        float passedTIme = Time.time - startedTime;
        int remainSeconds = (int)(totalTimeInSec - passedTIme);
        int min = remainSeconds / 60;
        int sec = remainSeconds % 60;
        string time = (min < 10) ? "0" + min : min.ToString();
        time += ":";
        time+= (sec < 10) ? "0" + sec : sec.ToString();
        timeText.text = time;
        return remainSeconds;
    }
    void ClearDisplay() {
        timeText.text = "";
    }
}
