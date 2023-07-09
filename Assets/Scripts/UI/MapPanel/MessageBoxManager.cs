using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxManager : MonoBehaviour
{
    [SerializeField] Text msgBox;
    [SerializeField] GameObject background;

    private void Awake()
    {
   //     Debug.Log("message init srtart");
        EventManager.StartListening(MyEvents.EVENT_MESSAGE_TRIGGERED, OnMessageReceived);
      //  Debug.Log("message init end");
    }
    private void OnDestroy()
    {

        EventManager.StopListening(MyEvents.EVENT_MESSAGE_TRIGGERED, OnMessageReceived);
    }

    private void OnMessageReceived(EventObject eo)
    {
        string msg = eo.GetString();
        float time = eo.GetFloat();
        StartCoroutine(SetMessage(msg,time));
    }

    private IEnumerator SetMessage(string msg, float time)
    {
        msgBox.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
        msgBox.text = msg;
        if (time <= 0) time = 3f;
        yield return new WaitForSeconds(time);
        msgBox.text = "";
        msgBox.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
    }
}
